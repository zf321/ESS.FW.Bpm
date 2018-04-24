using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.task;
using ESS.FW.Bpm.Engine.Impl.Util.xml;
using ESS.FW.Bpm.Engine.Impl.Variable;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Variable;

namespace ESS.FW.Bpm.Engine.Application.Impl.Event
{
    /// <summary>
    ///     
    /// </summary>
    public class ProcessApplicationEventParseListener : IBpmnParseListener
    {
        public static readonly IDelegateListener<IBaseDelegateExecution> ExecutionListener = new ProcessApplicationEventListenerDelegate();
        public static readonly ITaskListener TaskListener = new ProcessApplicationEventListenerDelegate();

        public virtual void ParseStartEvent(Element startEventElement, ScopeImpl scope, ActivityImpl startEventActivity)
        {
            AddStartEventListener(startEventActivity);
            AddEndEventListener(startEventActivity);
        }

        public virtual void ParseExclusiveGateway(Element exclusiveGwElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddStartEventListener(activity);
            AddEndEventListener(activity);
        }

        public virtual void ParseInclusiveGateway(Element inclusiveGwElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddStartEventListener(activity);
            AddEndEventListener(activity);
        }

        public virtual void ParseParallelGateway(Element parallelGwElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddStartEventListener(activity);
            AddEndEventListener(activity);
        }

        public virtual void ParseScriptTask(Element scriptTaskElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddStartEventListener(activity);
            AddEndEventListener(activity);
        }

        public virtual void ParseServiceTask(Element serviceTaskElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddStartEventListener(activity);
            AddEndEventListener(activity);
        }

        public virtual void ParseBusinessRuleTask(Element businessRuleTaskElement, ScopeImpl scope,
            ActivityImpl activity)
        {
            AddStartEventListener(activity);
            AddEndEventListener(activity);
        }

        public virtual void ParseTask(Element taskElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddStartEventListener(activity);
            AddEndEventListener(activity);
        }

        public virtual void ParseManualTask(Element manualTaskElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddStartEventListener(activity);
            AddEndEventListener(activity);
        }

        public virtual void ParseUserTask(Element userTaskElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddStartEventListener(activity);
            AddEndEventListener(activity);
            var activityBehavior =  activity.ActivityBehavior as UserTaskActivityBehavior;
            var taskDefinition = activityBehavior.TaskDefinition;
            AddTaskCreateListeners(taskDefinition);
            AddTaskAssignmentListeners(taskDefinition);
            AddTaskCompleteListeners(taskDefinition);
            AddTaskDeleteListeners(taskDefinition);
        }

        public virtual void ParseEndEvent(Element endEventElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddStartEventListener(activity);
            AddEndEventListener(activity);
        }

        public virtual void ParseBoundaryTimerEventDefinition(Element timerEventDefinition, bool interrupting,
            ActivityImpl timerActivity)
        {
            // start and end event listener are set by parseBoundaryEvent()
        }

        public virtual void ParseBoundaryErrorEventDefinition(Element errorEventDefinition, bool interrupting,
            ActivityImpl activity, ActivityImpl nestedErrorEventActivity)
        {
            // start and end event listener are set by parseBoundaryEvent()
        }

        public virtual void ParseSubProcess(Element subProcessElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddStartEventListener(activity);
            AddEndEventListener(activity);
        }

        public virtual void ParseCallActivity(Element callActivityElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddStartEventListener(activity);
            AddEndEventListener(activity);
        }

        public virtual void ParseProperty(Element propertyElement, VariableDeclaration variableDeclaration,
            ActivityImpl activity)
        {
        }

        public virtual void ParseSequenceFlow(Element sequenceFlowElement, ScopeImpl scopeElement,
            TransitionImpl transition)
        {
            AddTakeEventListener(transition);
        }

        public virtual void ParseSendTask(Element sendTaskElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddStartEventListener(activity);
            AddEndEventListener(activity);
        }

