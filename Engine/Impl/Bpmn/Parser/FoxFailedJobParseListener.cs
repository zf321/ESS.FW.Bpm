using System;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Helper;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Util.xml;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Parser
{
    public class FoxFailedJobParseListener : AbstractBpmnParseListener
    {
        protected internal const string Type = "type";
        protected internal const string StartTimerEvent = "startTimerEvent";
        protected internal const string BoundaryTimer = "boundaryTimer";
        protected internal const string IntermediateSignalThrow = "intermediateSignalThrow";
        protected internal const string IntermediateTimer = "intermediateTimer";
        protected internal const string SignalEventDefinition = "signalEventDefinition";
        protected internal const string MultiInstanceLoopCharacteristics = "multiInstanceLoopCharacteristics";

        protected internal const string ExtensionElements = "extensionElements";
        protected internal const string FailedJobRetryTimeCycle = "failedJobRetryTimeCycle";

        /// <summary>
        ///     deprecated since 7.4, use camunda ns.
        /// </summary>
        [Obsolete] public static readonly Namespace FoxEngineNs = new Namespace("http://www.camunda.com/fox");

        public static readonly PropertyKey<string> FoxFailedJobConfiguration =
            new PropertyKey<string>("FOX_FAILED_JOB_CONFIGURATION");

        public override void ParseStartEvent(Element startEventElement, ScopeImpl scope, ActivityImpl startEventActivity)
        {
            var type = startEventActivity.Properties.Get(BpmnProperties.Type);
            if (!ReferenceEquals(type, null) && type.Equals(StartTimerEvent))
                SetFailedJobRetryTimeCycleValue(startEventElement, startEventActivity);
        }

        public override void ParseBoundaryEvent(Element boundaryEventElement, ScopeImpl scopeElement,
            ActivityImpl nestedActivity)
        {
            var type = nestedActivity.Properties.Get(BpmnProperties.Type);
            if (!ReferenceEquals(type, null) && type.Equals(BoundaryTimer))
                SetFailedJobRetryTimeCycleValue(boundaryEventElement, nestedActivity);
        }

        public override void ParseIntermediateThrowEvent(Element intermediateEventElement, ScopeImpl scope,
            ActivityImpl activity)
        {
            var type = activity.Properties.Get(BpmnProperties.Type);
            if (!ReferenceEquals(type, null))
                SetFailedJobRetryTimeCycleValue(intermediateEventElement, activity);
        }

        public override void ParseIntermediateCatchEvent(Element intermediateEventElement, ScopeImpl scope,
            ActivityImpl activity)
        {
            var type = activity.Properties.Get(BpmnProperties.Type);
            if (!ReferenceEquals(type, null) && type.Equals(IntermediateTimer))
                SetFailedJobRetryTimeCycleValue(intermediateEventElement, activity);
        }

        public override void ParseScriptTask(Element scriptTaskElement, ScopeImpl scope, ActivityImpl activity)
        {
            ParseActivity(scriptTaskElement, activity);
        }

        public override void ParseServiceTask(Element serviceTaskElement, ScopeImpl scope, ActivityImpl activity)
        {
            ParseActivity(serviceTaskElement, activity);
        }

        public override void ParseBusinessRuleTask(Element businessRuleTaskElement, ScopeImpl scope,
            ActivityImpl activity)
        {
            ParseActivity(businessRuleTaskElement, activity);
        }

        public override void ParseTask(Element taskElement, ScopeImpl scope, ActivityImpl activity)
        {
            ParseActivity(taskElement, activity);
        }

        public override void ParseUserTask(Element userTaskElement, ScopeImpl scope, ActivityImpl activity)
        {
            ParseActivity(userTaskElement, activity);
        }

        public override void ParseCallActivity(Element callActivityElement, ScopeImpl scope, ActivityImpl activity)
        {
            ParseActivity(callActivityElement, activity);
        }

        public override void ParseReceiveTask(Element receiveTaskElement, ScopeImpl scope, ActivityImpl activity)
        {
            ParseActivity(receiveTaskElement, activity);
        }

        public override void ParseSendTask(Element sendTaskElement, ScopeImpl scope, ActivityImpl activity)
        {
            ParseActivity(sendTaskElement, activity);
        }

        public override void ParseSubProcess(Element subProcessElement, ScopeImpl scope, ActivityImpl activity)
        {
            ParseActivity(subProcessElement, activity);
        }

        public override void ParseTransaction(Element transactionElement, ScopeImpl scope, ActivityImpl activity)
        {
            ParseActivity(transactionElement, activity);
        }

        protected internal virtual bool IsAsync(ActivityImpl activity)
        {
            return activity.AsyncBefore || activity.AsyncAfter;
        }

        protected internal virtual void ParseActivity(Element element, ActivityImpl activity)
        {
            if (IsMultiInstance(activity))
            {
                // in case of multi-instance, the extension elements is set according to the async attributes
                // the extension for multi-instance body is set on the element of the activity
                var miBody = activity.ParentFlowScopeActivity;
                if (IsAsync(miBody))
                    SetFailedJobRetryTimeCycleValue(element, miBody);
                // the extension for inner activity is set on the multiInstanceLoopCharacteristics element
                if (IsAsync(activity))
                {
                    var multiInstanceLoopCharacteristics = element.element(MultiInstanceLoopCharacteristics);
                    SetFailedJobRetryTimeCycleValue(multiInstanceLoopCharacteristics, activity);
                }
            }
            else if (IsAsync(activity))
            {
                SetFailedJobRetryTimeCycleValue(element, activity);
            }
        }

        protected internal virtual void SetFailedJobRetryTimeCycleValue(Element element, ActivityImpl activity)
        {
            var extensionElements = element.element(ExtensionElements);
            if (extensionElements != null)
            {
                var failedJobRetryTimeCycleElement = extensionElements.ElementNs(FoxEngineNs,
                    FailedJobRetryTimeCycle);
                if (failedJobRetryTimeCycleElement == null)
                    failedJobRetryTimeCycleElement = extensionElements.ElementNs(BpmnParse.CamundaBpmnExtensionsNs,
                        FailedJobRetryTimeCycle);
                if (failedJobRetryTimeCycleElement != null)
                {
                    var failedJobRetryTimeCycleValue = failedJobRetryTimeCycleElement.Text;
                    activity.Properties.Set(FoxFailedJobConfiguration, failedJobRetryTimeCycleValue);
                }
            }
        }

        protected internal virtual bool IsMultiInstance(ActivityImpl activity)
        {
            // #isMultiInstance() don't work since the property is not set yet
            var parent = activity.ParentFlowScopeActivity;
            return (parent != null) && parent.ActivityBehavior is MultiInstanceActivityBehavior;
        }
    }
}