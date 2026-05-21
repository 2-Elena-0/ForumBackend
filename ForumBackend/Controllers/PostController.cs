using ForumBackend.Contracts.Posts;
using ForumBackend.Ef.Entities;
using ForumBackend.Filters.Post;
using ForumBackend.Filters.Topic;
using ForumBackend.Services.Post;
using Microsoft.AspNetCore.Mvc;

namespace ForumBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController(IPostService postService, ILogger<PostController> logger) : ControllerBase
{
    // [NonAction]
    // private static PostResponseWithImagesContract CreatePostResponseWithAvatarContract(PostResponseContract post,
    //     string avatar)
    // {
    //     return new PostResponseWithImagesContract
    //     {
    //         Uid = post.Uid,
    //         Name = post.Name,
    //         Body = post.Body,
    //         CreatedAt = post.CreatedAt,
    //         Favorites = post.Favorites,
    //         Likes =  post.Likes,
    //         UserUId = post.UserUId,
    //         UserDeleted = post.UserDeleted,
    //     };
    // }
    
    [HttpGet]
    public async Task<ActionResult<PostResponseContract[]>> GetAll(CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all posts.");

        var posts = await postService.GetAllAsync(cancellationToken);

        logger.LogInformation("Posts received {Count} posts.", posts.Count);

        return Ok(posts);
    }
    
    [HttpGet]
    [PostExceptionFilter]
    [Route("{uid:guid}")]
    public async Task<ActionResult<PostResponseContract[]>> GetUserPosts([FromRoute] Guid uid, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting user posts.");

        var posts = await postService.GetAllByUserUidAsync(uid, cancellationToken);

        logger.LogInformation("Posts received {Count} posts.", posts.Count);

        return Ok(posts);
    }

    [PostExceptionFilter]
    [HttpGet("byUid/{uid:guid}")]
    public async Task<ActionResult<PostResponseWithImagesContract>> GetByUid([FromRoute] Guid uid,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting post with uid: {PostUid}.", uid);

        var post = await postService.GetByUidAsync(uid, cancellationToken);

        if (post is null)
        {
            logger.LogWarning("post with uid {PostUid} was not found.", uid);
            return NotFound();
        }

        logger.LogInformation("post received with uid: {PostUid}.", uid);

        return Ok(post);
    }

    [HttpPost]
    public async Task<ActionResult<PostResponseContract>> Create(
        [FromBody] CreatePostRequestContract request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating post with postname {PostName}.", request.Name);

        var createdPost = await postService.CreateAsync(request, cancellationToken);

        logger.LogInformation("Created post with postname: {PostName}. post UId: {UId}", createdPost.Name,
            createdPost.Uid);

        return CreatedAtAction(nameof(GetByUid), new { uid = createdPost.Uid }, createdPost);
    }

    [PostExceptionFilter]
    [HttpPut("{uid:guid}")]
    public async Task<ActionResult<PostResponseContract>> Update(
        [FromRoute] Guid uid,
        [FromBody] UpdatePostRequestContract request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating post with uid: {PostUid}.", uid);

        var updatedPost = await postService.UpdateAsync(uid, request, cancellationToken);

        if (updatedPost is null)
        {
            logger.LogWarning("post with uid {PostUid} was not found for update.", uid);
            return NotFound();
        }

        logger.LogInformation("post with uid: {PostUid} updated.", uid);

        return Ok(updatedPost);
    }
    
    [PostExceptionFilter]
    [TopicExceptionFilter]
    [HttpPut("{uid:guid}/topic/{topicUid:guid}")]
    public async Task<ActionResult<PostResponseContract>> AddTopic(
        [FromRoute] Guid uid,
        [FromRoute] Guid topicUid,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Adding topic {topicUid} to post with uid: {PostUid}.", topicUid, uid);

        var updatedPost = await postService.AddTopicToPostAsync(uid, topicUid, cancellationToken);

        if (updatedPost is null)
        {
            logger.LogWarning("post or topic was not found");
            return NotFound();
        }

        logger.LogInformation("post with uid: {PostUid} updated.", uid);

        return Ok(updatedPost);
    }

    [PostExceptionFilter]
    [HttpDelete("{uid:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid uid, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting post with uid: {PostUid}.", uid);

        var deleted = await postService.DeleteAsync(uid, cancellationToken);

        if (!deleted)
        {
            logger.LogWarning("post with uid {PostUid} was not found for deletion.", uid);
            return NotFound();
        }

        logger.LogInformation("post with uid: {PostUid} deleted.", uid);

        return NoContent();
    }
}