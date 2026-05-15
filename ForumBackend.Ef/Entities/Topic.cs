using System;
using System.Collections.Generic;

namespace ForumBackend.Ef.Entities;

public partial class Topic
{
    public int Id { get; set; }

    public Guid Uid { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<InterestingTopic> InterestingTopics { get; set; } = new List<InterestingTopic>();

    public virtual ICollection<PostsTopic> PostsTopics { get; set; } = new List<PostsTopic>();
}
