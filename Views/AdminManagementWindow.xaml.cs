using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AppOrderNilon.Models;
using AppOrderNilon.Services;
using AppOrderNilon.Views;

namespace AppOrderNilon.Views
{
    public partial class AdminManagementWindow : Window
    {
        private AdminService _adminService;
        private List<Admin> _allAdmins;
        private List<Staff> _allStaff;
        private Admin _selectedAdmin;
        private Staff _selectedStaff;

        public AdminManagementWindow()
        {
            InitializeComponent();
            _adminService = new AdminService();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                LoadAdmins();
                LoadStaff();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAdmins()
        {
            try
            {
                _allAdmins = _adminService.GetAllAdmins() ?? new List<Admin>();
                dgAdmins.ItemsSource = _allAdmins;

                // Update UI based on admin count
                UpdateAdminUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách admin: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                _allAdmins = new List<Admin>();
                dgAdmins.ItemsSource = _allAdmins;
            }
        }

        private void LoadStaff()
        {
            try
            {
                _allStaff = _adminService.GetAllStaff() ?? new List<Staff>();
                dgStaff.ItemsSource = _allStaff;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách staff: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                _allStaff = new List<Staff>();
                dgStaff.ItemsSource = _allStaff;
            }
        }

        private void UpdateAdminUI()
        {
            // Ensure _allAdmins is not null
            if (_allAdmins == null)
            {
                _allAdmins = new List<Admin>();
            }

            // Disable delete button if only one admin remains
            if (_allAdmins.Count <= 1)
            {
                btnDeleteAdmin.IsEnabled = false;
                btnDeleteAdmin.ToolTip = "Không thể xóa admin cuối cùng";
            }
            else
            {
                btnDeleteAdmin.IsEnabled = _selectedAdmin != null;
                btnDeleteAdmin.ToolTip = "Xóa admin đã chọn";
            }
        }

        // Admin Management
        private void AdminSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                // Ensure _allAdmins is not null
                if (_allAdmins == null)
                {
                    _allAdmins = new List<Admin>();
                }

                string searchText = txtAdminSearch?.Text?.ToLower() ?? "";
                if (string.IsNullOrWhiteSpace(searchText) || searchText == "tìm kiếm admin...")
                {
                    dgAdmins.ItemsSource = _allAdmins;
                    return;
                }

                var filteredAdmins = _allAdmins.Where(a =>
                    (a.Username?.ToLower().Contains(searchText) == true) ||
                    (a.FullName?.ToLower().Contains(searchText) == true) ||
                    (a.Email?.ToLower().Contains(searchText) == true) ||
                    (a.Phone?.ToLower().Contains(searchText) == true)
                ).ToList();

                dgAdmins.ItemsSource = filteredAdmins;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Admin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Ensure _allAdmins is not null
            if (_allAdmins == null)
            {
                _allAdmins = new List<Admin>();
            }

            _selectedAdmin = dgAdmins.SelectedItem as Admin;
            btnEditAdmin.IsEnabled = _selectedAdmin != null;

            // Update delete button state
            if (_selectedAdmin != null && _allAdmins.Count > 1)
            {
                btnDeleteAdmin.IsEnabled = true;
                btnDeleteAdmin.ToolTip = "Xóa admin đã chọn";
            }
            else
            {
                btnDeleteAdmin.IsEnabled = false;
                btnDeleteAdmin.ToolTip = _allAdmins.Count <= 1 ? "Không thể xóa admin cuối cùng" : "Chọn admin để xóa";
            }
        }

