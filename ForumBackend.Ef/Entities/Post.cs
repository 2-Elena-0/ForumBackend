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

    public virtual ICollection<PostsImage> PostsImages { get; set; } = new List<PostsImage>();

    public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();

    public virtual ICollection<User> UserFavorites { get; set; } = new List<User>();

    public virtual ICollection<User> UserLikes { get; set; } = new List<User>();
}
