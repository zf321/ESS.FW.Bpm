using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Handler;
using ESS.FW.Bpm.Engine.History.Impl.Producer;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Util.xml;
using ESS.FW.Bpm.Engine.Impl.Variable;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.History.Impl.Parser
{
    /// <summary>
    ///     <para>
    ///         This class is responsible for wiring history as execution listeners into process execution.
    ///     </para>
    ///     <para>
    ///         NOTE: the role of this class has changed since 7.0: in order to customize history behavior it is
    ///         usually not necessary to override this class but rather the <seealso cref="IHistoryEventProducer" /> for
    ///         customizing data acquisition and <seealso cref="IHistoryEventHandler" /> for customizing the persistence
    ///         behavior
    ///         or if you need a history event stream.
    ///          
    ///         
    ///     </para>
    /// </summary>
    public class HistoryParseListener : IBpmnParseListener
    {
        protected internal IDelegateListener<IBaseDelegateExecution> ActivityInstanceEndListener;

        protected internal IDelegateListener<IBaseDelegateExecution> ActivityInstanceStartListener;

        // The history level set in the process engine configuration
        protected internal IHistoryLevel HistoryLevel;
        protected internal IDelegateListener<IBaseDelegateExecution> ProcessInstanceEndListener;

        // Cached listeners
        // listeners can be reused for a given process engine instance but cannot be cached in static fields since
        // different process engine instances on the same Classloader may have different HistoryEventProducer
        // configurations wired
        protected internal IDelegateListener<IBaseDelegateExecution> ProcessInstanceStartListener;

        protected internal ITaskListener UserTaskAssignmentHandler;
        protected internal ITaskListener UserTaskIdHandler;

        public HistoryParseListener(IHistoryLevel historyLevel, IHistoryEventProducer historyEventProducer)
        {
            this.HistoryLevel = historyLevel;
            InitExecutionListeners(historyEventProducer, historyLevel);
        }

        public virtual void ParseExclusiveGateway(Element exclusiveGwElement, ScopeImpl scope, ActivityImpl activity)
        {
            addActivityHandlers(activity);
        }

        public virtual void ParseInclusiveGateway(Element inclusiveGwElement, ScopeImpl scope, ActivityImpl activity)
        {
            addActivityHandlers(activity);
        }

        public virtual void ParseCallActivity(Element callActivityElement, ScopeImpl scope, ActivityImpl activity)
        {
            addActivityHandlers(activity);
        }

        public virtual void ParseManualTask(Element manualTaskElement, ScopeImpl scope, ActivityImpl activity)
        {
            addActivityHandlers(activity);
        }

        public virtual void ParseReceiveTask(Element receiveTaskElement, ScopeImpl scope, ActivityImpl activity)
        {
            addActivityHandlers(activity);
        }

        public virtual void ParseScriptTask(Element scriptTaskElement, ScopeImpl scope, ActivityImpl activity)
        {
            addActivityHandlers(activity);
        }

        public virtual void ParseTask(Element taskElement, ScopeImpl scope, ActivityImpl activity)
        {
            addActivityHandlers(activity);
        }

        public virtual void ParseUserTask(Element userTaskElement, ScopeImpl scope, ActivityImpl activity)
        {
            addActivityHandlers(activity);

            if (HistoryLevel.IsHistoryEventProduced(HistoryEventTypes.TaskInstanceCreate, null))
            {
                var taskDefinition = ((UserTaskActivityBehavior) activity.ActivityBehavior).TaskDefinition;
                taskDefinition.AddBuiltInTaskListener(TaskListenerFields.EventnameAssignment,
                    UserTaskAssignmentHandler);
                taskDefinition.AddBuiltInTaskListener(TaskListenerFields.EventnameCreate, UserTaskIdHandler);
            }
        }

        public virtual void ParseServiceTask(Element serviceTaskElement, ScopeImpl scope, ActivityImpl activity)
        {
            addActivityHandlers(activity);
        }

        public virtual void ParseBusinessRuleTask(Element businessRuleTaskElement, ScopeImpl scope,
            ActivityImpl activity)
        {
            addActivityHandlers(activity);
        }

        public virtual void ParseSubProcess(Element subProcessElement, ScopeImpl scope, ActivityImpl activity)
        {
            addActivityHandlers(activity);
        }

        public virtual void ParseStartEvent(Element startEventElement, ScopeImpl scope, ActivityImpl activity)
        {
            addActivityHandlers(activity);
        }

        public virtual void ParseSendTask(Element sendTaskElement, ScopeImpl scope, ActivityImpl activity)
        {
            addActivityHandlers(activity);
        }

        public virtual void ParseEndEvent(Element endEventElement, ScopeImpl scope, ActivityImpl activity)
        {
            addActivityHandlers(activity);
        }

        public virtual void ParseParallelGateway(Element parallelGwElement, ScopeImpl scope, ActivityImpl activity)
        {
            addActivityHandlers(activity);
        }

        public virtual void ParseBoundaryTimerEventDefinition(Element timerEventDefinition, bool interrupting,
            ActivityImpl timerActivity)
        {
        }

        public virtual void ParseBoundaryErrorEventDefinition(Element errorEventDefinition, bool interrupting,
            ActivityImpl activity, ActivityImpl nestedErrorEventActivity)
        {
        }

        public virtual void ParseIntermediateTimerEventDefinition(Element timerEventDefinition,
            ActivityImpl timerActivity)
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

        public virtual void ParseBoundarySignalEventDefinition(Element signalEventDefinition, bool interrupting,
            ActivityImpl signalActivity)
        {
        }

        public virtual void ParseEventBasedGateway(Element eventBasedGwElement, ScopeImpl scope, ActivityImpl activity)
        {
            addActivityHandlers(activity);
        }

        public virtual void ParseMultiInstanceLoopCharacteristics(Element activityElement,
            Element multiInstanceLoopCharacteristicsElement, ActivityImpl activity)
        {
            addActivityHandlers(activity);
        }

        public virtual void ParseIntermediateSignalCatchEventDefinition(Element signalEventDefinition,
            ActivityImpl signalActivity)
        {
        }

        public virtual void ParseTransaction(Element transactionElement, ScopeImpl scope, ActivityImpl activity)
        {
            addActivityHandlers(activity);
        }

        public virtual void ParseCompensateEventDefinition(Element compensateEventDefinition,
            ActivityImpl compensationActivity)
        {
        }

        public virtual void ParseIntermediateThrowEvent(Element intermediateEventElement, ScopeImpl scope,
            ActivityImpl activity)
        {
            addActivityHandlers(activity);
        }

        public virtual void ParseIntermediateCatchEvent(Element intermediateEventElement, ScopeImpl scope,
            ActivityImpl activity)
        {
            // do not write history for link events
            if ((string)activity.GetProperty("type")!= ActivityTypes.IntermediateEventLink)
                addActivityHandlers(activity);
        }

        public virtual void ParseBoundaryEvent(Element boundaryEventElement, ScopeImpl scopeElement,
            ActivityImpl activity)
        {
            addActivityHandlers(activity);
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


        protected internal virtual void InitExecutionListeners(IHistoryEventProducer historyEventProducer,
            IHistoryLevel historyLevel)
        {
            ProcessInstanceStartListener = new ProcessInstanceStartListener(historyEventProducer, historyLevel);
            ProcessInstanceEndListener = new ProcessInstanceEndListener(historyEventProducer, historyLevel);

            ActivityInstanceStartListener = new ActivityInstanceStartListener(historyEventProducer, historyLevel);
            ActivityInstanceEndListener = new ActivityInstanceEndListener(historyEventProducer, historyLevel);

            UserTaskAssignmentHandler = new ActivityInstanceUpdateListener(historyEventProducer, historyLevel);
            UserTaskIdHandler = UserTaskAssignmentHandler;
        }

        public virtual void ParseProcess(Element processElement, ProcessDefinitionEntity processDefinition)
        {
            if (HistoryLevel.IsHistoryEventProduced(HistoryEventTypes.ProcessInstanceEnd, null))
            {
                processDefinition.AddBuiltInListener(PvmEvent.EventNameEnd, ProcessInstanceEndListener);
            }
        }

        public virtual void ParseRootElement(Element rootElement, IList<ProcessDefinitionEntity> processDefinitions)
        {
        }

        // helper methods ///////////////////////////////////////////////////////////

        protected internal virtual void addActivityHandlers(ActivityImpl activity)
        {
            if (HistoryLevel.IsHistoryEventProduced(HistoryEventTypes.ActivityInstanceStart, null))
                activity.AddBuiltInListener(PvmEvent.EventNameStart, ActivityInstanceStartListener, 0);
            if (HistoryLevel.IsHistoryEventProduced(HistoryEventTypes.ActivityInstanceEnd, null))
                activity.AddBuiltInListener(PvmEvent.EventNameEnd, ActivityInstanceEndListener);
        }

    }
}