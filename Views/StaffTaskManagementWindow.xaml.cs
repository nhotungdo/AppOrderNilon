using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AppOrderNilon.Models;

namespace AppOrderNilon.Views
{
    public partial class StaffTaskManagementWindow : Window
    {
        private Staff currentStaff;
        private List<StaffTask> allTasks;
        private List<StaffTask> filteredTasks;

        public StaffTaskManagementWindow(Staff staff)
        {
            InitializeComponent();
            currentStaff = staff;
            LoadData();
            InitializeControls();
        }

        private void LoadData()
        {
            // Load sample data for now
            LoadSampleTasks();
            filteredTasks = new List<StaffTask>(allTasks);
        }

        private void LoadSampleTasks()
        {
            allTasks = new List<StaffTask>
            {
                new StaffTask
                {
                    TaskId = 1,
                    TaskName = "Xử lý đơn hàng #123",
                    Description = "Kiểm tra và chuẩn bị đơn hàng cho khách hàng VIP",
                    DueDate = DateTime.Now.AddHours(2),
                    Priority = "Cao",
                    Status = "Đang thực hiện"
                },
                new StaffTask
                {
                    TaskId = 2,
                    TaskName = "Kiểm tra tồn kho",
                    Description = "Kiểm tra số lượng sản phẩm nilon trong kho",
                    DueDate = DateTime.Now.AddDays(1),
                    Priority = "Trung bình",
                    Status = "Chờ thực hiện"
                },
                new StaffTask
                {
                    TaskId = 3,
                    TaskName = "Liên hệ khách hàng",
                    Description = "Gọi điện xác nhận đơn hàng #124",
                    DueDate = DateTime.Now.AddHours(1),
                    Priority = "Cao",
                    Status = "Hoàn thành"
                },
                new StaffTask
                {
                    TaskId = 4,
                    TaskName = "Cập nhật báo cáo",
                    Description = "Cập nhật báo cáo doanh số tháng 8",
                    DueDate = DateTime.Now.AddDays(2),
                    Priority = "Thấp",
                    Status = "Chờ thực hiện"
                }
            };
        }

        private void InitializeControls()
        {
            txtStaffName.Text = currentStaff?.FullName ?? "Nhân viên";
            cmbStatusFilter.SelectedIndex = 0;
            RefreshTasksList();
        }

        private void RefreshTasksList()
        {
            dgTasks.ItemsSource = null;
            dgTasks.ItemsSource = filteredTasks;
        }

        private void ApplyFilters()
        {
            var query = allTasks.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                var searchTerm = txtSearch.Text.ToLower();
                query = query.Where(t =>
                    t.TaskName.ToLower().Contains(searchTerm) ||
                    t.Description.ToLower().Contains(searchTerm) ||
                    t.Status.ToLower().Contains(searchTerm)
                );
            }

            // Apply status filter
            if (cmbStatusFilter.SelectedIndex > 0)
            {
                var selectedStatus = (cmbStatusFilter.SelectedItem as ComboBoxItem)?.Content.ToString();
                query = query.Where(t => t.Status == selectedStatus);
            }

            filteredTasks = query.ToList();
            RefreshTasksList();
        }

        // Event Handlers
        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void StatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            ApplyFilters();
            MessageBox.Show("Đã làm mới dữ liệu!", "Thông báo");
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Open add task dialog
                StaffTaskFormWindow taskForm = new StaffTaskFormWindow(currentStaff);
                if (taskForm.ShowDialog() == true)
                {
                    LoadData();
                    ApplyFilters();
                    MessageBox.Show("Đã thêm nhiệm vụ mới!", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm nhiệm vụ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Tasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection change if needed
        }

        private void UpdateTask_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var task = button.DataContext as StaffTask;

            if (task != null)
            {
                try
                {
                    // Open update task dialog
                    StaffTaskFormWindow taskForm = new StaffTaskFormWindow(currentStaff, task);
                    if (taskForm.ShowDialog() == true)
                    {
                        LoadData();
                        ApplyFilters();
                        MessageBox.Show("Đã cập nhật nhiệm vụ!", "Thông báo");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi cập nhật nhiệm vụ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var task = button.DataContext as StaffTask;

            if (task != null)
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa nhiệm vụ '{task.TaskName}'?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    allTasks.Remove(task);
                    ApplyFilters();
                    MessageBox.Show("Đã xóa nhiệm vụ!", "Thông báo");
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}