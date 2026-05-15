using System;
using System.Collections.Generic;

namespace ForumBackend.Ef.Entities;

public partial class Favorite
{
    public Guid User { get; set; }

    public Guid Post { get; set; }

    public long Id { get; set; }

    public virtual Post PostNavigation { get; set; } = null!;

    public virtual User UserNavigation { get; set; } = null!;
}
