using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Helper;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.tree;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{

    /// <summary>
    /// </summary>
    public abstract class AbstractInstantiationCmd : AbstractProcessInstanceModificationCommand
    {
        protected internal string AncestorActivityInstanceId;

        protected internal IVariableMap variables;
        protected internal IVariableMap variablesLocal;

        public AbstractInstantiationCmd(string processInstanceId, string ancestorActivityInstanceId)
            : base(processInstanceId)
        {
            this.AncestorActivityInstanceId = ancestorActivityInstanceId;
            variables = new VariableMapImpl();
            variablesLocal = new VariableMapImpl();
        }

        public virtual IVariableMap Variables
        {
            get { return variables; }
        }

        public virtual IVariableMap VariablesLocal
        {
            get { return variablesLocal; }
        }

        protected internal abstract string TargetElementId { get; }

        public virtual void AddVariable(string name, object value)
        {
            variables.PutValue(name, value);
        }

        public virtual void AddVariableLocal(string name, object value)
        {
            variablesLocal.PutValue(name, value);
        }

        public virtual void AddVariables(IDictionary<string, ITypedValue> variables)
        {
            this.variables.PutAll(variables);
        }

        public virtual void AddVariablesLocal(IDictionary<string, ITypedValue> variables)
        {
            this.variablesLocal.PutAll(variables);
        }
        
        public override object Execute(CommandContext commandContext)
        {
            ExecutionEntity processInstance = commandContext.ExecutionManager.FindExecutionById(processInstanceId);
            
            ProcessDefinitionImpl processDefinition = processInstance.GetProcessDefinition();

            CoreModelElement elementToInstantiate = GetTargetElement(processDefinition);

            EnsureUtil.EnsureNotNull(typeof(NotValidException), DescribeFailure("Element '" + TargetElementId + "' does not exist in process '" + processDefinition.Id + "'"), "element", elementToInstantiate);

            // rebuild the mapping because the execution tree changes with every iteration
            var mapping = new ActivityExecutionTreeMapping(commandContext, processInstanceId);

            // before instantiating an activity, two things have to be determined:
            //
            // activityStack:
            // For the activity to instantiate, we build a stack of parent flow scopes
            // for which no executions exist yet and that have to be instantiated
            //
            // scopeExecution:
            // This is typically the execution under which a new sub tree has to be created.
            // if an explicit ancestor activity instance is set:
            //   - this is the scope execution for that ancestor activity instance
            //   - throws exception if that scope execution is not in the parent hierarchy
            //     of the activity to be started
            // if no explicit ancestor activity instance is set:
            //   - this is the execution of the first parent/ancestor flow scope that has an execution
            //   - throws an exception if there is more than one such execution

            var targetFlowScope = GetTargetFlowScope(processDefinition);

            // prepare to walk up the flow scope hierarchy and collect the flow scope activities
            var stackCollector = new ActivityStackCollector();
            var walker = new FlowScopeWalker(targetFlowScope);
            walker.AddPreVisitor(stackCollector);

            ExecutionEntity scopeExecution = null;

            // if no explicit ancestor activity instance is set
            if (ReferenceEquals(AncestorActivityInstanceId, null))
            {
                // walk until a scope is reached for which executions exist
                walker.WalkWhile((element) => mapping.GetExecutions(element)
                                           .Count > 0 || element == processDefinition);

                var flowScopeExecutions = mapping.GetExecutions(walker.CurrentElement);

                if (flowScopeExecutions.Count > 1)
                {
                    throw new ProcessEngineException("Ancestor activity execution is ambiguous for activity " +
                                                     targetFlowScope);
                }

                //scopeExecution = flowScopeExecutions.GetEnumerator().Next();
                scopeExecution = flowScopeExecutions.First();
            }

            var activitiesToInstantiate = stackCollector.ActivityStack;
            activitiesToInstantiate.Reverse();

            // We have to make a distinction between
            // - "regular" activities for which the activity stack can be instantiated and started
            //   right away
            // - interrupting or cancelling activities for which we have to ensure that
            //   the interruption and cancellation takes place before we instantiate the activity stack
            ActivityImpl topMostActivity = null;
            ScopeImpl flowScope = null;
            if (activitiesToInstantiate.Count > 0)
            {
                topMostActivity = (ActivityImpl)activitiesToInstantiate[0];
                flowScope = topMostActivity.FlowScope;
            }
            else if (elementToInstantiate is ActivityImpl)
            {
                topMostActivity = (ActivityImpl)elementToInstantiate;
                flowScope = topMostActivity.FlowScope;
            }
            else if (elementToInstantiate is TransitionImpl)
            {
                var transitionToInstantiate = (TransitionImpl)elementToInstantiate;
                flowScope = transitionToInstantiate.Source.FlowScope;
            }

            if (!SupportsConcurrentChildInstantiation(flowScope))
                throw new ProcessEngineException("Concurrent instantiation not possible for " + "activities in scope " +
                                                 flowScope.Id);

            var startBehavior = ActivityStartBehavior.ConcurrentInFlowScope;
            if (topMostActivity != null)
            {
                startBehavior = topMostActivity.ActivityStartBehavior;

                if (activitiesToInstantiate.Count > 0)
                {
                    // this is in BPMN relevant if there is an interrupting event sub process.
                    // we have to distinguish between instantiation of the start event and any other activity.
                    // instantiation of the start event means interrupting behavior; instantiation
                    // of any other ITask means no interruption.
                    IPvmActivity initialActivity = topMostActivity.Properties.Get(BpmnProperties.InitialActivity);
                    IPvmActivity secondTopMostActivity = null;
                    if (activitiesToInstantiate.Count > 1)
                        secondTopMostActivity = activitiesToInstantiate[1];
                    else if (elementToInstantiate.GetType().IsSubclassOf(typeof(ActivityImpl)))
                    {
                        secondTopMostActivity = (IPvmActivity)elementToInstantiate;
                    }

                    if (initialActivity != secondTopMostActivity)
                        startBehavior = ActivityStartBehavior.ConcurrentInFlowScope;
                }
            }
            //throw new Exception("startBehavior值："+(int)startBehavior);//2
            switch (startBehavior)
            {
                case ActivityStartBehavior.CancelEventScope:
                    {
                        ScopeImpl scopeToCancel = (ScopeImpl) topMostActivity.EventScope;
                        var executionToCancel = GetSingleExecutionForScope(mapping, scopeToCancel);
                        if (executionToCancel != null)
                        {
                            executionToCancel.DeleteCascade("Cancelling activity " + topMostActivity + " executed.",
                                skipCustomListeners, skipIoMappings);
                            Instantiate(executionToCancel.Parent, activitiesToInstantiate, elementToInstantiate);
                        }
                        else
                        {
                            var flowScopeExecution = GetSingleExecutionForScope(mapping, topMostActivity.FlowScope);
                            InstantiateConcurrent(flowScopeExecution, activitiesToInstantiate, elementToInstantiate);
                        }
                        break;
                    }
                case ActivityStartBehavior.InterruptEventScope:
                    {
                        ScopeImpl scopeToCancel = (ScopeImpl) topMostActivity.EventScope;
                        var executionToCancel = GetSingleExecutionForScope(mapping, scopeToCancel);
                        executionToCancel.Interrupt("Interrupting activity " + topMostActivity + " executed.",
                            skipCustomListeners, skipIoMappings);
                        executionToCancel.SetActivity(null);
                        executionToCancel.LeaveActivityInstance();
                        Instantiate(executionToCancel, activitiesToInstantiate, elementToInstantiate);
                        break;
                    }
                case ActivityStartBehavior.InterruptFlowScope:
                    {
                        var scopeToCancel = topMostActivity.FlowScope;
                        var executionToCancel = GetSingleExecutionForScope(mapping, scopeToCancel);
                        executionToCancel.Interrupt("Interrupting activity " + topMostActivity + " executed.",
                            skipCustomListeners, skipIoMappings);
                        executionToCancel.SetActivity(null);
                        executionToCancel.LeaveActivityInstance();
                        Instantiate(executionToCancel, activitiesToInstantiate, elementToInstantiate);
                        break;
                    }
                default:
                    {
                        // if all child executions have been cancelled
                        // or this execution has ended executing its scope, it can be reused
                        if (!scopeExecution.HasChildren() && (scopeExecution.GetActivity() == null || scopeExecution.IsEnded))
                        {
                            // reuse the scope execution
                            Instantiate(scopeExecution, activitiesToInstantiate, elementToInstantiate);
                        }
                        else
                        {
                            // if the activity is not cancelling/interrupting, it can simply be instantiated as
                            // a concurrent child of the scopeExecution
                            InstantiateConcurrent(scopeExecution, activitiesToInstantiate, elementToInstantiate);
                        }
                        break;
                    }
            }

            return null;
        }

        /// <summary>
        ///     Cannot create more than inner instance in a sequential MI construct
        /// </summary>
        protected internal virtual bool SupportsConcurrentChildInstantiation(ScopeImpl flowScope)
        {
            if (flowScope == null) return true;
            IActivityBehavior behavior = flowScope.ActivityBehavior;
            return behavior == null || !(behavior is SequentialMultiInstanceActivityBehavior);
        }

        protected internal virtual ExecutionEntity GetSingleExecutionForScope(ActivityExecutionTreeMapping mapping,
            ScopeImpl scope)
        {
            var executions = mapping.GetExecutions(scope);

            if (executions.Count > 0)
                if (executions.Count > 1)
                    throw new ProcessEngineException("Executions for activity " + scope + " ambiguous");
            return null;
        }

        protected internal virtual bool IsConcurrentStart(ActivityStartBehavior startBehavior)
        {
            return (startBehavior == ActivityStartBehavior.Default) ||
                   (startBehavior == ActivityStartBehavior.ConcurrentInFlowScope);
        }

        protected internal virtual void Instantiate(IActivityExecution ancestorScopeExecution,
            IList<IPvmActivity> parentFlowScopes, CoreModelElement targetElement)
        {
            if (targetElement is IPvmTransition)
            {
                ancestorScopeExecution.ExecuteActivities(parentFlowScopes, null, (IPvmTransition)targetElement,
                    variables, variablesLocal, skipCustomListeners, skipIoMappings);
            }
            else if (targetElement is IPvmActivity)
            {
                ancestorScopeExecution.ExecuteActivities(parentFlowScopes, (IPvmActivity)targetElement, null, variables,
                    variablesLocal, skipCustomListeners, skipIoMappings);
            }
            else
            {
                throw new ProcessEngineException("Cannot instantiate element " + targetElement);
            }
        }


        protected internal virtual void InstantiateConcurrent(ExecutionEntity ancestorScopeExecution,
            IList<IPvmActivity> parentFlowScopes, CoreModelElement targetElement)
        {
            if (targetElement is IPvmTransition)
            {
                ancestorScopeExecution.ExecuteActivitiesConcurrent(parentFlowScopes, null, (IPvmTransition)targetElement,
                    variables, variablesLocal, skipCustomListeners, skipIoMappings);
            }
            else if (targetElement is IPvmActivity)
            {
                ancestorScopeExecution.ExecuteActivitiesConcurrent(parentFlowScopes, (IPvmActivity)targetElement, null,
                    variables, variablesLocal, skipCustomListeners, skipIoMappings);
            }
            else
            {
                throw new ProcessEngineException("Cannot instantiate element " + targetElement);
            }
        }

        protected internal abstract ScopeImpl GetTargetFlowScope(ProcessDefinitionImpl processDefinition);

        protected internal abstract CoreModelElement GetTargetElement(ProcessDefinitionImpl processDefinition);
        
    }
}