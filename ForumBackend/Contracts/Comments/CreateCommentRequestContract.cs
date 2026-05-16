using System.ComponentModel.DataAnnotations;

namespace ForumBackend.Contracts.Comments;

public class CreateCommentRequestContract
{
    [Required]
    public required Guid UserUId { get; init; }
    
    [Required]
    public required Guid PostId { get; init; }
    
    [Required]
    public required Guid ReplyId { get; init; }
    
    [Required]
    public required string Body { get; init; }
}