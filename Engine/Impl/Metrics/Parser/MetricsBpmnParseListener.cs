using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Util.xml;

namespace ESS.FW.Bpm.Engine.Impl.Metrics.Parser
{
    /// <summary>
    ///     
    /// </summary>
    public class MetricsBpmnParseListener : AbstractBpmnParseListener
    {
        public static MetricsExecutionListener ActivityInstanceStartCounter =
            new MetricsExecutionListener(Engine.Management.Metrics.ActivtyInstanceStart);

        public static MetricsExecutionListener ActivityInstanceEndCounter =
            new MetricsExecutionListener(Engine.Management.Metrics.ActivtyInstanceEnd);

        protected internal virtual void AddListeners(ActivityImpl activity)
        {
            activity.AddBuiltInListener(ExecutionListenerFields.EventNameStart, ActivityInstanceStartCounter);
            activity.AddBuiltInListener(ExecutionListenerFields.EventNameEnd, ActivityInstanceEndCounter);
        }

        public override void ParseStartEvent(Element startEventElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddListeners(activity);
        }

        public override void ParseExclusiveGateway(Element exclusiveGwElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddListeners(activity);
        }

        public override void ParseInclusiveGateway(Element inclusiveGwElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddListeners(activity);
        }

        public override void ParseParallelGateway(Element parallelGwElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddListeners(activity);
        }

        public override void ParseScriptTask(Element scriptTaskElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddListeners(activity);
        }

        public override void ParseServiceTask(Element serviceTaskElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddListeners(activity);
        }

        public override void ParseBusinessRuleTask(Element businessRuleTaskElement, ScopeImpl scope,
            ActivityImpl activity)
        {
            AddListeners(activity);
        }

        public override void ParseTask(Element taskElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddListeners(activity);
        }

        public override void ParseManualTask(Element manualTaskElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddListeners(activity);
        }

        public override void ParseUserTask(Element userTaskElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddListeners(activity);
        }

        public override void ParseEndEvent(Element endEventElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddListeners(activity);
        }

        public override void ParseSubProcess(Element subProcessElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddListeners(activity);
        }

        public override void ParseCallActivity(Element callActivityElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddListeners(activity);
        }

        public override void ParseSendTask(Element sendTaskElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddListeners(activity);
        }

        public override void ParseReceiveTask(Element receiveTaskElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddListeners(activity);
        }

        public override void ParseEventBasedGateway(Element eventBasedGwElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddListeners(activity);
        }

        public override void ParseTransaction(Element transactionElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddListeners(activity);
        }

        public override void ParseIntermediateThrowEvent(Element intermediateEventElement, ScopeImpl scope,
            ActivityImpl activity)
        {
            AddListeners(activity);
        }

        public override void ParseIntermediateCatchEvent(Element intermediateEventElement, ScopeImpl scope,
            ActivityImpl activity)
        {
            AddListeners(activity);
        }

        public override void ParseBoundaryEvent(Element boundaryEventElement, ScopeImpl scopeElement,
            ActivityImpl activity)
        {
            AddListeners(activity);
        }

        public override void ParseMultiInstanceLoopCharacteristics(Element activityElement,
            Element multiInstanceLoopCharacteristicsElement, ActivityImpl activity)
        {
            AddListeners(activity);
        }
    }
}