using System;
using System.Collections.Generic;

namespace ForumBackend.Ef.Entities;

public partial class Topic
{
    public int Id { get; set; }

    public Guid Uid { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
