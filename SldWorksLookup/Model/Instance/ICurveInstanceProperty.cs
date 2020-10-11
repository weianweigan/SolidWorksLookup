using System;

namespace SldWorksLookup.Model
{
    public class ICurveInstanceProperty : InstanceProperty
    {
        internal ICurveInstanceProperty(object instance, Type type, bool init = true) : base(instance, type, init)
        {
        }

        protected override void InitUnSupportedMember()
        {
            _notSupportedMembers.Add(new NotSupportedMember("ILineParams", PropertyClsfi.Property, NOTSUPPORTMEMBER));
            _notSupportedMembers.Add(new NotSupportedMember("ICircleParams", PropertyClsfi.Property, NOTSUPPORTMEMBER));
        }
    }
}
