using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Variable;


namespace ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope
{
    /// <summary>
    ///     Wraps a variable scope as if it has no parent such that it is reduced to its local
    ///     variables. For example <seealso cref="#getVariable(String)" /> simply delegates to
    ///     <seealso cref="#getVariableLocal(String)" />.
    ///     
    /// </summary>
    public class VariableScopeLocalAdapter : IVariableScope
    {
        protected internal IVariableScope WrappedScope;

        public VariableScopeLocalAdapter(IVariableScope wrappedScope)
        {
            this.WrappedScope = wrappedScope;
        }

        public virtual string VariableScopeKey
        {
            get { return WrappedScope.VariableScopeKey; }
        }

        public virtual IDictionary<string, object> Variables
        {
            get { return VariablesLocal; }
            set { VariablesLocal = value; }
        }

        public virtual IVariableMap VariablesTyped
        {
            get { return VariablesLocalTyped; }
        }

        public virtual IVariableMap GetVariablesTyped(bool deserializeValues)
        {
            return GetVariablesLocalTyped(deserializeValues);
        }

        public virtual IDictionary<string, object> VariablesLocal
        {
            get { return WrappedScope.VariablesLocal; }
            set { WrappedScope.VariablesLocal = value; }
        }

        public virtual IVariableMap VariablesLocalTyped
        {
            get { return WrappedScope.VariablesLocalTyped; }
        }

        public virtual IVariableMap GetVariablesLocalTyped(bool deserializeValues)
        {
            return WrappedScope.GetVariablesLocalTyped(deserializeValues);
        }

        public virtual object GetVariable(string variableName)
        {
            return GetVariableLocal(variableName);
        }

        public virtual object GetVariableLocal(string variableName)
        {
            return WrappedScope.GetVariableLocal(variableName);
        }

        public virtual T GetVariableTyped<T>(string variableName)
        {
            return GetVariableLocalTyped<T>(variableName);
        }

        public virtual T GetVariableTyped<T>(string variableName, bool deserializeValue)
        {
            return GetVariableLocalTyped<T>(variableName, deserializeValue);
        }

        public virtual T GetVariableLocalTyped<T>(string variableName)
        {
            return WrappedScope.GetVariableLocalTyped<T>(variableName);
        }

        public virtual T GetVariableLocalTyped<T>(string variableName, bool deserializeValue)
        {
            return WrappedScope.GetVariableLocalTyped<T>(variableName, deserializeValue);
        }

        public virtual ISet<string> VariableNames
        {
            get { return VariableNamesLocal; }
        }

        public virtual ISet<string> VariableNamesLocal
        {
            get { return WrappedScope.VariableNamesLocal; }
        }

        public virtual void SetVariable(string variableName, object value)
        {
            SetVariableLocal(variableName, value);
        }

        public virtual void SetVariableLocal(string variableName, object value)
        {
            WrappedScope.SetVariableLocal(variableName, value);
        }


        public virtual bool HasVariables()
        {
            return HasVariablesLocal();
        }

        public virtual bool HasVariablesLocal()
        {
            return WrappedScope.HasVariablesLocal();
        }

        public virtual bool HasVariable(string variableName)
        {
            return HasVariableLocal(variableName);
        }

        public virtual bool HasVariableLocal(string variableName)
        {
            return WrappedScope.HasVariableLocal(variableName);
        }

        public virtual void RemoveVariable(string variableName)
        {
            RemoveVariableLocal(variableName);
        }

        public virtual void RemoveVariableLocal(string variableName)
        {
            WrappedScope.RemoveVariableLocal(variableName);
        }

        public virtual void RemoveVariables(ICollection<string> variableNames)
        {
            RemoveVariablesLocal(variableNames);
        }

        public virtual void RemoveVariablesLocal(ICollection<string> variableNames)
        {
            WrappedScope.RemoveVariablesLocal(variableNames);
        }

        public virtual void RemoveVariables()
        {
            RemoveVariablesLocal();
        }

        public virtual void RemoveVariablesLocal()
        {
            WrappedScope.RemoveVariablesLocal();
        }
    }
}