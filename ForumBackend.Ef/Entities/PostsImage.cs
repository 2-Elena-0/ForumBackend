using System;
using System.Collections.Generic;

namespace ForumBackend.Ef.Entities;

public partial class PostsImage
{
    public Guid Post { get; set; }

    public string Image { get; set; } = null!;

    public long Id { get; set; }

    public virtual Post PostNavigation { get; set; } = null!;
}
