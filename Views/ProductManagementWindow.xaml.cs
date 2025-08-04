using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AppOrderNilon.Models;
using AppOrderNilon.Services;

namespace AppOrderNilon.Views
{
    public partial class ProductManagementWindow : Window
    {
        private List<Product> allProducts;
        private List<Category> categories;
        private List<Supplier> suppliers;
        private ProductService _productService;
        private CategoryService _categoryService;
        private SupplierService _supplierService;

        public ProductManagementWindow()
        {
            InitializeComponent();
            var context = new AppOrderNilonContext();
            _productService = new ProductService(context);
            _categoryService = new CategoryService(context);
            _supplierService = new SupplierService(context);
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                // Load data from database
                allProducts = await _productService.GetAllProductsAsync() ?? new List<Product>();
                LoadCategories();
                LoadSuppliers();
                RefreshProductGrid();
                UpdateStatusBar();
            }
            catch (Exception ex)
            {
                // Log error for debugging
                System.Diagnostics.Debug.WriteLine($"Database error: {ex.Message}");
                
                // Fallback to sample data without showing error popup
                LoadSampleData();
                RefreshProductGrid();
                UpdateStatusBar();
            }
        }

        private void LoadSampleData()
        {
            allProducts = new List<Product>
            {
                                new Product
                {
                    ProductId = 1,
                    ProductName = "Nilon lót sàn 0.2mm",
                    CategoryId = 1,
                    SupplierId = 1,
                    Description = "Nilon lót sàn xây dựng",
                    Thickness = 0.2m,
                    Size = "2m x 100m",
                    UnitPrice = 50000,
                    StockQuantity = 100,
                    ImagePath = "/images/nilon.jpg"
                },
                new Product
                {
                    ProductId = 2,
                    ProductName = "Mũ bảo hộ ABS",
                    CategoryId = 2,
                    SupplierId = 2,
                    Description = "Mũ bảo hộ chất lượng cao",
                    Thickness = null,
                    Size = "One size",
                    UnitPrice = 150000,
                    StockQuantity = 5,
                    ImagePath = "/images/helmet.jpg"
                },
                new Product
                {
                    ProductId = 3,
                    ProductName = "Găng tay cao su",
                    CategoryId = 2,
                    SupplierId = 2,
                    Description = "Găng tay bảo hộ chống hóa chất",
                    Thickness = null,
                    Size = "L",
                    UnitPrice = 30000,
                    StockQuantity = 200,
                    ImagePath = "/images/gloves.jpg"
                }
            };

            // Set navigation properties
            LoadCategories();
            LoadSuppliers();

            allProducts[0].Category = categories[0];
            allProducts[0].Supplier = suppliers[0];
            allProducts[1].Category = categories[1];
            allProducts[1].Supplier = suppliers[1];
            allProducts[2].Category = categories[1];
            allProducts[2].Supplier = suppliers[1];
        }

        private void LoadCategories()
        {
            try
            {
                categories = _productService.GetAllCategories() ?? new List<Category>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh mục: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                // Fallback to sample data
                categories = new List<Category>
                {
                    new Category { CategoryId = 1, CategoryName = "Nilon xây dựng", Description = "Nilon lót sàn, bạt phủ công trình", Quantity = 100 },
                    new Category { CategoryId = 2, CategoryName = "Vật liệu bảo hộ", Description = "Mũ, găng tay, giày bảo hộ", Quantity = 205 },
                    new Category { CategoryId = 3, CategoryName = "Vật liệu xây dựng", Description = "Xi măng, gạch, thép", Quantity = 0 }
                };
            }

            cmbCategory.Items.Clear();
            cmbCategory.Items.Add("Tất cả danh mục");
            foreach (var category in categories)
            {
                cmbCategory.Items.Add(category.CategoryName);
            }
            cmbCategory.SelectedIndex = 0;
        }

        private void LoadSuppliers()
        {
            try
            {
                suppliers = _productService.GetAllSuppliers() ?? new List<Supplier>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải nhà cung cấp: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                // Fallback to sample data
                suppliers = new List<Supplier>
                {
                    new Supplier { SupplierId = 1, SupplierName = "Công ty Nilon ABC", ContactName = "Nguyễn Văn A", Phone = "0901234567", Email = "abc@nilon.com", Address = "123 Đường Láng, Hà Nội" },
                    new Supplier { SupplierId = 2, SupplierName = "Công ty Bảo Hộ XYZ", ContactName = "Trần Thị B", Phone = "0912345678", Email = "xyz@baoho.com", Address = "456 Đường Giải Phóng, Hà Nội" }
                };
            }
        }

