using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using AppOrderNilon.Models;
using AppOrderNilon.Views;

namespace AppOrderNilon.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
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

            // TODO: Implement proper authentication with database
            // For now, using sample authentication logic
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
            }
        }

        private bool AuthenticateUser(string username, string password, out string userRole, out object user)
        {
            userRole = "";
            user = null;

            // TODO: Replace with database authentication
            // Sample authentication logic for demonstration
            
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
            switch (userRole)
            {
                case "Admin":
                    DashboardWindow adminDashboard = new DashboardWindow();
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

        private void Register_Click(object sender, MouseButtonEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }

        private void ForgotPassword_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Chức năng quên mật khẩu sẽ được implement sau!", "Thông báo", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
} 