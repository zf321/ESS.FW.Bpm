using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Dmn;
using ESS.FW.Bpm.Engine.Dmn.Impl;

namespace ESS.FW.Bpm.Engine.Impl
{
    /// <summary>
    ///     
    /// </summary>
    public class DecisionServiceImpl : ServiceImpl, IDecisionService
    {
        public virtual IDmnDecisionTableResult EvaluateDecisionTableById(string decisionDefinitionId,
            IDictionary<string, object> variables)
        {
            return EvaluateDecisionTableById(decisionDefinitionId).SetVariables(variables).Evaluate();
        }

        public virtual IDmnDecisionTableResult EvaluateDecisionTableByKey(string decisionDefinitionKey,
            IDictionary<string, object> variables)
        {
            return EvaluateDecisionTableByKey(decisionDefinitionKey).SetVariables(variables).Evaluate();
        }

        public virtual IDmnDecisionTableResult EvaluateDecisionTableByKeyAndVersion(string decisionDefinitionKey,
            int? version, IDictionary<string, object> variables)
        {
            return EvaluateDecisionTableByKey(decisionDefinitionKey).SetVersion(version).SetVariables(variables).Evaluate();
        }

        public virtual IDecisionEvaluationBuilder EvaluateDecisionTableByKey(string decisionDefinitionKey)
        {
            return DecisionTableEvaluationBuilderImpl.EvaluateDecisionTableByKey(CommandExecutor, decisionDefinitionKey);
        }

        public virtual IDecisionEvaluationBuilder EvaluateDecisionTableById(string decisionDefinitionId)
        {
            return DecisionTableEvaluationBuilderImpl.EvaluateDecisionTableById(CommandExecutor, decisionDefinitionId);
        }

        public virtual IDecisionsEvaluationBuilder EvaluateDecisionByKey(string decisionDefinitionKey)
        {
            return DecisionEvaluationBuilderImpl.EvaluateDecisionByKey(CommandExecutor, decisionDefinitionKey);
        }

        public virtual IDecisionsEvaluationBuilder EvaluateDecisionById(string decisionDefinitionId)
        {
            return DecisionEvaluationBuilderImpl.EvaluateDecisionById(CommandExecutor, decisionDefinitionId);
        }
    }
}