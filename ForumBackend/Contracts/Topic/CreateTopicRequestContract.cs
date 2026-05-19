using System.ComponentModel.DataAnnotations;

namespace ForumBackend.Contracts.Topic;

public class CreateTopicRequestContract
{
    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Name op topic must be between 3 and 50 characters.")]
    [RegularExpression(@"^[А-Яа-я][А-Яа-я0-9_]{2,49}$", ErrorMessage = "Name op topic must start with a letter and contain only letters, numbers, and underscores.")]
    public required string Name { get; init; }
    
    [Required]
    public string? Description { get; init; }
}