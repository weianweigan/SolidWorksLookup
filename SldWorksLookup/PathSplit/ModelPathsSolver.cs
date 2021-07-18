using SolidWorks.Interop.sldworks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SldWorksLookup.PathSplit
{
    public class ModelPathsSolver
    {
        private readonly IModelDoc2 _doc;

        public ModelPathsSolver(IModelDoc2 doc)
        {
            _doc = doc;
        }

        public List<SketchWrapper> GetSketchWrappers()
        {
            return (_doc.FeatureManager.GetFeatures(false) as object[])
                .Cast<IFeature>()
                .Where(p => p.GetTypeName2() == "ProfileFeature")
                .Select(p => new SketchWrapper(p)).ToList();
        }
    }
}
