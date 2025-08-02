using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AppOrderNilon.Models;

namespace AppOrderNilon.Views
{
    public partial class CustomerDashboardWindow : Window
    {
        private Customer currentCustomer;
        private List<Order> customerOrders;
        private List<CustomerPromotion> promotions;

        public CustomerDashboardWindow(Customer customer)
        {
            InitializeComponent();
            currentCustomer = customer;
            LoadData();
            InitializeControls();
        }

        private void LoadData()
        {
            // TODO: Load data from database
            // For now, using sample data
            LoadSampleData();
            LoadPromotions();
            UpdateOrderHistory();
        }

        private void LoadSampleData()
        {
            // Sample orders for this customer
            customerOrders = new List<Order>
            {
                new Order { OrderId = 123, CustomerId = 1, OrderDate = new DateTime(2025, 8, 1), TotalAmount = 250000, Status = "Shipped" },
                new Order { OrderId = 124, CustomerId = 1, OrderDate = new DateTime(2025, 8, 3), TotalAmount = 180000, Status = "Processing" },
                new Order { OrderId = 125, CustomerId = 1, OrderDate = new DateTime(2025, 7, 25), TotalAmount = 320000, Status = "Completed" },
                new Order { OrderId = 126, CustomerId = 1, OrderDate = new DateTime(2025, 7, 15), TotalAmount = 150000, Status = "Completed" }
            };
        }

        private void LoadPromotions()
        {
            promotions = new List<CustomerPromotion>
            {
                new CustomerPromotion 
                { 
                    PromotionId = 1, 
                    Title = "Giảm 10% cho đơn hàng đầu tiên", 
                    Description = "Áp dụng cho tất cả sản phẩm nilon",
                    DiscountPercent = 10,
                    ExpiryDate = new DateTime(2025, 12, 31),
                    IsActive = true
                },
                new CustomerPromotion 
                { 
                    PromotionId = 2, 
                    Title = "Miễn phí vận chuyển", 
                    Description = "Cho đơn hàng từ 500,000 VNĐ",
                    DiscountPercent = 0,
                    ExpiryDate = new DateTime(2025, 8, 15),
                    IsActive = true
                },
                new CustomerPromotion 
                { 
                    PromotionId = 3, 
                    Title = "Tích điểm thưởng", 
                    Description = "1,250 điểm hiện có - Đổi 1,000 điểm = 50,000 VNĐ",
                    DiscountPercent = 0,
                    ExpiryDate = new DateTime(2025, 12, 31),
                    IsActive = true
                }
            };
        }

        private void InitializeControls()
        {
            // Set customer name
            txtCustomerName.Text = currentCustomer?.CustomerName ?? "Khách hàng";
            txtProfileName.Text = currentCustomer?.CustomerName ?? "Khách hàng";
            txtProfileEmail.Text = currentCustomer?.Email ?? "";
            txtProfilePhone.Text = currentCustomer?.Phone ?? "";
            txtProfileAddress.Text = currentCustomer?.Address ?? "";

            // Set default filter
            cmbOrderFilter.SelectedIndex = 0;
        }

        private void UpdateOrderHistory()
        {
            dgOrderHistory.ItemsSource = customerOrders;
        }

        // Event Handlers
        private void MyOrders_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Open detailed order management window
            MessageBox.Show("Mở quản lý đơn hàng chi tiết", "Thông báo");
        }

        private void PlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Open new order window
            MessageBox.Show("Mở form đặt hàng mới", "Thông báo");
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Open profile management window
            MessageBox.Show("Mở quản lý hồ sơ", "Thông báo");
        }

        private void Rewards_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Open rewards/promotions window
            MessageBox.Show("Mở quản lý ưu đãi", "Thông báo");
        }

        private void Support_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Open support window
            MessageBox.Show("Mở hỗ trợ khách hàng", "Thông báo");
        }

        private void TrackOrder_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Open order tracking window
            MessageBox.Show("Theo dõi đơn hàng #123", "Thông báo");
        }

        private void DownloadInvoice_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Generate and download invoice
            MessageBox.Show("Đang tải hóa đơn...", "Thông báo");
        }

        private void OrderFilter_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (cmbOrderFilter.SelectedItem is ComboBoxItem selectedItem)
            {
                var filterText = selectedItem.Content.ToString();
                var filteredOrders = customerOrders;

                switch (filterText)
                {
                    case "Đang xử lý":
                        filteredOrders = customerOrders.Where(o => o.Status == "Processing").ToList();
                        break;
                    case "Đã giao":
                        filteredOrders = customerOrders.Where(o => o.Status == "Shipped").ToList();
                        break;
                    case "Hoàn thành":
                        filteredOrders = customerOrders.Where(o => o.Status == "Completed").ToList();
                        break;
                    default:
                        // "Tất cả" - no filtering
                        break;
                }

                dgOrderHistory.ItemsSource = filteredOrders;
            }
        }

        private void RefreshOrders_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            UpdateOrderHistory();
            MessageBox.Show("Đã làm mới dữ liệu!", "Thông báo");
        }

        private void ViewOrderDetails_Click(object sender, RoutedEventArgs e)
        {
            if (dgOrderHistory.SelectedItem is Order selectedOrder)
            {
                // TODO: Open order details window
                MessageBox.Show($"Xem chi tiết đơn hàng #{selectedOrder.OrderId}", "Thông báo");
            }
        }

        private void CancelOrder_Click(object sender, RoutedEventArgs e)
        {
            if (dgOrderHistory.SelectedItem is Order selectedOrder)
            {
                var result = MessageBox.Show($"Bạn có chắc chắn muốn hủy đơn hàng #{selectedOrder.OrderId}?", 
                    "Xác nhận hủy đơn hàng", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    // TODO: Cancel order logic
                    selectedOrder.Status = "Canceled";
                    UpdateOrderHistory();
                    MessageBox.Show("Đã hủy đơn hàng thành công!", "Thông báo");
                }
            }
        }

        private void ContactSupport_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Open support contact form
            MessageBox.Show("Mở form liên hệ hỗ trợ", "Thông báo");
        }

        private void ViewReports_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Open customer reports window
            MessageBox.Show("Mở báo cáo khách hàng", "Thông báo");
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Open profile edit window
            MessageBox.Show("Mở chỉnh sửa hồ sơ", "Thông báo");
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

    // Customer Promotion Model
    public class CustomerPromotion
    {
        public int PromotionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }
    }
} 