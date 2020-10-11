using SolidWorks.Interop.sldworks;
using System;

namespace SldWorksLookup.Model
{
    /// <summary>
    /// <see cref="ISldWorks"/>接口实例节点，子节点为<see cref="IModelDoc2"/>节点
    /// </summary>
    public class ISldWorksInstaceTree : InstanceTree
    {
        public ISldWorksInstaceTree(InstanceProperty instanceProperty) : base(instanceProperty)
        {
            if (instanceProperty.InstanceType.Name != nameof(ISldWorks))
            {
                throw new ArgumentException($"{instanceProperty.InstanceType.FullName} is not ISldWorks");
            }

            AddDocumentNodes();
        }

        private void AddDocumentNodes()
        {
            AddNodes<ISldWorks, IModelDoc2>(app => app.GetDocuments() as object[],
                doc => $"{doc?.GetTitle()}({nameof(IModelDoc2)})");
        }
    }
}
