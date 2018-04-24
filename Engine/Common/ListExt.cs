using System.Collections;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Common
{
    public class ListExt : List<object>, IList
    {
        public static IList EMPTY_LIST = new ListExt();
        public static IList ConvertToObj(IList<object> source)
        {
            return source as List<object>;
        }
        public static List<string> ConvertToString(IList<object> data)
        {
            List<string> r = new List<string>();
            foreach (var item in data)
            {
                r.Add((string)item);
            }
            return r;
        }
        public static IList<T> ConvertToListT<T>(IList data)
        {
            IList<T> result = new List<T>();
            foreach (var item in data)
            {
                result.Add((T)item);
            }
            return result;
        }

        public static IList ConvertToIlist<T>(IList<T> data)
        {
            ListExt r=new ListExt();
            foreach (var item in data)
            {
                r.Add(item);
            }
            return r;
        }
    }
}
