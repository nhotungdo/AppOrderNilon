using System;
using System.Collections.Generic;

namespace AppOrderNilon.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? CustomerId { get; set; }

    public int? StaffId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = null!;
    public string CustomerName { get; set; } = null!;


    public string? Notes { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Staff? Staff { get; set; }

    // Navigation properties for display

    public string StaffName => Staff?.FullName ?? "Unknown";
}
