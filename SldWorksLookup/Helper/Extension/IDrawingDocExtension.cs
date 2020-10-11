using SolidWorks.Interop.sldworks;
using System.Collections.Generic;

namespace SldWorksLookup
{
    public static class IDrawingDocExtension
    {
        public static IEnumerable<ISheet> GetSheets(this IDrawingDoc doc)
        {
            var sheets = doc.GetSheetNames() as string[];
            if (sheets == null)
            {
                yield break;
            }
            foreach (var name in sheets)
            {
                var sheet = doc.Sheet[name];
                if (sheet == null)
                {
                    continue;
                }
                yield return sheet;
            }
        }
    }
}
