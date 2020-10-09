using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace SldWorksLookup.Model
{
    public class LookupPropertyDescriptor : PropertyDescriptor
    {

        LookupProperty _prop;
        object _parent;

        public LookupPropertyDescriptor(LookupProperty prop, object parent)
          : base(prop.DisplayName, null)
        {
            _prop = prop;
            _parent = parent;
        }

        public override AttributeCollection Attributes
        {
            get
            {
                List<Attribute> attributes = new List<Attribute>();

                if (!string.IsNullOrEmpty(_prop.Category))
                {
                    attributes.Add(new CategoryAttribute(_prop.Category));
                }
                if (!string.IsNullOrEmpty(_prop.DisplayName))
                {
                    attributes.Add(new CategoryAttribute(_prop.DisplayName));
                }
                if (!string.IsNullOrEmpty(_prop.Description))
                {
                    attributes.Add(new DescriptionAttribute(_prop.Description));
                }
                if (_prop.EditorType != null)
                {
                    attributes.Add(new EditorAttribute(_prop.EditorType, _prop.EditorType));
                }

                attributes.Add(new BrowsableAttribute(_prop.Browserable));
                attributes.Add(new ReadOnlyAttribute(_prop.IsReadOnly));

                if (_prop.PropertyOrder != null)
                {
                    attributes.Add(new PropertyOrderAttribute(_prop.PropertyOrder.Value));
                }

                return new AttributeCollection(attributes.ToArray());
            }
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override object GetValue(object component)
        {
            return _prop.Value;
        }

        public override void ResetValue(object component)
        {

        }

        public override void SetValue(object component, object value)
        {
            if (value != null)
            {
                _prop.Value = value;
            }
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override Type ComponentType
          => _parent.GetType();

        public override bool IsReadOnly
          => _prop.IsReadOnly;

        public override Type PropertyType
        {
            get
            {
                return _prop.PropertyType;
            }
        }
    }
}
