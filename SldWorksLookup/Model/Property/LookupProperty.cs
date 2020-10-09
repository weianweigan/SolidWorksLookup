using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SldWorksLookup.Model
{

    public abstract class PropertyEntity
    {
        public string DisplayName { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public bool Browserable { get; set; } = true;

        public int? PropertyOrder { get; set; }

        public bool IsReadOnly { get; set; } = false;

        public virtual PropertyClsfi PropertyClsfi { get;protected set; } = PropertyClsfi.Property;
    }

    public abstract class LookupProperty : LookupProperty<object>
    {
        public LookupProperty(string displayName, object value,Type type)
        {
            DisplayName = displayName;
            EditorType = type;
            Value = value;
            PropertyType = type;
        }

    }

    public abstract class LookupProperty<TProperty>:PropertyEntity
    {
        public Type PropertyType { get; set; }

        public TProperty Value { get; set; }

        public Type EditorType { get; set; }

        public LookupProperty(string displayName, TProperty value, Type type)
        {
            DisplayName = displayName;
            EditorType = type;
            Value = value;
            PropertyType = type;
        }

        protected LookupProperty()
        {
        }
    }
}
