using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace MvvmToolkit.Uwp.StateTriggers
{
    public class AdaptiveTrigger : StateTriggerValueBase
    {
        public double MinWindowWidth
        {
            get { return (double) GetValue(MinWindowWidthProperty); }
            set { SetValue(MinWindowWidthProperty, value); }
        }

        public static readonly DependencyProperty MinWindowWidthProperty =
            DependencyProperty.Register("MinWindowWidth", typeof(double), typeof(AdaptiveTrigger),
                new PropertyMetadata(0.0, OnWindowSizeChanged));

        public double MinWindowHeight
        {
            get { return (double) GetValue(MinWindowHeightProperty); }
            set { SetValue(MinWindowHeightProperty, value); }
        }

        public static readonly DependencyProperty MinWindowHeightProperty =
            DependencyProperty.Register("MinWindowHeight", typeof(double), typeof(AdaptiveTrigger),
                new PropertyMetadata(0.0, OnWindowSizeChanged));

        public double MaxWindowWidth
        {
            get { return (double) GetValue(MaxWindowWidthProperty); }
            set { SetValue(MaxWindowWidthProperty, value); }
        }

        public static readonly DependencyProperty MaxWindowWidthProperty =
            DependencyProperty.Register("MaxWindowWidth", typeof(double), typeof(AdaptiveTrigger),
                new PropertyMetadata(double.MaxValue, OnWindowSizeChanged));

        public double MaxWindowHeight
        {
            get { return (double) GetValue(MaxWindowHeightProperty); }
            set { SetValue(MaxWindowHeightProperty, value); }
        }

        public static readonly DependencyProperty MaxWindowHeightProperty =
            DependencyProperty.Register("MaxWindowHeight", typeof(double), typeof(AdaptiveTrigger),
                new PropertyMetadata(double.MaxValue, OnWindowSizeChanged));

        private static void OnWindowSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignMode.DesignModeEnabled)
            {
                return;
            }

            var instance = (AdaptiveTrigger) d;
            instance.UpdateTrigger();
        }

        private void UpdateTrigger()
        {
            var window = CoreApplication.GetCurrentView()?.CoreWindow;
            if (window != null)
            {
                OnCoreWindowOnSizeChanged(new Size(window.Bounds.Width, window.Bounds.Height));
            }
        }

        public AdaptiveTrigger()
        {
            if (DesignMode.DesignModeEnabled)
            {
                return;
            }

            var window = CoreApplication.GetCurrentView()?.CoreWindow;
            if (window == null)
            {
                return;
            }

            var weakEvent = new WeakEventListener<AdaptiveTrigger, CoreWindow, WindowSizeChangedEventArgs>(this)
            {
                OnEventAction = (instance, s, e) => OnCoreWindowOnSizeChanged(s, e),
                OnDetachAction = (instance, weakEventListener) => window.SizeChanged -= weakEventListener.OnEvent
            };
            window.SizeChanged += weakEvent.OnEvent;
        }

        private void OnCoreWindowOnSizeChanged(CoreWindow sender, WindowSizeChangedEventArgs args)
        {
            OnCoreWindowOnSizeChanged(args.Size);
        }

        private void OnCoreWindowOnSizeChanged(Size size)
        {
            IsActive = (size.Height >= MinWindowHeight) && (size.Width >= MinWindowWidth) &&
                (size.Height < MaxWindowHeight) && (size.Width < MaxWindowWidth) &&
                (MinWindowHeight <= MaxWindowHeight) && (MinWindowWidth <= MaxWindowWidth);
        }
    }
}