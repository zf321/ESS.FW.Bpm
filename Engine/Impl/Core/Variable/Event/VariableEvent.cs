using ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.Core.Variable.Event
{
    /// <summary>
    ///     
    /// </summary>
    public class VariableEvent
    {
        protected internal string eventName;
        protected internal AbstractVariableScope sourceScope;

        protected internal ICoreVariableInstance variableInstance;

        public VariableEvent(ICoreVariableInstance variableInstance, string eventName, AbstractVariableScope sourceScope)
        {
            this.variableInstance = variableInstance;
            this.eventName = eventName;
            this.sourceScope = sourceScope;
        }

        public virtual ICoreVariableInstance VariableInstance
        {
            get { return variableInstance; }
        }

        public virtual string EventName
        {
            get { return eventName; }
        }

        public virtual AbstractVariableScope SourceScope
        {
            get { return sourceScope; }
        }

        internal string GetEventName()
        {
            return eventName;
        }
    }
}