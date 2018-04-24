using System;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository.Impl;
using Newtonsoft.Json.Linq;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    ///     
    ///     
    /// </summary>
    public abstract class TimerChangeProcessDefinitionSuspensionStateJobHandler :
        IJobHandler<TimerChangeProcessDefinitionSuspensionStateJobHandler.ProcessDefinitionSuspensionStateConfiguration>
    {
        protected internal const string JobHandlerCfgBy = "by";
        protected internal const string JobHandlerCfgProcessDefinitionId = "processDefinitionId";
        protected internal const string JobHandlerCfgProcessDefinitionKey = "processDefinitionKey";
        protected internal const string JobHandlerCfgProcessDefinitionTenantId = "processDefinitionTenantId";

        protected internal const string JobHandlerCfgIncludeProcessInstances = "includeProcessInstances";
        public abstract string Type { get; }

        public virtual void Execute(IJobHandlerConfiguration configuration,
            ExecutionEntity execution, CommandContext commandContext, string tenantId)
        {
            var cmd = GetCommand((ProcessDefinitionSuspensionStateConfiguration) configuration);
            cmd.DisableLogUserOperation();
            cmd.Execute(commandContext);
        }

        public virtual IJobHandlerConfiguration NewConfiguration(string canonicalString)
        {
            throw new NotImplementedException();
            //var jsonObject = new JObject(canonicalString);

            //return ProcessDefinitionSuspensionStateConfiguration.FromJson(jsonObject);
        }

        public virtual void OnDelete(IJobHandlerConfiguration configuration, JobEntity jobEntity)
        {
            // do nothing
        }

        public abstract void OnDelete<T>(T configuration, JobEntity jobEntity);

        public abstract void Execute<T>(T configuration, ExecutionEntity execution, CommandContext commandContext,
            string tenantId);

        protected internal abstract AbstractSetProcessDefinitionStateCmd GetCommand(
            ProcessDefinitionSuspensionStateConfiguration configuration);

        public class ProcessDefinitionSuspensionStateConfiguration : IJobHandlerConfiguration
        {
            protected internal string By;
            protected internal bool IncludeProcessInstances;
            protected internal bool IsTenantIdSet;
            protected internal string ProcessDefinitionId;

            protected internal string ProcessDefinitionKey;
            protected internal string TenantId;

            public virtual string ToCanonicalString()
            {
                //throw new NotImplementedException();
                //var json = new JObject();
                JObject json = new JObject();
                
                json.Add(JobHandlerCfgBy, By);
                json.Add(JobHandlerCfgProcessDefinitionKey, ProcessDefinitionKey);
                json.Add(JobHandlerCfgIncludeProcessInstances, IncludeProcessInstances);
                json.Add(JobHandlerCfgProcessDefinitionId, ProcessDefinitionId);

                if (IsTenantIdSet)
                    if (!ReferenceEquals(TenantId, null))
                        json.Add(JobHandlerCfgProcessDefinitionTenantId, TenantId);
                    else
                        json.Add(JobHandlerCfgProcessDefinitionTenantId, null);

                return json.ToString();
            }

            public virtual UpdateProcessDefinitionSuspensionStateBuilderImpl CreateBuilder()
            {
                var builder = new UpdateProcessDefinitionSuspensionStateBuilderImpl();

                if (By.Equals(JobHandlerCfgProcessDefinitionId))
                {
                    builder.ByProcessDefinitionId(ProcessDefinitionId);
                }
                else if (By.Equals(JobHandlerCfgProcessDefinitionKey))
                {
                    builder.byProcessDefinitionKey(ProcessDefinitionKey);

                    if (IsTenantIdSet)
                        if (!ReferenceEquals(TenantId, null))
                            builder.processDefinitionTenantId(TenantId);
                        else
                            builder.processDefinitionWithoutTenantId();
                }
                else
                {
                    throw new ProcessEngineException("Unexpected job handler configuration for property '" +
                                                     JobHandlerCfgBy + "': " + By);
                }

                builder.includeProcessInstances(IncludeProcessInstances);

                return builder;
            }

            public static ProcessDefinitionSuspensionStateConfiguration FromJson(object jsonObject)
            {
                throw new NotImplementedException();
                //var config = new ProcessDefinitionSuspensionStateConfiguration();

                //config.By = jsonObject.Property(JobHandlerCfgBy).ToString();
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
                //if (jsonObject.Property(JobHandlerCfgIncludeProcessInstances) != null)
                //    config.IncludeProcessInstances =
                //        bool.Parse(jsonObject.Property(JobHandlerCfgIncludeProcessInstances).ToString());

                //return config;
            }

            public static ProcessDefinitionSuspensionStateConfiguration ByProcessDefinitionId(
                string processDefinitionId, bool includeProcessInstances)
            {
                var configuration = new ProcessDefinitionSuspensionStateConfiguration();

                configuration.By = JobHandlerCfgProcessDefinitionId;
                configuration.ProcessDefinitionId = processDefinitionId;
                configuration.IncludeProcessInstances = includeProcessInstances;

                return configuration;
            }

            public static ProcessDefinitionSuspensionStateConfiguration ByProcessDefinitionKey(
                string processDefinitionKey, bool includeProcessInstances)
            {
                var configuration = new ProcessDefinitionSuspensionStateConfiguration();

                configuration.By = JobHandlerCfgProcessDefinitionKey;
                configuration.ProcessDefinitionKey = processDefinitionKey;
                configuration.IncludeProcessInstances = includeProcessInstances;

                return configuration;
            }

            public static ProcessDefinitionSuspensionStateConfiguration ByProcessDefinitionKeyAndTenantId(
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