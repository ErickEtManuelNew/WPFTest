using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WPFTest.Models;
using WPFTest.Services;

namespace WPFTest.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;
        private readonly IEmailService _emailService;

        [ObservableProperty]
        private string? username;

        [ObservableProperty]
        private string? email;

        [ObservableProperty]
        private string? errorMessage;

        public RegisterViewModel(IDatabaseService databaseService, IEmailService emailService)
        {
            _databaseService = databaseService;
            _emailService = emailService;
        }

        [RelayCommand]
        private async Task Register(PasswordBox passwordBox)
        {
            try
            {
                ErrorMessage = null;

                if (string.IsNullOrWhiteSpace(Username))
                {
                    ErrorMessage = "Please enter a username";
                    return;
                }

                if (string.IsNullOrWhiteSpace(Email))
                {
                    ErrorMessage = "Please enter an email address";
                    return;
                }

                if (string.IsNullOrWhiteSpace(passwordBox.Password))
                {
                    ErrorMessage = "Please enter a password";
                    return;
                }

                // Check if username already exists
                if (await _databaseService.UsernameExistsAsync(Username))
                {
                    ErrorMessage = "Username already exists";
                    return;
                }

                // Check if email already exists
                if (await _databaseService.EmailExistsAsync(Email))
                {
                    ErrorMessage = "Email already exists";
                    return;
                }

                // Generate verification token
                var verificationToken = Guid.NewGuid().ToString("N");

                // Create user account
                var user = new UserAccount
                {
                    Username = Username.Trim(),
                    Email = Email.Trim(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordBox.Password),
                    Role = UserRole.Editor, // Default role
                    IsActive = false, // Account needs verification
                    VerificationToken = verificationToken,
                    CreatedAt = DateTime.Now
                };

                // Save user to database
                if (await _databaseService.AddUserAsync(user))
                {
                    try
                    {
                        // Send verification email
                        await SendVerificationEmail(user.Email, verificationToken);

                        MessageBox.Show(
                            "Registration successful! Please check your email to verify your account.",
                            "Registration Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );

                        // Close the registration window
                        CloseWindow();
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = $"Account created but failed to send verification email: {ex.Message}";
                        MessageBox.Show(
                            "Your account was created but we couldn't send the verification email. Please contact support.",
                            "Email Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning
                        );
                    }
                }
                else
                {
                    ErrorMessage = "Failed to create account";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Registration failed: {ex.Message}";
            }
        }

        private async Task SendVerificationEmail(string email, string token)
        {
            var verificationLink = $"cmsapp://verify?token={token}";
            var subject = "Verify your CMS account";
            var body = $@"
                <h2>Welcome to CMS!</h2>
                <p>Your verification code is: <strong>{token}</strong></p>
                <p>You can verify your account by:</p>
                <ol>
                    <li>Opening the CMS application</li>
                    <li>Clicking on 'Verify Account' in the login window</li>
                    <li>Entering your verification code</li>
                </ol>
                <p>If you didn't create an account, you can safely ignore this email.</p>";

            await _emailService.SendEmailAsync(email, subject, body);
        }

        [RelayCommand]
        private void Cancel()
        {
            CloseWindow();
        }

        private void CloseWindow()
        {
            if (Application.Current.Windows.Count > 0)
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.DataContext == this)
                    {
                        window.DialogResult = false;
                        window.Close();
                        break;
                    }
                }
            }
        }
    }
}