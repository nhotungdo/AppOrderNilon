using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppOrderNilon.Models;

namespace AppOrderNilon.Services
{
    public class OrderService
    {
        private readonly AppOrderNilonContext _context;

        public OrderService()
        {
            _context = new AppOrderNilonContext();
        }

        public OrderService(AppOrderNilonContext context)
        {
            _context = context;
        }

        // Get all orders
        public List<Order> GetAllOrders()
        {
            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Staff)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        // Get all orders async
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Staff)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        // Get order by ID
        public Order GetOrderById(int orderId)
        {
            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Staff)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.OrderId == orderId);
        }

        // Search orders
        public List<Order> SearchOrders(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAllOrders();

            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Staff)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => (o.Notes != null && o.Notes.Contains(searchTerm)) ||
                           (o.Customer != null && o.Customer.CustomerName.Contains(searchTerm)) ||
                           (o.Staff != null && o.Staff.FullName.Contains(searchTerm)))
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        // Filter orders by status
        public List<Order> GetOrdersByStatus(string status)
        {
            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Staff)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        // Filter orders by date range
        public List<Order> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Staff)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        // Create new order
        public bool CreateOrder(Order order, List<OrderDetail> orderDetails)
        {
            try
            {
                // Validate input parameters
                if (order == null)
                {
                    System.Diagnostics.Debug.WriteLine("CreateOrder: Order is null");
                    return false;
                }

                if (orderDetails == null || orderDetails.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("CreateOrder: Order details is null or empty");
                    return false;
                }

                // Validate order properties
                if (!order.CustomerId.HasValue || !order.StaffId.HasValue)
                {
                    System.Diagnostics.Debug.WriteLine("CreateOrder: CustomerId or StaffId is null");
                    return false;
                }

                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    System.Diagnostics.Debug.WriteLine($"CreateOrder: Starting transaction for order creation");

                    // Validate stock availability
                    foreach (var detail in orderDetails)
                    {
                        var product = _context.Products.FirstOrDefault(p => p.ProductId == detail.ProductId);
                        if (product == null)
                        {
                            System.Diagnostics.Debug.WriteLine($"CreateOrder: Product not found: ID={detail.ProductId}");
                            throw new InvalidOperationException($"Không tìm thấy sản phẩm với ID: {detail.ProductId}");
                        }

                        if (detail.Quantity > product.StockQuantity)
                        {
                            System.Diagnostics.Debug.WriteLine($"CreateOrder: Insufficient stock for product {product.ProductName}. Available: {product.StockQuantity}, Required: {detail.Quantity}");
                            throw new InvalidOperationException($"Sản phẩm '{product.ProductName}' vượt quá tồn kho. Tồn kho: {product.StockQuantity}, Yêu cầu: {detail.Quantity}");
                        }
                    }

                    // Set order properties
                    order.OrderDate = DateTime.Now;
                    order.Status = "Pending";
                    order.TotalAmount = 0;

                    System.Diagnostics.Debug.WriteLine($"CreateOrder: Adding order to database - CustomerID={order.CustomerId}, StaffID={order.StaffId}, OrderDate={order.OrderDate}");

                    // Add order to context and save to get the ID
                    _context.Orders.Add(order);
                    _context.SaveChanges();

                    System.Diagnostics.Debug.WriteLine($"CreateOrder: Order saved to database with ID={order.OrderId}");

                    // Add order details
                    foreach (var detail in orderDetails)
                    {
                        detail.OrderId = order.OrderId;
                        detail.Subtotal = detail.Quantity * detail.UnitPrice;
                        _context.OrderDetails.Add(detail);

                        System.Diagnostics.Debug.WriteLine($"CreateOrder: Added order detail - OrderID={detail.OrderId}, ProductID={detail.ProductId}, Quantity={detail.Quantity}, Subtotal={detail.Subtotal}");
                    }

                    // Save order details
                    _context.SaveChanges();

                    // Calculate and update total amount
                    order.TotalAmount = orderDetails.Sum(od => od.Subtotal);
                    _context.SaveChanges();

                    System.Diagnostics.Debug.WriteLine($"CreateOrder: Order completed successfully - OrderID={order.OrderId}, TotalAmount={order.TotalAmount}");

                    // Verify the order was saved
                    var savedOrder = _context.Orders
                        .Include(o => o.OrderDetails)
                        .FirstOrDefault(o => o.OrderId == order.OrderId);

                    if (savedOrder != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"CreateOrder: Verification successful - Order {savedOrder.OrderId} with {savedOrder.OrderDetails.Count} details saved to database");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"CreateOrder: Verification failed - Order {order.OrderId} not found in database after save");
                        throw new InvalidOperationException("Đơn hàng không được lưu vào database");
                    }

                    transaction.Commit();
                    System.Diagnostics.Debug.WriteLine($"CreateOrder: Transaction committed successfully");
                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"CreateOrder: Error during transaction: {ex.Message}");
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CreateOrder: Failed to create order: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"CreateOrder: Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        // Update order
        public bool UpdateOrder(Order order, List<OrderDetail> orderDetails)
        {
            try
            {
                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    // Update order
                    var existingOrder = _context.Orders.Find(order.OrderId);
                    if (existingOrder != null)
                    {
                        existingOrder.CustomerId = order.CustomerId;
                        existingOrder.StaffId = order.StaffId;
                        existingOrder.Status = order.Status;
                        existingOrder.Notes = order.Notes;

                        // Remove existing order details
                        var existingDetails = _context.OrderDetails.Where(od => od.OrderId == order.OrderId);
                        _context.OrderDetails.RemoveRange(existingDetails);

                        // Add new order details
                        foreach (var detail in orderDetails)
                        {
                            detail.OrderId = order.OrderId;
                            detail.Subtotal = detail.Quantity * detail.UnitPrice;
                            _context.OrderDetails.Add(detail);
                        }

                        // Update total amount
                        existingOrder.TotalAmount = orderDetails.Sum(od => od.Subtotal);
                        _context.SaveChanges();
                    }

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch
            {
                return false;
            }
        }

        // Delete order
        public bool DeleteOrder(int orderId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"DeleteOrder: Attempting to delete order {orderId}");

                var order = _context.Orders
                    .Include(o => o.OrderDetails)
                    .FirstOrDefault(o => o.OrderId == orderId);

                if (order == null)
                {
                    System.Diagnostics.Debug.WriteLine($"DeleteOrder: Order {orderId} not found");
                    return false;
                }

                System.Diagnostics.Debug.WriteLine($"DeleteOrder: Found order {orderId} with {order.OrderDetails.Count} details");

                // Remove order details first
                if (order.OrderDetails.Any())
                {
                    _context.OrderDetails.RemoveRange(order.OrderDetails);
                    System.Diagnostics.Debug.WriteLine($"DeleteOrder: Removed {order.OrderDetails.Count} order details");
                }

                // Remove order
                _context.Orders.Remove(order);
                _context.SaveChanges();

                System.Diagnostics.Debug.WriteLine($"DeleteOrder: Successfully deleted order {orderId}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeleteOrder: Error deleting order {orderId}: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"DeleteOrder: Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        // Update order status
        public bool UpdateOrderStatus(int orderId, string status)
        {
            try
            {
                var order = _context.Orders.Find(orderId);
                if (order != null)
                {
                    order.Status = status;
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

        // Get order statistics
        public OrderStatistics GetOrderStatistics()
        {
            try
            {
                var now = DateTime.Now;
                var startOfMonth = new DateTime(now.Year, now.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                var monthlyOrders = _context.Orders
                    .Where(o => o.OrderDate >= startOfMonth && o.OrderDate <= endOfMonth)
                    .ToList();

                return new OrderStatistics
                {
                    TotalOrders = _context.Orders.Count(),
                    PendingOrders = _context.Orders.Count(o => o.Status == "Pending"),
                    CompletedOrders = _context.Orders.Count(o => o.Status == "Completed"),
                    CanceledOrders = _context.Orders.Count(o => o.Status == "Canceled"),
                    MonthlyOrders = monthlyOrders.Count,
                    MonthlyRevenue = monthlyOrders.Any() ? monthlyOrders.Sum(o => o.TotalAmount) : 0m,
                    TotalRevenue = _context.Orders.Any() ? _context.Orders.Sum(o => o.TotalAmount) : 0m
                };
            }
            catch
            {
                return new OrderStatistics
                {
                    TotalOrders = 0,
                    PendingOrders = 0,
                    CompletedOrders = 0,
                    CanceledOrders = 0,
                    MonthlyOrders = 0,
                    MonthlyRevenue = 0m,
                    TotalRevenue = 0m
                };
            }
        }

        // Get recent orders
        public List<Order> GetRecentOrders(int count = 5)
        {
            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Staff)
                .OrderByDescending(o => o.OrderDate)
                .Take(count)
                .ToList();
        }

        // Get orders by customer
        public List<Order> GetOrdersByCustomer(int customerId)
        {
            return _context.Orders
                .Include(o => o.Staff)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        // Get orders by staff
        public List<Order> GetOrdersByStaff(int staffId)
        {
            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => o.StaffId == staffId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }

    public class OrderStatistics
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CanceledOrders { get; set; }
        public int MonthlyOrders { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}