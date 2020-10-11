using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SldWorksLookup.Model
{
    public class IRefPlaneLookupValue : LookupValue
    {
        public const string TYPENAME = nameof(IRefPlane);

        public IRefPlaneLookupValue(MethodInfo value, object parentInstance) : base(value, parentInstance)
        {

        }

        public IRefPlaneLookupValue(object value, PropertyInfo propertyInfo) : base(value, propertyInfo)
        {
        }

        protected override Type PropertyReDirectType(Type valueType)
        {
            Type type;
            switch (_propertyInfo.Name)
            {
                case "BoundingBox":
                    type = typeof(IMathPoint);
                    break;
                case "CornerPoints":
                    type = typeof(IMathPoint);
                    break;
                default:
                    type = base.PropertyReDirectType(valueType);
                    break;
            }

            return type;
        }
    }

}
