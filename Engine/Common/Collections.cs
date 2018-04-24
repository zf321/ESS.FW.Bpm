using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Common
{
    public class Collections
    {
        public static IDictionary<TKey,TValue> SingletonMap<TKey, TValue>(TKey key,TValue value)
        {
            IDictionary<TKey, TValue> data = new Dictionary<TKey, TValue>();
            data.Add(key, value);
            return data;
        }

        public static IDictionary<TKey, TValue> AddAll<TKey, TValue>(TKey key, TValue value)
        {
            IDictionary<TKey, TValue> data = new Dictionary<TKey, TValue>();
            data.Add(key, value);
            return data;
        }
    }
}
