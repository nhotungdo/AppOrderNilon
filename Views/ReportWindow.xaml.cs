using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AppOrderNilon.Models;

namespace AppOrderNilon.Views
{
    public partial class ReportWindow : Window
    {
        private List<Order> orders;
        private List<Product> products;
        private List<Customer> customers;

        public ReportWindow()
        {
            InitializeComponent();
            LoadData();
            InitializeControls();
        }

        private void LoadData()
        {
            // TODO: Load data from database
            // For now, using sample data
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            // Sample customers (initialize first)
            customers = new List<Customer>
            {
                new Customer { CustomerId = 1, CustomerName = "Công ty Xây dựng Minh Anh", Phone = "0987654321", Email = "minhanh@construction.com", Address = "789 Đường Láng, Hà Nội", Notes = "Khách hàng VIP" },
                new Customer { CustomerId = 2, CustomerName = "Lê Văn C", Phone = "0971234567", Email = "levanc@gmail.com", Address = "123 Đường Nguyễn Trãi, Hà Nội", Notes = "" }
            };

            // Sample products
            products = new List<Product>
            {
                new Product { ProductId = 1, ProductName = "Nilon lót sàn 0.2mm", UnitPrice = 50000, StockQuantity = 100 },
                new Product { ProductId = 2, ProductName = "Mũ bảo hộ ABS", UnitPrice = 150000, StockQuantity = 5 },
                new Product { ProductId = 3, ProductName = "Găng tay cao su", UnitPrice = 30000, StockQuantity = 200 }
            };

            // Sample orders
            orders = new List<Order>
            {
                new Order { OrderId = 1, CustomerId = 1, OrderDate = new DateTime(2025, 8, 1), TotalAmount = 250000, Status = "Completed" },
                new Order { OrderId = 2, CustomerId = 2, OrderDate = new DateTime(2025, 8, 2), TotalAmount = 180000, Status = "Pending" },
                new Order { OrderId = 3, CustomerId = 1, OrderDate = new DateTime(2025, 8, 3), TotalAmount = 320000, Status = "Completed" }
            };

            // Set navigation properties (after customers and orders are initialized)
            orders[0].Customer = customers[0];
            orders[1].Customer = customers[1];
            orders[2].Customer = customers[0];
        }

        private void InitializeControls()
        {
            // Set default values
            cmbReportType.SelectedIndex = 0;
            cmbPeriod.SelectedIndex = 2; // Month
            dpFromDate.SelectedDate = DateTime.Now.AddMonths(-1);
            dpToDate.SelectedDate = DateTime.Now;

            // Generate initial report
            GenerateReport();
        }

        private void ReportType_Changed(object sender, SelectionChangedEventArgs e)
        {
            UpdateReportTitles();
        }

        private void DateRange_Changed(object sender, SelectionChangedEventArgs e)
        {
            // Handle date range changes
        }

        private void Period_Changed(object sender, SelectionChangedEventArgs e)
        {
            // Handle period changes
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            GenerateReport();
        }

