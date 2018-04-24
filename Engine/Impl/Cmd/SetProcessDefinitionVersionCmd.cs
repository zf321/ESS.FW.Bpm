using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Producer;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Deploy.Cache;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Persistence.Entity.Impl;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    

    /// <summary>
    ///     <seealso cref="Command" /> that changes the process definition version of an existing
    ///     process instance.
    ///     Warning: This command will NOT perform any migration magic and simply set the
    ///     process definition version in the database, assuming that the user knows,
    ///     what he or she is doing.
    ///     This is only useful for simple migrations. The new process definition MUST
    ///     have the exact same activity id to make it still run.
    ///     Furthermore, activities referenced by sub-executions and jobs that belong to
    ///     the process instance MUST exist in the new process definition version.
    ///     The command will fail, if there is already a <seealso cref="ProcessInstance" /> or
    ///     <seealso cref="HistoricProcessInstance" /> using the new process definition version and
    ///     the same business key as the <seealso cref="ProcessInstance" /> that is to be migrated.
    ///     If the process instance is not currently waiting but actively running, then
    ///     this would be a case for optimistic locking, meaning either the version
    ///     update or the "real work" wins, i.e., this is a race condition.
    /// </summary>
    [Serializable]
    public class SetProcessDefinitionVersionCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;
        private readonly int? _processDefinitionVersion;

        private readonly string _processInstanceId;

        public SetProcessDefinitionVersionCmd(string processInstanceId, int? processDefinitionVersion)
        {
            EnsureUtil.EnsureNotEmpty("The process instance id is mandatory", "processInstanceId", processInstanceId);
            EnsureUtil.EnsureNotNull("The process definition version is mandatory", "processDefinitionVersion",
                processDefinitionVersion);
            EnsureUtil.EnsurePositive("The process definition version must be positive", "processDefinitionVersion",
                processDefinitionVersion.Value);
            this._processInstanceId = processInstanceId;
            this._processDefinitionVersion = processDefinitionVersion;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            var configuration = commandContext.ProcessEngineConfiguration;

            //check that the new process definition is just another version of the same
            //process definition that the process instance is using

           IExecutionManager executionManager = commandContext.ExecutionManager;
            ExecutionEntity processInstance = executionManager.FindExecutionById(_processInstanceId);
            if (processInstance == null)
            {
                throw new ProcessEngineException("No process instance found for id = '" + _processInstanceId + "'.");
            }
            else if (!processInstance.IsProcessInstanceExecution)
            {
                throw new ProcessEngineException("A process instance id is required, but the provided id " + "'" + _processInstanceId + "' " + "points to a child execution of process instance " + "'" + processInstance.ProcessInstanceId + "'. " + "Please invoke the " + this.GetType().Name + " with a root execution id.");
            }
            ProcessDefinitionImpl currentProcessDefinitionImpl = processInstance.ProcessDefinition;

            DeploymentCache deploymentCache = configuration.DeploymentCache;
            ProcessDefinitionEntity currentProcessDefinition;
            if (currentProcessDefinitionImpl is ProcessDefinitionEntity)
            {
                currentProcessDefinition = (ProcessDefinitionEntity)currentProcessDefinitionImpl;
            }
            else
            {
                currentProcessDefinition = deploymentCache.FindDeployedProcessDefinitionById(currentProcessDefinitionImpl.Id);
            }

            ProcessDefinitionEntity newProcessDefinition = deploymentCache.FindDeployedProcessDefinitionByKeyVersionAndTenantId(currentProcessDefinition.Key, _processDefinitionVersion, currentProcessDefinition.TenantId);

            ValidateAndSwitchVersionOfExecution(commandContext, processInstance, newProcessDefinition);

            IHistoryLevel historyLevel = configuration.HistoryLevel;
            if (historyLevel.IsHistoryEventProduced(HistoryEventTypes.ProcessInstanceUpdate, processInstance))
            {
                HistoryEventProcessor.ProcessHistoryEvents(new HistoryEventCreatorAnonymousInnerClass(this, processInstance));
            }

            // switch all sub-executions of the process instance to the new process definition version
            IList<ExecutionEntity> childExecutions = executionManager.FindExecutionsByProcessInstanceId(_processInstanceId);
            foreach (ExecutionEntity executionEntity in childExecutions)
            {
                ValidateAndSwitchVersionOfExecution(commandContext, executionEntity, newProcessDefinition);
            }

            // switch all jobs to the new process definition version
            IList<JobEntity> jobs = commandContext.JobManager.FindJobsByProcessInstanceId(_processInstanceId);
            IList<JobDefinitionEntity> currentJobDefinitions = commandContext.JobDefinitionManager.FindByProcessDefinitionId(currentProcessDefinition.Id);
            IList<JobDefinitionEntity> newVersionJobDefinitions = commandContext.JobDefinitionManager.FindByProcessDefinitionId(newProcessDefinition.Id);

            IDictionary<string, string> jobDefinitionMapping = GetJobDefinitionMapping(currentJobDefinitions, newVersionJobDefinitions);
            foreach (JobEntity jobEntity in jobs)
            {
                SwitchVersionOfJob(jobEntity, newProcessDefinition, jobDefinitionMapping);
            }

            // switch all incidents to the new process definition version
            IList<IncidentEntity> incidents = commandContext.IncidentManager.FindIncidentsByProcessInstance(_processInstanceId);
            foreach (IncidentEntity incidentEntity in incidents)
            {
                SwitchVersionOfIncident(commandContext, incidentEntity, newProcessDefinition);
            }

            // add an entry to the op log
            PropertyChange change = new PropertyChange("processDefinitionVersion", currentProcessDefinition.Version, _processDefinitionVersion);
            commandContext.OperationLogManager.LogProcessInstanceOperation(UserOperationLogEntryFields.OperationTypeModifyProcessInstance, _processInstanceId, null, null, new List<PropertyChange>() {change});
            return null;
        }

        protected internal virtual IDictionary<string, string> GetJobDefinitionMapping(
            IList<JobDefinitionEntity> currentJobDefinitions, IList<JobDefinitionEntity> newVersionJobDefinitions)
        {
            IDictionary<string, string> mapping = new Dictionary<string, string>();

            foreach (var currentJobDefinition in currentJobDefinitions)
                foreach (var newJobDefinition in newVersionJobDefinitions)
                    if (JobDefinitionsMatch(currentJobDefinition, newJobDefinition))
                    {
                        mapping[currentJobDefinition.Id] = newJobDefinition.Id;
                        break;
                    }

            return mapping;
        }

        protected internal virtual bool JobDefinitionsMatch(JobDefinitionEntity currentJobDefinition,
            JobDefinitionEntity newJobDefinition)
        {
            var activitiesMatch = currentJobDefinition.ActivityId.Equals(newJobDefinition.ActivityId);

            var typesMatch = (ReferenceEquals(currentJobDefinition.JobType, null) &&
                              ReferenceEquals(newJobDefinition.JobType, null)) ||
                             (!ReferenceEquals(currentJobDefinition.JobType, null) &&
                              currentJobDefinition.JobType.Equals(newJobDefinition.JobType));

            var configurationsMatch = (ReferenceEquals(currentJobDefinition.JobConfiguration, null) &&
                                       ReferenceEquals(newJobDefinition.JobConfiguration, null)) ||
                                      (!ReferenceEquals(currentJobDefinition.JobConfiguration, null) &&
                                       currentJobDefinition.JobConfiguration.Equals(newJobDefinition.JobConfiguration));

            return activitiesMatch && typesMatch && configurationsMatch;
        }

        protected internal virtual void SwitchVersionOfJob(JobEntity jobEntity,
            ProcessDefinitionEntity newProcessDefinition, IDictionary<string, string> jobDefinitionMapping)
        {
            jobEntity.ProcessDefinitionId = newProcessDefinition.Id;
            jobEntity.DeploymentId = newProcessDefinition.DeploymentId;

            var newJobDefinitionId = jobDefinitionMapping[jobEntity.JobDefinitionId];
            jobEntity.JobDefinitionId = newJobDefinitionId;
        }

        protected internal virtual void SwitchVersionOfIncident(CommandContext commandContext,
            IncidentEntity incidentEntity, ProcessDefinitionEntity newProcessDefinition)
        {
            incidentEntity.ProcessDefinitionId = newProcessDefinition.Id;
        }

        protected internal virtual void ValidateAndSwitchVersionOfExecution(CommandContext commandContext,
            ExecutionEntity execution, ProcessDefinitionEntity newProcessDefinition)
        {
            // check that the new process definition version contains the current activity
            if (execution.Activity != null)
            {
                string activityId = execution.Activity.Id;
                IPvmActivity newActivity = newProcessDefinition.FindActivity(activityId);

                if (newActivity == null)
                {
                    throw new ProcessEngineException("The new process definition " + "(key = '" + newProcessDefinition.Key + "') " + "does not contain the current activity " + "(id = '" + activityId + "') " + "of the process instance " + "(id = '" + _processInstanceId + "').");
                }

                // clear cached activity so that outgoing transitions are refreshed
                execution.Activity = (newActivity);
            }

            // switch the process instance to the new process definition version
            execution.SetProcessDefinition(newProcessDefinition);

            // and change possible existing tasks (as the process definition id is stored there too)
            IList<TaskEntity> tasks = commandContext.TaskManager.FindTasksByExecutionId(execution.Id);
            foreach (TaskEntity taskEntity in tasks)
            {
                taskEntity.ProcessDefinitionId = newProcessDefinition.Id;
            }
        }

        private class HistoryEventCreatorAnonymousInnerClass : HistoryEventProcessor.HistoryEventCreator
        {
            private readonly SetProcessDefinitionVersionCmd _outerInstance;

            private ExecutionEntity _processInstance;

            public HistoryEventCreatorAnonymousInnerClass(SetProcessDefinitionVersionCmd outerInstance,
                ExecutionEntity processInstance)
            {
                this._outerInstance = outerInstance;
                this._processInstance = processInstance;
            }

            public override HistoryEvent CreateHistoryEvent(IHistoryEventProducer producer)
            {
                return producer.CreateProcessInstanceUpdateEvt(_processInstance);
            }
        }
    }
}