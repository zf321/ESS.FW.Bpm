using System;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping.value;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Batch.Impl
{
    //using JobDeclaration = org.camunda.bpm.engine.impl.jobexecutor.JobDeclaration;

    //using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
    //using MessageEntity = org.camunda.bpm.engine.impl.persistence.entity.MessageEntity;

    /// <summary>
    ///     Job declaration for batch seed jobs. The batch seed job
    ///     creates all batch jobs required to complete the batch.
    /// </summary>
    [Serializable]
    public class BatchSeedJobDeclaration : JobDeclaration<MessageEntity>
    {
        protected Type objType = typeof(BatchEntity);

        public BatchSeedJobDeclaration() : base(BatchSeedJobHandler.TYPE)
        {
        }

        public override IParameterValueProvider JobPriorityProvider
        {
            get
            {
                var batchJobPriority = Context.ProcessEngineConfiguration.BatchJobPriority;
                return new ConstantValueProvider(batchJobPriority);
            }
        }

        protected internal override ExecutionEntity ResolveExecution(object batch)
        {
            return null;
        }

        protected internal override MessageEntity NewJobInstance(object batch)
        {
            return new MessageEntity();
        }

        protected internal override IJobHandlerConfiguration ResolveJobHandlerConfiguration(object batch)
        {
            return new BatchSeedJobHandler.BatchSeedJobConfiguration((batch as BatchEntity).Id);
        }

        protected internal override string ResolveJobDefinitionId(object batch)
        {
            return (batch as BatchEntity).SeedJobDefinitionId;
        }
    }
}