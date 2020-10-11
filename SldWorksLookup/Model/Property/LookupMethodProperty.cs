using System;
using System.Reflection;

namespace SldWorksLookup.Model
{
    public class LookupMethodProperty : LookupProperty
    {
        protected LookupMethodProperty(string displayName, LookupValue value, Type type, MethodInfo methodInfo, object instance): base(displayName, value,type)
        {
            MethodInfo = methodInfo;
            Instance = instance;
        }

        public static LookupMethodProperty Create(MethodInfo methodInfo,object instance)
        {
            var lookupValue = LookupValue.CreateMethodValue(methodInfo, instance);
            return new LookupMethodProperty(methodInfo.Name, lookupValue, typeof(LookupValue), methodInfo, instance)
            {
                Category = "Method"
            };
        }

        public static LookupMethodProperty CreateMsgOnly(MethodInfo methodInfo,object instance, string msg)
        {
            var lookupValue = LookupValue.CreateMethodValueMsgOnly(methodInfo, instance,msg);
            return new LookupMethodProperty(methodInfo.Name, lookupValue, typeof(LookupValue), methodInfo, instance)
            {
                Category = "Method"
            };
        }

        /// <summary>
        /// 方法信息
        /// </summary>
        public MethodInfo  MethodInfo { get; private set; }

        /// <summary>
        /// 调用该方法的实例
        /// </summary>
        public object Instance { get;private set; }

        public override PropertyClsfi PropertyClsfi { get;protected set; } = PropertyClsfi.Method;

        public override string ToString()
        {
            return MethodInfo.Name;
        }
    }

}
