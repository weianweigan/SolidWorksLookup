using SolidWorks.Interop.sldworks;

namespace SldWorksLookup.Model
{
    public class IFace2InstanceTree : InstanceTree
    {
        public IFace2InstanceTree(InstanceProperty instanceProperty) : base(instanceProperty)
        {
            NodeStatus = NodeStatus.NeedRun;
            NodeToolTip = "Get Face's Loop";
        }

        public override void AddNodesLazy()
        {
            AddNodes<IFace2, ILoop2>(face => face.GetLoops() as object[]);
            NodeStatus = NodeStatus.Ok;
            NodeToolTip = "Ok";
        }
    }
}
