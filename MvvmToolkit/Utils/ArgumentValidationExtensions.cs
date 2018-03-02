using System;

namespace MvvmToolkit.Utils
{
    public static class ArgumentValidationExtensions
    {
        public static void ValidateNotNull(this object argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentException("Must not be null!", argumentName);
            }
        }

        public static void ValidateNotNullEmptyWhiteSpace(this string argument, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                throw new ArgumentException("Must not be null, empty or whitespace!", argumentName);
            }
        }
    }
}
