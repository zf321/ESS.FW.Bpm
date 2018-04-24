using System;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Management.Impl;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    public abstract class AbstractSetJobStateCmd : AbstractSetStateCmd
    {
        protected internal string JobDefinitionId;

        protected internal string JobId;
        protected internal string ProcessDefinitionId;
        protected internal string ProcessDefinitionKey;

        protected internal string ProcessDefinitionTenantId;
        protected internal bool ProcessDefinitionTenantIdSet;
        protected internal string ProcessInstanceId;

        public AbstractSetJobStateCmd(IUpdateJobSuspensionStateBuilder builder) 
            : base(false, null)
        {
            JobId = builder.JobId;
            JobDefinitionId = builder.JobDefinitionId;
            ProcessInstanceId = builder.ProcessInstanceId;
            ProcessDefinitionId = builder.ProcessDefinitionId;
            ProcessDefinitionKey = builder.ProcessDefinitionKey;

            ProcessDefinitionTenantIdSet = builder.ProcessDefinitionTenantIdSet;
            ProcessDefinitionTenantId = builder.processDefinitionTenantId;
        }

        protected internal override void CheckParameters(CommandContext commandContext)
        {
            if (ReferenceEquals(JobId, null) && ReferenceEquals(JobDefinitionId, null) &&
                ReferenceEquals(ProcessInstanceId, null) && ReferenceEquals(ProcessDefinitionId, null) &&
                ReferenceEquals(ProcessDefinitionKey, null))
                throw new ProcessEngineException(
                    "Job id, job definition id, process instance id, process definition id nor process definition key cannot be null");
        }

        protected internal override void CheckAuthorization(CommandContext commandContext)
        {

            foreach (ICommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                if (!string.ReferenceEquals(JobId, null))
                {

                    IJobManager jobManager = commandContext.JobManager;
                    JobEntity job = jobManager.FindJobById(JobId);

                    if (job != null)
                    {

                        string processInstanceId = job.ProcessInstanceId;
                        if (!string.ReferenceEquals(processInstanceId, null))
                        {
                            checker.CheckUpdateProcessInstanceById(processInstanceId);
                        }
                        else
                        {
                            // start timer job is not assigned to a specific process
                            // instance, that's why we have to check whether there
                            // exists a UPDATE_INSTANCES permission on process definition or
                            // a UPDATE permission on any process instance
                            string processDefinitionKey = job.ProcessDefinitionKey;
                            if (!string.ReferenceEquals(processDefinitionKey, null))
                            {
                                checker.CheckUpdateProcessInstanceByProcessDefinitionKey(processDefinitionKey);
                            }
                        }
                        // if (processInstanceId == null && processDefinitionKey == null):
                        // job is not assigned to any process instance nor process definition
                        // then it is always possible to activate/suspend the corresponding job
                        // -> no authorization check necessary
                    }
                }
                else
                {

                    if (!string.IsNullOrEmpty(JobDefinitionId))
                    {

                        JobDefinitionEntity jobDefinition = commandContext.JobDefinitionManager.FindById(JobDefinitionId);

                        if (jobDefinition != null)
                        {
                            string processDefinitionKey = jobDefinition.ProcessDefinitionKey;
                            checker.CheckUpdateProcessInstanceByProcessDefinitionKey(processDefinitionKey);
                        }

                    }
                    else
                    {

                        if (!string.ReferenceEquals(ProcessInstanceId, null))
                        {
                            checker.CheckUpdateProcessInstanceById(ProcessInstanceId);
                        }
                        else
                        {

                            if (!string.ReferenceEquals(ProcessDefinitionId, null))
                            {
                                checker.CheckUpdateProcessInstanceByProcessDefinitionId(ProcessDefinitionId);
                            }
                            else
                            {

                                if (!string.ReferenceEquals(ProcessDefinitionKey, null))
                                {
                                    checker.CheckUpdateProcessInstanceByProcessDefinitionKey(ProcessDefinitionKey);
                                }
                            }
                        }
                    }
                }
            }
        }

        protected internal override void UpdateSuspensionState(CommandContext commandContext,
            ISuspensionState suspensionState)
        {
            IJobManager jobManager = commandContext.JobManager;

            if (!string.IsNullOrEmpty(JobId))
            {
                jobManager.UpdateJobSuspensionStateById(JobId, suspensionState);
            }
            else if (!string.IsNullOrEmpty(JobDefinitionId))
            {
                jobManager.UpdateJobSuspensionStateByJobDefinitionId(JobDefinitionId, suspensionState);
            }
            else if (!string.IsNullOrEmpty(ProcessInstanceId))
            {
                jobManager.UpdateJobSuspensionStateByProcessInstanceId(ProcessInstanceId, suspensionState);
            }
            else if (!string.IsNullOrEmpty(ProcessDefinitionId))
            {
                jobManager.UpdateJobSuspensionStateByProcessDefinitionId(ProcessDefinitionId, suspensionState);
            }
            else if (!string.IsNullOrEmpty(ProcessDefinitionKey))
            {
                if (!ProcessDefinitionTenantIdSet)
                {
                    jobManager.UpdateJobSuspensionStateByProcessDefinitionKey(ProcessDefinitionKey, suspensionState);
                }
                else
                {
                    jobManager.UpdateJobSuspensionStateByProcessDefinitionKeyAndTenantId(ProcessDefinitionKey, ProcessDefinitionTenantId, suspensionState);
                }
            }
        }

        protected internal override void LogUserOperation(CommandContext commandContext)
        {
            PropertyChange propertyChange = new PropertyChange(SuspensionStateProperty, null, NewSuspensionState.Name);
            commandContext.OperationLogManager.LogJobOperation(LogEntryOperation, JobId, JobDefinitionId, ProcessInstanceId, ProcessDefinitionId, ProcessDefinitionKey, propertyChange);
        }
    }
}