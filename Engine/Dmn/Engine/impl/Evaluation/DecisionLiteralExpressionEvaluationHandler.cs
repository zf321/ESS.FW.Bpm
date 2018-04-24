using System.Collections.Generic;

using ESS.FW.Bpm.Engine.Dmn.engine.@delegate;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.@delegate;
using ESS.FW.Bpm.Engine.Variable.Context;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.evaluation
{
    public class DecisionLiteralExpressionEvaluationHandler : IDmnDecisionLogicEvaluationHandler
    {
        protected internal readonly ExpressionEvaluationHandler expressionEvaluationHandler;

        protected internal readonly string LiteralExpressionLanguage;

        public DecisionLiteralExpressionEvaluationHandler(DefaultDmnEngineConfiguration configuration)
        {
            expressionEvaluationHandler = new ExpressionEvaluationHandler(configuration);

            LiteralExpressionLanguage = configuration.DefaultLiteralExpressionLanguage;
        }

        public virtual IDmnDecisionLogicEvaluationEvent Evaluate(IDmnDecision decision, IVariableContext variableContext)
        {
            var evaluationResult = new DmnDecisionLiteralExpressionEvaluationEventImpl();
            evaluationResult.Decision = decision;
            evaluationResult.ExecutedDecisionElements = 1;

            var dmnDecisionLiteralExpression = (DmnDecisionLiteralExpressionImpl) decision.DecisionLogic;
            var variable = dmnDecisionLiteralExpression.Variable;
            var expression = dmnDecisionLiteralExpression.Expression;

            var evaluateExpression = EvaluateLiteralExpression(expression, variableContext);
            ITypedValue typedValue = variable.TypeDefinition.Transform(evaluateExpression);

            evaluationResult.OutputValue = typedValue;
            evaluationResult.OutputName = variable.Name;

            return evaluationResult;
        }

        public virtual IDmnDecisionResult GenerateDecisionResult(IDmnDecisionLogicEvaluationEvent @event)
        {
            var evaluationEvent = (IDmnDecisionLiteralExpressionEvaluationEvent) @event;

            var result = new DmnDecisionResultEntriesImpl();
            result.Add(evaluationEvent.OutputName, evaluationEvent.OutputValue);

            return new DmnDecisionResultImpl(new List<IDmnDecisionResultEntries> {result});
        }

        protected internal virtual object EvaluateLiteralExpression(DmnExpressionImpl expression,
            IVariableContext variableContext)
        {
            var expressionLanguage = expression.ExpressionLanguage;
            if (ReferenceEquals(expressionLanguage, null))
                expressionLanguage = LiteralExpressionLanguage;
            return expressionEvaluationHandler.EvaluateExpression(expressionLanguage, expression, variableContext);
        }
    }
}