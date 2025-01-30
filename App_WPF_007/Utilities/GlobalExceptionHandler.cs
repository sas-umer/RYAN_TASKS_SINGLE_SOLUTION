using System;
using System.Windows;
using Serilog;

namespace App_WPF_007
{
    public static class GlobalExceptionHandler
    {
        public static void HandleException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            Log.Error(ex, "Unhandled exception occurred");
            MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void HandleDispatcherException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            Log.Error(ex, "Dispatcher unhandled exception occurred");
            MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
