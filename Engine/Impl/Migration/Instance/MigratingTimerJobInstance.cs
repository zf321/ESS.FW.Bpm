using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance
{
    /// <summary>
    ///     
    /// </summary>
    public class MigratingTimerJobInstance : MigratingJobInstance
    {
        protected internal TimerDeclarationImpl TargetJobDeclaration;

        protected internal ScopeImpl TimerTriggerTargetScope;
        protected internal bool UpdateEvent;

        public MigratingTimerJobInstance(JobEntity jobEntity) : base(jobEntity)
        {
        }

        public MigratingTimerJobInstance(JobEntity jobEntity, JobDefinitionEntity jobDefinitionEntity,
            ScopeImpl targetScope, bool updateEvent, TimerDeclarationImpl targetTimerDeclaration)
            : base(jobEntity, jobDefinitionEntity, targetScope)
        {
            TimerTriggerTargetScope = DetermineTimerTriggerTargetScope(jobEntity, targetScope);
            this.UpdateEvent = updateEvent;
            TargetJobDeclaration = targetTimerDeclaration;
        }

        protected internal virtual ScopeImpl DetermineTimerTriggerTargetScope(JobEntity jobEntity, ScopeImpl targetScope)
        {
            if (TimerStartEventSubprocessJobHandler.TYPE.Equals(jobEntity.JobHandlerType))
                return targetScope.FlowScope;
            return targetScope;
        }

        protected internal override void migrateJobHandlerConfiguration()
        {
            //var configuration = (TimerEventJobHandler.TimerJobConfiguration) jobEntity.JobHandlerConfiguration;
            //configuration.TimerElementKey = timerTriggerTargetScope.Id;
            //jobEntity.JobHandlerConfiguration = configuration;

            //if (updateEvent)
            //{
            //    targetJobDeclaration.updateJob((TimerEntity) jobEntity);
            //}
        }
    }
}