using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppOrderNilon.Models;

namespace AppOrderNilon.Services
{
    public class CustomerService
    {
        private readonly AppOrderNilonContext _context;

        public CustomerService(AppOrderNilonContext context)
        {
            _context = context;
        }

        // Lấy danh sách tất cả Customers
        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers
                .Include(c => c.Orders)
                .OrderBy(c => c.CustomerName)
                .ToListAsync();
        }

        // Lấy Customer theo ID
        public async Task<Customer?> GetCustomerByIdAsync(int customerId)
        {
            return await _context.Customers
                .Include(c => c.Orders)
                .ThenInclude(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        // Lấy Customer theo Username
        public async Task<Customer?> GetCustomerByUsernameAsync(string username)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Username == username);
        }

        // Kiểm tra Username đã tồn tại
        public async Task<bool> IsUsernameExistsAsync(string username, int? excludeCustomerId = null)
        {
            var query = _context.Customers.Where(c => c.Username == username);

            if (excludeCustomerId.HasValue)
            {
                query = query.Where(c => c.CustomerId != excludeCustomerId.Value);
            }

            return await query.AnyAsync();
        }

        // Tìm kiếm Customers
        public async Task<List<Customer>> SearchCustomersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllCustomersAsync();

            return await _context.Customers
                .Include(c => c.Orders)
                .Where(c => c.CustomerName.Contains(searchTerm) ||
                           c.Phone.Contains(searchTerm) ||
                           c.Email.Contains(searchTerm) ||
                           c.Address.Contains(searchTerm) ||
                           c.Notes.Contains(searchTerm))
                .OrderBy(c => c.CustomerName)
                .ToListAsync();
        }

        // Lấy VIP customers
        public async Task<List<Customer>> GetVIPCustomersAsync()
        {
            return await _context.Customers
                .Include(c => c.Orders)
                .Where(c => c.Notes.Contains("VIP"))
                .OrderBy(c => c.CustomerName)
                .ToListAsync();
        }

        // Lấy customers theo type
        public async Task<List<Customer>> GetCustomersByTypeAsync(string customerType)
        {
            switch (customerType.ToLower())
            {
                case "vip":
                    return await GetVIPCustomersAsync();
                case "regular":
                    return await _context.Customers
                        .Include(c => c.Orders)
                        .Where(c => !c.Notes.Contains("VIP"))
                        .OrderBy(c => c.CustomerName)
                        .ToListAsync();
                default:
                    return await GetAllCustomersAsync();
            }
        }

        // Tạo mới Customer
        public async Task<bool> CreateCustomerAsync(Customer customer, string password)
        {
            try
            {
                // Kiểm tra Username đã tồn tại
                if (await IsUsernameExistsAsync(customer.Username))
                {
                    throw new InvalidOperationException("Username đã tồn tại.");
                }

                // Mã hóa password
                customer.PasswordHash = HashPassword(password);

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Cập nhật Customer
        public async Task<bool> UpdateCustomerAsync(Customer customer, string? newPassword = null)
        {
            try
            {
                var existingCustomer = await _context.Customers.FindAsync(customer.CustomerId);
                if (existingCustomer == null)
                {
                    throw new InvalidOperationException("Không tìm thấy khách hàng.");
                }

                // Kiểm tra Username đã tồn tại (trừ chính nó)
                if (await IsUsernameExistsAsync(customer.Username, customer.CustomerId))
                {
                    throw new InvalidOperationException("Username đã tồn tại.");
                }

                // Cập nhật thông tin
                existingCustomer.Username = customer.Username;
                existingCustomer.CustomerName = customer.CustomerName;
                existingCustomer.Phone = customer.Phone;
                existingCustomer.Email = customer.Email;
                existingCustomer.Address = customer.Address;
                existingCustomer.Notes = customer.Notes;

                // Cập nhật password nếu có
                if (!string.IsNullOrEmpty(newPassword))
                {
                    existingCustomer.PasswordHash = HashPassword(newPassword);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Xóa Customer
        public async Task<bool> DeleteCustomerAsync(int customerId)
        {
            try
            {
                var customer = await _context.Customers
                    .Include(c => c.Orders)
                    .FirstOrDefaultAsync(c => c.CustomerId == customerId);

                if (customer == null)
                {
                    throw new InvalidOperationException("Không tìm thấy khách hàng.");
                }

                // Kiểm tra xem Customer có đơn hàng liên quan không
                if (customer.Orders.Any())
                {
                    throw new InvalidOperationException("Không thể xóa khách hàng vì có đơn hàng liên quan.");
                }

                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Get customer statistics
        public CustomerStatistics GetCustomerStatistics()
        {
            var totalCustomers = _context.Customers.Count();
            var vipCustomers = _context.Customers.Count(c => c.Notes.Contains("VIP"));
            var regularCustomers = totalCustomers - vipCustomers;
            var customersWithOrders = _context.Customers.Count(c => c.Orders.Any());

            return new CustomerStatistics
            {
                TotalCustomers = totalCustomers,
                VIPCustomers = vipCustomers,
                RegularCustomers = regularCustomers,
                CustomersWithOrders = customersWithOrders
            };
        }

        // Get customer order history
        public List<Order> GetCustomerOrderHistory(int customerId)
        {
            return _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        // Get top customers by order value
        public List<CustomerOrderSummary> GetTopCustomersByOrderValue(int top = 10)
        {
            try
            {
                return _context.Customers
                    .Include(c => c.Orders)
                    .Where(c => c.Orders.Any())
                    .Select(c => new CustomerOrderSummary
                    {
                        CustomerId = c.CustomerId,
                        CustomerName = c.CustomerName,
                        TotalOrders = c.Orders.Count,
                        TotalValue = c.Orders.Any() ? c.Orders.Sum(o => o.TotalAmount) : 0m,
                        LastOrderDate = c.Orders.Any() ? c.Orders.Max(o => o.OrderDate) : DateTime.MinValue
                    })
                    .OrderByDescending(c => c.TotalValue)
                    .Take(top)
                    .ToList();
            }
            catch
            {
                return new List<CustomerOrderSummary>();
            }
        }

        // Xác thực đăng nhập
        public async Task<Customer?> AuthenticateAsync(string username, string password)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Username == username);

            if (customer != null && VerifyPassword(password, customer.PasswordHash))
            {
                return customer;
            }

            return null;
        }

        // Mã hóa password
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        // Xác minh password
        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }
    }

    // Data Transfer Objects
    public class CustomerStatistics
    {
        public int TotalCustomers { get; set; }
        public int VIPCustomers { get; set; }
        public int RegularCustomers { get; set; }
        public int CustomersWithOrders { get; set; }
    }

    public class CustomerOrderSummary
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
        public decimal TotalValue { get; set; }
        public DateTime LastOrderDate { get; set; }
    }
}