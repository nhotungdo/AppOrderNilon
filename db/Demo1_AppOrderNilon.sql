-- Tạo cơ sở dữ liệu
CREATE DATABASE AppOrderNilon;
GO

USE AppOrderNilon;
GO

-- Tạo bảng Admins
CREATE TABLE Admins (
    AdminID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100),
    Email NVARCHAR(100),
    Phone NVARCHAR(20)
);
GO

-- Tạo bảng Staff
CREATE TABLE Staff (
    StaffID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100),
    Email NVARCHAR(100),
    Phone NVARCHAR(20)
);
GO

-- Tạo bảng Customers
CREATE TABLE Customers (
   CustomerID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    CustomerName NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20),
    Email NVARCHAR(100),
    Address NVARCHAR(255),
    Notes NVARCHAR(500)
);
GO

-- Tạo bảng Categories
CREATE TABLE Categories (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255),
    Quantity INT NOT NULL DEFAULT 0 -- Tổng số lượng sản phẩm trong danh mục
);
GO

-- Tạo bảng Suppliers
CREATE TABLE Suppliers (
    SupplierID INT PRIMARY KEY IDENTITY(1,1),
    SupplierName NVARCHAR(100) NOT NULL,
    ContactName NVARCHAR(100),
    Phone NVARCHAR(20),
    Email NVARCHAR(100),
    Address NVARCHAR(255)
);
GO

-- Tạo bảng Products
CREATE TABLE Products (
    ProductID INT PRIMARY KEY IDENTITY(1,1),
    ProductName NVARCHAR(100) NOT NULL,
    CategoryID INT,
    SupplierID INT,
    Description NVARCHAR(255),
    Thickness DECIMAL(5,2), -- Độ dày (dành cho nilon, tính bằng mm)
    Size NVARCHAR(50), -- Kích thước (ví dụ: 2m x 100m)
    UnitPrice DECIMAL(18,2) NOT NULL,
    StockQuantity INT NOT NULL DEFAULT 0,
    ImagePath NVARCHAR(255), -- Đường dẫn hình ảnh sản phẩm
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID),
    FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID)
);
GO

-- Tạo bảng Orders
CREATE TABLE Orders (
    OrderID INT PRIMARY KEY IDENTITY(1,1),
    CustomerID INT,
    StaffID INT,
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    TotalAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    Status NVARCHAR(50) NOT NULL, -- Pending, Completed, Canceled
    Notes NVARCHAR(500),
	 CustomerName NVARCHAR(255) NULL,
    FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID),
    FOREIGN KEY (StaffID) REFERENCES Staff(StaffID)
);
GO

-- Tạo bảng OrderDetails
CREATE TABLE OrderDetails (
    OrderDetailID INT PRIMARY KEY IDENTITY(1,1),
    OrderID INT,
    ProductID INT,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    Subtotal DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);
GO

-- Tạo bảng Reports
CREATE TABLE Reports (
    ReportID INT PRIMARY KEY IDENTITY(1,1),
    ReportType NVARCHAR(50) NOT NULL, -- Revenue, Inventory, BestSelling
    StartDate DATETIME,
    EndDate DATETIME,
    GeneratedDate DATETIME NOT NULL DEFAULT GETDATE(),
    Data NVARCHAR(MAX), -- Dữ liệu báo cáo dạng JSON hoặc text
    AdminID INT,
    FOREIGN KEY (AdminID) REFERENCES Admins(AdminID)
);
GO

-- Tạo Trigger để tự động cập nhật Quantity trong Categories khi StockQuantity trong Products thay đổi
CREATE TRIGGER UpdateCategoryQuantity
ON Products
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    -- Cập nhật Quantity cho tất cả danh mục
    UPDATE Categories
    SET Quantity = ISNULL((
        SELECT SUM(StockQuantity)
        FROM Products
        WHERE Products.CategoryID = Categories.CategoryID
    ), 0);
END;
GO

-- Tạo các chỉ mục để tối ưu hóa truy vấn
CREATE INDEX IX_Products_CategoryID ON Products(CategoryID);
CREATE INDEX IX_Products_SupplierID ON Products(SupplierID);
CREATE INDEX IX_Orders_CustomerID ON Orders(CustomerID);
CREATE INDEX IX_Orders_StaffID ON Orders(StaffID);
CREATE INDEX IX_Orders_OrderDate ON Orders(OrderDate);
CREATE INDEX IX_OrderDetails_OrderID ON OrderDetails(OrderID);
CREATE INDEX IX_OrderDetails_ProductID ON OrderDetails(ProductID);
CREATE INDEX IX_Reports_AdminID ON Reports(AdminID);
GO

