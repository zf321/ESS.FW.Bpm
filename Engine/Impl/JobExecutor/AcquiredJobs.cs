using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    ///      
    ///     
    /// </summary>
    public class AcquiredJobs
    {
        private IList<IList<string>> _acquiredJobBatches = new List<IList<string>>();
        private ISet<string> _acquiredJobs = new HashSet<string>();

       private  int _numberOfJobsAttemptedToAcquire;

        private int _numberOfJobsFailedToLock;

        public AcquiredJobs(int numberOfJobsAttemptedToAcquire)
        {
            this._numberOfJobsAttemptedToAcquire = numberOfJobsAttemptedToAcquire;
        }

        public virtual IList<IList<string>> JobIdBatches => _acquiredJobBatches;

        public virtual void AddJobIdBatch(IList<string> jobIds)
        {
            if (jobIds.Count > 0)
            {
                _acquiredJobBatches.Add(jobIds);
                _acquiredJobs.UnionWith(jobIds);
            }
        }

        public virtual void AddJobIdBatch(string jobId)
        {
            var list = new List<string> {jobId};
            AddJobIdBatch(list);
        }

        public virtual bool Contains(string jobId)
        {
            return _acquiredJobs.Contains(jobId);
        }

        public virtual int Size()
        {
            return _acquiredJobs.Count;
        }

        public virtual void RemoveJobId(string id)
        {
            _numberOfJobsFailedToLock++;

            _acquiredJobs.Remove(id);

            for (int i = _acquiredJobBatches.Count - 1; i >= 0; i--)
            {
                _acquiredJobBatches[i].Remove(id);
                if (_acquiredJobBatches[i].Count == 0)
                    _acquiredJobBatches.Remove(_acquiredJobBatches[i]);
            }            
        }
        
        public virtual int NumberOfJobsFailedToLock => _numberOfJobsFailedToLock;

        public virtual int NumberOfJobsAttemptedToAcquire => _numberOfJobsAttemptedToAcquire;
    }
}