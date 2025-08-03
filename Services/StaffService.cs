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
    public class StaffService
    {
        private readonly AppOrderNilonContext _context;

        public StaffService(AppOrderNilonContext context)
        {
            _context = context;
        }

        // Lấy danh sách tất cả Staff
        public async Task<List<Staff>> GetAllStaffAsync()
        {
            return await _context.Staff
                .OrderBy(s => s.FullName)
                .ToListAsync();
        }

        // Lấy Staff theo ID
        public async Task<Staff?> GetStaffByIdAsync(int staffId)
        {
            return await _context.Staff
                .FirstOrDefaultAsync(s => s.StaffId == staffId);
        }

        // Lấy Staff theo Username
        public async Task<Staff?> GetStaffByUsernameAsync(string username)
        {
            return await _context.Staff
                .FirstOrDefaultAsync(s => s.Username == username);
        }

        // Kiểm tra Username đã tồn tại
        public async Task<bool> IsUsernameExistsAsync(string username, int? excludeStaffId = null)
        {
            var query = _context.Staff.Where(s => s.Username == username);

            if (excludeStaffId.HasValue)
            {
                query = query.Where(s => s.StaffId != excludeStaffId.Value);
            }

            return await query.AnyAsync();
        }

        // Tạo mới Staff
        public async Task<bool> CreateStaffAsync(Staff staff, string password)
        {
            try
            {
                // Kiểm tra Username đã tồn tại
                if (await IsUsernameExistsAsync(staff.Username))
                {
                    throw new InvalidOperationException("Username đã tồn tại.");
                }

                // Mã hóa password
                staff.PasswordHash = HashPassword(password);

                _context.Staff.Add(staff);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Cập nhật Staff
        public async Task<bool> UpdateStaffAsync(Staff staff, string? newPassword = null)
        {
            try
            {
                var existingStaff = await _context.Staff.FindAsync(staff.StaffId);
                if (existingStaff == null)
                {
                    throw new InvalidOperationException("Không tìm thấy Staff.");
                }

                // Kiểm tra Username đã tồn tại (trừ chính nó)
                if (await IsUsernameExistsAsync(staff.Username, staff.StaffId))
                {
                    throw new InvalidOperationException("Username đã tồn tại.");
                }

                // Cập nhật thông tin
                existingStaff.Username = staff.Username;
                existingStaff.FullName = staff.FullName;
                existingStaff.Email = staff.Email;
                existingStaff.Phone = staff.Phone;

                // Cập nhật password nếu có
                if (!string.IsNullOrEmpty(newPassword))
                {
                    existingStaff.PasswordHash = HashPassword(newPassword);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Xóa Staff
        public async Task<bool> DeleteStaffAsync(int staffId)
        {
            try
            {
                var staff = await _context.Staff
                    .Include(s => s.Orders)
                    .FirstOrDefaultAsync(s => s.StaffId == staffId);

                if (staff == null)
                {
                    throw new InvalidOperationException("Không tìm thấy Staff.");
                }

                // Kiểm tra xem Staff có đơn hàng liên quan không
                if (staff.Orders.Any())
                {
                    throw new InvalidOperationException("Không thể xóa Staff vì có đơn hàng liên quan.");
                }

                _context.Staff.Remove(staff);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Xác thực đăng nhập
        public async Task<Staff?> AuthenticateAsync(string username, string password)
        {
            var staff = await _context.Staff
                .FirstOrDefaultAsync(s => s.Username == username);

            if (staff != null && VerifyPassword(password, staff.PasswordHash))
            {
                return staff;
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

        // Tìm kiếm Staff
        public async Task<List<Staff>> SearchStaffAsync(string searchTerm)
        {
            return await _context.Staff
                .Where(s => s.Username.Contains(searchTerm) ||
                           s.FullName.Contains(searchTerm) ||
                           s.Email.Contains(searchTerm) ||
                           s.Phone.Contains(searchTerm))
                .OrderBy(s => s.FullName)
                .ToListAsync();
        }
    }
}