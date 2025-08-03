using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AppOrderNilon.Models;
using AppOrderNilon.Services;

namespace AppOrderNilon.Views
{
    public partial class StaffManagementWindow : Window
    {
        private readonly StaffService _staffService;
        private readonly AppOrderNilonContext _context;
        private List<Staff> _allStaff;
        private Staff? _selectedStaff;

        public StaffManagementWindow()
        {
            InitializeComponent();
            _context = new AppOrderNilonContext();
            _staffService = new StaffService(_context);
            _allStaff = new List<Staff>();
            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            try
            {
                txtStatus.Text = "Đang tải dữ liệu...";
                _allStaff = await _staffService.GetAllStaffAsync();
                dgStaff.ItemsSource = _allStaff;
                txtStatus.Text = $"Đã tải {_allStaff.Count} nhân viên";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "Lỗi khi tải dữ liệu";
            }
        }

        private async void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var searchTerm = txtSearch.Text.Trim();
                if (string.IsNullOrEmpty(searchTerm))
                {
                    dgStaff.ItemsSource = _allStaff;
                }
                else
                {
                    var filteredStaff = await _staffService.SearchStaffAsync(searchTerm);
                    dgStaff.ItemsSource = filteredStaff;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgStaff_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedStaff = dgStaff.SelectedItem as Staff;
            btnEdit.IsEnabled = _selectedStaff != null;
            btnDelete.IsEnabled = _selectedStaff != null;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var staffForm = new StaffFormWindow();
            if (staffForm.ShowDialog() == true)
            {
                LoadDataAsync();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedStaff == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var staffForm = new StaffFormWindow(_selectedStaff);
            if (staffForm.ShowDialog() == true)
            {
                LoadDataAsync();
            }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedStaff == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa nhân viên '{_selectedStaff.FullName}'?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    txtStatus.Text = "Đang xóa...";
                    var success = await _staffService.DeleteStaffAsync(_selectedStaff.StaffId);

                    if (success)
                    {
                        MessageBox.Show("Xóa nhân viên thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadDataAsync();
                    }
                    else
                    {
                        MessageBox.Show("Không thể xóa nhân viên.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        txtStatus.Text = "Lỗi khi xóa";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa nhân viên: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtStatus.Text = "Lỗi khi xóa";
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _context?.Dispose();
            base.OnClosed(e);
        }
    }
}