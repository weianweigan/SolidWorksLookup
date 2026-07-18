using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SldWorksLookup
{
    public static class ObjectMatcherUtil
    {
        public static bool IsArray(this object obj)
        {
            return obj is Array;
        }

        public static bool IsValueArray(this object obj)
        {
            if (obj is Array array)
            {
                foreach (var item in array)
                {
                    if (item != null)
                    {
                        return item.GetType().IsValueType || item is string;
                    }
                }
            }

            return false;
        }

        public static IEnumerable<object> ObjToArray(this object obj)
        {
            var array = obj as Array;
            if (array == null)
            {
                yield break;
            }
            foreach (var item in array)
            {
                yield return item;
            }
        }
    }
}
