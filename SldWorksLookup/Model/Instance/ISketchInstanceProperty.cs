using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
