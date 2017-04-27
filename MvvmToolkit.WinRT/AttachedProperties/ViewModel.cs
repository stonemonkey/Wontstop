using System;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml;

namespace MvvmToolkit.WinRT.AttachedProperties
{
    public class Model : Attribute
    {
    }

    public class ViewModel
    {
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.RegisterAttached(
                "Model",
                typeof(object),
                typeof(ViewModel),
                new PropertyMetadata(null, OnModelChanged));

        public static void SetModel(UIElement element, object value)
        {
            element.SetValue(ModelProperty, value);
        }

        public static object GetModel(UIElement element)
        {
            return element.GetValue(ModelProperty);
        }

        // TODO: rework error handling
        private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = (FrameworkElement) d;
            if (element?.DataContext == null)
            {
                return;
            }

            var viewModel = element.DataContext;
            var type = viewModel.GetType();
            var property = type.GetRuntimeProperties()
                .Single(x => x.GetCustomAttributes<Model>(true).Any());
            if (property.PropertyType == e.NewValue.GetType())
            {
                property.SetValue(viewModel, e.NewValue);
            }
        }
    }
}
