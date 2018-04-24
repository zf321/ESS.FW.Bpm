using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope
{
    /// <summary>
    ///     
    /// </summary>
    public class VariableStore/*<T> where T : ICoreVariableInstance*/
    {
        protected internal List<IVariableStoreObserver> Observers;
        protected internal IDictionary<string, ICoreVariableInstance> variables;

        protected internal IVariablesProvider VariablesProvider;

        public VariableStore() : this(VariableCollectionProvider.EmptyVariables())
        {
        }

        public VariableStore(IVariablesProvider provider, params IVariableStoreObserver[] observers)
        {
            VariablesProvider = provider;
            Observers = new List<IVariableStoreObserver>();
            Observers.AddRange(observers);
        }

        protected internal virtual IDictionary<string, ICoreVariableInstance> VariablesMap
        {
            get
            {
                ForceInitialization();

                return variables;
            }
        }
        [NotMapped]
        public virtual IList<ICoreVariableInstance> Variables
        {
            get { return new List<ICoreVariableInstance>(VariablesMap.Values); }
        }

        public virtual bool Empty
        {
            get { return VariablesMap.Count == 0; }
        }

        public virtual ISet<string> Keys
        {
            get { return new HashSet<string>(VariablesMap.Keys); }
        }

        public virtual bool Initialized
        {
            get { return variables != null && variables.Count>0; }
        }

        /// <summary>
        ///     The variables provider can be exchanged as long as the variables are not yet initialized
        /// </summary>
        public virtual void SetVariablesProvider(IVariablesProvider variablesProvider)
        {
            if (variables != null)
            {
                // already initialized
            }
            VariablesProvider = variablesProvider;
        }

        public virtual ICoreVariableInstance GetVariable(string name)
        {
            if (VariablesMap.ContainsKey(name))
                return VariablesMap[name];
            return default(ICoreVariableInstance);
        }


        public virtual void AddVariable(ICoreVariableInstance value)
        {
            if (ContainsKey(value.Name))
                throw ProcessEngineLogger.CoreLogger.DuplicateVariableInstanceException(value);

            VariablesMap[value.Name] = value;

            foreach (var listener in Observers)
                listener.OnAdd(value);
        }

        public virtual void UpdateVariable(ICoreVariableInstance value)
        {
            if (!ContainsKey(value.Name))
                throw ProcessEngineLogger.CoreLogger.DuplicateVariableInstanceException(value);
        }

        public virtual bool ContainsValue(ICoreVariableInstance value)
        {
            return VariablesMap.Values.Contains(value);
        }

        public virtual bool ContainsKey(string key)
        {
            return VariablesMap.ContainsKey(key);
        }

        public virtual void ForceInitialization()
        {
            if (!Initialized)
            {
                variables = new Dictionary<string, ICoreVariableInstance>();
                    var varPros = VariablesProvider.ProvideVariables();
                    if (varPros != null && varPros.Count > 0)
                    {
                        //ENGINE-20005 Detroying scope ProcessInstance
                        foreach (var variable in varPros)
                        {
                            variables[variable.Name] = variable;
                        }
                    }
            }
        }

        public virtual ICoreVariableInstance RemoveVariable(string variableName)
        {
            if (!VariablesMap.ContainsKey(variableName))
                return default(ICoreVariableInstance);

            ICoreVariableInstance value;
            VariablesMap.TryGetValue(variableName, out value);

            VariablesMap.Remove(variableName);

            foreach (var observer in Observers)
                observer.OnRemove(value);

            return value;
        }

        public virtual void RemoveVariables()
        {
            var valuesIt = VariablesMap.Values.GetEnumerator();

            while (valuesIt.MoveNext())
            {
                var nextVariable = valuesIt.Current;
                
                foreach (var observer in Observers)
                    observer.OnRemove(nextVariable);
            }
        }

        public virtual void AddObserver(IVariableStoreObserver observer)
        {
            Observers.Add(observer);
        }

        public virtual void RemoveObserver(IVariableStoreObserver observer)
        {
            Observers.Remove(observer);
        }
    }

    public interface IVariableStoreObserver/*<in T> where T : ICoreVariableInstance*/
    {
        void OnAdd(ICoreVariableInstance variable);

        void OnRemove(ICoreVariableInstance variable);
    }

    public interface IVariablesProvider/*<T> where T : ICoreVariableInstance*/
    {
        ICollection<ICoreVariableInstance> ProvideVariables();
    }
}