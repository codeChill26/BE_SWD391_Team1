using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class user
{
    public string id { get; set; } = null!;

    public string name { get; set; } = null!;

    public string email { get; set; } = null!;

    public string password_hash { get; set; } = null!;

    public string status { get; set; } = null!;

    public DateTime? created_at { get; set; }

    public virtual ICollection<audit_log> audit_logs { get; set; } = new List<audit_log>();

    public virtual ICollection<role> roles { get; set; } = new List<role>();
}
