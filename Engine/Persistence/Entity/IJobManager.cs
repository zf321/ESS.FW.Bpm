using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.DataAccess;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IJobManager : IRepository<JobEntity,string>
    {
        void CancelTimers(ExecutionEntity execution);
        void DeleteJob(JobEntity job);
        void DeleteJob(JobEntity job, bool fireDeleteEvent);
        JobEntity FindJobById(string jobId);
        IList<JobEntity> FindJobsByConfiguration(string jobHandlerType, string jobHandlerConfiguration, string tenantId);
        IList<JobEntity> FindJobsByExecutionId(string executionId);
        IList<JobEntity> FindJobsByJobDefinitionId(string jobDefinitionId);
        IList<JobEntity> FindJobsByProcessInstanceId(string processInstanceId);
        IList<JobEntity> FindNextJobsToExecute(Page page);
        IList<TimerEntity> FindTimersByExecutionId(string executionId);
        IList<TimerEntity> FindUnlockedTimersByDuedate(DateTime duedate, Page page);
        IList<JobEntity> FindJobsByExecutable();
        void InsertAndHintJobExecutor(JobEntity jobEntity);
        void InsertJob(JobEntity job);
        void Schedule(TimerEntity timer);
        void Send(MessageEntity message);
        void UpdateFailedJobRetriesByJobDefinitionId(string jobDefinitionId, int retries);
        void UpdateJobPriorityByDefinitionId(string jobDefinitionId, long priority);
        void UpdateJobSuspensionStateById(string jobId, ISuspensionState suspensionState);
        void UpdateJobSuspensionStateByJobDefinitionId(string jobDefinitionId, ISuspensionState suspensionState);
        void UpdateJobSuspensionStateByProcessDefinitionId(string processDefinitionId, ISuspensionState suspensionState);
        void UpdateJobSuspensionStateByProcessDefinitionKey(string processDefinitionKey, ISuspensionState suspensionState);
        void UpdateJobSuspensionStateByProcessDefinitionKeyAndTenantId(string processDefinitionKey, string processDefinitionTenantId, ISuspensionState suspensionState);
        void UpdateJobSuspensionStateByProcessInstanceId(string processInstanceId, ISuspensionState suspensionState);
        void UpdateStartTimerJobSuspensionStateByProcessDefinitionId(string processDefinitionId, ISuspensionState suspensionState);
        void UpdateStartTimerJobSuspensionStateByProcessDefinitionKey(string processDefinitionKey, ISuspensionState suspensionState);
        void UpdateStartTimerJobSuspensionStateByProcessDefinitionKeyAndTenantId(string processDefinitionKey, string processDefinitionTenantId, ISuspensionState suspensionState);
        JobEntity FindJobByHandlerType(string handlerType);
        void ReSchedule(JobEntity jobEntity, DateTime currentTime);
    }
}