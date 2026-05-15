using System;
using System.Collections.Generic;

namespace ForumBackend.Ef.Entities;

public partial class Post
{
    public long Id { get; set; }

    public Guid Uid { get; set; }

    public string Name { get; set; } = null!;

    public string Body { get; set; } = null!;

    public long Likes { get; set; }

    public long Favorites { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public bool UserDeleted { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<Favorite> FavoritesNavigation { get; set; } = new List<Favorite>();

    public virtual ICollection<Like> LikesNavigation { get; set; } = new List<Like>();

    public virtual ICollection<PostsTopic> PostsTopics { get; set; } = new List<PostsTopic>();
}
