using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AppOrderNilon.Models;
using AppOrderNilon.Services;

namespace AppOrderNilon.Views
{
    public partial class OrderManagementWindow : Window
    {
        private List<Order> allOrders = new();
        private List<Customer> customers = new();
        private List<Staff> staff = new();

        private OrderService _orderService;
        private CustomerService _customerService;
        private AdminService _adminService;

        public OrderManagementWindow()
        {
            InitializeComponent();
            _orderService = new OrderService();
            _customerService = new CustomerService(new AppOrderNilonContext());
            _adminService = new AdminService();
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                // Load data from database
                allOrders = await _orderService.GetAllOrdersAsync();
                customers = await _customerService.GetAllCustomersAsync();
                staff = await _adminService.GetAllStaffAsync();

                RefreshOrderGrid();
                UpdateStatusBar();
            }
            catch (Exception ex)
            {
                // Log error for debugging
                System.Diagnostics.Debug.WriteLine($"Database error in OrderManagement: {ex.Message}");
                
                // Fallback to sample data if database is not available
                LoadSampleData();
                RefreshOrderGrid();
                UpdateStatusBar();
            }
        }

        private void LoadSampleData()
        {
            // Load sample customers and staff first
            LoadCustomers();
            LoadStaff();

            allOrders = new List<Order>
            {
                new Order
                {
                    OrderId = 1,
                    CustomerId = 1,
                    StaffId = 1,
                    OrderDate = new DateTime(2025, 8, 1),
                    TotalAmount = 250000,
                    Status = "Completed",
                    Notes = "Giao hàng nhanh",
                    Customer = customers[0],
                    Staff = staff[0]
                },
                new Order
                {
                    OrderId = 2,
                    CustomerId = 2,
                    StaffId = 2,
                    OrderDate = new DateTime(2025, 8, 2),
                    TotalAmount = 180000,
                    Status = "Pending",
                    Notes = "",
                    Customer = customers[1],
                    Staff = staff[1]
                }
            };
        }

        private void LoadCustomers()
        {
            customers = new List<Customer>
            {
                new Customer { CustomerId = 1, CustomerName = "Công ty Xây dựng Minh Anh", Phone = "0987654321", Email = "minhanh@construction.com", Address = "789 Đường Láng, Hà Nội", Notes = "Khách hàng VIP" },
                new Customer { CustomerId = 2, CustomerName = "Lê Văn C", Phone = "0971234567", Email = "levanc@gmail.com", Address = "123 Đường Nguyễn Trãi, Hà Nội", Notes = "" }
            };
        }

        private void LoadStaff()
        {
            staff = new List<Staff>
            {
                new Staff { StaffId = 1, Username = "staff1", PasswordHash = "hashed_password_staff1", FullName = "Trần Nhân Viên", Email = "staff1@app.com", Phone = "0918765432" },
                new Staff { StaffId = 2, Username = "staff2", PasswordHash = "hashed_password_staff2", FullName = "Phạm Nhân Viên", Email = "staff2@app.com", Phone = "0917654321" }
            };
        }

        private void RefreshOrderGrid()
        {
            var filteredOrders = allOrders.AsEnumerable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string searchTerm = txtSearch.Text.ToLower();
                filteredOrders = filteredOrders.Where(o =>
                    (o.Customer?.CustomerName?.ToLower().Contains(searchTerm) == true) ||
                    (o.Notes?.ToLower().Contains(searchTerm) == true));
            }

            // Apply status filter
            if (cmbStatus.SelectedIndex > 0)
            {
                string selectedStatus = "";
                switch (cmbStatus.SelectedIndex)
                {
                    case 1: selectedStatus = "Pending"; break;
                    case 2: selectedStatus = "Completed"; break;
                    case 3: selectedStatus = "Canceled"; break;
                }
                filteredOrders = filteredOrders.Where(o => o.Status == selectedStatus);
            }

