using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class product
{
    public string id { get; set; } = null!;

    public string name { get; set; } = null!;

    public string category_id { get; set; } = null!;

    public string type { get; set; } = null!;

    public string unit { get; set; } = null!;

    public virtual product_category category { get; set; } = null!;

    public virtual ICollection<inventory> inventories { get; set; } = new List<inventory>();

    public virtual ICollection<order_item> order_items { get; set; } = new List<order_item>();
}
