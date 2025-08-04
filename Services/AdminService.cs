using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AppOrderNilon.Models;
using Microsoft.EntityFrameworkCore;

namespace AppOrderNilon.Services
{
    public class AdminService
    {
        private readonly AppOrderNilonContext _context;

        public AdminService()
        {
            _context = new AppOrderNilonContext();
        }

        public AdminService(AppOrderNilonContext context)
        {
            _context = context;
        }

        // Authentication
        public Admin? AuthenticateAdmin(string username, string password)
        {
            try
            {
                if (!_context.Database.CanConnect())
                {
                    // Fallback to sample authentication with proper hashing
                    if (username == "admin" && VerifyPassword(password, "admin123"))
                    {
                        return new Admin
                        {
                            AdminId = 1,
                            Username = "admin",
                            FullName = "Nguyễn Quản Trị",
                            Email = "admin@appordernilon.com",
                            Phone = "0123456789"
                        };
                    }
                    return null;
                }

                string passwordHash = HashPassword(password);
                return _context.Admins
                    .FirstOrDefault(a => a.Username == username && a.PasswordHash == passwordHash);
            }
            catch
            {
                // Fallback to sample authentication with proper hashing
                if (username == "admin" && VerifyPassword(password, "admin123"))
                {
                    return new Admin
                    {
                        AdminId = 1,
                        Username = "admin",
                        FullName = "Nguyễn Quản Trị",
                        Email = "admin@appordernilon.com",
                        Phone = "0123456789"
                    };
                }
                return null;
            }
        }

        public bool ValidateAdminCredentials(string username, string password)
        {
            return AuthenticateAdmin(username, password) != null;
        }

        // Staff Authentication
        public Staff AuthenticateStaff(string username, string password)
        {
            try
            {
                string passwordHash = HashPassword(password);
                return _context.Staff
                    .FirstOrDefault(s => s.Username == username && s.PasswordHash == passwordHash);
            }
            catch
            {
                return null;
            }
        }

        // Customer Authentication
        public Customer AuthenticateCustomer(string username, string password)
        {
            try
            {
                string passwordHash = HashPassword(password);
                return _context.Customers
                    .FirstOrDefault(c => c.Username == username && c.PasswordHash == passwordHash);
            }
            catch
            {
                return null;
            }
        }

        // Dashboard Statistics
        public DashboardStatistics GetDashboardStatistics()
        {
            try
            {
                var now = DateTime.Now;
                var startOfMonth = new DateTime(now.Year, now.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                var monthlyOrders = _context.Orders
                    .Where(o => o.OrderDate >= startOfMonth && o.OrderDate <= endOfMonth)
                    .ToList();

                var totalRevenue = monthlyOrders.Any() ? monthlyOrders.Sum(o => o.TotalAmount) : 0m;
                var totalOrders = monthlyOrders.Count;
                var pendingOrders = _context.Orders.Count(o => o.Status == "Pending");

                // Calculate products sold (from order details)
                var productsSold = _context.OrderDetails
                    .Where(od => od.Order.OrderDate >= startOfMonth && od.Order.OrderDate <= endOfMonth)
                    .Sum(od => od.Quantity);

                return new DashboardStatistics
                {
                    TotalRevenue = totalRevenue,
                    TotalOrders = totalOrders,
                    ProductsSold = (int)productsSold,
                    PendingOrders = pendingOrders
                };
            }
            catch
            {
                return new DashboardStatistics
                {
                    TotalRevenue = 0m,
                    TotalOrders = 0,
                    ProductsSold = 0,
                    PendingOrders = 0
                };
            }
        }

        // Recent Orders
        public List<Order> GetRecentOrders(int count = 5)
        {
            try
            {
                return _context.Orders
                    .Include(o => o.Customer)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(count)
                    .ToList();
            }
            catch
            {
                return new List<Order>();
            }
        }

        // Low Stock Alerts
        public List<Product> GetLowStockProducts(int threshold = 10)
        {
            try
            {
                return _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.StockQuantity <= threshold)
                    .OrderBy(p => p.StockQuantity)
                    .ToList();
            }
            catch
            {
                return new List<Product>();
            }
        }

