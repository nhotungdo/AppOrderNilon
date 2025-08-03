using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AppOrderNilon.Models;
using AppOrderNilon.Services;

namespace AppOrderNilon.Views
{
    public partial class ReportFormWindow : Window
    {
        private readonly ReportService _reportService;
        private readonly AppOrderNilonContext _context;
        private readonly Report _report;

        public ReportFormWindow(Report report)
        {
            InitializeComponent();
            _context = new AppOrderNilonContext();
            _reportService = new ReportService(_context);
            _report = report;
            LoadReportData();
        }

        private void LoadReportData()
        {
            try
            {
                // Set report type
                foreach (ComboBoxItem item in cboReportType.Items)
                {
                    if (item.Tag?.ToString() == _report.ReportType)
                    {
                        cboReportType.SelectedItem = item;
                        break;
                    }
                }

                // Set dates
                dpStartDate.SelectedDate = _report.StartDate;
                dpEndDate.SelectedDate = _report.EndDate;
                txtGeneratedDate.Text = _report.GeneratedDate.ToString("dd/MM/yyyy HH:mm:ss");
                txtReportData.Text = _report.Data ?? "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu báo cáo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
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

                // Update report data
                _report.ReportType = (cboReportType.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? _report.ReportType;
                _report.StartDate = dpStartDate.SelectedDate.Value;
                _report.EndDate = dpEndDate.SelectedDate.Value;
                _report.Data = txtReportData.Text.Trim();

                var success = await _reportService.UpdateReportAsync(_report);
                if (success)
                {
                    MessageBox.Show("Cập nhật báo cáo thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Không thể cập nhật báo cáo.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            _context?.Dispose();
            base.OnClosed(e);
        }
    }
}