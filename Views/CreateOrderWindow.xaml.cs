using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using AppOrderNilon.Models;

namespace AppOrderNilon.Views
{
    public partial class CreateOrderWindow : Window
    {
        private ObservableCollection<OrderDetailData> _selectedProducts;
        private ObservableCollection<Customer> _customers;
        private ObservableCollection<Product> _products;
        private decimal _subtotal = 0;
        private decimal _discountPercent = 0;
        private decimal _totalAmount = 0;

        public class OrderDetailData
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public decimal UnitPrice { get; set; }
            public int Quantity { get; set; }
            public decimal Subtotal { get; set; }
            public string Notes { get; set; } = string.Empty;
        }

        public CreateOrderWindow()
        {
            InitializeComponent();
            InitializeData();
        }

        private void InitializeData()
        {
            _selectedProducts = new ObservableCollection<OrderDetailData>();
            dgSelectedProducts.ItemsSource = _selectedProducts;

            LoadCustomers();
            LoadProducts();
            UpdateOrderSummary();
        }

        private void LoadCustomers()
        {
            // Sample data - in real app, this would load from database
            _customers = new ObservableCollection<Customer>
            {
                new Customer { CustomerId = 1, CustomerName = "Công ty Xây dựng Minh Anh", Phone = "0901234567", Email = "info@minhanh.com", Address = "123 Đường ABC, Quận 1, TP.HCM" },
                new Customer { CustomerId = 2, CustomerName = "Lê Văn C", Phone = "0912345678", Email = "levanc@gmail.com", Address = "456 Đường XYZ, Quận 2, TP.HCM" },
                new Customer { CustomerId = 3, CustomerName = "Công ty Thầu XYZ", Phone = "0923456789", Email = "contact@xyz.com", Address = "789 Đường DEF, Quận 3, TP.HCM" }
            };

            cboCustomer.ItemsSource = _customers;
            cboCustomer.DisplayMemberPath = "CustomerName";
        }

        private void LoadProducts()
        {
            // Sample data - in real app, this would load from database
            _products = new ObservableCollection<Product>
            {
                new Product { ProductId = 1, ProductName = "Nilon lót sàn 2m x 50m", UnitPrice = 150000, StockQuantity = 50 },
                new Product { ProductId = 2, ProductName = "Mũ bảo hộ lao động", UnitPrice = 25000, StockQuantity = 100 },
                new Product { ProductId = 3, ProductName = "Găng tay bảo hộ", UnitPrice = 15000, StockQuantity = 200 },
                new Product { ProductId = 4, ProductName = "Xi măng Hà Tiên PCB40", UnitPrice = 85000, StockQuantity = 30 }
            };

            cboProduct.ItemsSource = _products;
            cboProduct.DisplayMemberPath = "ProductName";
        }

