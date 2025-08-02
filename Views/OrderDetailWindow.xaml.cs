using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AppOrderNilon.Models;

namespace AppOrderNilon.Views
{
    public partial class OrderDetailWindow : Window
    {
        private Order currentOrder;
        private List<Customer> customers;
        private List<Staff> staff;
        private bool isEditMode;

        public OrderDetailWindow(Order order, List<Customer> customers, List<Staff> staff)
        {
            InitializeComponent();
            this.currentOrder = order;
            this.customers = customers;
            this.staff = staff;
            this.isEditMode = order != null;

            LoadOrderData();
        }

        private void LoadOrderData()
        {
            if (isEditMode && currentOrder != null)
            {
                txtHeader.Text = $"Chi tiết đơn hàng #{currentOrder.OrderId}";
                txtOrderID.Text = currentOrder.OrderId.ToString();
                txtOrderDate.Text = currentOrder.OrderDate.ToString("dd/MM/yyyy");
                txtStatus.Text = GetStatusText(currentOrder.Status);
                txtStaffName.Text = currentOrder.StaffName;
                txtCustomerName.Text = currentOrder.CustomerName;
                txtTotalAmount.Text = $"₫{currentOrder.TotalAmount:N0}";
                txtNotes.Text = currentOrder.Notes;

                // Load customer details
                var customer = customers.FirstOrDefault(c => c.CustomerId == currentOrder.CustomerId);
                if (customer != null)
                {
                    txtCustomerPhone.Text = customer.Phone;
                    txtCustomerEmail.Text = customer.Email;
                    txtCustomerAddress.Text = customer.Address;
                }

                // Load order details
                LoadOrderDetails();
            }
            else
            {
                txtHeader.Text = "Tạo đơn hàng mới";
                txtOrderID.Text = "Tự động";
                txtOrderDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                txtStatus.Text = "Chờ xử lý";
                txtTotalAmount.Text = "₫0";
            }
        }

        private void LoadOrderDetails()
        {
            // TODO: Load order details from database
            // For now, using sample data
            var orderDetails = new List<OrderDetail>
            {
                new OrderDetail
                {
                    OrderDetailId = 1,
                    OrderId = currentOrder.OrderId,
                    ProductId = 1,
                    Quantity = 2,
                    UnitPrice = 50000,
                    Subtotal = 100000
                },
                new OrderDetail
                {
                    OrderDetailId = 2,
                    OrderId = currentOrder.OrderId,
                    ProductId = 2,
                    Quantity = 1,
                    UnitPrice = 150000,
                    Subtotal = 150000
                }
            };

            // Set navigation properties (simulate products)
            var products = new List<Product>
            {
                new Product { ProductId = 1, ProductName = "Nilon lót sàn 0.2mm" },
                new Product { ProductId = 2, ProductName = "Mũ bảo hộ ABS" }
            };

            orderDetails[0].Product = products[0];
            orderDetails[1].Product = products[1];

            dgOrderDetails.ItemsSource = orderDetails;
        }

        private string GetStatusText(string status)
        {
            return status switch
            {
                "Pending" => "Chờ xử lý",
                "Completed" => "Đã hoàn thành",
                "Canceled" => "Đã hủy",
                _ => status
            };
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isEditMode)
                {
                    // Update existing order
                    UpdateOrder();
                }
                else
                {
                    // Create new order
                    CreateOrder();
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu đơn hàng: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateOrder()
        {
            // TODO: Save to database
            MessageBox.Show("Đơn hàng đã được tạo thành công!", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UpdateOrder()
        {
            // TODO: Update in database
            MessageBox.Show("Đơn hàng đã được cập nhật thành công!", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}