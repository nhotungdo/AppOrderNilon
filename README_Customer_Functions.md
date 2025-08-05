# Customer Dashboard Functions - AppOrderNilon

## Tổng quan
Đã thêm các chức năng hoàn chỉnh cho Customer Dashboard, thay thế các placeholder MessageBox bằng các window quản lý thực tế.

## Các chức năng đã implement

### 1. **Đơn hàng của tôi** (My Orders)
- **Window**: `CustomerOrderManagementWindow`
- **Chức năng**:
  - Hiển thị danh sách đơn hàng của khách hàng
  - Tìm kiếm đơn hàng theo mã hoặc trạng thái
  - Lọc đơn hàng theo trạng thái (Chờ xử lý, Đang xử lý, Đã giao, Hoàn thành, Đã hủy)
  - Xem chi tiết đơn hàng
  - Hủy đơn hàng (chỉ cho đơn hàng chưa xử lý hoặc đang xử lý)
  - Làm mới dữ liệu

### 2. **Đặt hàng mới** (Place New Order)
- **Window**: `CreateOrderWindow` (sử dụng window có sẵn)
- **Chức năng**:
  - Tạo đơn hàng mới
  - Chọn sản phẩm và số lượng
  - Tính toán tổng tiền
  - Xác nhận và lưu đơn hàng
  - Refresh dữ liệu sau khi đặt hàng thành công

### 3. **Hồ sơ** (Profile)
- **Window**: `CustomerProfileWindow`
- **Chức năng**:
  - Xem và chỉnh sửa thông tin cá nhân
  - Cập nhật họ tên, số điện thoại, email, địa chỉ
  - Thay đổi mật khẩu (tùy chọn)
  - Validation form (kiểm tra định dạng email, số điện thoại)
  - Hash mật khẩu khi thay đổi

### 4. **Ưu đãi** (Promotions/Offers)
- **Window**: `CustomerPromotionWindow`
- **Chức năng**:
  - Hiển thị điểm thưởng hiện tại
  - Thống kê điểm đã sử dụng và tổng điểm tích lũy
  - Hiển thị thông tin đổi điểm (1,000 điểm = 50,000 VNĐ)
  - Danh sách các ưu đãi hiện có với:
    - Tên và mô tả ưu đãi
    - Phần trăm giảm giá
    - Ngày hết hạn
    - Trạng thái hoạt động

### 5. **Hỗ trợ** (Support)
- **Window**: `CustomerSupportWindow`
- **Chức năng**:
  - Form liên hệ hỗ trợ với:
    - Chọn chủ đề (Vấn đề đơn hàng, Sản phẩm, Thanh toán, Tài khoản, Đề xuất, Khác)
    - Mô tả vấn đề (tối thiểu 10 ký tự)
    - Số điện thoại và email liên hệ
    - Validation form
  - FAQ (Câu hỏi thường gặp) với các chủ đề:
    - Cách đặt hàng
    - Thời gian giao hàng
    - Chính sách hủy đơn hàng
    - Phương thức thanh toán
    - Cách tích điểm thưởng
    - Chính sách đổi trả

## Cấu trúc file

### XAML Files
- `CustomerOrderManagementWindow.xaml` - Giao diện quản lý đơn hàng
- `CustomerProfileWindow.xaml` - Giao diện hồ sơ khách hàng
- `CustomerPromotionWindow.xaml` - Giao diện ưu đãi và điểm thưởng
- `CustomerSupportWindow.xaml` - Giao diện hỗ trợ khách hàng

### Code-behind Files
- `CustomerOrderManagementWindow.xaml.cs` - Logic quản lý đơn hàng
- `CustomerProfileWindow.xaml.cs` - Logic quản lý hồ sơ
- `CustomerPromotionWindow.xaml.cs` - Logic hiển thị ưu đãi
- `CustomerSupportWindow.xaml.cs` - Logic gửi yêu cầu hỗ trợ

## Tính năng kỹ thuật

### Error Handling
- Try-catch blocks cho tất cả các window
- MessageBox thông báo lỗi chi tiết
- Validation form với thông báo lỗi cụ thể

### UI/UX
- Giao diện hiện đại với Material Design colors
- Responsive layout
- Placeholder text cho các input field
- Loading states và feedback cho user

### Data Management
- Sample data cho demo
- Observable collections cho real-time updates
- LINQ queries cho filtering và searching
- Proper data binding

### Security
- Password hashing (SHA256)
- Input validation
- Null safety checks

## Cách sử dụng

1. **Đăng nhập** vào hệ thống với tài khoản khách hàng
2. **Chọn menu item** từ Customer Dashboard
3. **Thực hiện các thao tác** trong window tương ứng
4. **Lưu hoặc hủy** thay đổi
5. **Quay lại** dashboard chính

## Lưu ý

- Tất cả dữ liệu hiện tại là sample data
- Trong production, cần kết nối với database thực
- Có thể cần thêm logging và monitoring
- Nên implement caching cho performance
- Có thể thêm email notifications cho các thao tác quan trọng

## Build Status
✅ **Build thành công** - Không có lỗi compilation
⚠️ **Warnings** - Chỉ có warnings về nullable reference types (không ảnh hưởng chức năng) 