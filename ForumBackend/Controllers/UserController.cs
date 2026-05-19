using ForumBackend.Contracts.User;
using ForumBackend.Filters.Post;
using ForumBackend.Filters.User;
using ForumBackend.Services.User;
using Microsoft.AspNetCore.Mvc;

namespace ForumBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService, ILogger<UserController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<UserResponseContract[]>> GetAll(CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all users.");

        var users = await userService.GetAllAsync(cancellationToken);

        logger.LogInformation("Users received {Count} users.", users.Count);

        return Ok(users);
    }

    [UserExceptionFilter]
    [HttpGet("{uid:guid}")]
    public async Task<ActionResult<UserResponseContract>> GetByUid([FromRoute] Guid uid,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting user with uid: {UserUid}.", uid);

        var user = await userService.GetByUidAsync(uid, cancellationToken);

        if (user is null)
        {
            logger.LogWarning("User with uid {UserUid} was not found.", uid);
            return NotFound();
        }

        logger.LogInformation("User received with uid: {UserUid}.", uid);

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserResponseContract>> Create(
        [FromBody] CreateUserRequestContract request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating user with username {Username}.", request.Name);

        var createdUser = await userService.CreateAsync(request, cancellationToken);

        logger.LogInformation("Created user with username: {Username}. User UId: {UId}", createdUser.Name,
            createdUser.Uid);

        return CreatedAtAction(nameof(GetByUid), new { uid = createdUser.Uid }, createdUser);
    }

    [UserExceptionFilter]
    [HttpPut("{uid:guid}")]
    public async Task<ActionResult<UserResponseContract>> Update(
        [FromRoute] Guid uid,
        [FromBody] UpdateUserRequestContract request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating user with uid: {UserUid}.", uid);

        var updatedUser = await userService.UpdateAsync(uid, request, cancellationToken);

        if (updatedUser is null)
        {
            logger.LogWarning("User with uid {UserUid} was not found for update.", uid);
            return NotFound();
        }

        logger.LogInformation("User with uid: {UserUid} updated.", uid);

        return Ok(updatedUser);
    }

    [UserExceptionFilter]
    [PostExceptionFilter]
    [HttpPut("{uid:guid}/like/{postId:guid}")]
    public async Task<ActionResult<UserResponseContract>> Like(
        [FromRoute] Guid uid, [FromRoute] Guid postId, CancellationToken cancellationToken)
    {
        logger.LogInformation("User with uid {uid} liking post {postUid}.", uid, postId);
        var updatedUser = await userService.AddLikePostAsync(uid, postId, cancellationToken);

        if (updatedUser is null)
        {
            logger.LogWarning("User or post not found");
            return NotFound();
        }
        
        logger.LogInformation("User with uid: {UserUid} updated.", uid);
        return Ok(updatedUser);
    }
    
    [UserExceptionFilter]
    [PostExceptionFilter]
    [HttpPut("{uid:guid}/favorite/{postId:guid}")]
    public async Task<ActionResult<UserResponseContract>> Favorite(
        [FromRoute] Guid uid, [FromRoute] Guid postId, CancellationToken cancellationToken)
    {
        logger.LogInformation("User with uid {uid} saving post {postUid}.", uid, postId);
        var updatedUser = await userService.AddFavoritePostAsync(uid, postId, cancellationToken);

        if (updatedUser is null)
        {
            logger.LogWarning("User or post not found");
            return NotFound();
        }
        
        logger.LogInformation("User with uid: {UserUid} updated.", uid);
        return Ok(updatedUser);
    }
    
    [UserExceptionFilter]
    [PostExceptionFilter]
    [HttpPut("{uid:guid}/follow/{userUid:guid}")]
    public async Task<ActionResult<UserResponseContract>> Follow(
        [FromRoute] Guid uid, [FromRoute] Guid userUid, CancellationToken cancellationToken)
    {
        logger.LogInformation("User with uid {uid} starting follow user {userUid}.", uid, userUid);
        var updatedUser = await userService.AddFollowAsync(uid, userUid, cancellationToken);

        if (updatedUser is null)
        {
            logger.LogWarning("User not found");
            return NotFound();
        }
        
        logger.LogInformation("User with uid: {uid} updated.", uid);
        return Ok(updatedUser);
    }
    
    [UserExceptionFilter]
    [PostExceptionFilter]
    [HttpPut("{uid:guid}/topic/{topicUid:guid}")]
    public async Task<ActionResult<UserResponseContract>> AddTopic(
        [FromRoute] Guid uid, [FromRoute] Guid topicUid, CancellationToken cancellationToken)
    {
        logger.LogInformation("User with uid {uid} adding interesting topic {topicUid}.", uid, topicUid);
        var updatedUser = await userService.AddFollowAsync(uid, topicUid, cancellationToken);

        if (updatedUser is null)
        {
            logger.LogWarning("User or topic not found");
            return NotFound();
        }
        
        logger.LogInformation("User with uid: {uid} updated.", uid);
        return Ok(updatedUser);
    }


    [UserExceptionFilter]
    [HttpDelete("{uid:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid uid, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting user with uid: {UserUid}.", uid);

        var deleted = await userService.DeleteAsync(uid, cancellationToken);

        if (!deleted)
        {
            logger.LogWarning("User with uid {UserUid} was not found for deletion.", uid);
            return NotFound();
        }

        logger.LogInformation("User with uid: {UserUid} deleted.", uid);

        return NoContent();
    }
}