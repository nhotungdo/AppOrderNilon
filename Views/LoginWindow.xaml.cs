using System.Windows;
using System.Windows.Input;
using AppOrderNilon.Views;

namespace AppOrderNilon.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            txtUsername.Focus();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin đăng nhập!", "Thông báo", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // TODO: Implement authentication logic here
            // For now, just show a message and open dashboard
            MessageBox.Show("Đăng nhập thành công!", "Thông báo", 
                MessageBoxButton.OK, MessageBoxImage.Information);
            
            // Open dashboard
            DashboardWindow dashboardWindow = new DashboardWindow();
            dashboardWindow.Show();
            this.Close();
        }

        private void Register_Click(object sender, MouseButtonEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }

        private void ForgotPassword_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Chức năng quên mật khẩu sẽ được implement sau!", "Thông báo", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
} 