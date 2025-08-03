using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AppOrderNilon.Models;
using AppOrderNilon.Services;

namespace AppOrderNilon.Views
{
    public partial class SupplierManagementWindow : Window
    {
        private readonly SupplierService _supplierService;
        private readonly AppOrderNilonContext _context;
        private List<Supplier> _allSuppliers;
        private Supplier? _selectedSupplier;

        public SupplierManagementWindow()
        {
            InitializeComponent();
            _context = new AppOrderNilonContext();
            _supplierService = new SupplierService(_context);
            _allSuppliers = new List<Supplier>();
            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            try
            {
                txtStatus.Text = "Đang tải dữ liệu...";
                _allSuppliers = await _supplierService.GetAllSuppliersAsync();
                dgSuppliers.ItemsSource = _allSuppliers;
                txtStatus.Text = $"Đã tải {_allSuppliers.Count} nhà cung cấp";
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
                    dgSuppliers.ItemsSource = _allSuppliers;
                }
                else
                {
                    var filteredSuppliers = await _supplierService.SearchSuppliersAsync(searchTerm);
                    dgSuppliers.ItemsSource = filteredSuppliers;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgSuppliers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedSupplier = dgSuppliers.SelectedItem as Supplier;
            btnEdit.IsEnabled = _selectedSupplier != null;
            btnDelete.IsEnabled = _selectedSupplier != null;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var supplierForm = new SupplierFormWindow();
            if (supplierForm.ShowDialog() == true)
            {
                LoadDataAsync();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSupplier == null)
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp cần sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var supplierForm = new SupplierFormWindow(_selectedSupplier);
            if (supplierForm.ShowDialog() == true)
            {
                LoadDataAsync();
            }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSupplier == null)
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp cần xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa nhà cung cấp '{_selectedSupplier.SupplierName}'?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    txtStatus.Text = "Đang xóa...";
                    var success = await _supplierService.DeleteSupplierAsync(_selectedSupplier.SupplierId);
                    
                    if (success)
                    {
                        MessageBox.Show("Xóa nhà cung cấp thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadDataAsync();
                    }
                    else
                    {
                        MessageBox.Show("Không thể xóa nhà cung cấp.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        txtStatus.Text = "Lỗi khi xóa";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa nhà cung cấp: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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