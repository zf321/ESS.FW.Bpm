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


    [Serializable]
    public class BatchJobDeclaration : JobDeclaration<MessageEntity>
    {
        protected Type objType = typeof(BatchJobContext);
        public BatchJobDeclaration(string jobHandlerType) : base(jobHandlerType)
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

        protected internal override ExecutionEntity ResolveExecution(object context)
        {
            return null;
        }

        protected internal override MessageEntity NewJobInstance(object context)
        {
            return new MessageEntity();
        }

        protected internal override IJobHandlerConfiguration ResolveJobHandlerConfiguration(object context)
        {
            return new BatchJobConfiguration((context as BatchJobContext).Configuration.Id);
        }

        protected internal override string ResolveJobDefinitionId(object context)
        {
            return (context as BatchJobContext).Batch.BatchJobDefinitionId;
        }
    }
}