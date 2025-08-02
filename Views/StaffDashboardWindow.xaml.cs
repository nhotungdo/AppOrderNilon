using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AppOrderNilon.Models;

namespace AppOrderNilon.Views
{
    public partial class StaffDashboardWindow : Window
    {
        private Staff currentStaff;
        private List<Order> assignedOrders;
        private List<StaffTask> staffTasks;
        private List<Product> inventoryItems;

        public StaffDashboardWindow(Staff staff)
        {
            InitializeComponent();
            currentStaff = staff;
            LoadData();
            InitializeControls();
        }

        private void LoadData()
        {
            // TODO: Load data from database
            // For now, using sample data
            LoadSampleData();
            LoadStaffTasks();
            LoadInventoryData();
            UpdatePerformanceMetrics();
        }

        private void LoadSampleData()
        {
            // Sample orders assigned to this staff
            assignedOrders = new List<Order>
            {
                new Order { OrderId = 1, CustomerId = 1, OrderDate = new DateTime(2025, 8, 1), TotalAmount = 250000, Status = "Pending" },
                new Order { OrderId = 2, CustomerId = 2, OrderDate = new DateTime(2025, 8, 2), TotalAmount = 180000, Status = "Processing" },
                new Order { OrderId = 3, CustomerId = 1, OrderDate = new DateTime(2025, 8, 3), TotalAmount = 320000, Status = "Shipped" }
            };

            // Sample customers
            var customers = new List<Customer>
            {
                new Customer { CustomerId = 1, CustomerName = "Công ty Xây dựng Minh Anh", Phone = "0987654321", Email = "minhanh@construction.com", Address = "789 Đường Láng, Hà Nội", Notes = "Khách hàng VIP" },
                new Customer { CustomerId = 2, CustomerName = "Lê Văn C", Phone = "0971234567", Email = "levanc@gmail.com", Address = "123 Đường Nguyễn Trãi, Hà Nội", Notes = "" }
            };

            // Set navigation properties
            assignedOrders[0].Customer = customers[0];
            assignedOrders[1].Customer = customers[1];
            assignedOrders[2].Customer = customers[0];
        }

        private void LoadStaffTasks()
        {
            staffTasks = new List<StaffTask>
            {
                new StaffTask
                {
                    TaskId = 1,
                    TaskName = "Xử lý đơn hàng #123",
                    Description = "Kiểm tra và chuẩn bị đơn hàng cho khách hàng VIP",
                    DueDate = DateTime.Now.AddHours(2),
                    Priority = "Cao",
                    Status = "Đang thực hiện"
                },
                new StaffTask
                {
                    TaskId = 2,
                    TaskName = "Kiểm tra tồn kho",
                    Description = "Kiểm tra số lượng sản phẩm nilon trong kho",
                    DueDate = DateTime.Now.AddDays(1),
                    Priority = "Trung bình",
                    Status = "Chờ thực hiện"
                },
                new StaffTask
                {
                    TaskId = 3,
                    TaskName = "Liên hệ khách hàng",
                    Description = "Gọi điện xác nhận đơn hàng #124",
                    DueDate = DateTime.Now.AddHours(1),
                    Priority = "Cao",
                    Status = "Hoàn thành"
                }
            };

            dgTasks.ItemsSource = staffTasks;
        }

        private void LoadInventoryData()
        {
            inventoryItems = new List<Product>
            {
                new Product { ProductId = 1, ProductName = "Nilon lót sàn 0.2mm", StockQuantity = 100, UnitPrice = 50000 },
                new Product { ProductId = 2, ProductName = "Mũ bảo hộ ABS", StockQuantity = 5, UnitPrice = 150000 },
                new Product { ProductId = 3, ProductName = "Găng tay cao su", StockQuantity = 200, UnitPrice = 30000 }
            };

            dgInventory.ItemsSource = inventoryItems;
        }

        private void InitializeControls()
        {
            // Set staff name
            txtStaffName.Text = currentStaff?.FullName ?? "Nhân viên";

            // Update order counts
            UpdateOrderCounts();
        }

        private void UpdateOrderCounts()
        {
            txtPendingOrders.Text = assignedOrders.Count(o => o.Status == "Pending").ToString();
            txtProcessingOrders.Text = assignedOrders.Count(o => o.Status == "Processing").ToString();
            txtShippedOrders.Text = assignedOrders.Count(o => o.Status == "Shipped").ToString();
            txtCompletedOrders.Text = assignedOrders.Count(o => o.Status == "Completed").ToString();
        }

        private void UpdatePerformanceMetrics()
        {
            // TODO: Calculate real performance metrics
            // For now, using sample data
        }

        // Event Handlers
        private void Orders_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Open order management window
            MessageBox.Show("Mở quản lý đơn hàng", "Thông báo");
        }

        private void Tasks_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Open task management window
            MessageBox.Show("Mở quản lý nhiệm vụ", "Thông báo");
        }

        private void Inventory_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Open inventory management window
            MessageBox.Show("Mở quản lý tồn kho", "Thông báo");
        }

        private void Customers_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Open customer management window
            MessageBox.Show("Mở quản lý khách hàng", "Thông báo");
        }

        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Open reports window
            MessageBox.Show("Mở báo cáo", "Thông báo");
        }

        private void RefreshOrders_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            UpdateOrderCounts();
            MessageBox.Show("Đã làm mới dữ liệu!", "Thông báo");
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Open add task dialog
            MessageBox.Show("Thêm nhiệm vụ mới", "Thông báo");
        }

        private void UpdateTask_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Open update task dialog
            MessageBox.Show("Cập nhật nhiệm vụ", "Thông báo");
        }

        private void RequestRestock_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Send restock request
            MessageBox.Show("Đã gửi yêu cầu nhập hàng!", "Thông báo");
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }
    }

    // Staff Task Model
    public class StaffTask
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}