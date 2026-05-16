using System.ComponentModel.DataAnnotations;

namespace ForumBackend.Contracts.Comments;

public class UpdateCommentRequestContract
{
    [Required] public string Body { get; set; }
    [Required] public bool WasDeleted { get; set; }
}