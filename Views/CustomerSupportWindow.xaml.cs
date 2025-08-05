using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using AppOrderNilon.Models;

namespace AppOrderNilon.Views
{
    public partial class CustomerSupportWindow : Window
    {
        private Customer currentCustomer;

        public CustomerSupportWindow(Customer customer)
        {
            InitializeComponent();
            currentCustomer = customer;
            InitializeControls();
        }

        private void InitializeControls()
        {
            txtCustomerName.Text = currentCustomer?.CustomerName ?? "Khách hàng";
            
            // Pre-fill contact information
            txtContactPhone.Text = currentCustomer?.Phone ?? "";
            txtContactEmail.Text = currentCustomer?.Email ?? "";
            
            // Set default subject
            cmbSubject.SelectedIndex = 0;
        }

        private void SendSupportRequest_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
                return;

            try
            {
                // Get form data
                var subject = (cmbSubject.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Khác";
                var description = txtDescription.Text.Trim();
                var contactPhone = txtContactPhone.Text.Trim();
                var contactEmail = txtContactEmail.Text.Trim();

                // In a real application, you would save this to database
                // For now, just show a success message
                var message = $"Yêu cầu hỗ trợ đã được gửi thành công!\n\n" +
                             $"Chủ đề: {subject}\n" +
                             $"Mô tả: {description}\n" +
                             $"Số điện thoại: {contactPhone}\n" +
                             $"Email: {contactEmail}\n\n" +
                             $"Chúng tôi sẽ liên hệ với bạn trong vòng 24 giờ.";

                MessageBox.Show(message, "Gửi yêu cầu thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                // Clear form
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gửi yêu cầu hỗ trợ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateForm()
        {
            // Validate subject
            if (cmbSubject.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn chủ đề!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbSubject.Focus();
                return false;
            }

            // Validate description
            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Vui lòng mô tả vấn đề của bạn!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtDescription.Focus();
                return false;
            }

            if (txtDescription.Text.Length < 10)
            {
                MessageBox.Show("Mô tả vấn đề phải có ít nhất 10 ký tự!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtDescription.Focus();
                return false;
            }

            // Validate phone
            if (string.IsNullOrWhiteSpace(txtContactPhone.Text))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại liên hệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtContactPhone.Focus();
                return false;
            }

            string phonePattern = @"^[0-9+\-\s()]+$";
            if (!Regex.IsMatch(txtContactPhone.Text, phonePattern))
            {
                MessageBox.Show("Số điện thoại không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtContactPhone.Focus();
                return false;
            }

            // Validate email (optional but must be valid if provided)
            if (!string.IsNullOrWhiteSpace(txtContactEmail.Text))
            {
                string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                if (!Regex.IsMatch(txtContactEmail.Text, emailPattern))
                {
                    MessageBox.Show("Email không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtContactEmail.Focus();
                    return false;
                }
            }

            return true;
        }

        private void ClearForm()
        {
            cmbSubject.SelectedIndex = 0;
            txtDescription.Clear();
            txtContactPhone.Text = currentCustomer?.Phone ?? "";
            txtContactEmail.Text = currentCustomer?.Email ?? "";
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
} 