using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Common.Extensions;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    public class JobAcquisitionContext
    {
        
        /// <summary>
        /// obs that were rejected from execution in the acquisition cycle
        ///     due to lacking execution resources.
        ///     With an execution thread pool, these jobs could not be submitted due to
        ///     saturation of the underlying job queue.
        /// </summary>
        public IDictionary<string, IList<IList<string>>> RejectedJobBatchesByEngine { get; protected internal set; }

        /// <summary>
        /// Jobs that have been acquired in previous cycles and are supposed to
        ///     be re-submitted for execution
        /// </summary>
        public IDictionary<string, IList<IList<string>>> AdditionalJobBatchesByEngine { get; protected internal set; }

        /// <summary>
        /// Jobs that were acquired in the current acquisition cycle.
        /// </summary>
        public IDictionary<string, AcquiredJobs> AcquiredJobsByEngine { get; protected internal set; }

        public JobAcquisitionContext()
        {
            RejectedJobBatchesByEngine = new Dictionary<string, IList<IList<string>>>();
            AdditionalJobBatchesByEngine = new Dictionary<string, IList<IList<string>>>();
            AcquiredJobsByEngine = new Dictionary<string, AcquiredJobs>();
        }

        public virtual void SubmitRejectedBatch(string engineName, IList<string> jobIds)
        {
            CollectionUtil.AddToMapOfLists(RejectedJobBatchesByEngine, engineName, jobIds);
        }

        public virtual void SubmitAcquiredJobs(string engineName, AcquiredJobs acquiredJobs)
        {
            AcquiredJobsByEngine[engineName] = acquiredJobs;
        }

        public virtual void SubmitAdditionalJobBatch(string engineName, IList<string> jobIds)
        {
            CollectionUtil.AddToMapOfLists(AdditionalJobBatchesByEngine, engineName, jobIds);
        }

        public virtual void Reset()
        {
            AdditionalJobBatchesByEngine.Clear();

            // jobs that were rejected in the previous acquisition cycle
            // are to be resubmitted for execution in the current cycle
            RejectedJobBatchesByEngine.ForEach(d =>
            {
                AdditionalJobBatchesByEngine.Add(d.Key, d.Value);
            });

            RejectedJobBatchesByEngine.Clear();
            AcquiredJobsByEngine.Clear();
            AcquisitionException = null;
            AcquisitionTime = 0;
            JobAdded = false;
        }

        /// <returns> true, if for all engines there were less jobs acquired than requested </returns>
        public virtual bool AreAllEnginesIdle()
        {
            foreach (var acquiredJobs in AcquiredJobsByEngine.Values)
            {
                var jobsAcquired = acquiredJobs.JobIdBatches.Count + acquiredJobs.NumberOfJobsFailedToLock;

                if (jobsAcquired >= acquiredJobs.NumberOfJobsAttemptedToAcquire)
                    return false;
            }
            return true;
        }

        /// <summary>
        ///  true if at least one job could not be locked, regardless of engine
        /// </summary>
        public virtual bool HasJobAcquisitionLockFailureOccurred()
        {
            return AcquiredJobsByEngine.Values.Any(acquiredJobs => acquiredJobs.NumberOfJobsFailedToLock > 0);
        }

        public virtual long AcquisitionTime { get; set; }

        public virtual System.Exception AcquisitionException { get; set; }

        public virtual bool JobAdded { get; set; }
        
    }
}