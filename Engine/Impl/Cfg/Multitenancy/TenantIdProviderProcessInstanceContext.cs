
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Variable;

namespace ESS.FW.Bpm.Engine.Impl.Cfg.Multitenancy
{
    /// <summary>
    ///     Provides information about a starting process instance to a <seealso cref="ITenantIdProvider" /> implementation.
    ///     
    ///     
    /// </summary>
    public class TenantIdProviderProcessInstanceContext
    {
        protected internal IProcessDefinition processDefinition;

        protected internal IDelegateCaseExecution superCaseExecution;

        protected internal IDelegateExecution superExecution;

        protected internal IVariableMap variables;

        public TenantIdProviderProcessInstanceContext(IProcessDefinition processDefinition, IVariableMap variables)
        {
            this.processDefinition = processDefinition;
            this.variables = variables;
        }

        public TenantIdProviderProcessInstanceContext(IProcessDefinition processDefinition, IVariableMap variables,
            IDelegateExecution superExecution) : this(processDefinition, variables)
        {
            this.superExecution = superExecution;
        }

        public TenantIdProviderProcessInstanceContext(IProcessDefinition processDefinition, IVariableMap variables,
            IDelegateCaseExecution superCaseExecution) : this(processDefinition, variables)
        {
            this.superCaseExecution = superCaseExecution;
        }

        /// <returns> the process definition of the process instance which is being started </returns>
        public virtual IProcessDefinition ProcessDefinition
        {
            get { return processDefinition; }
        }

        /// <returns> the variables which were passed to the starting process instance </returns>
        public virtual IVariableMap Variables
        {
            get { return variables; }
        }

        /// <returns>
        ///     the super execution. Null if the starting process instance is a root process instance and not started using a call
        ///     activity.
        ///     If the process instance is started using a call activity, this method returns the execution in the super process
        ///     instance executing the call activity.
        /// </returns>
        public virtual IDelegateExecution SuperExecution
        {
            get { return superExecution; }
        }

        /// <returns>
        ///     the super case execution. Null if the starting process instance is not a sub process instance started using a
        ///     CMMN case ITask.
        /// </returns>
        public virtual IDelegateCaseExecution SuperCaseExecution
        {
            get { return superCaseExecution; }
        }
    }
}