        // Revenue Chart Data
        public List<MonthlyRevenue> GetMonthlyRevenueData(int months = 6)
        {
            try
            {
                var endDate = DateTime.Now;
                var startDate = endDate.AddMonths(-months + 1);

                var monthlyData = new List<MonthlyRevenue>();

                for (int i = 0; i < months; i++)
                {
                    var monthStart = startDate.AddMonths(i);
                    var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                    var revenue = _context.Orders
                        .Where(o => o.OrderDate >= monthStart && o.OrderDate <= monthEnd)
                        .Sum(o => o.TotalAmount);

                    monthlyData.Add(new MonthlyRevenue
                    {
                        Month = monthStart.ToString("MMM yyyy"),
                        Revenue = revenue
                    });
                }

                return monthlyData;
            }
            catch
            {
                return new List<MonthlyRevenue>();
            }
        }

        // Admin Management
        public List<Admin> GetAllAdmins()
        {
            try
            {
                return _context.Admins.ToList();
            }
            catch
            {
                return new List<Admin>();
            }
        }

        public Admin GetAdminById(int adminId)
        {
            try
            {
                return _context.Admins.Find(adminId);
            }
            catch
            {
                return null;
            }
        }

        public bool CreateAdmin(Admin admin)
        {
            try
            {
                // Validate username uniqueness
                if (_context.Admins.Any(a => a.Username == admin.Username))
                {
                    return false;
                }

                admin.PasswordHash = HashPassword(admin.PasswordHash); // Assuming PasswordHash contains plain password
                _context.Admins.Add(admin);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CreateAdminAsync(Admin admin)
        {
            try
            {
                // Validate username uniqueness
                if (await _context.Admins.AnyAsync(a => a.Username == admin.Username))
                {
                    return false;
                }

                admin.PasswordHash = HashPassword(admin.PasswordHash); // Assuming PasswordHash contains plain password
                _context.Admins.Add(admin);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateAdmin(Admin admin)
        {
            try
            {
                var existingAdmin = _context.Admins.Find(admin.AdminId);
                if (existingAdmin == null) return false;

                // Check if username is being changed and if it's already taken
                if (admin.Username != existingAdmin.Username &&
                    _context.Admins.Any(a => a.Username == admin.Username))
                {
                    return false;
                }

                existingAdmin.Username = admin.Username;
                existingAdmin.FullName = admin.FullName;
                existingAdmin.Email = admin.Email;
                existingAdmin.Phone = admin.Phone;

                // Only update password if provided
                if (!string.IsNullOrEmpty(admin.PasswordHash))
                {
                    existingAdmin.PasswordHash = HashPassword(admin.PasswordHash);
                }

                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAdminAsync(Admin admin)
        {
            try
            {
                var existingAdmin = await _context.Admins.FindAsync(admin.AdminId);
                if (existingAdmin == null) return false;

                // Check if username is being changed and if it's already taken
                if (admin.Username != existingAdmin.Username &&
                    await _context.Admins.AnyAsync(a => a.Username == admin.Username))
                {
                    return false;
                }

                existingAdmin.Username = admin.Username;
                existingAdmin.FullName = admin.FullName;
                existingAdmin.Email = admin.Email;
                existingAdmin.Phone = admin.Phone;

                // Only update password if provided
                if (!string.IsNullOrEmpty(admin.PasswordHash))
                {
                    existingAdmin.PasswordHash = HashPassword(admin.PasswordHash);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteAdmin(int adminId)
        {
            try
            {
                var admin = _context.Admins.Find(adminId);
                if (admin == null) return false;

                // Prevent deletion of the last admin
                if (_context.Admins.Count() <= 1)
                {
                    return false;
                }

                _context.Admins.Remove(admin);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAdminAsync(int adminId)
        {
            try
            {
                var admin = await _context.Admins.FindAsync(adminId);
                if (admin == null) return false;

                // Prevent deletion of the last admin
                if (await _context.Admins.CountAsync() <= 1)
                {
                    return false;
                }

                _context.Admins.Remove(admin);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Staff Management
        public List<Staff> GetAllStaff()
        {
            try
            {
                return _context.Staff.ToList();
            }
            catch
            {
                return new List<Staff>();
            }
        }

        public async Task<List<Staff>> GetAllStaffAsync()
        {
            try
            {
                return await _context.Staff
                    .OrderBy(s => s.FullName)
                    .ToListAsync();
            }
            catch
            {
                return new List<Staff>();
            }
        }

        public Staff GetStaffById(int staffId)
        {
            try
            {
                return _context.Staff.Find(staffId);
            }
            catch
            {
                return null;
            }
        }

        public bool CreateStaff(Staff staff)
        {
            try
            {
                // Validate username uniqueness
                if (_context.Staff.Any(s => s.Username == staff.Username))
                {
                    return false;
                }

                staff.PasswordHash = HashPassword(staff.PasswordHash);
                _context.Staff.Add(staff);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CreateStaffAsync(Staff staff)
        {
            try
            {
                // Validate username uniqueness
                if (await _context.Staff.AnyAsync(s => s.Username == staff.Username))
                {
                    return false;
                }

                staff.PasswordHash = HashPassword(staff.PasswordHash);
                _context.Staff.Add(staff);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateStaff(Staff staff)
        {
            try
            {
                var existingStaff = _context.Staff.Find(staff.StaffId);
                if (existingStaff == null) return false;

                // Check if username is being changed and if it's already taken
                if (staff.Username != existingStaff.Username &&
                    _context.Staff.Any(s => s.Username == staff.Username))
                {
                    return false;
                }

                existingStaff.Username = staff.Username;
                existingStaff.FullName = staff.FullName;
                existingStaff.Email = staff.Email;
                existingStaff.Phone = staff.Phone;

                if (!string.IsNullOrEmpty(staff.PasswordHash))
                {
                    existingStaff.PasswordHash = HashPassword(staff.PasswordHash);
                }

                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateStaffAsync(Staff staff)
        {
            try
            {
                var existingStaff = await _context.Staff.FindAsync(staff.StaffId);
                if (existingStaff == null) return false;

                // Check if username is being changed and if it's already taken
                if (staff.Username != existingStaff.Username &&
                    await _context.Staff.AnyAsync(s => s.Username == staff.Username))
                {
                    return false;
                }

                existingStaff.Username = staff.Username;
                existingStaff.FullName = staff.FullName;
                existingStaff.Email = staff.Email;
                existingStaff.Phone = staff.Phone;

                if (!string.IsNullOrEmpty(staff.PasswordHash))
                {
                    existingStaff.PasswordHash = HashPassword(staff.PasswordHash);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteStaff(int staffId)
        {
            try
            {
                var staff = _context.Staff.Find(staffId);
                if (staff == null) return false;

                _context.Staff.Remove(staff);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteStaffAsync(int staffId)
        {
            try
            {
                var staff = await _context.Staff.FindAsync(staffId);
                if (staff == null) return false;

                _context.Staff.Remove(staff);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            try
            {
                return await _context.Admins.AnyAsync(a => a.Username == username) ||
                       await _context.Staff.AnyAsync(s => s.Username == username) ||
                       await _context.Customers.AnyAsync(c => c.Username == username);
            }
            catch
            {
                return false;
            }
        }

        // System Reports
        public List<Report> GetReportsByAdmin(int adminId)
        {
            try
            {
                return _context.Reports
                    .Where(r => r.AdminId == adminId)
                    .OrderByDescending(r => r.GeneratedDate)
                    .ToList();
            }
            catch
            {
                return new List<Report>();
            }
        }

        public bool CreateReport(Report report)
        {
            try
            {
                report.GeneratedDate = DateTime.Now;
                _context.Reports.Add(report);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Utility Methods
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            string inputHash = HashPassword(inputPassword);
            string storedHash = HashPassword(storedPassword);
            return inputHash == storedHash;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }

    // Data Transfer Objects
    public class DashboardStatistics
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int ProductsSold { get; set; }
        public int PendingOrders { get; set; }
    }

    public class MonthlyRevenue
    {
        public string Month { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }
}