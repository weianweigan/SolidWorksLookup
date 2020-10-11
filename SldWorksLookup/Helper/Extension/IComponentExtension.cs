using SolidWorks.Interop.sldworks;
using System.Collections.Generic;

namespace SldWorksLookup
{
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
}
