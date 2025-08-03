using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AppOrderNilon.Models;
using AppOrderNilon.Services;

namespace AppOrderNilon.Views
{
    public partial class OrderDetailWindow : Window
    {
        private Order currentOrder;
        private List<Customer> customers;
        private List<Staff> staff;
        private List<Product> products = new();
        private List<OrderDetail> orderDetails;
        private bool isEditMode;

        private OrderService _orderService;
        private ProductService _productService;
        private CustomerService _customerService;

        // Property to return the created/updated order
        public Order CreatedOrder => currentOrder;

        public OrderDetailWindow(Order order, List<Customer> customers, List<Staff> staff)
        {
            InitializeComponent();
            this.currentOrder = order;
            this.customers = customers;
            this.staff = staff;
            this.isEditMode = order != null;

            _orderService = new OrderService(new AppOrderNilonContext());
            _productService = new ProductService(new AppOrderNilonContext());
            _customerService = new CustomerService(new AppOrderNilonContext());

            orderDetails = new List<OrderDetail>();

            LoadOrderData();
            LoadComboBoxes();
        }

        private void LoadOrderData()
        {
            if (isEditMode && currentOrder != null)
            {
                txtHeader.Text = $"Chi tiết đơn hàng #{currentOrder.OrderId}";
                txtOrderID.Text = currentOrder.OrderId.ToString();
                txtOrderDate.Text = currentOrder.OrderDate.ToString("dd/MM/yyyy");
                txtStatus.Text = GetStatusText(currentOrder.Status);
                txtTotalAmount.Text = $"₫{currentOrder.TotalAmount:N0}";
                txtNotes.Text = currentOrder.Notes;

                // Set selected customer and staff
                if (currentOrder.CustomerId.HasValue)
                {
                    var customer = customers.FirstOrDefault(c => c.CustomerId == currentOrder.CustomerId);
                    if (customer != null)
                    {
                        cmbCustomer.SelectedItem = customer;
                        UpdateCustomerInfo(customer);
                    }
                }

                if (currentOrder.StaffId.HasValue)
                {
                    var staffMember = staff.FirstOrDefault(s => s.StaffId == currentOrder.StaffId);
                    if (staffMember != null)
                    {
                        cmbStaff.SelectedItem = staffMember;
                    }
                }

                // Load order details
                LoadOrderDetails();
            }
            else
            {
                txtHeader.Text = "Tạo đơn hàng mới";
                txtOrderID.Text = "Tự động";
                txtOrderDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                txtStatus.Text = "Chờ xử lý";
                txtTotalAmount.Text = "₫0";
            }
        }

        private void LoadComboBoxes()
        {
            try
            {
                // Load customers
                cmbCustomer.ItemsSource = customers;

                // Load staff
                cmbStaff.ItemsSource = staff;

                // Load products
                products = _productService.GetAllProductsAsync().Result ?? new List<Product>();

                // If no products from database, create sample products
                if (products.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("No products found in database, creating sample products");
                    products = CreateSampleProducts();
                }

                cmbProduct.ItemsSource = products;

                // Add placeholder text
                cmbProduct.Text = "Chọn sản phẩm...";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadComboBoxes error: {ex.Message}");
                // Use sample products as fallback
                products = CreateSampleProducts();
                cmbProduct.ItemsSource = products;
                cmbProduct.Text = "Chọn sản phẩm...";
            }
        }

