using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;

namespace SldWorksLookup.Model
{
    internal class IModelDoc2InstanceTree : InstanceTree
    {
        public IModelDoc2InstanceTree(InstanceProperty instanceProperty) : base(instanceProperty)
        {
            if (instanceProperty.InstanceType.Name != "IModelDoc2")
            {
                throw new ArgumentException($"{instanceProperty.InstanceType.FullName} is not IModelDoc2");
            }

            AddFeatureNodes(instanceProperty);
            AddIPartDocNode();
        }

        private void AddFeatureNodes(InstanceProperty instanceProperty)
        {
            AddNodes<IModelDoc2, IFeature>(doc => doc.FeatureManager.GetFeatures(true) as object[], node => $"{node.Name}({nameof(IFeature)})");
        }

        private void AddIPartDocNode()
        {
            var mdlDoc = InstanceProperty.Instance as IModelDoc2;
            var docType = (swDocumentTypes_e)mdlDoc.GetType();

            switch (docType)
            {
                case swDocumentTypes_e.swDocPART:
                    AddNode<IModelDoc2, IPartDoc>(doc => doc as IPartDoc);
                    break;
                case swDocumentTypes_e.swDocASSEMBLY:
                    AddNode<IModelDoc2, IAssemblyDoc>(doc => doc as IAssemblyDoc);
                    break;
                case swDocumentTypes_e.swDocDRAWING:
                    AddNode<IModelDoc2, IDrawingDoc>(doc => doc as IDrawingDoc);
                    break;
            }
        }
    }
}
