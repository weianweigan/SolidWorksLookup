using System;

namespace SldWorksLookup.Model
{
    internal class ISketchInstanceProperty : InstanceProperty
    {
        public ISketchInstanceProperty(object instance, Type type, bool init = true) : base(instance, type, init)
        {
        }

        protected override void InitUnSupportedMember()
        {
            _notSupportedMembers.Add(new NotSupportedMember("IModelToSketchXform", PropertyClsfi.Property, NOTSUPPORTMEMBER));
        }
    }
}
