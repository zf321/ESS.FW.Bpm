using System;
using System.Collections.Generic;
using System.IO;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Batch.Impl
{


    /// <summary>
    ///     Common methods for batch job handlers based on list of ids, providing serialization, configuration instantiation,
    ///     etc.
    ///     
    /// </summary>
    public abstract class AbstractBatchJobHandler<BatchConfiguration> : IBatchJobHandler
    {
        //protected internal abstract DeleteHistoricProcessInstanceBatchConfigurationJsonConverter JsonConverterInstance {
         //   get; }

        public virtual string Type
        {
            get { throw new NotImplementedException(); }
        }

        public abstract IJobDeclaration JobDeclaration { get; }

        //public abstract JobDeclaration<BatchJobContext, MessageEntity> JobDeclaration {get;}

        public virtual bool CreateJobs(BatchEntity batch)
        {
            throw new NotImplementedException();
            var commandContext = Context.CommandContext;
            //ByteArrayManager byteArrayManager = commandContext.ByteArrayManager;
            //JobManager jobManager = commandContext.JobManager;

            //var configuration = readConfiguration(batch.ConfigurationBytes);

            var batchJobsPerSeed = batch.BatchJobsPerSeed;
            var invocationsPerBatchJob = batch.InvocationsPerBatchJob;

            //var ids = configuration.Ids;
            //var numberOfItemsToProcess = Math.Min(invocationsPerBatchJob*batchJobsPerSeed, ids.Count);
            // view of process instances to process
            //IList<string> processIds = ids.subList(0, numberOfItemsToProcess);

            var createdJobs = 0;
            //while (processIds.Count > 0)
            //{
            //  int lastIdIndex = Math.Min(invocationsPerBatchJob, processIds.Count);
            //  // view of process instances for this job
            //  IList<string> idsForJob = processIds.subList(0, lastIdIndex);

            //  T jobConfiguration = createJobConfiguration(configuration, idsForJob);
            //  ResourceEntity configurationEntity = saveConfiguration(byteArrayManager, jobConfiguration);

            //  JobEntity job = createBatchJob(batch, configurationEntity);
            //  postProcessJob(configuration, job);
            //  jobManager.insertAndHintJobExecutor(job);

            //  idsForJob.Clear();
            //  createdJobs++;
            //}

            // update created jobs for batch
            batch.JobsCreated = batch.JobsCreated + createdJobs;

            // update batch configuration
            //batch.ConfigurationBytes = writeConfiguration(configuration);

            //return ids.Count == 0;
            return true;
        }

        public virtual void DeleteJobs(BatchEntity batch)
        {
            //IList<JobEntity> jobs = Context.CommandContext.JobManager.findJobsByJobDefinitionId(batch.BatchJobDefinitionId);

            //foreach (JobEntity job in jobs)
            //{
            //  //job.delete();
            //}
        }

        public IJobHandlerConfiguration NewConfiguration(string canonicalString)
        {
            return new BatchJobConfiguration(canonicalString);
        }

        public void OnDelete(IJobHandlerConfiguration configuration, JobEntity jobEntity)
        {
            var byteArrayId = ((BatchJobConfiguration)configuration).ConfigurationByteArrayId;
            if (!ReferenceEquals(byteArrayId, null))
            {
                //Context.CommandContext.ByteArrayManager.deleteByteArrayById(byteArrayId);
            }
        }

        // protected internal virtual ResourceEntity saveConfiguration(ByteArrayManager byteArrayManager, T jobConfiguration)
        // {
        //ResourceEntity configurationEntity = new ResourceEntity();
        //configurationEntity.Bytes = writeConfiguration(jobConfiguration);
        //byteArrayManager.insert(configurationEntity);
        //return configurationEntity;
        // }

        public virtual byte[] WriteConfiguration(IJobHandlerConfiguration configuration)
        {
            //JSONObject jsonObject = JsonConverterInstance.toJsonObject(configuration);

            var outStream = new MemoryStream();
            //Writer writer = StringUtil.writerForStream(outStream);

            //jsonObject.write(writer);
            //IoUtil.flushSilently(writer);

            return outStream.ToArray();
        }

        public virtual IJobHandlerConfiguration ReadConfiguration(byte[] serializedConfiguration)
        {
            //Reader jsonReader = StringUtil.readerFromBytes(serializedConfiguration);
            //return JsonConverterInstance.toObject(new JSONObject(new JSONTokener(jsonReader)));
            return default(IJobHandlerConfiguration);
        }

        public virtual void Execute(IJobHandlerConfiguration configuration, ExecutionEntity execution,
            CommandContext commandContext, string tenantId)
        {
            throw new NotImplementedException();
        }

        protected internal abstract IJobHandlerConfiguration CreateJobConfiguration(IJobHandlerConfiguration configuration, IList<string> processIdsForJob);

        protected internal virtual void PostProcessJob(IJobHandlerConfiguration configuration, JobEntity job)
        {
            // do nothing as default
        }


        protected internal virtual JobEntity CreateBatchJob(BatchEntity batch, ResourceEntity configuration)
        {
            //BatchJobContext creationContext = new BatchJobContext(batch, configuration);
            //return JobDeclaration.createJobInstance(creationContext);
            return null;
        }
    }
}