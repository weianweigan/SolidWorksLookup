using GalaSoft.MvvmLight;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;

namespace SldWorksLookup.Model
{

    public class InstanceProperty : ObservableObject
    {
        #region Fields

        private LookupProperties _properties;
        private bool _hasInit = false;
        protected List<NotSupportedMember> _notSupportedMembers = new List<NotSupportedMember>()
        {
            //多个接口都包含此属性，但无法被托管代码执行
            new NotSupportedMember("IMaterialPropertyValues",PropertyClsfi.Property,NOTSUPPORTMEMBER)
        };
        protected const string NOTSUPPORTMEMBER = "VBA, VB.NET, C#, and C++/CLI: Not supported";

        #endregion

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

        private InstanceProperty()
        {

        }

        protected InstanceProperty(object instance, Type type,bool init = true)
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

        /// <summary>
        /// 创建描述此类型的实例对象
        /// </summary>
        /// <param name="instance">实例</param>
        /// <param name="type">类型</param>
        /// <param name="init">是否初始化</param>
        /// <returns><see cref="InstanceProperty"/>类型</returns>
        public static InstanceProperty Create(object instance,Type type,bool init = true)
        {
            InstanceProperty instanceProperty;
            switch (type.Name)
            {
                case nameof(IMathTransform):
                    instanceProperty = new IMathTransformInstanceProperty(instance, type, init);
                    break;
                case nameof(ICurve):
                    instanceProperty = new ICurveInstanceProperty(instance, type, init);
                    break;
                case nameof(IEdge):
                    instanceProperty = new IEdgeInstanceProperty(instance, type, init);
                    break;
                case nameof(IFace2):
                    instanceProperty = new IFace2InstanceProperty(instance, type, init);
                    break;
                case nameof(ISurface):
                    instanceProperty = new ISurfaceInstanceProperty(instance, type, init);
                    break;
                case nameof(ISketch):
                    instanceProperty = new ISketchInstanceProperty(instance, type, init);
                    break;
                default:
                    instanceProperty = new InstanceProperty(instance, type, init);
                    break;
            }

            return instanceProperty;
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
                InitUnSupportedMember();
            }
            else
            {
                Properties.Clear();
            }
        }

        protected void GetMethods()
        {
            var methods = InstanceType.GetMethods();

            //不需要并行，容易产生异常，速度提升不明显
            //var results = methods.AsParallel().Select(method =>
            //{
            //    var result = TryMethodToLookup(method, Instance, out var lookupProperty);
            //    return Tuple.Create(result, lookupProperty);
            //}).Where(p => p.Item1).Select(p => p.Item2);

            //Properties.Properties.AddRange(results);

            foreach (var method in methods)
            {
                if (IsMethodSupport(method.Name,out string msg))
                {
                    var flag = TryMethodToLookup(method, Instance, out var lookupProperty);
                    if (flag)
                    {
                        Properties.Add(lookupProperty);
                    }
                }
                else
                {
                    Properties.Add(LookupMethodProperty.CreateMsgOnly(method, Instance,msg));
                }
            }
        }

        protected void GetProperties()
        {
            var propertyInfos = InstanceType.GetProperties();

            //不需要并行，容易产生异常，速度提升不明显
            //var results = properties.AsParallel().Select(property =>
            //{
            //    var result = TryPropertyToLookup(property, Instance, out var lookupProperty);
            //    return Tuple.Create(result, lookupProperty);
            //}).Where(p => p.Item1).Select(p => p.Item2);

            //Properties.Properties.AddRange(results);

            foreach (var property in propertyInfos)
            {
                if (IsPropertySupport(property.Name,out string msg))
                {
                    //带有索引器的属性
                    if (property.GetMethod.GetParameters().Length > 0)
                    {
                        //生成方法
                        var flag = TryMethodToLookup(property.GetMethod, Instance, out var getLookupProperty);
                        if (flag)
                        {
                            Properties.Add(getLookupProperty);
                        }
                        flag = TryMethodToLookup(property.SetMethod, Instance, out var setLookupProperty);
                        if (flag)
                        {
                            Properties.Add(setLookupProperty);
                        }
                    }
                    else//普通属性
                    {
                        var flag = TryPropertyToLookup(property, Instance, out var lookupProperty);
                        if (flag)
                        {
                            Properties.Add(lookupProperty);
                        }
                    }
                }
                else
                {
                    //无法Snoop的属性
                    Properties.Add(LookupPropertyProperty.CreateMsgOnly(property, msg));
                }
            }
        }

        protected bool TryPropertyToLookup(PropertyInfo propertyInfo, object instance, out LookupProperty lookupProperty)
        {
            lookupProperty = default;
            try
            {
                lookupProperty = LookupPropertyProperty.Create(propertyInfo, instance);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot Get{propertyInfo.PropertyType} -- {ex.Message}");
            }
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

        /// <summary>
        /// 判断此实例中某个接口是否指出
        /// </summary>
        /// <returns></returns>
        private bool IsMemberSupport(string memberName, out string msg, PropertyClsfi propertyClsfi)
        {
            var member = _notSupportedMembers.Find(p => p.PropertyClsfi == propertyClsfi && p.Name == memberName);

            if (member.IsVaildObject())
            {
                msg = member.Msg;
                return false;
            }
            else
            {
                msg = string.Empty;
                return true;
            }
        }

        #endregion

        #region Virtual Methods

        public virtual bool IsPropertySupport(string propertyName,out string msg)
        {
            return IsMemberSupport(propertyName, out msg,PropertyClsfi.Property);
        }

        public virtual bool IsMethodSupport(string methodName, out string msg)
        {
            return IsMemberSupport(methodName, out msg, PropertyClsfi.Method);
        }

        /// <summary>
        /// 初始化不支持列表
        /// </summary>
        protected virtual void InitUnSupportedMember() { }

        #endregion

    }
}
