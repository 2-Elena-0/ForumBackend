using System;
using System.Collections.Generic;

namespace ForumBackend.Ef.Entities;

public partial class Role
{
    public string Name { get; set; } = null!;

    public bool CreatePost { get; set; }

    public bool RefactorAnyPost { get; set; }

    public bool DeleteAnyPost { get; set; }

    public bool CreateComment { get; set; }

    public bool DeleteAnyComment { get; set; }

    public int? ValidDays { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
