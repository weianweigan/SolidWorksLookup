using SolidWorks.Interop.sldworks;
using System.Linq;

namespace SldWorksLookup.Model
{
    public class IComponent2InstanceTree : InstanceTree
    {
        public IComponent2InstanceTree(InstanceProperty instanceProperty) : base(instanceProperty)
        {
            NodeStatus = NodeStatus.NeedRun;
            NodeToolTip = "Get Component's Feats";
        }

        public override void AddNodesLazy()
        {
            AddNodes<IComponent2, IFeature>(comp => comp.GetFeatures().ToArray(), 
                feat => $"{feat.Name}({nameof(IFeature)})");
        }
    }
}