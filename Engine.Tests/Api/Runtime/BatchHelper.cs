using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Batch.History;
using ESS.FW.Bpm.Engine.Batch.Impl;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    public abstract class BatchHelper
    {
        protected internal ProcessEngineRule EngineRule;
        protected internal PluggableProcessEngineTestCase TestCase;

        public BatchHelper(ProcessEngineRule engineRule)
        {
            EngineRule = engineRule;
        }

        public BatchHelper(PluggableProcessEngineTestCase testCase)
        {
            TestCase = testCase;
        }

        protected internal virtual IManagementService ManagementService
        {
            get
            {
                if (EngineRule != null)
                    return EngineRule.ManagementService;
                return null;
            }
        }

        protected internal virtual IHistoryService HistoryService
        {
            get
            {
                if (EngineRule != null)
                    return EngineRule.HistoryService;
                return null;
            }
        }

        public virtual IJob GetJobForDefinition(IJobDefinition jobDefinition)
        {
            if (jobDefinition != null)
                return ManagementService.CreateJobQuery(c=>c.JobDefinitionId ==jobDefinition.Id)
                    .First();
            return null;
        }

        public virtual IList<IJob> GetJobsForDefinition(IJobDefinition jobDefinition)
        {
            return ManagementService.CreateJobQuery(c=>c.JobDefinitionId ==jobDefinition.Id)
                
                .ToList();
        }

        public virtual void ExecuteJob(IJob job)
        {
            Assert.NotNull(job, "IJob to execute does not exist");
            try
            {
                ManagementService.ExecuteJob(job.Id);
            }
            catch (BadUserRequestException e)
            {
                throw e;
            }
            catch (System.Exception)
            {
                // ignore
            }
        }

        public virtual IJobDefinition GetSeedJobDefinition(IBatch batch)
        {
            return ManagementService.CreateJobDefinitionQuery(c=>c.Id == batch.SeedJobDefinitionId&&c.JobType==BatchSeedJobHandler.TYPE)
                .First();
        }

        public virtual IJob GetSeedJob(IBatch batch)
        {
            return GetJobForDefinition(GetSeedJobDefinition(batch));
        }

        public virtual void ExecuteSeedJob(IBatch batch)
        {
            ExecuteJob(GetSeedJob(batch));
        }

        public virtual IJobDefinition GetMonitorJobDefinition(IBatch batch)
        {
            return ManagementService.CreateJobDefinitionQuery(c=>c.Id == batch.MonitorJobDefinitionId&&c.JobType==BatchMonitorJobHandler.TYPE)
                .First();
        }

        public virtual IJob GetMonitorJob(IBatch batch)
        {
            return GetJobForDefinition(GetMonitorJobDefinition(batch));
        }

        public virtual void ExecuteMonitorJob(IBatch batch)
        {
            ExecuteJob(GetMonitorJob(batch));
        }

        public virtual void CompleteMonitorJobs(IBatch batch)
        {
            while (GetMonitorJob(batch) != null)
                ExecuteMonitorJob(batch);
        }

        public virtual void CompleteSeedJobs(IBatch batch)
        {
            while (GetSeedJob(batch) != null)
                ExecuteSeedJob(batch);
        }

        public abstract IJobDefinition GetExecutionJobDefinition(IBatch batch);

        public virtual IList<IJob> GetExecutionJobs(IBatch batch)
        {
            return GetJobsForDefinition(GetExecutionJobDefinition(batch));
        }

        public virtual void ExecuteJobs(IBatch batch)
        {
            foreach (var job in GetExecutionJobs(batch))
                ExecuteJob(job);
        }

        public virtual void CompleteBatch(IBatch batch)
        {
            CompleteSeedJobs(batch);
            CompleteExecutionJobs(batch);
            CompleteMonitorJobs(batch);
        }

        public virtual void CompleteJobs(IBatch batch, int Count)
        {
            var jobs = GetExecutionJobs(batch);
            Assert.True(jobs.Count >= Count);
            for (var i = 0; i < Count; i++)
                ExecuteJob(jobs[i]);
        }

        public virtual void FailExecutionJobs(IBatch batch, int Count)
        {
            SetRetries(batch, Count, 0);
        }

        public virtual void SetRetries(IBatch batch, int Count, int retries)
        {
            var jobs = GetExecutionJobs(batch);
            Assert.True(jobs.Count >= Count);

            var managementService = ManagementService;
            for (var i = 0; i < Count; i++)
                managementService.SetJobRetries(jobs[i].Id, retries);
        }

        public virtual void CompleteExecutionJobs(IBatch batch)
        {
            while (GetExecutionJobs(batch)
                       .Count > 0)
                ExecuteJobs(batch);
        }

        public virtual IHistoricBatch GetHistoricBatch(IBatch batch)
        {
            return HistoryService.CreateHistoricBatchQuery()
               // .BatchId(batch.Id)
                .First();
        }

        public virtual IList<IHistoricJobLog> GetHistoricSeedJobLog(IBatch batch)
        {
            return HistoryService.CreateHistoricJobLogQuery()
               // .JobDefinitionId(batch.SeedJobDefinitionId)
                //.OrderPartiallyByOccurrence()
                /*.Asc()*/
                
                .ToList();
        }

        public virtual IList<IHistoricJobLog> GetHistoricMonitorJobLog(IBatch batch)
        {
            return HistoryService.CreateHistoricJobLogQuery()
                //.JobDefinitionId(batch.MonitorJobDefinitionId)
                //.OrderPartiallyByOccurrence()
                /*.Asc()*/
                
                .ToList();
        }

        public virtual IList<IHistoricJobLog> GetHistoricMonitorJobLog(IBatch batch, IJob monitorJob)
        {
            return HistoryService.CreateHistoricJobLogQuery()
                //.JobDefinitionId(batch.MonitorJobDefinitionId)
                //.JobId(monitorJob.Id)
                //.OrderPartiallyByOccurrence()
                /*.Asc()*/
                
                .ToList();
        }

        public virtual IList<IHistoricJobLog> GetHistoricBatchJobLog(IBatch batch)
        {
            return HistoryService.CreateHistoricJobLogQuery()
               // .JobDefinitionId(batch.BatchJobDefinitionId)
                //.OrderPartiallyByOccurrence()
                /*.Asc()*/
                
                .ToList();
        }

        public virtual DateTime AddSeconds(DateTime date, int seconds)
        {
            return new DateTime(date.Ticks + seconds * 1000);
        }

        public virtual DateTime AddSecondsToClock(int seconds)
        {
            var newDate = AddSeconds(ClockUtil.CurrentTime, seconds);
            ClockUtil.CurrentTime = newDate;
            return newDate;
        }

        /// <summary>
        ///     Remove all batches and historic batches. Usually called in <seealso cref="org.junit>" /> method.
        /// </summary>
        public virtual void RemoveAllRunningAndHistoricBatches()
        {
            var historyService = HistoryService;
            var managementService = ManagementService;

            foreach (var batch in managementService.CreateBatchQuery()
                
                .ToList())
                managementService.DeleteBatch(batch.Id, true);

            // remove history of completed batches
            foreach (var historicBatch in historyService.CreateHistoricBatchQuery()
                
                .ToList())
                historyService.DeleteHistoricBatch(historicBatch.Id);
        }
    }
}