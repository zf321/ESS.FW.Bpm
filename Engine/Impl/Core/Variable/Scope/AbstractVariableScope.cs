using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Event;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using ESS.FW.Bpm.Engine.Variable;

using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;
using Newtonsoft.Json;

namespace ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope
{
    /// <summary>
    ///     
    ///      
    /// </summary>
    [Serializable]
    public abstract class AbstractVariableScope : IVariableScope, IVariableEventDispatcher
    {
        private const long SerialVersionUid = 1L;

        // TODO: move this?
        protected internal ELContext cachedElContext;
        //VariableInstanceEntity
        //protected internal abstract VariableStore<ICoreVariableInstance> VariableStore { get; }
        [NotMapped]
        protected internal abstract VariableStore VariableStore { get; }
        [NotMapped]
        protected internal abstract IVariableInstanceFactory<ICoreVariableInstance> VariableInstanceFactory { get; }

        protected abstract IList<IVariableInstanceLifecycleListener<ICoreVariableInstance>> VariableInstanceLifecycleListeners { get; }

        public abstract AbstractVariableScope ParentVariableScope { get; }

        [JsonIgnore]
        public virtual IVariableMap VariablesTyped
        {
            get { return GetVariablesTyped(true); }
        }

        [JsonIgnore]
        public virtual IVariableMap VariablesLocalTyped
        {
            get { return GetVariablesLocalTyped(true); }
        }
        [NotMapped]
        [JsonIgnore]
        public virtual IList<ICoreVariableInstance> VariableInstancesLocal
        {
            get { return VariableStore.Variables; }
        }

        protected internal virtual AbstractVariableScope SourceActivityVariableScope
        {
            get { return this; }
        }
        [NotMapped]
        public virtual ELContext CachedElContext
        {
            get { return cachedElContext; }
            set { cachedElContext = value; }
        }

        public virtual void DispatchEvent(VariableEvent variableEvent)
        {
            // default implementation does nothing
        }

        // get variable map /////////////////////////////////////////


        public virtual string VariableScopeKey
        {
            get { return "scope"; }
        }
        [NotMapped]
        [JsonIgnore]
        [IgnoreDataMember]
        public IDictionary<string, object> Variables
        {
            get
            {
                return GetVariables();
            }
            set
            {
                SetVariables(value);
            }
        }
       
        [NotMapped]
        [JsonIgnore]
        [IgnoreDataMember]
        public IDictionary<string, object> VariablesLocal
        {
            get { return VariablesLocalTyped; }
            set
            {
                if (value != null)
                {
                    foreach (string variableName in value.Keys)
                    {
                        Object val = null;
                        if (value is VariableMapImpl)
                        {
                            val = ((VariableMapImpl)value).GetValueTyped<ITypedValue>(variableName);
                        }
                        else
                        {
                            val = value[variableName];
                        }
                        SetVariableLocal(variableName, val);
                    }
                }

            }

        }

        // get single variable /////////////////////////////////////

        public virtual object GetVariable(string variableName)
        {
            return GetVariable(variableName, true);
        }

        public virtual object GetVariableLocal(string variableName)
        {
            return GetVariableLocal(variableName, true);
        }

        public virtual T GetVariableTyped<T>(string variableName)
        {
            return GetVariableTyped<T>(variableName, true);
        }

        public virtual T GetVariableTyped<T>(string variableName, bool deserializeValue)
        {
            return GetTypedValueFromVariableInstance<T>(deserializeValue, GetVariableInstance(variableName));
        }

        public virtual T GetVariableLocalTyped<T>(string variableName)
        {
            return GetVariableLocalTyped<T>(variableName, true);
        }

        public virtual T GetVariableLocalTyped<T>(string variableName, bool deserializeValue)
        {
            return GetTypedValueFromVariableInstance<T>(deserializeValue, GetVariableInstanceLocal(variableName));
        }

        public virtual bool HasVariables()
        {
            if (!VariableStore.Empty)
                return true;
            var parentScope = ParentVariableScope;
            return (parentScope != null) && parentScope.HasVariables();
        }

