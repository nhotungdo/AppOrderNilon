using System.Windows;
using AppOrderNilon.Services;
using System;

namespace AppOrderNilon
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Initialize database
                using var dbService = new DatabaseService();

                if (!dbService.TestConnection())
                {
                    // Silently use sample data without showing warning
                    System.Diagnostics.Debug.WriteLine("Database connection failed. Using sample data.");
                }
                else
                {
                    dbService.InitializeDatabase();
                }
            }
            catch (Exception ex)
            {
                // Log error but don't show popup to user
                System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex.Message}");
            }
        }
    }
}
