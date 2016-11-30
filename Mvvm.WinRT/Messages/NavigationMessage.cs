namespace Mvvm.WinRT.Messages
{
    public class NavigationMessage
    {
        public object Sender { get; set; }

        public object Parameter { get; set; }

        public int NavigationMode { get; set; }
    }
}