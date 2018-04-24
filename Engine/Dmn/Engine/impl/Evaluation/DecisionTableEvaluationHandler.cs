using System;
using System.Collections.Generic;


using ESS.FW.Bpm.Engine.Dmn.engine.@delegate;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.@delegate;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Context;
using ESS.FW.Bpm.Engine.Variable.Context.Impl;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Dmn.Feel;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.evaluation
{
    public class DecisionTableEvaluationHandler : IDmnDecisionLogicEvaluationHandler
    {
        protected internal readonly IList<IDmnDecisionTableEvaluationListener> EvaluationListeners;

        protected internal readonly ExpressionEvaluationHandler expressionEvaluationHandler;
        protected internal readonly IFeelEngine feelEngine;
        protected internal readonly string inputEntryExpressionLanguage;

        protected internal readonly string inputExpressionExpressionLanguage;
        protected internal readonly string outputEntryExpressionLanguage;

        public DecisionTableEvaluationHandler(DefaultDmnEngineConfiguration configuration)
        {
            expressionEvaluationHandler = new ExpressionEvaluationHandler(configuration);
            feelEngine = configuration.FeelEngine;

            EvaluationListeners = configuration.DecisionTableEvaluationListeners;

            inputExpressionExpressionLanguage = configuration.DefaultInputExpressionExpressionLanguage;
            inputEntryExpressionLanguage = configuration.DefaultInputEntryExpressionLanguage;
            outputEntryExpressionLanguage = configuration.DefaultOutputEntryExpressionLanguage;
        }

        public virtual IDmnDecisionLogicEvaluationEvent Evaluate(IDmnDecision decision, IVariableContext variableContext)
        {
            var evaluationResult = new DmnDecisionTableEvaluationEventImpl();
            evaluationResult.DecisionTable = decision;

            var decisionTable = (DmnDecisionTableImpl) decision.DecisionLogic;
            evaluationResult.ExecutedDecisionElements = CalculateExecutedDecisionElements(decisionTable);

            EvaluateDecisionTable(decisionTable, variableContext, evaluationResult);

            // apply hit policy
            decisionTable.HitPolicyHandler.apply(evaluationResult);

            // notify listeners
            foreach (var evaluationListener in EvaluationListeners)
                evaluationListener.Notify(evaluationResult);

            return evaluationResult;
        }

        public virtual IDmnDecisionResult GenerateDecisionResult(IDmnDecisionLogicEvaluationEvent @event)
        {
            var evaluationResult = (IDmnDecisionTableEvaluationEvent) @event;

            IList<IDmnDecisionResultEntries> ruleResults = new List<IDmnDecisionResultEntries>();

            if (!ReferenceEquals(evaluationResult.CollectResultName, null) ||
                (evaluationResult.CollectResultValue != null))
            {
                var ruleResult = new DmnDecisionResultEntriesImpl();
                ruleResult.PutValue(evaluationResult.CollectResultName, evaluationResult.CollectResultValue);
                ruleResults.Add(ruleResult);
            }
            else
            {
                foreach (var evaluatedRule in evaluationResult.MatchingRules)
                {
                    var ruleResult = new DmnDecisionResultEntriesImpl();
                    foreach (var evaluatedOutput in evaluatedRule.OutputEntries.Values)
                    {
                        ruleResult.PutValue(evaluatedOutput.OutputName, evaluatedOutput.Value);
                    }
                    ruleResults.Add(ruleResult);
                }
            }

            return new DmnDecisionResultImpl(ruleResults);
        }

        protected internal virtual long CalculateExecutedDecisionElements(DmnDecisionTableImpl decisionTable)
        {
            return (decisionTable.Inputs.Count + decisionTable.Outputs.Count)*decisionTable.Rules.Count;
        }

        protected internal virtual void EvaluateDecisionTable(DmnDecisionTableImpl decisionTable,
            IVariableContext variableContext, DmnDecisionTableEvaluationEventImpl evaluationResult)
        {
            var inputSize = decisionTable.Inputs.Count;
            IList<DmnDecisionTableRuleImpl> matchingRules = new List<DmnDecisionTableRuleImpl>(decisionTable.Rules);
            for (var inputIdx = 0; inputIdx < inputSize; inputIdx++)
            {
                // evaluate input
                var input = decisionTable.Inputs[inputIdx];
                var evaluatedInput = EvaluateInput(input, variableContext);
                evaluationResult.Inputs.Add(evaluatedInput);

                // compose local variable context out of global variable context enhanced with the value of the current input.
                var localVariableContext = GetLocalVariableContext(input, evaluatedInput, variableContext);

                // filter rules applicable with this input
                matchingRules = EvaluateInputForAvailableRules(inputIdx, input, matchingRules, localVariableContext);
            }

            SetEvaluationOutput(decisionTable, matchingRules, variableContext, evaluationResult);
        }

        protected internal virtual IDmnEvaluatedInput EvaluateInput(DmnDecisionTableInputImpl input,
            IVariableContext variableContext)
        {
            var evaluatedInput = new DmnEvaluatedInputImpl(input);

            var expression = input.Expression;
            if (expression != null)
            {
                object value = EvaluateInputExpression(expression, variableContext);
                ITypedValue typedValue = expression.TypeDefinition.Transform(value);
                evaluatedInput.Value = typedValue;
            }
            else
            {
                evaluatedInput.Value = Variables.UntypedNullValue();
            }

            return evaluatedInput;
        }

        protected internal virtual IList<DmnDecisionTableRuleImpl> EvaluateInputForAvailableRules(int conditionIdx,
            DmnDecisionTableInputImpl input, IList<DmnDecisionTableRuleImpl> availableRules,
            IVariableContext variableContext)
        {
            IList<DmnDecisionTableRuleImpl> matchingRules = new List<DmnDecisionTableRuleImpl>();
            foreach (var availableRule in availableRules)
            {
                var condition = availableRule.Conditions[conditionIdx];
                if (IsConditionApplicable(input, condition, variableContext))
                    matchingRules.Add(availableRule);
            }
            return matchingRules;
        }

        protected internal virtual bool IsConditionApplicable(DmnDecisionTableInputImpl input,
            DmnExpressionImpl condition, IVariableContext variableContext)
        {
            var result = EvaluateInputEntry(input, condition, variableContext);
            return (result != null) && result.Equals(true);
        }

        protected internal virtual void SetEvaluationOutput(DmnDecisionTableImpl decisionTable,
            IList<DmnDecisionTableRuleImpl> matchingRules, IVariableContext variableContext,
            DmnDecisionTableEvaluationEventImpl evaluationResult)
        {
            var decisionTableOutputs = decisionTable.Outputs;

            IList<IDmnEvaluatedDecisionRule> evaluatedDecisionRules = new List<IDmnEvaluatedDecisionRule>();
            foreach (var matchingRule in matchingRules)
            {
                var evaluatedRule = EvaluateMatchingRule(decisionTableOutputs, matchingRule, variableContext);
                evaluatedDecisionRules.Add(evaluatedRule);
            }
            evaluationResult.MatchingRules = evaluatedDecisionRules;
        }

        protected internal virtual IDmnEvaluatedDecisionRule EvaluateMatchingRule(
            IList<DmnDecisionTableOutputImpl> decisionTableOutputs, DmnDecisionTableRuleImpl matchingRule,
            IVariableContext variableContext)
        {
            var evaluatedDecisionRule = new DmnEvaluatedDecisionRuleImpl(matchingRule);
            var outputEntries = EvaluateOutputEntries(decisionTableOutputs, matchingRule, variableContext);
            evaluatedDecisionRule.OutputEntries = outputEntries;

            return evaluatedDecisionRule;
        }

        protected internal virtual IVariableContext GetLocalVariableContext(DmnDecisionTableInputImpl input,
            IDmnEvaluatedInput evaluatedInput, IVariableContext variableContext)
        {
            if (IsNonEmptyExpression(input.Expression))
            {
                var inputVariableName = evaluatedInput.InputVariable;

                return
                    CompositeVariableContext.Compose(
                        Variables.CreateVariables()
                            .PutValue("inputVariableName", inputVariableName)
                            .PutValueTyped(inputVariableName, evaluatedInput.Value)
                            .AsVariableContext(), variableContext);
            }
            return variableContext;
        }

        protected internal virtual bool IsNonEmptyExpression(DmnExpressionImpl expression)
        {
            return (expression != null) && !ReferenceEquals(expression.Expression, null) &&
                   (expression.Expression.Trim().Length > 0);
        }

        protected internal virtual object EvaluateInputExpression(DmnExpressionImpl expression,
            IVariableContext variableContext)
        {
            var expressionLanguage = expression.ExpressionLanguage;
            if (ReferenceEquals(expressionLanguage, null))
                expressionLanguage = inputExpressionExpressionLanguage;
            return expressionEvaluationHandler.EvaluateExpression(expressionLanguage, expression, variableContext);
        }

        protected internal virtual object EvaluateInputEntry(DmnDecisionTableInputImpl input,
            DmnExpressionImpl condition, IVariableContext variableContext)
        {
            if (IsNonEmptyExpression(condition))
            {
                var expressionLanguage = condition.ExpressionLanguage;
                if (ReferenceEquals(expressionLanguage, null))
                    expressionLanguage = inputEntryExpressionLanguage;
                if (expressionEvaluationHandler.IsFeelExpressionLanguage(expressionLanguage))
                    return EvaluateFeelSimpleUnaryTests(input, condition, variableContext);
                return expressionEvaluationHandler.EvaluateExpression(expressionLanguage, condition, variableContext);
            }
            return true; // input entries without expressions are true
        }

        protected internal virtual IDictionary<string, IDmnEvaluatedOutput> EvaluateOutputEntries(
            IList<DmnDecisionTableOutputImpl> decisionTableOutputs, DmnDecisionTableRuleImpl matchingRule,
            IVariableContext variableContext)
        {
            IDictionary<string, IDmnEvaluatedOutput> outputEntries = new Dictionary<string, IDmnEvaluatedOutput>();

            for (var outputIdx = 0; outputIdx < decisionTableOutputs.Count; outputIdx++)
            {
                // evaluate output entry, skip empty expressions
                var conclusion = matchingRule.Conclusions[outputIdx];
                if (IsNonEmptyExpression(conclusion))
                {
                    var value = EvaluateOutputEntry(conclusion, variableContext);

                    // transform to output type
                    var decisionTableOutput = decisionTableOutputs[outputIdx];
                    var typedValue = decisionTableOutput.TypeDefinition.Transform(value);

                    // set on result
                    var evaluatedOutput = new DmnEvaluatedOutputImpl(decisionTableOutput, typedValue);
                    outputEntries[decisionTableOutput.OutputName] = evaluatedOutput;
                }
            }

            return outputEntries;
        }

        protected internal virtual object EvaluateOutputEntry(DmnExpressionImpl conclusion,
            IVariableContext variableContext)
        {
            var expressionLanguage = conclusion.ExpressionLanguage;
            if (ReferenceEquals(expressionLanguage, null))
                expressionLanguage = outputEntryExpressionLanguage;
            return expressionEvaluationHandler.EvaluateExpression(expressionLanguage, conclusion, variableContext);
            //return null;
        }

        protected internal virtual object EvaluateFeelSimpleUnaryTests(DmnDecisionTableInputImpl input,
            DmnExpressionImpl condition, IVariableContext variableContext)
        {
            //return new NotImplementedException();
            var expressionText = condition.Expression;
            if (!ReferenceEquals(expressionText, null))
                return feelEngine.EvaluateSimpleUnaryTests(expressionText, input.InputVariable, variableContext);
            return null;
        }
    }
}