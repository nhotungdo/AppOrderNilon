using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppOrderNilon.Models;

namespace AppOrderNilon.Services
{
    public class ProductService
    {
        private readonly AppOrderNilonContext _context;

        public ProductService(AppOrderNilonContext context)
        {
            _context = context;
        }

        // Lấy danh sách tất cả Products
        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .OrderBy(p => p.ProductName)
                .ToListAsync();
        }

        // Lấy Product theo ID
        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        // Tìm kiếm Products
        public async Task<List<Product>> SearchProductsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllProductsAsync();

            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => p.ProductName.Contains(searchTerm) ||
                           (p.Description != null && p.Description.Contains(searchTerm)) ||
                           (p.Category != null && p.Category.CategoryName.Contains(searchTerm)) ||
                           (p.Supplier != null && p.Supplier.SupplierName.Contains(searchTerm)))
                .OrderBy(p => p.ProductName)
                .ToListAsync();
        }

        // Get products by category
        public List<Product> GetProductsByCategory(int categoryId)
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => p.CategoryId == categoryId)
                .OrderBy(p => p.ProductName)
                .ToList();
        }

        // Get products by supplier
        public List<Product> GetProductsBySupplier(int supplierId)
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => p.SupplierId == supplierId)
                .OrderBy(p => p.ProductName)
                .ToList();
        }

        // Get low stock products
        public List<Product> GetLowStockProducts(int threshold = 10)
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => p.StockQuantity <= threshold)
                .OrderBy(p => p.StockQuantity)
                .ToList();
        }

        // Get out of stock products
        public List<Product> GetOutOfStockProducts()
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => p.StockQuantity <= 0)
                .OrderBy(p => p.ProductName)
                .ToList();
        }

        // Tạo mới Product
        public async Task<bool> CreateProductAsync(Product product)
        {
            try
            {
                // Kiểm tra CategoryID và SupplierID hợp lệ
                var category = await _context.Categories.FindAsync(product.CategoryId);
                if (category == null)
                {
                    throw new InvalidOperationException("Danh mục không tồn tại.");
                }

                var supplier = await _context.Suppliers.FindAsync(product.SupplierId);
                if (supplier == null)
                {
                    throw new InvalidOperationException("Nhà cung cấp không tồn tại.");
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Cập nhật Product
        public async Task<bool> UpdateProductAsync(Product product)
        {
            try
            {
                var existingProduct = await _context.Products.FindAsync(product.ProductId);
                if (existingProduct == null)
                {
                    throw new InvalidOperationException("Không tìm thấy sản phẩm.");
                }

                // Kiểm tra CategoryID và SupplierID hợp lệ
                var category = await _context.Categories.FindAsync(product.CategoryId);
                if (category == null)
                {
                    throw new InvalidOperationException("Danh mục không tồn tại.");
                }

                var supplier = await _context.Suppliers.FindAsync(product.SupplierId);
                if (supplier == null)
                {
                    throw new InvalidOperationException("Nhà cung cấp không tồn tại.");
                }

                // Cập nhật thông tin
                existingProduct.ProductName = product.ProductName;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.SupplierId = product.SupplierId;
                existingProduct.Description = product.Description;
                existingProduct.Thickness = product.Thickness;
                existingProduct.Size = product.Size;
                existingProduct.UnitPrice = product.UnitPrice;
                existingProduct.StockQuantity = product.StockQuantity;
                existingProduct.ImagePath = product.ImagePath;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Xóa Product
        public async Task<bool> DeleteProductAsync(int productId)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.OrderDetails)
                    .FirstOrDefaultAsync(p => p.ProductId == productId);

                if (product == null)
                {
                    throw new InvalidOperationException("Không tìm thấy sản phẩm.");
                }

                // Kiểm tra xem Product có liên quan đến đơn hàng không
                if (product.OrderDetails.Any())
                {
                    throw new InvalidOperationException("Không thể xóa sản phẩm vì có đơn hàng liên quan.");
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Update stock quantity
        public bool UpdateStockQuantity(int productId, int newQuantity)
        {
            try
            {
                var product = _context.Products.Find(productId);
                if (product != null)
                {
                    product.StockQuantity = newQuantity;
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Get product statistics
        public ProductStatistics GetProductStatistics()
        {
            try
            {
                return new ProductStatistics
                {
                    TotalProducts = _context.Products.Count(),
                    LowStockProducts = _context.Products.Count(p => p.StockQuantity <= 10),
                    OutOfStockProducts = _context.Products.Count(p => p.StockQuantity <= 0),
                    TotalStockValue = _context.Products.Any() ? _context.Products.Sum(p => p.StockQuantity * p.UnitPrice) : 0m,
                    AveragePrice = _context.Products.Any() ? _context.Products.Average(p => p.UnitPrice) : 0m
                };
            }
            catch
            {
                return new ProductStatistics
                {
                    TotalProducts = 0,
                    LowStockProducts = 0,
                    OutOfStockProducts = 0,
                    TotalStockValue = 0m,
                    AveragePrice = 0m
                };
            }
        }

        // Get top selling products
        public List<ProductSalesSummary> GetTopSellingProducts(int top = 10)
        {
            try
            {
                return _context.OrderDetails
                    .GroupBy(od => od.ProductId)
                    .Select(g => new ProductSalesSummary
                    {
                        ProductId = (int)g.Key,
                        ProductName = g.First().Product.ProductName,
                        TotalQuantity = (int)g.Sum(od => od.Quantity),
                        TotalRevenue = g.Sum(od => od.Subtotal)
                    })
                    .OrderByDescending(p => p.TotalQuantity)
                    .Take(top)
                    .ToList();
            }
            catch
            {
                return new List<ProductSalesSummary>();
            }
        }

        // Get all categories
        public List<Category> GetAllCategories()
        {
            return _context.Categories
                .OrderBy(c => c.CategoryName)
                .ToList();
        }

        // Get all suppliers
        public List<Supplier> GetAllSuppliers()
        {
            return _context.Suppliers
                .OrderBy(s => s.SupplierName)
                .ToList();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }

    public class ProductStatistics
    {
        public int TotalProducts { get; set; }
        public int LowStockProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public decimal TotalStockValue { get; set; }
        public decimal AveragePrice { get; set; }
    }

    public class ProductSalesSummary
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}