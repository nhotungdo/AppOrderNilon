using System;
using System.Text.RegularExpressions;
using System.Windows;
using AppOrderNilon.Models;
using AppOrderNilon.Services;
using System.Linq; // Added for .Any()

namespace AppOrderNilon.Views
{
    public partial class AdminFormWindow : Window
    {
        private Admin _admin;
        private bool _isEditMode;
        private AdminService _adminService;

        public Admin Admin => _admin;

        public AdminFormWindow()
        {
            InitializeComponent();
            _admin = new Admin();
            _isEditMode = false;
            _adminService = new AdminService();
            txtTitle.Text = "Thêm Admin mới";
            btnSave.Content = "Thêm";
        }

        public AdminFormWindow(Admin admin)
        {
            InitializeComponent();
            _admin = admin;
            _isEditMode = true;
            _adminService = new AdminService();
            txtTitle.Text = "Sửa thông tin Admin";
            btnSave.Content = "Cập nhật";

            // Hide confirm password for edit mode
            txtConfirmPasswordLabel.Visibility = System.Windows.Visibility.Collapsed;
            txtConfirmPassword.Visibility = System.Windows.Visibility.Collapsed;

            LoadAdminData();
        }

        private void LoadAdminData()
        {
            txtUsername.Text = _admin.Username;
            txtFullName.Text = _admin.FullName;
            txtEmail.Text = _admin.Email;
            txtPhone.Text = _admin.Phone;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
                return;

            try
            {
                _admin.Username = txtUsername.Text.Trim();
                _admin.FullName = txtFullName.Text.Trim();
                _admin.Email = txtEmail.Text.Trim();
                _admin.Phone = txtPhone.Text.Trim();

                if (!_isEditMode)
                {
                    // For new admin, set password
                    _admin.PasswordHash = txtPassword.Password;
                }
                else if (!string.IsNullOrEmpty(txtPassword.Password))
                {
                    // For edit mode, only update password if provided
                    _admin.PasswordHash = txtPassword.Password;
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
            // Validate Username
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUsername.Focus();
                return false;
            }

            if (txtUsername.Text.Length < 3)
            {
                MessageBox.Show("Tên đăng nhập phải có ít nhất 3 ký tự!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUsername.Focus();
                return false;
            }

            // Check for special characters in username
            if (!Regex.IsMatch(txtUsername.Text, @"^[a-zA-Z0-9_]+$"))
            {
                MessageBox.Show("Tên đăng nhập chỉ được chứa chữ cái, số và dấu gạch dưới!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUsername.Focus();
                return false;
            }

            // Check username uniqueness
            if (!_isEditMode || (_isEditMode && txtUsername.Text != _admin.Username))
            {
                var existingAdmins = _adminService.GetAllAdmins();
                if (existingAdmins.Any(a => a.Username.Equals(txtUsername.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("Tên đăng nhập đã tồn tại!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtUsername.Focus();
                    return false;
                }
            }

            // Validate Password
            if (!_isEditMode && string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPassword.Focus();
                return false;
            }

            if (!_isEditMode && txtPassword.Password.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPassword.Focus();
                return false;
            }

            // Validate password strength
            if (!_isEditMode && !IsPasswordStrong(txtPassword.Password))
            {
                MessageBox.Show("Mật khẩu phải chứa ít nhất 1 chữ hoa, 1 chữ thường, 1 số và 1 ký tự đặc biệt!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPassword.Focus();
                return false;
            }

            // Validate Confirm Password (for new admin)
            if (!_isEditMode && txtPassword.Password != txtConfirmPassword.Password)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtConfirmPassword.Focus();
                return false;
            }

            // Validate Full Name
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Vui lòng nhập họ và tên!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtFullName.Focus();
                return false;
            }

            if (txtFullName.Text.Length < 2)
            {
                MessageBox.Show("Họ và tên phải có ít nhất 2 ký tự!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtFullName.Focus();
                return false;
            }

            // Validate Email
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

            // Validate Phone
            if (!string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                string phonePattern = @"^[0-9+\-\s()]+$";
                if (!Regex.IsMatch(txtPhone.Text, phonePattern))
                {
                    MessageBox.Show("Số điện thoại không hợp lệ!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtPhone.Focus();
                    return false;
                }

                // Remove all non-digit characters and check length
                string digitsOnly = Regex.Replace(txtPhone.Text, @"[^\d]", "");
                if (digitsOnly.Length < 10 || digitsOnly.Length > 11)
                {
                    MessageBox.Show("Số điện thoại phải có 10-11 chữ số!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtPhone.Focus();
                    return false;
                }
            }

            return true;
        }

        private bool IsPasswordStrong(string password)
        {
            // Check for at least one uppercase letter, one lowercase letter, one digit, and one special character
            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(c => !char.IsLetterOrDigit(c));

            return hasUpper && hasLower && hasDigit && hasSpecial;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}