using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using Microsoft.Win32;
using AppOrderNilon.Models;

namespace AppOrderNilon.Views
{
    public partial class ReportDetailWindow : Window
    {
        private readonly Report _report;

        public ReportDetailWindow(Report report)
        {
            InitializeComponent();
            _report = report;
            LoadReportData();
        }

        private void LoadReportData()
        {
            try
            {
                // Load report info
                txtReportId.Text = _report.ReportId.ToString();
                txtReportType.Text = _report.ReportType;
                txtStartDate.Text = _report.StartDate?.ToString("dd/MM/yyyy") ?? "N/A";
                txtEndDate.Text = _report.EndDate?.ToString("dd/MM/yyyy") ?? "N/A";
                txtGeneratedDate.Text = _report.GeneratedDate.ToString("dd/MM/yyyy HH:mm:ss");
                txtAdminName.Text = _report.Admin?.FullName ?? "N/A";

                // Load report data
                if (!string.IsNullOrEmpty(_report.Data))
                {
                    try
                    {
                        // Try to format JSON data for better readability
                        var jsonDocument = JsonDocument.Parse(_report.Data);
                        var formattedJson = JsonSerializer.Serialize(jsonDocument, new JsonSerializerOptions
                        {
                            WriteIndented = true
                        });
                        txtReportData.Text = formattedJson;
                    }
                    catch
                    {
                        // If not valid JSON, display as plain text
                        txtReportData.Text = _report.Data;
                    }
                }
                else
                {
                    txtReportData.Text = "Không có dữ liệu báo cáo.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu báo cáo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "JSON files (*.json)|*.json|Text files (*.txt)|*.txt|All files (*.*)|*.*",
                    FileName = $"Report_{_report.ReportType}_{_report.ReportId}_{DateTime.Now:yyyyMMdd_HHmmss}.json",
                    Title = "Lưu báo cáo"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var content = txtReportData.Text;
                    File.WriteAllText(saveFileDialog.FileName, content);

                    MessageBox.Show("Xuất báo cáo thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất báo cáo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}