using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class delivery
{
    public string id { get; set; } = null!;

    public string order_id { get; set; } = null!;

    public string status { get; set; } = null!;

    public DateTime? delivery_time { get; set; }

    public virtual order order { get; set; } = null!;
}
