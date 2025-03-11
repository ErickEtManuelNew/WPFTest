using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WPFTest.Models;
using WPFTest.ViewModels;

namespace WPFTest.Views
{
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;

        public MainWindow(IServiceProvider serviceProvider, UserAccount? currentUser = null)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;

            // Initialize MainViewModel with dependency injection
            var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
            mainViewModel.Initialize(currentUser);
            DataContext = mainViewModel;
        }
    }
}