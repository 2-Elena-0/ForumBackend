using ForumBackend.Contracts.Comments;
using ForumBackend.Ef;
using ForumBackend.Exceptions.Comment;
using Microsoft.EntityFrameworkCore;

namespace ForumBackend.Services.Comment;

public class CommentService(ForumDbContext dbContext, ILogger<CommentService> logger) : ICommentService
{
    public async Task<IReadOnlyCollection<CommentResponseContract>> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all comments");

        var comments = await dbContext.Comments.Select(x => new CommentResponseContract
        {
            Body = x.Body,
            CreatedAt = x.CreatedAt,
            Likes = x.Likes,
            PostUId = x.Post,
            Uid = x.Uid,
            ReplyUId = x.ToComment,
            UserUId = x.CreatedBy,
            WasDeleted = x.WasDeleted
        }).ToArrayAsync(cancellationToken);

        logger.LogInformation("Returning all comments. Count: {Count}", comments.Length);

        return comments;
    }

    public async Task<IReadOnlyCollection<CommentResponseContract>> GetAllByUserUidAsync(Guid userUid,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all comments by  user uid");

        var comments = await dbContext.Comments.Select(x => new CommentResponseContract
            {
                Body = x.Body,
                CreatedAt = x.CreatedAt,
                Likes = x.Likes,
                PostUId = x.Post,
                Uid = x.Uid,
                ReplyUId = x.ToComment,
                UserUId = x.CreatedBy,
                WasDeleted = x.WasDeleted
            })
            .Where(x => x.UserUId == userUid)
            .ToArrayAsync(cancellationToken);

        logger.LogInformation("Returning all user's comments. Count: {Count}", comments.Length);

        return comments;
    }

    public async Task<IReadOnlyCollection<CommentResponseContract>> GetAllRepliedByUidAsync(Guid uid,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all replied comments");

        var comments = await dbContext.Comments.Select(x => new CommentResponseContract
            {
                Body = x.Body,
                CreatedAt = x.CreatedAt,
                Likes = x.Likes,
                PostUId = x.Post,
                Uid = x.Uid,
                ReplyUId = x.ToComment,
                UserUId = x.CreatedBy,
                WasDeleted = x.WasDeleted
            })
            .Where(x => x.ReplyUId == uid)
            .ToArrayAsync(cancellationToken);

        logger.LogInformation("Returning all replied comments. Count: {Count}", comments.Length);

        return comments;
    }

    public async Task<CommentResponseContract?> GetByUidAsync(Guid uid, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting comment by uid");

        var comment = await dbContext.Comments.Select(x => new CommentResponseContract
        {
            Body = x.Body,
            CreatedAt = x.CreatedAt,
            Likes = x.Likes,
            PostUId = x.Post,
            Uid = x.Uid,
            ReplyUId = x.ToComment,
            UserUId = x.CreatedBy,
            WasDeleted = x.WasDeleted
        }).SingleOrDefaultAsync(x => x.Uid == uid);

        if (comment == null)
        {
            logger.LogWarning("Comment with uid {uid} not found", uid);
            throw new CommentNotFoundException($"Comment with uid {uid} not found");
        }

        logger.LogInformation("Returning comment by uid {uid}", comment.Uid);

        return comment;
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
            ToComment = request.ReplyId
        };

        await dbContext.Comments.AddAsync(comment, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Comment created with uid {commentUId}", comment.Uid);

        var response = new CommentResponseContract
        {
            Uid = comment.Uid,
            Body = comment.Body,
            CreatedAt = comment.CreatedAt,
            Likes = comment.Likes,
            PostUId = comment.Post,
            UserUId = comment.CreatedBy,
            ReplyUId = comment.ToComment,
            WasDeleted = comment.WasDeleted
        };

        logger.LogInformation("Comment created with uid {commentUId}. Response was created.", comment.Uid);

        return response;
    }

    public async Task<CommentResponseContract?> UpdateAsync(Guid uid, UpdateCommentRequestContract request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Start updating comment by user {userUid}", uid);

        var comment = await dbContext.Comments.SingleOrDefaultAsync(x => x.Uid == uid);

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

        var response = new CommentResponseContract
        {
            Uid = comment.Uid,
            Body = comment.Body,
            CreatedAt = comment.CreatedAt,
            Likes = comment.Likes,
            PostUId = comment.Post,
            UserUId = comment.CreatedBy,
            ReplyUId = comment.ToComment,
            WasDeleted = comment.WasDeleted
        };

        logger.LogInformation("Comment updated with uid {commentUId}. Response was created", comment.Uid);

        return response;
    }

    public async Task<bool> DeleteAsync(Guid uid, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting comment deleted.");

        var comment = await dbContext.Comments.Include(x => x.ToCommentNavigation)
            .SingleOrDefaultAsync(x => x.Uid == uid, cancellationToken);

        if (comment == null)
        {
            logger.LogWarning("Post with uid {uid} not found", uid);
            return false;
        }

        if (comment.ToCommentNavigation != null) dbContext.Comments.RemoveRange(comment.ToCommentNavigation);
        dbContext.Comments.Remove(comment);

        await dbContext.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Comment and replied comments deleted with uid {commentUId}", comment.Uid);
        return true;
    }
}