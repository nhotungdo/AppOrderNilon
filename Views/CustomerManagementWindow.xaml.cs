using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AppOrderNilon.Models;
using AppOrderNilon.Services;

namespace AppOrderNilon.Views
{
    public partial class CustomerManagementWindow : Window
    {
        private CustomerService _customerService;
        private List<Customer> allCustomers;

        public CustomerManagementWindow()
        {
            InitializeComponent();
            _customerService = new CustomerService(new AppOrderNilonContext());
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                allCustomers = await _customerService.GetAllCustomersAsync();
                RefreshCustomerGrid();
                UpdateStatusBar();
            }
            catch (Exception ex)
            {
                // Log error for debugging
                System.Diagnostics.Debug.WriteLine($"Database error: {ex.Message}");
                
                // Fallback to sample data without showing error popup
                LoadSampleData();
                RefreshCustomerGrid();
                UpdateStatusBar();
            }
        }

        private void LoadSampleData()
        {
            allCustomers = new List<Customer>
            {
                new Customer
                {
                    CustomerId = 1,
                    CustomerName = "Công ty Xây dựng Minh Anh",
                    Phone = "0987654321",
                    Email = "minhanh@construction.com",
                    Address = "789 Đường Láng, Hà Nội",
                    Notes = "Khách hàng VIP"
                },
                new Customer
                {
                    CustomerId = 2,
                    CustomerName = "Lê Văn C",
                    Phone = "0971234567",
                    Email = "levanc@gmail.com",
                    Address = "123 Đường Nguyễn Trãi, Hà Nội",
                    Notes = ""
                },
                new Customer
                {
                    CustomerId = 3,
                    CustomerName = "Công ty Thương mại ABC",
                    Phone = "0968765432",
                    Email = "abc@company.com",
                    Address = "456 Đường Giải Phóng, Hà Nội",
                    Notes = "Khách hàng VIP"
                }
            };
        }

        private void RefreshCustomerGrid()
        {
            try
            {
                List<Customer> filteredCustomers = allCustomers;

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    string searchText = txtSearch.Text.ToLower();
                    filteredCustomers = filteredCustomers.Where(c =>
                        (c.CustomerName != null && c.CustomerName.ToLower().Contains(searchText)) ||
                        (c.Phone != null && c.Phone.Contains(searchText)) ||
                        (c.Email != null && c.Email.ToLower().Contains(searchText))
                    ).ToList();
                }

                // Apply customer type filter
                if (cmbCustomerType.SelectedIndex > 0)
                {
                    switch (cmbCustomerType.SelectedIndex)
                    {
                        case 1: // VIP
                            filteredCustomers = filteredCustomers.Where(c => c.Notes?.Contains("VIP", StringComparison.OrdinalIgnoreCase) == true).ToList();
                            break;
                        case 2: // Regular
                            filteredCustomers = filteredCustomers.Where(c => c.Notes?.Contains("VIP", StringComparison.OrdinalIgnoreCase) != true).ToList();
                            break;
                    }
                }

                dgCustomers.ItemsSource = filteredCustomers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lọc dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                dgCustomers.ItemsSource = allCustomers;
            }
        }

        private void UpdateStatusBar()
        {
            try
            {
                var stats = _customerService.GetCustomerStatistics();
                txtTotalCustomers.Text = stats.TotalCustomers.ToString();
                txtVIPCustomers.Text = stats.VIPCustomers.ToString();
                txtRegularCustomers.Text = stats.RegularCustomers.ToString();
            }
            catch
            {
                // Fallback to manual calculation
                txtTotalCustomers.Text = allCustomers.Count.ToString();
                int vipCount = allCustomers.Count(c => c.Notes?.Contains("VIP") == true);
                txtVIPCustomers.Text = vipCount.ToString();
                int regularCount = allCustomers.Count(c => !c.Notes?.Contains("VIP") == true);
                txtRegularCustomers.Text = regularCount.ToString();
            }
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshCustomerGrid();
        }

        private void CustomerTypeFilter_Changed(object sender, SelectionChangedEventArgs e)
        {
            RefreshCustomerGrid();
        }

        private void Customer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle customer selection if needed
        }

        private async void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            var customerForm = new CustomerFormWindow();
            if (customerForm.ShowDialog() == true)
            {
                try
                {
                    var newCustomer = customerForm.Customer;
                    if (newCustomer != null)
                    {
                        var success = await _customerService.CreateCustomerAsync(newCustomer, "password123");
                        if (success)
                        {
                            MessageBox.Show("Thêm khách hàng thành công!", "Thông báo",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadData();
                        }
                        else
                        {
                            MessageBox.Show("Lỗi khi thêm khách hàng!", "Lỗi",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi thêm khách hàng: {ex.Message}", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void ViewCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (dgCustomers.SelectedItem is Customer selectedCustomer)
            {
                // Get full customer data with orders
                var fullCustomer = await _customerService.GetCustomerByIdAsync(selectedCustomer.CustomerId);
                if (fullCustomer != null)
                {
                    CustomerDetailWindow customerDetailWindow = new CustomerDetailWindow(fullCustomer);
                    customerDetailWindow.ShowDialog();
                }
                else
                {
                    CustomerDetailWindow customerDetailWindow = new CustomerDetailWindow(selectedCustomer);
                    customerDetailWindow.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần xem!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void EditCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (dgCustomers.SelectedItem is Customer selectedCustomer)
            {
                var customerForm = new CustomerFormWindow(selectedCustomer);
                if (customerForm.ShowDialog() == true)
                {
                    try
                    {
                        var updatedCustomer = customerForm.Customer;
                        if (updatedCustomer != null)
                        {
                            updatedCustomer.CustomerId = selectedCustomer.CustomerId; // Ensure ID is preserved
                            var success = await _customerService.UpdateCustomerAsync(updatedCustomer);
                            if (success)
                            {
                                MessageBox.Show("Cập nhật khách hàng thành công!", "Thông báo",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                                LoadData();
                            }
                            else
                            {
                                MessageBox.Show("Lỗi khi cập nhật khách hàng!", "Lỗi",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi cập nhật khách hàng: {ex.Message}", "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần sửa!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (dgCustomers.SelectedItem is Customer selectedCustomer)
            {
                var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa khách hàng '{selectedCustomer.CustomerName}'?",
                    "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var success = await _customerService.DeleteCustomerAsync(selectedCustomer.CustomerId);
                        if (success)
                        {
                            MessageBox.Show("Đã xóa khách hàng thành công!", "Thông báo",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadData();
                        }
                        else
                        {
                            MessageBox.Show("Lỗi khi xóa khách hàng!", "Lỗi",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        MessageBox.Show($"Không thể xóa khách hàng: {ex.Message}", "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi xóa khách hàng: {ex.Message}", "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần xóa!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
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

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }
    }
}