using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Util.xml;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Impl.Variable;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Parser
{
    /// <summary>
    ///     Abstract base class for implementing a <seealso cref="IBpmnParseListener" /> without being forced to implement
    ///     all methods provided, which makes the implementation more robust to future changes.
    ///     
    /// </summary>
    public class AbstractBpmnParseListener : IBpmnParseListener
    {
        public virtual void ParseStartEvent(Element startEventElement, ScopeImpl scope, ActivityImpl startEventActivity)
        {
        }

        public virtual void ParseExclusiveGateway(Element exclusiveGwElement, ScopeImpl scope, ActivityImpl activity)
        {
        }

        public virtual void ParseInclusiveGateway(Element inclusiveGwElement, ScopeImpl scope, ActivityImpl activity)
        {
        }

        public virtual void ParseParallelGateway(Element parallelGwElement, ScopeImpl scope, ActivityImpl activity)
        {
        }

        public virtual void ParseScriptTask(Element scriptTaskElement, ScopeImpl scope, ActivityImpl activity)
        {
        }

        public virtual void ParseServiceTask(Element serviceTaskElement, ScopeImpl scope, ActivityImpl activity)
        {
        }

        public virtual void ParseBusinessRuleTask(Element businessRuleTaskElement, ScopeImpl scope,
            ActivityImpl activity)
        {
        }

        public virtual void ParseTask(Element taskElement, ScopeImpl scope, ActivityImpl activity)
        {
        }

        public virtual void ParseManualTask(Element manualTaskElement, ScopeImpl scope, ActivityImpl activity)
        {
        }

        public virtual void ParseUserTask(Element userTaskElement, ScopeImpl scope, ActivityImpl activity)
        {
        }

        public virtual void ParseEndEvent(Element endEventElement, ScopeImpl scope, ActivityImpl activity)
        {
        }

        public virtual void ParseBoundaryTimerEventDefinition(Element timerEventDefinition, bool interrupting,
            ActivityImpl timerActivity)
        {
        }

        public virtual void ParseBoundaryErrorEventDefinition(Element errorEventDefinition, bool interrupting,
            ActivityImpl activity, ActivityImpl nestedErrorEventActivity)
        {
        }

        public virtual void ParseSubProcess(Element subProcessElement, ScopeImpl scope, ActivityImpl activity)
        {
        }

        public virtual void ParseCallActivity(Element callActivityElement, ScopeImpl scope, ActivityImpl activity)
        {
        }

        public virtual void ParseProperty(Element propertyElement, VariableDeclaration variableDeclaration,
            ActivityImpl activity)
        {
        }

        public virtual void ParseSequenceFlow(Element sequenceFlowElement, ScopeImpl scopeElement,
            TransitionImpl transition)
        {
        }

        public virtual void ParseSendTask(Element sendTaskElement, ScopeImpl scope, ActivityImpl activity)
        {
        }

        public virtual void ParseMultiInstanceLoopCharacteristics(Element activityElement,
            Element multiInstanceLoopCharacteristicsElement, ActivityImpl activity)
        {
        }

        public virtual void ParseIntermediateTimerEventDefinition(Element timerEventDefinition,
            ActivityImpl timerActivity)
        {
        }

        public virtual void ParseReceiveTask(Element receiveTaskElement, ScopeImpl scope, ActivityImpl activity)
        {
        }

        public virtual void ParseIntermediateSignalCatchEventDefinition(Element signalEventDefinition,
            ActivityImpl signalActivity)
        {
        }

        public virtual void ParseBoundarySignalEventDefinition(Element signalEventDefinition, bool interrupting,
            ActivityImpl signalActivity)
        {
        }

        public virtual void ParseEventBasedGateway(Element eventBasedGwElement, ScopeImpl scope, ActivityImpl activity)
        {
        }

        public virtual void ParseTransaction(Element transactionElement, ScopeImpl scope, ActivityImpl activity)
        {
        }

        public virtual void ParseCompensateEventDefinition(Element compensateEventDefinition,
            ActivityImpl compensationActivity)
        {
        }

        public virtual void ParseIntermediateThrowEvent(Element intermediateEventElement, ScopeImpl scope,
            ActivityImpl activity)
        {
        }

        public virtual void ParseIntermediateCatchEvent(Element intermediateEventElement, ScopeImpl scope,
            ActivityImpl activity)
        {
        }

        public virtual void ParseBoundaryEvent(Element boundaryEventElement, ScopeImpl scopeElement,
            ActivityImpl nestedActivity)
        {
        }

        public virtual void ParseIntermediateMessageCatchEventDefinition(Element messageEventDefinition,
            ActivityImpl nestedActivity)
        {
        }

        public virtual void ParseBoundaryMessageEventDefinition(Element element, bool interrupting,
            ActivityImpl messageActivity)
        {
        }

        public virtual void ParseBoundaryEscalationEventDefinition(Element escalationEventDefinition, bool interrupting,
            ActivityImpl boundaryEventActivity)
        {
        }

        public virtual void ParseBoundaryConditionalEventDefinition(Element element, bool interrupting,
            ActivityImpl conditionalActivity)
        {
        }

        public virtual void ParseIntermediateConditionalEventDefinition(Element conditionalEventDefinition,
            ActivityImpl conditionalActivity)
        {
        }

        public virtual void ParseConditionalStartEventForEventSubprocess(Element element,
            ActivityImpl conditionalActivity, bool interrupting)
        {
        }



        public virtual void ParseProcess(Element processElement, ProcessDefinitionEntity processDefinition)
        {
        }

        public virtual void ParseRootElement(Element rootElement, IList<Persistence.Entity.ProcessDefinitionEntity> processDefinitions)
        {
        }
    }
}