        public virtual void ParseMultiInstanceLoopCharacteristics(Element activityElement,
            Element multiInstanceLoopCharacteristicsElement, ActivityImpl activity)
        {
            AddStartEventListener(activity);
            AddEndEventListener(activity);
        }

        public virtual void ParseIntermediateTimerEventDefinition(Element timerEventDefinition,
            ActivityImpl timerActivity)
        {
            // start and end event listener are set by parseIntermediateCatchEvent()
        }

        public virtual void ParseReceiveTask(Element receiveTaskElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddStartEventListener(activity);
            AddEndEventListener(activity);
        }

        public virtual void ParseIntermediateSignalCatchEventDefinition(Element signalEventDefinition,
            ActivityImpl signalActivity)
        {
            // start and end event listener are set by parseIntermediateCatchEvent()
        }

        public virtual void ParseBoundarySignalEventDefinition(Element signalEventDefinition, bool interrupting,
            ActivityImpl signalActivity)
        {
            // start and end event listener are set by parseBoundaryEvent()
        }

        public virtual void ParseEventBasedGateway(Element eventBasedGwElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddStartEventListener(activity);
            AddEndEventListener(activity);
        }

        public virtual void ParseTransaction(Element transactionElement, ScopeImpl scope, ActivityImpl activity)
        {
            AddStartEventListener(activity);
            AddEndEventListener(activity);
        }

        public virtual void ParseCompensateEventDefinition(Element compensateEventDefinition,
            ActivityImpl compensationActivity)
        {
        }

        public virtual void ParseIntermediateThrowEvent(Element intermediateEventElement, ScopeImpl scope,
            ActivityImpl activity)
        {
            AddStartEventListener(activity);
            AddEndEventListener(activity);
        }

        public virtual void ParseIntermediateCatchEvent(Element intermediateEventElement, ScopeImpl scope,
            ActivityImpl activity)
        {
            AddStartEventListener(activity);
            AddEndEventListener(activity);
        }

        public virtual void ParseBoundaryEvent(Element boundaryEventElement, ScopeImpl scopeElement,
            ActivityImpl activity)
        {
            AddStartEventListener(activity);
            AddEndEventListener(activity);
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



        protected internal virtual void AddEndEventListener(ScopeImpl activity)
        {
            activity.AddExecutionListener(ExecutionListenerFields.EventNameEnd, ExecutionListener);
        }

        protected internal virtual void AddStartEventListener(ScopeImpl activity)
        {
            activity.AddExecutionListener(ExecutionListenerFields.EventNameStart, ExecutionListener);
        }

        protected internal virtual void AddTakeEventListener(TransitionImpl transition)
        {
            transition.AddExecutionListener(ExecutionListener);
        }

        protected internal virtual void AddTaskAssignmentListeners(TaskDefinition taskDefinition)
        {
            taskDefinition.AddTaskListener(TaskListenerFields.EventnameAssignment, TaskListener);
        }

        protected internal virtual void AddTaskCreateListeners(TaskDefinition taskDefinition)
        {
            taskDefinition.AddTaskListener(TaskListenerFields.EventnameCreate, TaskListener);
        }

        protected internal virtual void AddTaskCompleteListeners(TaskDefinition taskDefinition)
        {
            taskDefinition.AddTaskListener(TaskListenerFields.EventnameComplete, TaskListener);
        }

        protected internal virtual void AddTaskDeleteListeners(TaskDefinition taskDefinition)
        {
            taskDefinition.AddTaskListener(TaskListenerFields.EventnameDelete, TaskListener);
        }

        // BpmnParseListener implementation /////////////////////////////////////////////////////////

        public virtual void ParseProcess(Element processElement, ProcessDefinitionEntity processDefinition)
        {
            AddStartEventListener((ScopeImpl)processDefinition);
            AddEndEventListener((ScopeImpl)processDefinition);
        }

        public virtual void ParseRootElement(Element rootElement, IList<ProcessDefinitionEntity> processDefinitions)
        {
        }
    }
}