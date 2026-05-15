using System;
using System.Collections.Generic;

namespace ForumBackend.Ef.Entities;

public partial class InterestingTopic
{
    public long Id { get; set; }

    public Guid User { get; set; }

    public Guid Topic { get; set; }

    public virtual Topic TopicNavigation { get; set; } = null!;

    public virtual User UserNavigation { get; set; } = null!;
}
