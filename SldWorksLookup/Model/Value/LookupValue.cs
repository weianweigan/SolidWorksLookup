using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SldWorksLookup.View;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;

namespace SldWorksLookup.Model
{
    public class LookupValue : ObservableObject
    {
        #region Fields

        private RelayCommand _openCommand;
        private string _valueName;
        private bool _hasExecuted;
        private bool _notSupport;
        protected PropertyInfo _propertyInfo;

        #endregion

        #region Ctor

        private LookupValue() { }

        /// <summary>
        /// 构建一个属性值对象
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueType"></param>
        /// <param name="propertyClsfi"></param>
        protected LookupValue(object value,PropertyInfo propertyInfo)
        {
            Value = value;
            _propertyInfo = propertyInfo;
            var type = propertyInfo.PropertyType;

            if (type.IsInstanceOfType(typeof(string)))
            {
                type = typeof(string);
            }

            ValueType = type;
            PropertyClsfi = PropertyClsfi.Property;
            ValueName = ToPropertyValueString();
        }

        /// <summary>
        /// 初始化一个Method对象
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parentInstance"></param>
        protected LookupValue(MethodInfo value,object parentInstance)
        {
            Value = value;
            ValueType = typeof(MethodInfo);
            PropertyClsfi = PropertyClsfi.Method;
            ParentInstance = parentInstance;
            ValueName = value.Name;
        }

        public static LookupValue CreateMethodValue(MethodInfo method, object parentInstance)
        {
            LookupValue methodValue;
            switch (method.DeclaringType.Name)
            {
                case IRefPlaneLookupValue.TYPENAME:
                    methodValue = new IRefPlaneLookupValue(method, parentInstance);
                    break;
                case nameof(IFace2):
                    methodValue = new IFace2LookupValue(method, parentInstance);
                    break;
                default:
                    methodValue = new LookupValue(method, parentInstance);
                    break;
            }
            return methodValue;
        }

        public static LookupValue CreatePropertyValue(object value, PropertyInfo propertyInfo)
        {
                LookupValue property;
            
                switch (propertyInfo.DeclaringType.Name)
                {
                    case IRefPlaneLookupValue.TYPENAME:
                        property = new IRefPlaneLookupValue(value, propertyInfo);
                        break;
                    case nameof(IFace2):
                        property = new IFace2LookupValue(value, propertyInfo);
                        break;
                    default:
                        property = new LookupValue(value, propertyInfo);
                        break;
                }

            return property;
        }

        public static LookupValue CreateValue(object value,Type type)
        {
            var lookup = new LookupValue()
            {
                Value = value,
                ValueType = type,
                PropertyClsfi = PropertyClsfi.Property,
            };
            lookup.ValueName = lookup.ToPropertyValueString();
            return lookup;
        }

        public static LookupValue CreateMethodValueMsgOnly(MethodInfo method, object parentInstance, string msg)
        {
            return new LookupValue(method, parentInstance)
            {
                ValueName = msg,
                _notSupport = true
            };
        }

        #endregion

        #region Properties

        /// <summary>
        /// 类型
        /// </summary>
        public PropertyClsfi PropertyClsfi { get; protected set; }

        /// <summary>
        /// 值 ，方法类型的值为 <see cref="MethodInfo"/>
        /// </summary>
        public object Value { get; protected set; }

        /// <summary>
        /// 当前值名称
        /// </summary>
        public string ValueName { get => _valueName; protected set => Set(ref _valueName, value); }

        /// <summary>
        /// Value的类型 <see cref="Type"/>
        /// </summary>
        public Type ValueType { get; protected set; }

        /// <summary>
        /// 父对象实例
        /// </summary>
        public object ParentInstance { get; protected set; }

        /// <summary>
        /// 指示当前属性是否可以继续查找
        /// </summary>
        public bool CanSnoop
        {
            get
            {
                if (_notSupport)
                {
                    return false;
                }
                //空值或者值类型的不可以继续查找
                if (Value == null || Value.GetType().IsValueType || (ValueType != null && ValueType.IsValueType))
                {
                    return false;
                }

                if (Value is Array array)
                {
                    if (array.Length == 0)
                    {
                        return false;
                    }
                    if (array.GetValue(0).GetType().IsValueType)
                    {
                        return false;
                    }
                }

                //方法返回值为空也不可以继续查找
                if (PropertyClsfi == PropertyClsfi.Method && (Value as MethodInfo).ReturnType.Name == "Void")
                {
                    return true;
                }

                return true;
            }
        }

        public string ToolTip
        {
            get
            {
                switch (PropertyClsfi)
                {
                    case PropertyClsfi.Property:
                        return ValueType?.FullName;
                    case PropertyClsfi.Method:
                        return (Value as MethodInfo)?.ToString();
                    default:
                        return ToString();
                }
            }
        }

        /// <summary>
        /// 指示当前方法是否已经执行过
        /// </summary>
        public bool HasExecuted { get => _hasExecuted; set => Set(ref _hasExecuted, value); }

        public RelayCommand OpenCommand
        {
            get => _openCommand ?? (_openCommand = new RelayCommand(OpenClick, CanOpenClick));
            set => _openCommand = value;
        }

        #endregion

        #region Methods

