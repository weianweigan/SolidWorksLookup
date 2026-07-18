using System;

namespace SldWorksLookup.Helper
{
    internal static class EditScope
    {
        public static void Run(Action enter, Action exit, Action body)
        {
            enter();
            try
            {
                body();
            }
            finally
            {
                exit();
            }
        }
    }
}
