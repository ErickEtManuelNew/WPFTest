using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using WPFTest.Models;
using WPFTest.Repositories;
using WPFTest.Views;

namespace WPFTest.ViewModels
{
    public partial class UsersViewModel : BaseViewModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceProvider _serviceProvider;
        public ObservableCollection<UserAccount> Users { get; } = new();

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private UserRole? selectedRole;

        [ObservableProperty]
        private UserAccount? selectedUser;

        public ObservableCollection<UserRole> AvailableRoles { get; } = new(Enum.GetValues<UserRole>());

        public UsersViewModel(IUnitOfWork unitOfWork, IServiceProvider serviceProvider, UserAccount? currentUser)
        {
            _unitOfWork = unitOfWork;
            _serviceProvider = serviceProvider;
            CurrentUser = currentUser;
            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            try
            {
                var users = await _unitOfWork.Users.FindAsync(u => 
                    (SelectedRole == null || u.Role == SelectedRole) &&
                    (string.IsNullOrEmpty(SearchText) || 
                     u.Email.Contains(SearchText) || 
                     u.Username.Contains(SearchText)));

                Users.Clear();
                foreach (var user in users)
                {
                    Users.Add(user);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void ClearFilters()
        {
            SelectedRole = null;
            SearchText = string.Empty;
            LoadDataAsync();
        }

        [RelayCommand]
        private void AddUser()
        {
            if (!CanPerformAction(UserRole.Administrator))
            {
                MessageBox.Show("You don't have permission to add users.", 
                    "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var addUserWindow = _serviceProvider.GetRequiredService<AddUserWindow>();
            addUserWindow.Owner = Application.Current.MainWindow;

            if (addUserWindow.ShowDialog() == true)
            {
                LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task ToggleUserStatus(UserAccount user)
        {
            if (!CanPerformAction(UserRole.Administrator))
            {
                MessageBox.Show("You don't have permission to modify user status.", 
                    "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (user.Id == CurrentUser?.Id)
            {
                MessageBox.Show("You cannot modify your own account status.", 
                    "Operation Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var dbUser = await _unitOfWork.Users.GetByIdAsync(user.Id);
                if (dbUser != null)
                {
                    dbUser.IsActive = !dbUser.IsActive;
                    _unitOfWork.Users.Update(dbUser);
                    await _unitOfWork.SaveChangesAsync();
                    
                    user.IsActive = dbUser.IsActive; // Update the UI
                    MessageBox.Show($"User status updated successfully. User is now {(user.IsActive ? "active" : "inactive")}.", 
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating user status: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            LoadDataAsync();
        }

        partial void OnSelectedRoleChanged(UserRole? value)
        {
            LoadDataAsync();
        }
    }
}