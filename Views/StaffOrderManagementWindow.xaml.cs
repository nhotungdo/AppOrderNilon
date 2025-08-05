using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AppOrderNilon.Models;

namespace AppOrderNilon.Views
{
    public partial class StaffOrderManagementWindow : Window
    {
        private Staff currentStaff;
        private List<Order> allOrders;
        private List<Order> filteredOrders;

        public StaffOrderManagementWindow(Staff staff)
        {
            InitializeComponent();
            currentStaff = staff;
            LoadData();
            InitializeControls();
        }

        private void LoadData()
        {
            // Load sample data for now
            LoadSampleOrders();
            filteredOrders = new List<Order>(allOrders);
        }

        private void LoadSampleOrders()
        {
            allOrders = new List<Order>
            {
                new Order
                {
                    OrderId = 123,
                    CustomerId = 1,
                    OrderDate = DateTime.Now.AddDays(-2),
                    TotalAmount = 250000,
                    Status = "Chờ xử lý",
                    Customer = new Customer { CustomerId = 1, CustomerName = "Công ty Xây dựng Minh Anh" }
                },
                new Order
                {
                    OrderId = 124,
                    CustomerId = 2,
                    OrderDate = DateTime.Now.AddDays(-1),
                    TotalAmount = 180000,
                    Status = "Đang xử lý",
                    Customer = new Customer { CustomerId = 2, CustomerName = "Lê Văn C" }
                },
                new Order
                {
                    OrderId = 125,
                    CustomerId = 3,
                    OrderDate = DateTime.Now,
                    TotalAmount = 320000,
                    Status = "Đã giao",
                    Customer = new Customer { CustomerId = 3, CustomerName = "Nguyễn Thị D" }
                },
                new Order
                {
                    OrderId = 126,
                    CustomerId = 1,
                    OrderDate = DateTime.Now.AddDays(-3),
                    TotalAmount = 450000,
                    Status = "Hoàn thành",
                    Customer = new Customer { CustomerId = 1, CustomerName = "Công ty Xây dựng Minh Anh" }
                }
            };
        }

        private void InitializeControls()
        {
            txtStaffName.Text = currentStaff?.FullName ?? "Nhân viên";
            cmbStatusFilter.SelectedIndex = 0;
            RefreshOrdersList();
        }

        private void RefreshOrdersList()
        {
            dgOrders.ItemsSource = null;
            dgOrders.ItemsSource = filteredOrders;
        }

        private void ApplyFilters()
        {
            var query = allOrders.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                var searchTerm = txtSearch.Text.ToLower();
                query = query.Where(o =>
                    o.OrderId.ToString().Contains(searchTerm) ||
                    (o.Customer != null && o.Customer.CustomerName != null && o.Customer.CustomerName.ToLower().Contains(searchTerm)) ||
                    o.Status.ToLower().Contains(searchTerm)
                );
            }

            // Apply status filter
            if (cmbStatusFilter.SelectedIndex > 0)
            {
                var selectedStatus = (cmbStatusFilter.SelectedItem as ComboBoxItem)?.Content.ToString();
                query = query.Where(o => o.Status == selectedStatus);
            }

            filteredOrders = query.ToList();
            RefreshOrdersList();
        }

        // Event Handlers
        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void StatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            ApplyFilters();
            MessageBox.Show("Đã làm mới dữ liệu!", "Thông báo");
        }

        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            var selectedOrder = dgOrders.SelectedItem as Order;
            if (selectedOrder != null)
            {
                ViewOrderDetails(selectedOrder);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một đơn hàng để xem chi tiết!", "Thông báo");
            }
        }

        private void Orders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection change if needed
        }

        private void ProcessOrder_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var order = button.DataContext as Order;

            if (order != null)
            {
                ProcessOrder(order);
            }
        }

        private void ViewOrderDetails_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var order = button.DataContext as Order;

            if (order != null)
            {
                ViewOrderDetails(order);
            }
        }

        private void ProcessOrder(Order order)
        {
            var customerName = order.Customer?.CustomerName ?? "Không xác định";
            var result = MessageBox.Show(
                $"Bạn có muốn xử lý đơn hàng #{order.OrderId}?\nKhách hàng: {customerName}",
                "Xác nhận xử lý đơn hàng",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                // Update order status
                switch (order.Status)
                {
                    case "Chờ xử lý":
                        order.Status = "Đang xử lý";
                        break;
                    case "Đang xử lý":
                        order.Status = "Đã giao";
                        break;
                    case "Đã giao":
                        order.Status = "Hoàn thành";
                        break;
                }

                ApplyFilters();
                MessageBox.Show($"Đã cập nhật trạng thái đơn hàng #{order.OrderId} thành '{order.Status}'", "Thông báo");
            }
        }

        private void ViewOrderDetails(Order order)
        {
            try
            {
                // Create sample data for customers and staff
                var customers = new List<Customer>
                {
                    new Customer { CustomerId = 1, CustomerName = "Công ty Xây dựng Minh Anh", Phone = "0987654321", Email = "minhanh@construction.com" },
                    new Customer { CustomerId = 2, CustomerName = "Lê Văn C", Phone = "0971234567", Email = "levanc@gmail.com" },
                    new Customer { CustomerId = 3, CustomerName = "Nguyễn Thị D", Phone = "0969876543", Email = "nguyenthid@gmail.com" }
                };

                var staff = new List<Staff>
                {
                    new Staff { StaffId = 1, FullName = "Trần Nhân Viên", Email = "staff1@company.com" },
                    new Staff { StaffId = 2, FullName = "Nguyễn Văn A", Email = "staff2@company.com" },
                    new Staff { StaffId = 3, FullName = "Lê Thị B", Email = "staff3@company.com" }
                };

                // Open order detail window
                OrderDetailWindow detailWindow = new OrderDetailWindow(order, customers, staff);
                detailWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở chi tiết đơn hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}