using ForumBackend.Contracts.Comments;
using ForumBackend.Filters.Comment;
using ForumBackend.Services.Comment;
using ForumBackend.Services.Post;
using Microsoft.AspNetCore.Mvc;

namespace ForumBackend.Controllers;

[Controller]
[Route("api/[controller]")]
public class CommentController(ICommentService commentService, ILogger<CommentController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CommentResponseContract[]>> GetAll(CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all comments.");

        var comments = await commentService.GetAllAsync(cancellationToken);

        logger.LogInformation("comments received {Count} comments.", comments.Count);

        return Ok(comments);
    }

    [HttpGet]
    [CommentExceptionFilter]
    [Route("api/CommentUser/{uid:guid}")]
    public async Task<ActionResult<CommentResponseContract[]>> GetUserComments([FromRoute] Guid uid,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting comment posts.");

        var comments = await commentService.GetAllByUserUidAsync(uid, cancellationToken);

        logger.LogInformation("comments received {Count} comments.", comments.Count);

        return Ok(comments);
    }

    [HttpGet]
    [CommentExceptionFilter]
    [Route("api/CommentReply/{uid:guid}")]
    public async Task<ActionResult<CommentResponseContract[]>> GetCommentReplies([FromRoute] Guid uid,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting comment posts.");

        var comments = await commentService.GetAllRepliedByUidAsync(uid, cancellationToken);

        logger.LogInformation("comments received {Count} comments.", comments.Count);

        return Ok(comments);
    }

    [CommentExceptionFilter]
    [HttpGet("{uid:guid}")]
    public async Task<ActionResult<CommentResponseContract>> GetByUid([FromRoute] Guid uid,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting comment with uid: {PostUid}.", uid);

        var comment = await commentService.GetByUidAsync(uid, cancellationToken);

        if (comment is null)
        {
            logger.LogWarning("comment with uid {CommentUid} was not found.", uid);
            return NotFound();
        }

        logger.LogInformation("comment received with uid: {CommentUid}.", uid);

        return Ok(comment);
    }

    [HttpPost]
    public async Task<ActionResult<CommentResponseContract>> Create(
        [FromBody] CreateCommentRequestContract request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating comment by uid user {UserUid}.", request.UserUId);

        var createdComment = await commentService.CreateAsync(request, cancellationToken);

        logger.LogInformation("Created comment with user uid: {UserUid}. post UId: {UId}", request.UserUId,
            createdComment.Uid);

        return CreatedAtAction(nameof(GetByUid), new { uid = createdComment.Uid }, createdComment);
    }

    [CommentExceptionFilter]
    [HttpPut("{uid:guid}")]
    public async Task<ActionResult<CommentResponseContract>> Update(
        [FromRoute] Guid uid,
        [FromBody] UpdateCommentRequestContract request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating comment with uid: {CommentUid}.", uid);

        var updatedComment = await commentService.UpdateAsync(uid, request, cancellationToken);

        if (updatedComment is null)
        {
            logger.LogWarning("comment with uid {CommentUid} was not found for update.", uid);
            return NotFound();
        }

        logger.LogInformation("comment with uid: {CommentUid} updated.", uid);

        return Ok(updatedComment);
    }

    [CommentExceptionFilter]
    [HttpDelete("{uid:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid uid, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting comment with uid: {CommentUid}.", uid);

        var deleted = await commentService.DeleteAsync(uid, cancellationToken);

        if (!deleted)
        {
            logger.LogWarning("comment with uid {CommentUid} was not found for deletion.", uid);
            return NotFound();
        }

        logger.LogInformation("comment with uid: {CommentUid} deleted.", uid);

        return NoContent();
    }
}