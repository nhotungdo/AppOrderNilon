using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppOrderNilon.Models;

namespace AppOrderNilon.Services
{
    public class ReportService
    {
        private readonly AppOrderNilonContext _context;

        public ReportService(AppOrderNilonContext context)
        {
            _context = context;
        }

        // Lấy danh sách tất cả Reports
        public async Task<List<Report>> GetAllReportsAsync()
        {
            return await _context.Reports
                .Include(r => r.Admin)
                .OrderByDescending(r => r.GeneratedDate)
                .ToListAsync();
        }

        // Lấy Report theo ID
        public async Task<Report?> GetReportByIdAsync(int reportId)
        {
            return await _context.Reports
                .Include(r => r.Admin)
                .FirstOrDefaultAsync(r => r.ReportId == reportId);
        }

        // Tạo mới Report
        public async Task<bool> CreateReportAsync(Report report)
        {
            try
            {
                report.GeneratedDate = DateTime.Now;
                _context.Reports.Add(report);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Cập nhật Report
        public async Task<bool> UpdateReportAsync(Report report)
        {
            try
            {
                var existingReport = await _context.Reports.FindAsync(report.ReportId);
                if (existingReport == null)
                {
                    throw new InvalidOperationException("Không tìm thấy báo cáo.");
                }

                existingReport.ReportType = report.ReportType;
                existingReport.StartDate = report.StartDate;
                existingReport.EndDate = report.EndDate;
                existingReport.Data = report.Data;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Xóa Report
        public async Task<bool> DeleteReportAsync(int reportId)
        {
            try
            {
                var report = await _context.Reports.FindAsync(reportId);
                if (report == null)
                {
                    throw new InvalidOperationException("Không tìm thấy báo cáo.");
                }

                _context.Reports.Remove(report);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Tạo báo cáo doanh thu
        public async Task<Report> GenerateRevenueReportAsync(DateTime startDate, DateTime endDate, int adminId)
        {
            var revenueData = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalRevenue = g.Sum(o => o.TotalAmount),
                    OrderCount = g.Count(),
                    AverageOrderValue = g.Average(o => o.TotalAmount)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync();

            var report = new Report
            {
                ReportType = "Revenue",
                StartDate = startDate,
                EndDate = endDate,
                AdminId = adminId,
                GeneratedDate = DateTime.Now,
                Data = JsonSerializer.Serialize(revenueData)
            };

            await CreateReportAsync(report);
            return report;
        }

        // Tạo báo cáo tồn kho
        public async Task<Report> GenerateInventoryReportAsync(int adminId)
        {
            var inventoryData = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Select(p => new
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    CategoryName = p.Category.CategoryName,
                    SupplierName = p.Supplier.SupplierName,
                    StockQuantity = p.StockQuantity,
                    UnitPrice = p.UnitPrice,
                    TotalValue = p.StockQuantity * p.UnitPrice
                })
                .OrderBy(x => x.CategoryName)
                .ThenBy(x => x.ProductName)
                .ToListAsync();

            var report = new Report
            {
                ReportType = "Inventory",
                StartDate = DateTime.Now.Date,
                EndDate = DateTime.Now.Date,
                AdminId = adminId,
                GeneratedDate = DateTime.Now,
                Data = JsonSerializer.Serialize(inventoryData)
            };

            await CreateReportAsync(report);
            return report;
        }

        // Tạo báo cáo sản phẩm bán chạy
        public async Task<Report> GenerateBestSellingReportAsync(DateTime startDate, DateTime endDate, int adminId)
        {
            var bestSellingData = await _context.OrderDetails
                .Include(od => od.Product)
                .Include(od => od.Order)
                .Where(od => od.Order.OrderDate >= startDate && od.Order.OrderDate <= endDate)
                .GroupBy(od => new { od.ProductId, od.Product.ProductName })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    TotalQuantity = g.Sum(od => od.Quantity),
                    TotalRevenue = g.Sum(od => od.Subtotal),
                    AverageUnitPrice = g.Average(od => od.UnitPrice)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(20)
                .ToListAsync();

            var report = new Report
            {
                ReportType = "BestSelling",
                StartDate = startDate,
                EndDate = endDate,
                AdminId = adminId,
                GeneratedDate = DateTime.Now,
                Data = JsonSerializer.Serialize(bestSellingData)
            };

            await CreateReportAsync(report);
            return report;
        }

        // Lấy Reports theo loại
        public async Task<List<Report>> GetReportsByTypeAsync(string reportType)
        {
            return await _context.Reports
                .Include(r => r.Admin)
                .Where(r => r.ReportType == reportType)
                .OrderByDescending(r => r.GeneratedDate)
                .ToListAsync();
        }

        // Lấy Reports theo Admin
        public async Task<List<Report>> GetReportsByAdminAsync(int adminId)
        {
            return await _context.Reports
                .Include(r => r.Admin)
                .Where(r => r.AdminId == adminId)
                .OrderByDescending(r => r.GeneratedDate)
                .ToListAsync();
        }

        // Lấy Reports theo khoảng thời gian
        public async Task<List<Report>> GetReportsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Reports
                .Include(r => r.Admin)
                .Where(r => r.GeneratedDate >= startDate && r.GeneratedDate <= endDate)
                .OrderByDescending(r => r.GeneratedDate)
                .ToListAsync();
        }

        // Tìm kiếm Reports
        public async Task<List<Report>> SearchReportsAsync(string searchTerm)
        {
            return await _context.Reports
                .Include(r => r.Admin)
                .Where(r => r.ReportType.Contains(searchTerm) ||
                           r.Admin.FullName.Contains(searchTerm))
                .OrderByDescending(r => r.GeneratedDate)
                .ToListAsync();
        }

        // Lấy thống kê tổng quan
        public async Task<object> GetReportStatisticsAsync()
        {
            var totalReports = await _context.Reports.CountAsync();
            var reportsByType = await _context.Reports
                .GroupBy(r => r.ReportType)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToListAsync();

            var recentReports = await _context.Reports
                .OrderByDescending(r => r.GeneratedDate)
                .Take(5)
                .Select(r => new { r.ReportType, r.GeneratedDate })
                .ToListAsync();

            return new
            {
                TotalReports = totalReports,
                ReportsByType = reportsByType,
                RecentReports = recentReports
            };
        }
    }
} 