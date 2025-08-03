using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using AppOrderNilon.Models;

namespace AppOrderNilon.Views
{
    public partial class CustomerFormWindow : Window
    {
        private Customer _customer;
        private bool _isEditMode;

        public Customer Customer => _customer;

        public CustomerFormWindow()
        {
            InitializeComponent();
            _customer = new Customer();
            _isEditMode = false;
            txtTitle.Text = "Thêm Khách hàng mới";
            btnSave.Content = "Thêm";
        }

        public CustomerFormWindow(Customer customer)
        {
            InitializeComponent();
            _customer = customer;
            _isEditMode = true;
            txtTitle.Text = "Sửa thông tin Khách hàng";
            btnSave.Content = "Cập nhật";

            LoadCustomerData();
        }

        private void LoadCustomerData()
        {
            txtCustomerName.Text = _customer.CustomerName;
            txtPhone.Text = _customer.Phone;
            txtEmail.Text = _customer.Email;
            txtAddress.Text = _customer.Address;
            txtNotes.Text = _customer.Notes;
            txtUsername.Text = _customer.Username;

            // Set customer type
            if (_customer.Notes?.Contains("VIP") == true)
            {
                cmbCustomerType.SelectedIndex = 1; // VIP
            }
            else
            {
                cmbCustomerType.SelectedIndex = 0; // Regular
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
                return;

            try
            {
                _customer.CustomerName = txtCustomerName.Text.Trim();
                _customer.Phone = txtPhone.Text.Trim();
                _customer.Email = txtEmail.Text.Trim();
                _customer.Address = txtAddress.Text.Trim();
                _customer.Notes = txtNotes.Text.Trim();
                _customer.Username = txtUsername.Text.Trim();

                // Update notes based on customer type
                var selectedType = cmbCustomerType.SelectedItem as ComboBoxItem;
                if (selectedType?.Content.ToString() == "Khách hàng VIP")
                {
                    if (!_customer.Notes.Contains("VIP"))
                    {
                        _customer.Notes = string.IsNullOrEmpty(_customer.Notes) ? "VIP" : _customer.Notes + " (VIP)";
                    }
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu thông tin: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateForm()
        {
            // Validate Customer Name
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên khách hàng!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtCustomerName.Focus();
                return false;
            }

            // Validate Phone
            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPhone.Focus();
                return false;
            }

            // Validate phone format
            string phonePattern = @"^[0-9+\-\s()]+$";
            if (!Regex.IsMatch(txtPhone.Text, phonePattern))
            {
                MessageBox.Show("Số điện thoại không hợp lệ!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPhone.Focus();
                return false;
            }

            // Validate Email (optional but must be valid if provided)
            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                if (!Regex.IsMatch(txtEmail.Text, emailPattern))
                {
                    MessageBox.Show("Email không hợp lệ!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtEmail.Focus();
                    return false;
                }
            }

            // Validate Address
            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtAddress.Focus();
                return false;
            }

            // Validate Username
            if (!string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                if (txtUsername.Text.Length < 3)
                {
                    MessageBox.Show("Tên đăng nhập phải có ít nhất 3 ký tự!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtUsername.Focus();
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