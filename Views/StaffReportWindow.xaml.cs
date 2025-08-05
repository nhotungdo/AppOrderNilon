using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using AppOrderNilon.Models;

namespace AppOrderNilon.Views
{
    public partial class StaffReportWindow : Window
    {
        private Staff currentStaff;
        private List<Order> sampleOrders;
        private List<StaffTask> sampleTasks;
        private List<Product> sampleProducts;

        public StaffReportWindow(Staff staff)
        {
            InitializeComponent();
            currentStaff = staff;
            LoadSampleData();
            InitializeControls();
        }

        private void LoadSampleData()
        {
            // Load sample data for reports
            LoadSampleOrders();
            LoadSampleTasks();
            LoadSampleProducts();
        }

        private void LoadSampleOrders()
        {
            sampleOrders = new List<Order>
            {
                new Order { OrderId = 123, CustomerId = 1, OrderDate = DateTime.Now.AddDays(-2), TotalAmount = 250000, Status = "Completed" },
                new Order { OrderId = 124, CustomerId = 2, OrderDate = DateTime.Now.AddDays(-1), TotalAmount = 180000, Status = "Shipped" },
                new Order { OrderId = 125, CustomerId = 3, OrderDate = DateTime.Now, TotalAmount = 320000, Status = "Processing" },
                new Order { OrderId = 126, CustomerId = 1, OrderDate = DateTime.Now.AddDays(-3), TotalAmount = 450000, Status = "Completed" }
            };
        }

        private void LoadSampleTasks()
        {
            sampleTasks = new List<StaffTask>
            {
                new StaffTask { TaskId = 1, TaskName = "Xử lý đơn hàng #123", Status = "Hoàn thành", DueDate = DateTime.Now.AddDays(-1) },
                new StaffTask { TaskId = 2, TaskName = "Kiểm tra tồn kho", Status = "Đang thực hiện", DueDate = DateTime.Now.AddDays(1) },
                new StaffTask { TaskId = 3, TaskName = "Liên hệ khách hàng", Status = "Hoàn thành", DueDate = DateTime.Now.AddHours(-2) },
                new StaffTask { TaskId = 4, TaskName = "Cập nhật báo cáo", Status = "Chờ thực hiện", DueDate = DateTime.Now.AddDays(2) }
            };
        }

        private void LoadSampleProducts()
        {
            sampleProducts = new List<Product>
            {
                new Product { ProductId = 1, ProductName = "Nilon lót sàn 0.2mm", StockQuantity = 100, UnitPrice = 50000 },
                new Product { ProductId = 2, ProductName = "Mũ bảo hộ ABS", StockQuantity = 5, UnitPrice = 150000 },
                new Product { ProductId = 3, ProductName = "Găng tay cao su", StockQuantity = 200, UnitPrice = 30000 },
                new Product { ProductId = 4, ProductName = "Kính bảo hộ", StockQuantity = 0, UnitPrice = 80000 }
            };
        }

        private void InitializeControls()
        {
            txtStaffName.Text = currentStaff?.FullName ?? "Nhân viên";
            cmbReportType.SelectedIndex = 0;
            cmbTimeRange.SelectedIndex = 0;
        }

        // Event Handlers
        private void ReportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateReportTitle();
        }

        private void TimeRange_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle time range change if needed
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var reportType = (cmbReportType.SelectedItem as ComboBoxItem)?.Content.ToString();
                var timeRange = (cmbTimeRange.SelectedItem as ComboBoxItem)?.Content.ToString();

                if (string.IsNullOrEmpty(reportType) || string.IsNullOrEmpty(timeRange))
                {
                    MessageBox.Show("Vui lòng chọn loại báo cáo và thời gian!", "Thông báo");
                    return;
                }

                GenerateReport(reportType, timeRange);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo báo cáo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateReportTitle()
        {
            var reportType = (cmbReportType.SelectedItem as ComboBoxItem)?.Content.ToString();
            txtReportTitle.Text = reportType ?? "Báo cáo";
        }

        private void GenerateReport(string reportType, string timeRange)
        {
            spReportContent.Children.Clear();

            switch (reportType)
            {
                case "Báo cáo đơn hàng":
                    GenerateOrderReport(timeRange);
                    break;
                case "Báo cáo nhiệm vụ":
                    GenerateTaskReport(timeRange);
                    break;
                case "Báo cáo tồn kho":
                    GenerateInventoryReport(timeRange);
                    break;
                case "Báo cáo hiệu suất":
                    GeneratePerformanceReport(timeRange);
                    break;
            }
        }

