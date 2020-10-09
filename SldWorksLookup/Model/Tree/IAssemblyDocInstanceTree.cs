using SolidWorks.Interop.sldworks;
using System;

namespace SldWorksLookup.Model
{
    public class IAssemblyDocInstanceTree : InstanceTree
    {
        public IAssemblyDocInstanceTree(InstanceProperty instanceProperty) : base(instanceProperty)
        {
            AddComponentsNode();
           
        }

        private void AddComponentsNode()
        {
            AddNodes<IAssemblyDoc, IComponent2>(doc => doc.GetComponents(true) as object[], comp => $"{comp?.Name}({nameof(IComponent2)})");
        }

    }
}