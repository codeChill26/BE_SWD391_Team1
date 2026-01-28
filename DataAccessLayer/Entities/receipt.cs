using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class receipt
{
    public string id { get; set; } = null!;

    public string order_id { get; set; } = null!;

    public DateTime? received_at { get; set; }

    public int? quality_rating { get; set; }

    public string? quality_note { get; set; }

    public virtual order order { get; set; } = null!;
}
