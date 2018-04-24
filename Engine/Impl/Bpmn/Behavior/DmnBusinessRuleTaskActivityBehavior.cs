using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Dmn.Impl.Result;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     Implementation of a Bpmn BusinessRuleTask executing a DMN Decision.
    ///     The decision is resolved as a <seealso cref="BaseCallableElement" />.
    ///     The decision is executed in the context of the current <seealso cref="IVariableScope" />.
    ///     
    /// </summary>
    public class DmnBusinessRuleTaskActivityBehavior : AbstractBpmnActivityBehavior
    {
        protected internal readonly BaseCallableElement CallableElement;
        protected internal readonly IDecisionResultMapper DecisionResultMapper;
        protected internal readonly string ResultVariable;

        public DmnBusinessRuleTaskActivityBehavior(BaseCallableElement callableElement, string resultVariableName,
            IDecisionResultMapper decisionResultMapper)
        {
            this.CallableElement = callableElement;
            ResultVariable = resultVariableName;
            this.DecisionResultMapper = decisionResultMapper;
        }
        
        public override void Execute(IActivityExecution execution)
        {
            ExecuteWithErrorPropagation(execution, () =>
            {
                DecisionEvaluationUtil.EvaluateDecision((AbstractVariableScope)execution, CallableElement,
                    ResultVariable, DecisionResultMapper);
                Leave(execution);
            });
        }
        
    }
}