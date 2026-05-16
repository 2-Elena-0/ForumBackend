using System.ComponentModel.DataAnnotations;

namespace ForumBackend.Contracts.Role;

public class CreateRoleRequestContract
{
    [Required] public required string Name { get; init; }
    [Required] public required bool CreatePost { get; init; }
    [Required] public required bool EditAnyPost { get; init; }
    [Required] public required bool DeleteAnyPost { get; init; }
    [Required] public required bool CreateComment { get; init; }
    [Required] public required bool DeleteAnyComment { get; init; }
}