using System;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WPFTest.Repositories;

namespace WPFTest.ViewModels
{
    public partial class VerifyViewModel : ObservableObject
    {
        private readonly IUnitOfWork _unitOfWork;

        [ObservableProperty]
        private string? verificationToken;

        [ObservableProperty]
        private string? errorMessage;

        public VerifyViewModel(IUnitOfWork unitOfWork, string? token = null)
        {
            _unitOfWork = unitOfWork;
            VerificationToken = token;

            if (!string.IsNullOrEmpty(token))
            {
                VerifyCommand.ExecuteAsync(null);
            }
        }

        [RelayCommand]
        private async Task Verify()
        {
            try
            {
                ErrorMessage = null;

                if (string.IsNullOrWhiteSpace(VerificationToken))
                {
                    ErrorMessage = "Please enter your verification code";
                    return;
                }

                var success = await _unitOfWork.Users.VerifyUserAsync(VerificationToken.Trim());
                if (success)
                {
                    await _unitOfWork.SaveChangesAsync();
                    MessageBox.Show(
                        "Your email has been verified successfully! You can now log in.",
                        "Verification Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                    CloseWindow(true);
                }
                else
                {
                    ErrorMessage = "Invalid or expired verification code";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Verification failed: {ex.Message}";
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