        private void RefreshProductGrid()
        {
            var filteredProducts = allProducts.AsEnumerable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string searchTerm = txtSearch.Text.ToLower();
                filteredProducts = filteredProducts.Where(p =>
                    p.ProductName.ToLower().Contains(searchTerm) ||
                    p.Description?.ToLower().Contains(searchTerm) == true);
            }

            // Apply category filter
            if (cmbCategory.SelectedIndex > 0)
            {
                string selectedCategory = cmbCategory.SelectedItem.ToString();
                filteredProducts = filteredProducts.Where(p => p.CategoryName == selectedCategory);
            }

            // Apply price filter
            if (cmbPriceFilter.SelectedIndex > 0)
            {
                switch (cmbPriceFilter.SelectedIndex)
                {
                    case 1: // Dưới 100k
                        filteredProducts = filteredProducts.Where(p => p.UnitPrice < 100000);
                        break;
                    case 2: // 100k - 500k
                        filteredProducts = filteredProducts.Where(p => p.UnitPrice >= 100000 && p.UnitPrice <= 500000);
                        break;
                    case 3: // Trên 500k
                        filteredProducts = filteredProducts.Where(p => p.UnitPrice > 500000);
                        break;
                }
            }

            dgProducts.ItemsSource = filteredProducts.ToList();
        }

        private void UpdateStatusBar()
        {
            txtTotalProducts.Text = allProducts.Count.ToString();
            int lowStockCount = allProducts.Count(p => p.StockQuantity <= 5);
            txtLowStockProducts.Text = lowStockCount.ToString();
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshProductGrid();
        }

        private void CategoryFilter_Changed(object sender, SelectionChangedEventArgs e)
        {
            RefreshProductGrid();
        }

        private void PriceFilter_Changed(object sender, SelectionChangedEventArgs e)
        {
            RefreshProductGrid();
        }

        private void Product_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle product selection if needed
        }

        private async void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductDetailWindow productDetailWindow = new ProductDetailWindow(null, categories, suppliers);
            if (productDetailWindow.ShowDialog() == true)
            {
                try
                {
                    var newProduct = productDetailWindow.GetProduct();
                    if (newProduct != null)
                    {
                        bool success = await _productService.CreateProductAsync(newProduct);
                        if (success)
                        {
                            MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadData(); // Reload data from database
                        }
                        else
                        {
                            MessageBox.Show("Lỗi khi thêm sản phẩm!", "Lỗi",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi thêm sản phẩm: {ex.Message}", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (dgProducts.SelectedItem is Product selectedProduct)
            {
                ProductDetailWindow productDetailWindow = new ProductDetailWindow(selectedProduct, categories, suppliers);
                if (productDetailWindow.ShowDialog() == true)
                {
                    try
                    {
                        var updatedProduct = productDetailWindow.GetProduct();
                        if (updatedProduct != null)
                        {
                            updatedProduct.ProductId = selectedProduct.ProductId; // Ensure ID is preserved
                            bool success = await _productService.UpdateProductAsync(updatedProduct);
                            if (success)
                            {
                                MessageBox.Show("Cập nhật sản phẩm thành công!", "Thông báo",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                                LoadData(); // Reload data from database
                            }
                            else
                            {
                                MessageBox.Show("Lỗi khi cập nhật sản phẩm!", "Lỗi",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi cập nhật sản phẩm: {ex.Message}", "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần sửa!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (dgProducts.SelectedItem is Product selectedProduct)
            {
                var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa sản phẩm '{selectedProduct.ProductName}'?",
                    "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        bool success = await _productService.DeleteProductAsync(selectedProduct.ProductId);
                        if (success)
                        {
                            MessageBox.Show("Đã xóa sản phẩm thành công!", "Thông báo",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadData(); // Reload data from database
                        }
                        else
                        {
                            MessageBox.Show("Lỗi khi xóa sản phẩm!", "Lỗi",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi xóa sản phẩm: {ex.Message}", "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BackToDashboard_Click(object sender, RoutedEventArgs e)
        {
            DashboardWindow dashboardWindow = new DashboardWindow();
            dashboardWindow.Show();
            this.Close();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }
    }
}