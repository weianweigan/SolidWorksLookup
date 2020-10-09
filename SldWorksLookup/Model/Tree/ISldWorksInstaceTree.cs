using SolidWorks.Interop.sldworks;
using System;
using System.Linq;

namespace SldWorksLookup.Model
{
    public class ISldWorksInstaceTree : InstanceTree
    {
        public ISldWorksInstaceTree(InstanceProperty instanceProperty) : base(instanceProperty)
        {
            if (instanceProperty.InstanceType.Name != "ISldWorks")
            {
                throw new ArgumentException($"{instanceProperty.InstanceType.FullName} is not ISldWorks");
            }
            AddDocumentNodes();
        }

        private void AddDocumentNodes()
        {
            AddNodes<ISldWorks, IModelDoc2>(app => app.GetDocuments() as object[]);

            ////Get Documents
            //var app = InstanceProperty.Instance as ISldWorks;
            //var docs = app.GetDocuments() as object[];

            ////Check
            //if (docs == null)
            //{
            //    return;
            //}

            ////ToNodes
            //var nodes = docs.OfType<IModelDoc2>()
            //    .Select(doc => InstanceTree.Create(new InstanceProperty(doc, typeof(IModelDoc2), false)));

            ////Add to Children
            //foreach (var node in nodes)
            //{
            //    Children.Add(node);
            //}
        }
    }
}
