using System;
using System.Text.RegularExpressions;
using System.Windows;
using AppOrderNilon.Models;

namespace AppOrderNilon.Views
{
    public partial class CustomerProfileWindow : Window
    {
        private Customer _customer;
        private Customer _originalCustomer;

        public Customer UpdatedCustomer => _customer;

        public CustomerProfileWindow(Customer customer)
        {
            InitializeComponent();
            _originalCustomer = customer;
            _customer = new Customer
            {
                CustomerId = customer.CustomerId,
                Username = customer.Username,
                PasswordHash = customer.PasswordHash,
                CustomerName = customer.CustomerName,
                Phone = customer.Phone,
                Email = customer.Email,
                Address = customer.Address,
                Notes = customer.Notes
            };
            LoadCustomerData();
        }

        private void LoadCustomerData()
        {
            txtUsername.Text = _customer.Username;
            txtCustomerName.Text = _customer.CustomerName;
            txtPhone.Text = _customer.Phone ?? "";
            txtEmail.Text = _customer.Email ?? "";
            txtAddress.Text = _customer.Address ?? "";
            txtNotes.Text = _customer.Notes ?? "";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
                return;

            try
            {
                // Update customer information
                _customer.CustomerName = txtCustomerName.Text.Trim();
                _customer.Phone = txtPhone.Text.Trim();
                _customer.Email = txtEmail.Text.Trim();
                _customer.Address = txtAddress.Text.Trim();
                _customer.Notes = txtNotes.Text.Trim();

                // Update password if provided
                if (!string.IsNullOrWhiteSpace(txtNewPassword.Password))
                {
                    // In a real application, you would hash the password here
                    _customer.PasswordHash = HashPassword(txtNewPassword.Password);
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu thông tin: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateForm()
        {
            // Validate Customer Name
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                MessageBox.Show("Vui lòng nhập họ và tên!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtCustomerName.Focus();
                return false;
            }

            // Validate Phone
            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPhone.Focus();
                return false;
            }

            // Validate phone format
            string phonePattern = @"^[0-9+\-\s()]+$";
            if (!Regex.IsMatch(txtPhone.Text, phonePattern))
            {
                MessageBox.Show("Số điện thoại không hợp lệ!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPhone.Focus();
                return false;
            }

            // Validate Email (optional but must be valid if provided)
            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                if (!Regex.IsMatch(txtEmail.Text, emailPattern))
                {
                    MessageBox.Show("Email không hợp lệ!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtEmail.Focus();
                    return false;
                }
            }

            // Validate Address
            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtAddress.Focus();
                return false;
            }

            // Validate new password if provided
            if (!string.IsNullOrWhiteSpace(txtNewPassword.Password))
            {
                if (txtNewPassword.Password.Length < 6)
                {
                    MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtNewPassword.Focus();
                    return false;
                }
            }

            return true;
        }

        private string HashPassword(string password)
        {
            // Simple hash for demo purposes
            // In a real application, use proper password hashing like BCrypt
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
} 