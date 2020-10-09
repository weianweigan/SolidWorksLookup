using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

namespace SldWorksLookup.Model
{
    public class LookupProperties : ICustomTypeDescriptor
    {
        private List<LookupProperty> _properties = new List<LookupProperty>();

        public List<LookupProperty> Properties => _properties;

        /// <summary>
        /// 过滤 I接口方法
        /// </summary>
        public bool LookupInterfaceMethod { get; set; }

        public string Name { get; set; }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            PropertyDescriptorCollection pds = new PropertyDescriptorCollection(null);

            for (int i = 0; i < _properties.Count; i++)
            {
                if (LookupInterfaceMethod && _properties[i].Category == "Method")
                {
                    if (_properties[i].DisplayName.StartsWith("I"))
                    {
                        pds.Add(new LookupPropertyDescriptor(_properties[i], this));
                    }
                }
                else
                {
                    pds.Add(new LookupPropertyDescriptor(_properties[i], this));
                }
            }

            return pds;
        }

        public void Add(LookupProperty property)
        {
            _properties.Add(property);
        }

        public void AddReturnProperty(LookupProperty property)
        {
            var result = _properties.Find(p => p.Category == "Result");
            if (result != null)
            {
                _properties.Remove(result);
            }
            _properties.Add(property);
        }

        public void Clear()
        {
            _properties.Clear();
        }

        #region Use default TypeDescriptor stuff

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, noCustomTypeDesc: true);
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName(this, noCustomTypeDesc: true);
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, noCustomTypeDesc: true);
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return TypeDescriptor.GetConverter(this, noCustomTypeDesc: true);
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, noCustomTypeDesc: true);
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, noCustomTypeDesc: true);
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, noCustomTypeDesc: true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, noCustomTypeDesc: true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, noCustomTypeDesc: true);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(this, attributes, noCustomTypeDesc: true);
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        public override string ToString()
        {
            return Name ?? base.ToString();
        }

        #endregion
    }
}
