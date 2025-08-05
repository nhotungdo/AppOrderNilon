using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AppOrderNilon.Models;

namespace AppOrderNilon.Views
{
    public partial class StaffRestockRequestWindow : Window
    {
        private Staff currentStaff;
        private Product selectedProduct;
        private List<Product> availableProducts;

        public StaffRestockRequestWindow(Staff staff)
        {
            InitializeComponent();
            currentStaff = staff;
            LoadData();
            InitializeControls();
        }

        public StaffRestockRequestWindow(Staff staff, Product product)
        {
            InitializeComponent();
            currentStaff = staff;
            selectedProduct = product;
            LoadData();
            InitializeControls();
            SetSelectedProduct();
        }

        private void LoadData()
        {
            // Load sample products
            availableProducts = new List<Product>
            {
                new Product { ProductId = 1, ProductName = "Nilon lót sàn 0.2mm", StockQuantity = 100, UnitPrice = 50000 },
                new Product { ProductId = 2, ProductName = "Mũ bảo hộ ABS", StockQuantity = 5, UnitPrice = 150000 },
                new Product { ProductId = 3, ProductName = "Găng tay cao su", StockQuantity = 200, UnitPrice = 30000 },
                new Product { ProductId = 4, ProductName = "Kính bảo hộ", StockQuantity = 0, UnitPrice = 80000 },
                new Product { ProductId = 5, ProductName = "Nilon đen 0.1mm", StockQuantity = 8, UnitPrice = 25000 }
            };
        }

        private void InitializeControls()
        {
            // Populate product combo box
            cmbProduct.ItemsSource = availableProducts;
            cmbProduct.DisplayMemberPath = "ProductName";
            cmbProduct.SelectedValuePath = "ProductId";

            // Set default values
            cmbPriority.SelectedIndex = 1; // Trung bình
            txtQuantity.Text = "50";
        }

        private void SetSelectedProduct()
        {
            if (selectedProduct != null)
            {
                cmbProduct.SelectedItem = availableProducts.FirstOrDefault(p => p.ProductId == selectedProduct.ProductId);
                UpdateCurrentStock();
            }
        }

        private void UpdateCurrentStock()
        {
            var selectedProduct = cmbProduct.SelectedItem as Product;
            if (selectedProduct != null)
            {
                txtCurrentStock.Text = selectedProduct.StockQuantity.ToString();
                
                // Set suggested quantity based on current stock
                if (selectedProduct.StockQuantity <= 10)
                {
                    txtQuantity.Text = "100";
                }
                else if (selectedProduct.StockQuantity <= 50)
                {
                    txtQuantity.Text = "50";
                }
                else
                {
                    txtQuantity.Text = "20";
                }
            }
            else
            {
                txtCurrentStock.Text = "0";
                txtQuantity.Text = "0";
            }
        }

        private bool ValidateForm()
        {
            if (cmbProduct.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbProduct.Focus();
                return false;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Vui lòng nhập số lượng hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtQuantity.Focus();
                return false;
            }

            if (cmbPriority.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn mức ưu tiên!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbPriority.Focus();
                return false;
            }

            return true;
        }

        // Event Handlers
        private void Product_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCurrentStock();
        }

        private void SendRequest_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
                return;

            try
            {
                var product = cmbProduct.SelectedItem as Product;
                var quantity = int.Parse(txtQuantity.Text);
                var priority = (cmbPriority.SelectedItem as ComboBoxItem)?.Content.ToString();
                var notes = txtNotes.Text.Trim();

                // Create restock request
                var request = new RestockRequest
                {
                    RequestId = GetNextRequestId(),
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    RequestedQuantity = quantity,
                    CurrentStock = product.StockQuantity,
                    Priority = priority,
                    Notes = notes,
                    RequestedBy = currentStaff.FullName,
                    RequestDate = DateTime.Now,
                    Status = "Chờ duyệt"
                };

                // TODO: Save to database
                // For now, just show success message
                MessageBox.Show($"Đã gửi yêu cầu nhập hàng thành công!\n\nSản phẩm: {product.ProductName}\nSố lượng: {quantity}\nMức ưu tiên: {priority}", 
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gửi yêu cầu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private int GetNextRequestId()
        {
            // TODO: Get next ID from database
            // For now, return a random number
            return new Random().Next(1000, 9999);
        }
    }

    // Restock Request Model
    public class RestockRequest
    {
        public int RequestId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int RequestedQuantity { get; set; }
        public int CurrentStock { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string RequestedBy { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
} 