using SolidWorks.Interop.sldworks;
using System;
using System.Reflection;

namespace SldWorksLookup.Model
{
    internal class IFace2LookupValue : LookupValue
    {
        public IFace2LookupValue(object value, PropertyInfo propertyInfo) : base(value, propertyInfo)
        {
        }

        public IFace2LookupValue(MethodInfo value, object parentInstance) : base(value, parentInstance)
        {
        }

        protected override Type ReturnValueReDirectType(Type valueType)
        {
            Type type;

            if (Value is MethodInfo method)
            {
                switch (method.Name)
                {
                    case "GetSurface":
                        type = typeof(ISurface);
                        break;
                    default:
                        type = base.ReturnValueReDirectType(valueType);
                        break;
                }
            }
            else
            {
                type = base.ReturnValueReDirectType(valueType);
            }

            return type;
        }
    }
}