            // Apply date filter
            if (dpFromDate.SelectedDate.HasValue)
            {
                filteredOrders = filteredOrders.Where(o => o.OrderDate >= dpFromDate.SelectedDate.Value);
            }

            if (dpToDate.SelectedDate.HasValue)
            {
                filteredOrders = filteredOrders.Where(o => o.OrderDate <= dpToDate.SelectedDate.Value);
            }

            dgOrders.ItemsSource = filteredOrders.ToList();
        }

        private void UpdateStatusBar()
        {
            try
            {
                var stats = _orderService.GetOrderStatistics();
                txtTotalOrders.Text = $"Tổng số đơn hàng: {stats.TotalOrders}";
                txtPendingOrders.Text = $"Đơn hàng chờ xử lý: {stats.PendingOrders}";
                txtTotalRevenue.Text = $"Tổng doanh thu: ₫{stats.TotalRevenue:N0}";
            }
            catch
            {
                // Fallback to local calculation
                txtTotalOrders.Text = $"Tổng số đơn hàng: {allOrders.Count}";
                txtPendingOrders.Text = $"Đơn hàng chờ xử lý: {allOrders.Count(o => o.Status == "Pending")}";
                txtTotalRevenue.Text = $"Tổng doanh thu: ₫{allOrders.Sum(o => o.TotalAmount):N0}";
            }
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshOrderGrid();
        }

        private void StatusFilter_Changed(object sender, SelectionChangedEventArgs e)
        {
            RefreshOrderGrid();
        }

        private void DateFilter_Changed(object sender, SelectionChangedEventArgs e)
        {
            RefreshOrderGrid();
        }

        private void Order_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle order selection if needed
        }

        private void AddOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CreateOrderWindow createOrderWindow = new CreateOrderWindow();
                createOrderWindow.ShowDialog();

                // Refresh order list after creating new order
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo đơn hàng: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewOrder_Click(object sender, RoutedEventArgs e)
        {
            if (dgOrders.SelectedItem is Order selectedOrder)
            {
                OrderDetailWindow orderDetailWindow = new OrderDetailWindow(selectedOrder, customers, staff);
                orderDetailWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn đơn hàng cần xem!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void EditOrder_Click(object sender, RoutedEventArgs e)
        {
            if (dgOrders.SelectedItem is Order selectedOrder)
            {
                try
                {
                    OrderDetailWindow orderDetailWindow = new OrderDetailWindow(selectedOrder, customers, staff);
                    if (orderDetailWindow.ShowDialog() == true)
                    {
                        // Get the updated order
                        var updatedOrder = orderDetailWindow.CreatedOrder;
                        if (updatedOrder != null)
                        {
                            // Update the order in database
                            bool success = _orderService.UpdateOrder(updatedOrder, updatedOrder.OrderDetails?.ToList() ?? new List<OrderDetail>());
                            if (success)
                            {
                                MessageBox.Show($"Đơn hàng #{updatedOrder.OrderId} đã được cập nhật!", "Thông báo",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                                LoadData(); // Reload data from database
                            }
                            else
                            {
                                MessageBox.Show("Lỗi khi cập nhật đơn hàng!", "Lỗi",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi cập nhật đơn hàng: {ex.Message}", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn đơn hàng cần sửa!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (dgOrders.SelectedItem is Order selectedOrder)
            {
                var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa đơn hàng #{selectedOrder.OrderId}?",
                    "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        bool success = _orderService.DeleteOrder(selectedOrder.OrderId);
                        if (success)
                        {
                            MessageBox.Show("Đã xóa đơn hàng thành công!", "Thông báo",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadData(); // Reload data from database
                        }
                        else
                        {
                            MessageBox.Show("Lỗi khi xóa đơn hàng!", "Lỗi",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi xóa đơn hàng: {ex.Message}", "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn đơn hàng cần xóa!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BackToDashboard_Click(object sender, RoutedEventArgs e)
        {
            DashboardWindow dashboardWindow = new DashboardWindow();
            dashboardWindow.Show();
            this.Close();
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
}