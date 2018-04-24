using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class ISetExtend
    {
        public static ISet<T> AddAll<T>(this ISet<T> source,ISet<T> data)
        {
            foreach(var item in data)
            {
                source.Add(item);
            }
            return source;
        }
        public static ISet<T> AddAll<T>(this ISet<T> source ,IList<T> data)
        {
            foreach (var item in data)
            {
                source.Add(item);
            }
            return source;
        }
    }
}
