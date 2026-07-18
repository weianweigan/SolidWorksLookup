using System;
using System.Reflection;

namespace SldWorksLookup.Model
{
    public class LookupParameterProperty : LookupProperty
    {
        public LookupParameterProperty(ParameterInfo parameter, object value) : base(parameter.Name, value, GetEffectiveType(parameter.ParameterType))
        {
            IsReadOnly = false;
        }

        public LookupParameterProperty(ParameterInfo parameter) : base(parameter.Name, CreateInstace(parameter.ParameterType), GetEffectiveType(parameter.ParameterType))
        {
            IsReadOnly = false;
        }

        public static object CreateInstace(Type type)
        {
            var effectiveType = GetEffectiveType(type);
            if (effectiveType.IsValueType && Nullable.GetUnderlyingType(effectiveType) == null)
            {
                return Activator.CreateInstance(effectiveType);
            }

            return null;
        }

        internal static Type GetEffectiveType(Type type)
        {
            return type.IsByRef ? type.GetElementType() : type;
        }
    }

}
