using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Util.xml;
using ESS.FW.Bpm.Engine.Impl.Variable;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Variable;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Parser
{
    /// <summary>
    ///     Listener which can be registered within the engine to receive events during parsing (and
    ///     maybe influence it). Instead of implementing this interface you might consider to extend
    ///     the <seealso cref="AbstractBpmnParseListener" />, which contains an empty implementation for all methods
    ///     and makes your implementation easier and more robust to future changes.
    ///     
    ///     
    /// </summary>
    public interface IBpmnParseListener
    {
        void ParseProcess(Element processElement, ProcessDefinitionEntity processDefinition);
        void ParseStartEvent(Element startEventElement, ScopeImpl scope, ActivityImpl startEventActivity);
        void ParseExclusiveGateway(Element exclusiveGwElement, ScopeImpl scope, ActivityImpl activity);
        void ParseInclusiveGateway(Element inclusiveGwElement, ScopeImpl scope, ActivityImpl activity);
        void ParseParallelGateway(Element parallelGwElement, ScopeImpl scope, ActivityImpl activity);
        void ParseScriptTask(Element scriptTaskElement, ScopeImpl scope, ActivityImpl activity);
        void ParseServiceTask(Element serviceTaskElement, ScopeImpl scope, ActivityImpl activity);
        void ParseBusinessRuleTask(Element businessRuleTaskElement, ScopeImpl scope, ActivityImpl activity);
        void ParseTask(Element taskElement, ScopeImpl scope, ActivityImpl activity);
        void ParseManualTask(Element manualTaskElement, ScopeImpl scope, ActivityImpl activity);
        void ParseUserTask(Element userTaskElement, ScopeImpl scope, ActivityImpl activity);
        void ParseEndEvent(Element endEventElement, ScopeImpl scope, ActivityImpl activity);

        void ParseBoundaryTimerEventDefinition(Element timerEventDefinition, bool interrupting,
            ActivityImpl timerActivity);

        void ParseBoundaryErrorEventDefinition(Element errorEventDefinition, bool interrupting, ActivityImpl activity,
            ActivityImpl nestedErrorEventActivity);

        void ParseSubProcess(Element subProcessElement, ScopeImpl scope, ActivityImpl activity);
        void ParseCallActivity(Element callActivityElement, ScopeImpl scope, ActivityImpl activity);
        void ParseProperty(Element propertyElement, VariableDeclaration variableDeclaration, ActivityImpl activity);
        void ParseSequenceFlow(Element sequenceFlowElement, ScopeImpl scopeElement, TransitionImpl transition);
        void ParseSendTask(Element sendTaskElement, ScopeImpl scope, ActivityImpl activity);

        void ParseMultiInstanceLoopCharacteristics(Element activityElement,
            Element multiInstanceLoopCharacteristicsElement, ActivityImpl activity);

        void ParseIntermediateTimerEventDefinition(Element timerEventDefinition, ActivityImpl timerActivity);
        void ParseRootElement(Element rootElement, IList<ProcessDefinitionEntity> processDefinitions);
        void ParseReceiveTask(Element receiveTaskElement, ScopeImpl scope, ActivityImpl activity);
        void ParseIntermediateSignalCatchEventDefinition(Element signalEventDefinition, ActivityImpl signalActivity);
        void ParseIntermediateMessageCatchEventDefinition(Element messageEventDefinition, ActivityImpl nestedActivity);

        void ParseBoundarySignalEventDefinition(Element signalEventDefinition, bool interrupting,
            ActivityImpl signalActivity);

        void ParseEventBasedGateway(Element eventBasedGwElement, ScopeImpl scope, ActivityImpl activity);
        void ParseTransaction(Element transactionElement, ScopeImpl scope, ActivityImpl activity);
        void ParseCompensateEventDefinition(Element compensateEventDefinition, ActivityImpl compensationActivity);
        void ParseIntermediateThrowEvent(Element intermediateEventElement, ScopeImpl scope, ActivityImpl activity);
        void ParseIntermediateCatchEvent(Element intermediateEventElement, ScopeImpl scope, ActivityImpl activity);
        void ParseBoundaryEvent(Element boundaryEventElement, ScopeImpl scopeElement, ActivityImpl nestedActivity);
        void ParseBoundaryMessageEventDefinition(Element element, bool interrupting, ActivityImpl messageActivity);

        void ParseBoundaryEscalationEventDefinition(Element escalationEventDefinition, bool interrupting,
            ActivityImpl boundaryEventActivity);

        void ParseBoundaryConditionalEventDefinition(Element element, bool interrupting,
            ActivityImpl conditionalActivity);

        void ParseIntermediateConditionalEventDefinition(Element conditionalEventDefinition,
            ActivityImpl conditionalActivity);

        void ParseConditionalStartEventForEventSubprocess(Element element, ActivityImpl conditionalActivity,
            bool interrupting);
    }
}