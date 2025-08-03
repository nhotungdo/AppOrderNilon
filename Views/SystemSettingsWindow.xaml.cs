using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Data;

namespace AppOrderNilon.Views
{
    public partial class SystemSettingsWindow : Window
    {
        public SystemSettingsWindow()
        {
            InitializeComponent();
            LoadCurrentSettings();
        }

        private void LoadCurrentSettings()
        {
            // Load current database settings from appsettings.json or registry
            // For now, using default values
            txtServer.Text = "NHOTUNG\\SQLEXPRESS";
            txtDatabase.Text = "AppOrderNilon";
            txtUsername.Text = "sa";
            txtPassword.Password = "123";

            // Load system settings
            txtCompanyName.Text = "AppOrderNilon";
            txtAddress.Text = "123 Đường ABC, Quận XYZ, TP.HCM";
            txtPhone.Text = "0123456789";
            txtEmail.Text = "info@appordernilon.com";
        }

        private void TestConnection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string connectionString = BuildConnectionString();
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MessageBox.Show("Kết nối cơ sở dữ liệu thành công!", "Thông báo", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối: {ex.Message}", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string BuildConnectionString()
        {
            return $"Data Source={txtServer.Text};" +
                   $"Database={txtDatabase.Text};" +
                   $"User Id={txtUsername.Text};" +
                   $"Password={txtPassword.Password};" +
                   "TrustServerCertificate=true;" +
                   "Trusted_Connection=SSPI;" +
                   "Encrypt=false;";
        }

        private void BackupData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "SQL Backup Files (*.bak)|*.bak|All files (*.*)|*.*",
                    FileName = $"AppOrderNilon_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak",
                    Title = "Chọn vị trí lưu file sao lưu"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string backupPath = saveFileDialog.FileName;
                    string connectionString = BuildConnectionString();

                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string backupQuery = $"BACKUP DATABASE [{txtDatabase.Text}] TO DISK = '{backupPath}'";
                        
                        using (var command = new SqlCommand(backupQuery, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show($"Sao lưu dữ liệu thành công!\nFile: {backupPath}", "Thông báo", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi sao lưu: {ex.Message}", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RestoreData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "SQL Backup Files (*.bak)|*.bak|All files (*.*)|*.*",
                    Title = "Chọn file sao lưu để khôi phục"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    var result = MessageBox.Show("Bạn có chắc chắn muốn khôi phục dữ liệu?\nDữ liệu hiện tại sẽ bị ghi đè!", 
                        "Xác nhận khôi phục", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        string backupPath = openFileDialog.FileName;
                        string connectionString = BuildConnectionString();

                        using (var connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            
                            // Set database to single user mode
                            string singleUserQuery = $"ALTER DATABASE [{txtDatabase.Text}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
                            using (var command = new SqlCommand(singleUserQuery, connection))
                            {
                                command.ExecuteNonQuery();
                            }

                            // Restore database
                            string restoreQuery = $"RESTORE DATABASE [{txtDatabase.Text}] FROM DISK = '{backupPath}' WITH REPLACE";
                            using (var command = new SqlCommand(restoreQuery, connection))
                            {
                                command.ExecuteNonQuery();
                            }

                            // Set database back to multi user mode
                            string multiUserQuery = $"ALTER DATABASE [{txtDatabase.Text}] SET MULTI_USER";
                            using (var command = new SqlCommand(multiUserQuery, connection))
                            {
                                command.ExecuteNonQuery();
                            }
                        }

                        MessageBox.Show("Khôi phục dữ liệu thành công!", "Thông báo", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi khôi phục: {ex.Message}", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CleanupData_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn dọn dẹp dữ liệu?\nCác dữ liệu cũ sẽ bị xóa!", 
                "Xác nhận dọn dẹp", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    string connectionString = BuildConnectionString();
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        
                        // Clean up old data (example: delete orders older than 1 year)
                        string cleanupQuery = @"
                            DELETE FROM OrderDetails WHERE OrderId IN (
                                SELECT OrderId FROM Orders WHERE OrderDate < DATEADD(YEAR, -1, GETDATE())
                            );
                            DELETE FROM Orders WHERE OrderDate < DATEADD(YEAR, -1, GETDATE());";
                        
                        using (var command = new SqlCommand(cleanupQuery, connection))
                        {
                            int affectedRows = command.ExecuteNonQuery();
                            MessageBox.Show($"Dọn dẹp dữ liệu thành công!\nĐã xóa {affectedRows} bản ghi cũ.", "Thông báo", 
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi dọn dẹp: {ex.Message}", "Lỗi", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Save database settings
                SaveDatabaseSettings();
                
                // Save system settings
                SaveSystemSettings();

                MessageBox.Show("Lưu cài đặt thành công!", "Thông báo", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu cài đặt: {ex.Message}", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveDatabaseSettings()
        {
            // Save to registry or configuration file
            using (var key = Registry.CurrentUser.CreateSubKey(@"Software\AppOrderNilon\Database"))
            {
                key.SetValue("Server", txtServer.Text);
                key.SetValue("Database", txtDatabase.Text);
                key.SetValue("Username", txtUsername.Text);
                // Note: In production, password should be encrypted
                key.SetValue("Password", txtPassword.Password);
            }
        }

        private void SaveSystemSettings()
        {
            // Save to registry or configuration file
            using (var key = Registry.CurrentUser.CreateSubKey(@"Software\AppOrderNilon\System"))
            {
                key.SetValue("CompanyName", txtCompanyName.Text);
                key.SetValue("Address", txtAddress.Text);
                key.SetValue("Phone", txtPhone.Text);
                key.SetValue("Email", txtEmail.Text);
            }
        }

        private void ResetSettings_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn đặt lại tất cả cài đặt về mặc định?", 
                "Xác nhận đặt lại", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                LoadCurrentSettings();
                MessageBox.Show("Đã đặt lại cài đặt về mặc định!", "Thông báo", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BackToDashboard_Click(object sender, RoutedEventArgs e)
        {
            DashboardWindow dashboard = new DashboardWindow();
            dashboard.Show();
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