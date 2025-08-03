using System;
using System.Threading.Tasks;
using System.Windows;
using AppOrderNilon.Models;
using AppOrderNilon.Services;

namespace AppOrderNilon.Views
{
    public partial class SupplierFormWindow : Window
    {
        private readonly SupplierService _supplierService;
        private readonly AppOrderNilonContext _context;
        private readonly Supplier? _supplier;
        private readonly bool _isEditMode;

        public SupplierFormWindow()
        {
            InitializeComponent();
            _context = new AppOrderNilonContext();
            _supplierService = new SupplierService(_context);
            _isEditMode = false;
            SetupForAdd();
        }

        public SupplierFormWindow(Supplier supplier)
        {
            InitializeComponent();
            _context = new AppOrderNilonContext();
            _supplierService = new SupplierService(_context);
            _supplier = supplier;
            _isEditMode = true;
            SetupForEdit();
        }

        private void SetupForAdd()
        {
            txtTitle.Text = "THÊM NHÀ CUNG CẤP MỚI";
        }

        private void SetupForEdit()
        {
            txtTitle.Text = "SỬA NHÀ CUNG CẤP";
            txtSupplierName.Text = _supplier?.SupplierName ?? "";
            txtContactName.Text = _supplier?.ContactName ?? "";
            txtPhone.Text = _supplier?.Phone ?? "";
            txtEmail.Text = _supplier?.Email ?? "";
            txtAddress.Text = _supplier?.Address ?? "";
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSupplierName.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên nhà cung cấp.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtSupplierName.Focus();
                    return;
                }

                if (_isEditMode && _supplier != null)
                {
                    // Update existing supplier
                    _supplier.SupplierName = txtSupplierName.Text.Trim();
                    _supplier.ContactName = txtContactName.Text.Trim();
                    _supplier.Phone = txtPhone.Text.Trim();
                    _supplier.Email = txtEmail.Text.Trim();
                    _supplier.Address = txtAddress.Text.Trim();

                    var success = await _supplierService.UpdateSupplierAsync(_supplier);
                    if (success)
                    {
                        MessageBox.Show("Cập nhật nhà cung cấp thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Không thể cập nhật nhà cung cấp.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    // Create new supplier
                    var newSupplier = new Supplier
                    {
                        SupplierName = txtSupplierName.Text.Trim(),
                        ContactName = txtContactName.Text.Trim(),
                        Phone = txtPhone.Text.Trim(),
                        Email = txtEmail.Text.Trim(),
                        Address = txtAddress.Text.Trim()
                    };

                    var success = await _supplierService.CreateSupplierAsync(newSupplier);
                    if (success)
                    {
                        MessageBox.Show("Thêm nhà cung cấp thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Không thể thêm nhà cung cấp.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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