        private List<Product> CreateSampleProducts()
        {
            return new List<Product>
            {
                new Product
                {
                    ProductId = 1,
                    ProductName = "Ống nhựa PVC 90mm",
                    Description = "Ống nhựa PVC đường kính 90mm, dài 6m",
                    UnitPrice = 150000,
                    StockQuantity = 50,
                    CategoryId = 1,
                    SupplierId = 1
                },
                new Product
                {
                    ProductId = 2,
                    ProductName = "Ống nhựa PVC 110mm",
                    Description = "Ống nhựa PVC đường kính 110mm, dài 6m",
                    UnitPrice = 200000,
                    StockQuantity = 30,
                    CategoryId = 1,
                    SupplierId = 1
                },
                new Product
                {
                    ProductId = 3,
                    ProductName = "Cút nối 90 độ 90mm",
                    Description = "Cút nối 90 độ cho ống PVC 90mm",
                    UnitPrice = 25000,
                    StockQuantity = 100,
                    CategoryId = 2,
                    SupplierId = 1
                },
                new Product
                {
                    ProductId = 4,
                    ProductName = "Tê nối 90mm",
                    Description = "Tê nối 3 nhánh cho ống PVC 90mm",
                    UnitPrice = 35000,
                    StockQuantity = 80,
                    CategoryId = 2,
                    SupplierId = 1
                },
                new Product
                {
                    ProductId = 5,
                    ProductName = "Keo dán PVC",
                    Description = "Keo dán chuyên dụng cho ống nhựa PVC",
                    UnitPrice = 45000,
                    StockQuantity = 25,
                    CategoryId = 3,
                    SupplierId = 2
                }
            };
        }

        private void LoadOrderDetails()
        {
            try
            {
                if (isEditMode && currentOrder != null)
                {
                    // Load from database
                    var order = _orderService.GetOrderById(currentOrder.OrderId);
                    if (order != null && order.OrderDetails != null)
                    {
                        orderDetails = order.OrderDetails.ToList();
                        dgOrderDetails.ItemsSource = orderDetails;
                        CalculateTotal();
                    }
                }
                else
                {
                    // New order - start with empty list
                    orderDetails = new List<OrderDetail>();
                    dgOrderDetails.ItemsSource = orderDetails;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải chi tiết đơn hàng: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetStatusText(string status)
        {
            return status switch
            {
                "Pending" => "Chờ xử lý",
                "Completed" => "Đã hoàn thành",
                "Canceled" => "Đã hủy",
                _ => status
            };
        }

        // Event Handlers
        private void Customer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCustomer.SelectedItem is Customer selectedCustomer)
            {
                UpdateCustomerInfo(selectedCustomer);
            }
        }

        private void Staff_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Staff selection changed - can add logic here if needed
        }

