using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.DB.EntityManager.Cache;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    ///job执行器的上下文
    /// </summary>
    public class JobExecutorContext
    {
        //public virtual ICollection<string> CurrentProcessorJobQueue { get; } = new LinkedList<string>();
        public virtual Queue<string> CurrentProcessorJobQueue { get; } = new Queue<string>();

        public virtual JobEntity CurrentJob { set; get; }

        public virtual DbEntityCache EntityCache { get; set; }

        public virtual bool ExecutingExclusiveJob => CurrentJob?.Exclusive ?? false;
    }
}