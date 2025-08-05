using System;
using System.Windows;
using AppOrderNilon.Models;

namespace AppOrderNilon.Views
{
    public partial class StaffTaskFormWindow : Window
    {
        private Staff currentStaff;
        private StaffTask currentTask;
        private bool isEditMode;

        public StaffTaskFormWindow(Staff staff)
        {
            InitializeComponent();
            currentStaff = staff;
            isEditMode = false;
            InitializeControls();
        }

        public StaffTaskFormWindow(Staff staff, StaffTask task)
        {
            InitializeComponent();
            currentStaff = staff;
            currentTask = task;
            isEditMode = true;
            InitializeControls();
            LoadTaskData();
        }

        private void InitializeControls()
        {
            if (isEditMode)
            {
                txtTitle.Text = "Cập nhật nhiệm vụ";
                this.Title = "Cập nhật nhiệm vụ";
            }
            else
            {
                txtTitle.Text = "Thêm nhiệm vụ mới";
                this.Title = "Thêm nhiệm vụ mới";
            }

            // Set default values
            dpDueDate.SelectedDate = DateTime.Now.AddDays(1);
            cmbPriority.SelectedIndex = 1; // Trung bình
            cmbStatus.SelectedIndex = 0; // Chờ thực hiện
        }

        private void LoadTaskData()
        {
            if (currentTask != null)
            {
                txtTaskName.Text = currentTask.TaskName;
                txtDescription.Text = currentTask.Description;
                dpDueDate.SelectedDate = currentTask.DueDate;
                
                // Set priority
                switch (currentTask.Priority)
                {
                    case "Thấp":
                        cmbPriority.SelectedIndex = 0;
                        break;
                    case "Trung bình":
                        cmbPriority.SelectedIndex = 1;
                        break;
                    case "Cao":
                        cmbPriority.SelectedIndex = 2;
                        break;
                }

                // Set status
                switch (currentTask.Status)
                {
                    case "Chờ thực hiện":
                        cmbStatus.SelectedIndex = 0;
                        break;
                    case "Đang thực hiện":
                        cmbStatus.SelectedIndex = 1;
                        break;
                    case "Hoàn thành":
                        cmbStatus.SelectedIndex = 2;
                        break;
                }
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtTaskName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên nhiệm vụ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTaskName.Focus();
                return false;
            }

            if (dpDueDate.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn hạn hoàn thành!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpDueDate.Focus();
                return false;
            }

            if (cmbPriority.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn mức ưu tiên!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbPriority.Focus();
                return false;
            }

            if (cmbStatus.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn trạng thái!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbStatus.Focus();
                return false;
            }

            return true;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
                return;

            try
            {
                if (isEditMode)
                {
                    // Update existing task
                    UpdateTask();
                }
                else
                {
                    // Create new task
                    CreateTask();
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu nhiệm vụ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateTask()
        {
            var newTask = new StaffTask
            {
                TaskId = GetNextTaskId(),
                TaskName = txtTaskName.Text.Trim(),
                Description = txtDescription.Text.Trim(),
                DueDate = dpDueDate.SelectedDate.Value,
                Priority = (cmbPriority.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString() ?? "Trung bình",
                Status = (cmbStatus.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString() ?? "Chờ thực hiện"
            };

            // TODO: Save to database
            // For now, just show success message
            MessageBox.Show("Đã tạo nhiệm vụ mới thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UpdateTask()
        {
            if (currentTask != null)
            {
                currentTask.TaskName = txtTaskName.Text.Trim();
                currentTask.Description = txtDescription.Text.Trim();
                currentTask.DueDate = dpDueDate.SelectedDate.Value;
                currentTask.Priority = (cmbPriority.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString() ?? "Trung bình";
                currentTask.Status = (cmbStatus.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString() ?? "Chờ thực hiện";

                // TODO: Update in database
                // For now, just show success message
                MessageBox.Show("Đã cập nhật nhiệm vụ thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private int GetNextTaskId()
        {
            // TODO: Get next ID from database
            // For now, return a random number
            return new Random().Next(1000, 9999);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
} 