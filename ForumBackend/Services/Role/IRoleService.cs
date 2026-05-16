using ForumBackend.Contracts.Role;

namespace ForumBackend.Services.Role;

public interface IRoleService
{
    Task<RoleResponseContract?> GetByNameAsync(string name, CancellationToken cancellationToken);

    Task<RoleResponseContract> CreateAsync(CreateRoleRequestContract request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(string name, CancellationToken cancellationToken);
}