        private void OpenClick()
        {
            if (!CanSnoop)
            {
                return;
            }
            //解析属性
            if (PropertyClsfi == PropertyClsfi.Property)
            {
                PropertySnoop();
            }
            //执行方法
            else if (PropertyClsfi == PropertyClsfi.Method)
            {
                MethodSnoop();
            }
        }

        protected void PropertySnoop()
        {
            //判断是否是数组类型的
            if (Value is Array array)
            {
                List<InstanceProperty> properties = new List<InstanceProperty>();
                foreach (var item in array)
                {
                    var reDirectType = PropertyReDirectType(item.GetType());
                    var ins = InstanceProperty.Create(item, reDirectType);

                    properties.Add(ins);
                }
                var propertiesWindow = new LookupPropertyWindow(properties);
                propertiesWindow.ShowDialog();
            }
            else
            {
                var reDirectType = PropertyReDirectType(ValueType);

                var instanceProperty = InstanceProperty.Create(Value, reDirectType);
                var propertyWindow = new LookupPropertyWindow(instanceProperty);
                propertyWindow.ShowDialog();
            }
        }

        protected void MethodSnoop()
        {
            var methodInfo = Value as MethodInfo;

            var returnType = methodInfo.ReturnType;

            var parameters = methodInfo.GetParameters();

            if (parameters.Length == 0)
            {
                var result = MessageBox.Show($"Execute {methodInfo.ToString()} ?", "Execute", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    if (returnType.Name == "Void")
                    {
                        try
                        {
                            methodInfo.Invoke(ParentInstance, null);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return;
                        }
                    }
                    else
                    {
                        var valueResult = methodInfo.Invoke(ParentInstance, null);
                        if (valueResult == null)
                        {
                            MessageBox.Show("Return No Result <NULL>");
                            return;
                        }
                        if (returnType.IsValueType || returnType == typeof(string))
                        {
                            ValueName = $"{methodInfo.Name} => {valueResult}";
                        }
                        else
                        {
                            if (valueResult.IsValueArray())
                            {
                                var insProperties = valueResult.ObjToArray()
                                    .Select(p => InstanceProperty.Create(p, p.GetType()))
                                    .ToList();
                                var propertyWindow = new LookupPropertyWindow(insProperties);
                                propertyWindow.ShowDialog();
                            }
                            else
                            {
                                var reDirectType = ReturnValueReDirectType(returnType);
                                var instanceProperty = InstanceProperty.Create(valueResult, reDirectType);
                                var propertyWindow = new LookupPropertyWindow(instanceProperty);
                                propertyWindow.ShowDialog();
                            }
                        }
                    }

                    HasExecuted = true;
                }
            }

            else
            {
                //解析参数和值
                var instanceProperty = new MethodInstanceProperty(methodInfo, ParentInstance);
                var propertyWindow = new LookupPropertyWindow(instanceProperty);
                propertyWindow.ShowDialog();
            }
        }

        /// <summary>
        /// 将类型重新定位到SolidWorks Interface ，以便执行反射；命名空间需要在<see cref="SolidWorks.Interop.sldworks"/>
        /// </summary>
        /// <param name="valueType">SolidWorks <see cref="Type"/></param>
        /// <returns>I[Type] 以I开头的SolidWorks Interface</returns>
        protected virtual Type PropertyReDirectType(Type valueType)
        {
            if (Regex.IsMatch(valueType.FullName, "SolidWorks.Interop.sldworks.[A-Za-z]+"))
            {
                return TypeMatcherUtil.Match(valueType);
            }
            else
            {
                return valueType;
            }
        }

        /// <summary>
        /// 将类型重新定位到SolidWorks Interface ，以便执行反射；命名空间需要在<see cref="SolidWorks.Interop.sldworks"/>
        /// </summary>
        /// <param name="valueType">SolidWorks <see cref="Type"/></param>
        /// <returns>I[Type] 以I开头的SolidWorks Interface</returns>
        protected virtual Type ReturnValueReDirectType(Type valueType)
        {
            if (Regex.IsMatch(valueType.FullName, "SolidWorks.Interop.sldworks.[A-Za-z]+"))
            {
                return TypeMatcherUtil.Match(valueType);
            }
            else
            {
                return valueType;
            }
        }

        private bool CanOpenClick()
        {
            return true;
        }

        public override string ToString()
        {
            if (Value == null)
            {
                return "<NULL>";
            }
            else if (PropertyClsfi == PropertyClsfi.Method)
            {
                return (Value as MethodInfo).Name;
            }
            else if (PropertyClsfi == PropertyClsfi.Property)
            {
                return ToPropertyValueString();
            }

            return Value.ToString();
        }

        /// <summary>
        /// 在PropertyGrid中显示属性的值
        /// </summary>
        private string ToPropertyValueString()
        {
            if (Value == null)
            {
                return "<NULL>";
            };

            //值类型数组的显示值
            if (Value is Array array)
            {
                if (array.Length > 0)
                {
                    var arrayItemType = array.GetValue(0).GetType();
                    if (arrayItemType.IsValueType || arrayItemType == typeof(string))
                    {
                        string strValue = string.Empty;
                        foreach (var item in array)
                        {
                            strValue += string.IsNullOrEmpty(strValue) ? item.ToString() : $",{item.ToString()}";
                        }
                        return strValue;
                    }
                }
            }

            var valueName = Value.ToString();

            if (valueName == "System.__ComObject")
            {
                valueName = ValueType.Name;
            }

            return valueName;
        }

        #endregion
    }
}
