using System;
using System.Collections.Generic;

namespace ForumBackend.Ef.Entities;

public partial class Comment
{
    public long Id { get; set; }

    public Guid Uid { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public string Body { get; set; } = null!;

    public Guid Post { get; set; }

    public long Likes { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Post PostNavigation { get; set; } = null!;
}
