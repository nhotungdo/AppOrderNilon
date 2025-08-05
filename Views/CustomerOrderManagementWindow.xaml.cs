using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AppOrderNilon.Models;

namespace AppOrderNilon.Views
{
    public partial class CustomerOrderManagementWindow : Window
    {
        private Customer currentCustomer;
        private List<Order> allOrders;
        private List<Order> filteredOrders;

        public CustomerOrderManagementWindow(Customer customer)
        {
            InitializeComponent();
            currentCustomer = customer;
            LoadData();
            InitializeControls();
        }

        private void LoadData()
        {
            LoadSampleOrders();
            ApplyFilters();
        }

        private void LoadSampleOrders()
        {
            // Sample orders for this customer
            allOrders = new List<Order>
            {
                new Order 
                { 
                    OrderId = 123, 
                    CustomerId = currentCustomer.CustomerId, 
                    OrderDate = DateTime.Now.AddDays(-5), 
                    TotalAmount = 250000, 
                    Status = "Chờ xử lý"
                },
                new Order 
                { 
                    OrderId = 124, 
                    CustomerId = currentCustomer.CustomerId, 
                    OrderDate = DateTime.Now.AddDays(-3), 
                    TotalAmount = 180000, 
                    Status = "Đang xử lý"
                },
                new Order 
                { 
                    OrderId = 125, 
                    CustomerId = currentCustomer.CustomerId, 
                    OrderDate = DateTime.Now.AddDays(-10), 
                    TotalAmount = 320000, 
                    Status = "Đã giao"
                },
                new Order 
                { 
                    OrderId = 126, 
                    CustomerId = currentCustomer.CustomerId, 
                    OrderDate = DateTime.Now.AddDays(-15), 
                    TotalAmount = 150000, 
                    Status = "Hoàn thành"
                },
                new Order 
                { 
                    OrderId = 127, 
                    CustomerId = currentCustomer.CustomerId, 
                    OrderDate = DateTime.Now.AddDays(-20), 
                    TotalAmount = 450000, 
                    Status = "Hoàn thành"
                }
            };
        }

        private void InitializeControls()
        {
            txtCustomerName.Text = currentCustomer?.CustomerName ?? "Khách hàng";
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

        private void ViewOrderDetails_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var order = button.DataContext as Order;
            
            if (order != null)
            {
                ViewOrderDetails(order);
            }
        }

        private void CancelOrder_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var order = button.DataContext as Order;
            
            if (order != null)
            {
                CancelOrder(order);
            }
        }

        private void ViewOrderDetails(Order order)
        {
            try
            {
                // Create sample data for customers and staff
                var customers = new List<Customer> { currentCustomer };
                var staff = new List<Staff>
                {
                    new Staff { StaffId = 1, FullName = "Trần Nhân Viên", Email = "staff1@company.com" },
                    new Staff { StaffId = 2, FullName = "Nguyễn Văn A", Email = "staff2@company.com" }
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

        private void CancelOrder(Order order)
        {
            if (order.Status == "Chờ xử lý" || order.Status == "Đang xử lý")
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn hủy đơn hàng #{order.OrderId}?\nTổng tiền: {order.TotalAmount:N0} VNĐ",
                    "Xác nhận hủy đơn hàng",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    // Update order status
                    order.Status = "Đã hủy";
                    ApplyFilters();
                    MessageBox.Show($"Đã hủy đơn hàng #{order.OrderId} thành công!", "Thông báo");
                }
            }
            else
            {
                MessageBox.Show("Chỉ có thể hủy đơn hàng ở trạng thái 'Chờ xử lý' hoặc 'Đang xử lý'!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
} 