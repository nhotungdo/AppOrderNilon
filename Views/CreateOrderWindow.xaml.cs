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
        private Customer _currentCustomer;

        public class OrderDetailData
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public decimal UnitPrice { get; set; }
            public int Quantity { get; set; }
            public decimal Subtotal { get; set; }
            public string Notes { get; set; } = string.Empty;
        }

        public CreateOrderWindow(Customer currentCustomer = null)
        {
            InitializeComponent();
            _currentCustomer = currentCustomer;
            InitializeData();

            // Set focus to product search for better UX
            txtProductSearch.Focus();

            // Add keyboard shortcuts
            this.KeyDown += CreateOrderWindow_KeyDown;
        }

        private void CreateOrderWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Ctrl+S to save order
            if (e.Key == System.Windows.Input.Key.S &&
                (System.Windows.Input.Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Control) == System.Windows.Input.ModifierKeys.Control)
            {
                btnSaveOrder_Click(sender, e);
                e.Handled = true;
            }

            // Ctrl+P to save and print
            if (e.Key == System.Windows.Input.Key.P &&
                (System.Windows.Input.Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Control) == System.Windows.Input.ModifierKeys.Control)
            {
                btnSaveAndPrint_Click(sender, e);
                e.Handled = true;
            }

            // F5 to preview order
            if (e.Key == System.Windows.Input.Key.F5)
            {
                btnPreview_Click(sender, e);
                e.Handled = true;
            }
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
            if (_currentCustomer != null)
            {
                // If current customer is provided, use only that customer
                _customers = new ObservableCollection<Customer> { _currentCustomer };
                cboCustomer.ItemsSource = _customers;
                cboCustomer.SelectedItem = _currentCustomer;
                cboCustomer.IsEnabled = false; // Disable selection since it's the current customer

                // Hide the "Add Customer" button since we're using current customer
                btnAddCustomer.Visibility = Visibility.Collapsed;

                // Auto-fill delivery address
                txtDeliveryAddress.Text = _currentCustomer.Address ?? "";
            }
            else
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
                    MessageBox.Show("❌ Vui lòng nhập số lượng hợp lệ (lớn hơn 0)!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtQuantity.Focus();
                    return;
                }

                // Check stock availability
                if (quantity > selectedProduct.StockQuantity)
                {
                    MessageBox.Show($"⚠️ Số lượng vượt quá tồn kho!\n\nSản phẩm: {selectedProduct.ProductName}\nTồn kho: {selectedProduct.StockQuantity}\nBạn yêu cầu: {quantity}",
                        "Cảnh báo tồn kho", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Check if product already exists in order
                var existingProduct = _selectedProducts.FirstOrDefault(p => p.ProductId == selectedProduct.ProductId);
                if (existingProduct != null)
                {
                    var newTotalQuantity = existingProduct.Quantity + quantity;
                    if (newTotalQuantity > selectedProduct.StockQuantity)
                    {
                        MessageBox.Show($"⚠️ Tổng số lượng vượt quá tồn kho!\n\nSản phẩm: {selectedProduct.ProductName}\nTồn kho: {selectedProduct.StockQuantity}\nĐã có: {existingProduct.Quantity}\nThêm: {quantity}\nTổng: {newTotalQuantity}",
                            "Cảnh báo tồn kho", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    existingProduct.Quantity = newTotalQuantity;
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

                // Show success message
                MessageBox.Show($"✅ Đã thêm sản phẩm thành công!\n\n📦 {selectedProduct.ProductName}\n📊 Số lượng: {quantity}\n💰 Thành tiền: {(selectedProduct.UnitPrice * quantity):N0} VNĐ",
                    "Thêm sản phẩm", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("❌ Vui lòng chọn sản phẩm từ danh sách!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                cboProduct.Focus();
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
                // Validate discount range (0-100%)
                if (discountPercent < 0)
                {
                    txtDiscountPercent.Text = "0";
                    _discountPercent = 0;
                }
                else if (discountPercent > 100)
                {
                    txtDiscountPercent.Text = "100";
                    _discountPercent = 100;
                }
                else
                {
                    _discountPercent = discountPercent;
                }
                UpdateOrderSummary();
            }
            else
            {
                // If invalid input, reset to 0
                if (!string.IsNullOrEmpty(txtDiscountPercent.Text))
                {
                    txtDiscountPercent.Text = "0";
                }
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

            if (txtSubtotal != null)
                txtSubtotal.Text = $"{_subtotal:N0} VNĐ";
            if (txtDiscountAmount != null)
                txtDiscountAmount.Text = $"{discountAmount:N0} VNĐ";
            if (txtTotalAmount != null)
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
            // Validate customer selection
            if (cboCustomer.SelectedItem == null)
            {
                MessageBox.Show("❌ Vui lòng chọn khách hàng!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                cboCustomer.Focus();
                return false;
            }

            // Validate products
            if (_selectedProducts == null || _selectedProducts.Count == 0)
            {
                MessageBox.Show("❌ Vui lòng thêm ít nhất một sản phẩm vào đơn hàng!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Validate delivery address
            if (string.IsNullOrWhiteSpace(txtDeliveryAddress.Text))
            {
                MessageBox.Show("❌ Vui lòng nhập địa chỉ giao hàng!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtDeliveryAddress.Focus();
                return false;
            }

            // Validate total amount
            if (_totalAmount <= 0)
            {
                MessageBox.Show("❌ Tổng tiền đơn hàng phải lớn hơn 0!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Validate stock availability for all products
            foreach (var product in _selectedProducts)
            {
                var originalProduct = _products.FirstOrDefault(p => p.ProductId == product.ProductId);
                if (originalProduct != null && product.Quantity > originalProduct.StockQuantity)
                {
                    MessageBox.Show($"⚠️ Sản phẩm '{product.ProductName}' vượt quá tồn kho!\n\nTồn kho: {originalProduct.StockQuantity}\nĐơn hàng: {product.Quantity}",
                        "Cảnh báo tồn kho", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
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
                var customer = cboCustomer.SelectedItem as Customer;

                var confirmMessage = $"📋 XÁC NHẬN LƯU ĐƠN HÀNG\n\n";
                confirmMessage += $"👤 Khách hàng: {customer?.CustomerName}\n";
                confirmMessage += $"📦 Số sản phẩm: {_selectedProducts?.Count ?? 0} loại\n";
                confirmMessage += $"💰 Tổng tiền: {_totalAmount:N0} VNĐ\n";
                confirmMessage += $"📋 Mã đơn hàng: {orderCode}\n\n";
                confirmMessage += $"Bạn có muốn lưu đơn hàng này?";

                var result = MessageBox.Show(confirmMessage,
                    "Xác nhận lưu đơn hàng", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Simulate saving with progress
                    var progressMessage = $"💾 Đang lưu đơn hàng...\n\n";
                    progressMessage += $"📋 Mã: {orderCode}\n";
                    progressMessage += $"👤 Khách hàng: {customer?.CustomerName}\n";
                    progressMessage += $"📦 Sản phẩm: {_selectedProducts?.Count ?? 0} loại\n";
                    progressMessage += $"💰 Tổng tiền: {_totalAmount:N0} VNĐ";

                    MessageBox.Show(progressMessage, "Đang lưu...", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Success message
                    var successMessage = $"✅ ĐÃ LƯU ĐƠN HÀNG THÀNH CÔNG!\n\n";
                    successMessage += $"📋 Mã đơn hàng: {orderCode}\n";
                    successMessage += $"👤 Khách hàng: {customer?.CustomerName}\n";
                    successMessage += $"📦 Số sản phẩm: {_selectedProducts?.Count ?? 0} loại\n";
                    successMessage += $"💰 Tổng tiền: {_totalAmount:N0} VNĐ\n";
                    successMessage += $"📅 Ngày tạo: {DateTime.Now:dd/MM/yyyy HH:mm}\n\n";
                    successMessage += $"Đơn hàng đã được lưu vào hệ thống.";

                    MessageBox.Show(successMessage, "Lưu đơn hàng thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (printAfterSave)
                    {
                        var printMessage = $"🖨️ ĐANG IN HÓA ĐƠN\n\n";
                        printMessage += $"📋 Mã đơn hàng: {orderCode}\n";
                        printMessage += $"👤 Khách hàng: {customer?.CustomerName}\n";
                        printMessage += $"💰 Tổng tiền: {_totalAmount:N0} VNĐ\n\n";
                        printMessage += $"Vui lòng kiểm tra máy in...";

                        MessageBox.Show(printMessage, "In hóa đơn",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    // Close window
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ LỖI KHI LƯU ĐƠN HÀNG\n\nChi tiết lỗi: {ex.Message}\n\nVui lòng thử lại hoặc liên hệ hỗ trợ.",
                    "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
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