using System;
using System.Collections.Generic;
using System.Globalization;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.DB.EntityManager;
using ESS.FW.Bpm.Engine.Impl.DB.EntityManager.Operation;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Variable;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    ///     
    /// </summary>
    public class AcquireJobsCmd : ICommand<AcquiredJobs>, IOptimisticLockingListener
    {
        private readonly JobExecutor.JobExecutor _jobExecutor;

        protected internal AcquiredJobs AcquiredJobs;
        protected internal int NumJobsToAcquire;
        private static bool _isRunning = false;

        public AcquireJobsCmd(JobExecutor.JobExecutor jobExecutor) : this(jobExecutor, jobExecutor.MaxJobsPerAcquisition)
        {
        }

        public AcquireJobsCmd(JobExecutor.JobExecutor jobExecutor, int numJobsToAcquire)
        {
            this._jobExecutor = jobExecutor;
            this.NumJobsToAcquire = numJobsToAcquire;
        }

        public virtual AcquiredJobs Execute(CommandContext commandContext)
        {
            AcquiredJobs = new AcquiredJobs(NumJobsToAcquire);
            if (!_isRunning)
            {
                _isRunning = true;

                IList<JobEntity> jobs = commandContext.JobManager.FindNextJobsToExecute(new Page(0, NumJobsToAcquire));

                IDictionary<string, IList<string>> exclusiveJobsByProcessInstance = new Dictionary<string, IList<string>>();

                foreach (var job in jobs)
                {
                    LockJob(job);

                    if (job.Exclusive)
                    {
                        // java中null值可以作为字典的key
                        var list = exclusiveJobsByProcessInstance.GetValueOrNull(job.ProcessInstanceId ?? "null");
                        if (list == null)
                        {
                            list = new List<string>();
                            exclusiveJobsByProcessInstance[job.ProcessInstanceId ?? "null"] = list;
                        }
                        list.Add(job.Id);
                    }
                    else
                    {
                        AcquiredJobs.AddJobIdBatch(job.Id);
                    }
                }

                foreach (var jobIds in exclusiveJobsByProcessInstance.Values)
                    AcquiredJobs.AddJobIdBatch(jobIds);

                // register an OptimisticLockingListener which is notified about jobs which cannot be acquired.
                // the listener removes them from the list of acquired jobs.
                commandContext.RegisterOptimisticLockingListener(this);

                _isRunning = false;
            }
            return AcquiredJobs;
        }

        protected internal virtual void LockJob(JobEntity job)
        {
            var lockOwner = _jobExecutor.LockOwner;
            job.LockOwner = lockOwner;

            var lockTimeInMillis = _jobExecutor.LockTimeInMillis;

            DateTime d = new DateTime(ClockUtil.CurrentTime.Ticks);
            d = d.AddMilliseconds(lockTimeInMillis);
            job.LockExpirationTime = d;

            //var gregorianCalendar = new GregorianCalendar();
            //gregorianCalendar.Time = ClockUtil.CurrentTime;
            //gregorianCalendar.add(DateTime.MILLISECOND, lockTimeInMillis);
            //job.LockExpirationTime = gregorianCalendar.Time;
        }

        public virtual Type EntityType => typeof(JobEntity);

        public virtual void FailedOperation(DbOperation operation)
        {
            var dbEntityOperation = operation as DbEntityOperation;
            if (dbEntityOperation == null) return;
            var entityOperation = dbEntityOperation;
            if (entityOperation.EntityType.IsSubclassOf(typeof(JobEntity)))
                AcquiredJobs.RemoveJobId(entityOperation.Entity.Id);
        }

    }
}