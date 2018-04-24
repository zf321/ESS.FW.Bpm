using System.Collections.Generic;
using System.Linq;

namespace ESS.FW.Bpm.Engine.Impl.Util
{
    /// <summary>
    ///     helper/convience methods for working with collections.
    ///     
    /// </summary>
    public class CollectionUtil
    {
        // No need to instantiate
        private CollectionUtil()
        {
        }

        /// <summary>
        ///     Helper method that creates a singleton map.
        ///     Alternative for Collections.singletonMap(), since that method returns a
        ///     generic typed map
        ///     <K, T>
        ///         depending on the input type, but we often need a
        ///         <String, Object> map.
        /// </summary>
        public static IDictionary<string, object> SingletonMap(string key, object value)
        {
            IDictionary<string, object> map = new Dictionary<string, object>();
            map[key] = value;
            return map;
        }

        /// <summary>
        ///     cannot be reliably used for SQL parameters on MyBatis < 3.3.0
        /// </summary>
        public static IList<T> AsArrayList<T>(T[] values)
        {
            return values.ToList();
        }

        public static ISet<T> AsHashSet<T>(params T[] elements)
        {
            var eles = elements.ToArray();
            var tt = new HashSet<T>();
            tt.Union(elements);
            return tt;
        }

        public static void AddToMapOfLists<TS, T>(IDictionary<TS, IList<T>> map, TS key, T value)
        {
            IList<T> list;
            map.TryGetValue(key, out list);
            if (list == null)
            {
                list = new List<T>();
                map[key] = list;
            }
            list.Add(value);
        }

        public static void AddToMapOfSets<TS, T>(IDictionary<TS, ISet<T>> map, TS key, T value)
        {
            ISet<T> set;
            map.TryGetValue(key, out set);
            //var set = map[key];
            if (set == null)
            {
                set = new HashSet<T>();
                map[key] = set;
            }
            set.Add(value);
        }

        public static void AddCollectionToMapOfSets<TS, T>(IDictionary<TS, ISet<T>> map, TS key, ICollection<T> values)
        {
            ISet<T> set;
            map.TryGetValue(key, out set);
            //var set = map[key];
            if (set == null)
            {
                set = new HashSet<T>();
                map[key] = set;
            }
            set.Union(values);
        }
    }
}