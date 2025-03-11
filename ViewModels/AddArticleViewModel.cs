using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WPFTest.Models;

namespace WPFTest.ViewModels
{
    public partial class AddArticleViewModel : ObservableObject
    {
        [ObservableProperty]
        private string title = string.Empty;

        [ObservableProperty]
        private string description = string.Empty;

        private string priceText = "0";
        public string PriceText
        {
            get => priceText;
            set
            {
                if (SetProperty(ref priceText, value))
                {
                    try
                    {
                        if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out decimal result))
                        {
                            Price = result;
                            ErrorMessage = string.Empty;
                        }
                        else
                        {
                            ErrorMessage = "Invalid price format";
                        }
                    }
                    catch (Exception)
                    {
                        ErrorMessage = "Invalid price format";
                    }
                }
            }
        }

        private string stockText = "0";
        public string StockText
        {
            get => stockText;
            set
            {
                if (SetProperty(ref stockText, value))
                {
                    try
                    {
                        if (int.TryParse(value, out int result))
                        {
                            Stock = result;
                            ErrorMessage = string.Empty;
                        }
                        else
                        {
                            ErrorMessage = "Invalid stock format";
                        }
                    }
                    catch (Exception)
                    {
                        ErrorMessage = "Invalid stock format";
                    }
                }
            }
        }

        [ObservableProperty]
        private decimal price;

        [ObservableProperty]
        private int stock;

        [ObservableProperty]
        private Category? selectedCategory;

        [ObservableProperty]
        private string? errorMessage;

        public ObservableCollection<Category> Categories { get; } = new ObservableCollection<Category>();

        partial void OnPriceChanged(decimal value)
        {
            if (value < 0)
            {
                ErrorMessage = "Price cannot be negative";
            }
            else
            {
                ErrorMessage = string.Empty;
            }
        }

        partial void OnStockChanged(int value)
        {
            if (value < 0)
            {
                ErrorMessage = "Stock cannot be negative";
            }
            else
            {
                ErrorMessage = string.Empty;
            }
        }

        public Article? GetArticle()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Title))
                {
                    ErrorMessage = "Title is required";
                    return null;
                }

                if (SelectedCategory == null)
                {
                    ErrorMessage = "Please select a category";
                    return null;
                }

                decimal parsedPrice;
                if (!decimal.TryParse(PriceText, NumberStyles.Any, CultureInfo.CurrentCulture, out parsedPrice))
                {
                    ErrorMessage = "Invalid price format";
                    return null;
                }

                int parsedStock;
                if (!int.TryParse(StockText, out parsedStock))
                {
                    ErrorMessage = "Invalid stock format";
                    return null;
                }

                if (parsedPrice < 0)
                {
                    ErrorMessage = "Price cannot be negative";
                    return null;
                }

                if (parsedStock < 0)
                {
                    ErrorMessage = "Stock cannot be negative";
                    return null;
                }

                var article = new Article
                {
                    Title = Title.Trim(),
                    Description = (Description ?? string.Empty).Trim(),
                    Price = Math.Round(parsedPrice, 2),
                    Stock = parsedStock,
                    CategoryId = SelectedCategory.Id,
                    Category = SelectedCategory,
                    CreatedAt = DateTime.Now
                };

                return article;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error creating article: {ex.Message}";
                MessageBox.Show($"Failed to create article: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
} 