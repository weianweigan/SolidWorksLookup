using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SldWorksLookup.View;
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
        private RelayCommand _openCommand;
        private string _valueName;
        private bool _hasExecuted;

        protected LookupValue() { }

        public static LookupValue CreateMethodValue(MethodInfo method, object parentInstance)
        {

            return new LookupValue()
            {
                Value = method,
                ValueType = typeof(MethodInfo),
                PropertyClsfi = PropertyClsfi.Method,
                ParentInstance = parentInstance,
                ValueName = method.Name
            };
        }

        public static LookupValue CreatePropertyValue(object value, PropertyInfo propertyInfo)
        {
            var propertyType = propertyInfo.PropertyType == typeof(string) ? typeof(string) : propertyInfo.PropertyType;
            var property = new LookupValue()
            {
                Value = value,
                ValueType = propertyType,
                PropertyClsfi = PropertyClsfi.Property,
            };

            property.ValueName = property.ToPropertyValueString();

            return property;
        }

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
        public bool HasExecuted { get => _hasExecuted; set => Set(ref _hasExecuted ,value); }

        public RelayCommand OpenCommand
        {
            get => _openCommand ?? (_openCommand = new RelayCommand(OpenClick, CanOpenClick));
            set => _openCommand = value;
        }

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
                //判断是否是数组类型的
                if (Value is Array array)
                {
                    List<InstanceProperty> properties = new List<InstanceProperty>();
                    foreach (var item in array)
                    {
                        var ins = new InstanceProperty(item, item.GetType());
                        properties.Add(ins);
                    }
                    var propertiesWindow = new LookupPropertyWindow(properties);
                    propertiesWindow.ShowDialog();
                }
                else
                {
                    var reDirectType = ReDirectType(ValueType);
                    var instanceProperty = new InstanceProperty(Value, reDirectType);
                    var propertyWindow = new LookupPropertyWindow(instanceProperty);
                    propertyWindow.ShowDialog();
                }
            }
            //执行方法
            else if (PropertyClsfi == PropertyClsfi.Method)
            {
                MethodSnoop();
            }
        }

        private void MethodSnoop()
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
                                    .Select(p => new InstanceProperty(p, p.GetType()))
                                    .ToList();
                                var propertyWindow = new LookupPropertyWindow(insProperties);
                                propertyWindow.ShowDialog();
                            }
                            else
                            {
                                var reDirectType = ReDirectType(returnType);
                                var instanceProperty = new InstanceProperty(valueResult, reDirectType);
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

        private Type ReDirectType(Type valueType)
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
