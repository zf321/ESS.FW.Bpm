using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Variable;
using ESS.FW.Bpm.Engine.Variable;


namespace ESS.FW.Bpm.Engine.Impl.EL
{
    /// <summary>
    ///     Variable-scope only used to resolve variables when NO execution is active but
    ///     expression-resolving is needed. This occurs eg. when start-form properties have default's
    ///     defined. Even though variables are not available yet, expressions should be resolved
    ///     anyway.
    ///     
    /// </summary>
    public class StartProcessVariableScope : IVariableScope
    {
        private static readonly IVariableMap EmptyVariableMap;

        /// <summary>
        ///     Since a <seealso cref="StartProcessVariableScope" /> has no state, it's safe to use the same
        ///     instance to prevent too many useless instances created.
        /// </summary>
        public static StartProcessVariableScope SharedInstance { get; } = new StartProcessVariableScope();


        public virtual string VariableScopeKey
        {
            get { return "scope"; }
        }

        public virtual object GetVariable(string variableName)
        {
            return null;
        }

        public virtual object GetVariableLocal(string variableName)
        {
            return null;
        }

        public virtual IVariableMap GetVariablesTyped(bool deserializeObjectValues)
        {
            return GetVariables();
        }

        public virtual IVariableMap VariablesLocalTyped
        {
            get { return GetVariablesLocalTyped(true); }
        }

        public virtual IVariableMap VariablesTyped
        {
            get { return GetVariablesTyped(true); }
        }

        public virtual IVariableMap GetVariablesLocalTyped(bool deserializeObjectValues)
        {
            return GetVariablesLocal();
        }

        public virtual T GetVariableTyped<T>(string variableName)
        {
            return default(T);
        }

        public virtual T GetVariableTyped<T>(string variableName, bool deserializeObjectValue)
        {
            return default(T);
        }

        public virtual T GetVariableLocalTyped<T>(string variableName)
        {
            return default(T);
        }

        public virtual T GetVariableLocalTyped<T>(string variableName, bool deserializeObjectValue)
        {
            return default(T);
        }
        
        public virtual ISet<string> VariableNames
        {
            get { return null; }
        }

        public virtual ISet<string> VariableNamesLocal
        {
            get { return null; }
        }

        public IDictionary<string, object> Variables
        {
            get { throw new NotImplementedException(); }

            set { throw new NotImplementedException(); }
        }

        public IDictionary<string, object> VariablesLocal
        {
            get { throw new NotImplementedException(); }

            set { throw new NotImplementedException(); }
        }

        public virtual void SetVariable(string variableName, object value)
        {
            throw new NotSupportedException("No execution active, no variables can be set");
        }

        public virtual void SetVariableLocal(string variableName, object value)
        {
            throw new NotSupportedException("No execution active, no variables can be set");
        }

        public virtual bool HasVariables()
        {
            return false;
        }

        public virtual bool HasVariablesLocal()
        {
            return false;
        }

        public virtual bool HasVariable(string variableName)
        {
            return false;
        }

        public virtual bool HasVariableLocal(string variableName)
        {
            return false;
        }

        public virtual void RemoveVariable(string variableName)
        {
            throw new NotSupportedException("No execution active, no variables can be removed");
        }

        public virtual void RemoveVariableLocal(string variableName)
        {
            throw new NotSupportedException("No execution active, no variables can be removed");
        }

        public virtual void RemoveVariables()
        {
            throw new NotSupportedException("No execution active, no variables can be removed");
        }

        public virtual void RemoveVariablesLocal()
        {
            throw new NotSupportedException("No execution active, no variables can be removed");
        }

        public virtual void RemoveVariables(ICollection<string> variableNames)
        {
            throw new NotSupportedException("No execution active, no variables can be removed");
        }

        public virtual void RemoveVariablesLocal(ICollection<string> variableNames)
        {
            throw new NotSupportedException("No execution active, no variables can be removed");
        }

        public virtual IVariableMap GetVariables()
        {
            return EmptyVariableMap;
        }

        public virtual IVariableMap GetVariablesLocal()
        {
            return EmptyVariableMap;
        }

        public virtual object GetVariable(string variableName, bool deserializeObjectValue)
        {
            return null;
        }

        public virtual object GetVariableLocal(string variableName, bool deserializeObjectValue)
        {
            return null;
        }

        public virtual ICoreVariableInstance GetVariableInstance(string name)
        {
            return null;
        }

        public virtual ICoreVariableInstance GetVariableInstanceLocal(string name)
        {
            return null;
        }
    }
}