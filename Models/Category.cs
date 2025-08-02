using System;
using System.Collections.Generic;

namespace AppOrderNilon.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public int Quantity { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
