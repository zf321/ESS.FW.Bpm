using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESS.FW.Bpm.Engine.Impl.Core.Model
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public abstract class CoreModelElement
    {
        private const long SerialVersionUid = 1L;

        /// <summary>
        ///     contains built-in listeners
        /// </summary>
        protected internal IDictionary<string, IList<IDelegateListener<IBaseDelegateExecution>>> builtInListeners =
            new Dictionary<string, IList<IDelegateListener<IBaseDelegateExecution>>>();

        protected internal IDictionary<string, IList<IVariableListener<IDelegateVariableInstance<IBaseDelegateExecution>>>>
            builtInVariableListeners
                =
                new Dictionary<string, IList<IVariableListener<IDelegateVariableInstance<IBaseDelegateExecution>>>>();

        protected internal string id;

        /// <summary>
        ///     contains all listeners (built-in + user-provided)
        /// </summary>
        protected internal IDictionary<string, IList<IDelegateListener<IBaseDelegateExecution>>> listeners =
            new Dictionary<string, IList<IDelegateListener<IBaseDelegateExecution>>>();

        protected internal string name;
        protected internal Properties properties = new Properties();

        protected internal IDictionary<string, IList<IVariableListener<IDelegateVariableInstance<IBaseDelegateExecution>>>> variableListeners =
            new Dictionary<string, IList<IVariableListener<IDelegateVariableInstance<IBaseDelegateExecution>>>>();

        public CoreModelElement(string id)
        {
            this.id = id;
        }

        public virtual string Id
        {
            get { return id; }
            set { id = value; }
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        ///     Returns the properties of the element.
        /// </summary>
        /// <returns> the properties </returns>
        [NotMapped]
        public virtual Properties Properties
        {
            get { return properties; }
            set { properties = value; }
        }

        public virtual IDictionary<string, IList<IDelegateListener<IBaseDelegateExecution>>> Listeners => listeners;

        public virtual IDictionary<string, IList<IDelegateListener<IBaseDelegateExecution>>> BuiltInListeners
            => builtInListeners;

        public virtual IDictionary<string, IList<IVariableListener<IDelegateVariableInstance<IBaseDelegateExecution>>>> BuiltInVariableListeners
            => builtInVariableListeners;

        public virtual IDictionary<string, IList<IVariableListener<IDelegateVariableInstance<IBaseDelegateExecution>>>> VariableListeners
        {
            get { return variableListeners; }
        }

        /// <seealso cref="Properties# set( PropertyKey, Object)">
        /// </seealso>
        public virtual void SetProperty(string name, object value)
        {
            properties.Set(new PropertyKey<object>(name), value);
        }

        /// <seealso cref="Properties# get( PropertyKey"
        /// )
        /// </seealso>
        public virtual object GetProperty(string name)
        {
            return properties.Get(new PropertyKey<object>(name));
        }


        //event listeners //////////////////////////////////////////////////////////
        public virtual IList<IDelegateListener<IBaseDelegateExecution>> GetListeners(string eventName)
        {
            IList<IDelegateListener<IBaseDelegateExecution>> listenerList = Listeners.ContainsKey(eventName) ? Listeners[eventName] : new List<IDelegateListener<IBaseDelegateExecution>>();
            return listenerList;
        }

        public virtual IList<IDelegateListener<IBaseDelegateExecution>> GetBuiltInListeners(string eventName)
        {
            IList<IDelegateListener<IBaseDelegateExecution>> listenerList;
            if (BuiltInListeners.TryGetValue(eventName, out listenerList))
                return listenerList;

            return new List<IDelegateListener<IBaseDelegateExecution>>();
        }

        public virtual IList<IVariableListener<IDelegateVariableInstance<IBaseDelegateExecution>>> GetVariableListenersLocal(string eventName)
        {
            IList<IVariableListener<IDelegateVariableInstance<IBaseDelegateExecution>>> listenerList;
            if (VariableListeners.TryGetValue(eventName, out listenerList))
                return listenerList;

            return new List<IVariableListener<IDelegateVariableInstance<IBaseDelegateExecution>>>();
        }

        public virtual IList<IVariableListener<IDelegateVariableInstance<IBaseDelegateExecution>>> GetBuiltInVariableListenersLocal(string eventName)
        {
            IList<IVariableListener<IDelegateVariableInstance<IBaseDelegateExecution>>> listenerList;
            if (BuiltInVariableListeners.TryGetValue(eventName, out listenerList))
                return listenerList;

            return new List<IVariableListener<IDelegateVariableInstance<IBaseDelegateExecution>>>();
        }
        public virtual void AddListener(string eventName, IDelegateListener<IBaseDelegateExecution> listener)
        {
            AddListener(eventName, listener, -1);
        }
        public virtual void AddBuiltInListener(string eventName, IDelegateListener<IBaseDelegateExecution> listener)
        {
            AddBuiltInListener(eventName, listener, -1);
        }

        public virtual void AddBuiltInListener(string eventName, IDelegateListener<IBaseDelegateExecution> listener, int index)
        {
            AddListenerToMap(listeners, eventName, listener, index);
            AddListenerToMap(builtInListeners, eventName,  listener, index);
        }

        public virtual void AddListener(string eventName, IDelegateListener<IBaseDelegateExecution> listener, int index)
        {
            AddListenerToMap(listeners, eventName, listener, index);
        }

        protected internal virtual void AddListenerToMap(IDictionary<string, IList<IDelegateListener<IBaseDelegateExecution>>> listenerMap, string eventName,
            IDelegateListener<IBaseDelegateExecution> listener, int index)
        {
            IList<IDelegateListener<IBaseDelegateExecution>> listeners;
            listenerMap.TryGetValue(eventName, out listeners);
            if (listeners == null)
            {
                listeners = new List<IDelegateListener<IBaseDelegateExecution>>();
                listenerMap[eventName] = listeners;
            }
            if (index < 0)
                listeners.Add(listener);
            else
                listeners.Insert(index, listener);
        }

        protected internal virtual void AddListenerToMap(IDictionary<string, IList<IVariableListener<IDelegateVariableInstance<IBaseDelegateExecution>>>> listenerMap, string eventName,
           IVariableListener<IDelegateVariableInstance<IBaseDelegateExecution>> listener, int index)
        {
            IList<IVariableListener<IDelegateVariableInstance<IBaseDelegateExecution>>> listeners;
            listenerMap.TryGetValue(eventName, out listeners);
            if (listeners == null)
            {
                listeners = new List<IVariableListener<IDelegateVariableInstance<IBaseDelegateExecution>>>();
                listenerMap[eventName] = listeners;
            }
            if (index < 0)
                listeners.Add(listener);
            else
                listeners.Insert(index, listener);
        }

        public virtual void AddVariableListener(string eventName, IVariableListener<IDelegateVariableInstance<IBaseDelegateExecution>> listener)
        {
            AddVariableListener(eventName, listener, -1);
        }

        public virtual void AddVariableListener(string eventName, IVariableListener<IDelegateVariableInstance<IBaseDelegateExecution>> listener, int index)
        {
            AddListenerToMap(variableListeners, eventName, listener, index);
        }

        public virtual void AddBuiltInVariableListener(string eventName, IVariableListener<IDelegateVariableInstance<IBaseDelegateExecution>> listener)
        {
            AddBuiltInVariableListener(eventName, listener, -1);
        }

        public virtual void AddBuiltInVariableListener(string eventName, IVariableListener<IDelegateVariableInstance<IBaseDelegateExecution>> listener, int index)
        {
            AddListenerToMap(variableListeners, eventName, listener, index);
            AddListenerToMap(builtInVariableListeners, eventName, listener,index);
        }
    }
} 