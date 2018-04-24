using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Variable;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    
    public class GetActivityInstanceCmd : ICommand<IActivityInstance>
    {
        protected internal string ProcessInstanceId;

        public GetActivityInstanceCmd(string processInstanceId)
        {
            this.ProcessInstanceId = processInstanceId;
        }

        public virtual IActivityInstance Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("processInstanceId", ProcessInstanceId);
            IList<ExecutionEntity> executionList = LoadProcessInstance(ProcessInstanceId, commandContext);

            if (executionList.Count == 0)
            {
                return null;
            }

            CheckGetActivityInstance(ProcessInstanceId, commandContext);

            IList<ExecutionEntity> nonEventScopeExecutions = FilterNonEventScopeExecutions(executionList);
            IList<ExecutionEntity> leaves = FilterLeaves(nonEventScopeExecutions);//.OrderBy(m => m.Id).ToList();
            // Leaves must be ordered in a predictable way (e.g. by ID)
            // in order to return a stable execution tree with every repeated invocation of this command.
            // For legacy process instances, there may miss scope executions for activities that are now a scope.
            // In this situation, there may be multiple scope candidates for the same instance id; which one
            // can depend on the order the leaves are iterated.
            OrderById(leaves);

            ExecutionEntity processInstance = FilterProcessInstance(executionList);

            if (processInstance.IsEnded)
                return null;

            //create act instance for process instance

            ActivityInstanceImpl processActInst = CreateActivityInstance(processInstance, processInstance.ProcessDefinition, ProcessInstanceId, null);
            IDictionary<string, ActivityInstanceImpl> activityInstances = new Dictionary<string, ActivityInstanceImpl>();
            activityInstances[ProcessInstanceId] = processActInst;

            IDictionary<string, TransitionInstanceImpl> transitionInstances = new Dictionary<string, TransitionInstanceImpl>();

            foreach (ExecutionEntity leaf in leaves)
            {
                // skip leafs without activity, e.g. if only the process instance exists after cancellation
                // it will not have an activity set
                if (leaf.Activity == null)
                {
                    continue;
                }

                IDictionary<ScopeImpl, PvmExecutionImpl> activityExecutionMapping = leaf.CreateActivityExecutionMapping();
                IDictionary<ScopeImpl, PvmExecutionImpl> scopeInstancesToCreate = new Dictionary<ScopeImpl, PvmExecutionImpl>(activityExecutionMapping);

                // create an activity/transition instance for each leaf that executes a non-scope activity
                // and does not throw compensation
                if (leaf.ActivityInstanceId != null)
                {

                    if (!CompensationBehavior.IsCompensationThrowing(leaf) || LegacyBehavior.IsCompensationThrowing(leaf, activityExecutionMapping))
                    {
                        string parentActivityInstanceId = null;

                        parentActivityInstanceId = activityExecutionMapping[leaf.Activity.FlowScope].ParentActivityInstanceId;

                        ActivityInstanceImpl leafInstance = CreateActivityInstance(leaf, (ScopeImpl)leaf.Activity, leaf.ActivityInstanceId, parentActivityInstanceId);
                        activityInstances[leafInstance.Id] = leafInstance;

                        scopeInstancesToCreate.Remove((ScopeImpl)leaf.Activity);
                    }
                }
                else
                {
                    TransitionInstanceImpl transitionInstance = CreateTransitionInstance(leaf);
                    transitionInstances[transitionInstance.Id] = transitionInstance;

                    scopeInstancesToCreate.Remove((ScopeImpl)leaf.Activity);
                }

                LegacyBehavior.RemoveLegacyNonScopesFromMapping(scopeInstancesToCreate);
                scopeInstancesToCreate.Remove(leaf.ProcessDefinition);

                // create an activity instance for each scope (including compensation throwing executions)
                foreach (KeyValuePair<ScopeImpl, PvmExecutionImpl> scopeExecutionEntry in scopeInstancesToCreate)
                {
                    ScopeImpl scope = scopeExecutionEntry.Key;
                    PvmExecutionImpl scopeExecution = scopeExecutionEntry.Value;

                    string activityInstanceId = null;
                    string parentActivityInstanceId = null;

                    activityInstanceId = scopeExecution.ParentActivityInstanceId;
                    parentActivityInstanceId = activityExecutionMapping[scope.FlowScope].ParentActivityInstanceId;

                    if (activityInstances.ContainsKey(activityInstanceId))
                    {
                        continue;
                    }
                    else
                    {
                        // regardless of the tree structure (compacted or not), the scope's activity instance id
                        // is the activity instance id of the parent execution and the parent activity instance id
                        // of that is the actual parent activity instance id
                        ActivityInstanceImpl scopeInstance = CreateActivityInstance(scopeExecution, scope, activityInstanceId, parentActivityInstanceId);
                        activityInstances[activityInstanceId] = scopeInstance;
                    }
                }
            }

            LegacyBehavior.RepairParentRelationships(activityInstances.Values, ProcessInstanceId);
            PopulateChildInstances(activityInstances, transitionInstances);

            return processActInst;
        }

        protected internal virtual void CheckGetActivityInstance(string processInstanceId, CommandContext commandContext)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckReadProcessInstance(processInstanceId);
        }

        protected internal virtual void OrderById(IList<ExecutionEntity> leaves)
        {
            Array.Sort(leaves.ToArray(), ExecutionIdComparator.Instance);
        }

        protected internal virtual ActivityInstanceImpl CreateActivityInstance(PvmExecutionImpl scopeExecution, ScopeImpl scope, string activityInstanceId, string parentActivityInstanceId)
        {
            ActivityInstanceImpl actInst = new ActivityInstanceImpl();

            actInst.Id = activityInstanceId;
            actInst.ParentActivityInstanceId = parentActivityInstanceId;
            actInst.ProcessInstanceId = scopeExecution.ProcessInstanceId;
            actInst.ProcessDefinitionId = scopeExecution.ProcessDefinitionId;
            actInst.BusinessKey = scopeExecution.BusinessKey;
            actInst.ActivityId = scope.Id;

            string name = scope.Name;
            if (string.IsNullOrEmpty(name))
            {
                name = (string)scope.GetProperty("name");
            }
            actInst.ActivityName = name;

            if (scope.Id.Equals(scopeExecution.ProcessDefinition.Id))
            {
                actInst.ActivityType = "processDefinition";
            }
            else
            {
                actInst.ActivityType = (string)scope.GetProperty("type");
            }

            IList<string> executionIds = new List<string>();
            executionIds.Add(scopeExecution.Id);

            foreach (PvmExecutionImpl childExecution in scopeExecution.NonEventScopeExecutions)
            {
                // add all concurrent children that are not in an activity
                if (childExecution.IsConcurrent && string.IsNullOrEmpty(childExecution.ActivityId))
                {
                    executionIds.Add(childExecution.Id);
                }
            }
            actInst.ExecutionIds = executionIds.ToArray();

            return actInst;
        }

        protected internal virtual TransitionInstanceImpl CreateTransitionInstance(PvmExecutionImpl execution)
        {
            TransitionInstanceImpl transitionInstance = new TransitionInstanceImpl();

            // can use execution id as persistent ID for transition as an execution
            // can execute as most one transition at a time.
            transitionInstance.Id = execution.Id;
            transitionInstance.ParentActivityInstanceId = execution.ParentActivityInstanceId;
            transitionInstance.ProcessInstanceId = execution.ProcessInstanceId;
            transitionInstance.ProcessDefinitionId = execution.ProcessDefinitionId;
            transitionInstance.ExecutionId = execution.Id;
            transitionInstance.ActivityId = execution.ActivityId;

            ActivityImpl activity = (ActivityImpl)execution.Activity;
            if (activity != null)
            {
                string name = activity.Name;
                if (string.IsNullOrEmpty(name))
                {
                    name = (string)activity.GetProperty("name");
                }
                transitionInstance.ActivityName = name;
                transitionInstance.ActivityType = (string)activity.GetProperty("type");
            }

            return transitionInstance;
        }

        protected internal virtual void PopulateChildInstances(IDictionary<string, ActivityInstanceImpl> activityInstances, IDictionary<string, TransitionInstanceImpl> transitionInstances)
        {
            IDictionary<ActivityInstanceImpl, IList<ActivityInstanceImpl>> childActivityInstances = new Dictionary<ActivityInstanceImpl, IList<ActivityInstanceImpl>>();
            IDictionary<ActivityInstanceImpl, IList<TransitionInstanceImpl>> childTransitionInstances = new Dictionary<ActivityInstanceImpl, IList<TransitionInstanceImpl>>();

            foreach (ActivityInstanceImpl instance in activityInstances.Values)
            {
                if (!string.IsNullOrEmpty(instance.ParentActivityInstanceId))
                {
                    ActivityInstanceImpl parentInstance = activityInstances.GetValueOrNull(instance.ParentActivityInstanceId);
                    if (parentInstance == null)
                    {
                        throw new ProcessEngineException("No parent activity instance with id " + instance.ParentActivityInstanceId + " generated");
                    }
                    PutListElement(childActivityInstances, parentInstance, instance);
                }
            }

            foreach (TransitionInstanceImpl instance in transitionInstances.Values)
            {
                if (!string.IsNullOrEmpty(instance.ParentActivityInstanceId))
                {
                    ActivityInstanceImpl parentInstance = activityInstances.GetValueOrNull(instance.ParentActivityInstanceId);
                    if (parentInstance == null)
                    {
                        throw new ProcessEngineException("No parent activity instance with id " + instance.ParentActivityInstanceId + " generated");
                    }
                    PutListElement(childTransitionInstances, parentInstance, instance);
                }
            }

            foreach (KeyValuePair<ActivityInstanceImpl, IList<ActivityInstanceImpl>> entry in childActivityInstances)
            {
                ActivityInstanceImpl instance = entry.Key;
                IList<ActivityInstanceImpl> childInstances = entry.Value;
                if (childInstances != null)
                {
                    instance.ChildActivityInstances = childInstances.ToArray();
                }
            }

            foreach (KeyValuePair<ActivityInstanceImpl, IList<TransitionInstanceImpl>> entry in childTransitionInstances)
            {
                ActivityInstanceImpl instance = entry.Key;
                IList<TransitionInstanceImpl> childInstances = entry.Value;
                if (childTransitionInstances != null)
                {
                    instance.ChildTransitionInstances = childInstances.ToArray();
                }
            }

        }

        protected internal virtual void PutListElement<TS, T>(IDictionary<TS, IList<T>> mapOfLists, TS key, T listElement)
        {
            //var list = mapOfLists.ContainsKey(key) ? mapOfLists[key] : null;
            var list = mapOfLists.GetValueOrNull(key);
            if (list == null)
            {
                list = new List<T>();
                mapOfLists[key] = list;
            }
            list.Add(listElement);
        }

        protected internal virtual ExecutionEntity FilterProcessInstance(IList<ExecutionEntity> executionList)
        {
            foreach (ExecutionEntity execution in executionList)
            {
                if (execution.IsProcessInstanceExecution)
                {
                    return execution;
                }
            }

            throw new ProcessEngineException("Could not determine process instance execution");
        }

        protected internal virtual IList<ExecutionEntity> FilterLeaves(IList<ExecutionEntity> executionList)
        {
            IList<ExecutionEntity> leaves = new List<ExecutionEntity>();
            foreach (var execution in executionList)
            {
                // although executions executing throwing compensation events are not leaves in the tree,
                // they are treated as leaves since their child executions are logical children of their parent scope execution
                if (execution.NonEventScopeExecutions.Count == 0 || CompensationBehavior.IsCompensationThrowing(execution))
                {
                    leaves.Add(execution);
                }
            }
            return leaves;
        }

        protected internal virtual IList<ExecutionEntity> FilterNonEventScopeExecutions(
            IList<ExecutionEntity> executionList)
        {
            IList<ExecutionEntity> nonEventScopeExecutions = new List<ExecutionEntity>();
            foreach (var execution in executionList)
            {
                if (!execution.IsEventScope)
                {
                    nonEventScopeExecutions.Add(execution);
                }

            }

            return nonEventScopeExecutions;
        }

        protected internal virtual IList<ExecutionEntity> LoadProcessInstance(string processInstanceId, CommandContext commandContext)
        {

            //IList<ExecutionEntity> result = null;
            // first try to load from cache
            // check whether the process instance is already (partially) loaded in command context
            //TODO 弃用缓存
            //IList<ExecutionEntity> cachedExecutions = commandContext.DbEntityCache.GetEntitiesByType<ExecutionEntity>();//.DbEntityManager.GetCachedEntitiesByType<ExecutionEntity>(typeof(ExecutionEntity));
            //foreach (ExecutionEntity executionEntity in cachedExecutions)
            //{
            //    if (processInstanceId.Equals(executionEntity.ProcessInstanceId))
            //    {
            //        // found one execution from process instance
            //        result = new List<IActivityExecution>();
            //        ExecutionEntity processInstance = executionEntity.GetProcessInstance();//.ProcessInstance;
            //        // add process instance
            //        result.Add(processInstance);
            //        LoadChildExecutionsFromCache(processInstance, result);
            //        break;
            //    }
            //}
            //TODO 取消缓存，源码取消缓存，从db获取也会异常，
            //if (result == null)
            //{
                // if the process instance could not be found in cache, load from database
                return LoadFromDb(processInstanceId, commandContext);
            //}

            //return result;
        }

        protected internal virtual IList<ExecutionEntity> LoadFromDb(string processInstanceId, CommandContext commandContext)
        {

            IList<ExecutionEntity> executions = commandContext.ExecutionManager.FindExecutionsByProcessInstanceId(processInstanceId);
            ExecutionEntity processInstance = commandContext.ExecutionManager.FindExecutionById(processInstanceId);
            // initialize parent/child sets
            if (processInstance != null)
            {
                processInstance.RestoreProcessInstance(executions, null, null, null, null, null, null);
            }

            return executions;
        }

        /// <summary>
        ///     Loads all executions that are part of this process instance tree from the dbSqlSession cache.
        ///     (optionally querying the db if a child is not already loaded.
        /// </summary>
        /// <param name="execution"> the current root execution (already contained in childExecutions) </param>
        /// <param name="childExecutions"> the list in which all child executions should be collected </param>
        protected internal virtual void LoadChildExecutionsFromCache(ExecutionEntity execution,
            List<ExecutionEntity> childExecutions)
        {
            IList<ExecutionEntity> childrenOfThisExecution = execution.Executions.Cast<ExecutionEntity>().ToList();
            if (childrenOfThisExecution != null && childrenOfThisExecution.Count() > 0)
            {
                childExecutions.AddRange(childrenOfThisExecution);
                foreach (var child in childrenOfThisExecution)
                {
                    LoadChildExecutionsFromCache(child, childExecutions);
                }

            }
        }

        public class ExecutionIdComparator : IComparer<ExecutionEntity>
        {
            public static readonly ExecutionIdComparator Instance = new ExecutionIdComparator();

            public virtual int Compare(ExecutionEntity o1, ExecutionEntity o2)
            {
                return o1.Id.CompareTo(o2.Id);
            }
        }
    }
}