using System;

namespace SldWorksLookup.Model
{
    internal class ISurfaceInstanceProperty : InstanceProperty
    {
        public ISurfaceInstanceProperty(object instance, Type type, bool init = true) : base(instance, type, init)
        {
        }

        protected override void InitUnSupportedMember()
        {
            _notSupportedMembers.Add(new NotSupportedMember("ICylinderParams", PropertyClsfi.Property, NOTSUPPORTMEMBER));

            _notSupportedMembers.Add(new NotSupportedMember("IPlaneParams", PropertyClsfi.Property, NOTSUPPORTMEMBER));

            _notSupportedMembers.Add(new NotSupportedMember("IConeParams", PropertyClsfi.Property, NOTSUPPORTMEMBER));

            _notSupportedMembers.Add(new NotSupportedMember("ISphereParams", PropertyClsfi.Property, NOTSUPPORTMEMBER));

            _notSupportedMembers.Add(new NotSupportedMember("ITorusParams", PropertyClsfi.Property, NOTSUPPORTMEMBER));
        }
    }
}