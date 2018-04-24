using System;
using System.Collections;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl
{
    [Serializable]
    public class DmnDecisionRuleResultImpl : IDmnDecisionRuleResult
    {
        

        public static readonly DmnEngineLogger LOG = DmnLogger.ENGINE_LOGGER;

        protected internal readonly IDictionary<string, ITypedValue> outputValues =
            new Dictionary<string, ITypedValue>();

        public bool Empty
        {
            get { return outputValues.Count == 0; }
        }

        public virtual IDictionary<string, object> EntryMap
        {
            get
            {
                IDictionary<string, object> valueMap = new Dictionary<string, object>();

                foreach (var key in outputValues.Keys)
                    valueMap[key] = get(key);

                return valueMap;
            }
        }

        public virtual IDictionary<string, ITypedValue> EntryMapTyped
        {
            get { return outputValues; }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        public int Count { get; }
        public bool IsReadOnly { get; }

        public bool ContainsKey(string key)
        {
            throw new NotImplementedException();
        }

        public void Add(string key, object value)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, out object value)
        {
            throw new NotImplementedException();
        }

        public object this[string key]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public ICollection<string> Keys { get; }
        public ICollection<object> Values { get; }

        public T getFirstEntry<T>()
        {
            throw new NotImplementedException();
        }

        public T getFirstEntryTyped<T>()
        {
            throw new NotImplementedException();
        }

        public T getSingleEntry<T>()
        {
            throw new NotImplementedException();
        }

        public T getSingleEntryTyped<T>()
        {
            throw new NotImplementedException();
        }

        public T getEntry<T>(string name)
        {
            throw new NotImplementedException();
        }

        public T getEntryTyped<T>(string name)
        {
            throw new NotImplementedException();
        }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public <T> T getEntry(String name)
        public virtual ITypedValue getEntry(string name)
        {
            return outputValues[name];
        }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public <T extends TypedValue> T getEntryTyped(String name)
        public virtual ITypedValue getEntryTyped(string name)
        {
            return outputValues[name];
        }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public <T extends TypedValue> T getFirstEntryTyped()
        public virtual ITypedValue getFirstEntryTyped()
        {
            if (outputValues.Count > 0)
                return outputValues.Values.GetEnumerator().Current;
            return null;
        }

        public virtual ITypedValue getSingleEntryTyped()
        {
            if (outputValues.Count > 1)
            {
                //throw LOG.decisionOutputHasMoreThanOneValue(this);
            }
            return getFirstEntryTyped();
        }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> T getFirstEntry()
        public virtual ITypedValue getFirstEntry()
        {
            if (outputValues.Count > 0)
                return getFirstEntryTyped();
            return null;
        }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> T getSingleEntry()
        public virtual ITypedValue getSingleEntry()
        {
            if (outputValues.Count > 0)
                return getSingleEntryTyped();
            return null;
        }

        public virtual void putValue(string name, ITypedValue value)
        {
            outputValues[name] = value;
        }

        public virtual void putAllValues(IDictionary<string, ITypedValue> values)
        {
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
            //outputValues.putAll(values);
        }

        public int size()
        {
            return outputValues.Count;
        }

        public bool containsKey(string key)
        {
            return outputValues.ContainsKey(key);
        }

        public ISet<string> keySet()
        {
            return (ISet<string>) outputValues.Keys;
        }

        public ICollection<object> values()
        {
            IList<object> values = new List<object>();

            foreach (var typedValue in outputValues.Values)
                values.Add(typedValue);

            return values;
        }

        public override string ToString()
        {
            return outputValues.ToString();
        }

        public bool containsValue(object value)
        {
            return values().Contains(value);
        }

        public object get(object key)
        {
            //var typedValue = outputValues[key];
            //if (typedValue != null)
            //{
            //    return typedValue.Value;
            //}
            return null;
        }

        public object put(string key, object value)
        {
            throw new NotSupportedException("decision output is immutable");
        }

        public object remove(object key)
        {
            throw new NotSupportedException("decision output is immutable");
        }

        public void putAll<T1>(IDictionary<string, T1> m)
        {
            throw new NotSupportedException("decision output is immutable");
        }

        public void clear()
        {
            throw new NotSupportedException("decision output is immutable");
        }

        public ISet<KeyValuePair<string, object>> entrySet()
        {
            ISet<KeyValuePair<string, object>> entrySet = new HashSet<KeyValuePair<string, object>>();

            foreach (var typedEntry in outputValues)
            {
                //KeyValuePair<string, object> entry = new DmnDecisionRuleOutputEntry(this, typedEntry.Key, typedEntry.Value);
                //entrySet.Add(entry);
            }

            return entrySet;
        }

        protected internal class DmnDecisionRuleOutputEntry
        {
            protected internal readonly string key;
            private readonly DmnDecisionRuleResultImpl outerInstance;
            protected internal readonly ITypedValue typedValue;

            public DmnDecisionRuleOutputEntry(DmnDecisionRuleResultImpl outerInstance, string key,
                ITypedValue typedValue)
            {
                this.outerInstance = outerInstance;
                this.key = key;
                this.typedValue = typedValue;
            }

            public string Key
            {
                get { return key; }
            }

            public object Value
            {
                get
                {
                    if (typedValue != null)
                        return typedValue;
                    return null;
                }
            }

            public object setValue(object value)
            {
                throw new NotSupportedException("decision output entry is immutable");
            }
        }
    }
}