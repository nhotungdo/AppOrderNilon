using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppOrderNilon.Models;

namespace AppOrderNilon.Services
{
    public class SupplierService
    {
        private readonly AppOrderNilonContext _context;

        public SupplierService(AppOrderNilonContext context)
        {
            _context = context;
        }

        // Lấy danh sách tất cả Suppliers
        public async Task<List<Supplier>> GetAllSuppliersAsync()
        {
            return await _context.Suppliers
                .Include(s => s.Products)
                .OrderBy(s => s.SupplierName)
                .ToListAsync();
        }

        // Lấy Supplier theo ID
        public async Task<Supplier?> GetSupplierByIdAsync(int supplierId)
        {
            return await _context.Suppliers
                .Include(s => s.Products)
                .FirstOrDefaultAsync(s => s.SupplierId == supplierId);
        }

        // Lấy Supplier theo tên
        public async Task<Supplier?> GetSupplierByNameAsync(string supplierName)
        {
            return await _context.Suppliers
                .FirstOrDefaultAsync(s => s.SupplierName == supplierName);
        }

        // Kiểm tra SupplierName đã tồn tại
        public async Task<bool> IsSupplierNameExistsAsync(string supplierName, int? excludeSupplierId = null)
        {
            var query = _context.Suppliers.Where(s => s.SupplierName == supplierName);
            
            if (excludeSupplierId.HasValue)
            {
                query = query.Where(s => s.SupplierId != excludeSupplierId.Value);
            }

            return await query.AnyAsync();
        }

        // Tạo mới Supplier
        public async Task<bool> CreateSupplierAsync(Supplier supplier)
        {
            try
            {
                // Kiểm tra SupplierName đã tồn tại
                if (await IsSupplierNameExistsAsync(supplier.SupplierName))
                {
                    throw new InvalidOperationException("Tên nhà cung cấp đã tồn tại.");
                }

                _context.Suppliers.Add(supplier);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Cập nhật Supplier
        public async Task<bool> UpdateSupplierAsync(Supplier supplier)
        {
            try
            {
                var existingSupplier = await _context.Suppliers.FindAsync(supplier.SupplierId);
                if (existingSupplier == null)
                {
                    throw new InvalidOperationException("Không tìm thấy nhà cung cấp.");
                }

                // Kiểm tra SupplierName đã tồn tại (trừ chính nó)
                if (await IsSupplierNameExistsAsync(supplier.SupplierName, supplier.SupplierId))
                {
                    throw new InvalidOperationException("Tên nhà cung cấp đã tồn tại.");
                }

                // Cập nhật thông tin
                existingSupplier.SupplierName = supplier.SupplierName;
                existingSupplier.ContactName = supplier.ContactName;
                existingSupplier.Phone = supplier.Phone;
                existingSupplier.Email = supplier.Email;
                existingSupplier.Address = supplier.Address;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Xóa Supplier
        public async Task<bool> DeleteSupplierAsync(int supplierId)
        {
            try
            {
                var supplier = await _context.Suppliers
                    .Include(s => s.Products)
                    .FirstOrDefaultAsync(s => s.SupplierId == supplierId);

                if (supplier == null)
                {
                    throw new InvalidOperationException("Không tìm thấy nhà cung cấp.");
                }

                // Kiểm tra xem Supplier có sản phẩm liên quan không
                if (supplier.Products.Any())
                {
                    throw new InvalidOperationException("Không thể xóa nhà cung cấp vì có sản phẩm liên quan.");
                }

                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Lấy danh sách Suppliers với số lượng sản phẩm
        public async Task<List<Supplier>> GetSuppliersWithProductCountAsync()
        {
            return await _context.Suppliers
                .Include(s => s.Products)
                .Select(s => new Supplier
                {
                    SupplierId = s.SupplierId,
                    SupplierName = s.SupplierName,
                    ContactName = s.ContactName,
                    Phone = s.Phone,
                    Email = s.Email,
                    Address = s.Address
                })
                .OrderBy(s => s.SupplierName)
                .ToListAsync();
        }

        // Tìm kiếm Suppliers
        public async Task<List<Supplier>> SearchSuppliersAsync(string searchTerm)
        {
            return await _context.Suppliers
                .Include(s => s.Products)
                .Where(s => s.SupplierName.Contains(searchTerm) || 
                           s.ContactName.Contains(searchTerm) ||
                           s.Email.Contains(searchTerm) ||
                           s.Phone.Contains(searchTerm) ||
                           s.Address.Contains(searchTerm))
                .OrderBy(s => s.SupplierName)
                .ToListAsync();
        }

        // Lấy Suppliers có sản phẩm
        public async Task<List<Supplier>> GetSuppliersWithProductsAsync()
        {
            return await _context.Suppliers
                .Include(s => s.Products)
                .Where(s => s.Products.Any())
                .OrderBy(s => s.SupplierName)
                .ToListAsync();
        }

        // Lấy Suppliers theo email
        public async Task<List<Supplier>> GetSuppliersByEmailAsync(string email)
        {
            return await _context.Suppliers
                .Where(s => s.Email.Contains(email))
                .OrderBy(s => s.SupplierName)
                .ToListAsync();
        }

        // Lấy Suppliers theo phone
        public async Task<List<Supplier>> GetSuppliersByPhoneAsync(string phone)
        {
            return await _context.Suppliers
                .Where(s => s.Phone.Contains(phone))
                .OrderBy(s => s.SupplierName)
                .ToListAsync();
        }
    }
} 