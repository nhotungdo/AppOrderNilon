using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AppOrderNilon.Models;

namespace AppOrderNilon.Views
{
    public partial class ProductDetailWindow : Window
    {
        private Product currentProduct;
        private List<Category> categories;
        private List<Supplier> suppliers;
        private bool isEditMode;

        public ProductDetailWindow(Product product, List<Category> categories, List<Supplier> suppliers)
        {
            InitializeComponent();
            this.currentProduct = product;
            this.categories = categories;
            this.suppliers = suppliers;
            this.isEditMode = product != null;
            
            LoadComboBoxes();
            LoadProductData();
        }

        private void LoadComboBoxes()
        {
            // Load categories
            cmbCategory.Items.Clear();
            foreach (var category in categories)
            {
                cmbCategory.Items.Add(category.CategoryName);
            }

            // Load suppliers
            cmbSupplier.Items.Clear();
            foreach (var supplier in suppliers)
            {
                cmbSupplier.Items.Add(supplier.SupplierName);
            }
        }

        private void LoadProductData()
        {
            if (isEditMode && currentProduct != null)
            {
                txtHeader.Text = "Sửa sản phẩm";
                txtProductName.Text = currentProduct.ProductName;
                txtDescription.Text = currentProduct.Description;
                txtThickness.Text = currentProduct.Thickness?.ToString() ?? "";
                txtSize.Text = currentProduct.Size;
                txtUnitPrice.Text = currentProduct.UnitPrice.ToString();
                txtStockQuantity.Text = currentProduct.StockQuantity.ToString();
                txtImagePath.Text = currentProduct.ImagePath;

                // Set selected category
                if (!string.IsNullOrEmpty(currentProduct.CategoryName))
                {
                    cmbCategory.SelectedItem = currentProduct.CategoryName;
                }

                // Set selected supplier
                if (!string.IsNullOrEmpty(currentProduct.SupplierName))
                {
                    cmbSupplier.SelectedItem = currentProduct.SupplierName;
                }
            }
            else
            {
                txtHeader.Text = "Thêm sản phẩm mới";
                txtStockQuantity.Text = "0";
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs())
                return;

            try
            {
                if (isEditMode)
                {
                    // Update existing product
                    UpdateProduct();
                }
                else
                {
                    // Create new product
                    CreateProduct();
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu sản phẩm: {ex.Message}", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateProduct()
        {
            // TODO: Save to database
            MessageBox.Show("Sản phẩm đã được tạo thành công!", "Thông báo", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UpdateProduct()
        {
            // TODO: Update in database
            MessageBox.Show("Sản phẩm đã được cập nhật thành công!", "Thông báo", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool ValidateInputs()
        {
            // Validate product name
            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên sản phẩm!", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtProductName.Focus();
                return false;
            }

            // Validate category
            if (cmbCategory.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn danh mục!", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbCategory.Focus();
                return false;
            }

            // Validate supplier
            if (cmbSupplier.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp!", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbSupplier.Focus();
                return false;
            }

            // Validate unit price
            if (!decimal.TryParse(txtUnitPrice.Text, out decimal unitPrice) || unitPrice <= 0)
            {
                MessageBox.Show("Vui lòng nhập giá bán hợp lệ!", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUnitPrice.Focus();
                return false;
            }

            // Validate stock quantity
            if (!int.TryParse(txtStockQuantity.Text, out int stockQuantity) || stockQuantity < 0)
            {
                MessageBox.Show("Vui lòng nhập số lượng tồn kho hợp lệ!", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtStockQuantity.Focus();
                return false;
            }

            // Validate thickness (optional)
            if (!string.IsNullOrWhiteSpace(txtThickness.Text))
            {
                if (!decimal.TryParse(txtThickness.Text, out decimal thickness) || thickness < 0)
                {
                    MessageBox.Show("Vui lòng nhập độ dày hợp lệ!", "Lỗi", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtThickness.Focus();
                    return false;
                }
            }

            return true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
} 