        public virtual bool HasVariablesLocal()
        {
            return !VariableStore.Empty;
        }

        public virtual bool HasVariable(string variableName)
        {
            if (HasVariableLocal(variableName))
                return true;
            var parentScope = ParentVariableScope;
            return (parentScope != null) && parentScope.HasVariable(variableName);
        }

        public virtual bool HasVariableLocal(string variableName)
        {
            return VariableStore.ContainsKey(variableName);
        }
        [JsonIgnore]
        public virtual ISet<string> VariableNames
        {
            get { return CollectVariableNames(new HashSet<string>()); }
        }

        [JsonIgnore]
        public virtual ISet<string> VariableNamesLocal
        {
            get { return VariableStore.Keys; }
        }

        public virtual void RemoveVariables()
        {
            foreach (var variableInstance in VariableStore.Variables)
                InvokeVariableLifecycleListenersDelete(variableInstance, SourceActivityVariableScope);

            VariableStore.RemoveVariables();
        }

        public virtual void RemoveVariablesLocal()
        {
            IList<string> variableNames = new List<string>(VariableNamesLocal);
            foreach (var variableName in variableNames)
                RemoveVariableLocal(variableName);
        }

        public virtual void RemoveVariables(ICollection<string> variableNames)
        {
            if (variableNames != null)
                foreach (var variableName in variableNames)
                    RemoveVariable(variableName);
        }

        public virtual void RemoveVariablesLocal(ICollection<string> variableNames)
        {
            if (variableNames != null)
                foreach (var variableName in variableNames)
                    RemoveVariableLocal(variableName);
        }

        public virtual void SetVariable(string variableName, object value)
        {
            ITypedValue typedValue = ESS.FW.Bpm.Engine.Variable.Variables.UnTypedValue(value);
            SetVariable(variableName, typedValue, SourceActivityVariableScope);
        }

        public virtual void SetVariableLocal(string variableName, object value)
        {
            ITypedValue typedValue = new UntypedValueImpl(value);
            SetVariableLocal(variableName, typedValue, SourceActivityVariableScope);
        }

        public virtual void RemoveVariable(string variableName)
        {
            RemoveVariable(variableName, SourceActivityVariableScope);
        }

        public virtual void RemoveVariableLocal(string variableName)
        {
            RemoveVariableLocal(variableName, SourceActivityVariableScope);
        }






        public virtual IVariableMap GetVariables()
        {
            return VariablesTyped;
        }

        public virtual IVariableMap GetVariablesTyped(bool deserializeValues)
        {
            var variableMap = new VariableMapImpl();
            CollectVariables(variableMap, null, false, deserializeValues);
            return variableMap;
        }

        public virtual IVariableMap GetVariablesLocal()
        {
            return VariablesLocalTyped;
        }

        public virtual IVariableMap GetVariablesLocalTyped(bool deserializeObjectValues)
        {
            var variables = new VariableMapImpl();
            CollectVariables(variables, null, true, deserializeObjectValues);
            return variables;
        }

        public virtual void CollectVariables(VariableMapImpl resultVariables, ICollection<string> variableNames,
            bool isLocal, bool deserializeValues)
        {
            var collectAll = variableNames == null;

            var localVariables = VariableInstancesLocal;
            foreach (var var in localVariables)
                if (!resultVariables.ContainsKey(var.Name) && (collectAll || variableNames.Contains(var.Name)))
                {
                    resultVariables.Put(@var.Name, @var.GetTypedValue(deserializeValues));
                }
            if (!isLocal)
            {
                var parentScope = ParentVariableScope;
                // Do not propagate to parent if all variables in 'variableNames' are already collected!
                if ((parentScope != null) && (collectAll || !resultVariables.KeySet().Equals(variableNames)))
                    parentScope.CollectVariables(resultVariables, variableNames, isLocal, deserializeValues);
            }
        }

        public virtual object GetVariable(string variableName, bool deserializeObjectValue)
        {
            return GetValueFromVariableInstance(deserializeObjectValue, GetVariableInstance(variableName));
        }

