using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using WPFTest.Models;
using WPFTest.Services;
using WPFTest.Views;

namespace WPFTest.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IDatabaseService _databaseService;
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private bool isLoading;

        public LoginViewModel(IDatabaseService databaseService, IServiceProvider serviceProvider)
        {
            _databaseService = databaseService;
            _serviceProvider = serviceProvider;
        }

        [RelayCommand]
        private async Task Login(PasswordBox passwordBox)
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                MessageBox.Show("Please enter both username and password.", 
                    "Login Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IsLoading = true;
            try
            {
                var user = await _databaseService.AuthenticateAsync(Username, passwordBox.Password);
                if (user != null)
                {
                    if (!user.IsActive)
                    {
                        MessageBox.Show("Your account is not active. Please verify your email.", 
                            "Login Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                    var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
                    mainViewModel.Initialize(user);
                    mainWindow.DataContext = mainViewModel;

                    Application.Current.MainWindow.Close();
                    Application.Current.MainWindow = mainWindow;
                    mainWindow.Show();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.", 
                        "Login Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login failed: {ex.Message}", 
                    "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void Register()
        {
            var registerWindow = _serviceProvider.GetRequiredService<RegisterWindow>();
            var registerViewModel = _serviceProvider.GetRequiredService<RegisterViewModel>();
            registerWindow.DataContext = registerViewModel;
            registerWindow.Owner = Application.Current.MainWindow;
            registerWindow.ShowDialog();
        }

        [RelayCommand]
        private void VerifyAccount()
        {
            var verifyWindow = _serviceProvider.GetRequiredService<VerifyWindow>();
            verifyWindow.Owner = Application.Current.MainWindow;
            verifyWindow.ShowDialog();
        }
    }
}