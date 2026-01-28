using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class inventory
{
    public string id { get; set; } = null!;

    public string product_id { get; set; } = null!;

    public string location_type { get; set; } = null!;

    public string location_id { get; set; } = null!;

    public decimal quantity { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual product product { get; set; } = null!;
}
