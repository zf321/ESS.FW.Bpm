using System;
using System.Collections.Generic;
using System.Text;
using ESS.FW.Bpm.Engine.Variable.Context;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Variable
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class VariableMapImpl : Dictionary<string, object>, IVariableMap, IVariableContext
    {
        private const long SerialVersionUid = 1L;

        //protected internal IDictionary<string, ITypedValue> variables = new Dictionary<string, ITypedValue>();

        public VariableMapImpl(VariableMapImpl map) : base(map)
        {
        }

        public VariableMapImpl(IDictionary<string, ITypedValue> map)
        {
            if (map != null)
                PutAll(map);
        }
        public VariableMapImpl(IDictionary<string,object> dic)
        {
            foreach(var item in dic)
            {
                Put(item.Key, item.Value);
            }
        }
        public VariableMapImpl()
        {
        }

        public virtual bool Empty
        {
            get { return Count == 0; }
        }

        public virtual ICollection<string> KeySet()
        {
            return Keys;
        }

        public virtual ITypedValue Resolve(string variableName)
        {
            return GetValueTyped<ITypedValue>(variableName);
        }

        public virtual bool ContainsVariable(string variableName)
        {
            return ContainsKey(variableName);
        }

        // VariableMap implementation //////////////////////////////

        public virtual IVariableMap PutValue(string name, object value)
        {
            Put(name, value);
            return this;
        }

        public virtual IVariableMap PutValueTyped(string name, ITypedValue value)
        {
            this[name] = value;
            return this;
        }

        public object GetValue(string name, System.Type type)
        {
            throw new NotImplementedException();
        }

        public virtual T GetValueTyped<T>(string name) where T : ITypedValue
        {
            if (!this.ContainsKey(name))
            {
                return default(T);
            }
            return (T) this[name];
        }

        public virtual void PutAll(IDictionary<string, ITypedValue> m)
        {
            if (m != null)
                if (m is VariableMapImpl)
                    foreach (var value in ((VariableMapImpl) m))
                        this.Add(value.Key,value.Value);
                else
                    foreach (var entry in m.SetOfKeyValuePairs())
                        Put(entry.Key, entry.Value);
        }

        public virtual IVariableContext AsVariableContext()
        {
            return this;
        }

        public virtual T GetValue<T>(string name, System.Type type)
        {
            var @object = Get(name);
            if (@object == null)
                return default(T);
            if (type.IsAssignableFrom(@object.GetType()))
                return (T) @object;
            throw new InvalidCastException("Cannot cast variable named '" + name + "' with value '" + @object +
                                           "' to type '" + type + "'.");
        }

        // java.uitil Map<String, Object> implementation ////////////////////////////////////////
        

        //public virtual bool ContainsValue(object value)
        //{
        //    foreach (var varValue in this.Values())
        //        if (value == varValue)
        //            return true;
        //        else if ((value != null) && value.Equals(varValue))
        //            return true;
        //    return false;
        //}

        public virtual object Get(string key)
        {
            var typedValue =(ITypedValue) this[key];

            if (typedValue != null)
                return typedValue.Value;
            return null;
        }

        public virtual object Put(string key, object value)
        {
            //ITypedValue typedValue = Variables.UnTypedValue(value);            
            //ITypedValue prevValue = this.ContainsKey(key) ? this[key] as ITypedValue : null;
            //this[key] = typedValue;

            var typedValue = Variables.UnTypedValue(value);

            var prevValue = this[key] = typedValue;

            if (prevValue != null)
                return ((ITypedValue)prevValue).Value;
            return null;
        }

        public virtual object Remove(string key)
        {
            var prevValue = (ITypedValue)this[key];
            this.Remove(key);

            if (prevValue != null)
                return prevValue.Value;
            return null;
        }

        //public virtual ICollection<object> Values()
        //{
        //    // NOTE: cannot naively return List of values here. A proper implementation must return a
        //    // Collection which is backed by the actual variable map
        //    return (ICollection<object>)this.Values;
        //    //return new AbstractCollectionAnonymousInnerClass(this);
        //}

        //public virtual ISet<KeyValuePair<string, object>> EntrySet()
        //{
        //    // NOTE: cannot naively return Set of entries here. A proper implementation must
        //    // return a Set which is backed by the actual map
        //    return variables;
        //    //return new AbstractSetAnonymousInnerClass(this);
        //}

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("{\n");
            foreach (var variable in this.SetOfKeyValuePairs())
            {
                stringBuilder.Append("  ");
                stringBuilder.Append(variable.Key);
                stringBuilder.Append(" => ");
                stringBuilder.Append(variable.Value);
                stringBuilder.Append("\n");
            }
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }

        //        var iterator = _outerInstance.variables.Values.GetEnumerator();
        //        //ORIGINAL LINE: final java.util.Iterator<TypedValue> iterator = variables.values().iterator();
        //        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //        // wrapped iterator. Must be local to the iterator() method
        //    {


        //    public virtual IEnumerator<object> Iterator()
        //    }
        //        _outerInstance = outerInstance;
        //    {

        //    public AbstractCollectionAnonymousInnerClass(VariableMapImpl outerInstance)
        //    private readonly VariableMapImpl _outerInstance;
        //{

        //private class AbstractCollectionAnonymousInnerClass : ICollection<object>
        //}
        //    return new Dictionary<string, object>(variables);
        //{

        //public virtual IDictionary<string, object> AsValueMap()
        //}
        //    return AsValueMap().GetHashCode();
        //{

        //public override int GetHashCode()
        //}
        //    return AsValueMap().Equals(obj);
        //{

        //public override bool Equals(object obj)

        //        return new IteratorAnonymousInnerClass(this, iterator);
        //    }

        //    public virtual int Size()
        //    {
        //        return _outerInstance.variables.Count;
        //    }

        //    private class IteratorAnonymousInnerClass : IEnumerator<object>
        //    {
        //        private readonly AbstractCollectionAnonymousInnerClass _outerInstance;

        //        private readonly IEnumerator<ITypedValue> _iterator;

        //        public IteratorAnonymousInnerClass(AbstractCollectionAnonymousInnerClass outerInstance,
        //            IEnumerator<ITypedValue> iterator)
        //        {
        //            _outerInstance = outerInstance;
        //            _iterator = iterator;
        //        }

        //        public virtual bool HasNext()
        //        {
        //            //JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
        //            return _iterator.hasNext();
        //        }

        //        public virtual object Next()
        //        {
        //            //JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
        //            return _iterator.next().Value;
        //        }

        //        public virtual void Remove()
        //        {
        //            //JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
        //            _iterator.remove();
        //        }
        //    }
        //}

        //private class AbstractSetAnonymousInnerClass : AbstractSet<KeyValuePair<string, object>>
        //{
        //    private readonly VariableMapImpl _outerInstance;

        //    public AbstractSetAnonymousInnerClass(VariableMapImpl outerInstance)
        //    {
        //        _outerInstance = outerInstance;
        //    }


        //    public virtual IEnumerator<KeyValuePair<string, object>> Iterator()
        //    {
        //        return new IteratorAnonymousInnerClass2(this);
        //    }

        //    public virtual int Size()
        //    {
        //        return _outerInstance.variables.Count;
        //    }

        //    private class IteratorAnonymousInnerClass2 : IEnumerator<KeyValuePair<string, object>>
        //    {
        //        private readonly AbstractSetAnonymousInnerClass _outerInstance;


        //        // wrapped iterator. Must be local to the iterator() method
        //        internal readonly IEnumerator<KeyValuePair<string, ITypedValue>> Iterator;

        //        public IteratorAnonymousInnerClass2(AbstractSetAnonymousInnerClass outerInstance)
        //        {
        //            _outerInstance = outerInstance;
        //            Iterator = outerInstance._outerInstance.variables.SetOfKeyValuePairs().GetEnumerator();
        //        }

        //        public virtual bool HasNext()
        //        {
        //            return Iterator.hasNext();
        //        }

        //        public virtual KeyValuePair<string, object> Next()
        //        {
        //            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //            //ORIGINAL LINE: final java.util.Map.Entry<String, TypedValue> underlyingEntry = iterator.next();
        //            KeyValuePair<string, ITypedValue> underlyingEntry = Iterator.next();

        //            // return wrapper backed by the underlying entry
        //            return new EntryAnonymousInnerClass(this, underlyingEntry);
        //        }

        //        public virtual void Remove()
        //        {
        //            Iterator.Remove();
        //        }

        //        private class EntryAnonymousInnerClass : Dictionary<string, object>
        //        {
        //            private readonly IteratorAnonymousInnerClass2 _outerInstance;

        //            private KeyValuePair<string, ITypedValue> _underlyingEntry;

        //            public EntryAnonymousInnerClass(IteratorAnonymousInnerClass2 outerInstance,
        //                KeyValuePair<string, ITypedValue> underlyingEntry)
        //            {
        //                _outerInstance = outerInstance;
        //                _underlyingEntry = underlyingEntry;
        //            }

        //            public virtual string Key
        //            {
        //                get { return _underlyingEntry.Key; }
        //            }

        //            public virtual object Value
        //            {
        //                get { return _underlyingEntry.Value.Value; }
        //            }

        //            public virtual object SetValue(object value)
        //            {
        //                ITypedValue typedValue = Variable.variables.UntypedValue(value);
        //                return _underlyingEntry.setValue(typedValue);
        //            }

        //            public sealed override bool Equals(object o)
        //            {
        //                if (!(o is DictionaryEntry))
        //                    return false;
        //                var e = (DictionaryEntry) o;
        //                object k1 = Key;
        //                var k2 = e.Key;
        //                if ((k1 == k2) || ((k1 != null) && k1.Equals(k2)))
        //                {
        //                    var v1 = Value;
        //                    var v2 = e.Value;
        //                    if ((v1 == v2) || ((v1 != null) && v1.Equals(v2)))
        //                        return true;
        //                }
        //                return false;
        //            }

        //            public sealed override int GetHashCode()
        //            {
        //                var key = Key;
        //                var value = Value;
        //                return (ReferenceEquals(key, null) ? 0 : key.GetHashCode()) ^
        //                       (value == null ? 0 : value.GetHashCode());
        //            }
        //        }
        //    }
        //}
    }
}