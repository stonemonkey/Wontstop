using System;
using System.Reflection;
using Windows.UI.Xaml.Markup;

namespace Common.Uwp.Utils
{
    /// <summary>
    /// Boolean to object and viceversa helpers
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// Safely casts an object to a boolean
        /// </summary>
        /// <param name="parameter">Parameter to cast to a boolean</param>
        /// <returns>Bool value or false if cast failed</returns>
        public static bool TryParseBool(this object parameter)
        {
            var parsed = false;
            if (parameter != null)
            {
                bool.TryParse(parameter.ToString(), out parsed);
            }

            return parsed;
        }

        /// <summary>
        /// Converts a value from a source type to a target type.
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <param name="targetType">The target type</param>
        /// <returns>The converted value</returns>
        public static object Convert(this object value, Type targetType)
        {
            if (targetType.IsInstanceOfType(value))
            {
                return value;
            }

            return XamlBindingHelper.ConvertValue(targetType, value);
        }
    }
}