using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WPFTest.Models;
using WPFTest.Repositories;

namespace WPFTest.ViewModels
{
    public partial class AddUserViewModel : ObservableObject
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddUserViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private UserRole role = UserRole.Editor;

        [ObservableProperty]
        private string? errorMessage;

        public IEnumerable<UserRole> AvailableRoles => Enum.GetValues<UserRole>();

        [RelayCommand]
        private async Task AddUser(PasswordBox passwordBox)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Username))
                {
                    ErrorMessage = "Username is required";
                    return;
                }

                if (string.IsNullOrWhiteSpace(Email))
                {
                    ErrorMessage = "Email is required";
                    return;
                }

                if (string.IsNullOrWhiteSpace(passwordBox.Password))
                {
                    ErrorMessage = "Password is required";
                    return;
                }

                // Check if username already exists
                var existingUser = await _unitOfWork.Users.FindAsync(u => u.Username == Username);
                if (existingUser.Any())
                {
                    ErrorMessage = "Username already exists";
                    return;
                }

                // Check if email already exists
                if (!await _unitOfWork.Users.IsEmailUniqueAsync(Email))
                {
                    ErrorMessage = "Email already exists";
                    return;
                }

                var user = new UserAccount
                {
                    Username = Username.Trim(),
                    Email = Email.Trim(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordBox.Password),
                    Role = Role,
                    IsActive = true, // Admin-created accounts are active by default
                    CreatedAt = DateTime.Now
                };

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();
                
                MessageBox.Show("User added successfully!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                CloseWindow(true);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error adding user: {ex.Message}";
                MessageBox.Show($"Failed to add user: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            CloseWindow(false);
        }

        private void CloseWindow(bool result)
        {
            if (Application.Current.Windows.Count > 0)
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.DataContext == this)
                    {
                        window.DialogResult = result;
                        window.Close();
                        break;
                    }
                }
            }
        }
    }
}