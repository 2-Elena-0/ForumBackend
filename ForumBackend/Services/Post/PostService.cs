using ForumBackend.Contracts.Posts;
using ForumBackend.Ef;
using ForumBackend.Ef.Entities;
using ForumBackend.Exceptions.Topic;
using ForumBackend.Filters.Post;
using Microsoft.EntityFrameworkCore;

namespace ForumBackend.Services.Post;

public class PostService(ForumDbContext dbContext, ILogger<PostService> logger) : IPostService
{
    private static PostResponseContract CreateResponse(Ef.Entities.Post post)
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
            UserDeleted = post.UserDeleted,
        };
    }

    private PostResponseWithImagesContract createResponseWithImage(Ef.Entities.Post post)
    {
        logger.LogWarning("images: {}", post.PostsImages);
        var images = new List<string>();

        foreach (var image in post.PostsImages)
        {
            images.Add(image.Image);
        }

        return new PostResponseWithImagesContract
        {
            Uid = post.Uid,
            UserUId = post.CreatedBy,
            Name = post.Name,
            Body = post.Body,
            CreatedAt = post.CreatedAt,
            Favorites = post.Favorites,
            Likes = post.Likes,
            UserDeleted = post.UserDeleted,
            Images = images
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

        var posts = await dbContext.Posts.Select(x => new PostResponseContract
            {
                Uid = x.Uid,
                UserUId = x.CreatedBy,
                Name = x.Name,
                Body = x.Body,
                CreatedAt = x.CreatedAt,
                Favorites = x.Favorites,
                Likes = x.Likes,
                UserDeleted = x.UserDeleted,
            })
            .Where(x => x.UserUId == userUid)
            .ToArrayAsync(cancellationToken);

        logger.LogInformation("Finished getting all post for user. Posts count: {Count}", posts.Length);

        return posts;
    }

    public async Task<PostResponseWithImagesContract?> GetByUidAsync(Guid uid, CancellationToken cancellationToken)
    {
        logger.LogInformation("Start Getting post by uid: {Uid}", uid);

        var post = await dbContext.Posts.Include(x => x.PostsImages).SingleOrDefaultAsync(x => x.Uid == uid, cancellationToken);

        if (post == null)
        {
            logger.LogWarning("Post with uId {Uid} not found", uid);
            throw new PostNotFoundException($"Post with uId {uid} not found");
        }
        
        var response = createResponseWithImage(post);

        logger.LogInformation("Finished getting post by uid: {Uid}", post.Uid);
        return response;
    }

    public async Task<PostResponseWithImagesContract> CreateAsync(CreatePostRequestContract request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Start Creating post: {Post}", request.Name);

        var post = new Ef.Entities.Post
        {
            CreatedBy = request.UserUId,
            Name = request.Name,
            Body = request.Body,
            PostsImages = new List<PostsImage>()
        };

        await dbContext.Posts.AddAsync(post, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        foreach (var image in request.Images)
        {
            var postImage = new PostsImage
            {
                Image = image,
                Post = post.Uid,
            };
            
            await dbContext.PostsImages.AddAsync(postImage, cancellationToken);
        }
        
        dbContext.Update(post);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Finished Creating post: {Post}. Post images: {im}", post.Uid, post.PostsImages.Count);

        var response = createResponseWithImage(post);

        logger.LogInformation("Finished Creating post: {Post}. Response created.", response.Uid);

        return response;
    }

    public async Task<PostResponseWithImagesContract?> UpdateAsync(Guid uid, UpdatePostRequestContract request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Start Updating post: {Post}", uid);

        var post = dbContext.Posts.Include(x => x.PostsImages).SingleOrDefault(x => x.Uid == uid);

        if (post == null)
        {
            logger.LogWarning("Post with uId {Uid} not found", uid);
            throw new PostNotFoundException($"Post with uId {uid} not found");
        }

        post.Name = request.Name;
        post.Body = request.Body;
        post.PostsImages.Clear();

        foreach (var image in request.Images)
        {
            post.PostsImages.Add(new PostsImage
            {
                Post = uid,
                Image = image,
            });
        }

        dbContext.Update(post);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Finished Updating post: {Post}", post.Uid);

        var response = createResponseWithImage(post);

        logger.LogInformation("Finished Updating post: {Post}. Response created.", response.Uid);

        return response;
    }

    public async Task<PostResponseContract?> AddTopicToPostAsync(
        Guid postUid, Guid topicUid, CancellationToken cancellationToken)
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

        var post = dbContext.Posts.Include(x => x.PostsImages).SingleOrDefault(x => x.Uid == uid);

        if (post == null)
        {
            logger.LogWarning("Post with {uId} not found", uid);
            return false;
        }

        post.Topics.Clear();
        post.Comments.Clear();
        post.UserFavorites.Clear();
        post.UserLikes.Clear();
        post.PostsImages.Clear();

        dbContext.PostsImages.RemoveRange(post.PostsImages);
        
        dbContext.Comments.RemoveRange(post.Comments);

        dbContext.Posts.Remove(post);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Finished Deleting post: {Post}", post.Uid);
        return true;
    }
}