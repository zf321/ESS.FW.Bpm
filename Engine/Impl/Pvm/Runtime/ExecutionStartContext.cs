using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Core.Instance;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime
{
    /// <summary>
    ///     
    /// </summary>
    [System.Serializable]
    public class ExecutionStartContext
    {
        protected internal bool delayFireHistoricVariableEvents;

        protected internal IDictionary<string, object> variables;
        protected internal IDictionary<string, object> variablesLocal;

        public ExecutionStartContext() : this(true)
        {
        }

        public ExecutionStartContext(bool delayFireHistoricVariableEvents)
        {
            this.delayFireHistoricVariableEvents = delayFireHistoricVariableEvents;
        }

        public virtual bool DelayFireHistoricVariableEvents => delayFireHistoricVariableEvents;

        public virtual InstantiationStack InstantiationStack { get; set; }


        public virtual IDictionary<string, object> Variables
        {
            set { variables = value; }
        }

        public virtual IDictionary<string, object> VariablesLocal
        {
            set { variablesLocal = value; }
        }

        public virtual void ExecutionStarted(IActivityExecution execution)
        {
            if (execution is ExecutionEntity && delayFireHistoricVariableEvents)
            {
                ExecutionEntity executionEntity = (ExecutionEntity)execution;
                executionEntity.FireHistoricVariableInstanceCreateEvents();
            }

            IActivityExecution parent = execution;
            while ((parent != null) && (parent.ExecutionStartContext != null))
            {
                parent.DisposeExecutionStartContext();
                parent =  parent.Parent;
            }
        }

        public virtual void ApplyVariables(CoreExecution execution)
        {
            execution.Variables = variables;
            execution.VariablesLocal = variablesLocal;
        }
    }
}