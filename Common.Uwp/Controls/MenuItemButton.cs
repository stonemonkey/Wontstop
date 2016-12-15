using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Common.Uwp.Controls
{
    public sealed class MenuItemButton : Button
    {
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected", typeof(bool), typeof(MenuItemButton), new PropertyMetadata(default(bool), IsSelectedChanged));

        private static void IsSelectedChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (MenuItemButton) d;

            VisualStateManager.GoToState(control, control.IsSelected ? "SelectionVisible" : "SelectionCollapsed", false);
        }

        public bool IsSelected
        {
            get { return (bool) GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty SelectedBackgroundProperty = DependencyProperty.Register(
            "SelectedBackground", typeof(Brush), typeof(MenuItemButton), new PropertyMetadata(default(Brush)));

        public Brush SelectedBackground
        {
            get { return (Brush) GetValue(SelectedBackgroundProperty); }
            set { SetValue(SelectedBackgroundProperty, value); }
        }

        public static readonly DependencyProperty LabelStyleProperty = DependencyProperty.Register(
            "LabelStyle", typeof(Style), typeof(MenuItemButton), new PropertyMetadata(default(Style)));

        public Style LabelStyle
        {
            get { return (Style) GetValue(LabelStyleProperty); }
            set { SetValue(LabelStyleProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            "Label", typeof(string), typeof(MenuItemButton), new PropertyMetadata(default(string)));

        public string Label
        {
            get { return (string) GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty CompactWidthProperty = DependencyProperty.Register(
            "CompactWidth", typeof(double), typeof(MenuItemButton), new PropertyMetadata(default(double)));

        public double CompactWidth
        {
            get { return (double) GetValue(CompactWidthProperty); }
            set { SetValue(CompactWidthProperty, value); }
        }

        public MenuItemButton()
        {
            DefaultStyleKey = GetType();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            VisualStateManager.GoToState(this, IsSelected ? "SelectionVisible" : "SelectionCollapsed", false);
        }
    }
}