        private void GenerateOrderReport(string timeRange)
        {
            var header = new TextBlock
            {
                Text = $"BÁO CÁO ĐƠN HÀNG - {timeRange.ToUpper()}",
                FontSize = 16,
                FontWeight = System.Windows.FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 20),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            spReportContent.Children.Add(header);

            // Summary statistics
            var totalOrders = sampleOrders.Count;
            var completedOrders = sampleOrders.Count(o => o.Status == "Completed");
            var totalRevenue = sampleOrders.Sum(o => o.TotalAmount);

            var summary = new StackPanel { Margin = new Thickness(0, 0, 0, 20) };

            summary.Children.Add(CreateSummaryItem("Tổng số đơn hàng:", totalOrders.ToString()));
            summary.Children.Add(CreateSummaryItem("Đơn hàng hoàn thành:", completedOrders.ToString()));
            summary.Children.Add(CreateSummaryItem("Tổng doanh thu:", $"{totalRevenue:N0} VNĐ"));
            summary.Children.Add(CreateSummaryItem("Tỷ lệ hoàn thành:", $"{(double)completedOrders / totalOrders * 100:F1}%"));

            spReportContent.Children.Add(summary);

            // Order details table
            var tableHeader = new TextBlock
            {
                Text = "CHI TIẾT ĐƠN HÀNG",
                FontSize = 14,
                FontWeight = System.Windows.FontWeights.Bold,
                Margin = new Thickness(0, 20, 0, 10)
            };
            spReportContent.Children.Add(tableHeader);

            var dataGrid = new DataGrid
            {
                AutoGenerateColumns = false,
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                IsReadOnly = true,
                GridLinesVisibility = DataGridGridLinesVisibility.Horizontal,
                HeadersVisibility = DataGridHeadersVisibility.Column,
                RowHeaderWidth = 0,
                Height = 200,
                ItemsSource = sampleOrders
            };

            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Mã đơn hàng", Binding = new System.Windows.Data.Binding("OrderId"), Width = 100 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Ngày đặt", Binding = new System.Windows.Data.Binding("OrderDate") { StringFormat = "dd/MM/yyyy" }, Width = 100 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Tổng tiền", Binding = new System.Windows.Data.Binding("TotalAmount") { StringFormat = "N0 VNĐ" }, Width = 120 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Trạng thái", Binding = new System.Windows.Data.Binding("Status"), Width = 100 });

            spReportContent.Children.Add(dataGrid);
        }

        private void GenerateTaskReport(string timeRange)
        {
            var header = new TextBlock
            {
                Text = $"BÁO CÁO NHIỆM VỤ - {timeRange.ToUpper()}",
                FontSize = 16,
                FontWeight = System.Windows.FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 20),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            spReportContent.Children.Add(header);

            // Summary statistics
            var totalTasks = sampleTasks.Count;
            var completedTasks = sampleTasks.Count(t => t.Status == "Hoàn thành");
            var inProgressTasks = sampleTasks.Count(t => t.Status == "Đang thực hiện");

            var summary = new StackPanel { Margin = new Thickness(0, 0, 0, 20) };

            summary.Children.Add(CreateSummaryItem("Tổng số nhiệm vụ:", totalTasks.ToString()));
            summary.Children.Add(CreateSummaryItem("Nhiệm vụ hoàn thành:", completedTasks.ToString()));
            summary.Children.Add(CreateSummaryItem("Nhiệm vụ đang thực hiện:", inProgressTasks.ToString()));
            summary.Children.Add(CreateSummaryItem("Tỷ lệ hoàn thành:", $"{(double)completedTasks / totalTasks * 100:F1}%"));

            spReportContent.Children.Add(summary);

            // Task details table
            var tableHeader = new TextBlock
            {
                Text = "CHI TIẾT NHIỆM VỤ",
                FontSize = 14,
                FontWeight = System.Windows.FontWeights.Bold,
                Margin = new Thickness(0, 20, 0, 10)
            };
            spReportContent.Children.Add(tableHeader);

            var dataGrid = new DataGrid
            {
                AutoGenerateColumns = false,
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                IsReadOnly = true,
                GridLinesVisibility = DataGridGridLinesVisibility.Horizontal,
                HeadersVisibility = DataGridHeadersVisibility.Column,
                RowHeaderWidth = 0,
                Height = 200,
                ItemsSource = sampleTasks
            };

            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Tên nhiệm vụ", Binding = new System.Windows.Data.Binding("TaskName"), Width = 200 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Hạn", Binding = new System.Windows.Data.Binding("DueDate") { StringFormat = "dd/MM/yyyy" }, Width = 100 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Trạng thái", Binding = new System.Windows.Data.Binding("Status"), Width = 100 });

            spReportContent.Children.Add(dataGrid);
        }

        private void GenerateInventoryReport(string timeRange)
        {
            var header = new TextBlock
            {
                Text = $"BÁO CÁO TỒN KHO - {timeRange.ToUpper()}",
                FontSize = 16,
                FontWeight = System.Windows.FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 20),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            spReportContent.Children.Add(header);

            // Summary statistics
            var totalProducts = sampleProducts.Count;
            var lowStockProducts = sampleProducts.Count(p => p.StockQuantity <= 10);
            var outOfStockProducts = sampleProducts.Count(p => p.StockQuantity == 0);
            var totalValue = sampleProducts.Sum(p => p.StockQuantity * p.UnitPrice);

            var summary = new StackPanel { Margin = new Thickness(0, 0, 0, 20) };

            summary.Children.Add(CreateSummaryItem("Tổng số sản phẩm:", totalProducts.ToString()));
            summary.Children.Add(CreateSummaryItem("Sản phẩm sắp hết:", lowStockProducts.ToString()));
            summary.Children.Add(CreateSummaryItem("Sản phẩm hết hàng:", outOfStockProducts.ToString()));
            summary.Children.Add(CreateSummaryItem("Tổng giá trị tồn kho:", $"{totalValue:N0} VNĐ"));

            spReportContent.Children.Add(summary);

            // Inventory details table
            var tableHeader = new TextBlock
            {
                Text = "CHI TIẾT TỒN KHO",
                FontSize = 14,
                FontWeight = System.Windows.FontWeights.Bold,
                Margin = new Thickness(0, 20, 0, 10)
            };
            spReportContent.Children.Add(tableHeader);

            var dataGrid = new DataGrid
            {
                AutoGenerateColumns = false,
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                IsReadOnly = true,
                GridLinesVisibility = DataGridGridLinesVisibility.Horizontal,
                HeadersVisibility = DataGridHeadersVisibility.Column,
                RowHeaderWidth = 0,
                Height = 200,
                ItemsSource = sampleProducts
            };

            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Tên sản phẩm", Binding = new System.Windows.Data.Binding("ProductName"), Width = 200 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Tồn kho", Binding = new System.Windows.Data.Binding("StockQuantity"), Width = 100 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Đơn giá", Binding = new System.Windows.Data.Binding("UnitPrice") { StringFormat = "N0 VNĐ" }, Width = 120 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Giá trị", Binding = new System.Windows.Data.Binding(".") { Converter = new StockValueConverter() }, Width = 120 });

            spReportContent.Children.Add(dataGrid);
        }

        private void GeneratePerformanceReport(string timeRange)
        {
            var header = new TextBlock
            {
                Text = $"BÁO CÁO HIỆU SUẤT - {timeRange.ToUpper()}",
                FontSize = 16,
                FontWeight = System.Windows.FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 20),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            spReportContent.Children.Add(header);

            // Performance metrics
            var summary = new StackPanel { Margin = new Thickness(0, 0, 0, 20) };

            summary.Children.Add(CreateSummaryItem("Đơn hàng xử lý:", "15/20"));
            summary.Children.Add(CreateSummaryItem("Thời gian phản hồi TB:", "2.5 giờ"));
            summary.Children.Add(CreateSummaryItem("Tỷ lệ hoàn thành nhiệm vụ:", "85%"));
            summary.Children.Add(CreateSummaryItem("Đánh giá khách hàng:", "4.5/5"));

            spReportContent.Children.Add(summary);

            // Performance chart placeholder
            var chartPlaceholder = new TextBlock
            {
                Text = "📊 Biểu đồ hiệu suất sẽ được hiển thị ở đây",
                FontSize = 14,
                Foreground = System.Windows.Media.Brushes.Gray,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 50, 0, 0)
            };
            spReportContent.Children.Add(chartPlaceholder);
        }

        private TextBlock CreateSummaryItem(string label, string value)
        {
            return new TextBlock
            {
                Text = $"{label} {value}",
                FontSize = 12,
                Margin = new Thickness(0, 5, 0, 5)
            };
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    // Simple converter for stock value calculation
    public class StockValueConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Product product)
            {
                return (product.StockQuantity * product.UnitPrice).ToString("N0") + " VNĐ";
            }
            return "0 VNĐ";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}