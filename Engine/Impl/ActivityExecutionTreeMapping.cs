using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace ESS.FW.Bpm.Engine.Impl
{

    /// <summary>
    ///     Maps an activity (plain activities + their containing flow scopes) to the scope executions
    ///     that are executing them. For every instance of a scope, there is one such execution.
    ///     
    /// </summary>
    public class ActivityExecutionTreeMapping
    {
        protected internal IDictionary<ScopeImpl, ISet<ExecutionEntity>> ActivityExecutionMapping;
        protected internal CommandContext CommandContext;
        protected internal ProcessDefinitionImpl ProcessDefinition;
        protected internal string ProcessInstanceId;

        public ActivityExecutionTreeMapping(CommandContext commandContext, string processInstanceId)
        {
            ActivityExecutionMapping = new Dictionary<ScopeImpl, ISet<ExecutionEntity>>();
            this.CommandContext = commandContext;
            this.ProcessInstanceId = processInstanceId;

            Initialize();
        }

        protected internal virtual void SubmitExecution(ExecutionEntity execution, ScopeImpl scope)
        {
            GetExecutions(scope).Add(execution);
        }

        public virtual ISet<ExecutionEntity> GetExecutions(ScopeImpl activity)
        {
            var executionsForActivity = ActivityExecutionMapping.ContainsKey(activity) ? ActivityExecutionMapping[activity]:null;
            if (executionsForActivity == null)
            {
                executionsForActivity = new HashSet<ExecutionEntity>();
                ActivityExecutionMapping[activity] = executionsForActivity;
            }

            return executionsForActivity;
        }

        public virtual ExecutionEntity GetExecution(IActivityInstance activityInstance)
        {
            ScopeImpl scope = null;

            if (activityInstance.Id.Equals(activityInstance.ProcessInstanceId))
                scope = ProcessDefinition;
            else
                scope = (ScopeImpl) ProcessDefinition.FindActivity(activityInstance.ActivityId);

            return Intersect(GetExecutions(scope), activityInstance.ExecutionIds);
        }

        protected internal virtual ExecutionEntity Intersect(ISet<ExecutionEntity> executions, string[] executionIds)
        {
            ISet<string> executionIdSet = new HashSet<string>();
            foreach (var executionId in executionIds)
                executionIdSet.Add(executionId);

            foreach (var execution in executions)
            {
                if (executionIdSet.Contains(execution.Id))
                {
                    return execution;
                }
            }
            throw new ProcessEngineException("Could not determine execution");
        }

        protected internal virtual void Initialize()
        {
            ExecutionEntity processInstance = CommandContext.ExecutionManager.FindExecutionById(ProcessInstanceId);
            this.ProcessDefinition = processInstance.GetProcessDefinition();

            IList<IActivityExecution> executions = FetchExecutionsForProcessInstance(processInstance);
            executions.Add(processInstance);

            IList<IActivityExecution> leaves = FindLeaves(executions);

            AssignExecutionsToActivities(leaves);
        }

        protected internal virtual void AssignExecutionsToActivities(IList<IActivityExecution> leaves)
        {
            foreach (ExecutionEntity leaf in leaves)
            {
                //throw new NotImplementedException();
                ScopeImpl activity = leaf.GetActivity() as ScopeImpl;

                if (activity != null)
                {
                    if (!string.ReferenceEquals(leaf.ActivityInstanceId, null))
                    {
                        EnsureUtil.EnsureNotNull("activity", activity);
                        SubmitExecution(leaf, activity);
                    }
                    MergeScopeExecutions(leaf);


                }
                else if (leaf.IsProcessInstanceExecution)
                {
                    SubmitExecution(leaf, leaf.GetProcessDefinition());
                }
            }
        }

        protected internal virtual void MergeScopeExecutions(ExecutionEntity leaf)
        {
            IDictionary<ScopeImpl, PvmExecutionImpl> mapping = leaf.CreateActivityExecutionMapping();

            foreach (KeyValuePair<ScopeImpl, PvmExecutionImpl> mappingEntry in mapping)
            {
                ScopeImpl scope = mappingEntry.Key;
                ExecutionEntity scopeExecution = (ExecutionEntity)mappingEntry.Value;

                SubmitExecution(scopeExecution, scope);
            }
        }

        protected internal virtual IList<IActivityExecution> FetchExecutionsForProcessInstance(IActivityExecution execution)
        {
            List<IActivityExecution> executions = new List<IActivityExecution>();
            executions.AddRange(execution.GetExecutionsEntity());
            foreach (IActivityExecution  child in execution.Executions)
            {
                executions.AddRange(FetchExecutionsForProcessInstance(child));
            }

            return executions;
        }

        protected internal virtual IList<IActivityExecution> FindLeaves(IList<IActivityExecution> executions)
        {
            IList<IActivityExecution> leaves = new List<IActivityExecution>();

            foreach (var execution in executions)
                if (IsLeaf(execution))
                    leaves.Add(execution);

            return leaves;
        }

        /// <summary>
        ///     event-scope executions are not considered in this mapping and must be ignored
        /// </summary>
        protected internal virtual bool IsLeaf(IActivityExecution execution)
        {
            if (CompensationBehavior.IsCompensationThrowing((PvmExecutionImpl)execution))
            {
                return true;
            }
            return !execution.IsEventScope && execution.NonEventScopeExecutions.Count == 0;
        }
    }
}