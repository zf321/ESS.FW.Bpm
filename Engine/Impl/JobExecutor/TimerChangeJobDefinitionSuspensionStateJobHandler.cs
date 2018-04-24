using System;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Management.Impl;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using Newtonsoft.Json.Linq;

//using org.camunda.bpm.engine.impl.Util.json;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    ///     
    /// </summary>
    public abstract class TimerChangeJobDefinitionSuspensionStateJobHandler :
        IJobHandler<TimerChangeJobDefinitionSuspensionStateJobHandler.JobDefinitionSuspensionStateConfiguration>
    {
        protected internal const string JobHandlerCfgBy = "by";
        protected internal const string JobHandlerCfgJobDefinitionId = "jobDefinitionId";
        protected internal const string JobHandlerCfgProcessDefinitionId = "processDefinitionId";
        protected internal const string JobHandlerCfgProcessDefinitionKey = "processDefinitionKey";
        protected internal const string JobHandlerCfgProcessDefinitionTenantId = "processDefinitionTenantId";

        protected internal const string JobHandlerCfgIncludeJobs = "includeJobs";
        public abstract string Type { get; }

        public virtual IJobHandlerConfiguration NewConfiguration(string canonicalString)
        {
            var jsonObject = new JObject(canonicalString);

            return JobDefinitionSuspensionStateConfiguration.FromJson(jsonObject);
        }

        public virtual void Execute(IJobHandlerConfiguration configuration, ExecutionEntity execution,
            CommandContext commandContext, string tenantId)
        {
            var cmd = GetCommand((JobDefinitionSuspensionStateConfiguration) configuration);
            cmd.DisableLogUserOperation();
            cmd.Execute(commandContext);
        }

        public virtual void OnDelete(IJobHandlerConfiguration configuration, JobEntity jobEntity)
        {
            // do nothing
        }

        public abstract void OnDelete<T>(T configuration, JobEntity jobEntity);

        public abstract void Execute<T>(T configuration, ExecutionEntity execution, CommandContext commandContext,
            string tenantId);

        protected internal abstract AbstractSetJobDefinitionStateCmd GetCommand(
            JobDefinitionSuspensionStateConfiguration configuration);

        public class JobDefinitionSuspensionStateConfiguration : IJobHandlerConfiguration
        {
            protected internal string By;
            protected internal bool IncludeJobs;
            protected internal bool IsTenantIdSet;

            protected internal string JobDefinitionId;
            protected internal string ProcessDefinitionId;
            protected internal string ProcessDefinitionKey;
            protected internal string TenantId;

            public virtual string ToCanonicalString()
            {
                throw new NotImplementedException();
                //var json = new object();

                //json.Add(JobHandlerCfgBy, By);
                //json.Add(JobHandlerCfgJobDefinitionId, JobDefinitionId);
                //json.Add(JobHandlerCfgProcessDefinitionKey, ProcessDefinitionKey);
                //json.Add(JobHandlerCfgIncludeJobs, IncludeJobs);
                //json.Add(JobHandlerCfgProcessDefinitionId, ProcessDefinitionId);

                //if (IsTenantIdSet)
                //    if (!ReferenceEquals(TenantId, null))
                //        json.Add(JobHandlerCfgProcessDefinitionTenantId, TenantId);
                //    else
                //        json.Add(JobHandlerCfgProcessDefinitionTenantId, null);

                //return json.ToString();
            }

            public virtual UpdateJobDefinitionSuspensionStateBuilderImpl CreateBuilder()
            {
                var builder = new UpdateJobDefinitionSuspensionStateBuilderImpl();

                if (JobHandlerCfgProcessDefinitionId.Equals(By))
                {
                    builder.ByProcessDefinitionId(ProcessDefinitionId);
                }
                else if (JobHandlerCfgJobDefinitionId.Equals(By))
                {
                    builder.ByJobDefinitionId(JobDefinitionId);
                }
                else if (JobHandlerCfgProcessDefinitionKey.Equals(By))
                {
                    builder.ByProcessDefinitionKey(ProcessDefinitionKey);

                    if (IsTenantIdSet)
                        if (!ReferenceEquals(TenantId, null))
                            builder.SetProcessDefinitionTenantId(TenantId);
                        else
                            builder.GetProcessDefinitionWithoutTenantId();
                }
                else
                {
                    throw new ProcessEngineException("Unexpected job handler configuration for property '" +
                                                     JobHandlerCfgBy + "': " + By);
                }

                builder.SetIncludeJobs(IncludeJobs);

                return builder;
            }

            public static JobDefinitionSuspensionStateConfiguration FromJson(Object jsonObject)
            {
                throw new NotImplementedException();
                //var config = new JobDefinitionSuspensionStateConfiguration();

                //config.By = jsonObject.Property(JobHandlerCfgBy).ToString();
                //if (jsonObject.Property(JobHandlerCfgJobDefinitionId) != null)
                //    config.JobDefinitionId = jsonObject.Property(JobHandlerCfgJobDefinitionId).ToString();
                //if (jsonObject.Property(JobHandlerCfgProcessDefinitionId) != null)
                //    config.ProcessDefinitionId = jsonObject.Property(JobHandlerCfgProcessDefinitionId).ToString();
                //if (jsonObject.Property(JobHandlerCfgProcessDefinitionKey) != null)
                //    config.ProcessDefinitionKey = jsonObject.Property(JobHandlerCfgProcessDefinitionKey).ToString();
                //if (jsonObject.Property(JobHandlerCfgProcessDefinitionTenantId) != null)
                //{
                //    config.IsTenantIdSet = true;
                //    if (jsonObject.Property(JobHandlerCfgProcessDefinitionTenantId) != null)
                //        config.TenantId = jsonObject.Property(JobHandlerCfgProcessDefinitionTenantId).ToString();
                //}
                //if (jsonObject.Property(JobHandlerCfgIncludeJobs) != null)
                //    config.IncludeJobs = bool.Parse(jsonObject.Property(JobHandlerCfgIncludeJobs).ToString());

                //return config;
            }

            public static JobDefinitionSuspensionStateConfiguration ByJobDefinitionId(string jobDefinitionId,
                bool includeJobs)
            {
                var configuration = new JobDefinitionSuspensionStateConfiguration();
                configuration.By = JobHandlerCfgJobDefinitionId;
                configuration.JobDefinitionId = jobDefinitionId;
                configuration.IncludeJobs = includeJobs;

                return configuration;
            }

            public static JobDefinitionSuspensionStateConfiguration ByProcessDefinitionId(string processDefinitionId,
                bool includeJobs)
            {
                var configuration = new JobDefinitionSuspensionStateConfiguration();

                configuration.By = JobHandlerCfgProcessDefinitionId;
                configuration.ProcessDefinitionId = processDefinitionId;
                configuration.IncludeJobs = includeJobs;

                return configuration;
            }

            public static JobDefinitionSuspensionStateConfiguration ByProcessDefinitionKey(string processDefinitionKey,
                bool includeJobs)
            {
                var configuration = new JobDefinitionSuspensionStateConfiguration();

                configuration.By = JobHandlerCfgProcessDefinitionKey;
                configuration.ProcessDefinitionKey = processDefinitionKey;
                configuration.IncludeJobs = includeJobs;

                return configuration;
            }

            public static JobDefinitionSuspensionStateConfiguration ByProcessDefinitionKeyAndTenantId(
                string processDefinitionKey, string tenantId, bool includeProcessInstances)
            {
                var configuration = ByProcessDefinitionKey(processDefinitionKey, includeProcessInstances);

                configuration.IsTenantIdSet = true;
                configuration.TenantId = tenantId;

                return configuration;
            }
        }
    }
}