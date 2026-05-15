using System;
using System.Collections.Generic;

namespace ForumBackend.Ef.Entities;

public partial class Like
{
    public long Id { get; set; }

    public Guid User { get; set; }

    public Guid Post { get; set; }

    public virtual User User1 { get; set; } = null!;

    public virtual Post UserNavigation { get; set; } = null!;
}
