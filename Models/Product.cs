using System;
using System.Collections.Generic;

namespace AppOrderNilon.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public int? CategoryId { get; set; }

    public int? SupplierId { get; set; }

    public string? Description { get; set; }

    public decimal? Thickness { get; set; }

    public string? Size { get; set; }

    public decimal UnitPrice { get; set; }

    public int StockQuantity { get; set; }

    public string? ImagePath { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Supplier? Supplier { get; set; }

    // Navigation properties for display
    public string CategoryName => Category?.CategoryName ?? "Unknown";
    public string SupplierName => Supplier?.SupplierName ?? "Unknown";
    
    // Computed property for stock status
    public string StockStatus 
    { 
        get 
        {
            if (StockQuantity <= 10)
                return "Sắp hết";
            else if (StockQuantity <= 50)
                return "Trung bình";
            else
                return "Đủ";
        }
    }
}
