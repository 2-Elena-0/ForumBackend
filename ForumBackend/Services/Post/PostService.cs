using ForumBackend.Contracts.Posts;
using ForumBackend.Ef;
using ForumBackend.Filters.Post;
using Microsoft.EntityFrameworkCore;

namespace ForumBackend.Services.Post;

public class PostService(ForumDbContext dbContext, ILogger<PostService> logger) : IPostService
{
    public async Task<IReadOnlyCollection<PostResponseContract>> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Start Getting all posts");

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
        }).ToArrayAsync(cancellationToken);

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
                UserDeleted = x.UserDeleted
            })
            .Where(x => x.UserUId == userUid)
            .ToArrayAsync(cancellationToken);

        logger.LogInformation("Finished getting all post for user. Posts count: {Count}", posts.Length);

        return posts;
    }

    public async Task<PostResponseContract?> GetByUidAsync(Guid uid, CancellationToken cancellationToken)
    {
        logger.LogInformation("Start Getting post by uid: {Uid}", uid);

        var post = await dbContext.Posts.Select(x => new PostResponseContract
            {
                Uid = x.Uid,
                UserUId = x.CreatedBy,
                Name = x.Name,
                Body = x.Body,
                CreatedAt = x.CreatedAt,
                Favorites = x.Favorites,
                Likes = x.Likes,
                UserDeleted = x.UserDeleted
            })
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

        var response = new PostResponseContract
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

        var response = new PostResponseContract
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

        logger.LogInformation("Finished Updating post: {Post}. Response created.", response.Uid);

        return response;
    }

    public async Task<bool> DeleteAsync(Guid uid, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting post deletion. Post uId: {uId}", uid);

        var post = await dbContext.Posts.Include(x => x.Comments).Include(x => x.FavoritesNavigation)
            .Include(x => x.LikesNavigation).Include(x => x.PostsTopics)
            .SingleOrDefaultAsync(x => x.Uid == uid, cancellationToken);

        if (post == null)
        {
            logger.LogWarning("Post with {uId} not found", uid);
            return false;
        }

        dbContext.Comments.RemoveRange(post.Comments);
        dbContext.Favorites.RemoveRange(post.FavoritesNavigation);
        dbContext.Likes.RemoveRange(post.LikesNavigation);
        dbContext.PostsTopics.RemoveRange(post.PostsTopics);
        dbContext.Posts.Remove(post);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Finished Deleting post: {Post}", post.Uid);
        return true;
    }
}