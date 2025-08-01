﻿using System;
using System.Collections.Generic;

namespace AppOrderNilon.Models;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int? OrderId { get; set; }

    public int? ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Subtotal { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }

    // Navigation properties for display
    public string ProductName => Product?.ProductName ?? "Unknown";
}
