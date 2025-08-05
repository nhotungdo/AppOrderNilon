# Sửa lỗi CreateOrderWindow - Customer không cần chọn khách hàng

## Vấn đề ban đầu
Trong cửa sổ "Tạo đơn hàng mới", customer phải chọn khách hàng từ dropdown, điều này không hợp lý vì customer chính là khách hàng đang đăng nhập.

## Giải pháp đã thực hiện

### 1. Sửa constructor của CreateOrderWindow
```csharp
// Trước
public CreateOrderWindow()
{
    InitializeComponent();
    InitializeData();
}

// Sau
public CreateOrderWindow(Customer currentCustomer = null)
{
    InitializeComponent();
    _currentCustomer = currentCustomer;
    InitializeData();
}
```

### 2. Thêm field để lưu customer hiện tại
```csharp
private Customer _currentCustomer;
```

### 3. Sửa method LoadCustomers()
```csharp
private void LoadCustomers()
{
    if (_currentCustomer != null)
    {
        // Nếu có customer hiện tại, chỉ hiển thị customer đó
        _customers = new ObservableCollection<Customer> { _currentCustomer };
        cboCustomer.ItemsSource = _customers;
        cboCustomer.SelectedItem = _currentCustomer;
        cboCustomer.IsEnabled = false; // Vô hiệu hóa selection
        
        // Ẩn nút "Thêm khách hàng mới"
        btnAddCustomer.Visibility = Visibility.Collapsed;
        
        // Tự động điền địa chỉ giao hàng
        txtDeliveryAddress.Text = _currentCustomer.Address ?? "";
    }
    else
    {
        // Giữ nguyên logic cũ cho trường hợp không có customer
        // (dành cho staff hoặc admin)
    }
}
```

### 4. Sửa CustomerDashboardWindow để truyền customer hiện tại
```csharp
// Trong CustomerDashboardWindow.xaml.cs
private void PlaceOrder_Click(object sender, RoutedEventArgs e)
{
    try
    {
        CreateOrderWindow orderWindow = new CreateOrderWindow(currentCustomer);
        if (orderWindow.ShowDialog() == true)
        {
            LoadData();
            MessageBox.Show("Đặt hàng thành công!", "Thông báo");
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Lỗi khi đặt hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
```

## Kết quả

### Khi customer đăng nhập và tạo đơn hàng:
- ✅ **Không cần chọn khách hàng** - Tự động hiển thị thông tin customer hiện tại
- ✅ **ComboBox bị vô hiệu hóa** - Không thể thay đổi khách hàng
- ✅ **Nút "Thêm khách hàng mới" bị ẩn** - Không cần thiết
- ✅ **Tự động điền địa chỉ giao hàng** - Từ thông tin customer hiện tại

### Khi staff/admin sử dụng (không truyền customer):
- ✅ **Vẫn hoạt động bình thường** - Có thể chọn khách hàng từ danh sách
- ✅ **Có thể thêm khách hàng mới** - Nút "Thêm khách hàng mới" vẫn hiển thị

## Lợi ích
1. **UX tốt hơn** - Customer không cần thao tác không cần thiết
2. **Giảm lỗi** - Không thể chọn nhầm khách hàng khác
3. **Tự động hóa** - Thông tin được điền sẵn
4. **Tương thích ngược** - Staff/admin vẫn sử dụng được bình thường

## Files đã sửa
- `AppOrderNilon/Views/CreateOrderWindow.xaml.cs`
- `AppOrderNilon/Views/CustomerDashboardWindow.xaml.cs` 