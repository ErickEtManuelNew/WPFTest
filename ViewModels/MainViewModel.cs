using System;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using WPFTest.Models;
using WPFTest.Views;
using WPFTest.Services;
using WPFTest.ViewModels;

namespace WPFTest.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private object? currentView;

        public MainViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Initialize(UserAccount? user)
        {
            CurrentUser = user;
            ShowUsers(); // Show users view by default
        }

        [RelayCommand]
        private void ShowUsers()
        {
            var usersViewModel = _serviceProvider.GetRequiredService<UsersViewModel>();
            usersViewModel.CurrentUser = CurrentUser;
            CurrentView = usersViewModel;
        }

        [RelayCommand]
        private void ShowArticles()
        {
            var articlesViewModel = _serviceProvider.GetRequiredService<ArticlesViewModel>();
            articlesViewModel.CurrentUser = CurrentUser;
            CurrentView = articlesViewModel;
        }

        [RelayCommand]
        private void ShowPublications()
        {
            // TODO: Implement publications management
            MessageBox.Show("Publications management coming soon!", 
                "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void Logout()
        {
            var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
            var loginViewModel = _serviceProvider.GetRequiredService<LoginViewModel>();
            loginWindow.DataContext = loginViewModel;
            Application.Current.MainWindow.Close();
            Application.Current.MainWindow = loginWindow;
            loginWindow.Show();
        }

        [RelayCommand]
        private void Exit()
        {
            Application.Current.Shutdown();
        }
    }
}