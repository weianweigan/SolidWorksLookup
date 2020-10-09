using SolidWorks.Interop.sldworks;

namespace SldWorksLookup.Model
{
    public class ILoop2InstanceTree : InstanceTree
    {
        public ILoop2InstanceTree(InstanceProperty instanceProperty) : base(instanceProperty)
        {
            NodeToolTip = "Get Loop's Edge";
            NodeStatus = NodeStatus.NeedRun;
        }

        public override void AddNodesLazy()
        {
            AddNodes<ILoop2, IEdge>(loop => loop.GetEdges() as object[]);
            NodeStatus = NodeStatus.Ok;
        }
    }
}
