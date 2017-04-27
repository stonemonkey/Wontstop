using System;
using System.Globalization;
using Windows.UI.Xaml;

namespace MvvmToolkit.Uwp.StateTriggers
{
    public class EqualsStateTrigger : StateTriggerValueBase
    {
        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value", typeof(object), typeof(EqualsStateTrigger), new PropertyMetadata(null, OnValuePropertyChanged));

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (EqualsStateTrigger)d;
            obj.UpdateTrigger();
        }

        private void UpdateTrigger()
        {
            IsActive = AreValuesEqual(Value, EqualTo, true);
        }

        public object EqualTo
        {
            get { return GetValue(EqualToProperty); }
            set { SetValue(EqualToProperty, value); }
        }

        public static readonly DependencyProperty EqualToProperty =
            DependencyProperty.Register(
                "EqualTo", typeof(object), typeof(EqualsStateTrigger), new PropertyMetadata(null, OnValuePropertyChanged));

        internal static bool AreValuesEqual(object value1, object value2, bool convertType)
        {
            if (value1 == value2)
            {
                return true;
            }
            if ((value1 != null) && (value2 != null) && convertType)
            {
                return ConvertTypeEquals(value1, value2) || ConvertTypeEquals(value2, value1);
            }

            return false;
        }

        private static bool ConvertTypeEquals(object value1, object value2)
        {
            if (value2 is Enum)
            {
                value1 = ConvertToEnum(value2.GetType(), value1);
            }
            else
            {
                value1 = Convert.ChangeType(value1, value2.GetType(), CultureInfo.InvariantCulture);
            }

            return value2.Equals(value1);
        }

        private static object ConvertToEnum(Type enumType, object value)
        {
            try
            {
                return Enum.IsDefined(enumType, value) ? Enum.ToObject(enumType, value) : null;
            }
            catch
            {
                return null;
            }
        }
    }
}