        public virtual object GetVariableLocal(string variableName, bool deserializeObjectValue)
        {
            return GetValueFromVariableInstance(deserializeObjectValue, GetVariableInstanceLocal(variableName));
        }

        protected internal virtual object GetValueFromVariableInstance(bool deserializeObjectValue,
            ICoreVariableInstance variableInstance)
        {
            if (variableInstance != null)
            {
                var typedValue = variableInstance.GetTypedValue(deserializeObjectValue);
                if (typedValue != null && typedValue.Value != null)
                    return typedValue.Value;
                //return typedValue;
                else
                    return
                        variableInstance.Execution.ProcessBusinessKey;
            }
            return null;
        }

        private T GetTypedValueFromVariableInstance<T>(bool deserializeValue, ICoreVariableInstance variableInstance)
        {
            if (variableInstance != null)
            {
                var r= variableInstance.GetTypedValue(deserializeValue);
                if(typeof(T)==typeof(string))
                {
                    return (T)r.Value;
                }
                return (T)variableInstance.GetTypedValue(deserializeValue);
            }
            else
            {
                return default(T);
            }
        }

        public virtual ICoreVariableInstance GetVariableInstance(string variableName)
        {
            var variableInstance = GetVariableInstanceLocal(variableName);
            if (variableInstance != null)
                return variableInstance;
            var parentScope = ParentVariableScope;
            if (parentScope != null)
                return parentScope.GetVariableInstance(variableName);
            return null;
        }

        public virtual ICoreVariableInstance GetVariableInstanceLocal(string name)
        {
            return VariableStore.GetVariable(name);
        }

        protected internal virtual ISet<string> CollectVariableNames(ISet<string> variableNames)
        {
            var parentScope = ParentVariableScope;
            if (parentScope != null)
            {
                foreach (var collectVariableName in parentScope.CollectVariableNames(variableNames))
                {
                    variableNames.Add(collectVariableName);
                }
            }
            foreach (var variableInstance in VariableStore.Variables)
                variableNames.Add(variableInstance.Name);
            return variableNames;
        }

        public virtual void SetVariables<T1>(IDictionary<string, T1> variables)
        {
            if (variables != null)
                foreach (var variableName in variables.Keys)
                {
                    object value = null;
                    if (variables is IVariableMap)
                        value = ((IVariableMap)variables).GetValueTyped<ITypedValue>(variableName);
                    else
                        value = variables[variableName];
                    SetVariable(variableName, value);
                }
        }

        public virtual void SetVariablesLocal<T1>(IDictionary<string, T1> variables)
        {
            if (variables != null)
                foreach (var variableName in variables.Keys)
                {
                    object value = null;
                    if (variables is IVariableMap)
                        value = ((IVariableMap)variables).GetValueTyped<ITypedValue>(variableName);
                    else
                        value = variables[variableName];
                    SetVariableLocal(variableName, value);
                }
        }

        protected internal virtual void SetVariable(string variableName, ITypedValue value,
            AbstractVariableScope sourceActivityVariableScope)
        {
            if (HasVariableLocal(variableName))
            {
                SetVariableLocal(variableName, value, sourceActivityVariableScope);
                return;
            }
            var parentVariableScope = ParentVariableScope;
            if (parentVariableScope != null)
            {
                if (sourceActivityVariableScope == null)
                    parentVariableScope.SetVariable(variableName, value);
                else
                    parentVariableScope.SetVariable(variableName, value, sourceActivityVariableScope);
                return;
            }
            SetVariableLocal(variableName, value, sourceActivityVariableScope);
        }

        public virtual void SetVariableLocal(string variableName, ITypedValue value,
            AbstractVariableScope sourceActivityExecution)
        {
            var variableStore = VariableStore;

            if (variableStore.ContainsKey(variableName))
            {
                var existingInstance = variableStore.GetVariable(variableName);
                existingInstance.SetValue(value);
                InvokeVariableLifecycleListenersUpdate(existingInstance, sourceActivityExecution);
            }
            else
            {
                var variableValue = VariableInstanceFactory.Build(variableName, value, false);
                VariableStore.AddVariable(variableValue as VariableInstanceEntity);
                InvokeVariableLifecycleListenersCreate(variableValue, sourceActivityExecution);
            }
        }

