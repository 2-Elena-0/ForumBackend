using ForumBackend.Contracts.Posts;
using ForumBackend.Ef;
using ForumBackend.Exceptions.Topic;
using ForumBackend.Filters.Post;
using Microsoft.EntityFrameworkCore;

namespace ForumBackend.Services.Post;

public class PostService(ForumDbContext dbContext, ILogger<PostService> logger) : IPostService
{
    private PostResponseContract CreateResponse(Ef.Entities.Post post)
    {
        return new PostResponseContract
        {
            Uid = post.Uid,
            UserUId = post.CreatedBy,
            Name = post.Name,
            Body = post.Body,
            CreatedAt = post.CreatedAt,
            Favorites = post.Favorites,
            Likes = post.Likes,
            UserDeleted = post.UserDeleted
        };
    }
    
    public async Task<IReadOnlyCollection<PostResponseContract>> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Start Getting all posts");

        var posts = await dbContext.Posts.Select(x => CreateResponse(x)).ToArrayAsync(cancellationToken);

        logger.LogInformation("Finished getting all users. Posts count: {Count}", posts.Length);

        return posts;
    }

    public async Task<IReadOnlyCollection<PostResponseContract>> GetAllByUserUidAsync(Guid userUid,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Start Getting all posts by user uid: {UserId}", userUid);

        var posts = await dbContext.Posts.Select(x => CreateResponse(x))
            .Where(x => x.UserUId == userUid)
            .ToArrayAsync(cancellationToken);

        logger.LogInformation("Finished getting all post for user. Posts count: {Count}", posts.Length);

        return posts;
    }

    public async Task<PostResponseContract?> GetByUidAsync(Guid uid, CancellationToken cancellationToken)
    {
        logger.LogInformation("Start Getting post by uid: {Uid}", uid);

        var post = await dbContext.Posts.Select(x => CreateResponse(x))
            .Where(x => x.Uid == uid)
            .SingleOrDefaultAsync(cancellationToken);

        if (post == null)
        {
            logger.LogWarning("Post with uId {Uid} not found", uid);
            throw new PostNotFoundException($"Post with uId {uid} not found");
        }

        logger.LogInformation("Finished getting post by uid: {Uid}", post.Uid);
        return post;
    }

    public async Task<PostResponseContract> CreateAsync(CreatePostRequestContract request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Start Creating post: {Post}", request.Name);

        var post = new Ef.Entities.Post
        {
            CreatedBy = request.UserUId,
            Name = request.Name,
            Body = request.Body,
        };

        await dbContext.Posts.AddAsync(post);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Finished Creating post: {Post}", post.Uid);

        var response = CreateResponse(post);

        logger.LogInformation("Finished Creating post: {Post}. Response created.", response.Uid);

        return response;
    }

    public async Task<PostResponseContract?> UpdateAsync(Guid uid, UpdatePostRequestContract request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Start Updating post: {Post}", uid);

        var post = dbContext.Posts.SingleOrDefault(x => x.Uid == uid);

        if (post == null)
        {
            logger.LogWarning("Post with uId {Uid} not found", uid);
            throw new PostNotFoundException($"Post with uId {uid} not found");
        }

        post.Name = request.Name;
        post.Body = request.Body;
        post.UserDeleted = request.UserDeleted;

        dbContext.Update(post);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Finished Updating post: {Post}", post.Uid);

        var response = CreateResponse(post);

        logger.LogInformation("Finished Updating post: {Post}. Response created.", response.Uid);

        return response;
    }

    public async Task<PostResponseContract?> AddTopicToPostAsync(Guid postUid, Guid topicUid, UpdatePostRequestContract request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting add topic {topicUid} to post {postUid}", topicUid, postUid);
        
        var post = dbContext.Posts.SingleOrDefault(x => x.Uid == postUid);
        if (post == null)
        {
            logger.LogWarning("Post with uId {Uid} not found", postUid);
            throw new PostNotFoundException($"Post with uId {postUid} not found");
        }
        
        var topic = dbContext.Topics.SingleOrDefault(x => x.Uid == topicUid);
        if (topic == null)
        {
            logger.LogWarning("Topic with uId {Uid} not found", topicUid);
            throw new TopicNotFoundException($"Topic with uId {topicUid} not found");
        }
        
        post.Topics.Add(topic);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Finished add topic: {Topic}. Response created.", topicUid);
        
        var response = CreateResponse(post);
        
        logger.LogInformation("Finished add topic: {Topic}. Response created.", response.Uid);
        
        return response;
    }

    public async Task<bool> DeleteAsync(Guid uid, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting post deletion. Post uId: {uId}", uid);

        var post = dbContext.Posts.Include(x => x.Topics).Include(x => x.Likes).Include(x => x.Favorites)
            .Include(x => x.Comments).SingleOrDefault(x => x.Uid == uid);

        if (post == null)
        {
            logger.LogWarning("Post with {uId} not found", uid);
            return false;
        }
        
        post.Topics.Clear();
        post.Comments.Clear();
        post.UserFavorites.Clear();
        post.UserLikes.Clear();
        
        dbContext.Comments.RemoveRange(post.Comments);
        
        dbContext.Posts.Remove(post);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Finished Deleting post: {Post}", post.Uid);
        return true;
    }
}