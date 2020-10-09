using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;

namespace SldWorksLookup.Model
{
    public class IPartDocInstanceTree : InstanceTree
    {
        public IPartDocInstanceTree(InstanceProperty instanceProperty) : base(instanceProperty)
        {
            NodeStatus = NodeStatus.NeedRun;
            NodeToolTip = "Get Bodies";
        }

        public override void AddNodesLazy()
        {
            AddNodes<IPartDoc, IBody2>(doc => doc.GetBodies((int)swBodyType_e.swAllBodies) as object[]);
            NodeStatus = NodeStatus.Ok;
        }

    }
}
