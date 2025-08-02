using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AppOrderNilon.Models;

namespace AppOrderNilon.Views
{
    public partial class ProductManagementWindow : Window
    {
        private List<Product> allProducts;
        private List<Category> categories;
        private List<Supplier> suppliers;

        public ProductManagementWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            // TODO: Load data from database
            // For now, using sample data
            LoadSampleData();
            RefreshProductGrid();
            UpdateStatusBar();
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
            categories = new List<Category>
            {
                new Category { CategoryId = 1, CategoryName = "Nilon xây dựng", Description = "Nilon lót sàn, bạt phủ công trình", Quantity = 100 },
                new Category { CategoryId = 2, CategoryName = "Vật liệu bảo hộ", Description = "Mũ, găng tay, giày bảo hộ", Quantity = 205 },
                new Category { CategoryId = 3, CategoryName = "Vật liệu xây dựng", Description = "Xi măng, gạch, thép", Quantity = 0 }
            };

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
            suppliers = new List<Supplier>
            {
                new Supplier { SupplierId = 1, SupplierName = "Công ty Nilon ABC", ContactName = "Nguyễn Văn A", Phone = "0901234567", Email = "abc@nilon.com", Address = "123 Đường Láng, Hà Nội" },
                new Supplier { SupplierId = 2, SupplierName = "Công ty Bảo Hộ XYZ", ContactName = "Trần Thị B", Phone = "0912345678", Email = "xyz@baoho.com", Address = "456 Đường Giải Phóng, Hà Nội" }
            };
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

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductDetailWindow productDetailWindow = new ProductDetailWindow(null, categories, suppliers);
            if (productDetailWindow.ShowDialog() == true)
            {
                // TODO: Add new product to database
                LoadData(); // Reload data
            }
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (dgProducts.SelectedItem is Product selectedProduct)
            {
                ProductDetailWindow productDetailWindow = new ProductDetailWindow(selectedProduct, categories, suppliers);
                if (productDetailWindow.ShowDialog() == true)
                {
                    // TODO: Update product in database
                    LoadData(); // Reload data
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần sửa!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (dgProducts.SelectedItem is Product selectedProduct)
            {
                var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa sản phẩm '{selectedProduct.ProductName}'?",
                    "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // TODO: Delete product from database
                    allProducts.Remove(selectedProduct);
                    RefreshProductGrid();
                    UpdateStatusBar();
                    MessageBox.Show("Đã xóa sản phẩm thành công!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
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