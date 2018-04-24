using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
using System.Linq;
using System.Transactions;
using Autofac.Features.AttributeFilters;

using ESS.FW.Common.Extensions;
using ESS.FW.Common.Components;
using ESS.FW.Common.Utilities;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{

    /// <summary>
    ///  
    /// 
    /// </summary>
    [Component]
    public class JobManager : AbstractManagerNet<JobEntity>, IJobManager
    {

        protected IHistoricJobLogManager historicJobLogManager;
        public JobManager(DbContext dbContex, ILoggerFactory loggerFactory, IHistoricJobLogManager _historicJobLogManager, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
            historicJobLogManager = _historicJobLogManager;
        }

        public virtual void InsertJob(JobEntity job)
        {
            Add(job);
            historicJobLogManager.FireJobCreatedEvent(job);
        }

        public virtual void DeleteJob(JobEntity job)
        {
            DeleteJob(job, true);
        }

        public virtual void DeleteJob(JobEntity job, bool fireDeleteEvent)
        {
            Delete(job);

            if (fireDeleteEvent)
            {
                historicJobLogManager.FireJobDeletedEvent(job);
            }

        }

        public virtual void InsertAndHintJobExecutor(JobEntity jobEntity)
        {
            jobEntity.Insert();
            if (context.Impl.Context.ProcessEngineConfiguration.HintJobExecutor)
            {
                HintJobExecutor(jobEntity);
            }
        }
        //异步？
        public virtual void Send(MessageEntity message)
        {
            message.Insert();
            if (context.Impl.Context.ProcessEngineConfiguration.HintJobExecutor)
            {
                HintJobExecutor(message);
            }
        }

        public virtual void Schedule(TimerEntity timer)
        {

            DateTime? duedate = timer.Duedate;
            EnsureUtil.EnsureNotNull("duedate", duedate);

            timer.Insert();

            // Check if this timer fires before the next time the job executor will check for new timers to fire.
            // This is highly unlikely because normally waitTimeInMillis is 5000 (5 seconds)
            // and timers are usually set further in the future

            JobExecutor jobExecutor = context.Impl.Context.ProcessEngineConfiguration.JobExecutor;
            int waitTimeInMillis = jobExecutor.WaitTimeInMillis;
            if (((DateTime)duedate).Ticks < (ClockUtil.CurrentTime.Ticks + waitTimeInMillis))
            {
                HintJobExecutor(timer);
            }
        }

        protected internal virtual void HintJobExecutor(JobEntity job)
        {

            JobExecutor jobExecutor = context.Impl.Context.ProcessEngineConfiguration.JobExecutor;
            if (!jobExecutor.IsActive)
            {
                return;
            }

            JobExecutorContext jobExecutorContext = context.Impl.Context.JobExecutorContext;
            ITransactionListener transactionListener = null;
            if (!job.Suspended && job.Exclusive && jobExecutorContext != null && jobExecutorContext.ExecutingExclusiveJob && AreInSameProcessInstance(job, jobExecutorContext.CurrentJob))
            {
                // lock job & add to the queue of the current processor
                DateTime currentTime = ClockUtil.CurrentTime;
                job.LockExpirationTime = new DateTime(currentTime.Ticks + jobExecutor.LockTimeInMillis);
                job.LockOwner = jobExecutor.LockOwner;
                transactionListener = new ExclusiveJobAddedNotification(job.Id, jobExecutorContext);
            }
            else
            {
                // notify job executor:
                transactionListener = new MessageAddedNotification(jobExecutor);
            }
            context.Impl.Context.CommandContext.TransactionContext.AddTransactionListener(TransactionJavaStatus.Committed, transactionListener);
        }

        protected internal virtual bool AreInSameProcessInstance(JobEntity job1, JobEntity job2)
        {
            if (job1 == null || job2 == null)
            {
                return false;
            }

            string instance1 = job1.ProcessInstanceId;
            string instance2 = job2.ProcessInstanceId;

            return instance1 != null && instance1.Equals(instance2);
        }

        public virtual void CancelTimers(ExecutionEntity execution)
        {
            IList<TimerEntity> timers = context.Impl.Context.CommandContext.JobManager.FindTimersByExecutionId(execution.Id);

            foreach (TimerEntity timer in timers)
            {
                timer.Delete();
            }
        }

        public virtual JobEntity FindJobById(string jobId)
        {

            //return (JobEntity) DbEntityManager.SelectOne("selectJob", jobId);
            return Get(jobId);
        }
        public virtual IList<JobEntity> FindNextJobsToExecute(Page page)
        {
            //resources/org/camunda/bpm/engine/impl/mapping/entity/job.xml-<select id="selectNextJobsToExecute">

            var now = ClockUtil.CurrentTime;

            var predicate = ParameterBuilder.True<JobEntity>()
                .AndParam(c => c.RetriesFromPersistence > 0)
                .AndParam(c => c.Duedate == null || c.Duedate <= now)
                .AndParam(c => string.IsNullOrEmpty(c.LockOwner) || c.LockExpirationTime < now)
                .AndParam(c => c.SuspensionState == 1);

            if (context.Impl.Context.ProcessEngineConfiguration.JobExecutorDeploymentAware)
            {
                var registeredDeployments = context.Impl.Context.ProcessEngineConfiguration.RegisteredDeployments;
                if (registeredDeployments != null && registeredDeployments.Count > 0)
                {
                    predicate =
                        predicate.AndParam(
                            c =>
                                string.IsNullOrEmpty(c.DeploymentId) ||
                                registeredDeployments.Contains(c.DeploymentId));
                }
                else
                {
                    predicate = predicate.AndParam(c => string.IsNullOrEmpty(c.DeploymentId));
                }
            }

            predicate =
                predicate.AndParam(
                    c =>
                        c.Exclusive == false ||
                        //(c.Exclusive == true && !DbContext.Set<JobEntity>().Any(j => j.ProcessInstanceId == c.ProcessInstanceId && j.Exclusive == true && (!string.IsNullOrEmpty(j.LockOwner) && j.LockExpirationTime >= now)))
                        //(c.Exclusive == true && !DbContext.Set<JobEntity>().Where(j => j.ProcessInstanceId == c.ProcessInstanceId).Any(j => j.Exclusive == true && (j.LockOwner != null && j.LockExpirationTime >= now)))
                        (c.Exclusive == true && DbContext.Set<JobEntity>()
                             .Count(j => j.ProcessInstanceId == c.ProcessInstanceId && j.Exclusive &&
                                         (!string.IsNullOrEmpty(j.LockOwner) && j.LockExpirationTime >= now)) <= 0)
                );

            var datas = Find(predicate).ToList();

            var orderbyFlag = false;
            IOrderedEnumerable<JobEntity> tmp = null;
            if (context.Impl.Context.ProcessEngineConfiguration.JobExecutorAcquireByPriority)
            {
                tmp = datas.OrderByDescending(c => c.Priority);
                orderbyFlag = true;
            }
            if (context.Impl.Context.ProcessEngineConfiguration.JobExecutorPreferTimerJobs)
            {
                if (orderbyFlag)
                {
                    tmp = tmp.ThenByDescending(c => c.Type);
                }
                else
                {
                    tmp = datas.OrderByDescending(c => c.Type);
                    orderbyFlag = true;
                }
            }
            if (context.Impl.Context.ProcessEngineConfiguration.jobExecutorAcquireByDueDate)
            {
                if (orderbyFlag)
                {
                    tmp = tmp.ThenBy(c => c.Duedate);
                }
                else
                {
                    tmp = datas.OrderBy(c => c.Duedate);
                    orderbyFlag = true;
                }
            }

            if (orderbyFlag)
            {
                if (page != null)
                    return tmp.Skip(page.FirstResult).Take(page.MaxResults).ToList();
                return tmp.ToList();
            }
            if (page != null)
                return datas.OrderBy(c => c.Id).Skip(page.FirstResult).Take(page.MaxResults).ToList();
            return datas;

            //var tmp = datas.ToList();
            //return tmp;

        }

        public virtual IList<JobEntity> FindJobsByExecutionId(string executionId)
        {

            //return DbEntityManager.SelectList("selectJobsByExecutionId", executionId);
            return Find(m => m.ExecutionId == executionId).ToList();
        }

        public virtual IList<JobEntity> FindJobsByProcessInstanceId(string processInstanceId)
        {

            //return DbEntityManager.SelectList("selectJobsByProcessInstanceId", processInstanceId);
            return Find(m => m.ProcessInstanceId == processInstanceId).ToList();
        }

        public virtual IList<JobEntity> FindJobsByJobDefinitionId(string jobDefinitionId)
        {

            //return DbEntityManager.SelectList("selectJobsByJobDefinitionId", jobDefinitionId);
            return Find(m => m.JobDefinitionId == jobDefinitionId).ToList();
        }
        public virtual IList<TimerEntity> FindUnlockedTimersByDuedate(DateTime duedate, Page page)
        {
            //const string query = "selectUnlockedTimersByDuedate";
            return (from a in DbContext.Set<TimerEntity>()
                    where /*a.Type == "timer" &&*/ a.Duedate != null && a.Retries > 0 && a.Duedate < duedate
                    && (a.LockOwner == null || a.LockExpirationTime < duedate)
                    orderby a.Duedate
                    select a).Skip(page.FirstResult).Take(page.MaxResults).ToList();
        }

        public IList<JobEntity> FindJobsByExecutable()
        {
            DateTime now = ClockUtil.CurrentTime;
            var datas = (from res in DbContext.Set<JobEntity>()
                join tmp in DbContext.Set<ExecutionEntity>()
                on res.ProcessInstanceId equals tmp.Id into procIns
                from pi in procIns.DefaultIfEmpty()
                where res.RetriesFromPersistence > 0 && (res.Duedate == null || res.Duedate <= now) &
                      (res.ExecutionId == null || (pi.SuspensionState == 1 || pi.SuspensionState == null))
                select res).ToList();                
            
            return datas;
        }

        public virtual IList<TimerEntity> FindTimersByExecutionId(string executionId)
        {

            //return DbEntityManager.SelectList("selectTimersByExecutionId", executionId);
            return Find(m => m.ExecutionId == executionId).ToList().Cast<TimerEntity>().ToList();// as IList<TimerEntity>;
        }
        //public virtual IList<IJob> FindJobsByQueryCriteria(JobQueryImpl jobQuery, Page page)
        //{
        //    throw new NotImplementedException();
        //    ConfigureQuery(jobQuery);
        //    //return DbEntityManager.SelectList("selectJobByQueryCriteria", jobQuery, page);
        //}

        public virtual IList<JobEntity> FindJobsByConfiguration(string jobHandlerType, string jobHandlerConfiguration, string tenantId)
        {

            //IDictionary<string, string> @params = new Dictionary<string, string>();
            //@params["handlerType"] = jobHandlerType;
            //@params["handlerConfiguration"] = jobHandlerConfiguration;
            //@params["tenantId"] = tenantId;

            //if (TimerCatchIntermediateEventJobHandler.TYPE.Equals(jobHandlerType) || TimerExecuteNestedActivityJobHandler.TYPE.Equals(jobHandlerType) || TimerStartEventJobHandler.TYPE.Equals(jobHandlerType) || TimerStartEventSubprocessJobHandler.TYPE.Equals(jobHandlerType))
            //{
            //    //string queryValue = jobHandlerConfiguration +  JOB_HANDLER_CONFIG_PROPERTY_DELIMITER + //JOB_HANDLER_CONFIG_PROPERTY_FOLLOW_UP_JOB_CREATED;
            //    //@params["handlerConfigurationWithFollowUpJobCreatedProperty"] = queryValue;
            //}

            //return DbEntityManager.SelectList("selectJobsByConfiguration", @params);
            try
            {
                return
                    Find(
                        m =>
                            m.JobHandlerType == jobHandlerType && m.TenantId == tenantId &&
                            m.JobHandlerConfigurationRaw == jobHandlerConfiguration).ToList();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        //public virtual long FindJobCountByQueryCriteria(JobQueryImpl jobQuery)
        //{
        //    throw new NotImplementedException();
        //    //          ConfigureQuery(jobQuery);
        //    //return (long?) DbEntityManager.SelectOne("selectJobCountByQueryCriteria", jobQuery);
        //}

        public virtual void UpdateJobSuspensionStateById(string jobId, ISuspensionState suspensionState)
        {
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["jobId"] = jobId;
            //parameters["suspensionState"] = suspensionState.StateCode;

            //DbEntityManager.Update(typeof(JobEntity), "updateJobSuspensionStateByParameters", ConfigureParameterizedQuery(parameters));
            Find(m => m.Id == jobId).ForEach((t) =>
            {
                t.Revision = t.RevisionNext;
                t.SuspensionState = suspensionState.StateCode;
            });
        }

        public virtual void UpdateJobSuspensionStateByJobDefinitionId(string jobDefinitionId, ISuspensionState suspensionState)
        {
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["jobDefinitionId"] = jobDefinitionId;
            //parameters["suspensionState"] = suspensionState.StateCode;
            //DbEntityManager.Update(typeof(JobEntity), "updateJobSuspensionStateByParameters", ConfigureParameterizedQuery(parameters));
            Find(m => m.JobDefinitionId == jobDefinitionId).ForEach((t) =>
            {
                t.Revision = t.RevisionNext;
                t.SuspensionState = suspensionState.StateCode;
            });
        }

        public virtual void UpdateJobSuspensionStateByProcessInstanceId(string processInstanceId, ISuspensionState suspensionState)
        {
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["processInstanceId"] = processInstanceId;
            //parameters["suspensionState"] = suspensionState.StateCode;
            //DbEntityManager.Update(typeof(JobEntity), "updateJobSuspensionStateByParameters", ConfigureParameterizedQuery(parameters));
            Find(m => m.ProcessInstanceId == processInstanceId).ForEach((t) =>
            {
                t.Revision = t.RevisionNext;
                t.SuspensionState = suspensionState.StateCode;
            });
        }

        public virtual void UpdateJobSuspensionStateByProcessDefinitionId(string processDefinitionId, ISuspensionState suspensionState)
        {
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["processDefinitionId"] = processDefinitionId;
            //parameters["suspensionState"] = suspensionState.StateCode;
            //DbEntityManager.Update(typeof(JobEntity), "updateJobSuspensionStateByParameters", ConfigureParameterizedQuery(parameters));
            Find(m => m.ProcessDefinitionId == processDefinitionId).ForEach((t) =>
            {
                t.Revision = t.RevisionNext;
                t.SuspensionState = suspensionState.StateCode;
            });
        }

        public virtual void UpdateStartTimerJobSuspensionStateByProcessDefinitionId(string processDefinitionId, ISuspensionState suspensionState)
        {
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["processDefinitionId"] = processDefinitionId;
            //parameters["suspensionState"] = suspensionState.StateCode;
            //parameters["handlerType"] = TimerStartEventJobHandler.TYPE;
            //DbEntityManager.Update(typeof(JobEntity), "updateJobSuspensionStateByParameters", ConfigureParameterizedQuery(parameters));

            var predicate =
                ParameterBuilder.True<JobEntity>().AndParam(c => c.JobHandlerType == TimerStartEventJobHandler.TYPE);

            if (!string.IsNullOrEmpty(processDefinitionId))
                predicate = predicate.AndParam(c => c.ProcessDefinitionId == processDefinitionId);

            var jobs = Find(predicate);

            foreach (var job in jobs)
            {
                job.Revision = job.RevisionNext;
                job.SuspensionState = suspensionState.StateCode;
            }
            DbContext.SaveChanges();


            //Find(m => m.ProcessDefinitionId == processDefinitionId && m.JobHandlerType == TimerStartEventJobHandler.TYPE).ForEach((t) =>
            //{
            //    t.Revision = t.RevisionNext;
            //    t.SuspensionState = suspensionState.StateCode;
            //    DbContext.SaveChanges();
            //});
        }

        public virtual void UpdateJobSuspensionStateByProcessDefinitionKey(string processDefinitionKey, ISuspensionState suspensionState)
        {
            Find(j => j.ProcessDefinitionKey == processDefinitionKey)
                .ForEach((j) =>
                {
                    j.Revision = j.Revision + 1;
                    j.SuspensionState = suspensionState.StateCode;
                });
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["processDefinitionKey"] = processDefinitionKey;
            //parameters["isProcessDefinitionTenantIdSet"] = false;
            //parameters["suspensionState"] = suspensionState.StateCode;
            //DbEntityManager.Update(typeof(JobEntity), "updateJobSuspensionStateByParameters", ConfigureParameterizedQuery(parameters));
        }

        public virtual void UpdateJobSuspensionStateByProcessDefinitionKeyAndTenantId(string processDefinitionKey, string processDefinitionTenantId, ISuspensionState suspensionState)
        {
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["processDefinitionKey"] = processDefinitionKey;
            //parameters["isProcessDefinitionTenantIdSet"] = true;
            //parameters["processDefinitionTenantId"] = processDefinitionTenantId;
            //parameters["suspensionState"] = suspensionState.StateCode;

            //DbEntityManager.Update(typeof(JobEntity), "updateJobSuspensionStateByParameters", ConfigureParameterizedQuery(parameters));
            throw new NotImplementedException("复杂查询，更新");
            //var list = DbEntityManager.SelectList(m => m.ProcessDefinitionKey == processDefinitionKey && m.ProcessDefinitionId != null && m.TenantId != null && m.TenantId == processDefinitionKey);
            //foreach (var item in list)
            //{
            //    item.suspensionState = suspensionState.StateCode;
            //}
            //DbEntityManager.Update(list);
        }

        public virtual void UpdateStartTimerJobSuspensionStateByProcessDefinitionKey(string processDefinitionKey, ISuspensionState suspensionState)
        {
            Find(c => c.ProcessDefinitionKey == processDefinitionKey && c.JobHandlerType == TimerStartEventJobHandler.TYPE)
                .ForEach((d) =>
                {
                    d.Revision = d.Revision + 1;
                    d.SuspensionState = suspensionState.StateCode;
                });
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["processDefinitionKey"] = processDefinitionKey;
            //parameters["isProcessDefinitionTenantIdSet"] = false;
            //parameters["suspensionState"] = suspensionState.StateCode;
            //parameters["handlerType"] = TimerStartEventJobHandler.TYPE;
            //DbEntityManager.Update(typeof(JobEntity), "updateJobSuspensionStateByParameters", ConfigureParameterizedQuery(parameters));
        }

        public virtual void UpdateStartTimerJobSuspensionStateByProcessDefinitionKeyAndTenantId(string processDefinitionKey, string processDefinitionTenantId, ISuspensionState suspensionState)
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["processDefinitionKey"] = processDefinitionKey;
            parameters["isProcessDefinitionTenantIdSet"] = true;
            parameters["processDefinitionTenantId"] = processDefinitionTenantId;
            parameters["suspensionState"] = suspensionState.StateCode;
            parameters["handlerType"] = TimerStartEventJobHandler.TYPE;
            throw new NotImplementedException("复杂查询，更新");
            //DbEntityManager.Update(typeof(JobEntity), "updateJobSuspensionStateByParameters", ConfigureParameterizedQuery(parameters));
        }

        public virtual void UpdateFailedJobRetriesByJobDefinitionId(string jobDefinitionId, int retries)
        {
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["jobDefinitionId"] = jobDefinitionId;
            //parameters["retries"] = retries;
            //DbEntityManager.Update(typeof(JobEntity), "updateFailedJobRetriesByParameters", parameters);
            Find(m => m.RetriesFromPersistence == 0 && m.JobDefinitionId == jobDefinitionId).ForEach((t) =>
            {
                t.Revision = t.RevisionNext;
                t.LockOwner = null;
                t.LockExpirationTime = null;
                t.RetriesFromPersistence = retries;
            });
        }

        public virtual void UpdateJobPriorityByDefinitionId(string jobDefinitionId, long priority)
        {
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["jobDefinitionId"] = jobDefinitionId;
            //parameters["priority"] = priority;

            //DbEntityManager.Update(typeof(JobEntity), "updateJobPriorityByDefinitionId", parameters);
            Find(m => m.JobDefinitionId == jobDefinitionId).ForEach((t) =>
            {
                t.Priority = priority;
            });
        }

        //protected internal virtual void ConfigureQuery(JobQueryImpl query)
        //{
        //    AuthorizationManager.ConfigureJobQuery(query);
        //    TenantManager.ConfigureQuery(query);
        //}

        //protected internal virtual ListQueryParameterObject ConfigureParameterizedQuery(object parameter)
        //{
        //    return TenantManager.ConfigureQuery(parameter);
        //}
        public JobEntity FindJobByHandlerType(String handlerType)
        {
            return First(c => c.JobHandlerType == handlerType);
        }

        public virtual void ReSchedule(JobEntity jobEntity, DateTime newDuedate)
        {
            jobEntity.Init(CommandContext);
            jobEntity.SuspensionState = SuspensionStateFields.Active.StateCode;
            jobEntity.Duedate = newDuedate;
            HintJobExecutorIfNeeded(jobEntity, newDuedate);
        }

        private void HintJobExecutorIfNeeded(JobEntity jobEntity, DateTime duedate)
        {
            // Check if this timer fires before the next time the job executor will check for new timers to fire.
            // This is highly unlikely because normally waitTimeInMillis is 5000 (5 seconds)
            // and timers are usually set further in the future
            JobExecutor jobExecutor = context.Impl.Context.ProcessEngineConfiguration.JobExecutor;
            int waitTimeInMillis = jobExecutor.WaitTimeInMillis;
            if (duedate.Ticks < (ClockUtil.CurrentTime.TimeOfDay.Ticks + waitTimeInMillis))
            {
                HintJobExecutor(jobEntity);
            }
        }

    }

}