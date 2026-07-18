using System;

namespace SldWorksLookup.Helper
{
    internal static class ConstructionCleanup
    {
        public static void Run(Action initialize, Action cleanup)
        {
            try
            {
                initialize();
            }
            catch
            {
                cleanup();
                throw;
            }
        }
    }
}
