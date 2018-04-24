using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Javax.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Scripting.Engine
{
    public class ScriptBindings: IBindings
    {
        /// <summary>
        /// The script engine implementations put some key/value pairs into the binding.
        /// This list contains those keys, such that they wouldn't be stored as process variable.
        /// 
        /// This list contains the keywords for JUEL, Javascript and Groovy.
        /// </summary>
        protected internal static readonly IList<string> UNSTORED_KEYS = new List<string>() { "out", "out:print", "lang:import", "context", "elcontext", "print", "println", "S", "XML", "JSON", ScriptEngine_Fields.ARGV, "execution", "__doc__" }; // do not export python doc string -  jRuby is only setting this variable and execution instead of exporting any other variables -  Spin Internal Variable -  Spin Internal Variable -  Spin Internal Variable

        protected internal IList<IResolver> scriptResolvers;
        protected internal IVariableScope variableScope;

        protected internal IBindings wrappedBindings;

        /// <summary>
        /// if true, all script variables will be set in the variable scope. </summary>
        protected internal bool autoStoreScriptVariables;

        public ScriptBindings(IList<IResolver> scriptResolvers, IVariableScope variableScope, IBindings wrappedBindings)
        {
            this.scriptResolvers = scriptResolvers;
            this.variableScope = variableScope;
            this.wrappedBindings = wrappedBindings;
            autoStoreScriptVariables = IsAutoStoreScriptVariablesEnabled();
        }

        protected internal virtual bool IsAutoStoreScriptVariablesEnabled()
        {
            ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
            if (processEngineConfiguration != null)
            {
                return processEngineConfiguration.IsAutoStoreScriptVariables;
            }
            return false;
        }

        public virtual bool ContainsKey(string key)
        {
            foreach (IResolver scriptResolver in scriptResolvers)
            {
                if (scriptResolver.ContainsKey(key))
                {
                    return true;
                }
            }
            return wrappedBindings.ContainsKey(key);
        }

        public virtual object Get(string key)
        {
            object result = null;

            if (wrappedBindings.ContainsKey((string)key))
            {
                result = wrappedBindings.Get((string)key);

            }
            else
            {
                foreach (IResolver scriptResolver in scriptResolvers)
                {
                    if (scriptResolver.ContainsKey(key))
                    {
                        result = scriptResolver.Get(key);
                    }
                }
            }

            return result;
        }

        public virtual object Put(string name, object value)
        {

            if (autoStoreScriptVariables)
            {
                if (!UNSTORED_KEYS.Contains(name))
                {
                    object oldValue = variableScope.GetVariable(name);
                    variableScope.SetVariable(name, value);
                    return oldValue;
                }
            }

            return wrappedBindings.Put(name, value);
        }

        //public virtual IList<KeyValuePair<string, object>> EntrySet()
        //{
        //    //JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'entrySet' method:
        //    return calculateBindingMap().EntrySet();
        //}

        public virtual ICollection<string> KeySet()
        {
            return CalculateBindingMap().Keys;
        }

        public virtual int Size()
        {
            return CalculateBindingMap().Count;
        }

        public virtual ICollection<object> Values()
        {
            return CalculateBindingMap().Values;
        }

        public virtual void PutAll(IDictionary<string,object> toMerge) //where T1 : String where ? : Object
        {
            if (toMerge == null)
            {
                throw new System.NullReferenceException("toMerge map is null");
            }
            foreach (KeyValuePair<string, object> entry in toMerge)
            {
                string key = entry.Key;
                CheckKey(key);
                Put(key, entry.Value);
            }
        }

        public virtual object Remove(string key)
        {
            if (UNSTORED_KEYS.Contains(key))
            {
                return null;
            }
            return wrappedBindings.Remove(key);
        }

        public virtual void Clear()
        {
            wrappedBindings.Clear();
        }

        public virtual bool ContainsValue(object value)
        {
            return CalculateBindingMap().Values.Contains(value);//.ContainsValue(value);
        }

        public virtual bool IsEmpty()
        {
            return CalculateBindingMap().Count == 0;
        }

        protected internal virtual IDictionary<string, object> CalculateBindingMap()
        {

            IDictionary<string, object> bindingMap = new Dictionary<string, object>();

            foreach (IResolver resolver in scriptResolvers)
            {
                foreach (string key in resolver.KeySet())
                {
                    bindingMap[key] = resolver.Get(key);
                }
            }

            var wrappedBindingsEntries = wrappedBindings.GetAll();//.EntrySet();
            foreach (var entry in wrappedBindingsEntries)
            {
                bindingMap[entry.Key] = entry.Value;
            }

            return bindingMap;
        }
        private void CheckKey(object key)
        {
            if (key == null)
            {
                throw new System.NullReferenceException("key can not be null");
            }
            if (!(key is string))
            {
                throw new System.InvalidCastException("key should be a String");
            }
            if (key.Equals(""))
            {
                throw new System.ArgumentException("key can not be empty");
            }
        }

        public IDictionary<string, object> GetAll()
        {
            return CalculateBindingMap();
        }
    }
}
