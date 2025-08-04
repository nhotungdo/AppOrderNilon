using System;
using System.Text.RegularExpressions;
using System.Windows;
using AppOrderNilon.Models;
using AppOrderNilon.Services;

namespace AppOrderNilon.Views
{
    public partial class StaffFormWindow : Window
    {
        private AdminService _adminService;
        private Staff _staff;
        private bool _isEditMode;
        private string _originalUsername;

        public Staff Staff => _staff;

        public StaffFormWindow(Staff staff = null)
        {
            InitializeComponent();
            _adminService = new AdminService();
            _staff = staff ?? new Staff();
            _isEditMode = staff != null;
            _originalUsername = staff?.Username ?? "";

            if (_isEditMode)
            {
                PopulateForm();
                txtUsername.IsEnabled = false; // Username không được sửa trong edit mode
            }

            UpdateSaveButton();
        }

        private void PopulateForm()
        {
            txtUsername.Text = _staff.Username ?? "";
            txtFullName.Text = _staff.FullName ?? "";
            txtEmail.Text = _staff.Email ?? "";
            txtPhone.Text = _staff.Phone ?? "";
        }

        private void UpdateSaveButton()
        {
            bool isValid = ValidateForm();
            btnSave.IsEnabled = isValid;
        }

        private bool ValidateForm()
        {
            // Clear previous validation message
            txtValidationMessage.Text = "";

            // Check required fields
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                txtValidationMessage.Text = "Username là bắt buộc.";
                return false;
            }

            if (!_isEditMode && string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                txtValidationMessage.Text = "Password là bắt buộc.";
                return false;
            }

            if (!_isEditMode && txtPassword.Password != txtConfirmPassword.Password)
            {
                txtValidationMessage.Text = "Password và xác nhận password không khớp.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                txtValidationMessage.Text = "Họ và tên là bắt buộc.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                txtValidationMessage.Text = "Email là bắt buộc.";
                return false;
            }

            // Validate email format
            if (!IsValidEmail(txtEmail.Text))
            {
                txtValidationMessage.Text = "Email không hợp lệ.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                txtValidationMessage.Text = "Số điện thoại là bắt buộc.";
                return false;
            }

            // Validate phone format
            if (!IsValidPhone(txtPhone.Text))
            {
                txtValidationMessage.Text = "Số điện thoại không hợp lệ.";
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

        private void txtUsername_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateSaveButton();
        }

        private void txtPassword_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            UpdateSaveButton();
        }

        private void txtConfirmPassword_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            UpdateSaveButton();
        }

        private void txtFullName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateSaveButton();
        }

        private void txtEmail_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateSaveButton();
        }

        private void txtPhone_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateSaveButton();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
                return;

            try
            {
                // Check if username already exists (except for current staff in edit mode)
                if (!_isEditMode || txtUsername.Text != _originalUsername)
                {
                    bool usernameExists = await _adminService.IsUsernameExistsAsync(txtUsername.Text);
                    if (usernameExists)
                    {
                        txtValidationMessage.Text = "Username đã tồn tại. Vui lòng chọn username khác.";
                        return;
                    }
                }

                // Populate staff object
                _staff.Username = txtUsername.Text.Trim();
                _staff.FullName = txtFullName.Text.Trim();
                _staff.Email = txtEmail.Text.Trim();
                _staff.Phone = txtPhone.Text.Trim();

                // Handle password
                if (!_isEditMode || !string.IsNullOrEmpty(txtPassword.Password))
                {
                    _staff.PasswordHash = HashPassword(txtPassword.Password);
                }

                // Save to database
                bool success;
                if (_isEditMode)
                {
                    success = await _adminService.UpdateStaffAsync(_staff);
                }
                else
                {
                    success = await _adminService.CreateStaffAsync(_staff);
                }

                if (success)
                {
                    MessageBox.Show(_isEditMode ? "Cập nhật Staff thành công!" : "Thêm Staff thành công!",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    txtValidationMessage.Text = _isEditMode ? "Lỗi khi cập nhật Staff." : "Lỗi khi thêm Staff.";
                }
            }
            catch (Exception ex)
            {
                txtValidationMessage.Text = $"Lỗi: {ex.Message}";
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}