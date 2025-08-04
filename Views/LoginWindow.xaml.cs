using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using AppOrderNilon.Models;
using AppOrderNilon.Views;
using AppOrderNilon.Services;

namespace AppOrderNilon.Views
{
    public partial class LoginWindow : Window
    {
        private AdminService _adminService;

        public LoginWindow()
        {
            InitializeComponent();
            _adminService = new AdminService();
            txtUsername.Focus();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin đăng nhập!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate input
            if (username.Length < 3)
            {
                MessageBox.Show("Tên đăng nhập phải có ít nhất 3 ký tự!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUsername.Focus();
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPassword.Focus();
                return;
            }

            try
            {
                // Try to authenticate user
                if (AuthenticateUser(username, password, out string userRole, out object user))
                {
                    string displayName = GetDisplayName(user, userRole);
                    MessageBox.Show($"Đăng nhập thành công!\n\nChào mừng {displayName}\nRole: {userRole}", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // Open appropriate dashboard based on role
                    OpenDashboard(userRole, user);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!\n\nTài khoản mẫu:\n- Admin: admin1/123456\n- Staff: staff1/123456\n- Customer: customer1/123456", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đăng nhập: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool AuthenticateUser(string username, string password, out string userRole, out object user)
        {
            userRole = "";
            user = null;

            try
            {
                // Try database authentication first

                // Admin authentication
                var admin = _adminService.AuthenticateAdmin(username, password);
                if (admin != null)
                {
                    userRole = "Admin";
                    user = admin;
                    return true;
                }

                // Staff authentication
                var staff = _adminService.AuthenticateStaff(username, password);
                if (staff != null)
                {
                    userRole = "Staff";
                    user = staff;
                    return true;
                }

                // Customer authentication
                var customer = _adminService.AuthenticateCustomer(username, password);
                if (customer != null)
                {
                    userRole = "Customer";
                    user = customer;
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                System.Diagnostics.Debug.WriteLine($"Database authentication failed: {ex.Message}");
            }

            // Fallback authentication logic for demonstration
            // Admin authentication
            if ((username == "admin1" || username == "admin2") && password == "123456")
            {
                userRole = "Admin";
                if (username == "admin1")
                {
                    user = new Admin
                    {
                        AdminId = 1,
                        Username = "admin1",
                        FullName = "Nguyễn Quản Trị",
                        Email = "admin1@app.com"
                    };
                }
                else
                {
                    user = new Admin
                    {
                        AdminId = 2,
                        Username = "admin2",
                        FullName = "Lê Quản Trị",
                        Email = "admin2@app.com"
                    };
                }
                return true;
            }

            // Staff authentication
            if ((username == "staff1" || username == "staff2") && password == "123456")
            {
                userRole = "Staff";
                if (username == "staff1")
                {
                    user = new Staff
                    {
                        StaffId = 1,
                        Username = "staff1",
                        FullName = "Trần Nhân Viên",
                        Email = "staff1@app.com"
                    };
                }
                else
                {
                    user = new Staff
                    {
                        StaffId = 2,
                        Username = "staff2",
                        FullName = "Phạm Nhân Viên",
                        Email = "staff2@app.com"
                    };
                }
                return true;
            }

            // Customer authentication
            if ((username == "customer1" || username == "customer2") && password == "123456")
            {
                userRole = "Customer";
                if (username == "customer1")
                {
                    user = new Customer
                    {
                        CustomerId = 1,
                        Username = "customer1",
                        CustomerName = "Công ty Xây dựng Minh Anh",
                        Email = "minhanh@construction.com",
                        Phone = "0987654321",
                        Address = "789 Đường Láng, Hà Nội"
                    };
                }
                else
                {
                    user = new Customer
                    {
                        CustomerId = 2,
                        Username = "customer2",
                        CustomerName = "Lê Văn C",
                        Email = "levanc@gmail.com",
                        Phone = "0971234567",
                        Address = "123 Đường Nguyễn Trãi, Hà Nội"
                    };
                }
                return true;
            }

            return false;
        }

        private string GetDisplayName(object user, string userRole)
        {
            switch (userRole)
            {
                case "Admin":
                    return (user as Admin)?.FullName ?? "Admin";
                case "Staff":
                    return (user as Staff)?.FullName ?? "Staff";
                case "Customer":
                    return (user as Customer)?.CustomerName ?? "Customer";
                default:
                    return "User";
            }
        }

        private void OpenDashboard(string userRole, object user)
        {
            try
            {
                switch (userRole)
                {
                    case "Admin":
                        DashboardWindow adminDashboard = new DashboardWindow(user as Admin);
                        adminDashboard.Show();
                        break;

                    case "Staff":
                        StaffDashboardWindow staffDashboard = new StaffDashboardWindow(user as Staff);
                        staffDashboard.Show();
                        break;

                    case "Customer":
                        CustomerDashboardWindow customerDashboard = new CustomerDashboardWindow(user as Customer);
                        customerDashboard.Show();
                        break;

                    default:
                        MessageBox.Show("Role không hợp lệ!", "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở dashboard: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Register_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                RegisterWindow registerWindow = new RegisterWindow();
                registerWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form đăng ký: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ForgotPassword_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Chức năng quên mật khẩu sẽ được implement sau!\n\nLiên hệ admin để được hỗ trợ.", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Handle Enter key press
        private void txtPassword_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Login_Click(sender, e);
            }
        }

        private void txtUsername_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                txtPassword.Focus();
            }
        }

        // Event handlers for text changes
        private void txtUsername_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Enable/disable login button based on input
            btnLogin.IsEnabled = !string.IsNullOrWhiteSpace(txtUsername.Text) &&
                                !string.IsNullOrWhiteSpace(txtPassword.Password);
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Enable/disable login button based on input
            btnLogin.IsEnabled = !string.IsNullOrWhiteSpace(txtUsername.Text) &&
                                !string.IsNullOrWhiteSpace(txtPassword.Password);
        }

        protected override void OnClosed(EventArgs e)
        {
            _adminService?.Dispose();
            base.OnClosed(e);
        }
    }
}