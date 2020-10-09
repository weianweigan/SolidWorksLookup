using SolidWorks.Interop.sldworks;

namespace SldWorksLookup.Model
{
    public class IBody2InstanceTree : InstanceTree
    {
        public IBody2InstanceTree(InstanceProperty instanceProperty) : base(instanceProperty)
        {
            NodeToolTip = "Get Body's Faces";
            NodeStatus = NodeStatus.NeedRun;
        }

        public override void AddNodesLazy()
        {
            AddNodes<IBody2, IFace2>(body => body.GetFaces() as object[]);
            NodeStatus = NodeStatus.Ok;
        }
    }
}
