using SolidWorks.Interop.sldworks;
using System.Collections.Generic;

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
}
