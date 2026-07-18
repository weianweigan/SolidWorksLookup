using System;

namespace SldWorksLookup.Model
{
    public static class LazyLoadCompletion
    {
        public static void Run(Action load, Action markOk)
        {
            load();
            markOk();
        }
    }
}
