using System;
using System.Reflection;

namespace SldWorksLookup.Helper
{
    internal static class ExceptionUtil
    {
        public static string GetUserMessage(Exception exception)
        {
            while (exception is TargetInvocationException && exception.InnerException != null)
            {
                exception = exception.InnerException;
            }

            if (!string.IsNullOrWhiteSpace(exception?.Message))
                return exception.Message;

            return "Command failed.";
        }
    }
}
