using System;

namespace SldWorksLookup.Helper
{
    internal static class SelectionAccessScope
    {
        public static void Run(Func<bool> acquire, Action release, Action body)
        {
            if (!acquire())
                throw new InvalidOperationException("Cannot access the feature selections.");

            try
            {
                body();
            }
            finally
            {
                release();
            }
        }
    }
}
