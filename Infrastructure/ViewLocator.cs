using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace WPFTest.Infrastructure
{
    public class ViewLocator : DataTemplateSelector
    {
        public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
        {
            if (item == null) return null;

            var viewModelName = item.GetType().Name;
            var viewName = viewModelName.Replace("ViewModel", "View");
            var viewType = Assembly.GetExecutingAssembly().GetType($"WPFTest.Views.{viewName}");

            if (viewType == null)
            {
                return null;
            }

            return new DataTemplate
            {
                VisualTree = new FrameworkElementFactory(viewType)
            };
        }
    }
} 