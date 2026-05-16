namespace ForumBackend.Contracts.Role;

public class RoleResponseContract
{
    public required string Name { get; init; }
    public required bool CreatePost { get; init; }
    public required bool EditAnyPost { get; init; }
    public required bool DeleteAnyPost { get; init; }
    public required bool CreateComment { get; init; }
    public required bool DeleteAnyComment { get; init; }
}