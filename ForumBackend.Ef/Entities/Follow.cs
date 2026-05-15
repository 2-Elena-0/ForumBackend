using System;
using System.Collections.Generic;

namespace ForumBackend.Ef.Entities;

public partial class Follow
{
    public long Id { get; set; }

    public Guid User { get; set; }

    public Guid Follow1 { get; set; }

    public virtual User Follow1Navigation { get; set; } = null!;

    public virtual User UserNavigation { get; set; } = null!;
}
