using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WPFTest.Models;
using WPFTest.Repositories;
using WPFTest.Views;

namespace WPFTest.ViewModels
{
    public partial class ArticlesViewModel : BaseViewModel
    {
        private readonly IUnitOfWork _unitOfWork;
        public ObservableCollection<Article> Articles { get; } = new ObservableCollection<Article>();
        public ObservableCollection<Category> Categories { get; } = new ObservableCollection<Category>();

        [ObservableProperty]
        private Category? selectedCategory;

        [ObservableProperty]
        private string searchText = string.Empty;

        public ArticlesViewModel(IUnitOfWork unitOfWork, UserAccount? currentUser)
        {
            _unitOfWork = unitOfWork;
            CurrentUser = currentUser;
            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            try
            {
                var categories = await _unitOfWork.Categories.GetCategoriesByTypeAsync(CategoryType.Article);
                Categories.Clear();
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }
                await LoadArticlesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadArticlesAsync()
        {
            try
            {
                var articles = await _unitOfWork.Articles.GetFilteredArticlesAsync(SelectedCategory?.Id, SearchText);
                Articles.Clear();
                foreach (var article in articles)
                {
                    Articles.Add(article);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading articles: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task RefreshArticles()
        {
            await LoadArticlesAsync();
        }

        [RelayCommand]
        private void ClearFilters()
        {
            SelectedCategory = null;
            SearchText = string.Empty;
            _ = LoadArticlesAsync();
        }

        partial void OnSelectedCategoryChanged(Category? value)
        {
            _ = LoadArticlesAsync();
        }

        partial void OnSearchTextChanged(string value)
        {
            _ = LoadArticlesAsync();
        }

        [RelayCommand]
        private async Task AddArticle()
        {
            try
            {
                var addArticleWindow = new AddArticleWindow();
                var viewModel = (AddArticleViewModel)addArticleWindow.DataContext;
                
                // Only add article categories
                foreach (var category in Categories.Where(c => c.Type == CategoryType.Article))
                {
                    viewModel.Categories.Add(category);
                }

                if (viewModel.Categories.Count > 0)
                {
                    viewModel.SelectedCategory = viewModel.Categories[0];
                }

                if (addArticleWindow.ShowDialog() == true)
                {
                    var article = viewModel.GetArticle();
                    if (article != null)
                    {
                        try
                        {
                            await _unitOfWork.Articles.AddAsync(article);
                            await _unitOfWork.SaveChangesAsync();
                            await LoadArticlesAsync(); // Refresh the list after adding
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Database error while adding article: {ex.Message}", 
                                "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to add article: {ex.Message}", 
                    "Add Article Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}