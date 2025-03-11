using System.Windows;
using WPFTest.ViewModels;

namespace WPFTest.Views
{
    public partial class VerifyWindow : Window
    {
        private readonly VerifyViewModel _viewModel;

        public VerifyWindow(VerifyViewModel viewModel, string? token = null)
        {
            InitializeComponent();
            _viewModel = viewModel;
            if (token != null)
            {
                _viewModel.VerificationToken = token;
            }
            DataContext = _viewModel;
        }
    }
}