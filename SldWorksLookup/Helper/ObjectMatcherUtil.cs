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
            bool flag = false;
            if (obj is Array array)
            {
                if (array.Length > 0 && array.GetValue(0).GetType().IsValueType)
                {
                    flag = true;
                }
            }
            return flag;
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
