using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class role
{
    public string id { get; set; } = null!;

    public string name { get; set; } = null!;

    public virtual ICollection<user> users { get; set; } = new List<user>();
}