        protected internal virtual void InvokeVariableLifecycleListenersCreate(ICoreVariableInstance variableInstance,
            AbstractVariableScope sourceScope)
        {
            InvokeVariableLifecycleListenersCreate(variableInstance, sourceScope, VariableInstanceLifecycleListeners);
        }

        protected internal virtual void InvokeVariableLifecycleListenersCreate(ICoreVariableInstance variableInstance,
            AbstractVariableScope sourceScope,
            IList<IVariableInstanceLifecycleListener<ICoreVariableInstance>> lifecycleListeners)
        {
            foreach (var lifecycleListener in lifecycleListeners)
                lifecycleListener.OnCreate(variableInstance, sourceScope);
        }

        protected internal virtual void InvokeVariableLifecycleListenersDelete(ICoreVariableInstance variableInstance,
            AbstractVariableScope sourceScope)
        {
            InvokeVariableLifecycleListenersDelete(variableInstance, sourceScope, VariableInstanceLifecycleListeners);
        }

        protected internal virtual void InvokeVariableLifecycleListenersDelete(ICoreVariableInstance variableInstance,
            AbstractVariableScope sourceScope,
            IList<IVariableInstanceLifecycleListener<ICoreVariableInstance>> lifecycleListeners)
        {
            foreach (var lifecycleListener in lifecycleListeners)
                lifecycleListener.OnDelete(variableInstance, sourceScope);
        }

        protected internal virtual void InvokeVariableLifecycleListenersUpdate(ICoreVariableInstance variableInstance,
            AbstractVariableScope sourceScope)
        {
            InvokeVariableLifecycleListenersUpdate(variableInstance, sourceScope, VariableInstanceLifecycleListeners);
        }

        protected internal virtual void InvokeVariableLifecycleListenersUpdate(ICoreVariableInstance variableInstance,
            AbstractVariableScope sourceScope,
            IList<IVariableInstanceLifecycleListener<ICoreVariableInstance>> lifecycleListeners)
        {
            foreach (var lifecycleListener in lifecycleListeners)
                lifecycleListener.OnUpdate(variableInstance, sourceScope);
        }

        /// <summary>
        ///     Sets a variable in the local scope. In contrast to
        ///     <seealso cref="#setVariableLocal(String, Object)" />, the variable is transient that
        ///     means it will not be stored in the data base. For example, a transient
        ///     variable can be used for a result variable that is only available for
        ///     output mapping.
        /// </summary>
        public virtual void SetVariableLocalTransient(string variableName, object value)
        {
            ITypedValue typedValue = Engine.Variable.Variables.UnTypedValue(value);

            VariableStore.AddVariable(VariableInstanceFactory.Build(variableName, typedValue, true));
        }

        protected internal virtual void RemoveVariable(string variableName,
            AbstractVariableScope sourceActivityExecution)
        {
            if (VariableStore.ContainsKey(variableName))
            {
                RemoveVariableLocal(variableName, sourceActivityExecution);
                return;
            }
            var parentVariableScope = ParentVariableScope;
            if (parentVariableScope != null)
                if (sourceActivityExecution == null)
                    parentVariableScope.RemoveVariable(variableName);
                else
                    parentVariableScope.RemoveVariable(variableName, sourceActivityExecution);
        }

        protected internal virtual void RemoveVariableLocal(string variableName,
            AbstractVariableScope sourceActivityExecution)
        {
            if (VariableStore.ContainsKey(variableName))
            {
                var variableInstance = VariableStore.GetVariable(variableName);

                InvokeVariableLifecycleListenersDelete(variableInstance, sourceActivityExecution);
                VariableStore.RemoveVariable(variableName);
            }
        }
    }
}