        private void GenerateReport()
        {
            if (dpFromDate.SelectedDate == null || dpToDate.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn khoảng thời gian báo cáo!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var fromDate = dpFromDate.SelectedDate.Value;
            var toDate = dpToDate.SelectedDate.Value;

            // Filter orders by date range
            var filteredOrders = orders.Where(o => o.OrderDate >= fromDate && o.OrderDate <= toDate).ToList();

            // Update summary cards
            UpdateSummaryCards(filteredOrders);

            // Generate report based on type
            switch (cmbReportType.SelectedIndex)
            {
                case 0: // Revenue Report
                    GenerateRevenueReport(filteredOrders);
                    break;
                case 1: // Best Selling Products
                    GenerateBestSellingReport();
                    break;
                case 2: // Inventory Report
                    GenerateInventoryReport();
                    break;
                case 3: // Customer Report
                    GenerateCustomerReport(filteredOrders);
                    break;
            }
        }

        private void UpdateSummaryCards(List<Order> filteredOrders)
        {
            decimal totalRevenue = filteredOrders.Where(o => o.Status == "Completed").Sum(o => o.TotalAmount);
            int totalOrders = filteredOrders.Count;
            decimal averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;
            int productsSold = filteredOrders.Count * 2; // Sample calculation

            txtTotalRevenue.Text = $"₫{totalRevenue:N0}";
            txtTotalOrders.Text = totalOrders.ToString();
            txtAverageOrderValue.Text = $"₫{averageOrderValue:N0}";
            txtProductsSold.Text = productsSold.ToString();

            string period = $"{dpFromDate.SelectedDate:dd/MM/yyyy} - {dpToDate.SelectedDate:dd/MM/yyyy}";
            txtRevenuePeriod.Text = period;
            txtOrdersPeriod.Text = period;
            txtAveragePeriod.Text = period;
            txtProductsPeriod.Text = period;
        }

        private void GenerateRevenueReport(List<Order> filteredOrders)
        {
            txtChartTitle.Text = "Biểu đồ doanh thu theo thời gian";
            txtTableTitle.Text = "Doanh thu chi tiết";

            // Sample revenue data
            var revenueData = new List<ReportDataItem>
            {
                new ReportDataItem { Name = "Tuần 1", Value = "₫250,000" },
                new ReportDataItem { Name = "Tuần 2", Value = "₫180,000" },
                new ReportDataItem { Name = "Tuần 3", Value = "₫320,000" },
                new ReportDataItem { Name = "Tuần 4", Value = "₫150,000" }
            };

            dgReportData.ItemsSource = revenueData;
        }

        private void GenerateBestSellingReport()
        {
            txtChartTitle.Text = "Biểu đồ sản phẩm bán chạy";
            txtTableTitle.Text = "Top sản phẩm bán chạy";

            var bestSellingData = new List<ReportDataItem>
            {
                new ReportDataItem { Name = "Nilon lót sàn 0.2mm", Value = "50 cuộn" },
                new ReportDataItem { Name = "Mũ bảo hộ ABS", Value = "25 cái" },
                new ReportDataItem { Name = "Găng tay cao su", Value = "100 đôi" }
            };

            dgReportData.ItemsSource = bestSellingData;
        }

        private void GenerateInventoryReport()
        {
            txtChartTitle.Text = "Biểu đồ tồn kho";
            txtTableTitle.Text = "Tình trạng tồn kho";

            var inventoryData = new List<ReportDataItem>
            {
                new ReportDataItem { Name = "Nilon lót sàn 0.2mm", Value = "100 cuộn" },
                new ReportDataItem { Name = "Mũ bảo hộ ABS", Value = "5 cái (Sắp hết)" },
                new ReportDataItem { Name = "Găng tay cao su", Value = "200 đôi" }
            };

            dgReportData.ItemsSource = inventoryData;
        }

        private void GenerateCustomerReport(List<Order> filteredOrders)
        {
            txtChartTitle.Text = "Biểu đồ khách hàng";
            txtTableTitle.Text = "Top khách hàng";

            var customerData = new List<ReportDataItem>
            {
                new ReportDataItem { Name = "Công ty Xây dựng Minh Anh", Value = "₫570,000" },
                new ReportDataItem { Name = "Lê Văn C", Value = "₫180,000" }
            };

            dgReportData.ItemsSource = customerData;
        }

        private void UpdateReportTitles()
        {
            switch (cmbReportType.SelectedIndex)
            {
                case 0:
                    txtChartTitle.Text = "Biểu đồ doanh thu theo thời gian";
                    txtTableTitle.Text = "Doanh thu chi tiết";
                    break;
                case 1:
                    txtChartTitle.Text = "Biểu đồ sản phẩm bán chạy";
                    txtTableTitle.Text = "Top sản phẩm bán chạy";
                    break;
                case 2:
                    txtChartTitle.Text = "Biểu đồ tồn kho";
                    txtTableTitle.Text = "Tình trạng tồn kho";
                    break;
                case 3:
                    txtChartTitle.Text = "Biểu đồ khách hàng";
                    txtTableTitle.Text = "Top khách hàng";
                    break;
            }
        }

        private void BackToDashboard_Click(object sender, RoutedEventArgs e)
        {
            DashboardWindow dashboardWindow = new DashboardWindow();
            dashboardWindow.Show();
            this.Close();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }
    }

    public class ReportDataItem
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}