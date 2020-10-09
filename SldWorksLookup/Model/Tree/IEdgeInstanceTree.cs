using SolidWorks.Interop.sldworks;

namespace SldWorksLookup.Model
{
    public class IEdgeInstanceTree : InstanceTree
    {
        public IEdgeInstanceTree(InstanceProperty instanceProperty) : base(instanceProperty)
        {
            NodeStatus = NodeStatus.NeedRun;
            NodeToolTip = "Get IEdge's Vertex";
        }

        public override void AddNodesLazy()
        {
            AddNodes<IEdge, IVertex>(edge => new object[] { edge.GetStartVertex(), edge.GetEndVertex() });
            NodeStatus = NodeStatus.Ok;
        }
    }
}
