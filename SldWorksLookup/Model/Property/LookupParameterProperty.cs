using System;
using System.Reflection;

namespace SldWorksLookup.Model
{
    public class LookupParameterProperty : LookupProperty
    {
        public LookupParameterProperty(ParameterInfo parameter, object value) : base(parameter.Name, value, parameter.ParameterType)
        {
            IsReadOnly = false;
        }

        public LookupParameterProperty(ParameterInfo parameter) : base(parameter.Name, CreateInstace(parameter.ParameterType), parameter.ParameterType)
        {
            IsReadOnly = false;
        }

        public static object CreateInstace(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            else
            {
                return new object();
            }
        }
    }

}
