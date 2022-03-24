using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using SldWorksLookup.Helper;

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

        public string ParentTypeName { get; internal set; }

        private bool CanHelpNavigate()
        {
            return true;
        }

        public void HelpNavigate()
        {
            var url = GetUrl();
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show(Properties.Resource.CannotGetHelpUrl);
            }
            else
            {
                Process.Start(url);
            }
        }

        public string GetUrl()
        {
            var url = string.Empty;
            var interfaceName = ParentTypeName;
            if (!interfaceName.StartsWith("I"))
                interfaceName = $"I{interfaceName}";

            var name = DisplayName;
            if (name.StartsWith("get_") || name.StartsWith("set_"))
                name = name.Substring(4, name.Length - 4);

            switch (PropertyClsfi)
            {
                case PropertyClsfi.None:
                    break;
                case PropertyClsfi.Property:
                    url = ApiUrlUtil.GetProperty(interfaceName, name);
                    break;
                case PropertyClsfi.Method:
                    url = ApiUrlUtil.GetProperty(interfaceName, name);
                    break;
                case PropertyClsfi.Event:
                    break;
                case PropertyClsfi.Class:
                    url = ApiUrlUtil.GetInterface(interfaceName);
                    break;
            }
            return url;
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
