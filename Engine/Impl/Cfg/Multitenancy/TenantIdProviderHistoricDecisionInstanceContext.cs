using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Repository;

namespace ESS.FW.Bpm.Engine.Impl.Cfg.Multitenancy
{
    /// <summary>
    ///     Provides information about a historic decision instance to a <seealso cref="ITenantIdProvider" /> implementation.
    ///     
    ///     
    /// </summary>
    public class TenantIdProviderHistoricDecisionInstanceContext
    {
        protected internal IDelegateCaseExecution caseExecution;

        protected internal IDecisionDefinition decisionDefinition;

        protected internal IDelegateExecution execution;

        public TenantIdProviderHistoricDecisionInstanceContext(IDecisionDefinition decisionDefinition)
        {
            this.decisionDefinition = decisionDefinition;
        }

        public TenantIdProviderHistoricDecisionInstanceContext(IDecisionDefinition decisionDefinition,
            IDelegateExecution execution) : this(decisionDefinition)
        {
            this.execution = execution;
        }

        public TenantIdProviderHistoricDecisionInstanceContext(IDecisionDefinition decisionDefinition,
            IDelegateCaseExecution caseExecution) : this(decisionDefinition)
        {
            this.caseExecution = caseExecution;
        }

        /// <returns> the decision definition of the historic decision instance which is being evaluated </returns>
        public virtual IDecisionDefinition DecisionDefinition
        {
            get { return decisionDefinition; }
        }

        /// <returns>
        ///     the execution. This method returns the execution of the process instance
        ///     which evaluated the decision definition.
        /// </returns>
        public virtual IDelegateExecution Execution
        {
            get { return execution; }
        }

        /// <returns>
        ///     the case execution. This method returns the case execution of the CMMN case task
        ///     which evaluated the decision definition.
        /// </returns>
        public virtual IDelegateCaseExecution CaseExecution
        {
            get { return caseExecution; }
        }
    }
}