using GalaSoft.MvvmLight;
using System;
using System.Linq;
using System.Reflection;

namespace SldWorksLookup.Model
{
    public class InstanceProperty : ObservableObject
    {
        private LookupProperties _properties;
        private bool _hasInit = false;

        #region Properties

        /// <summary>
        /// 实例
        /// </summary>
        public object Instance { get; private set; }

        /// <summary>
        /// 实例对应的类型
        /// </summary>
        public Type InstanceType { get; private set; }

        /// <summary>
        /// 此实例下的所有属性
        /// </summary>
        public LookupProperties Properties 
        { 
            get => _properties; 
            private set => Set(ref _properties ,value); 
        }

        #endregion

        #region Ctor

        public InstanceProperty()
        {

        }

        public InstanceProperty(object instance, Type type,bool init = true)
        {
            if (instance == null || type == null)
            {
                return;
            }

            Instance = instance;
            InstanceType = type;

            if (init)
            {
                SetInstance(instance, type);
            }
        }

        #endregion

        #region Methods

        public void SetInstance(object instance, Type type)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            //初始化属性列表
            InitProperties(type.Name);

            if (type.IsValueType || type == typeof(string))
            {
                var property = LookupPropertyProperty.CreateSimpleProperty(instance, type, "Value");
                Properties.Add(property);
            }
            else
            {
                //解析属性和方法
                GetProperties();
                GetMethods();
            }

            _hasInit = true;
        }

        public void LazyInit()
        {
            if (!_hasInit)
            {
                SetInstance(Instance, InstanceType);
                _hasInit = true;
            }
        }

        protected void InitProperties(string name)
        {
            if (Properties == null)
            {
                Properties = new LookupProperties() { Name = name };
            }
            else
            {
                Properties.Clear();
            }
        }

        protected void GetMethods()
        {
            var methods = InstanceType.GetMethods();

            var results = methods.AsParallel().Select(method =>
            {
                var result = TryMethodToLookup(method, Instance, out var lookupProperty);
                return Tuple.Create(result, lookupProperty);
            }).Where(p => p.Item1).Select(p => p.Item2);

            Properties.Properties.AddRange(results);

            //foreach (var method in methods)
            //{
            //    var flag = TryMethodToLookup(method, Instance, out var lookupProperty);
            //    if (flag)
            //    {
            //        Properties.Add(lookupProperty);
            //    }
            //}
        }

        protected void GetProperties()
        {
            var properties = InstanceType.GetProperties();

            var results = properties.AsParallel().Select(property =>
            {
                var result = TryPropertyToLookup(property, Instance, out var lookupProperty);
                return Tuple.Create(result, lookupProperty);
            }).Where(p => p.Item1).Select(p => p.Item2);

            Properties.Properties.AddRange(results);

            //foreach (var property in properties)
            //{
            //    var flag = TryPropertyToLookup(property, Instance, out var lookupProperty);
            //    if (flag)
            //    {
            //        Properties.Add(lookupProperty);
            //    }
            //}
        }

        protected bool TryPropertyToLookup(PropertyInfo propertyInfo, object instance, out LookupProperty lookupProperty)
        {
            lookupProperty = default;
            try
            {
                lookupProperty = LookupPropertyProperty.Create(propertyInfo, instance);
            }
            catch { }
            return lookupProperty != null;
        }

        protected bool TryMethodToLookup(MethodInfo method, object instance, out LookupProperty lookupProperty)
        {
            lookupProperty = default;
            try
            {
                lookupProperty = LookupMethodProperty.Create(method, instance);
            }
            catch { }
            return lookupProperty != null;
        }

        public override string ToString()
        {
            return InstanceType?.Name ?? (Properties?.Name ?? base.ToString());
        }

        #endregion
    }
}
