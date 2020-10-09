using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SldWorksLookup
{
    public static class IFeatureExtension
    {
        public static IEnumerable<IFeature> GetSubFeats(this IFeature feat)
        {
            var subFeat = feat.IGetFirstSubFeature();
            while (subFeat != null)
            {
                yield return subFeat;
                subFeat = subFeat.IGetNextSubFeature();
            }
        }
    }

    public static class IComponentExtension
    {
        public static IEnumerable<IFeature> GetFeatures(this IComponent2 component)
        {
            var feat = component.FirstFeature() as IFeature;
            while (feat != null)
            {
                yield return feat;
                feat = feat.GetNextFeature() as IFeature;
            }
        }
    }

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
