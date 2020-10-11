using System;

namespace SldWorksLookup.Model
{
    public class IFace2InstanceProperty : InstanceProperty
    {
        public IFace2InstanceProperty(object instance, Type type, bool init = true) : base(instance, type, init)
        {
        }

        protected override void InitUnSupportedMember()
        {
            _notSupportedMembers.Add(new NotSupportedMember("IGetSurface", PropertyClsfi.Method, NOTSUPPORTMEMBER));
        }
    }
}
