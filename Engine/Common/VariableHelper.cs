using ESS.FW.Bpm.Engine.Variable.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;

namespace ESS.FW.Bpm.Engine.Common
{
    public class VariableHelper
    {
        public static IDictionary<string,object> GetDictionary(IDictionary<string,ITypedValue> source)
        {
            IDictionary<string, object> data = new Dictionary<string, object>();
            foreach(var item in source)
            {
                data[item.Key] = item.Value;
            }
            return data;
        }
        public static IDictionary<string,ITypedValue> GetTypedValue(IDictionary<string, object> source)
        {
            IDictionary<string, ITypedValue> data = new Dictionary<string, ITypedValue>();
            foreach (var item in source)
            {
                if(item.Value is ITypedValue)
                {
                    data[item.Key] = (ITypedValue)item.Value;
                }
                else
                {
                    data[item.Key] = new UntypedValueImpl(item.Value);
                }
            }
            return data;
        }
    }
}
