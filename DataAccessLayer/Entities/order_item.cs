using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class order_item
{
    public string order_id { get; set; } = null!;

    public string product_id { get; set; } = null!;

    public decimal quantity { get; set; }

    public virtual order order { get; set; } = null!;

    public virtual product product { get; set; } = null!;
}
