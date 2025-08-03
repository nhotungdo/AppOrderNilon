using Microsoft.EntityFrameworkCore;
using AppOrderNilon.Models;
using System;
using System.IO;
using System.Linq;

namespace AppOrderNilon.Services
{
    public class DatabaseService : IDisposable
    {
        private readonly AppOrderNilonContext _context;

        public DatabaseService()
        {
            _context = new AppOrderNilonContext();
        }

        public bool TestConnection()
        {
            try
            {
                _context.Database.CanConnect();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool InitializeDatabase()
        {
            try
            {
                // Ensure database is created
                _context.Database.EnsureCreated();
                
                // Check if we have any data
                if (!_context.Admins.Any())
                {
                    SeedSampleData();
                }
                
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database initialization failed: {ex.Message}");
                return false;
            }
        }

        private void SeedSampleData()
        {
            try
            {
                // Add sample admin
                var admin = new Admin
                {
                    Username = "admin",
                    PasswordHash = HashPassword("admin123"),
                    FullName = "Nguyễn Quản Trị",
                    Email = "admin@appordernilon.com",
                    Phone = "0123456789"
                };
                _context.Admins.Add(admin);

                // Add sample staff
                var staff = new Staff
                {
                    Username = "staff",
                    PasswordHash = HashPassword("staff123"),
                    FullName = "Nguyễn Văn Nhân Viên",
                    Email = "staff@appordernilon.com",
                    Phone = "0987654321"
                };
                _context.Staff.Add(staff);

                // Add sample customer
                var customer = new Customer
                {
                    Username = "customer",
                    PasswordHash = HashPassword("customer123"),
                    CustomerName = "Công ty Xây dựng Minh Anh",
                    Email = "info@minhanh.com",
                    Phone = "0123456789",
                    Address = "123 Đường ABC, Quận 1, TP.HCM"
                };
                _context.Customers.Add(customer);

                // Add sample categories
                var categories = new[]
                {
                    new Category { CategoryName = "Nilon trong suốt", Description = "Nilon trong suốt các loại" },
                    new Category { CategoryName = "Nilon đen", Description = "Nilon đen các loại" },
                    new Category { CategoryName = "Nilon màu", Description = "Nilon màu các loại" }
                };
                _context.Categories.AddRange(categories);

                // Add sample suppliers
                var suppliers = new[]
                {
                    new Supplier { SupplierName = "Công ty TNHH Nilon ABC", ContactName = "Nguyễn Văn A", Phone = "0123456789" },
                    new Supplier { SupplierName = "Công ty CP Nilon XYZ", ContactName = "Trần Thị B", Phone = "0987654321" }
                };
                _context.Suppliers.AddRange(suppliers);

                _context.SaveChanges();

                // Add sample products
                var products = new[]
                {
                    new Product
                    {
                        ProductName = "Nilon trong suốt 0.05mm",
                        CategoryId = 1,
                        SupplierId = 1,
                        Description = "Nilon trong suốt độ dày 0.05mm",
                        Thickness = 0.05m,
                        Size = "2m x 100m",
                        UnitPrice = 50000m,
                        StockQuantity = 100
                    },
                    new Product
                    {
                        ProductName = "Nilon đen 0.08mm",
                        CategoryId = 2,
                        SupplierId = 1,
                        Description = "Nilon đen độ dày 0.08mm",
                        Thickness = 0.08m,
                        Size = "3m x 100m",
                        UnitPrice = 80000m,
                        StockQuantity = 50
                    }
                };
                _context.Products.AddRange(products);

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Sample data seeding failed: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context?.Dispose();
            }
        }

        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
} 