        private async void AddAdmin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var adminForm = new AdminFormWindow();
                if (adminForm.ShowDialog() == true)
                {
                    var newAdmin = adminForm.Admin;
                    if (await _adminService.CreateAdminAsync(newAdmin))
                    {
                        MessageBox.Show("Thêm admin thành công!", "Thông báo",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadAdmins();
                    }
                    else
                    {
                        MessageBox.Show("Lỗi khi thêm admin! Có thể tên đăng nhập đã tồn tại.", "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm admin: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void EditAdmin_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedAdmin == null) return;

            try
            {
                var adminForm = new AdminFormWindow(_selectedAdmin);
                if (adminForm.ShowDialog() == true)
                {
                    var updatedAdmin = adminForm.Admin;
                    if (await _adminService.UpdateAdminAsync(updatedAdmin))
                    {
                        MessageBox.Show("Cập nhật admin thành công!", "Thông báo",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadAdmins();
                    }
                    else
                    {
                        MessageBox.Show("Lỗi khi cập nhật admin! Có thể tên đăng nhập đã tồn tại.", "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật admin: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteAdmin_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedAdmin == null) return;

            // Ensure _allAdmins is not null
            if (_allAdmins == null)
            {
                _allAdmins = new List<Admin>();
            }

            // Prevent deletion of the last admin
            if (_allAdmins.Count <= 1)
            {
                MessageBox.Show("Không thể xóa admin cuối cùng trong hệ thống!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa admin '{_selectedAdmin.Username ?? "Unknown"}'?\n\nHành động này không thể hoàn tác!",
                "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (await _adminService.DeleteAdminAsync(_selectedAdmin.AdminId))
                    {
                        MessageBox.Show("Xóa admin thành công!", "Thông báo",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadAdmins();
                    }
                    else
                    {
                        MessageBox.Show("Lỗi khi xóa admin! Có thể admin này đang được sử dụng.", "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa admin: {ex.Message}", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RefreshAdmins_Click(object sender, RoutedEventArgs e)
        {
            LoadAdmins();
        }

        // Staff Management
        private void StaffSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                // Ensure _allStaff is not null
                if (_allStaff == null)
                {
                    _allStaff = new List<Staff>();
                }

                string searchText = txtStaffSearch?.Text?.ToLower() ?? "";
                if (string.IsNullOrWhiteSpace(searchText) || searchText == "tìm kiếm staff...")
                {
                    dgStaff.ItemsSource = _allStaff;
                    return;
                }

                var filteredStaff = _allStaff.Where(s =>
                    (s.Username?.ToLower().Contains(searchText) == true) ||
                    (s.FullName?.ToLower().Contains(searchText) == true) ||
                    (s.Email?.ToLower().Contains(searchText) == true) ||
                    (s.Phone?.ToLower().Contains(searchText) == true)
                ).ToList();

                dgStaff.ItemsSource = filteredStaff;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Staff_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedStaff = dgStaff.SelectedItem as Staff;
            btnEditStaff.IsEnabled = _selectedStaff != null;
            btnDeleteStaff.IsEnabled = _selectedStaff != null;
        }

        private async void AddStaff_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var staffForm = new StaffFormWindow();
                if (staffForm.ShowDialog() == true)
                {
                    var newStaff = staffForm.Staff;
                    if (await _adminService.CreateStaffAsync(newStaff))
                    {
                        MessageBox.Show("Thêm staff thành công!", "Thông báo",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadStaff();
                    }
                    else
                    {
                        MessageBox.Show("Lỗi khi thêm staff! Có thể tên đăng nhập đã tồn tại.", "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm staff: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void EditStaff_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedStaff == null) return;

            try
            {
                var staffForm = new StaffFormWindow(_selectedStaff);
                if (staffForm.ShowDialog() == true)
                {
                    var updatedStaff = staffForm.Staff;
                    if (await _adminService.UpdateStaffAsync(updatedStaff))
                    {
                        MessageBox.Show("Cập nhật staff thành công!", "Thông báo",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadStaff();
                    }
                    else
                    {
                        MessageBox.Show("Lỗi khi cập nhật staff! Có thể tên đăng nhập đã tồn tại.", "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật staff: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteStaff_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedStaff == null) return;

            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa staff '{_selectedStaff.Username ?? "Unknown"}'?\n\nHành động này không thể hoàn tác!",
                "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (await _adminService.DeleteStaffAsync(_selectedStaff.StaffId))
                    {
                        MessageBox.Show("Xóa staff thành công!", "Thông báo",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadStaff();
                    }
                    else
                    {
                        MessageBox.Show("Lỗi khi xóa staff! Có thể staff này đang được sử dụng.", "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa staff: {ex.Message}", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RefreshStaff_Click(object sender, RoutedEventArgs e)
        {
            LoadStaff();
        }

        // Navigation
        private void BackToDashboard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DashboardWindow dashboard = new DashboardWindow();
                dashboard.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chuyển về dashboard: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.Show();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi đăng xuất: {ex.Message}", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _adminService?.Dispose();
            base.OnClosed(e);
        }
    }
}