using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AppOrderNilon.Models;
using AppOrderNilon.Services;

namespace AppOrderNilon.Views
{
    public partial class StaffFormWindow : Window
    {
        private readonly StaffService _staffService;
        private readonly AppOrderNilonContext _context;
        private readonly Staff? _staff;
        private readonly bool _isEditMode;

        public StaffFormWindow()
        {
            InitializeComponent();
            _context = new AppOrderNilonContext();
            _staffService = new StaffService(_context);
            _isEditMode = false;
            SetupForAdd();
        }

        public StaffFormWindow(Staff staff)
        {
            InitializeComponent();
            _context = new AppOrderNilonContext();
            _staffService = new StaffService(_context);
            _staff = staff;
            _isEditMode = true;
            SetupForEdit();
        }

        private void SetupForAdd()
        {
            txtTitle.Text = "THÊM NHÂN VIÊN MỚI";
            gridChangePassword.Visibility = Visibility.Collapsed;
            gridNewPassword.Visibility = Visibility.Collapsed;
        }

        private void SetupForEdit()
        {
            txtTitle.Text = "SỬA NHÂN VIÊN";
            txtUsername.Text = _staff?.Username ?? "";
            txtFullName.Text = _staff?.FullName ?? "";
            txtEmail.Text = _staff?.Email ?? "";
            txtPhone.Text = _staff?.Phone ?? "";
            
            // Disable username field in edit mode
            txtUsername.IsEnabled = false;
            txtPassword.IsEnabled = false;
            
            gridChangePassword.Visibility = Visibility.Visible;
            gridNewPassword.Visibility = Visibility.Collapsed;
        }

        private void ChkChangePassword_Checked(object sender, RoutedEventArgs e)
        {
            gridNewPassword.Visibility = Visibility.Visible;
        }

        private void ChkChangePassword_Unchecked(object sender, RoutedEventArgs e)
        {
            gridNewPassword.Visibility = Visibility.Collapsed;
            txtNewPassword.Password = "";
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(txtUsername.Text))
                {
                    MessageBox.Show("Vui lòng nhập username.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtUsername.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtFullName.Text))
                {
                    MessageBox.Show("Vui lòng nhập họ tên.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtFullName.Focus();
                    return;
                }

                if (!_isEditMode && string.IsNullOrWhiteSpace(txtPassword.Password))
                {
                    MessageBox.Show("Vui lòng nhập password.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtPassword.Focus();
                    return;
                }

                if (_isEditMode && chkChangePassword.IsChecked == true && string.IsNullOrWhiteSpace(txtNewPassword.Password))
                {
                    MessageBox.Show("Vui lòng nhập password mới.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtNewPassword.Focus();
                    return;
                }

                if (_isEditMode && _staff != null)
                {
                    // Update existing staff
                    _staff.Username = txtUsername.Text.Trim();
                    _staff.FullName = txtFullName.Text.Trim();
                    _staff.Email = txtEmail.Text.Trim();
                    _staff.Phone = txtPhone.Text.Trim();

                    string? newPassword = null;
                    if (chkChangePassword.IsChecked == true)
                    {
                        newPassword = txtNewPassword.Password;
                    }

                    var success = await _staffService.UpdateStaffAsync(_staff, newPassword);
                    if (success)
                    {
                        MessageBox.Show("Cập nhật nhân viên thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Không thể cập nhật nhân viên.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    // Create new staff
                    var newStaff = new Staff
                    {
                        Username = txtUsername.Text.Trim(),
                        FullName = txtFullName.Text.Trim(),
                        Email = txtEmail.Text.Trim(),
                        Phone = txtPhone.Text.Trim()
                    };

                    var success = await _staffService.CreateStaffAsync(newStaff, txtPassword.Password);
                    if (success)
                    {
                        MessageBox.Show("Thêm nhân viên thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Không thể thêm nhân viên.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            _context?.Dispose();
            base.OnClosed(e);
        }
    }
} 
} 