using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance.parser
{
    /// <summary>
    ///     
    /// </summary>
    public class TransitionInstanceJobHandler :
        IMigratingDependentInstanceParseHandler<MigratingTransitionInstance, IList<JobEntity>>
    {
        public virtual void Handle(MigratingInstanceParseContext parseContext,
            MigratingTransitionInstance transitionInstance, IList<JobEntity> elements)
        {
            foreach (var job in elements)
            {
                if (!IsAsyncContinuation(job))
                    continue;

                var targetScope = transitionInstance.TargetScope;
                if (targetScope != null)
                {
                    var targetJobDefinitionEntity =
                        parseContext.GetTargetJobDefinition(transitionInstance.TargetScope.Id, job.JobHandlerType);

                    var migratingJobInstance = new MigratingAsyncJobInstance(job, targetJobDefinitionEntity,
                        transitionInstance.TargetScope);

                    transitionInstance.DependentJobInstance = migratingJobInstance;
                    parseContext.Submit(migratingJobInstance);
                }

                parseContext.Consume(job);
            }
        }

        protected internal static bool IsAsyncContinuation(JobEntity job)
        {
            return (job != null) && AsyncContinuationJobHandler.TYPE.Equals(job.JobHandlerType);
        }
    }
}