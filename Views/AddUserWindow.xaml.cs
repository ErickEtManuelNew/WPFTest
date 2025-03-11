using System.Windows;
using WPFTest.ViewModels;

namespace WPFTest.Views
{
    public partial class AddUserWindow : Window
    {
        private readonly AddUserViewModel _viewModel;

        public AddUserWindow(AddUserViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}