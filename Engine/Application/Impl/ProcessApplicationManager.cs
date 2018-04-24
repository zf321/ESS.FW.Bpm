using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Persistence.Deploy;
using ESS.FW.Common.Extensions;

namespace ESS.FW.Bpm.Engine.Application.Impl
{
    /// <summary>
    ///     
    /// </summary>
    public class ProcessApplicationManager
    {
        public static readonly ProcessApplicationLogger Log = ProcessEngineLogger.ProcessApplicationLogger;

        protected internal IDictionary<string, DefaultProcessApplicationRegistration> RegistrationsByDeploymentId =
            new Dictionary<string, DefaultProcessApplicationRegistration>();

        public virtual string RegistrationSummary
        {
            get
            {
                var builder = new StringBuilder();
                foreach (var entry in RegistrationsByDeploymentId)
                {
                    if (builder.Length > 0)
                        builder.Append(", ");
                    builder.Append(entry.Key);
                    builder.Append("->");
                    builder.Append(entry.Value.Reference.Name);
                }
                return builder.ToString();
            }
        }

        public virtual IProcessApplicationReference GetProcessApplicationForDeployment(string deploymentId)
        {
            var registration = RegistrationsByDeploymentId.ContainsKey(deploymentId)? RegistrationsByDeploymentId[deploymentId]:null;
            if (registration != null)
                return registration.Reference;
            return null;
        }

        public virtual IProcessApplicationRegistration RegisterProcessApplicationForDeployments(
            IList<string> deploymentsToRegister, IProcessApplicationReference reference)
        {
            lock (this)
            {
                // create process application registration
                var registration = CreateProcessApplicationRegistration(deploymentsToRegister, reference);
                // register with job executor
                CreateJobExecutorRegistrations(deploymentsToRegister);
                LogRegistration(deploymentsToRegister, reference);
                return registration;
            }
        }
        //取消了 reference IProcessApplicationReference
        public virtual IProcessApplicationRegistration RegisterProcessApplicationForDeployments(
            string deploymentToRegister, DeploymentEntity deloymentEntity)
        {
            lock (this)
            {
                var registration = CreateProcessApplicationRegistration(new List<string>() { deploymentToRegister }, null);
                // register with job executor
                CreateJobExecutorRegistrations(new List<string>() { deploymentToRegister });
                LogRegistration(new List<string>() { deploymentToRegister }, null);
                return registration;
            }
        }

        public virtual void ClearRegistrations()
        {
            lock (this)
            {
                RegistrationsByDeploymentId.Clear();
            }
        }

        public virtual void UnregisterProcessApplicationForDeployments(IList<string> deploymentIds,
            bool removeProcessesFromCache)
        {
            lock (this)
            {
                RemoveJobExecutorRegistrations(deploymentIds);
                RemoveProcessApplicationRegistration(deploymentIds, removeProcessesFromCache);
            }
        }

        public virtual bool HasRegistrations()
        {
            return !RegistrationsByDeploymentId.IsEmpty(); //.Count > 0;
        }

        protected internal virtual DefaultProcessApplicationRegistration CreateProcessApplicationRegistration(
            IList<string> deploymentsToRegister, IProcessApplicationReference reference)
        {
            var processEngineName = context.Impl.Context.ProcessEngineConfiguration.ProcessEngineName;

            var registration = new DefaultProcessApplicationRegistration(reference, deploymentsToRegister,
                processEngineName);
            // add to registration map
            foreach (var deploymentId in deploymentsToRegister)
                RegistrationsByDeploymentId[deploymentId] = registration;
            return registration;
        }

        protected internal virtual void RemoveProcessApplicationRegistration(IList<string> deploymentIds,
            bool removeProcessesFromCache)
        {
            foreach (var deploymentId in deploymentIds)
                try
                {
                    if (removeProcessesFromCache)
                    {
                        Context.ProcessEngineConfiguration.DeploymentCache.RemoveDeployment(deploymentId);
                    }
                }
                catch (System.Exception t)
                {
                    Log.CouldNotRemoveDefinitionsFromCache(t);
                }
                finally
                {
                    if (!ReferenceEquals(deploymentId, null))
                        RegistrationsByDeploymentId.Remove(deploymentId);
                }
        }

        protected internal virtual void CreateJobExecutorRegistrations(IList<string> deploymentIds)
        {
            try
            {
                Context.CommandContext.TransactionContext.AddTransactionListener(TransactionJavaStatus.RolledBack,
                    new DeploymentFailListener(deploymentIds));

                var registeredDeployments = Context.ProcessEngineConfiguration.RegisteredDeployments;
                registeredDeployments.AddAll(deploymentIds);
            }
            catch (System.Exception e)
            {
                throw Log.ExceptionWhileRegisteringDeploymentsWithJobExecutor(e);
            }
        }

