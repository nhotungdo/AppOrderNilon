using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using AppOrderNilon.Models;

namespace AppOrderNilon.Views
{
    public partial class RegisterWindow : Window
    {
        private string _selectedUserType = "Customer";

        public RegisterWindow()
        {
            InitializeComponent();
            
            // Set default selection
            cboUserType.SelectedIndex = 0;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cboUserType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboUserType.SelectedItem is ComboBoxItem selectedItem)
            {
                _selectedUserType = selectedItem.Tag.ToString();
            }
        }

        private void chkTerms_Checked(object sender, RoutedEventArgs e)
        {
            ValidateForm();
        }

        private void chkTerms_Unchecked(object sender, RoutedEventArgs e)
        {
            ValidateForm();
        }

        private void ValidateForm()
        {
            bool isValid = true;

            // Check required fields
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
                isValid = false;
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
                isValid = false;
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
                isValid = false;
            if (string.IsNullOrWhiteSpace(txtPhone.Text))
                isValid = false;
            if (string.IsNullOrWhiteSpace(txtPassword.Password))
                isValid = false;
            if (string.IsNullOrWhiteSpace(txtConfirmPassword.Password))
                isValid = false;

            // Check terms and conditions
            if (chkTerms.IsChecked != true)
                isValid = false;

            btnRegister.IsEnabled = isValid;
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate form
                if (!ValidateRegistrationData())
                    return;

                // Check if username already exists
                if (UsernameExists(txtUsername.Text))
                {
                    MessageBox.Show("Tên đăng nhập đã tồn tại! Vui lòng chọn tên khác.",
                        "Lỗi đăng ký", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Check if email already exists
                if (EmailExists(txtEmail.Text))
                {
                    MessageBox.Show("Email đã được sử dụng! Vui lòng sử dụng email khác.",
                        "Lỗi đăng ký", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Create user based on type
                bool success = false;
                string message = "";

                if (_selectedUserType == "Customer")
                {
                    success = RegisterCustomer();
                    message = "Khách hàng";
                }
                else if (_selectedUserType == "Staff")
                {
                    success = RegisterStaff();
                    message = "Nhân viên";
                }

                if (success)
                {
                    MessageBox.Show($"Đăng ký {message} thành công!\n\nBạn có thể đăng nhập bằng tài khoản vừa tạo.",
                        "Đăng ký thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"Có lỗi xảy ra khi đăng ký {message}. Vui lòng thử lại!",
                        "Lỗi đăng ký", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateRegistrationData()
        {
            // Validate full name
            if (string.IsNullOrWhiteSpace(txtFullName.Text) || txtFullName.Text.Length < 2)
            {
                MessageBox.Show("Họ và tên phải có ít nhất 2 ký tự!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtFullName.Focus();
                return false;
            }

            // Validate username
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || txtUsername.Text.Length < 3)
            {
                MessageBox.Show("Tên đăng nhập phải có ít nhất 3 ký tự!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUsername.Focus();
                return false;
            }

            // Validate email
            if (!IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Email không hợp lệ! Vui lòng nhập email đúng định dạng.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return false;
            }

            // Validate phone
            if (!IsValidPhone(txtPhone.Text))
            {
                MessageBox.Show("Số điện thoại không hợp lệ! Vui lòng nhập số điện thoại đúng định dạng.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPhone.Focus();
                return false;
            }

            // Validate password
            if (txtPassword.Password.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPassword.Focus();
                return false;
            }

            // Validate confirm password
            if (txtPassword.Password != txtConfirmPassword.Password)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtConfirmPassword.Focus();
                return false;
            }

            // Validate terms
            if (chkTerms.IsChecked != true)
            {
                MessageBox.Show("Vui lòng đồng ý với điều khoản sử dụng!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            try
            {
                var regex = new Regex(@"^[0-9]{10,11}$");
                return regex.IsMatch(phone.Replace(" ", ""));
            }
            catch
            {
                return false;
            }
        }

        private bool UsernameExists(string username)
        {
            // For demo purposes, check against hardcoded usernames
            string[] existingUsernames = { "admin1", "staff1", "customer1", "admin", "staff", "customer" };
            return Array.Exists(existingUsernames, x => x.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        private bool EmailExists(string email)
        {
            // For demo purposes, check against hardcoded emails
            string[] existingEmails = { "admin@example.com", "staff@example.com", "customer@example.com" };
            return Array.Exists(existingEmails, x => x.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        private bool RegisterCustomer()
        {
            try
            {
                // For demo purposes, simulate successful registration
                // In real app, this would save to database
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool RegisterStaff()
        {
            try
            {
                // For demo purposes, simulate successful registration
                // In real app, this would save to database
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string HashPassword(string password)
        {
            // Simple hash for demo - in real app, use proper hashing like BCrypt
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private void Login_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Close();
        }

        // Add event handlers for text changes to enable/disable register button
        private void txtFullName_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateForm();
        }

        private void txtUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateForm();
        }

        private void txtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateForm();
        }

        private void txtPhone_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateForm();
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ValidateForm();
        }

        private void txtConfirmPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ValidateForm();
        }
    }
}