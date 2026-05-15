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

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<Follow> FollowFollow1Navigations { get; set; } = new List<Follow>();

    public virtual ICollection<Follow> FollowUserNavigations { get; set; } = new List<Follow>();

    public virtual ICollection<InterestingTopic> InterestingTopics { get; set; } = new List<InterestingTopic>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual Role RoleNavigation { get; set; } = null!;
}
