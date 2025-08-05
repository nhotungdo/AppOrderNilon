# Hướng dẫn sử dụng các chức năng Staff Dashboard

## Tổng quan
Đã thêm các chức năng hoàn chỉnh cho Staff Dashboard với 5 menu chính:
1. **📋 Đơn hàng** - Quản lý đơn hàng
2. **✅ Nhiệm vụ** - Quản lý nhiệm vụ
3. **📦 Tồn kho** - Quản lý tồn kho
4. **👥 Khách hàng** - Quản lý khách hàng
5. **📊 Báo cáo** - Xem báo cáo

## Chi tiết các chức năng

### 1. Quản lý đơn hàng (StaffOrderManagementWindow)
**Chức năng:**
- Xem danh sách đơn hàng được giao
- Tìm kiếm đơn hàng theo mã, khách hàng, trạng thái
- Lọc đơn hàng theo trạng thái (Chờ xử lý, Đang xử lý, Đã giao, Hoàn thành)
- Xử lý đơn hàng (cập nhật trạng thái)
- Xem chi tiết đơn hàng

**Cách sử dụng:**
1. Click vào menu "📋 Đơn hàng"
2. Sử dụng ô tìm kiếm để tìm đơn hàng
3. Chọn trạng thái để lọc
4. Click "Xử lý" để cập nhật trạng thái đơn hàng
5. Click "Chi tiết" để xem thông tin chi tiết

### 2. Quản lý nhiệm vụ (StaffTaskManagementWindow)
**Chức năng:**
- Xem danh sách nhiệm vụ được giao
- Thêm nhiệm vụ mới
- Cập nhật trạng thái nhiệm vụ
- Xóa nhiệm vụ
- Tìm kiếm và lọc nhiệm vụ

**Cách sử dụng:**
1. Click vào menu "✅ Nhiệm vụ"
2. Click "+ Thêm nhiệm vụ" để tạo nhiệm vụ mới
3. Điền thông tin: tên, mô tả, hạn, ưu tiên, trạng thái
4. Click "Cập nhật" để sửa nhiệm vụ
5. Click "Xóa" để xóa nhiệm vụ

### 3. Quản lý tồn kho (StaffInventoryManagementWindow)
**Chức năng:**
- Xem danh sách sản phẩm và số lượng tồn kho
- Tìm kiếm sản phẩm
- Lọc theo trạng thái tồn kho (Đủ, Sắp hết, Hết hàng)
- Yêu cầu nhập hàng
- Xem chi tiết sản phẩm

**Cách sử dụng:**
1. Click vào menu "📦 Tồn kho"
2. Sử dụng ô tìm kiếm để tìm sản phẩm
3. Chọn trạng thái để lọc
4. Click "Yêu cầu nhập" để gửi yêu cầu nhập hàng
5. Click "Chi tiết" để xem thông tin sản phẩm

### 4. Quản lý khách hàng (StaffCustomerManagementWindow)
**Chức năng:**
- Xem danh sách khách hàng
- Tìm kiếm khách hàng theo tên, số điện thoại, email
- Lọc theo loại khách hàng (Cá nhân, Doanh nghiệp, VIP)
- Xem chi tiết khách hàng
- Liên hệ khách hàng

**Cách sử dụng:**
1. Click vào menu "👥 Khách hàng"
2. Sử dụng ô tìm kiếm để tìm khách hàng
3. Chọn loại khách hàng để lọc
4. Click "Chi tiết" để xem thông tin khách hàng
5. Click "Liên hệ" để ghi nhận yêu cầu liên hệ

### 5. Báo cáo (StaffReportWindow)
**Chức năng:**
- Tạo báo cáo đơn hàng
- Tạo báo cáo nhiệm vụ
- Tạo báo cáo tồn kho
- Tạo báo cáo hiệu suất
- Lọc theo thời gian (Hôm nay, Tuần này, Tháng này, Quý này, Năm nay)

**Cách sử dụng:**
1. Click vào menu "📊 Báo cáo"
2. Chọn loại báo cáo từ dropdown
3. Chọn khoảng thời gian
4. Click "Tạo báo cáo"
5. Xem kết quả báo cáo với thống kê và bảng dữ liệu

## Các window phụ trợ

### StaffTaskFormWindow
- Form thêm/sửa nhiệm vụ
- Validation đầy đủ
- Giao diện thân thiện

### StaffRestockRequestWindow
- Form yêu cầu nhập hàng
- Chọn sản phẩm và số lượng
- Đặt mức ưu tiên và ghi chú

## Tính năng đặc biệt

### Dashboard chính
- **Thống kê nhanh**: Hiển thị số đơn hàng hôm nay, nhiệm vụ hoàn thành, sản phẩm sắp hết
- **Tổng quan đơn hàng**: Biểu đồ trạng thái đơn hàng với số liệu thực tế
- **Nhiệm vụ được giao**: Bảng nhiệm vụ với khả năng cập nhật trạng thái
- **Giám sát tồn kho**: Bảng sản phẩm với cảnh báo sắp hết
- **Hiệu suất và thông báo**: KPI và thông báo real-time

### Giao diện
- Thiết kế hiện đại với màu cam chủ đạo (#FF6B35)
- Responsive và user-friendly
- Icons trực quan cho từng chức năng
- Validation và thông báo lỗi rõ ràng

## Lưu ý kỹ thuật

### Dữ liệu mẫu
- Hiện tại sử dụng dữ liệu mẫu để demo
- Có thể kết nối với database thực tế
- Các TODO comments chỉ ra vị trí cần tích hợp database

### Models đã cập nhật
- **Customer**: Thêm thuộc tính `CustomerType`
- **Product**: Đã có thuộc tính `StockStatus` (computed)
- **StaffTask**: Model mới cho quản lý nhiệm vụ
- **RestockRequest**: Model mới cho yêu cầu nhập hàng

### Error Handling
- Try-catch blocks cho tất cả operations
- MessageBox thông báo lỗi chi tiết
- Validation form đầy đủ

## Hướng dẫn phát triển tiếp

### Kết nối Database
1. Thay thế các LoadSampleData() bằng database calls
2. Implement các service classes
3. Sử dụng Entity Framework cho data access

### Thêm tính năng
1. Export báo cáo ra PDF/Excel
2. Thêm biểu đồ thống kê
3. Implement real-time notifications
4. Thêm tính năng chat với khách hàng

### Tối ưu hóa
1. Implement caching cho dữ liệu
2. Thêm pagination cho các bảng lớn
3. Optimize queries
4. Thêm logging và monitoring 