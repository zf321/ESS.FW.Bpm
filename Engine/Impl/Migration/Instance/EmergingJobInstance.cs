using System;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance
{
    /// <summary>
    ///     
    /// </summary>
    public class EmergingJobInstance : IEmergingInstance
    {
        protected internal TimerDeclarationImpl TimerDeclaration;

        public EmergingJobInstance(TimerDeclarationImpl timerDeclaration)
        {
            this.TimerDeclaration = timerDeclaration;
        }

        public virtual void Create(ExecutionEntity scopeExecution)
        {
           throw new NotImplementedException();
            //TimerDeclaration.createTimer(scopeExecution);
        }
    }
}