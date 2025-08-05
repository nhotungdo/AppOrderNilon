using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AppOrderNilon.Models;

namespace AppOrderNilon.Views
{
    public partial class StaffCustomerManagementWindow : Window
    {
        private Staff currentStaff;
        private List<Customer> allCustomers;
        private List<Customer> filteredCustomers;

        public StaffCustomerManagementWindow(Staff staff)
        {
            InitializeComponent();
            currentStaff = staff;
            LoadData();
            InitializeControls();
        }

        private void LoadData()
        {
            // Load sample data for now
            LoadSampleCustomers();
            filteredCustomers = new List<Customer>(allCustomers);
        }

        private void LoadSampleCustomers()
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
                    Notes = "Khách hàng VIP - Doanh nghiệp"
                },
                new Customer
                {
                    CustomerId = 2,
                    CustomerName = "Lê Văn C",
                    Phone = "0971234567",
                    Email = "levanc@gmail.com",
                    Address = "123 Đường Nguyễn Trãi, Hà Nội",
                    Notes = "Khách hàng cá nhân"
                },
                new Customer
                {
                    CustomerId = 3,
                    CustomerName = "Nguyễn Thị D",
                    Phone = "0969876543",
                    Email = "nguyenthid@gmail.com",
                    Address = "456 Đường Trần Phú, TP.HCM",
                    Notes = "Khách hàng thường xuyên - Cá nhân"
                },
                new Customer
                {
                    CustomerId = 4,
                    CustomerName = "Công ty Thương mại ABC",
                    Phone = "0241234567",
                    Email = "info@abc.com",
                    Address = "321 Đường Cầu Giấy, Hà Nội",
                    Notes = "Đối tác chiến lược - VIP"
                }
            };
        }

        private void InitializeControls()
        {
            txtStaffName.Text = currentStaff?.FullName ?? "Nhân viên";
            cmbTypeFilter.SelectedIndex = 0;
            RefreshCustomersList();
        }

        private void RefreshCustomersList()
        {
            dgCustomers.ItemsSource = null;
            dgCustomers.ItemsSource = filteredCustomers;
        }

        private void ApplyFilters()
        {
            var query = allCustomers.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                var searchTerm = txtSearch.Text.ToLower();
                query = query.Where(c =>
                    c.CustomerId.ToString().Contains(searchTerm) ||
                    c.CustomerName.ToLower().Contains(searchTerm) ||
                    c.Phone.Contains(searchTerm) ||
                    c.Email.ToLower().Contains(searchTerm) ||
                    c.Address.ToLower().Contains(searchTerm)
                );
            }

            // Apply type filter
            if (cmbTypeFilter.SelectedIndex > 0)
            {
                var selectedType = (cmbTypeFilter.SelectedItem as ComboBoxItem)?.Content.ToString();
                switch (selectedType)
                {
                    case "Doanh nghiệp":
                        query = query.Where(c => c.Notes != null && c.Notes.Contains("Doanh nghiệp"));
                        break;
                    case "Cá nhân":
                        query = query.Where(c => c.Notes != null && c.Notes.Contains("Cá nhân"));
                        break;
                    case "VIP":
                        query = query.Where(c => c.Notes != null && c.Notes.Contains("VIP"));
                        break;
                }
            }

            filteredCustomers = query.ToList();
            RefreshCustomersList();
        }

        // Event Handlers
        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void TypeFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            ApplyFilters();
            MessageBox.Show("Đã làm mới dữ liệu!", "Thông báo");
        }

        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            var selectedCustomer = dgCustomers.SelectedItem as Customer;
            if (selectedCustomer != null)
            {
                ViewCustomerDetails(selectedCustomer);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một khách hàng để xem chi tiết!", "Thông báo");
            }
        }

        private void Customers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection change if needed
        }

        private void ViewCustomerDetails_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var customer = button.DataContext as Customer;

            if (customer != null)
            {
                ViewCustomerDetails(customer);
            }
        }

        private void ContactCustomer_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var customer = button.DataContext as Customer;

            if (customer != null)
            {
                ContactCustomer(customer);
            }
        }

        private void ViewCustomerDetails(Customer customer)
        {
            try
            {
                // Open customer detail window
                CustomerDetailWindow detailWindow = new CustomerDetailWindow(customer);
                detailWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở chi tiết khách hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ContactCustomer(Customer customer)
        {
            try
            {
                var result = MessageBox.Show(
                    $"Bạn có muốn liên hệ với khách hàng {customer.CustomerName}?\n\n" +
                    $"Số điện thoại: {customer.Phone}\n" +
                    $"Email: {customer.Email}",
                    "Liên hệ khách hàng",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    // TODO: Open contact form or call log
                    MessageBox.Show($"Đã ghi nhận yêu cầu liên hệ với khách hàng {customer.CustomerName}!", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi liên hệ khách hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}