        protected internal virtual void RemoveJobExecutorRegistrations(IList<string> deploymentIds)
        {
            try
            {
                var registeredDeployments = context.Impl.Context.ProcessEngineConfiguration.RegisteredDeployments;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.Util.Collection 'removeAll' method:
                //registeredDeployments.removeAll(deploymentIds);
                foreach (var it in deploymentIds)
                    registeredDeployments.Remove(it);
            }
            catch (System.Exception e)
            {
                Log.ExceptionWhileUnregisteringDeploymentsWithJobExecutor(e);
            }
        }

        // logger ////////////////////////////////////////////////////////////////////////////

        protected internal virtual void LogRegistration(IList<string> deploymentIds,
            IProcessApplicationReference reference)
        {
            if (!Log.InfoEnabled)
                return;

            try
            {
                var builder = new StringBuilder();
                builder.Append("ProcessApplication '");
                builder.Append(reference.Name);
                builder.Append("' registered for DB deployments ");
                builder.Append(deploymentIds);
                builder.Append(". ");

                IList<IProcessDefinition> processDefinitions = new List<IProcessDefinition>();
                IList<ICaseDefinition> caseDefinitions = new List<ICaseDefinition>();

                var commandContext = Context.CommandContext;
                var processEngineConfiguration = context.Impl.Context.ProcessEngineConfiguration;
                var cmmnEnabled = processEngineConfiguration.CmmnEnabled;

                foreach (var deploymentId in deploymentIds)
                {
                    //DeploymentEntity deployment = commandContext.Get(typeof (DeploymentEntity),
                    //    deploymentId);

                    //if (deployment != null)
                    //{
                    //    ((List<ProcessDefinition>) processDefinitions).AddRange(
                    //        getDeployedProcessDefinitionArtifacts(deployment));

                    //    if (cmmnEnabled)
                    //    {
                    //        ((List<CaseDefinition>) caseDefinitions).AddRange(
                    //            getDeployedCaseDefinitionArtifacts(deployment));
                    //    }
                    //}
                }

                LogProcessDefinitionRegistrations(builder, processDefinitions);

                if (cmmnEnabled)
                    LogCaseDefinitionRegistrations(builder, caseDefinitions);

                Log.RegistrationSummary(builder.ToString());
            }
            catch (System.Exception e)
            {
                Log.ExceptionWhileLoggingRegistrationSummary(e);
            }
        }

        protected internal virtual IList<IProcessDefinition> GetDeployedProcessDefinitionArtifacts(
            DeploymentEntity deployment)
        {
            throw new NotImplementedException();
            var commandContext = Context.CommandContext;

            // in case deployment was created by this command
            //IList<ProcessDefinition> entities = deployment.DeployedProcessDefinitions;

            //if (entities == null)
            //{
            //    string deploymentId = deployment.Id;
            //    ProcessDefinitionManager manager = commandContext.ProcessDefinitionManager;
            //    return manager.findProcessDefinitionsByDeploymentId(deploymentId);
            //}

            return null;
            ;
        }

        protected internal virtual IList<ICaseDefinition> GetDeployedCaseDefinitionArtifacts(DeploymentEntity deployment)
        {
            var commandContext = Context.CommandContext;

             //in case deployment was created by this command
            IList<ICaseDefinition> entities = deployment.DeployedCaseDefinitions;

            if (entities == null)
            {
                string deploymentId = deployment.Id;
                //ICaseDefinitionManager caseDefinitionManager = commandContext.CaseDefinitionManager;
                //return caseDefinitionManager.findCaseDefinitionByDeploymentId(deploymentId);
            }

            return null;
        }

        protected internal virtual void LogProcessDefinitionRegistrations(StringBuilder builder,
            IList<IProcessDefinition> processDefinitions)
        {
            if (processDefinitions.Count == 0)
            {
                builder.Append("Deployment does not provide any process definitions.");
            }
            else
            {
                builder.Append("Will execute process definitions ");
                builder.Append("\n");
                foreach (var processDefinition in processDefinitions)
                {
                    builder.Append("\n");
                    builder.Append("        ");
                    builder.Append(processDefinition.Key);
                    builder.Append("[version: ");
                    builder.Append(processDefinition.Version);
                    builder.Append(", id: ");
                    builder.Append(processDefinition.Id);
                    builder.Append("]");
                }
                builder.Append("\n");
            }
        }

        protected internal virtual void LogCaseDefinitionRegistrations(StringBuilder builder,
            IList<ICaseDefinition> caseDefinitions)
        {
            if (caseDefinitions.Count == 0)
            {
                builder.Append("Deployment does not provide any case definitions.");
            }
            else
            {
                builder.Append("\n");
                builder.Append("Will execute case definitions ");
                builder.Append("\n");
                foreach (var caseDefinition in caseDefinitions)
                {
                    builder.Append("\n");
                    builder.Append("        ");
                    builder.Append(caseDefinition.Key);
                    builder.Append("[version: ");
                    builder.Append(caseDefinition.Version);
                    builder.Append(", id: ");
                    builder.Append(caseDefinition.Id);
                    builder.Append("]");
                }
                builder.Append("\n");
            }
        }
    }
}