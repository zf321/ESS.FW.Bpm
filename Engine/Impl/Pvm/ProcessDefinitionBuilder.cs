using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.Pvm
{
    /// <summary>
    ///      
    /// </summary>
    public class ProcessDefinitionBuilder
    {
        private readonly bool _instanceFieldsInitialized;


        protected internal ProcessDefinitionImpl ProcessDefinition;
        protected internal CoreModelElement ProcessElement;
        protected internal Stack<ScopeImpl> ScopeStack = new Stack<ScopeImpl>();
        protected internal TransitionImpl transition;
        protected internal IList<object[]> UnresolvedTransitions = new List<object[]>();

        public ProcessDefinitionBuilder() : this(null)
        {
            if (!_instanceFieldsInitialized)
            {
                InitializeInstanceFields();
                _instanceFieldsInitialized = true;
            }
        }

        public ProcessDefinitionBuilder(string processDefinitionId)
        {
            if (!_instanceFieldsInitialized)
            {
                InitializeInstanceFields();
                _instanceFieldsInitialized = true;
            }
            ProcessDefinition = new ProcessDefinitionImpl(processDefinitionId);
            ScopeStack.Push(ProcessDefinition);
        }

        protected internal virtual ActivityImpl Activity
        {
            get { return (ActivityImpl) ScopeStack.Peek(); }
        }

        private void InitializeInstanceFields()
        {
            ProcessElement = ProcessDefinition;
        }

        public virtual ProcessDefinitionBuilder CreateActivity(string id)
        {
            var activity = (ActivityImpl) ScopeStack.Peek().CreateActivity(id);
            ScopeStack.Push(activity);
            ProcessElement = activity;

            transition = null;

            return this;
        }

        public virtual ProcessDefinitionBuilder AttachedTo(string id, bool isInterrupting)
        {
            var activity = Activity;
            activity.EventScope = (IPvmScope) ProcessDefinition.FindActivity(id);

            if (isInterrupting)
                activity.ActivityStartBehavior = ActivityStartBehavior.InterruptEventScope;
            else
                activity.ActivityStartBehavior = ActivityStartBehavior.ConcurrentInFlowScope;

            return this;
        }


        public virtual ProcessDefinitionBuilder EndActivity()
        {
            ScopeStack.Pop();
            ProcessElement = ScopeStack.Peek();

            transition = null;

            return this;
        }

        public virtual ProcessDefinitionBuilder Initial()
        {
            ProcessDefinition.Initial = Activity;
            return this;
        }

        public virtual ProcessDefinitionBuilder StartTransition(string destinationActivityId)
        {
            return StartTransition(destinationActivityId, null);
        }

        public virtual ProcessDefinitionBuilder StartTransition(string destinationActivityId, string transitionId)
        {
            if (ReferenceEquals(destinationActivityId, null))
                throw new PvmException("destinationActivityId is null");
            var activity = Activity;
            transition = activity.CreateOutgoingTransition(transitionId);
            UnresolvedTransitions.Add(new object[] {transition, destinationActivityId});
            ProcessElement = transition;
            return this;
        }

        public virtual ProcessDefinitionBuilder EndTransition()
        {
            ProcessElement = ScopeStack.Peek();
            transition = null;
            return this;
        }

        public virtual ProcessDefinitionBuilder Transition(string destinationActivityId)
        {
            return Transition(destinationActivityId, null);
        }

        public virtual ProcessDefinitionBuilder Transition(string destinationActivityId, string transitionId)
        {
            StartTransition(destinationActivityId, transitionId);
            EndTransition();
            return this;
        }

        public virtual ProcessDefinitionBuilder Behavior(IActivityBehavior activityBehaviour)
        {
            Activity.activityBehavior = activityBehaviour;
            return this;
        }

        public virtual ProcessDefinitionBuilder Property(string name, object value)
        {
            ProcessElement.SetProperty(name, value);
            return this;
        }

        public virtual IPvmProcessDefinition BuildProcessDefinition()
        {
            foreach (var unresolvedTransition in UnresolvedTransitions)
            {
                var transition = (TransitionImpl) unresolvedTransition[0];
                var destinationActivityName = (string) unresolvedTransition[1];
                var destination = (ActivityImpl) ProcessDefinition.FindActivity(destinationActivityName);
                if (destination == null)
                    throw new System.Exception("destination '" + destinationActivityName +
                                        "' not found.  (referenced from transition in '" + transition.Source.Id + "')");
                transition.SetDestination(destination);
            }
            return ProcessDefinition;
        }

        public virtual ProcessDefinitionBuilder Scope()
        {
            Activity.IsScope = true;
            return this;
        }

        public virtual ProcessDefinitionBuilder ExecutionListener(IDelegateListener<IBaseDelegateExecution> executionListener)
        {
            if (transition != null)
                transition.AddExecutionListener(executionListener);
            else
                throw new PvmException("not in a transition scope");
            return this;
        }

        public virtual ProcessDefinitionBuilder ExecutionListener(string eventName, IDelegateListener<IBaseDelegateExecution> executionListener)
        {
            if (transition == null)
                ScopeStack.Peek().AddExecutionListener(eventName, executionListener);
            else
                transition.AddExecutionListener(executionListener);
            return this;
        }
    }
}