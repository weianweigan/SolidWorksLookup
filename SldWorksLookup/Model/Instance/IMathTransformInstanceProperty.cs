using System;

namespace SldWorksLookup.Model
{
    public class IMathTransformInstanceProperty : InstanceProperty
    {

        public IMathTransformInstanceProperty(object instance, Type type, bool init = true) : base(instance, type, init)
        {
           
        }

        protected override void InitUnSupportedMember()
        {
            _notSupportedMembers.Add(new NotSupportedMember("IArrayData", PropertyClsfi.Property, NOTSUPPORTMEMBER));
            _notSupportedMembers.Add(new NotSupportedMember("get_IArrayData", PropertyClsfi.Method, NOTSUPPORTMEMBER));
        }
    }
}
