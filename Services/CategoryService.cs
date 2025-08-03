using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppOrderNilon.Models;

namespace AppOrderNilon.Services
{
    public class CategoryService
    {
        private readonly AppOrderNilonContext _context;

        public CategoryService(AppOrderNilonContext context)
        {
            _context = context;
        }

        // Lấy danh sách tất cả Categories
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .Include(c => c.Products)
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        // Lấy Category theo ID
        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            return await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        }

        // Lấy Category theo tên
        public async Task<Category?> GetCategoryByNameAsync(string categoryName)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryName == categoryName);
        }

        // Kiểm tra CategoryName đã tồn tại
        public async Task<bool> IsCategoryNameExistsAsync(string categoryName, int? excludeCategoryId = null)
        {
            var query = _context.Categories.Where(c => c.CategoryName == categoryName);
            
            if (excludeCategoryId.HasValue)
            {
                query = query.Where(c => c.CategoryId != excludeCategoryId.Value);
            }

            return await query.AnyAsync();
        }

        // Tạo mới Category
        public async Task<bool> CreateCategoryAsync(Category category)
        {
            try
            {
                // Kiểm tra CategoryName đã tồn tại
                if (await IsCategoryNameExistsAsync(category.CategoryName))
                {
                    throw new InvalidOperationException("Tên danh mục đã tồn tại.");
                }

                // Quantity mặc định là 0
                category.Quantity = 0;

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Cập nhật Category
        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            try
            {
                var existingCategory = await _context.Categories.FindAsync(category.CategoryId);
                if (existingCategory == null)
                {
                    throw new InvalidOperationException("Không tìm thấy danh mục.");
                }

                // Kiểm tra CategoryName đã tồn tại (trừ chính nó)
                if (await IsCategoryNameExistsAsync(category.CategoryName, category.CategoryId))
                {
                    throw new InvalidOperationException("Tên danh mục đã tồn tại.");
                }

                // Cập nhật thông tin
                existingCategory.CategoryName = category.CategoryName;
                existingCategory.Description = category.Description;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Xóa Category
        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.CategoryId == categoryId);

                if (category == null)
                {
                    throw new InvalidOperationException("Không tìm thấy danh mục.");
                }

                // Kiểm tra xem Category có sản phẩm liên quan không
                if (category.Products.Any())
                {
                    throw new InvalidOperationException("Không thể xóa danh mục vì có sản phẩm liên quan.");
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Lấy danh sách Categories với số lượng sản phẩm
        public async Task<List<Category>> GetCategoriesWithProductCountAsync()
        {
            return await _context.Categories
                .Include(c => c.Products)
                .Select(c => new Category
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Description = c.Description,
                    Quantity = c.Products.Count
                })
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        // Tìm kiếm Categories
        public async Task<List<Category>> SearchCategoriesAsync(string searchTerm)
        {
            return await _context.Categories
                .Include(c => c.Products)
                .Where(c => c.CategoryName.Contains(searchTerm) || 
                           c.Description.Contains(searchTerm))
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        // Lấy Categories có sản phẩm
        public async Task<List<Category>> GetCategoriesWithProductsAsync()
        {
            return await _context.Categories
                .Include(c => c.Products)
                .Where(c => c.Products.Any())
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }
    }
} 