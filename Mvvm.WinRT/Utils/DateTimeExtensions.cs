using System;

namespace Mvvm.WinRT.Utils
{
    /// <summary>
    /// Helpers for working with DateTime.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts Unix time stamp to DateTime
        /// </summary>
        /// <param name="unixTimeStamp">Unix time stamp to apply conversion.</param>
        /// <returns>Conversion result.</returns>
        public static DateTime ToDateTime(this double unixTimeStamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimeStamp).ToLocalTime();
        }

        /// <summary>
        /// Converts DateTime to Unix time stamp.
        /// </summary>
        /// <param name="dateTime">The DateTime instance on which the conversion is applyed.</param>
        /// <returns>Conversion result.</returns>
        public static double ToUnixTimestamp(this DateTime dateTime)
        {
            return dateTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToLocalTime()).TotalSeconds;
        }

        /// <summary>
        /// Converts DateTimeOffset to Unix time stamp.
        /// </summary>
        /// <param name="dateTime">The DateTimeOffset instance on which the conversion is applyed.
        /// </param>
        /// <returns>Conversion result.</returns>
        public static double ToUnixTimestamp(this DateTimeOffset dateTime)
        {
            return dateTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToLocalTime()).TotalSeconds;
        }

        /// <summary>
        /// Determines if applied instance represents an earlier point in time than the target 
        /// instance.
        /// </summary>
        /// <param name="dateTime">Applied instance.</param>
        /// <param name="target">Target instance.</param>
        /// <returns>True if earlier, false otherwise.</returns>
        public static bool Earlier(this DateTime dateTime, DateTime target)
        {
            return dateTime.CompareTo(target) < 0;
        }

        /// <summary>
        /// Determines if applied instance represents an later point in time than the target 
        /// instance.
        /// </summary>
        /// <param name="dateTime">Applied instance.</param>
        /// <param name="target">Target instance.</param>
        /// <returns>True if later, false otherwise.</returns>
        public static bool Later(this DateTime dateTime, DateTime target)
        {
            return dateTime.CompareTo(target) > 0;
        }
    }
}