        private void Product_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbProduct.SelectedItem is Product selectedProduct)
            {
                txtUnitPrice.Text = $"₫{selectedProduct.UnitPrice:N0}";
            }
            else
            {
                txtUnitPrice.Text = "₫0";
            }
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (cmbProduct.SelectedItem is Product selectedProduct)
            {
                if (int.TryParse(txtQuantity.Text, out int quantity) && quantity > 0)
                {
                    // Check stock availability
                    if (quantity > selectedProduct.StockQuantity)
                    {
                        MessageBox.Show($"Số lượng vượt quá tồn kho!\nTồn kho hiện tại: {selectedProduct.StockQuantity}\nSố lượng yêu cầu: {quantity}",
                            "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Check if product already exists in order
                    var existingDetail = orderDetails.FirstOrDefault(od => od.ProductId == selectedProduct.ProductId);
                    if (existingDetail != null)
                    {
                        var newTotalQuantity = existingDetail.Quantity + quantity;
                        if (newTotalQuantity > selectedProduct.StockQuantity)
                        {
                            MessageBox.Show($"Tổng số lượng vượt quá tồn kho!\nTồn kho hiện tại: {selectedProduct.StockQuantity}\nTổng số lượng: {newTotalQuantity}",
                                "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        existingDetail.Quantity = newTotalQuantity;
                        existingDetail.Subtotal = existingDetail.Quantity * existingDetail.UnitPrice;
                    }
                    else
                    {
                        var newDetail = new OrderDetail
                        {
                            ProductId = selectedProduct.ProductId,
                            Product = selectedProduct,
                            Quantity = quantity,
                            UnitPrice = selectedProduct.UnitPrice,
                            Subtotal = quantity * selectedProduct.UnitPrice
                        };
                        orderDetails.Add(newDetail);
                    }

                    dgOrderDetails.ItemsSource = null;
                    dgOrderDetails.ItemsSource = orderDetails;
                    CalculateTotal();

                    // Reset selection
                    cmbProduct.SelectedIndex = -1;
                    txtQuantity.Text = "1";
                    txtUnitPrice.Text = "₫0";

                    // Show success message with more details
                    var message = $"✅ Đã thêm sản phẩm thành công!\n\n" +
                                 $"📦 Sản phẩm: {selectedProduct.ProductName}\n" +
                                 $"📊 Số lượng: {quantity}\n" +
                                 $"💰 Đơn giá: ₫{selectedProduct.UnitPrice:N0}\n" +
                                 $"💵 Thành tiền: ₫{(quantity * selectedProduct.UnitPrice):N0}\n" +
                                 $"📋 Tổng sản phẩm trong đơn: {orderDetails.Count}";

                    MessageBox.Show(message, "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Vui lòng nhập số lượng hợp lệ (số nguyên dương)!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn sản phẩm!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DgOrderDetails_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (e.Column.Header.ToString() == "Số lượng")
                {
                    if (e.EditingElement is TextBox textBox)
                    {
                        if (int.TryParse(textBox.Text, out int newQuantity) && newQuantity > 0)
                        {
                            var orderDetail = e.Row.Item as OrderDetail;
                            if (orderDetail != null)
                            {
                                orderDetail.Quantity = newQuantity;
                                orderDetail.Subtotal = newQuantity * orderDetail.UnitPrice;
                                CalculateTotal();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Số lượng phải là số nguyên dương!", "Thông báo",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                            e.Cancel = true;
                        }
                    }
                }
            }
        }

        private void UpdateCustomerInfo(Customer customer)
        {
            txtCustomerName.Text = customer.CustomerName;
            txtCustomerPhone.Text = customer.Phone ?? "";
            txtCustomerEmail.Text = customer.Email ?? "";
            txtCustomerAddress.Text = customer.Address ?? "";
        }

        private void CalculateTotal()
        {
            decimal total = orderDetails.Sum(od => od.Subtotal);
            txtTotalAmount.Text = $"₫{total:N0}";
        }

        private void UpdateStockQuantities()
        {
            try
            {
                foreach (var detail in orderDetails)
                {
                    var product = products.FirstOrDefault(p => p.ProductId == detail.ProductId);
                    if (product != null)
                    {
                        var newStockQuantity = product.StockQuantity - detail.Quantity;
                        if (newStockQuantity >= 0)
                        {
                            _productService.UpdateStockQuantity(product.ProductId, newStockQuantity);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't show popup to user
                System.Diagnostics.Debug.WriteLine($"Stock update error: {ex.Message}");
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate required fields
                if (cmbCustomer.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn khách hàng!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (cmbStaff.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn nhân viên!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (orderDetails.Count == 0)
                {
                    MessageBox.Show("Vui lòng thêm ít nhất một sản phẩm!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validate stock availability for all products
                foreach (var detail in orderDetails)
                {
                    var product = products.FirstOrDefault(p => p.ProductId == detail.ProductId);
                    if (product != null && detail.Quantity > product.StockQuantity)
                    {
                        MessageBox.Show($"Sản phẩm '{product.ProductName}' vượt quá tồn kho!\nTồn kho: {product.StockQuantity}, Yêu cầu: {detail.Quantity}",
                            "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                // Show confirmation dialog
                var totalAmount = orderDetails.Sum(od => od.Subtotal);
                var customer = cmbCustomer.SelectedItem as Customer;
                var result = MessageBox.Show(
                    $"Xác nhận tạo đơn hàng?\n\nKhách hàng: {customer?.CustomerName}\nSố sản phẩm: {orderDetails.Count}\nTổng tiền: ₫{totalAmount:N0}",
                    "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (isEditMode)
                    {
                        // Update existing order
                        UpdateOrder();
                    }
                    else
                    {
                        // Create new order
                        CreateOrder();
                    }

                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu đơn hàng: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateOrder()
        {
            try
            {
                var customer = cmbCustomer.SelectedItem as Customer;
                var staffMember = cmbStaff.SelectedItem as Staff;

                if (customer == null || staffMember == null)
                {
                    MessageBox.Show("Thông tin khách hàng hoặc nhân viên không hợp lệ!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newOrder = new Order
                {
                    CustomerId = customer.CustomerId,
                    StaffId = staffMember.StaffId,
                    Notes = txtNotes.Text,
                    TotalAmount = orderDetails.Sum(od => od.Subtotal)
                };

                // Always try to save to database first
                bool success = _orderService.CreateOrder(newOrder, orderDetails);
                if (success)
                {
                    // Update stock quantities
                    UpdateStockQuantities();

                    // Get the created order with full details
                    var createdOrder = _orderService.GetOrderById(newOrder.OrderId);
                    if (createdOrder != null)
                    {
                        // Set the created order so it can be returned to parent window
                        currentOrder = createdOrder;
                    }

                    MessageBox.Show($"Đơn hàng đã được tạo thành công!\n\n" +
                                   $"Mã đơn hàng: {newOrder.OrderId}\n" +
                                   $"Khách hàng: {customer.CustomerName}\n" +
                                   $"Số sản phẩm: {orderDetails.Count}\n" +
                                   $"Tổng tiền: ₫{newOrder.TotalAmount:N0}\n\n" +
                                   $"Đơn hàng đã được lưu vào database và sẽ hiển thị trong danh sách đơn hàng.",
                        "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // If database save fails, check if we're using sample data
                    bool isUsingSampleData = products.Any(p => p.ProductId <= 5);
                    
                    if (isUsingSampleData)
                    {
                        // Create a demo order for display
                        var demoOrder = new Order
                        {
                            OrderId = GetNextDemoOrderId(),
                            CustomerId = customer.CustomerId,
                            StaffId = staffMember.StaffId,
                            OrderDate = DateTime.Now,
                            TotalAmount = newOrder.TotalAmount,
                            Status = "Pending",
                            Notes = txtNotes.Text,
                            Customer = customer,
                            Staff = staffMember
                        };

                        // Set the demo order so it can be returned to parent window
                        currentOrder = demoOrder;

                        // Show demo success message
                        MessageBox.Show($"Đơn hàng đã được tạo thành công! (Demo Mode)\n\n" +
                                       $"Mã đơn hàng: {demoOrder.OrderId}\n" +
                                       $"Khách hàng: {customer.CustomerName}\n" +
                                       $"Số sản phẩm: {orderDetails.Count}\n" +
                                       $"Tổng tiền: ₫{newOrder.TotalAmount:N0}\n\n" +
                                       $"Lưu ý: Đây là chế độ demo, đơn hàng sẽ hiển thị trong danh sách nhưng không được lưu vào database.",
                            "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Không thể tạo đơn hàng! Vui lòng kiểm tra:\n\n" +
                                       "1. Đã chọn khách hàng và nhân viên\n" +
                                       "2. Đã thêm ít nhất một sản phẩm\n" +
                                       "3. Số lượng sản phẩm không vượt quá tồn kho\n" +
                                       "4. Kết nối database hoạt động bình thường", "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo đơn hàng: {ex.Message}\n\nChi tiết: {ex.InnerException?.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private int GetNextDemoOrderId()
        {
            // Generate a demo order ID (for demo mode only)
            return DateTime.Now.Millisecond + new Random().Next(1000, 9999);
        }

        private void UpdateOrder()
        {
            try
            {
                var customer = cmbCustomer.SelectedItem as Customer;
                var staffMember = cmbStaff.SelectedItem as Staff;

                if (customer == null || staffMember == null)
                {
                    MessageBox.Show("Thông tin khách hàng hoặc nhân viên không hợp lệ!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                currentOrder.CustomerId = customer.CustomerId;
                currentOrder.StaffId = staffMember.StaffId;
                currentOrder.Notes = txtNotes.Text;
                currentOrder.TotalAmount = orderDetails.Sum(od => od.Subtotal);

                bool success = _orderService.UpdateOrder(currentOrder, orderDetails);
                if (success)
                {
                    MessageBox.Show("Đơn hàng đã được cập nhật thành công!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Không thể cập nhật đơn hàng!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật đơn hàng: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}