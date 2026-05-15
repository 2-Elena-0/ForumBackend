using ForumBackend.Contracts.User;
using ForumBackend.Ef;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ForumBackend.Services.User;

public class UserService(ForumDbContext dbContext, ILogger<UserService> logger) : IUserService
{
    public async Task<IReadOnlyCollection<UserResponseContract>> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting getting all users");
        var users = await dbContext.Users.Select(x => new UserResponseContract
            {
                Uid = x.Uid,
                Name = x.Name,
                Email = x.Email,
                Description = x.Description ?? "",
                AvatarUrl = x.AvatarImage ?? "",
                CreatedAt = x.CreatedAt,
                FollowersCount = x.Followers,
                Role = x.Role,
                RoleGet = x.RoleGet
            })
            .ToArrayAsync(cancellationToken);

        logger.LogInformation("Finished getting all users. Users count: {Count}.", users.Length);

        return users;
    }

    public async Task<UserResponseContract?> GetByUidAsync(Guid uid, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting getting user by uid: {Uid}", uid);
        var user = await dbContext.Users.Select(x => new UserResponseContract
            {
                Uid = x.Uid,
                Name = x.Name,
                Email = x.Email,
                Description = x.Description ?? "",
                AvatarUrl = x.AvatarImage ?? "",
                CreatedAt = x.CreatedAt,
                FollowersCount = x.Followers,
                Role = x.Role,
                RoleGet = x.RoleGet
            })
            .Where(x => x.Uid == uid)
            .SingleOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            logger.LogWarning("User not found: {Uid}", uid);
            return null;
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

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Finished adding user with username: {Username}. User Id: {Id}, User UId: {Uid}",
            user.Name, user.Id, user.Uid);

        var response = new UserResponseContract
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
            return null;
        }

        user.Name = request.Name;
        user.AvatarImage = request.AvatarUrl;
        user.Description = request.Description;
        user.Role = request.Role;
        user.RoleGet = request.RoleDate;

        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Finished updating user with uid: {Uid}", uid);

        var response = new UserResponseContract
        {
            Uid = user.Uid,
            Name = user.Name,
            Email = user.Email,
            AvatarUrl = user.AvatarImage,
            Description = user.Description,
            CreatedAt = user.CreatedAt,
            FollowersCount = user.Followers,
            Role = user.Role,
            RoleGet = user.RoleGet
        };

        logger.LogInformation("Finished updating user with uid: {Uid}. Response was added", uid);

        return response;
    }

    public async Task<bool> DeleteAsync(Guid uid, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting user deletion. UserUid: {UserUid}.", uid);

        var user = await dbContext.Users.Include(x => x.Comments).Include(x => x.Posts)
            .Include(x => x.InterestingTopics).Include(x => x.Favorites).Include(x => x.Likes)
            .Include(x => x.FollowUserNavigations).Include(x => x.FollowFollow1Navigations)
            .SingleOrDefaultAsync(x => x.Uid == uid, cancellationToken);

        if (user == null)
        {
            logger.LogWarning("User not found: {Uid}", uid);
            return false;
        }

        dbContext.Likes.RemoveRange(user.Likes);
        dbContext.Favorites.RemoveRange(user.Favorites);
        dbContext.InterestingTopics.RemoveRange(user.InterestingTopics);
        dbContext.Follows.RemoveRange(user.FollowUserNavigations);
        dbContext.Follows.RemoveRange(user.FollowFollow1Navigations);
        dbContext.Users.Remove(user);

        await dbContext.SaveChangesAsync(cancellationToken); 
        
        logger.LogInformation("Deleted user {Uid}'s likes, favorites, interesting topics and follows", user.Uid);

        
        foreach (var comment in user.Comments)
        {
            comment.UserDeleted = true;
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
}