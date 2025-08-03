using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppOrderNilon.Models;

namespace AppOrderNilon.Services
{
    public class OrderDetailService
    {
        private readonly AppOrderNilonContext _context;

        public OrderDetailService(AppOrderNilonContext context)
        {
            _context = context;
        }

        // Lấy danh sách OrderDetails theo OrderID
        public async Task<List<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            return await _context.OrderDetails
                .Include(od => od.Product)
                .Include(od => od.Order)
                .Where(od => od.OrderId == orderId)
                .OrderBy(od => od.OrderDetailId)
                .ToListAsync();
        }

        // Lấy OrderDetail theo ID
        public async Task<OrderDetail?> GetOrderDetailByIdAsync(int orderDetailId)
        {
            return await _context.OrderDetails
                .Include(od => od.Product)
                .Include(od => od.Order)
                .FirstOrDefaultAsync(od => od.OrderDetailId == orderDetailId);
        }

        // Tạo mới OrderDetail
        public async Task<bool> CreateOrderDetailAsync(OrderDetail orderDetail)
        {
            try
            {
                // Kiểm tra ProductID hợp lệ
                var product = await _context.Products.FindAsync(orderDetail.ProductId);
                if (product == null)
                {
                    throw new InvalidOperationException("Sản phẩm không tồn tại.");
                }

                // Kiểm tra OrderID hợp lệ
                var order = await _context.Orders.FindAsync(orderDetail.OrderId);
                if (order == null)
                {
                    throw new InvalidOperationException("Đơn hàng không tồn tại.");
                }

                // Tính Subtotal
                orderDetail.UnitPrice = product.UnitPrice;
                orderDetail.Subtotal = orderDetail.Quantity * orderDetail.UnitPrice;

                _context.OrderDetails.Add(orderDetail);
                await _context.SaveChangesAsync();

                // Cập nhật TotalAmount của Order
                if (orderDetail.OrderId.HasValue)
                {
                    await UpdateOrderTotalAmountAsync(orderDetail.OrderId.Value);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        // Cập nhật OrderDetail
        public async Task<bool> UpdateOrderDetailAsync(OrderDetail orderDetail)
        {
            try
            {
                var existingOrderDetail = await _context.OrderDetails.FindAsync(orderDetail.OrderDetailId);
                if (existingOrderDetail == null)
                {
                    throw new InvalidOperationException("Không tìm thấy chi tiết đơn hàng.");
                }

                // Kiểm tra ProductID hợp lệ nếu thay đổi
                if (orderDetail.ProductId != existingOrderDetail.ProductId)
                {
                    var product = await _context.Products.FindAsync(orderDetail.ProductId);
                    if (product == null)
                    {
                        throw new InvalidOperationException("Sản phẩm không tồn tại.");
                    }
                    orderDetail.UnitPrice = product.UnitPrice;
                }

                // Cập nhật thông tin
                existingOrderDetail.ProductId = orderDetail.ProductId;
                existingOrderDetail.Quantity = orderDetail.Quantity;
                existingOrderDetail.UnitPrice = orderDetail.UnitPrice;
                existingOrderDetail.Subtotal = orderDetail.Quantity * orderDetail.UnitPrice;

                await _context.SaveChangesAsync();

                // Cập nhật TotalAmount của Order
                if (existingOrderDetail.OrderId.HasValue)
                {
                    await UpdateOrderTotalAmountAsync(existingOrderDetail.OrderId.Value);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        // Xóa OrderDetail
        public async Task<bool> DeleteOrderDetailAsync(int orderDetailId)
        {
            try
            {
                var orderDetail = await _context.OrderDetails.FindAsync(orderDetailId);
                if (orderDetail == null)
                {
                    throw new InvalidOperationException("Không tìm thấy chi tiết đơn hàng.");
                }

                var orderId = orderDetail.OrderId;

                _context.OrderDetails.Remove(orderDetail);
                await _context.SaveChangesAsync();

                // Cập nhật TotalAmount của Order
                if (orderId.HasValue)
                {
                    await UpdateOrderTotalAmountAsync(orderId.Value);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        // Cập nhật TotalAmount của Order
        private async Task UpdateOrderTotalAmountAsync(int orderId)
        {
            var totalAmount = await _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .SumAsync(od => od.Subtotal);

            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.TotalAmount = totalAmount;
                await _context.SaveChangesAsync();
            }
        }

        // Lấy OrderDetails với thông tin sản phẩm
        public async Task<List<OrderDetail>> GetOrderDetailsWithProductInfoAsync(int orderId)
        {
            return await _context.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.OrderId == orderId)
                .Select(od => new OrderDetail
                {
                    OrderDetailId = od.OrderDetailId,
                    OrderId = od.OrderId,
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    Subtotal = od.Subtotal,
                    Product = od.Product
                })
                .OrderBy(od => od.OrderDetailId)
                .ToListAsync();
        }

        // Tính tổng số lượng sản phẩm trong đơn hàng
        public async Task<int> GetTotalQuantityByOrderIdAsync(int orderId)
        {
            return await _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .SumAsync(od => od.Quantity);
        }

        // Tính tổng giá trị đơn hàng
        public async Task<decimal> GetTotalAmountByOrderIdAsync(int orderId)
        {
            return await _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .SumAsync(od => od.Subtotal);
        }

        // Lấy OrderDetails theo ProductID
        public async Task<List<OrderDetail>> GetOrderDetailsByProductIdAsync(int productId)
        {
            return await _context.OrderDetails
                .Include(od => od.Order)
                .Where(od => od.ProductId == productId)
                .OrderByDescending(od => od.Order.OrderDate)
                .ToListAsync();
        }

        // Kiểm tra sản phẩm có trong đơn hàng nào không
        public async Task<bool> IsProductInAnyOrderAsync(int productId)
        {
            return await _context.OrderDetails
                .AnyAsync(od => od.ProductId == productId);
        }

        // Lấy thống kê sản phẩm bán chạy
        public async Task<List<object>> GetBestSellingProductsAsync(int top = 10)
        {
            var result = await _context.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.Product != null)
                .GroupBy(od => new { od.ProductId, od.Product!.ProductName })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    TotalQuantity = g.Sum(od => od.Quantity),
                    TotalRevenue = g.Sum(od => od.Subtotal)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(top)
                .ToListAsync();

            return result.Cast<object>().ToList();
        }
    }
}