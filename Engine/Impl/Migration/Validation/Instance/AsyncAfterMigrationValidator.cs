using ESS.FW.Bpm.Engine.Impl.migration.instance;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instance
{
    public class AsyncAfterMigrationValidator : IMigratingTransitionInstanceValidator
    {
        public virtual void Validate(MigratingTransitionInstance migratingInstance,
            MigratingProcessInstance migratingProcessInstance,
            MigratingTransitionInstanceValidationReportImpl instanceReport)
        {
            var targetActivity = (ActivityImpl) migratingInstance.TargetScope;

            if ((targetActivity != null) && migratingInstance.AsyncAfter)
            {
                var jobInstance = migratingInstance.JobInstance;
                //var config =
                //    (AsyncContinuationJobHandler.AsyncContinuationConfiguration)
                //        jobInstance.JobEntity.JobHandlerConfiguration;
                //var sourceTransitionId = config.TransitionId;

                if (targetActivity.OutgoingTransitions.Count > 1)
                {
                    //if (ReferenceEquals(sourceTransitionId, null))
                    //{
                    //    instanceReport.addFailure("Transition instance is assigned to no sequence flow" +
                    //                              " and target activity has more than one outgoing sequence flow");
                    //}
                    //else
                    //{
                    //    var matchingOutgoingTransition = targetActivity.findOutgoingTransition(sourceTransitionId);
                    //    if (matchingOutgoingTransition == null)
                    //    {
                    //        instanceReport.addFailure("Transition instance is assigned to a sequence flow" +
                    //                                  " that cannot be matched in the target activity");
                    //    }
                    //}
                }
            }
        }
    }
}