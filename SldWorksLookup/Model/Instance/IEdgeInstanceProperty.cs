using SolidWorks.Interop.sldworks;
using System;

namespace SldWorksLookup.Model
{
    public class IEdgeInstanceProperty : InstanceProperty
    {
        internal IEdgeInstanceProperty(object instance, Type type, bool init = true) : base(instance, type, init)
        {
           
        }

        protected override void InitUnSupportedMember()
        {
             _notSupportedMembers.Add(new NotSupportedMember("IGetCurveParams",PropertyClsfi.Method, NOTSUPPORTMEMBER));
        }
    }
}
