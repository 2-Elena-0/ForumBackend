using ForumBackend.Contracts.Comments;
using ForumBackend.Contracts.Posts;
using ForumBackend.Ef;
using ForumBackend.Exceptions.Comment;
using Microsoft.EntityFrameworkCore;

namespace ForumBackend.Services.Comment;

public class CommentService(ForumDbContext dbContext, ILogger<CommentService> logger) : ICommentService
{
    private static string avatarBase = "./avatar/avatarBase.png";
    private CommentResponseContract CreateResponse(Ef.Entities.Comment comment)
    {
        var user = dbContext.Users.SingleOrDefault(x => x.Uid == comment.CreatedBy);
        
        return new CommentResponseContract
        {
            Uid = comment.Uid,
            Body = comment.Body,
            CreatedAt = comment.CreatedAt,
            Likes = comment.Likes,
            PostUId = comment.Post,
            UserUId = comment.CreatedBy,
            UserAvatar = user?.AvatarImage ?? avatarBase,
            UserName = user?.Name ?? "Пользователь не найден",
            WasDeleted = comment.WasDeleted
        };
    }

    public async Task<IReadOnlyCollection<CommentResponseContract>> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all comments");

        var comments = await dbContext.Comments.Select(x => x).ToArrayAsync(cancellationToken);
        
        var res = new List<CommentResponseContract>();
        foreach (var comment in comments)
        {
            var user = dbContext.Users.SingleOrDefault(x => x.Uid == comment.CreatedBy);
            
            res.Add(new CommentResponseContract
            {
                Uid = comment.Uid,
                Body = comment.Body,
                CreatedAt = comment.CreatedAt,
                Likes = comment.Likes,
                PostUId = comment.Post,
                UserUId = comment.CreatedBy,
                UserAvatar = user?.AvatarImage ?? avatarBase,
                UserName = user?.Name ?? "Пользователь не найден",
                WasDeleted = comment.WasDeleted
            });
        }

        logger.LogInformation("Returning all comments. Count: {Count}", comments.Length);

        return res;
    }

    public async Task<IReadOnlyCollection<CommentResponseContract>> GetAllByUserUidAsync(Guid userUid,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all comments by  user uid");

        var comments = await dbContext.Comments.Select(x => x)
            .Where(x => x.CreatedBy == userUid)
            .ToArrayAsync(cancellationToken);

        var res = new List<CommentResponseContract>();
        foreach (var comment in comments)
        {
            var user = dbContext.Users.SingleOrDefault(x => x.Uid == comment.CreatedBy);
            
            res.Add(new CommentResponseContract
            {
                Uid = comment.Uid,
                Body = comment.Body,
                CreatedAt = comment.CreatedAt,
                Likes = comment.Likes,
                PostUId = comment.Post,
                UserUId = comment.CreatedBy,
                UserAvatar = user?.AvatarImage ?? avatarBase,
                UserName = user?.Name ?? "Пользователь не найден",
                WasDeleted = comment.WasDeleted
            });
        }
        
        logger.LogInformation("Returning all user's comments. Count: {Count}", comments.Length);

        return res;
    }

    public async Task<IReadOnlyCollection<CommentResponseContract>> GetAllByPostUidAsync(Guid postUid,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all comments by  post uid");

        var comments = await dbContext.Comments.Select(x => x)
            .Where(x => x.Post == postUid)
            .ToArrayAsync(cancellationToken);

        var res = new List<CommentResponseContract>();
        foreach (var comment in comments)
        {
            var user = dbContext.Users.SingleOrDefault(x => x.Uid == comment.CreatedBy);
            
            res.Add(new CommentResponseContract
            {
                Uid = comment.Uid,
                Body = comment.Body,
                CreatedAt = comment.CreatedAt,
                Likes = comment.Likes,
                PostUId = comment.Post,
                UserUId = comment.CreatedBy,
                UserAvatar = user?.AvatarImage ?? avatarBase,
                UserName = user?.Name ?? "Пользователь не найден",
                WasDeleted = comment.WasDeleted
            });
        }
        logger.LogInformation("Returning all post's comments. Count: {Count}", comments.Length);

        return res;
    }

    public async Task<CommentResponseContract?> GetByUidAsync(Guid uid, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting comment by uid");

        var comment = await dbContext.Comments.Select(x => x)
            .SingleOrDefaultAsync(x => x.Uid == uid, cancellationToken);

        if (comment == null)
        {
            logger.LogWarning("Comment with uid {uid} not found", uid);
            throw new CommentNotFoundException($"Comment with uid {uid} not found");
        }
        
        var res = CreateResponse(comment);

        logger.LogInformation("Returning comment by uid {uid}", comment.Uid);

        return res;
    }

    public async Task<CommentResponseContract> CreateAsync(CreateCommentRequestContract request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Start creating comment by user {userUid}", request.UserUId);

        var comment = new Ef.Entities.Comment
        {
            Body = request.Body,
            CreatedBy = request.UserUId,
            Post = request.PostId,
        };

        await dbContext.Comments.AddAsync(comment, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Comment created with uid {commentUId}", comment.Uid);

        var response = CreateResponse(comment);

        logger.LogInformation("Comment created with uid {commentUId}. Response was created.", comment.Uid);

        return response;
    }

    public async Task<CommentResponseContract?> UpdateAsync(Guid uid, UpdateCommentRequestContract request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Start updating comment by user {userUid}", uid);

        var comment = await dbContext.Comments.SingleOrDefaultAsync(x => x.Uid == uid, cancellationToken);

        if (comment == null)
        {
            logger.LogWarning("Comment with uid {commentUid} not found", uid);
            throw new CommentNotFoundException($"Comment with uid {uid} not found");
        }

        comment.Body = request.Body;
        comment.WasDeleted = request.WasDeleted;

        dbContext.Comments.Update(comment);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Comment updated with uid {commentUId}", comment.Uid);

        var response = CreateResponse(comment);

        logger.LogInformation("Comment updated with uid {commentUId}. Response was created", comment.Uid);

        return response;
    }

    public async Task<bool> DeleteAsync(Guid uid, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting comment deleted.");

        var comment = await dbContext.Comments.SingleOrDefaultAsync(x => x.Uid == uid, cancellationToken);

        if (comment == null)
        {
            logger.LogWarning("Post with uid {uid} not found", uid);
            return false;
        }

        dbContext.Comments.Remove(comment);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Comment and replied comments deleted with uid {commentUId}", comment.Uid);
        return true;
    }
}