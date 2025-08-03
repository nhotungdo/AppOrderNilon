using System;
using System.Collections.Generic;
using System.Linq; // Added for .Sum()
using System.Windows;
using AppOrderNilon.Models;
using AppOrderNilon.Services;

namespace AppOrderNilon.Views
{
    public partial class CustomerDetailWindow : Window
    {
        private Customer _customer;
        private CustomerService _customerService;
        private List<Order> _customerOrders;

        public CustomerDetailWindow(Customer customer)
        {
            InitializeComponent();
            _customer = customer;
            _customerService = new CustomerService();
            
            if (_customer != null)
            {
                LoadCustomerData();
                LoadOrderHistory();
            }
        }

        private void LoadCustomerData()
        {
            txtCustomerName.Text = _customer.CustomerName ?? "";
            txtPhone.Text = _customer.Phone ?? "";
            txtEmail.Text = _customer.Email ?? "";
            txtAddress.Text = _customer.Address ?? "";
            txtNotes.Text = _customer.Notes ?? "";
            
            txtTitle.Text = $"Chi tiết Khách hàng: {_customer.CustomerName}";
        }

        private void LoadOrderHistory()
        {
            try
            {
                _customerOrders = _customerService.GetCustomerOrderHistory(_customer.CustomerId);
                dgOrders.ItemsSource = _customerOrders;
                
                // Update order summary
                txtOrderSummary.Text = $"Tổng cộng: {_customerOrders.Count} đơn hàng";
                
                // Calculate total value
                decimal totalValue = _customerOrders.Sum(o => o.TotalAmount);
                if (totalValue > 0)
                {
                    txtOrderSummary.Text += $" - Tổng giá trị: ₫{totalValue:N0}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải lịch sử đơn hàng: {ex.Message}", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                _customerOrders = new List<Order>();
                dgOrders.ItemsSource = _customerOrders;
                txtOrderSummary.Text = "Tổng cộng: 0 đơn hàng";
            }
        }

        private void ViewOrderDetails_Click(object sender, RoutedEventArgs e)
        {
            if (dgOrders.SelectedItem is Order selectedOrder)
            {
                try
                {
                    // Get customers and staff for OrderDetailWindow
                    var customers = _customerService.GetAllCustomers();
                    var staff = new List<Staff>(); // TODO: Get staff from service
                    
                    var orderDetailWindow = new OrderDetailWindow(selectedOrder, customers, staff);
                    orderDetailWindow.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi mở chi tiết đơn hàng: {ex.Message}", "Lỗi", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn đơn hàng cần xem chi tiết!", "Thông báo", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            _customerService?.Dispose();
            base.OnClosed(e);
        }
    }
} 