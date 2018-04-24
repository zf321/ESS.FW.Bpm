using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Dmn.engine;
using ESS.FW.Bpm.Engine.Dmn.engine.@delegate;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Producer;
using ESS.FW.Bpm.Engine.Impl.Core.Instance;
using ESS.FW.Bpm.Engine.Repository;

namespace ESS.FW.Bpm.Engine.History.Impl.Parser
{
    public class HistoryDecisionEvaluationListener : IDmnDecisionEvaluationListener
    {
        protected internal IDmnHistoryEventProducer EventProducer;
        protected internal IHistoryLevel HistoryLevel;

        public HistoryDecisionEvaluationListener(IDmnHistoryEventProducer historyEventProducer,
            IHistoryLevel historyLevel)
        {
            EventProducer = historyEventProducer;
            this.HistoryLevel = historyLevel;
        }

        public virtual void Notify(IDmnDecisionEvaluationEvent evaluationEvent)
        {
            var historyEvent = CreateHistoryEvent(evaluationEvent);

            if (historyEvent != null)
                Context.ProcessEngineConfiguration.HistoryEventHandler.HandleEvent(historyEvent);
        }

        protected internal virtual HistoryEvent CreateHistoryEvent(IDmnDecisionEvaluationEvent evaluationEvent)
        {
            var decisionTable = evaluationEvent.DecisionResult.Decision;
            if (IsDeployedDecisionTable(decisionTable) &&
                HistoryLevel.IsHistoryEventProduced(HistoryEventTypes.DmnDecisionEvaluate, decisionTable))
            {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.bpm.engine.impl.context.CoreExecutionContext<? extends org.camunda.bpm.engine.impl.core.instance.CoreExecution> executionContext = org.camunda.bpm.engine.impl.context.Context.getCoreExecutionContext();
                var executionContext = Context.CoreExecutionContext;
                if (executionContext != null)
                {
                    //var coreExecution = executionContext.Execution;
                    var coreExecution = executionContext.GetExecution<CoreExecution>();

                    //if (coreExecution is ExecutionEntity)
                    //{
                    //    var execution = (ExecutionEntity) coreExecution;
                    //    return eventProducer.createDecisionEvaluatedEvt(execution, evaluationEvent);
                    //}
                    //if (coreExecution is CaseExecutionEntity)
                    //{
                    //    var caseExecution = (CaseExecutionEntity) coreExecution;
                    //    return eventProducer.createDecisionEvaluatedEvt(caseExecution, evaluationEvent);
                    //}
                }

                return EventProducer.CreateDecisionEvaluatedEvt(evaluationEvent);
            }
            return null;
        }

        protected internal virtual bool IsDeployedDecisionTable(IDmnDecision decision)
        {
            if (decision is IDecisionDefinition)
                return !ReferenceEquals(((IDecisionDefinition) decision).Id, null);
            return false;
        }
    }
}