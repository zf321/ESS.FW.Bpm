
using ESS.FW.Bpm.Engine.Dmn.engine;
using ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository;
using ESS.FW.Bpm.Engine.Impl.Delegate;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Variable.Context;

namespace ESS.FW.Bpm.Engine.Dmn.Impl.Invocation
{
    /// <summary>
    ///     <seealso cref="DelegateInvocation" /> invoking a <seealso cref="DecisionDefinition" />
    ///     in a given <seealso cref="IVariableContext" />.
    ///     The DmnEngine instance is resolved from the Context.
    ///     The invocation result is a <seealso cref="IDmnDecisionResult" />.
    ///     The target of the invocation is the <seealso cref="DecisionDefinition" />.
    ///     
    /// </summary>
    public class DecisionInvocation : DelegateInvocation
    {
        protected internal IDecisionDefinition decisionDefinition;
        protected internal IVariableContext VariableContext;

        public DecisionInvocation(IDecisionDefinition decisionDefinition, IVariableContext variableContext)
            : base(null, (DecisionDefinitionEntity) decisionDefinition)
        {
            this.decisionDefinition = decisionDefinition;
            this.VariableContext = variableContext;
        }

        public override object InvocationResult
        {
            get { return (IDmnDecisionResult) base.InvocationResult; }
        }

        public virtual IDecisionDefinition DecisionDefinition
        {
            get { return decisionDefinition; }
        }
        protected internal override void Invoke()
        {
            IDmnEngine dmnEngine =context.Impl.Context.ProcessEngineConfiguration.DmnEngine;

            base.InvocationResult = dmnEngine.EvaluateDecision((IDmnDecision) decisionDefinition, VariableContext);
        }
    }
}