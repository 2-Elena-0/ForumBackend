using ForumBackend.Contracts.Role;
using ForumBackend.Ef;
using ForumBackend.Exceptions.Role;
using Microsoft.EntityFrameworkCore;

namespace ForumBackend.Services.Role;

public class RoleService(ForumDbContext dbContext, ILogger<RoleService> logger) : IRoleService
{
    public async Task<RoleResponseContract?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Getting role by name: {name}");
        
        var role = dbContext.Roles.Select(x => new RoleResponseContract
        {
            Name = x.Name,
            CreateComment =  x.CreateComment,
            CreatePost = x.CreatePost,
            DeleteAnyComment = x.DeleteAnyComment,
            DeleteAnyPost = x.DeleteAnyPost,
            EditAnyPost = x.RefactorAnyPost,
        }).SingleOrDefault(x => x.Name == name);

        if (role == null)
        {
            logger.LogWarning("Role not found: {name}", name);
            throw new RoleNotFoundException($"Role not found: {name}");
        }
        
        logger.LogInformation("Returning role: {name}", name);

        return role;
    }

    public async Task<RoleResponseContract> CreateAsync(CreateRoleRequestContract request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Creating role: {request.Name}");

        var role = new Ef.Entities.Role
        {
            Name = request.Name,
            CreateComment = request.CreateComment,
            CreatePost = request.CreatePost,
            DeleteAnyComment = request.DeleteAnyComment,
            DeleteAnyPost = request.DeleteAnyPost,
            RefactorAnyPost = request.EditAnyPost
        };
        
        dbContext.Roles.Add(role);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Role created: {name}", role.Name);

        var response = new RoleResponseContract
        {
            Name = role.Name,
            CreateComment = role.CreateComment,
            CreatePost = role.CreatePost,
            DeleteAnyComment = role.DeleteAnyComment,
            DeleteAnyPost = role.DeleteAnyPost,
            EditAnyPost = role.RefactorAnyPost
        };
        
        logger.LogInformation("Returning role: {name}. Response was created", response.Name);
        
        return response;
    }

    public async Task<bool> DeleteAsync(string name, CancellationToken cancellationToken)
    {
        logger.LogInformation("Start deleting role with name {name}", name);

        if (name == "standard" || name == "admin" || name == "banned")
        {
            logger.LogWarning("Cannot delete base role with name  {name}", name);
            throw new DeleteBaseRoleException($"Cannot delete base role with name  {name}");
        }

        var role = await dbContext.Roles.Include(x => x.Users).SingleOrDefaultAsync(x => x.Name == name, cancellationToken);

        if (role is null)
        {
            logger.LogWarning("Role with name {name} not found", name);
            return false;
        }

        foreach (var user in role.Users)
        {
            user.Role = "standard";
            dbContext.Users.Update(user);
        }

        dbContext.Roles.Remove(role);
        
        logger.LogInformation("Deleting was ended");
        
        return true;
    }
}