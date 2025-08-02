using System;
using System.Text.RegularExpressions;
using System.Windows;
using AppOrderNilon.Models;

namespace AppOrderNilon.Views
{
    public partial class CustomerDetailWindow : Window
    {
        private Customer currentCustomer;
        private bool isEditMode;

        public CustomerDetailWindow(Customer customer)
        {
            InitializeComponent();
            this.currentCustomer = customer;
            this.isEditMode = customer != null;
            
            LoadCustomerData();
        }

        private void LoadCustomerData()
        {
            if (isEditMode && currentCustomer != null)
            {
                txtHeader.Text = "Sửa thông tin khách hàng";
                txtCustomerName.Text = currentCustomer.CustomerName;
                txtPhone.Text = currentCustomer.Phone;
                txtEmail.Text = currentCustomer.Email;
                txtAddress.Text = currentCustomer.Address;
                txtNotes.Text = currentCustomer.Notes;
            }
            else
            {
                txtHeader.Text = "Thêm khách hàng mới";
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs())
                return;

            try
            {
                if (isEditMode)
                {
                    // Update existing customer
                    UpdateCustomer();
                }
                else
                {
                    // Create new customer
                    CreateCustomer();
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu khách hàng: {ex.Message}", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateCustomer()
        {
            // TODO: Save to database
            MessageBox.Show("Khách hàng đã được tạo thành công!", "Thông báo", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UpdateCustomer()
        {
            // TODO: Update in database
            MessageBox.Show("Khách hàng đã được cập nhật thành công!", "Thông báo", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool ValidateInputs()
        {
            // Validate customer name
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên khách hàng!", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtCustomerName.Focus();
                return false;
            }

            // Validate phone number
            if (!string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                string phonePattern = @"^[0-9]{10,11}$";
                if (!Regex.IsMatch(txtPhone.Text, phonePattern))
                {
                    MessageBox.Show("Số điện thoại không đúng định dạng!", "Lỗi", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtPhone.Focus();
                    return false;
                }
            }

            // Validate email
            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                if (!Regex.IsMatch(txtEmail.Text, emailPattern))
                {
                    MessageBox.Show("Email không đúng định dạng!", "Lỗi", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtEmail.Focus();
                    return false;
                }
            }

            return true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
} 