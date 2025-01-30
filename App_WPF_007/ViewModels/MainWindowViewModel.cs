using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using App_WPF_007.Services;
using BL.Models;
using CommunityToolkit.Mvvm.Input;
using Serilog;

namespace App_WPF_007.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly UserService _userService;
        private User _selectedUser;
        private string _username;
        private string _password;
        private string _jobTitle;

        public ObservableCollection<User> Users { get; set; }
        public ICommand LoadUsersCommand { get; }
        public ICommand AddUserCommand { get; }
        public ICommand UpdateUserCommand { get; }
        public ICommand DeleteUserCommand { get; }

        public MainWindowViewModel()
        {
            _userService = new UserService();
            Users = new ObservableCollection<User>();
            LoadUsersCommand = new RelayCommand(async () => await LoadUsers());
            AddUserCommand = new RelayCommand(async () => await AddUser());
            UpdateUserCommand = new RelayCommand(async () => await UpdateUser());
            DeleteUserCommand = new RelayCommand(async () => await DeleteUser());

            // Load users automatically when the ViewModel is instantiated
            LoadUsersCommand.Execute(null);
        }

        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
                OnPropertyChanged(nameof(Username));
                OnPropertyChanged(nameof(Password));
                OnPropertyChanged(nameof(JobTitle));
            }
        }

        public string Username
        {
            get => _selectedUser?.Username;
            set
            {
                if (_selectedUser != null)
                {
                    _selectedUser.Username = value;
                    OnPropertyChanged(nameof(Username));
                }
            }
        }

        public string Password
        {
            get => _selectedUser?.Password;
            set
            {
                if (_selectedUser != null)
                {
                    _selectedUser.Password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        public string JobTitle
        {
            get => _selectedUser?.JobTitle;
            set
            {
                if (_selectedUser != null)
                {
                    _selectedUser.JobTitle = value;
                    OnPropertyChanged(nameof(JobTitle));
                }
            }
        }

        private async Task LoadUsers()
        {
            const int maxRetries = 10;
            const int delay = 8000; // 8 seconds
            int attempt = 0;

            while (attempt < maxRetries)
            {
                try
                {
                    var users = await _userService.GetUsers();
                    Users.Clear();
                    foreach (var user in users)
                    {
                        Users.Add(user);
                    }
                    Log.Information("Users loaded successfully");
                    break; // Exit the loop if successful
                }
                catch (HttpRequestException ex)
                {
                    attempt++;
                    Log.Error($"Error fetching users: {ex.Message}. Attempt {attempt}/{maxRetries}");
                    MessageBox.Show($"Error fetching users: {ex.Message}. Attempt {attempt}/{maxRetries}");

                    if (attempt >= maxRetries)
                    {
                        MessageBox.Show("Max retry attempts reached. Unable to fetch users.");
                        break;
                    }

                    await Task.Delay(delay);
                }
            }
        }

        private async Task AddUser()
        {
            var user = new User
            {
                Username = Username,
                Password = Password,
                JobTitle = JobTitle
            };

            try
            {
                var response = await _userService.CreateUser(user);
                LogButtonAction("Add User", response.IsSuccessStatusCode);
                if (response.IsSuccessStatusCode)
                {
                    await LoadUsers();
                    Log.Information("User added successfully");
                }
                else
                {
                    Log.Error("Error adding user");
                    MessageBox.Show("Error adding user");
                }
            }
            catch (HttpRequestException ex)
            {
                Log.Error($"Error adding user: {ex.Message}");
                MessageBox.Show($"Error adding user: {ex.Message}");
            }
        }

        private async Task UpdateUser()
        {
            if (SelectedUser != null)
            {
                SelectedUser.Username = Username;
                SelectedUser.Password = Password;
                SelectedUser.JobTitle = JobTitle;

                try
                {
                    var response = await _userService.UpdateUser(SelectedUser.Id, SelectedUser);
                    LogButtonAction("Update User", response.IsSuccessStatusCode);
                    if (response.IsSuccessStatusCode)
                    {
                        await LoadUsers();
                        Log.Information("User updated successfully");
                    }
                    else
                    {
                        Log.Error("Error updating user");
                        MessageBox.Show("Error updating user");
                    }
                }
                catch (HttpRequestException ex)
                {
                    Log.Error($"Error updating user: {ex.Message}");
                    MessageBox.Show($"Error updating user: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Please select a user to update");
            }
        }

        private async Task DeleteUser()
        {
            if (SelectedUser != null)
            {
                try
                {
                    var response = await _userService.DeleteUser(SelectedUser.Id);
                    LogButtonAction("Delete User", response.IsSuccessStatusCode);
                    if (response.IsSuccessStatusCode)
                    {
                        await LoadUsers();
                        Log.Information("User deleted successfully");
                    }
                    else
                    {
                        Log.Error("Error deleting user");
                        MessageBox.Show("Error deleting user");
                    }
                }
                catch (HttpRequestException ex)
                {
                    Log.Error($"Error deleting user: {ex.Message}");
                    MessageBox.Show($"Error deleting user: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Please select a user to delete");
            }
        }

        private void LogButtonAction(string action, bool isSuccess)
        {
            var logMessage = $"Action: {action}\nSuccess: {isSuccess}";
            Log.Information(logMessage);
            MessageBox.Show(logMessage);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
