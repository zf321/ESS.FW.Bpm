using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance.parser
{
    /// <summary>
    ///     
    /// </summary>
    public class ActivityInstanceJobHandler :
        IMigratingDependentInstanceParseHandler<MigratingActivityInstance, IList<JobEntity>>
    {
        public virtual void Handle(MigratingInstanceParseContext parseContext,
            MigratingActivityInstance activityInstance, IList<JobEntity> elements)
        {
            var timerDeclarationsInEventScope = GetTimerDeclarationsByTriggeringActivity(activityInstance.TargetScope);

            foreach (var job in elements)
            {
                if (!IsTimerJob(job))
                    continue;

                //var migrationInstruction = parseContext.findSingleMigrationInstruction(job.ActivityId);
                //var targetActivity = parseContext.getTargetActivity(migrationInstruction);

                //if (targetActivity != null && activityInstance.migratesTo(targetActivity.EventScope))
                //{
                //    // the timer job is migrated
                //    JobDefinitionEntity targetJobDefinitionEntity =
                //        parseContext.getTargetJobDefinition(targetActivity.ActivityId, job.JobHandlerType);

                //    TimerDeclarationImpl targetTimerDeclaration = timerDeclarationsInEventScope.Remove(targetActivity.Id);

                //    MigratingJobInstance migratingTimerJobInstance = new MigratingTimerJobInstance(job,
                //        targetJobDefinitionEntity, targetActivity, migrationInstruction.UpdateEventTrigger,
                //        targetTimerDeclaration);
                //    activityInstance.addMigratingDependentInstance(migratingTimerJobInstance);
                //    parseContext.submit(migratingTimerJobInstance);
                //}
                //else
                //{
                //    // the timer job is removed
                //    MigratingJobInstance removingJobInstance = new MigratingTimerJobInstance(job);
                //    activityInstance.addRemovingDependentInstance(removingJobInstance);
                //    parseContext.submit(removingJobInstance);
                //}

                parseContext.Consume(job);
            }

            if (activityInstance.Migrates())
                AddEmergingTimerJobs(activityInstance, timerDeclarationsInEventScope.Values);
        }

        protected internal static bool IsTimerJob(JobEntity job)
        {
            // return job != null && job.Type.Equals(TimerEntity.TYPE);
            return true;
        }

        protected internal virtual void AddEmergingTimerJobs(MigratingActivityInstance owningInstance,
            ICollection<TimerDeclarationImpl> emergingDeclarations)
        {
            foreach (var timerDeclaration in emergingDeclarations)
                owningInstance.AddEmergingDependentInstance(new EmergingJobInstance(timerDeclaration));
        }

        protected internal virtual IDictionary<string, TimerDeclarationImpl> GetTimerDeclarationsByTriggeringActivity(
            ScopeImpl scope)
        {
            return new Dictionary<string, TimerDeclarationImpl>(TimerDeclarationImpl.GetDeclarationsForScope(scope));
        }
    }
}