        private void cboCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboCustomer.SelectedItem is Customer selectedCustomer)
            {
                txtDeliveryAddress.Text = selectedCustomer.Address ?? "";
            }
            else
            {
                txtDeliveryAddress.Text = "";
            }
        }

        private void txtProductSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Ensure _products is initialized
            if (_products == null)
            {
                LoadProducts();
                return;
            }

            string searchText = txtProductSearch.Text?.ToLower() ?? "";
            var filteredProducts = _products.Where(p =>
                p.ProductName?.ToLower().Contains(searchText) == true
            ).ToList();

            cboProduct.ItemsSource = filteredProducts;
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            // Ensure _selectedProducts is initialized
            if (_selectedProducts == null)
            {
                _selectedProducts = new ObservableCollection<OrderDetailData>();
                dgSelectedProducts.ItemsSource = _selectedProducts;
            }

            if (cboProduct.SelectedItem is Product selectedProduct)
            {
                if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
                {
                    MessageBox.Show("Vui lòng nhập số lượng hợp lệ!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Check if product already exists in order
                var existingProduct = _selectedProducts.FirstOrDefault(p => p.ProductId == selectedProduct.ProductId);
                if (existingProduct != null)
                {
                    existingProduct.Quantity += quantity;
                    existingProduct.Subtotal = existingProduct.UnitPrice * existingProduct.Quantity;
                }
                else
                {
                    var orderDetail = new OrderDetailData
                    {
                        ProductId = selectedProduct.ProductId,
                        ProductName = selectedProduct.ProductName,
                        UnitPrice = selectedProduct.UnitPrice,
                        Quantity = quantity,
                        Subtotal = selectedProduct.UnitPrice * quantity,
                        Notes = ""
                    };

                    _selectedProducts.Add(orderDetail);
                }

                // Clear inputs
                cboProduct.SelectedIndex = -1;
                txtQuantity.Text = "1";
                txtProductSearch.Text = "";

                UpdateOrderSummary();
                dgSelectedProducts.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn sản phẩm!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnRemoveProduct_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is OrderDetailData product)
            {
                if (_selectedProducts != null)
                {
                    _selectedProducts.Remove(product);
                    UpdateOrderSummary();
                }
            }
        }

        private void txtDiscountPercent_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (decimal.TryParse(txtDiscountPercent.Text ?? "", out decimal discountPercent))
            {
                _discountPercent = Math.Max(0, Math.Min(100, discountPercent));
                UpdateOrderSummary();
            }
            else
            {
                _discountPercent = 0;
                UpdateOrderSummary();
            }
        }

        private void UpdateOrderSummary()
        {
            // Ensure _selectedProducts is initialized
            if (_selectedProducts == null)
            {
                _selectedProducts = new ObservableCollection<OrderDetailData>();
                dgSelectedProducts.ItemsSource = _selectedProducts;
            }

            _subtotal = _selectedProducts?.Sum(p => p.Subtotal) ?? 0;
            decimal discountAmount = _subtotal * (_discountPercent / 100);
            _totalAmount = _subtotal - discountAmount;

            txtSubtotal.Text = $"{_subtotal:N0} VNĐ";
            txtDiscountAmount.Text = $"{discountAmount:N0} VNĐ";
            txtTotalAmount.Text = $"{_totalAmount:N0} VNĐ";
        }

        private void btnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng thêm khách hàng mới sẽ được triển khai!", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateOrder())
                return;

            var previewMessage = GenerateOrderPreview();
            MessageBox.Show(previewMessage, "Xem trước đơn hàng",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnSaveOrder_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateOrder())
                return;

            SaveOrder(false);
        }

        private void btnSaveAndPrint_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateOrder())
                return;

            SaveOrder(true);
        }

        private bool ValidateOrder()
        {
            if (cboCustomer.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn khách hàng!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                cboCustomer.Focus();
                return false;
            }

            if (_selectedProducts == null || _selectedProducts.Count == 0)
            {
                MessageBox.Show("Vui lòng thêm ít nhất một sản phẩm vào đơn hàng!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDeliveryAddress.Text))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ giao hàng!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtDeliveryAddress.Focus();
                return false;
            }

            return true;
        }

        private string GenerateOrderPreview()
        {
            var customer = cboCustomer.SelectedItem as Customer;
            var preview = $"📋 XEM TRƯỚC ĐƠN HÀNG\n\n";
            preview += $"👤 Khách hàng: {customer?.CustomerName}\n";
            preview += $"📞 Số điện thoại: {customer?.Phone}\n";
            preview += $"📧 Email: {customer?.Email}\n";
            preview += $"📍 Địa chỉ giao hàng: {txtDeliveryAddress.Text}\n\n";
            preview += $"📦 Sản phẩm ({_selectedProducts?.Count ?? 0} loại):\n";

            if (_selectedProducts != null)
            {
                foreach (var product in _selectedProducts)
                {
                    preview += $"  • {product.ProductName} x{product.Quantity} = {product.Subtotal:N0} VNĐ\n";
                }
            }

            preview += $"\n💰 Tổng tiền hàng: {_subtotal:N0} VNĐ\n";
            preview += $"🎫 Giảm giá: {_discountPercent}% ({_subtotal * (_discountPercent / 100):N0} VNĐ)\n";
            preview += $"💵 Tổng cộng: {_totalAmount:N0} VNĐ\n\n";
            preview += $"📝 Ghi chú: {txtOrderNotes.Text}";

            return preview;
        }

        private void SaveOrder(bool printAfterSave)
        {
            try
            {
                // In real app, this would save to database
                var orderCode = GenerateOrderCode();

                var result = MessageBox.Show($"Đơn hàng sẽ được lưu với mã: {orderCode}\n\nBạn có muốn tiếp tục?",
                    "Xác nhận lưu đơn hàng", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Simulate saving
                    MessageBox.Show($"✅ Đã lưu đơn hàng thành công!\n\n📋 Mã đơn hàng: {orderCode}\n💰 Tổng tiền: {_totalAmount:N0} VNĐ",
                        "Lưu đơn hàng", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (printAfterSave)
                    {
                        MessageBox.Show("🖨️ Đang in hóa đơn...", "In hóa đơn",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    // Close window
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi lưu đơn hàng: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GenerateOrderCode()
        {
            // Generate order code with format: DH + YYYYMMDD + 4 digits
            return $"DH{DateTime.Now:yyyyMMdd}{new Random().Next(1000, 9999)}";
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProducts.Count > 0)
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn quay lại? Dữ liệu đơn hàng sẽ bị mất!",
                    "Xác nhận quay lại", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProducts.Count > 0)
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn hủy? Dữ liệu đơn hàng sẽ bị mất!",
                    "Xác nhận hủy", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }
        }
    }
}