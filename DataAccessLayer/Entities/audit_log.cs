using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class audit_log
{
    public string id { get; set; } = null!;

    public string entity_type { get; set; } = null!;

    public string entity_id { get; set; } = null!;

    public string? user_id { get; set; }

    public string action { get; set; } = null!;

    public string? old_value { get; set; }

    public string? new_value { get; set; }

    public DateTime? created_at { get; set; }

    public virtual user? user { get; set; }
}