-- Chèn dữ liệu mẫu vào bảng Admins
INSERT INTO Admins (Username, PasswordHash, FullName, Email, Phone)
VALUES 
    ('admin1', '123456', N'Nguyễn Quản Trị', N'admin1@app.com', '0909876543'),
    ('admin2', '123456', N'Lê Quản Trị', N'admin2@app.com', '0908765432');
GO

-- Chèn dữ liệu mẫu vào bảng Staff
INSERT INTO Staff (Username, PasswordHash, FullName, Email, Phone)
VALUES 
    ('staff1', N'123456', N'Trần Nhân Viên', N'staff1@app.com', '0918765432'),
    ('staff2', N'123456', N'Phạm Nhân Viên', N'staff2@app.com', '0917654321');
GO

-- Chèn dữ liệu mẫu vào bảng Customers
INSERT INTO Customers (CustomerName, Phone, Email, Address, Notes)
VALUES 
    (N'Công ty Xây dựng Minh Anh', '0987654321', 'minhanh@construction.com', N'789 Đường Láng, Hà Nội', N'Khách hàng VIP'),
    (N'Cá nhân Lê Văn C', '0971234567', 'levanc@gmail.com', N'123 Đường Nguyễn Trãi, Hà Nội', '');
GO

-- Chèn dữ liệu mẫu vào bảng Categories
INSERT INTO Categories (CategoryName, Description, Quantity)
VALUES 
    (N'Nilon xây dựng', N'Nilon lót sàn, bạt phủ công trình', 0),
    (N'Vật liệu bảo hộ', N'Mũ, găng tay, giày bảo hộ', 0),
    (N'Vật liệu xây dựng', N'Xi măng, gạch, thép', 0);
GO

-- Chèn dữ liệu mẫu vào bảng Suppliers
INSERT INTO Suppliers (SupplierName, ContactName, Phone, Email, Address)
VALUES 
    (N'Công ty Nilon ABC', N'Nguyễn Văn A', '0901234567', 'abc@nilon.com', N'123 Đường Láng, Hà Nội'),
    (N'Công ty Bảo Hộ XYZ', N'Trần Thị B', '0912345678', 'xyz@baoho.com', N'456 Đường Giải Phóng, Hà Nội');
GO

-- Chèn dữ liệu mẫu vào bảng Products
INSERT INTO Products (ProductName, CategoryID, SupplierID, Description, Thickness, Size, UnitPrice, StockQuantity, ImagePath)
VALUES 
    (N'Nilon lót sàn 0.2mm', 1, 1, N'Nilon lót sàn xây dựng', 0.2, '2m x 100m', 50000, 100, '/images/nilon.jpg'),
    (N'Mũ bảo hộ ABS', 2, 2, N'Mũ bảo hộ chất lượng cao', NULL, 'One size', 150000, 50, '/images/helmet.jpg'),
    (N'Găng tay cao su', 2, 2, N'Găng tay bảo hộ chống hóa chất', NULL, 'L', 30000, 200, '/images/gloves.jpg');
GO

-- Cập nhật Quantity trong Categories sau khi thêm Products
UPDATE Categories
SET Quantity = ISNULL((
    SELECT SUM(StockQuantity)
    FROM Products
    WHERE Products.CategoryID = Categories.CategoryID
), 0);
GO

-- Chèn dữ liệu mẫu vào bảng Orders
INSERT INTO Orders (CustomerID, StaffID, OrderDate, TotalAmount, Status, Notes)
VALUES 
    (1, 1, '2025-08-01', 250000, 'Completed', N'Giao hàng nhanh'),
    (2, 2, '2025-08-02', 180000, 'Pending', '');
GO

-- Chèn dữ liệu mẫu vào bảng OrderDetails
INSERT INTO OrderDetails (OrderID, ProductID, Quantity, UnitPrice, Subtotal)
VALUES 
    (1, 1, 2, 50000, 100000),
    (1, 2, 1, 150000, 150000),
    (2, 3, 6, 30000, 180000);
GO

-- Chèn dữ liệu mẫu vào bảng Reports
INSERT INTO Reports (ReportType, StartDate, EndDate, Data, AdminID)
VALUES 
    ('Revenue', '2025-01-01', '2025-12-31', '{"total": 5000000}', 1),
    ('Inventory', '2025-08-01', '2025-08-31', '{"stock": 350}', 2);
GO

