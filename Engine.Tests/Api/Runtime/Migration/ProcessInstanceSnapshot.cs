using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    ///     Helper class to save the current state of a process instance.
    /// </summary>
    public class ProcessInstanceSnapshot
    {
        protected string processInstanceId;
        protected string processDefinitionId;
        protected string deploymentId;
        protected IActivityInstance activityTree;
        protected ExecutionTree executionTree;
        protected IList<IEventSubscription> eventSubscriptions;
        protected IList<IJob> jobs;
        protected IList<IJobDefinition> jobDefinitions;
        protected IList<ITask> tasks;
        protected internal IDictionary<string, IVariableInstance> variables;

        public ProcessInstanceSnapshot(string processInstanceId, string processDefinitionId)
        {
            this.processInstanceId = processInstanceId;
            this.processDefinitionId = processDefinitionId;
        }

        public virtual string ProcessInstanceId
        {
            get => processInstanceId;
            set => processInstanceId = value;
        }


        public virtual string ProcessDefinitionId
        {
            get => processDefinitionId;
            set => processDefinitionId = value;
        }

        public virtual string DeploymentId
        {
            set => deploymentId = value;
            get => deploymentId;
        }


        public virtual IActivityInstance ActivityTree
        {
            get
            {
                EnsurePropertySaved("activity tree", activityTree);
                return activityTree;
            }
            set => activityTree = value;
        }


        public virtual ExecutionTree ExecutionTree
        {
            get
            {
                EnsurePropertySaved("execution tree", executionTree);
                return executionTree;
            }
            set => executionTree = value;
        }


        public virtual IList<ITask> Tasks
        {
            set => tasks = value;
            get
            {
                EnsurePropertySaved("tasks", tasks);
                return tasks;
            }
        }

        public virtual ITask GetTaskForKey(string key)
        {
            return this.Tasks.FirstOrDefault(t => t.TaskDefinitionKey.Equals(key));
            //foreach (var task in Tasks)
            //    if (key.Equals(task.TaskDefinitionKey))
            //        return task;
            //return null;
        }

        public virtual IList<IEventSubscription> EventSubscriptions
        {
            get
            {
                EnsurePropertySaved("event subscriptions", eventSubscriptions);
                return eventSubscriptions;
            }
            set => eventSubscriptions = value;
        }

        public virtual IEventSubscription GetEventSubscriptionById(string id)
        {
            return this.eventSubscriptions.FirstOrDefault(e => e.Id.Equals(id));
            //foreach (var subscription in eventSubscriptions)
            //    if (subscription.Id.Equals(id))
            //        return subscription;

            //return null;
        }

        public virtual IEventSubscription GetEventSubscriptionForActivityIdAndEventName(string activityId, string eventName)
        {
            var collectedEventsubscriptions = GetEventSubscriptionsForActivityIdAndEventName(activityId, eventName);

            if (collectedEventsubscriptions.Count == 0)
                return null;
            if (collectedEventsubscriptions.Count == 1)
                return collectedEventsubscriptions[0];
            throw new System.Exception("There is more than one event subscription for activity " + activityId + " and event " + eventName);
        }

        public virtual IList<IEventSubscription> GetEventSubscriptionsForActivityIdAndEventName(string activityId, string eventName)
        {
            IList<IEventSubscription> collectedEventsubscriptions = new List<IEventSubscription>();

            foreach (var eventSubscription in EventSubscriptions)
                if (activityId.Equals(eventSubscription.ActivityId))
                    if (ReferenceEquals(eventName, null) && eventSubscription.EventName == null ||
                        !ReferenceEquals(eventName, null) && eventName.Equals(eventSubscription.EventName))
                        collectedEventsubscriptions.Add(eventSubscription);

            return collectedEventsubscriptions;
        }


        public virtual IList<IJob> Jobs
        {
            get
            {
                EnsurePropertySaved("jobs", jobs);
                return jobs;
            }
            set => jobs = value;
        }


        public virtual IList<IJobDefinition> JobDefinitions
        {
            get
            {
                EnsurePropertySaved("job definitions", jobDefinitions);
                return jobDefinitions;
            }
            set => jobDefinitions = value;
        }


        public virtual IJob GetJobForDefinitionId(string jobDefinitionId)
        {
            IList<IJob> collectedJobs = new List<IJob>();

            foreach (var job in Jobs)
                if (jobDefinitionId.Equals(job.JobDefinitionId))
                    collectedJobs.Add(job);

            if (collectedJobs.Count == 0)
                return null;
            if (collectedJobs.Count == 1)
                return collectedJobs[0];
            throw new System.Exception("There is more than one job for job definition " + jobDefinitionId);
        }

        public virtual IJob GetJobById(string jobId)
        {
            return Jobs.FirstOrDefault(j => j.Id.Equals(jobId));
            //foreach (var job in Jobs)
            //    if (jobId.Equals(job.Id))
            //        return job;

            //return null;
        }

        public virtual IJobDefinition GetJobDefinitionForActivityIdAndType(string activityId, string jobHandlerType)
        {
            IList<IJobDefinition> collectedDefinitions = new List<IJobDefinition>();
            foreach (var jobDefinition in JobDefinitions)
                if (activityId.Equals(jobDefinition.ActivityId) && jobHandlerType.Equals(jobDefinition.JobType))
                    collectedDefinitions.Add(jobDefinition);

            if (collectedDefinitions.Count == 0)
                return null;
            if (collectedDefinitions.Count == 1)
                return collectedDefinitions[0];
            throw new System.Exception("There is more than one job definition for activity " + activityId +
                                " and job handler type " + jobHandlerType);
        }


        public virtual ICollection<IVariableInstance> GetVariables()
        {
            return variables.Values;
        }

        public virtual void SetVariables(IList<IVariableInstance> variables)
        {
            this.variables = new Dictionary<string, IVariableInstance>();

            foreach (var variable in variables)
                this.variables[variable.Id] = variable;
        }

        public virtual IVariableInstance GetSingleVariable(string variableName)
        {
            return GetSingleVariable(new ConditionAnonymousInnerClass(variableName));
        }

        private sealed class ConditionAnonymousInnerClass : ICondition<IVariableInstance>
        {
            private readonly string _variableName;

            public ConditionAnonymousInnerClass(string variableName)
            {
                _variableName = variableName;
            }


            public bool Matches(IVariableInstance variable)
            {
                return _variableName.Equals(variable.Name);
            }
        }

        public virtual IVariableInstance GetSingleVariable(string executionId, string variableName)
        {
            return GetSingleVariable(new ConditionAnonymousInnerClass2(executionId, variableName));
        }

        private sealed class ConditionAnonymousInnerClass2 : ICondition<IVariableInstance>
        {
            private readonly string _executionId;
            private readonly string _variableName;

            public ConditionAnonymousInnerClass2(string executionId,
                string variableName)
            {
                _executionId = executionId;
                _variableName = variableName;
            }


            public bool Matches(IVariableInstance variable)
            {
                return _executionId.Equals(variable.ExecutionId) && _variableName.Equals(variable.Name);
            }
        }

        protected internal virtual IVariableInstance GetSingleVariable(ICondition<IVariableInstance> condition)
        {
            IList<IVariableInstance> matchingVariables = new List<IVariableInstance>();

            foreach (var variable in variables.Values)
                if (condition.Matches(variable))
                    matchingVariables.Add(variable);

            if (matchingVariables.Count == 1)
                return matchingVariables[0];
            if (matchingVariables.Count == 0)
                return null;
            throw new System.Exception("There is more than one variable that matches the given condition");
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public org.Camunda.bpm.Engine.Runtime.IVariableInstance getSingleTaskVariable(final String taskId, final String VariableName)
        public virtual IVariableInstance GetSingleTaskVariable(string taskId, string variableName)
        {
            return GetSingleVariable(new ConditionAnonymousInnerClass3(this, taskId, variableName));
        }



        public virtual IVariableInstance GetVariable(string id)
        {
            return variables[id];
        }

        protected internal virtual void EnsurePropertySaved(string name, object property)
        {
            EnsureUtil.EnsureNotNull(typeof(BadUserRequestException),
                "The snapshot has not saved the " + name + " of the process instance", name, property);
        }


        private class ConditionAnonymousInnerClass3 : ICondition<IVariableInstance>
        {
            private readonly ProcessInstanceSnapshot _outerInstance;

            private readonly string _taskId;
            private readonly string _variableName;

            public ConditionAnonymousInnerClass3(ProcessInstanceSnapshot outerInstance, string taskId,
                string variableName)
            {
                _outerInstance = outerInstance;
                _taskId = taskId;
                _variableName = variableName;
            }


            public virtual bool Matches(IVariableInstance variable)
            {
                return _variableName.Equals(variable.Name) && _taskId.Equals(variable.TaskId);
            }
        }

        protected internal interface ICondition<T>
        {
            bool Matches(T condition);
        }
    }
}