using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance
{
    /// <summary>
    ///     
    /// </summary>
    public class MigratingAsyncJobInstance : MigratingJobInstance
    {
        public MigratingAsyncJobInstance(JobEntity jobEntity, JobDefinitionEntity jobDefinitionEntity,
            ScopeImpl targetScope) : base(jobEntity, jobDefinitionEntity, targetScope)
        {
        }


        public virtual bool AsyncAfter
        {
            get
            {
                return true;
                //JobDefinition jobDefinition = jobEntity.JobDefinition;
                //return MessageJobDeclaration.ASYNC_AFTER.Equals(jobDefinition.JobConfiguration);
            }
        }

        public virtual bool AsyncBefore
        {
            get { return !AsyncAfter; }
        }

        protected internal override void migrateJobHandlerConfiguration()
        {
            //var configuration =
            //    (AsyncContinuationJobHandler.AsyncContinuationConfiguration) jobEntity.JobHandlerConfiguration;

            if (AsyncAfter)
            {
                //updateAsyncAfterTargetConfiguration(configuration);
            }
            else
            {
                UpdateAsyncBeforeTargetConfiguration();
            }
        }

        protected internal virtual void UpdateAsyncBeforeTargetConfiguration()
        {
            var targetConfiguration = new AsyncContinuationJobHandler.AsyncContinuationConfiguration();
            //var currentConfiguration =
            //    (AsyncContinuationJobHandler.AsyncContinuationConfiguration) jobEntity.JobHandlerConfiguration;

            //if (PvmAtomicOperation_Fields.PROCESS_START.CanonicalName.Equals(currentConfiguration.AtomicOperation))
            //{
            //    // process start always stays process start
            //    targetConfiguration.AtomicOperation = PvmAtomicOperation_Fields.PROCESS_START.CanonicalName;
            //}
            //else
            //{
            //    if (((ActivityImpl) targetScope).IncomingTransitions.Count == 0)
            //    {
            //        targetConfiguration.AtomicOperation =
            //            PvmAtomicOperation_Fields.ACTIVITY_START_CREATE_SCOPE.CanonicalName;
            //    }
            //    else
            //    {
            //        targetConfiguration.AtomicOperation =
            //            PvmAtomicOperation_Fields.TRANSITION_CREATE_SCOPE.CanonicalName;
            //    }
            //}


            //jobEntity.JobHandlerConfiguration = targetConfiguration;
        }

        protected internal virtual void UpdateAsyncAfterTargetConfiguration(
            AsyncContinuationJobHandler.AsyncContinuationConfiguration currentConfiguration)
        {
            var targetActivity = (ActivityImpl) targetScope;
            var outgoingTransitions = (IList<IPvmTransition>) targetActivity.OutgoingTransitions;

            var targetConfiguration = new AsyncContinuationJobHandler.AsyncContinuationConfiguration();

            if (outgoingTransitions.Count == 0)
            {
                targetConfiguration.AtomicOperation = PvmAtomicOperationFields.ActivityEnd.CanonicalName;
            }
            else
            {
                targetConfiguration.AtomicOperation =
                    PvmAtomicOperationFields.TransitionNotifyListenerTake.CanonicalName;

                if (outgoingTransitions.Count == 1)
                {
                    targetConfiguration.TransitionId = outgoingTransitions[0].Id;
                }
                else
                {
                    TransitionImpl matchingTargetTransition = null;
                    var currentTransitionId = currentConfiguration.TransitionId;
                    if (!ReferenceEquals(currentTransitionId, null))
                        matchingTargetTransition = (TransitionImpl) targetActivity.FindOutgoingTransition(currentTransitionId);

                    if (matchingTargetTransition != null)
                        targetConfiguration.TransitionId = matchingTargetTransition.Id;
                    else
                        throw new ProcessEngineException("Cannot determine matching outgoing sequence flow");
                }
            }

            //jobEntity.JobHandlerConfiguration = targetConfiguration;
        }
    }
}