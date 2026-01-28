using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class franchise_store
{
    public string id { get; set; } = null!;

    public string name { get; set; } = null!;

    public string address { get; set; } = null!;

    public string status { get; set; } = null!;

    public virtual ICollection<order> orders { get; set; } = new List<order>();
}
