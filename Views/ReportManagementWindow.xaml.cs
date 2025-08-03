using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AppOrderNilon.Models;
using AppOrderNilon.Services;

namespace AppOrderNilon.Views
{
    public partial class ReportManagementWindow : Window
    {
        private readonly ReportService _reportService;
        private readonly AppOrderNilonContext _context;
        private List<Report> _allReports;
        private Report? _selectedReport;
        private Admin _currentAdmin;

        public ReportManagementWindow()
        {
            InitializeComponent();
            _context = new AppOrderNilonContext();
            _reportService = new ReportService(_context);
            _allReports = new List<Report>();
            
            // Set default dates
            dpStartDate.SelectedDate = DateTime.Now.AddMonths(-1);
            dpEndDate.SelectedDate = DateTime.Now;
            
            // Set default report type
            cboReportType.SelectedIndex = 0;
            
            LoadDataAsync();
        }

        public ReportManagementWindow(Admin admin) : this()
        {
            _currentAdmin = admin;
        }

        private async void LoadDataAsync()
        {
            try
            {
                txtStatus.Text = "Đang tải dữ liệu...";
                _allReports = await _reportService.GetAllReportsAsync();
                dgReports.ItemsSource = _allReports;
                txtStatus.Text = $"Đã tải {_allReports.Count} báo cáo";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "Lỗi khi tải dữ liệu";
            }
        }

        private async void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var searchTerm = txtSearch.Text.Trim();
                if (string.IsNullOrEmpty(searchTerm))
                {
                    dgReports.ItemsSource = _allReports;
                }
                else
                {
                    var filteredReports = await _reportService.SearchReportsAsync(searchTerm);
                    dgReports.ItemsSource = filteredReports;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgReports_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedReport = dgReports.SelectedItem as Report;
            btnView.IsEnabled = _selectedReport != null;
            btnEdit.IsEnabled = _selectedReport != null;
            btnDelete.IsEnabled = _selectedReport != null;
        }

        private async void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cboReportType.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn loại báo cáo.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (dpStartDate.SelectedDate == null || dpEndDate.SelectedDate == null)
                {
                    MessageBox.Show("Vui lòng chọn khoảng thời gian.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (dpStartDate.SelectedDate > dpEndDate.SelectedDate)
                {
                    MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var reportType = (cboReportType.SelectedItem as ComboBoxItem)?.Tag?.ToString();
                var startDate = dpStartDate.SelectedDate.Value;
                var endDate = dpEndDate.SelectedDate.Value;
                var adminId = _currentAdmin?.AdminId ?? 1; // Default admin ID

                txtStatus.Text = "Đang tạo báo cáo...";
                Report? newReport = null;

                switch (reportType)
                {
                    case "Revenue":
                        newReport = await _reportService.GenerateRevenueReportAsync(startDate, endDate, adminId);
                        break;
                    case "Inventory":
                        newReport = await _reportService.GenerateInventoryReportAsync(adminId);
                        break;
                    case "BestSelling":
                        newReport = await _reportService.GenerateBestSellingReportAsync(startDate, endDate, adminId);
                        break;
                }

                if (newReport != null)
                {
                    MessageBox.Show("Tạo báo cáo thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadDataAsync();
                }
                else
                {
                    MessageBox.Show("Không thể tạo báo cáo.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo báo cáo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                txtStatus.Text = "Sẵn sàng";
            }
        }

        private void BtnView_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedReport == null)
            {
                MessageBox.Show("Vui lòng chọn báo cáo cần xem.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var reportDetailWindow = new ReportDetailWindow(_selectedReport);
            reportDetailWindow.ShowDialog();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedReport == null)
            {
                MessageBox.Show("Vui lòng chọn báo cáo cần sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var reportForm = new ReportFormWindow(_selectedReport);
            if (reportForm.ShowDialog() == true)
            {
                LoadDataAsync();
            }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedReport == null)
            {
                MessageBox.Show("Vui lòng chọn báo cáo cần xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa báo cáo '{_selectedReport.ReportType}'?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    txtStatus.Text = "Đang xóa...";
                    var success = await _reportService.DeleteReportAsync(_selectedReport.ReportId);
                    
                    if (success)
                    {
                        MessageBox.Show("Xóa báo cáo thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadDataAsync();
                    }
                    else
                    {
                        MessageBox.Show("Không thể xóa báo cáo.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        txtStatus.Text = "Lỗi khi xóa";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa báo cáo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtStatus.Text = "Lỗi khi xóa";
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _context?.Dispose();
            base.OnClosed(e);
        }
    }
} 