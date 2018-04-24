using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{

    /// <summary>
    ///     
    /// </summary>
    public abstract class AbstractProcessInstanceModificationCommand : ICommand<object>
    {
        protected internal string processInstanceId;
        protected internal bool skipCustomListeners;
        protected internal bool skipIoMappings;

        public AbstractProcessInstanceModificationCommand(string processInstanceId)
        {
            this.processInstanceId = processInstanceId;
        }

        public virtual bool SkipCustomListeners
        {
            set { skipCustomListeners = value; }
        }

        public virtual bool SkipIoMappings
        {
            set { skipIoMappings = value; }
        }

        public virtual string ProcessInstanceId
        {
            set { processInstanceId = value; }
        }

        public abstract object Execute(CommandContext commandContext);

        protected internal virtual IActivityInstance FindActivityInstance(IActivityInstance tree,
            string activityInstanceId)
        {
            if (activityInstanceId.Equals(tree.Id))
                return tree;
            foreach (var child in tree.ChildActivityInstances)
            {
                var matchingChildInstance = FindActivityInstance(child, activityInstanceId);
                if (matchingChildInstance != null)
                    return matchingChildInstance;
            }

            return null;
        }

        protected internal virtual ITransitionInstance FindTransitionInstance(IActivityInstance tree,
            string transitionInstanceId)
        {
            foreach (var childTransitionInstance in tree.ChildTransitionInstances)
                if (MatchesRequestedTransitionInstance(childTransitionInstance, transitionInstanceId))
                    return childTransitionInstance;

            foreach (var child in tree.ChildActivityInstances)
            {
                var matchingChildInstance = FindTransitionInstance(child, transitionInstanceId);
                if (matchingChildInstance != null)
                    return matchingChildInstance;
            }

            return null;
        }

        protected internal virtual bool MatchesRequestedTransitionInstance(ITransitionInstance instance,
            string queryInstanceId)
        {
            var match = instance.Id.Equals(queryInstanceId);

            // check if the execution queried for has been replaced by the given instance
            // => if yes, given instance is matched
            // this is a fix for CAM-4090 to tolerate inconsistent transition instance ids as described in CAM-4143
            if (!match)
            {
                // note: execution id = transition instance id
                //ExecutionEntity cachedExecution = Context.CommandContext.DbEntityManager.getCachedEntity<ExecutionEntity>(typeof(ExecutionEntity), queryInstanceId);

                // follow the links of execution replacement;
                // note: this can be at most two hops:
                // case 1:
                //   the query execution is the scope execution
                //     => tree may have expanded meanwhile
                //     => scope execution references replacing execution directly (one hop)
                //
                // case 2:
                //   the query execution is a concurrent execution
                //     => tree may have compacted meanwhile
                //     => concurrent execution references scope execution directly (one hop)
                //
                // case 3:
                //   the query execution is a concurrent execution
                //     => tree may have compacted/expanded/compacted/../expanded any number of times
                //     => the concurrent execution has been removed and therefore references the scope execution (first hop)
                //     => the scope execution may have been replaced itself again with another concurrent execution (second hop)
                //   note that the scope execution may have a long "history" of replacements, but only the last replacement is relevant here
                // if (cachedExecution != null)
                // {
                //ExecutionEntity replacingExecution = cachedExecution.resolveReplacedBy();

                //if (replacingExecution != null)
                //{
                //  match = replacingExecution.Id.Equals(instance.Id);
                //}
                // }
            }

            return match;
        }

        protected internal virtual ScopeImpl GetScopeForActivityInstance(ProcessDefinitionImpl processDefinition,
            IActivityInstance activityInstance)
        {
            var scopeId = activityInstance.ActivityId;

            if (processDefinition.Id.Equals(scopeId))
                return processDefinition;
            return (ScopeImpl)processDefinition.FindActivity(scopeId);
        }

        protected internal virtual ExecutionEntity GetScopeExecutionForActivityInstance(ExecutionEntity processInstance,
            ActivityExecutionTreeMapping mapping, IActivityInstance activityInstance)
        {
            EnsureUtil.EnsureNotNull("activityInstance", activityInstance);

            ProcessDefinitionImpl processDefinition = processInstance.ProcessDefinition;
            ScopeImpl scope = GetScopeForActivityInstance(processDefinition, activityInstance);

            ISet<ExecutionEntity> executions = mapping.GetExecutions(scope);
            ISet<string> activityInstanceExecutions = new HashSet<string>(activityInstance.ExecutionIds);

            // TODO: this is a hack around the activity instance tree
            // remove with fix of CAM-3574
            foreach (var activityInstanceExecutionId in activityInstance.ExecutionIds)
            {
                ExecutionEntity execution = context.Impl.Context.CommandContext.ExecutionManager.FindExecutionById(activityInstanceExecutionId);
                if (execution.IsConcurrent && execution.HasChildren())
                {
                    // concurrent executions have at most one child
                    IActivityExecution child = execution.executions.First();//.Executions[0];
                    activityInstanceExecutions.Add(child.Id);
                }
            }

            // find the scope execution for the given activity instance
            ISet<ExecutionEntity> retainedExecutionsForInstance = new HashSet<ExecutionEntity>();
            foreach (ExecutionEntity execution in executions)
            {
                if (activityInstanceExecutions.Contains(execution.Id))
                {
                    retainedExecutionsForInstance.Add(execution);
                }
            }

            if (retainedExecutionsForInstance.Count != 1)
                throw new ProcessEngineException("There are " + retainedExecutionsForInstance.Count +
                                                 " (!= 1) executions for activity instance " + activityInstance.Id);

            return retainedExecutionsForInstance.GetEnumerator().Current;
        }

        protected internal virtual string DescribeFailure(string detailMessage)
        {
            return "Cannot perform instruction: " + Describe() + "; " + detailMessage;
        }

        protected internal abstract string Describe();

        public override string ToString()
        {
            return Describe();
        }
    }
}