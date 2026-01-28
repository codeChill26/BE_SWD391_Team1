using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class order
{
    public string id { get; set; } = null!;

    public string? store_id { get; set; }

    public string? kitchen_id { get; set; }

    public string status { get; set; } = null!;

    public DateTime? expected_delivery_at { get; set; }

    public DateTime? created_at { get; set; }

    public virtual ICollection<delivery> deliveries { get; set; } = new List<delivery>();

    public virtual central_kitchen? kitchen { get; set; }

    public virtual ICollection<order_item> order_items { get; set; } = new List<order_item>();

    public virtual receipt? receipt { get; set; }

    public virtual franchise_store? store { get; set; }
}
