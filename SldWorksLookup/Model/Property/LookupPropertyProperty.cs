using System;
using System.Reflection;

namespace SldWorksLookup.Model
{
    public class LookupPropertyProperty : LookupProperty
    {
        protected LookupPropertyProperty(string displayName, object value, Type type) : base(displayName,value ,type)
        {
        }

        public static LookupPropertyProperty Create(PropertyInfo propertyInfo,object instance)
        {
            if (propertyInfo.Name == "IMaterialPropertyValues")
            {
                return null;
            }

            var value = propertyInfo.GetValue(instance);
            var valueType = propertyInfo.PropertyType;

            if (!propertyInfo.PropertyType.IsValueType && propertyInfo.PropertyType != typeof(string))
            {
                //不是值类型
                value = LookupValue.CreatePropertyValue(value, propertyInfo);
                valueType = typeof(LookupValue);
            }
            
            //值类型
            return new LookupPropertyProperty(propertyInfo.Name, value, valueType)
            {
                Category = "Property"
            };
        }

        public static LookupPropertyProperty CreateReturnProperty(object value,Type type,string name)
        {
            return new LookupPropertyProperty(name, value, type)
            {
                Category = "Result"
            };
        }

        public static LookupPropertyProperty CreateSimpleProperty(object value, Type type, string name)
        {
            return new LookupPropertyProperty(name, value, type);
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
