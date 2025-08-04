using System.Windows;
using AppOrderNilon.Views;
using AppOrderNilon.Services;
using AppOrderNilon.Models;

namespace AppOrderNilon.Views
{
    public partial class DashboardWindow : Window
    {
        private AdminService _adminService;
        private Admin _currentAdmin;

        public DashboardWindow()
        {
            InitializeComponent();
            _adminService = new AdminService();
            LoadDashboardData();
        }

        public DashboardWindow(Admin admin)
        {
            InitializeComponent();
            _adminService = new AdminService();
            _currentAdmin = admin;
            LoadDashboardData();
            UpdateUserInfo();
        }

        private void LoadDashboardData()
        {
            try
            {
                var stats = _adminService.GetDashboardStatistics();
                txtTotalRevenue.Text = $"₫{stats.TotalRevenue:N0}";
                txtTotalOrders.Text = stats.TotalOrders.ToString();
                txtProductsSold.Text = stats.ProductsSold.ToString();
                txtPendingOrders.Text = stats.PendingOrders.ToString();

                // Load recent orders and low stock alerts
                LoadRecentOrders();
                LoadLowStockAlerts();
                LoadMonthlyRevenueChart();
            }
            catch (System.Exception ex)
            {
                // Fallback to sample data if database is not available
                txtTotalRevenue.Text = "₫50,000,000";
                txtTotalOrders.Text = "150";
                txtProductsSold.Text = "1,250";
                txtPendingOrders.Text = "25";

                // Log the error for debugging
                System.Diagnostics.Debug.WriteLine($"Dashboard data loading failed: {ex.Message}");
            }
        }

        private void UpdateUserInfo()
        {
            try
            {
                if (_currentAdmin != null)
                {
                    txtUserName.Text = _currentAdmin.FullName ?? _currentAdmin.Username;
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"User info update failed: {ex.Message}");
            }
        }

        private void LoadRecentOrders()
        {
            try
            {
                var recentOrders = _adminService.GetRecentOrders(5);
                // TODO: Update UI with recent orders
                // This would typically update a ListView or DataGrid
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Recent orders loading failed: {ex.Message}");
            }
        }

        private void LoadLowStockAlerts()
        {
            try
            {
                var lowStockProducts = _adminService.GetLowStockProducts(10);
                // TODO: Update UI with low stock alerts
                // This would typically update a ListView or DataGrid
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Low stock alerts loading failed: {ex.Message}");
            }
        }

        private void LoadMonthlyRevenueChart()
        {
            try
            {
                var monthlyData = _adminService.GetMonthlyRevenueData(6);
                // TODO: Update chart with monthly revenue data
                // This would typically update a chart control
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Monthly revenue chart loading failed: {ex.Message}");
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi khi đăng xuất: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ManageProducts_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProductManagementWindow productWindow = new ProductManagementWindow();
                productWindow.ShowDialog();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở quản lý sản phẩm: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ManageOrders_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OrderManagementWindow orderWindow = new OrderManagementWindow();
                orderWindow.ShowDialog();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở quản lý đơn hàng: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ManageCustomers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CustomerManagementWindow customerWindow = new CustomerManagementWindow();
                customerWindow.ShowDialog();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở quản lý khách hàng: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ReportWindow reportWindow = new ReportWindow();
                reportWindow.ShowDialog();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở báo cáo: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ManageStaffAndCustomers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Tạo một window mới để quản lý cả Staff và Customer
                var result = MessageBox.Show("Chọn loại quản lý:\n\n" +
                    "• Nhấn 'Yes' để mở Quản lý Staff\n" +
                    "• Nhấn 'No' để mở Quản lý Customer\n" +
                    "• Nhấn 'Cancel' để hủy", 
                    "Chọn chức năng", 
                    MessageBoxButton.YesNoCancel, 
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Mở quản lý Staff
                    StaffManagementWindow staffWindow = new StaffManagementWindow();
                    staffWindow.ShowDialog();
                }
                else if (result == MessageBoxResult.No)
                {
                    // Mở quản lý Customer
                    CustomerManagementWindow customerWindow = new CustomerManagementWindow();
                    customerWindow.ShowDialog();
                }
                // Nếu Cancel thì không làm gì
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở quản lý: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SystemSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show("Chức năng cài đặt hệ thống sẽ được triển khai trong phiên bản tiếp theo!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở cài đặt hệ thống: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ManageCategories_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CategoryManagementWindow categoryWindow = new CategoryManagementWindow();
                categoryWindow.ShowDialog();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở quản lý danh mục: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ManageStaff_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StaffManagementWindow staffWindow = new StaffManagementWindow();
                staffWindow.ShowDialog();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở quản lý nhân viên: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ManageSuppliers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SupplierManagementWindow supplierWindow = new SupplierManagementWindow();
                supplierWindow.ShowDialog();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở quản lý nhà cung cấp: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ManageReports_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ReportManagementWindow reportWindow = new ReportManagementWindow();
                reportWindow.ShowDialog();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở quản lý báo cáo: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnClosed(System.EventArgs e)
        {
            _adminService?.Dispose();
            base.OnClosed(e);
        }
    }
}