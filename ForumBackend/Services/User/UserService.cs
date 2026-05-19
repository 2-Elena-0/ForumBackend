using ForumBackend.Contracts.User;
using ForumBackend.Ef;
using ForumBackend.Exceptions.Topic;
using ForumBackend.Exceptions.User;
using ForumBackend.Filters.Post;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ForumBackend.Services.User;

public class UserService(ForumDbContext dbContext, ILogger<UserService> logger) : IUserService
{
    private static UserResponseContract CreateResponse(Ef.Entities.User user)
    {
        return new UserResponseContract
        {
            Uid = user.Uid,
            Name = user.Name,
            Email = user.Email,
            AvatarUrl = user.AvatarImage ?? "",
            Description = user.Description ?? "",
            CreatedAt = user.CreatedAt,
            FollowersCount = user.Followers,
            Role = user.Role,
            RoleGet = user.RoleGet
        };
    }
    
    public async Task<IReadOnlyCollection<UserResponseContract>> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting getting all users");
        var users = await dbContext.Users.Select(x => CreateResponse(x))
            .ToArrayAsync(cancellationToken);

        logger.LogInformation("Finished getting all users. Users count: {Count}.", users.Length);

        return users;
    }

    public async Task<UserResponseContract?> GetByUidAsync(Guid uid, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting getting user by uid: {Uid}", uid);
        var user = await dbContext.Users.Select(x => CreateResponse(x))
            .Where(x => x.Uid == uid)
            .SingleOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            logger.LogWarning("User not found: {Uid}", uid);
            throw new UserNotFoundException($"User with uid {uid} not found");
        }

        logger.LogInformation("Finished getting user by uid: {Uid}", uid);

        return user;
    }

    public async Task<UserResponseContract> CreateAsync(CreateUserRequestContract request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting creating user with username: {Username}", request.Name);

        var hasher = new PasswordHasher<string>();
        var hashPwd = hasher.HashPassword(request.Email, request.Password);

        var user = new Ef.Entities.User
        {
            Name = request.Name,
            Email = request.Email,
            HashPassword = hashPwd,
            Role = request.Role,
        };

        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Finished adding user with username: {Username}. User Id: {Id}, User UId: {Uid}",
            user.Name, user.Id, user.Uid);

        var response = CreateResponse(user);

        logger.LogInformation("Finished adding user with username: {Username}. Request is added.", user.Name);

        return response;
    }

    public async Task<UserResponseContract?> UpdateAsync(Guid uid, UpdateUserRequestContract request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting updating user with uid: {Uid}", uid);

        var user = dbContext.Users.SingleOrDefault(x => x.Uid == uid);

        if (user == null)
        {
            logger.LogWarning("User not found: {Uid}", uid);
            throw new UserNotFoundException($"User with uid {uid} not found");
        }

        user.Name = request.Name;
        user.AvatarImage = request.AvatarUrl;
        user.Description = request.Description;
        user.Role = request.Role;
        user.RoleGet = request.RoleDate;

        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Finished updating user with uid: {Uid}", uid);

        var response = CreateResponse(user);

        logger.LogInformation("Finished updating user with uid: {Uid}. Response was added", uid);

        return response;
    }

    public async Task<UserResponseContract?> AddLikePostAsync(
        Guid userUid, Guid postUid, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting add like post with uid {postUid} to user {userUid}", postUid, userUid);

        var user = dbContext.Users.SingleOrDefault(x => x.Uid == userUid);

        if (user == null)
        {
            logger.LogWarning("User not found: {Uid}", userUid);
            throw new UserNotFoundException($"User with uid {userUid} not found");
        }

        var post = dbContext.Posts.SingleOrDefault(x => x.Uid == postUid);

        if (post == null)
        {
            logger.LogWarning("Post not found: {PostUid}", postUid);
            throw new PostNotFoundException($"Post with uid {postUid} not found");
        }

        user.PostLikes.Add(post);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Ending add like post to user");

        var response = CreateResponse(user);

        logger.LogInformation("Finished add like post to user");

        return response;
    }

    public async Task<UserResponseContract?> AddFavoritePostAsync(Guid userUid, 
        Guid postUid, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting add favorite post with uid {postUid} to user {userUid}", postUid, userUid);

        var user = dbContext.Users.SingleOrDefault(x => x.Uid == userUid);

        if (user == null)
        {
            logger.LogWarning("User not found: {Uid}", userUid);
            throw new UserNotFoundException($"User with uid {userUid} not found");
        }

        var post = dbContext.Posts.SingleOrDefault(x => x.Uid == postUid);

        if (post == null)
        {
            logger.LogWarning("Post not found: {PostUid}", postUid);
            throw new PostNotFoundException($"Post with uid {postUid} not found");
        }

        user.PostFavorites.Add(post);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Ending add favorite post to user");

        var response = CreateResponse(user);

        logger.LogInformation("Finished add favorite post to user");

        return response;
    }

    public async Task<UserResponseContract?> AddFollowAsync(
        Guid userFollowerUid, Guid followUid, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting add follow user with uid {followUid} to user {userFollowerUid}", followUid,
            userFollowerUid);

        var user = dbContext.Users.SingleOrDefault(x => x.Uid == userFollowerUid);

        if (user == null)
        {
            logger.LogWarning("User not found: {Uid}", userFollowerUid);
            throw new UserNotFoundException($"User with uid {userFollowerUid} not found");
        }
        
        var userFollow = dbContext.Users.SingleOrDefault(x => x.Uid == followUid);

        if (userFollow == null)
        {
            logger.LogWarning("Follow not found: {FollowUid}", followUid);
            throw new UserNotFoundException($"Follow with uid {followUid} not found");
        }
        
        user.Follows.Add(userFollow);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Ending add follow user to user");
        
        var response = CreateResponse(user);
        
        logger.LogInformation("Finished add follow user to user");
        
        return response;
    }

    public async Task<UserResponseContract?> AddInterestingTopic(
        Guid userUid, Guid topicUid, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting add Interesting topic with uid: {topicUid} to user {userUid}",  topicUid, userUid);
        
        var user = dbContext.Users.SingleOrDefault(x => x.Uid == userUid);
        if (user == null)
        {
            logger.LogWarning("User not found: {Uid}", userUid);
            throw new UserNotFoundException($"User with uid {userUid} not found");
        }
        
        var topic = dbContext.Topics.SingleOrDefault(x => x.Uid == topicUid);
        if (topic == null)
        {
            logger.LogWarning("Topic not found: {TopicUid}", topicUid);
            throw new TopicNotFoundException($"Topic with uid {topicUid} not found");
        }
        
        user.Topics.Add(topic);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Ending add Interesting topic to user");
        
        var response = CreateResponse(user);
        
        logger.LogInformation("Finished add Interesting topic to user");
        
        return response;
    }

    public async Task<bool> DeleteAsync(Guid uid, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting user deletion. UserUid: {UserUid}.", uid);

        var user = await dbContext.Users.Include(x => x.Comments).Include(x => x.Posts)
            .SingleOrDefaultAsync(x => x.Uid == uid, cancellationToken);
        if (user == null)
        {
            logger.LogWarning("User not found: {Uid}", uid);
            return false;
        }

        user.PostLikes.Clear();
        user.PostFavorites.Clear();
        user.Follows.Clear();
        user.Topics.Clear();
        dbContext.Users.Remove(user);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Deleted user {Uid}'s likes, favorites, interesting topics and follows", user.Uid);


        foreach (var comment in user.Comments)
        {
            comment.WasDeleted = true;
            dbContext.Comments.Update(comment);
        }

        foreach (var post in user.Posts)
        {
            post.UserDeleted = true;
            dbContext.Posts.Update(post);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Deleting was ended");
        return true;
    }

    public async Task<bool> CheckName(string name, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting check name. Name: {name}", name);
        
        var user = dbContext.Users.SingleOrDefault(x => x.Name == name);
        
        if (user == null)
        {
            logger.LogInformation("User not found: {Name}", name);
            return false;
        }
        
        return true;
    }

    public async Task<bool> CheckEmail(string email, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting check email. Email: {email}", email);
        
        var user = dbContext.Users.SingleOrDefault(x => x.Email == email);
        
        if (user == null)
        {
            logger.LogInformation("User not found: {email}", email);
            return false;
        }
        
        return true;
    }
}