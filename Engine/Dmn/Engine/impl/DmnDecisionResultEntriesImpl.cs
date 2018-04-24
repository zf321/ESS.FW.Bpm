using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Common;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl
{
    [Serializable]
    public class DmnDecisionResultEntriesImpl : IDmnDecisionResultEntries
    {


        public static readonly DmnEngineLogger LOG = DmnLogger.ENGINE_LOGGER;

        protected internal readonly IDictionary<string, object> outputValues = new Dictionary<string, object>();

        public virtual T GetEntry<T>(string name)
        {
            return (T)outputValues[name];
        }

        public ICollection<string> Keys
        {
            get { return outputValues.Keys; }
        }

        public ICollection<object> Values
        {
            get { return outputValues.Values; }
        }

        public int Count
        {
            get { return outputValues.Count; }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public IDictionary<string, object> EntryMap
        {
            get { return outputValues; }
        }

        public IDictionary<string, ITypedValue> EntryMapTyped
        {
            get { return VariableHelper.GetTypedValue(outputValues); }
        }

        public object this[string key]
        {
            get { return outputValues[key]; }

            set { outputValues[key] = value; }
        }

        public bool ContainsKey(string key)
        {
            return outputValues.ContainsKey(key);
        }

        public void Add(string key, object value)
        {
            outputValues.Add(key, value);
        }

        public bool Remove(string key)
        {
            return outputValues.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return outputValues.TryGetValue(key,out value);
        }

        public void Add(KeyValuePair<string, object> item)
        {
            outputValues.Add(item);
        }

        public void Clear()
        {
            outputValues.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return outputValues.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            outputValues.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
           return outputValues.Remove(item);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return outputValues.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return outputValues.GetEnumerator();
        }

        public object GetFirstEntry()
        {
            return GetFirstEntryTyped<ITypedValue>()?.Value;
        }

        public T GetSingleEntryTyped<T>()
        {
            if (this.outputValues.Count > 1)
            {
                throw LOG.decisionOutputHasMoreThanOneValue(this);
            }
            else
            {
                return this.GetFirstEntryTyped<T>();
            }
        }

        public T GetFirstEntryTyped<T>()
        {
            if (outputValues == null || outputValues.Count == 0)
            {
                return default(T);
            }
            return (T)this.outputValues.Values.ToList().FirstOrDefault();
        }

        public void PutValue(string outputName, ITypedValue value)
        {
            if (outputName == null)
            {
                outputName ="null";
            }
            this.outputValues.Add(outputName, value);
        }

        public T GetSingleEntry<T>()
        {
            return GetSingleEntryTyped<T>();
        }

        public T GetEntryTyped<T>(string name)
        {
            if (name == null)
            {
                name = "null";
            }
            return outputValues.ContainsKey(name) ? (T)outputValues[name] : default(T);
        }

        public ICollection<string> keySet()
        {
            return outputValues.Keys;
        }

        public override string ToString()
        {
            return outputValues.ToString();
        }
    }
}