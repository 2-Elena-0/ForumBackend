using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ForumBackend.Contracts.User;
using ForumBackend.Filters.Post;
using ForumBackend.Filters.Topic;
using ForumBackend.Filters.User;
using ForumBackend.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ForumBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService, ILogger<UserController> logger, IConfiguration _configuration) : ControllerBase
{
    [NonAction] 
    public string TokenGenerate(string uid, string email) 
    { 
        Console.WriteLine($"Cofiguration: {_configuration["Jwt:Key"]}"); 
        var claims = new[] 
        { 
            new Claim(ClaimTypes.NameIdentifier, uid), 
            new Claim(ClaimTypes.Email, email), 
        }; 
        var key = new SymmetricSecurityKey( 
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])); 
 
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); 
 
        var token = new JwtSecurityToken( 
            issuer: _configuration["Jwt:Issuer"], 
            audience: _configuration["Jwt:Audience"], 
            claims: claims, 
            expires: DateTime.Now.AddMinutes(60), 
            signingCredentials: creds); 
 
        var resToken = new JwtSecurityTokenHandler().WriteToken(token); 
        return resToken; 
    } 
    
    //[Authorize]
    [HttpGet]
    public async Task<ActionResult<UserResponseContract[]>> GetAll(CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all users.");

        var users = await userService.GetAllAsync(cancellationToken);

        logger.LogInformation("Users received {Count} users.", users.Count);

        return Ok(users);
    }

    //[Authorize]
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
    
    [UserExceptionFilter]
    [HttpPost("login")]
    public async Task<ActionResult> GetByUid([FromBody] UserLoginContract request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Trying login user with email: {User}.", request.Email);

        var user = await userService.LoginByEmailAsync(request, cancellationToken);

        if (user is null)
        {
            logger.LogWarning("User with uid {Email} was not found.", request.Email);
            return NotFound();
        }

        if (user.PwdVerificationResult == PasswordVerificationResult.Failed)
        {
            logger.LogInformation("User with uid {User} failed verification. Incorrect email or password", user.Uid);
            return Unauthorized();
        }

        var token = TokenGenerate(user.Uid.ToString(), user.Email);
        
        logger.LogInformation("User with email {} success verification", user.Email);

        return Ok(new {token = token, uid = user.Uid.ToString()});
    }
    
    [HttpPost("register")]
    public async Task<ActionResult> Register(
        [FromBody] CreateUserRequestContract request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating user with username {Username}.", request.Name);

        var createdUser = await userService.CreateAsync(request, cancellationToken);

        var token = TokenGenerate(createdUser.Uid.ToString(), createdUser.Email);
        
        logger.LogInformation("Created user with username: {Username}. User UId: {UId}", createdUser.Name,
            createdUser.Uid);

        return CreatedAtAction(nameof(GetByUid), new { uid = createdUser.Uid }, new { token = token, uid = createdUser.Uid.ToString()});
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
    [TopicExceptionFilter]
    [HttpPut("{uid:guid}/topic/{topicUid:guid}")]
    public async Task<ActionResult<UserResponseContract>> AddTopic(
        [FromRoute] Guid uid, [FromRoute] Guid topicUid, CancellationToken cancellationToken)
    {
        logger.LogInformation("User with uid {uid} adding interesting topic {topicUid}.", uid, topicUid);
        var updatedUser = await userService.AddInterestingTopic(uid, topicUid, cancellationToken);

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
    
    [HttpGet("checkEmail/{email}")]
    public async Task<IActionResult> CheckEmail([FromRoute] string email, CancellationToken cancellationToken)
    {
        logger.LogInformation("Checking availability user with email: {email}", email);

        var userCheck = await userService.CheckEmail(email, cancellationToken);

        logger.LogInformation("User was found: {}", userCheck);
        return Ok(userCheck);
    }
    
    [HttpGet("checkName/{name}")]
    public async Task<IActionResult> CheckName([FromRoute] string name, CancellationToken cancellationToken)
    {
        logger.LogInformation("Checking availability user with name: {name}", name);

        var userCheck = await userService.CheckName(name, cancellationToken);

        logger.LogInformation("User was found: {}", userCheck);
        return Ok(userCheck);
    }
}