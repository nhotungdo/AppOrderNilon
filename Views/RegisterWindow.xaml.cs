using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace AppOrderNilon.Views
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
            txtEmail.Focus();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs())
                return;

            // TODO: Implement registration logic here
            MessageBox.Show("Tài khoản đã được tạo thành công!", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);

            // Close this window and open login window
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private bool ValidateInputs()
        {
            // Check if all fields are filled
            if (string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrEmpty(txtPassword.Password) ||
                string.IsNullOrEmpty(txtConfirmPassword.Password))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Validate email format
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(txtEmail.Text, emailPattern))
            {
                MessageBox.Show("Email không đúng định dạng!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return false;
            }

            // Validate password length
            if (txtPassword.Password.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPassword.Focus();
                return false;
            }

            // Validate password confirmation
            if (txtPassword.Password != txtConfirmPassword.Password)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtConfirmPassword.Focus();
                return false;
            }

            return true;
        }

        private void BackToLogin_Click(object sender, MouseButtonEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}