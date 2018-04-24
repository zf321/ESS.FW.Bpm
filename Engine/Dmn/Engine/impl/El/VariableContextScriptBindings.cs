using ESS.FW.Bpm.Engine.Impl.Javax.Script;
using ESS.FW.Bpm.Engine.Variable.Context;
using ESS.FW.Bpm.Engine.Variable.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Dmn.Engine.impl.El
{
    public class VariableContextScriptBindings: IBindings
    {
        protected internal IBindings wrappedBindings;

        protected internal IVariableContext variableContext;

        public VariableContextScriptBindings(IBindings wrappedBindings, IVariableContext variableContext)
        {
            this.wrappedBindings = wrappedBindings;
            this.variableContext = variableContext;
        }

        /// <summary>
        /// Dedicated implementation which does not fall back on the <seealso cref="#calculateBindingMap()"/> for performance reasons
        /// </summary>
        public virtual bool ContainsKey(string key)
        {
            if (wrappedBindings.ContainsKey(key))
            {
                return true;
            }
            if (key is string)
            {
                return variableContext.ContainsVariable((string)key);
            }
            else
            {
                return false;
            }
        }
        public IDictionary<string,object> GetAll()
        {
            return wrappedBindings.GetAll();
        }
        /// <summary>
        /// Dedicated implementation which does not fall back on the <seealso cref="#calculateBindingMap()"/> for performance reasons
        /// </summary>
        public virtual object Get(string key)
        {
            object result = null;

            if (wrappedBindings.ContainsKey(key))
            {
                result = wrappedBindings.Get(key);
            }
            else
            {
                if (key is string)
                {
                    ITypedValue resolvedValue = variableContext.Resolve((string)key);
                    result = Unpack(resolvedValue);
                }
            }

            return result;
        }

        /// <summary>
        /// Dedicated implementation which does not fall back on the <seealso cref="#calculateBindingMap()"/> for performance reasons
        /// </summary>
        public virtual object Put(string name, object value)
        {
            // only write to the wrapped bindings
            return wrappedBindings.Put(name, value);
        }

        //public virtual ISet<Entry<string, object>> EntrySet()
        //{
        //    //JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'entrySet' method:
        //    return CalculateBindingMap().EntrySet();
        //}

        public virtual ICollection<string> KeySet()
        {
            return CalculateBindingMap().Keys;
        }

        public virtual int size()
        {
            return CalculateBindingMap().Count;
        }

        public virtual ICollection<object> Values()
        {
            return CalculateBindingMap().Values;
        }

        public virtual void PutAll(IDictionary<string,object> toMerge) //where T1 : String
        {
            //JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
            //ORIGINAL LINE: for (Entry<? extends String, ?> entry : toMerge.entrySet())
            foreach (KeyValuePair <string, object> entry in toMerge)
		{
                Put(entry.Key, entry.Value);
            }
        }

        public virtual object Remove(string key)
        {
            return wrappedBindings.Remove(key);
        }

        public virtual void Clear()
        {
            wrappedBindings.Clear();
        }

        public virtual bool ContainsValue(string value)
        {
            return CalculateBindingMap().Values.Contains(value);
        }

        public virtual bool isEmpty()
        {
            return CalculateBindingMap().Count == 0;
        }

        protected internal virtual IDictionary<string, object> CalculateBindingMap()
        {

            IDictionary<string, object> bindingMap = new Dictionary<string, object>();

           ICollection<string> keySet = variableContext.KeySet();
            foreach (string variableName in keySet)
            {
                bindingMap[variableName] = Unpack(variableContext.Resolve(variableName));
            }

            //Set<Entry<string, object>> wrappedBindingsEntries = wrappedBindings.entrySet();
            foreach (var entry in wrappedBindings.GetAll())
            {
                bindingMap[entry.Key] = entry.Value;
            }

            return bindingMap;
        }

        protected internal virtual object Unpack(ITypedValue resolvedValue)
        {
            if (resolvedValue != null)
            {
                return resolvedValue.Value;
            }
            return null;
        }

        public static VariableContextScriptBindings Wrap(IBindings wrappedBindings, IVariableContext variableContext)
        {
            return new VariableContextScriptBindings(wrappedBindings, variableContext);
        }

    }
}
