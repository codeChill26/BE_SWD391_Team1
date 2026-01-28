using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class product_category
{
    public string id { get; set; } = null!;

    public string name { get; set; } = null!;

    public virtual ICollection<product> products { get; set; } = new List<product>();
}
