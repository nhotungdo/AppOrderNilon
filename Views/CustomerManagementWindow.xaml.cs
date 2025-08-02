using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AppOrderNilon.Models;

namespace AppOrderNilon.Views
{
    public partial class CustomerManagementWindow : Window
    {
        private List<Customer> allCustomers;

        public CustomerManagementWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            // TODO: Load data from database
            // For now, using sample data
            LoadSampleData();
            RefreshCustomerGrid();
            UpdateStatusBar();
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
            var filteredCustomers = allCustomers.AsEnumerable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string searchTerm = txtSearch.Text.ToLower();
                filteredCustomers = filteredCustomers.Where(c => 
                    c.CustomerName.ToLower().Contains(searchTerm) ||
                    c.Phone.ToLower().Contains(searchTerm) ||
                    c.Email.ToLower().Contains(searchTerm) ||
                    c.Address.ToLower().Contains(searchTerm) ||
                    c.Notes.ToLower().Contains(searchTerm));
            }

            // Apply customer type filter
            if (cmbCustomerType.SelectedIndex > 0)
            {
                switch (cmbCustomerType.SelectedIndex)
                {
                    case 1: // VIP
                        filteredCustomers = filteredCustomers.Where(c => c.Notes.Contains("VIP"));
                        break;
                    case 2: // Regular
                        filteredCustomers = filteredCustomers.Where(c => !c.Notes.Contains("VIP"));
                        break;
                }
            }

            dgCustomers.ItemsSource = filteredCustomers.ToList();
        }

        private void UpdateStatusBar()
        {
            txtTotalCustomers.Text = allCustomers.Count.ToString();
            int vipCount = allCustomers.Count(c => c.Notes.Contains("VIP"));
            txtVIPCustomers.Text = vipCount.ToString();
            int regularCount = allCustomers.Count(c => !c.Notes.Contains("VIP"));
            txtRegularCustomers.Text = regularCount.ToString();
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

        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            CustomerDetailWindow customerDetailWindow = new CustomerDetailWindow(null);
            if (customerDetailWindow.ShowDialog() == true)
            {
                // TODO: Add new customer to database
                LoadData(); // Reload data
            }
        }

        private void ViewCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (dgCustomers.SelectedItem is Customer selectedCustomer)
            {
                CustomerDetailWindow customerDetailWindow = new CustomerDetailWindow(selectedCustomer);
                customerDetailWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần xem!", "Thông báo", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void EditCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (dgCustomers.SelectedItem is Customer selectedCustomer)
            {
                CustomerDetailWindow customerDetailWindow = new CustomerDetailWindow(selectedCustomer);
                if (customerDetailWindow.ShowDialog() == true)
                {
                    // TODO: Update customer in database
                    LoadData(); // Reload data
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần sửa!", "Thông báo", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (dgCustomers.SelectedItem is Customer selectedCustomer)
            {
                var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa khách hàng '{selectedCustomer.CustomerName}'?", 
                    "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    // TODO: Delete customer from database
                    allCustomers.Remove(selectedCustomer);
                    RefreshCustomerGrid();
                    UpdateStatusBar();
                    MessageBox.Show("Đã xóa khách hàng thành công!", "Thông báo", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
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
    }
} 