using System.Windows;
using AppOrderNilon.Views;

namespace AppOrderNilon
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeDashboard();
        }

        private void InitializeDashboard()
        {
            txtCurrentDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            LoadDashboard();
        }

        private void LoadDashboard()
        {
            // Load dashboard content
            txtPageTitle.Text = "Dashboard";
            MainFrame.Navigate(new DashboardWindow());
        }

        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {
            txtPageTitle.Text = "Dashboard";
            MainFrame.Navigate(new DashboardWindow());
        }

        private void btnProducts_Click(object sender, RoutedEventArgs e)
        {
            txtPageTitle.Text = "Quản lý sản phẩm";
            MainFrame.Navigate(new ProductManagementWindow());
        }

        private void btnOrders_Click(object sender, RoutedEventArgs e)
        {
            txtPageTitle.Text = "Quản lý đơn hàng";
            MainFrame.Navigate(new OrderManagementWindow());
        }

        private void btnCustomers_Click(object sender, RoutedEventArgs e)
        {
            txtPageTitle.Text = "Quản lý khách hàng";
            MainFrame.Navigate(new CustomerManagementWindow());
        }

        private void btnStaff_Click(object sender, RoutedEventArgs e)
        {
            txtPageTitle.Text = "Quản lý nhân viên";
            MainFrame.Navigate(new StaffManagementWindow());
        }

        private void btnSuppliers_Click(object sender, RoutedEventArgs e)
        {
            txtPageTitle.Text = "Quản lý nhà cung cấp";
            MainFrame.Navigate(new SupplierManagementWindow());
        }

        private void btnCategories_Click(object sender, RoutedEventArgs e)
        {
            txtPageTitle.Text = "Quản lý danh mục";
            MainFrame.Navigate(new CategoryManagementWindow());
        }

        private void btnReports_Click(object sender, RoutedEventArgs e)
        {
            txtPageTitle.Text = "Báo cáo";
            MainFrame.Navigate(new ReportManagementWindow());
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            txtPageTitle.Text = "Cài đặt hệ thống";
            MainFrame.Navigate(new SystemSettingsWindow());
        }

        private void btnAdmins_Click(object sender, RoutedEventArgs e)
        {
            txtPageTitle.Text = "Quản lý Admin";
            MainFrame.Navigate(new AdminManagementWindow());
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận đăng xuất", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                // Show login window
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                
                // Close main window
                this.Close();
            }
        }
    }
} 