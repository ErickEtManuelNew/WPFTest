using CommunityToolkit.Mvvm.ComponentModel;
using WPFTest.Models;

namespace WPFTest.ViewModels
{
    public abstract partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private UserAccount? currentUser;
        
        protected bool CanPerformAction(UserRole minimumRole)
        {
            return CurrentUser?.Role >= minimumRole;
        }
    }
} 