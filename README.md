# AppOrderNilon - Hệ thống Quản lý Đơn hàng Nilon

## Mô tả
AppOrderNilon là một ứng dụng WPF được thiết kế để quản lý đơn hàng và kinh doanh trong lĩnh vực nilon và vật liệu xây dựng. Ứng dụng cung cấp giao diện thân thiện và các tính năng quản lý toàn diện.

## Tính năng chính

### 1. Đăng nhập & Đăng ký
- **Đăng nhập**: Giao diện đơn giản với tên đăng nhập và mật khẩu
- **Đăng ký**: Form đăng ký với validation đầy đủ
- **Quên mật khẩu**: Chức năng khôi phục tài khoản

### 2. Dashboard (Trang chủ)
- **Thống kê tổng quan**: Doanh thu, đơn hàng, sản phẩm bán được
- **Biểu đồ trực quan**: Hiển thị dữ liệu kinh doanh
- **Thông báo & Cảnh báo**: Đơn hàng mới, tồn kho thấp
- **Thao tác nhanh**: Truy cập nhanh đến các chức năng chính

### 3. Quản lý Sản phẩm
- **Danh sách sản phẩm**: Hiển thị đầy đủ thông tin sản phẩm
- **Tìm kiếm & Lọc**: Theo danh mục, giá, tồn kho
- **Thêm/Sửa/Xóa**: Quản lý thông tin sản phẩm
- **Cảnh báo tồn kho**: Hiển thị sản phẩm sắp hết

### 4. Quản lý Đơn hàng
- **Danh sách đơn hàng**: Theo dõi tất cả đơn hàng
- **Tìm kiếm & Lọc**: Theo trạng thái, ngày, khách hàng
- **Tạo & Chỉnh sửa**: Quản lý đơn hàng chi tiết
- **Trạng thái đơn hàng**: Chờ xử lý, Đã hoàn thành, Đã hủy

### 5. Quản lý Khách hàng
- **Danh sách khách hàng**: Thông tin chi tiết khách hàng
- **Phân loại**: Khách hàng VIP và thường
- **Thêm/Sửa/Xóa**: Quản lý thông tin khách hàng

### 6. Báo cáo & Thống kê
- **Báo cáo doanh thu**: Theo thời gian, chu kỳ
- **Sản phẩm bán chạy**: Top sản phẩm được ưa chuộng
- **Báo cáo tồn kho**: Tình trạng tồn kho
- **Báo cáo khách hàng**: Phân tích khách hàng

## Cấu trúc dự án

```
AppOrderNilon/
├── Models/                 # Các model dữ liệu
│   ├── Admin.cs
│   ├── Category.cs
│   ├── Customer.cs
│   ├── Order.cs
│   ├── OrderDetail.cs
│   ├── Product.cs
│   ├── Report.cs
│   ├── Staff.cs
│   ├── Supplier.cs
│   └── AppOrderNilonContext.cs
├── Views/                  # Giao diện người dùng
│   ├── LoginWindow.xaml
│   ├── RegisterWindow.xaml
│   ├── DashboardWindow.xaml
│   ├── ProductManagementWindow.xaml
│   ├── ProductDetailWindow.xaml
│   ├── OrderManagementWindow.xaml
│   ├── OrderDetailWindow.xaml
│   ├── CustomerManagementWindow.xaml
│   ├── CustomerDetailWindow.xaml
│   └── ReportWindow.xaml
├── db/                     # Cơ sở dữ liệu
│   └── Demo1_AppOrderNilon.sql
├── App.xaml               # Entry point
└── README.md
```

## Cài đặt và Chạy

### Yêu cầu hệ thống
- Windows 10/11
- .NET 8.0 Runtime
- SQL Server (để kết nối database)

### Cài đặt
1. Clone hoặc download dự án
2. Mở file `AppOrderNilon.sln` trong Visual Studio
3. Restore NuGet packages
4. Build và chạy dự án

### Cấu hình Database
1. Chạy script SQL trong file `db/Demo1_AppOrderNilon.sql`
2. Cập nhật connection string trong `appsettings.json`
3. Chạy Entity Framework migrations (nếu cần)

## Sử dụng

### Đăng nhập
- Mở ứng dụng
- Nhập tên đăng nhập và mật khẩu
- Nhấn "Đăng nhập"

### Quản lý sản phẩm
- Từ Dashboard, nhấn "Quản lý sản phẩm"
- Sử dụng tìm kiếm và lọc để tìm sản phẩm
- Nhấn "Thêm sản phẩm" để tạo mới
- Nhấn "Sửa" hoặc "Xóa" để quản lý

### Quản lý đơn hàng
- Từ Dashboard, nhấn "Quản lý đơn hàng"
- Nhấn "Tạo đơn hàng" để tạo mới
- Xem chi tiết đơn hàng bằng nút "Xem"
- Cập nhật trạng thái đơn hàng

### Báo cáo
- Từ Dashboard, nhấn "Báo cáo & Thống kê"
- Chọn loại báo cáo và khoảng thời gian
- Nhấn "Tạo báo cáo" để xem kết quả

## Tính năng nâng cao

### Validation
- Kiểm tra dữ liệu đầu vào
- Hiển thị thông báo lỗi rõ ràng
- Ngăn chặn dữ liệu không hợp lệ

### Responsive Design
- Giao diện thích ứng với kích thước màn hình
- Layout tối ưu cho các thiết bị khác nhau

### Performance
- Lazy loading dữ liệu
- Tối ưu hóa truy vấn database
- Caching dữ liệu thường dùng

## Phát triển

### Công nghệ sử dụng
- **Frontend**: WPF (Windows Presentation Foundation)
- **Backend**: C# .NET 8.0
- **Database**: SQL Server với Entity Framework
- **Architecture**: MVVM Pattern

### Mở rộng tính năng
- Thêm biểu đồ nâng cao với LiveCharts
- Tích hợp báo cáo PDF/Excel
- Thêm tính năng backup/restore
- Tích hợp thanh toán online

## Hỗ trợ

Nếu gặp vấn đề hoặc cần hỗ trợ, vui lòng:
1. Kiểm tra log lỗi
2. Đảm bảo database đã được cấu hình đúng
3. Kiểm tra connection string
4. Liên hệ team phát triển

## Phiên bản

- **Version**: 1.0.0
- **Release Date**: 2025
- **Author**: AppOrderNilon Team 