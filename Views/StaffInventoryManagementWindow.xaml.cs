using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AppOrderNilon.Models;

namespace AppOrderNilon.Views
{
    public partial class StaffInventoryManagementWindow : Window
    {
        private Staff currentStaff;
        private List<Product> allProducts;
        private List<Product> filteredProducts;

        public StaffInventoryManagementWindow(Staff staff)
        {
            InitializeComponent();
            currentStaff = staff;
            LoadData();
            InitializeControls();
        }

        private void LoadData()
        {
            // Load sample data for now
            LoadSampleProducts();
            filteredProducts = new List<Product>(allProducts);
        }

        private void LoadSampleProducts()
        {
            allProducts = new List<Product>
            {
                new Product
                {
                    ProductId = 1,
                    ProductName = "Nilon lót sàn 0.2mm",
                    StockQuantity = 100,
                    UnitPrice = 50000
                },
                new Product
                {
                    ProductId = 2,
                    ProductName = "Mũ bảo hộ ABS",
                    StockQuantity = 5,
                    UnitPrice = 150000
                },
                new Product
                {
                    ProductId = 3,
                    ProductName = "Găng tay cao su",
                    StockQuantity = 200,
                    UnitPrice = 30000
                },
                new Product
                {
                    ProductId = 4,
                    ProductName = "Kính bảo hộ",
                    StockQuantity = 0,
                    UnitPrice = 80000
                },
                new Product
                {
                    ProductId = 5,
                    ProductName = "Nilon đen 0.1mm",
                    StockQuantity = 8,
                    UnitPrice = 25000
                }
            };
        }

        private void InitializeControls()
        {
            txtStaffName.Text = currentStaff?.FullName ?? "Nhân viên";
            cmbStatusFilter.SelectedIndex = 0;
            RefreshInventoryList();
        }

        private void RefreshInventoryList()
        {
            dgInventory.ItemsSource = null;
            dgInventory.ItemsSource = filteredProducts;
        }

        private void ApplyFilters()
        {
            var query = allProducts.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                var searchTerm = txtSearch.Text.ToLower();
                query = query.Where(p =>
                    p.ProductId.ToString().Contains(searchTerm) ||
                    p.ProductName.ToLower().Contains(searchTerm) ||
                    p.StockStatus.ToLower().Contains(searchTerm)
                );
            }

            // Apply status filter
            if (cmbStatusFilter.SelectedIndex > 0)
            {
                var selectedStatus = (cmbStatusFilter.SelectedItem as ComboBoxItem)?.Content.ToString();
                query = query.Where(p => p.StockStatus == selectedStatus);
            }

            filteredProducts = query.ToList();
            RefreshInventoryList();
        }

        // Event Handlers
        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void StatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            ApplyFilters();
            MessageBox.Show("Đã làm mới dữ liệu!", "Thông báo");
        }

        private void RequestRestock_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Open restock request window
                StaffRestockRequestWindow restockWindow = new StaffRestockRequestWindow(currentStaff);
                if (restockWindow.ShowDialog() == true)
                {
                    MessageBox.Show("Đã gửi yêu cầu nhập hàng!", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gửi yêu cầu nhập hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Inventory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection change if needed
        }

        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var product = button.DataContext as Product;

            if (product != null)
            {
                ViewProductDetails(product);
            }
        }

        private void RequestRestockItem_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var product = button.DataContext as Product;

            if (product != null)
            {
                RequestRestockForProduct(product);
            }
        }

        private void ViewProductDetails(Product product)
        {
            try
            {
                // Create sample data for categories and suppliers
                var categories = new List<Category>
                {
                    new Category { CategoryId = 1, CategoryName = "Nilon" },
                    new Category { CategoryId = 2, CategoryName = "Bảo hộ lao động" },
                    new Category { CategoryId = 3, CategoryName = "Dụng cụ" }
                };

                var suppliers = new List<Supplier>
                {
                    new Supplier { SupplierId = 1, SupplierName = "Công ty TNHH ABC", ContactName = "Nguyễn Văn A", Phone = "0987654321" },
                    new Supplier { SupplierId = 2, SupplierName = "Công ty TNHH XYZ", ContactName = "Lê Thị B", Phone = "0971234567" },
                    new Supplier { SupplierId = 3, SupplierName = "Công ty TNHH DEF", ContactName = "Trần Văn C", Phone = "0969876543" }
                };

                // Open product detail window
                ProductDetailWindow detailWindow = new ProductDetailWindow(product, categories, suppliers);
                detailWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở chi tiết sản phẩm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RequestRestockForProduct(Product product)
        {
            try
            {
                // Open restock request window for specific product
                StaffRestockRequestWindow restockWindow = new StaffRestockRequestWindow(currentStaff, product);
                if (restockWindow.ShowDialog() == true)
                {
                    MessageBox.Show($"Đã gửi yêu cầu nhập hàng cho sản phẩm: {product.ProductName}", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gửi yêu cầu nhập hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}