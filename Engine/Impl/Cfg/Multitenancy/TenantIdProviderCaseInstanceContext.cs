
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Variable;

namespace ESS.FW.Bpm.Engine.Impl.Cfg.Multitenancy
{
    /// <summary>
    ///     Provides information about a starting case instance to a <seealso cref="ITenantIdProvider" /> implementation.
    ///     
    ///     
    /// </summary>
    public class TenantIdProviderCaseInstanceContext
    {
        protected internal ICaseDefinition caseDefinition;

        protected internal IDelegateCaseExecution superCaseExecution;

        protected internal IDelegateExecution superExecution;

        protected internal IVariableMap variables;

        public TenantIdProviderCaseInstanceContext(ICaseDefinition caseDefinition, IVariableMap variables)
        {
            this.caseDefinition = caseDefinition;
            this.variables = variables;
        }

        public TenantIdProviderCaseInstanceContext(ICaseDefinition caseDefinition, IVariableMap variables,
            IDelegateExecution superExecution) : this(caseDefinition, variables)
        {
            this.superExecution = superExecution;
        }

        public TenantIdProviderCaseInstanceContext(ICaseDefinition caseDefinition, IVariableMap variables,
            IDelegateCaseExecution superCaseExecution) : this(caseDefinition, variables)
        {
            this.superCaseExecution = superCaseExecution;
        }

        /// <returns> the case definition of the case instance which is being started </returns>
        public virtual ICaseDefinition CaseDefinition
        {
            get { return caseDefinition; }
        }

        /// <returns> the variables which were passed to the starting case instance </returns>
        public virtual IVariableMap Variables
        {
            get { return variables; }
        }

        /// <returns>
        ///     the super execution. <code>null</code> if the starting case instance is a root process instance and not started
        ///     using a call activity.
        ///     If the case instance is started using a call activity, this method returns the execution in the super process
        ///     instance executing the call activity.
        /// </returns>
        public virtual IDelegateExecution SuperExecution
        {
            get { return superExecution; }
        }

        /// <returns>
        ///     the super case execution. <code>null</code> if the starting case instance is not a sub case instance started
        ///     using a CMMN case task.
        /// </returns>
        public virtual IDelegateCaseExecution SuperCaseExecution
        {
            get { return superCaseExecution; }
        }
    }
}