using SolidWorks.Interop.sldworks;
using System.Linq;

namespace SldWorksLookup.Model
{
    public class IFeatureInstanceTree : InstanceTree
    {
        public IFeatureInstanceTree(InstanceProperty instanceProperty) : base(instanceProperty)
        {
            AddSubFeatureNodes();
        }

        private void AddBodyNode()
        {
            AddNode<IFeature, IBody2>(feat => feat.IGetBody2(), node => $"{node.Name}({nameof(IBody2)})");
        }

        private void AddSubFeatureNodes()
        {
            AddNodes<IFeature, IFeature>(feat => feat.GetSubFeats().ToArray(),node => $"{node.Name}({nameof(IFeature)})");
        }
    }
}
