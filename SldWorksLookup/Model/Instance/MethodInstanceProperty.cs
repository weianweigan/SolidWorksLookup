using System;
using System.Linq;
using System.Reflection;

namespace SldWorksLookup.Model
{
    public class MethodInstanceProperty : InstanceProperty
    {
        public object ParentInstance { get; protected set; }

        public MethodInfo MethodInfo { get; protected set; }

        public MethodInstanceProperty(MethodInfo method,object parentInstance):base(parentInstance,typeof(MethodInfo),false)
        {
            SetMethod(method,parentInstance);
        }

        public void SetMethod(MethodInfo method, object parentInstance)
        {
            MethodInfo = method;
            ParentInstance = parentInstance;

            //Parameters => Properties
            var parameters = MethodInfo.GetParameters();

            InitProperties(MethodInfo.Name);

            var properties = parameters.Select(p => ParameterToLookup(p));

            int order = 0;
            foreach (var property in properties)
            {
                property.PropertyOrder = order++;
                Properties.Add(property);
            }
            
        }

        private LookupProperty ParameterToLookup(ParameterInfo p)
        {
            return new LookupParameterProperty(p)
            { 
                Category = "Parameters"
            };
        }
        
        /// <summary>
        /// 执行方法
        /// </summary>
        internal void Invoke()
        {
            //找到参数
            var parameters = Properties.Properties.OfType<LookupParameterProperty>();

            var nullParamenter = parameters.FirstOrDefault(p => p.Value == null);
            if (nullParamenter != null)
            {
                throw new ArgumentNullException($"{nullParamenter.PropertyType.Name} is Null");
            }

            var parametersValue = parameters.Select(p => p.Value).ToArray();

            var result = MethodInfo.Invoke(ParentInstance, parametersValue);

            //Result => Result;
            var resultProperty = LookupPropertyProperty.CreateReturnProperty(result, MethodInfo.ReturnType, MethodInfo.ReturnType.Name);

            Properties.AddReturnProperty(resultProperty);
        }

        public override string ToString()
        {
            return MethodInfo?.Name ?? base.ToString();
        }
    }
}
