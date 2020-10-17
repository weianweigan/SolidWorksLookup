using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SldWorksLookup
{
    public static class PIDComObjectMatcherUtil
    {
        public static readonly Type[] MatchTypes = new Type[]
        {
            typeof(IBody2),typeof(IFace2),typeof(ILoop2),typeof(IEdge),typeof(IVertex),
            typeof(IFeature),typeof(IConfiguration),typeof(IModelDoc2),typeof(IComponent2)
        };

        public static Type Match(object obj)
        {
            var desType = MatchTypes.Where(p => p.IsInstanceOfType(obj)).FirstOrDefault();

            return desType;
        }
    }
}
