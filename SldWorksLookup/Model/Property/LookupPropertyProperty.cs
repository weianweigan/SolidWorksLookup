using System;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace SldWorksLookup.Model
{
    public class LookupPropertyProperty : LookupProperty
    {
        protected LookupPropertyProperty(string displayName, object value, Type type) : base(displayName,value ,type)
        {
        }

        [HandleProcessCorruptedStateExceptions]
        public static LookupPropertyProperty Create(PropertyInfo propertyInfo,object instance)
        {
            try
            {
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
            catch(AccessViolationException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 创建一个不求值，只显示消息的属性
        /// </summary>
        [HandleProcessCorruptedStateExceptions]
        public static LookupPropertyProperty CreateMsgOnly(PropertyInfo propertyInfo,string msg)
        {
            //值类型
            return new LookupPropertyProperty(propertyInfo.Name, msg, typeof(string))
            {
                Category = "Property"
            };
        }

        public static LookupPropertyProperty CreateReturnProperty(object value,Type type,string name)
        {
            if (!type.IsValueType && type != typeof(string))
            {
                //不是值类型
                value = LookupValue.CreateValue(value, type);
                type = typeof(LookupValue);
            }
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
