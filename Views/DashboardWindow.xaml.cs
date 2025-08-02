using System.Windows;
using AppOrderNilon.Views;

namespace AppOrderNilon.Views
{
    public partial class DashboardWindow : Window
    {
        public DashboardWindow()
        {
            InitializeComponent();
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            // TODO: Load real data from database
            // For now, using sample data
            txtTotalRevenue.Text = "₫50,000,000";
            txtTotalOrders.Text = "150";
            txtProductsSold.Text = "1,250";
            txtPendingOrders.Text = "25";
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

        private void ManageProducts_Click(object sender, RoutedEventArgs e)
        {
            ProductManagementWindow productWindow = new ProductManagementWindow();
            productWindow.Show();
            this.Hide();
        }

        private void ManageOrders_Click(object sender, RoutedEventArgs e)
        {
            OrderManagementWindow orderWindow = new OrderManagementWindow();
            orderWindow.Show();
            this.Hide();
        }

        private void ManageCustomers_Click(object sender, RoutedEventArgs e)
        {
            CustomerManagementWindow customerWindow = new CustomerManagementWindow();
            customerWindow.Show();
            this.Hide();
        }

        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            ReportWindow reportWindow = new ReportWindow();
            reportWindow.Show();
            this.Hide();
        }
    }
} 