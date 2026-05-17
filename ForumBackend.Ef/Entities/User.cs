using System;
using System.Collections.Generic;

namespace ForumBackend.Ef.Entities;

public partial class User
{
    public long Id { get; set; }

    public Guid Uid { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string HashPassword { get; set; } = null!;

    public string? Description { get; set; }

    public string? AvatarImage { get; set; }

    public DateOnly CreatedAt { get; set; }

    public long Followers { get; set; }

    public string Role { get; set; } = null!;

    public DateOnly RoleGet { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual Role RoleNavigation { get; set; } = null!;

    public virtual ICollection<User> FollowersNavigation { get; set; } = new List<User>();

    public virtual ICollection<User> Follows { get; set; } = new List<User>();

    public virtual ICollection<Post> PostFavorites { get; set; } = new List<Post>();

    public virtual ICollection<Post> PostLikes { get; set; } = new List<Post>();

    public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();
}
