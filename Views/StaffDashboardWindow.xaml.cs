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
            try
            {
                // Mở window quản lý đơn hàng cho staff
                StaffOrderManagementWindow orderWindow = new StaffOrderManagementWindow(currentStaff);
                orderWindow.Show();
                this.WindowState = WindowState.Minimized;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở quản lý đơn hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Tasks_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Mở window quản lý nhiệm vụ cho staff
                StaffTaskManagementWindow taskWindow = new StaffTaskManagementWindow(currentStaff);
                taskWindow.Show();
                this.WindowState = WindowState.Minimized;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở quản lý nhiệm vụ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Inventory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Mở window quản lý tồn kho cho staff
                StaffInventoryManagementWindow inventoryWindow = new StaffInventoryManagementWindow(currentStaff);
                inventoryWindow.Show();
                this.WindowState = WindowState.Minimized;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở quản lý tồn kho: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Customers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Mở window quản lý khách hàng cho staff
                StaffCustomerManagementWindow customerWindow = new StaffCustomerManagementWindow(currentStaff);
                customerWindow.Show();
                this.WindowState = WindowState.Minimized;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở quản lý khách hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Mở window báo cáo cho staff
                StaffReportWindow reportWindow = new StaffReportWindow(currentStaff);
                reportWindow.Show();
                this.WindowState = WindowState.Minimized;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở báo cáo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshOrders_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            UpdateOrderCounts();
            MessageBox.Show("Đã làm mới dữ liệu!", "Thông báo");
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Mở dialog thêm nhiệm vụ mới
                StaffTaskFormWindow taskForm = new StaffTaskFormWindow(currentStaff);
                if (taskForm.ShowDialog() == true)
                {
                    LoadStaffTasks(); // Reload tasks after adding
                    MessageBox.Show("Đã thêm nhiệm vụ mới!", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm nhiệm vụ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateTask_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedTask = dgTasks.SelectedItem as StaffTask;
                if (selectedTask != null)
                {
                    // Mở dialog cập nhật nhiệm vụ
                    StaffTaskFormWindow taskForm = new StaffTaskFormWindow(currentStaff, selectedTask);
                    if (taskForm.ShowDialog() == true)
                    {
                        LoadStaffTasks(); // Reload tasks after updating
                        MessageBox.Show("Đã cập nhật nhiệm vụ!", "Thông báo");
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn một nhiệm vụ để cập nhật!", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật nhiệm vụ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RequestRestock_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedProduct = dgInventory.SelectedItem as Product;
                if (selectedProduct != null)
                {
                    // Mở dialog yêu cầu nhập hàng
                    StaffRestockRequestWindow restockWindow = new StaffRestockRequestWindow(currentStaff, selectedProduct);
                    if (restockWindow.ShowDialog() == true)
                    {
                        MessageBox.Show("Đã gửi yêu cầu nhập hàng!", "Thông báo");
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn một sản phẩm để yêu cầu nhập hàng!", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gửi yêu cầu nhập hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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