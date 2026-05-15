using System;
using System.Collections.Generic;

namespace ForumBackend.Ef.Entities;

public partial class PostsTopic
{
    public long Id { get; set; }

    public Guid Post { get; set; }

    public Guid Topic { get; set; }

    public virtual Post PostNavigation { get; set; } = null!;

    public virtual Topic TopicNavigation { get; set; } = null!;
}
