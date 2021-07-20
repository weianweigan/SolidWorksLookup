using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
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
            _doc = doc ?? throw new System.ArgumentNullException(nameof(doc));
        }

        public List<SketchWrapper> GetSketchWrappers()
        {
            if (_doc.GetType() == (int)swDocumentTypes_e.swDocASSEMBLY)
            {
                var assDoc = _doc as IAssemblyDoc;

                var skes = assDoc.GetSkeFeats();

                return skes.Select(p => new SketchWrapper(p.Item1,p.Item2)).ToList(); 
                
            }else if(_doc.GetType() == (int)swDocumentTypes_e.swDocPART)
            {
                return (_doc.FeatureManager.GetFeatures(false) as object[])
                    .Cast<IFeature>()
                    .Where(p => p.GetTypeName2() == "ProfileFeature")
                    .Select(p => new SketchWrapper(p)).ToList();
            }
            else
            {
                throw new System.NotSupportedException($"不支持的文档类型：{_doc.GetType()}");
            }
        }

        /// <summary>
        /// 导出当前模型的所有草图中的第一个链
        /// </summary>
        public void Export()
        {
            var sketchWappers = GetSketchWrappers();

            foreach (var skeWrapper in sketchWappers)
            {
                var chain = skeWrapper.GetChains().FirstOrDefault();

                chain?.ExportClick();
            }
        }
    }
}
