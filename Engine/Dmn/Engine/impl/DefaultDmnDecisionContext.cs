using System;
using System.Collections.Generic;


using ESS.FW.Bpm.Engine.Dmn.engine.@delegate;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.@delegate;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.evaluation;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.hitpolicy;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Context;
using ESS.FW.Bpm.Model.Dmn;


namespace ESS.FW.Bpm.Engine.Dmn.engine.impl
{
    /// <summary>
    ///     Context which evaluates a decision on a given input
    /// </summary>
    public class DefaultDmnDecisionContext
    {
        protected internal static readonly DmnEngineLogger LOG = DmnLogger.ENGINE_LOGGER;

        protected internal static readonly HitPolicyEntry COLLECT_HIT_POLICY = new HitPolicyEntry(HitPolicy.Collect,
            BuiltinAggregator.COUNT);

        protected internal static readonly HitPolicyEntry RULE_ORDER_HIT_POLICY =
            new HitPolicyEntry(HitPolicy.RuleOrder, BuiltinAggregator.COUNT);

        protected internal readonly IDictionary<Type, IDmnDecisionLogicEvaluationHandler> evaluationHandlers;

        protected internal readonly IList<IDmnDecisionEvaluationListener> evaluationListeners;

        public DefaultDmnDecisionContext(DefaultDmnEngineConfiguration configuration)
        {
            evaluationListeners = configuration.DecisionEvaluationListeners;

            evaluationHandlers = new Dictionary<Type, IDmnDecisionLogicEvaluationHandler>();
            evaluationHandlers[typeof(DmnDecisionTableImpl)] = new DecisionTableEvaluationHandler(configuration);
            evaluationHandlers[typeof(DmnDecisionLiteralExpressionImpl)] =
                new DecisionLiteralExpressionEvaluationHandler(configuration);
        }

        /// <summary>
        ///     Evaluate a decision with the given <seealso cref="IVariableContext" />
        /// </summary>
        /// <param name="decision"> the decision to evaluate </param>
        /// <param name="variableContext"> the available variable context </param>
        /// <returns> the result of the decision evaluation </returns>
        public virtual IDmnDecisionResult evaluateDecision(IDmnDecision decision, IVariableContext variableContext)
        {
            if (ReferenceEquals(decision.Key, null))
                throw LOG.unableToFindAnyDecisionTable();
            var variableMap = buildVariableMapFromVariableContext(variableContext);

            IList<IDmnDecision> requiredDecisions = new List<IDmnDecision>();
            buildDecisionTree(decision, requiredDecisions);

            IList<IDmnDecisionLogicEvaluationEvent> evaluatedEvents = new List<IDmnDecisionLogicEvaluationEvent>();
            IDmnDecisionResult evaluatedResult = null;

            foreach (var evaluateDecision in requiredDecisions)
            {
                var handler = getDecisionEvaluationHandler(evaluateDecision);
                var evaluatedEvent = handler.Evaluate(evaluateDecision, variableMap.AsVariableContext());
                evaluatedEvents.Add(evaluatedEvent);

                evaluatedResult = handler.GenerateDecisionResult(evaluatedEvent);
                if (decision != evaluateDecision)
                    addResultToVariableContext(evaluatedResult, variableMap, evaluateDecision);
            }

            generateDecisionEvaluationEvent(evaluatedEvents);
            return evaluatedResult;
        }

        protected internal virtual IVariableMap buildVariableMapFromVariableContext(IVariableContext variableContext)
        {
            var variableMap = Variables.CreateVariables();

            var variables = variableContext.KeySet();
            foreach (var variable in variables)
                variableMap.PutValue(variable, variableContext.Resolve(variable));

            return variableMap;
        }

        protected internal virtual void buildDecisionTree(IDmnDecision decision, IList<IDmnDecision> requiredDecisions)
        {
            if (requiredDecisions.Contains(decision))
                return;

            foreach (var dmnDecision in decision.RequiredDecisions)
                buildDecisionTree(dmnDecision, requiredDecisions);

            requiredDecisions.Add(decision);
        }

        protected internal virtual IDmnDecisionLogicEvaluationHandler getDecisionEvaluationHandler(IDmnDecision decision)
        {
            var key = decision.DecisionLogic.GetType();

            if (evaluationHandlers.ContainsKey(key))
                return evaluationHandlers[key];
            throw LOG.decisionLogicTypeNotSupported(decision.DecisionLogic);
        }

        protected internal virtual void addResultToVariableContext(IDmnDecisionResult evaluatedResult,
            IVariableMap variableMap, IDmnDecision evaluatedDecision)
        {
            var resultList = evaluatedResult.ResultList;

            if (resultList.Count == 0)
            {
            }
            else if ((resultList.Count == 1) && !isDecisionTableWithCollectOrRuleOrderHitPolicy(evaluatedDecision))
            {
                foreach (var it in evaluatedResult.SingleResult)
                    variableMap.PutValue(it.Key, it.Value);
                //variableMap.putAll(evaluatedResult.SingleResult);
            }
            else
            {
                ISet<string> outputs = new HashSet<string>();

                foreach (var resultMap in resultList)
                    outputs.UnionWith(resultMap.Keys);

                foreach (var output in outputs)
                {
                    var values = evaluatedResult.CollectEntries<object>(output);
                    variableMap.PutValue(output, values);
                }
            }
        }

        protected internal virtual bool isDecisionTableWithCollectOrRuleOrderHitPolicy(IDmnDecision evaluatedDecision)
        {
            var isDecisionTableWithCollectHitPolicy = false;

            if (evaluatedDecision.DecisionTable)
            {
                var decisionTable = (DmnDecisionTableImpl) evaluatedDecision.DecisionLogic;
                isDecisionTableWithCollectHitPolicy =
                    COLLECT_HIT_POLICY.Equals(decisionTable.HitPolicyHandler.HitPolicyEntry) ||
                    RULE_ORDER_HIT_POLICY.Equals(decisionTable.HitPolicyHandler.HitPolicyEntry);
            }

            return isDecisionTableWithCollectHitPolicy;
        }

        protected internal virtual void generateDecisionEvaluationEvent(
            IList<IDmnDecisionLogicEvaluationEvent> evaluatedEvents)
        {
            IDmnDecisionLogicEvaluationEvent rootEvaluatedEvent = null;
            var decisionEvaluationEvent = new DmnDecisionEvaluationEventImpl();
            var executedDecisionElements = 0L;

            foreach (var evaluatedEvent in evaluatedEvents)
            {
                executedDecisionElements += evaluatedEvent.ExecutedDecisionElements;
                rootEvaluatedEvent = evaluatedEvent;
            }

            decisionEvaluationEvent.DecisionResult = rootEvaluatedEvent;
            decisionEvaluationEvent.ExecutedDecisionElements = executedDecisionElements;

            evaluatedEvents.Remove(rootEvaluatedEvent);
            decisionEvaluationEvent.RequiredDecisionResults = evaluatedEvents;

            foreach (var evaluationListener in evaluationListeners)
                evaluationListener.Notify(decisionEvaluationEvent);
        }
    }
}