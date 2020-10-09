using System.Linq;
using SolidWorks.Interop.sldworks;

namespace SldWorksLookup.Model
{
    internal class IDrawingDocInstanceTree : InstanceTree
    {
        public IDrawingDocInstanceTree(InstanceProperty instanceProperty) : base(instanceProperty)
        {
            AddSheetNodes();
        }

        private void AddSheetNodes()
        {
            AddNodes<IDrawingDoc, ISheet>(doc => doc.GetSheets().ToArray(),sheet => $"{sheet?.GetName()}({nameof(ISheet)})");
        }
    }
}
