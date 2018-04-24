using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Repository.Impl;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Engine.Common.Utils;
using ESS.FW.Bpm.Engine.Persistence.Deploy;
using System.Text;
using System.Transactions;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Deployer;
using ESS.FW.Bpm.Engine.Persistence.Entity.Impl;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    [Serializable]
    public class DeployCmd : ICommand<IDeploymentWithDefinitions>
    {
        private const long SerialVersionUid = 1L;

        private static readonly CommandLogger Log = ProcessEngineLogger.CmdLogger;
        private static readonly TransactionLogger TxLog = ProcessEngineLogger.TxLogger;

        protected internal DeploymentBuilderImpl deploymentBuilder;

        public DeployCmd(DeploymentBuilderImpl deploymentBuilder)
        {
            this.deploymentBuilder = deploymentBuilder;
        }

        public virtual IDeploymentWithDefinitions Execute(CommandContext commandContext)
        {
            // ensure serial processing of multiple deployments on the same node.
            // We experienced deadlock situations with highly concurrent deployment of multiple
            // applications on Jboss & Wildfly
            lock (typeof(IProcessEngine))
            {
                return DoExecute(commandContext);
            }
        }

        /// <summary>
        /// 部署执行
        /// </summary>
        /// <param name="commandContext"></param>
        /// <returns></returns>
        protected internal virtual IDeploymentWithDefinitions DoExecute(CommandContext commandContext)
        {
            IDeploymentManager deploymentManager = commandContext.DeploymentManager;

            var deploymentIds = GetAllDeploymentIds(deploymentBuilder);
            if (deploymentIds.Count > 0)
            {
                var deploymentIdArray = deploymentIds.ToArray();
                IList<DeploymentEntity> deployments = deploymentManager.FindDeploymentsByIds(deploymentIdArray);
                EnsureDeploymentsWithIdsExists(deploymentIds, deployments);
            }

            CheckCreateAndReadDeployments(commandContext, deploymentIds);

            // set deployment name if it should retrieved from an existing deployment
            var nameFromDeployment = deploymentBuilder.GetNameFromDeployment();
            SetDeploymentName(nameFromDeployment, deploymentBuilder, commandContext);
            //TODO 加载resources到上下文
            // get resources to re-deploy
            var resources = GetResources(deploymentBuilder, commandContext);
            // .. and add them the builder
            AddResources(resources, deploymentBuilder);

            var resourceNames = deploymentBuilder.ResourceNames;
            if ((resourceNames == null) || (resourceNames.Count == 0))
                throw new NotValidException("No deployment resources contained to deploy.");
            IDeploymentWithDefinitions dep = commandContext.RunWithoutAuthorization<IDeploymentWithDefinitions>(() =>
            {
                AcquireExclusiveLock(commandContext);
                DeploymentEntity deployment = InitDeployment();
                //准备部署的xml数据源 byte[]
                IDictionary<string, ResourceEntity> resourcesToDeploy = ResolveResourcesToDeploy(commandContext, deployment);
                //准备忽略的resource
                IDictionary<string, ResourceEntity> resourcesToIgnore = new Dictionary<string, ResourceEntity>(deployment.Resources);
                foreach (var key in resourcesToDeploy.Keys)
                {
                    if (resourcesToIgnore.ContainsKey(key))
                    {
                        resourcesToIgnore.Remove(key);
                    }
                }

                if (resourcesToDeploy.Count > 0)
                {
                    Log.DebugCreatingNewDeployment();
                    deployment.Resources = resourcesToDeploy;
                    Deploy(deployment);
                }
                else
                {
                    Log.UsingExistingDeployment();
                    deployment = GetExistingDeployment(commandContext, deployment.Name);
                }

                ScheduleProcessDefinitionActivation(commandContext, deployment);

                if (deploymentBuilder is IProcessApplicationDeploymentBuilder)
                {
                    // for process application deployments, job executor registration is managed by
                    // process application manager
                    ISet<string> processesToRegisterFor = RetrieveProcessKeysFromResources(resourcesToIgnore);
                    IProcessApplicationRegistration registration = RegisterProcessApplication(commandContext, deployment, processesToRegisterFor);
                    return new ProcessApplicationDeploymentImpl(deployment, registration);
                }
                else
                {
                    RegisterWithJobExecutor(commandContext, deployment);
                }

                return deployment;
            });

            CreateUserOperationLog(deploymentBuilder, dep, commandContext);

            return dep;
        }

        protected internal virtual void CreateUserOperationLog(DeploymentBuilderImpl deploymentBuilder,
            IDeployment deployment, CommandContext commandContext)
        {
            IUserOperationLogManager logManager = commandContext.OperationLogManager;

            IList<PropertyChange> properties = new List<PropertyChange>();

            PropertyChange filterDuplicate = new PropertyChange("duplicateFilterEnabled", null, deploymentBuilder.IsDuplicateFilterEnabled);
            properties.Add(filterDuplicate);

            if (deploymentBuilder.IsDuplicateFilterEnabled)
            {
                PropertyChange deployChangedOnly = new PropertyChange("deployChangedOnly", null, deploymentBuilder.DeployChangedOnly);
                properties.Add(deployChangedOnly);
            }

            logManager.LogDeploymentOperation(UserOperationLogEntryFields.OperationTypeCreate, deployment.Id, properties);
        }

        protected internal virtual void SetDeploymentName(string deploymentId, DeploymentBuilderImpl deploymentBuilder,
            CommandContext commandContext)
        {
            if (!ReferenceEquals(deploymentId, null) && (deploymentId.Length > 0))
            {
                IDeploymentManager deploymentManager = commandContext.DeploymentManager;
                DeploymentEntity deployment = deploymentManager.FindDeploymentById(deploymentId);
                deploymentBuilder.Deployment.Name = deployment.Name;
            }
        }

        protected internal virtual IList<ResourceEntity> GetResources(DeploymentBuilderImpl deploymentBuilder,
            CommandContext commandContext)
        {
            List<ResourceEntity> resources = new List<ResourceEntity>();

            var deploymentIds = deploymentBuilder.Deployments;
            resources.AddRange(GetResourcesByDeploymentId(deploymentIds, commandContext));

            var deploymentResourcesById = deploymentBuilder.DeploymentResourcesById;
            resources.AddRange(GetResourcesById(deploymentResourcesById, commandContext));

            var deploymentResourcesByName = deploymentBuilder.DeploymentResourcesByName;
            resources.AddRange(GetResourcesByName(deploymentResourcesByName, commandContext));

            CheckDuplicateResourceName(resources);

            return resources;
        }

        protected internal virtual IList<ResourceEntity> GetResourcesByDeploymentId(ISet<string> deploymentIds,
            CommandContext commandContext)
        {
            List<ResourceEntity> result = new List<ResourceEntity>();

            if (deploymentIds.Count > 0)
            {
                IDeploymentManager deploymentManager = commandContext.DeploymentManager;

                foreach (string deploymentId in deploymentIds)
                {
                    DeploymentEntity deployment = deploymentManager.FindDeploymentById(deploymentId);
                    IDictionary<string, ResourceEntity> resources = deployment.Resources;
                    var values = resources.Values;
                    result.AddRange(values);
                }
            }

            return result;
        }

        protected internal virtual IList<ResourceEntity> GetResourcesById(
            IDictionary<string, ISet<string>> resourcesById, CommandContext commandContext)
        {
            List<ResourceEntity> result = new List<ResourceEntity>();

            //ResourceManager resourceManager = commandContext.ResourceManager as ResourceManager;
            var resourceManager = commandContext.ResourceManager;

            foreach (string deploymentId in resourcesById.Keys)
            {
                ISet<string> resourceIds = resourcesById[deploymentId];

                string[] resourceIdArray = resourceIds.ToArray();//.toArray(new string[resourceIds.Count]);
                IList<ResourceEntity> resources = resourceManager.FindResourceByDeploymentIdAndResourceIds(deploymentId, resourceIdArray);

                EnsureResourcesWithIdsExist(deploymentId, resourceIds, resources);

                result.AddRange(resources);
            }

            return result;
        }

        protected internal virtual IList<ResourceEntity> GetResourcesByName(
            IDictionary<string, ISet<string>> resourcesByName, CommandContext commandContext)
        {
            List<ResourceEntity> result = new List<ResourceEntity>();

            ResourceManager resourceManager = commandContext.ResourceManager as ResourceManager;

            foreach (string deploymentId in resourcesByName.Keys)
            {
                ISet<string> resourceNames = resourcesByName[deploymentId];

                string[] resourceNameArray = resourceNames.ToArray();//.toArray(new string[resourceNames.Count]);
                IList<ResourceEntity> resources = resourceManager.FindResourceByDeploymentIdAndResourceNames(deploymentId, resourceNameArray);

                EnsureResourcesWithNamesExist(deploymentId, resourceNames, resources);

                result.AddRange(resources);
            }

            return result;
        }

        protected internal virtual void AddResources(IList<ResourceEntity> resources,
            DeploymentBuilderImpl deploymentBuilder)
        {
            DeploymentEntity deployment = deploymentBuilder.Deployment;
            IDictionary<string, ResourceEntity> existingResources = deployment.Resources;

            foreach (ResourceEntity resource in resources)
            {
                string resourceName = resource.Name;

                if (existingResources != null && existingResources.ContainsKey(resourceName))
                {
                    string message = string.Format("Cannot add resource with id '{0}' and name '{1}' from " + "deployment with id '{2}' to new deployment because the new deployment contains " + "already a resource with same name.", resource.Id, resourceName, resource.DeploymentId);

                    throw new NotValidException(message);
                }

                System.IO.MemoryStream inputStream = new System.IO.MemoryStream(resource.Bytes);
                deploymentBuilder.AddInputStream(resourceName, inputStream);
            }
        }

        protected internal virtual void CheckDuplicateResourceName(IList<ResourceEntity> resources)
        {
            IDictionary<string, ResourceEntity> resourceMap = new Dictionary<string, ResourceEntity>();

            foreach (var resource in resources)
            {
                var name = resource.Name;

                ResourceEntity duplicate;

                if (resourceMap.TryGetValue(name, out duplicate))
                {
                    var deploymentId = resource.DeploymentId;
                    if (!deploymentId.Equals(duplicate.DeploymentId))
                    {
                        var message =
                            string.Format(
                                "The deployments with id '{0}' and '{1}' contain a resource with same name '{2}'.",
                                deploymentId, duplicate.DeploymentId, name);
                        throw new NotValidException(message);
                    }
                }
                resourceMap[name] = resource;
            }
        }

        protected internal virtual void EnsureDeploymentsWithIdsExists(ISet<string> expected,
            IList<DeploymentEntity> actual)
        {
            IDictionary<string, DeploymentEntity> deploymentMap = new Dictionary<string, DeploymentEntity>();
            foreach (var deployment in actual)
                deploymentMap[deployment.Id] = deployment;

            IList<string> missingDeployments = GetMissingElements(expected, deploymentMap);

            if (missingDeployments.Count > 0)
            {
                StringBuilder builder = new StringBuilder();

                builder.Append("The following deployments are not found by id: ");

                bool first = true;
                foreach (string missingDeployment in missingDeployments)
                {
                    if (!first)
                    {
                        builder.Append(", ");
                    }
                    else
                    {
                        first = false;
                    }
                    builder.Append(missingDeployment);
                }

                throw new NotFoundException(builder.ToString());
            }
        }

        protected internal virtual void EnsureResourcesWithIdsExist(string deploymentId, ISet<string> expectedIds,
            IList<ResourceEntity> actual)
        {
            IDictionary<string, ResourceEntity> resources = new Dictionary<string, ResourceEntity>();
            foreach (var resource in actual)
                resources[resource.Id] = resource;
            EnsureResourcesWithKeysExist(deploymentId, expectedIds, resources, "id");
        }

        protected internal virtual void EnsureResourcesWithNamesExist(string deploymentId, ISet<string> expectedNames,
            IList<ResourceEntity> actual)
        {
            IDictionary<string, ResourceEntity> resources = new Dictionary<string, ResourceEntity>();
            foreach (var resource in actual)
                resources[resource.Name] = resource;
            EnsureResourcesWithKeysExist(deploymentId, expectedNames, resources, "name");
        }

        protected internal virtual void EnsureResourcesWithKeysExist(string deploymentId, ISet<string> expectedKeys,
            IDictionary<string, ResourceEntity> actual, string valueProperty)
        {
            IList<string> missingResources = GetMissingElements(expectedKeys, actual);

            if (missingResources.Count > 0)
            {
                StringBuilder builder = new StringBuilder();

                builder.Append("The deployment with id '");
                builder.Append(deploymentId);
                builder.Append("' does not contain the following resources with ");
                builder.Append(valueProperty);
                builder.Append(": ");

                bool first = true;
                foreach (string missingResource in missingResources)
                {
                    if (!first)
                    {
                        builder.Append(", ");
                    }
                    else
                    {
                        first = false;
                    }
                    builder.Append(missingResource);
                }

                throw new NotFoundException(builder.ToString());
            }
        }

        protected internal virtual IList<string> GetMissingElements<T1>(ISet<string> expected,
            IDictionary<string, T1> actual)
        {
            IList<string> missingElements = new List<string>();
            foreach (var value in expected)
                if (!actual.ContainsKey(value))
                    missingElements.Add(value);
            return missingElements;
        }

        protected internal virtual ISet<string> GetAllDeploymentIds(DeploymentBuilderImpl deploymentBuilder)
        {
            ISet<string> result = new HashSet<string>();

            //var nameFromDeployment = deploymentBuilder.nameFromDeployment;
            string nameFromDeployment = deploymentBuilder.GetNameFromDeployment();
            if (!string.IsNullOrEmpty(nameFromDeployment))
                result.Add(nameFromDeployment);

            var deployments = deploymentBuilder.Deployments;
            result.AddAll(deployments);

            deployments = new HashSet<string>(deploymentBuilder.DeploymentResourcesById.Keys);
            result.AddAll(deployments);

            deployments = new HashSet<string>(deploymentBuilder.DeploymentResourcesByName.Keys);
            result.AddAll(deployments);

            return result;
        }

        protected internal virtual void CheckCreateAndReadDeployments(CommandContext commandContext,
            ISet<string> deploymentIds)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckCreateDeployment();
                foreach (var deploymentId in deploymentIds)
                    checker.CheckReadDeployment(deploymentId);
            }
        }

        protected internal virtual void AcquireExclusiveLock(CommandContext commandContext)
        {
            if (context.Impl.Context.ProcessEngineConfiguration.DeploymentLockUsed)
            {
                // Acquire global exclusive lock: this ensures that there can be only one
                // transaction in the cluster which is allowed to perform deployments.
                // This is important to ensure that duplicate filtering works correctly
                // in a multi-node cluster. See also https://app.camunda.com/jira/browse/CAM-2128

                // It is also important to ensure the uniqueness of a process definition key,
                // version and tenant-id since there is no database constraint to check it.

                commandContext.PropertyManager.AcquireExclusiveLock();
            }
            else
            {
                Log.WarnDisabledDeploymentLock();
            }
        }

        protected internal virtual DeploymentEntity InitDeployment()
        {
            DeploymentEntity deployment = deploymentBuilder.Deployment;
            //TODO 线程开始的时间，Reset重置为当前时间
            deployment.DeploymentTime = ClockUtil.CurrentTime;
            return deployment;
        }
        //准备部署的资源
        protected internal virtual IDictionary<string, ResourceEntity> ResolveResourcesToDeploy(
            CommandContext commandContext, DeploymentEntity deployment)
        {
            IDictionary<string, ResourceEntity> resourcesToDeploy = new Dictionary<string, ResourceEntity>();
            IDictionary<string, ResourceEntity> containedResources = deployment.Resources;

            if (deploymentBuilder.IsDuplicateFilterEnabled)
            {
                var source = deployment.Source;
                if (string.IsNullOrEmpty(source))//== null || source.Length == 0)
                    source = ProcessApplicationDeploymentFields.ProcessApplicationDeploymentSource;
                //查询ResourceManager.FindLatestResourcesByDeploymentName
                IDictionary<string, ResourceEntity> existingResources = commandContext.ResourceManager.FindLatestResourcesByDeploymentName(deployment.Name, new HashSet<string>(containedResources.Keys), source, deployment.TenantId);

                foreach (ResourceEntity deployedResource in containedResources.Values)
                {
                    string resourceName = deployedResource.Name;
                    ResourceEntity existingResource = existingResources.ContainsKey(resourceName) ? existingResources[resourceName] : null;

                    if (existingResource == null || existingResource.Generated || ResourcesDiffer(deployedResource, existingResource))
                    {
                        // resource should be deployed

                        if (deploymentBuilder.DeployChangedOnly)
                        {
                            resourcesToDeploy[resourceName] = deployedResource;
                        }
                        else
                        {
                            // all resources should be deployed
                            resourcesToDeploy = containedResources;
                            break;
                        }
                    }
                }
            }
            else
            {
                resourcesToDeploy = containedResources;
            }

            return resourcesToDeploy;
        }

        protected internal virtual bool ResourcesDiffer(ResourceEntity resource, ResourceEntity existing)
        {
            byte[] bytes = resource.Bytes;
            byte[] savedBytes = existing.Bytes;
            return bytes == savedBytes;
        }

        protected internal virtual void Deploy(DeploymentEntity deployment)
        {
            deployment.IsNew = true;
            Context.CommandContext.DeploymentManager.InsertDeployment(deployment);
        }

        protected internal virtual DeploymentEntity GetExistingDeployment(CommandContext commandContext,
            string deploymentName)
        {
            return commandContext.DeploymentManager.FindLatestDeploymentByName(deploymentName);
        }

        protected internal virtual void ScheduleProcessDefinitionActivation(CommandContext commandContext,
            DeploymentEntity deployment)
        {
            if (deploymentBuilder.ProcessDefinitionsActivationDate != null)
            {
                var repositoryService = commandContext.ProcessEngineConfiguration.RepositoryService;

                foreach (IProcessDefinition processDefinition in deployment.DeployedProcessDefinitions)
                {

                    //如果激活日期已设置，我们首先暂停所有流程定义 If activation date is set, we first suspend all the process definition
                    repositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionId(processDefinition.Id).Suspend();

                    //我们在规定的日期安排激活 And we schedule an activation at the provided date
                    repositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionId(processDefinition.Id).ExecutionDate((DateTime)deploymentBuilder.ProcessDefinitionsActivationDate).Activate();
                }
            }
        }

        protected internal virtual IProcessApplicationRegistration RegisterProcessApplication(
            CommandContext commandContext, DeploymentEntity deployment, ISet<string> processKeysToRegisterFor)
        {
            var appDeploymentBuilder = (ProcessApplicationDeploymentBuilderImpl)deploymentBuilder;
            var appReference = appDeploymentBuilder.ProcessApplicationReference;

            // build set of deployment ids this process app should be registered for:
            List<string> deploymentsToRegister = new List<string>() { deployment.Id };
            if (appDeploymentBuilder.IsResumePreviousVersions())
            {
                if (ResumePreviousBy.ResumeByProcessDefinitionKey == appDeploymentBuilder.ResumePreviousVersionsByRenamed)
                {
                    deploymentsToRegister.AddRange(ResumePreviousByProcessDefinitionKey(commandContext, deployment, processKeysToRegisterFor));
                }
                else if (ResumePreviousBy.ResumeByDeploymentName == appDeploymentBuilder.ResumePreviousVersionsByRenamed)
                {
                    deploymentsToRegister.AddRange(ResumePreviousByDeploymentName(commandContext, deployment));
                }
            }

            // register process application for deployments
            return (new RegisterProcessApplicationCmd(deploymentsToRegister, appReference)).Execute(commandContext);
        }

        /// <summary>
        ///     Searches in previous deployments for the same processes and retrieves the deployment ids.
        /// </summary>
        /// <param name="commandContext"> </param>
        /// <param name="deployment">
        ///     the current deployment
        /// </param>
        /// <param name="processKeysToRegisterFor">
        ///     the process keys this process application wants to register
        /// </param>
        /// <param name="deploymentsToRegister">
        ///     the set where to add further deployments this process application
        ///     should be registered for
        /// </param>
        /// <returns>
        ///     a set of deployment ids that contain versions of the
        ///     processKeysToRegisterFor
        /// </returns>
        protected internal virtual ISet<string> ResumePreviousByProcessDefinitionKey(CommandContext commandContext,
            DeploymentEntity deployment, ISet<string> processKeysToRegisterFor)
        {
            ISet<string> processDefinitionKeys = new HashSet<string>(processKeysToRegisterFor);

            var deployedProcesses = GetDeployedProcesses(deployment);
            foreach (var deployedProcess in deployedProcesses)
                if (deployedProcess.Version > 1)
                    processDefinitionKeys.Add(deployedProcess.Key);

            return FindDeploymentIdsForProcessDefinitions(commandContext, processDefinitionKeys);
        }

        /// <summary>
        ///     Searches for previous deployments with the same name.
        /// </summary>
        /// <param name="commandContext"> </param>
        /// <param name="deployment"> the current deployment </param>
        /// <returns> a set of deployment ids </returns>
        protected internal virtual IList<string> ResumePreviousByDeploymentName(CommandContext commandContext,
            DeploymentEntity deployment)
        {
            //throw new NotImplementedException();
            return commandContext.DeploymentManager.Find(m => m.Name == deployment.Name).Select(m => m.Id).ToList();// (new DeploymentQueryImpl()).deploymentName(deployment.Name).list();
            //ISet<string> deploymentIds = new HashSet<string>(previousDeployments.Count);
            //foreach (var d in previousDeployments)
            //{
            //    deploymentIds.Add(d.Id);
            //}
            //return deploymentIds;
            //return null;
        }

        protected internal virtual IList<IProcessDefinition> GetDeployedProcesses(DeploymentEntity deployment)
        {
            IList<IProcessDefinition> deployedProcessDefinitions = deployment.DeployedProcessDefinitions;
            if (deployedProcessDefinitions == null)
            {
                // existing deployment
                CommandContext commandContext = Context.CommandContext;
                IProcessDefinitionManager manager = commandContext.ProcessDefinitionManager;
                deployedProcessDefinitions = manager.FindProcessDefinitionsByDeploymentId(deployment.Id).Cast<IProcessDefinition>().ToList();
            }

            return deployedProcessDefinitions;
        }

        protected internal virtual ISet<string> RetrieveProcessKeysFromResources(
            IDictionary<string, ResourceEntity> resources)
        {
            ISet<string> keys = new HashSet<string>();

            foreach (var resource in resources.Values)
                if (IsBpmnResource(resource))
                {
                    System.IO.MemoryStream byteStream = new System.IO.MemoryStream(resource.Bytes);
                    IBpmnModelInstance model = Model.Bpmn.Bpmn.ReadModelFromStream(byteStream);
                    foreach (IProcess process in model.Definitions.GetChildElementsByType(typeof(IProcess)))
                    {
                        keys.Add(process.Id);
                    }
                }
                else if (IsCmmnResource(resource))
                {
                    throw new System.Exception("不支持Cmmn");
                    //System.IO.MemoryStream byteStream = new System.IO.MemoryStream(resource.Bytes);
                    //CmmnModelInstance model = Cmmn.readModelFromStream(byteStream);
                    //foreach (Case cmmnCase in model.Definitions.Cases)
                    //{
                    //    keys.Add(cmmnCase.Id);
                    //}
                }

            return keys;
        }

        protected internal virtual bool IsBpmnResource(ResourceEntity resourceEntity)
        {
            return StringUtil.HasAnySuffix(resourceEntity.Name, BpmnDeployer.BpmnResourceSuffixes);
        }

        protected internal virtual bool IsCmmnResource(ResourceEntity resourceEntity)
        {
            return false;
            //return StringUtil.hasAnySuffix(resourceEntity.Name, CmmnDeployer.CMMN_RESOURCE_SUFFIXES);
        }

        protected internal virtual ISet<string> FindDeploymentIdsForProcessDefinitions(CommandContext commandContext,
            ISet<string> processDefinitionKeys)
        {
            ISet<string> deploymentsToRegister = new HashSet<string>();

            if (processDefinitionKeys.Count > 0)
            {
                string[] keys = processDefinitionKeys.ToArray();//.toArray(new string[processDefinitionKeys.Count]);
                IProcessDefinitionManager processDefinitionManager = commandContext.ProcessDefinitionManager;
                IList<IProcessDefinition> previousDefinitions = processDefinitionManager.FindProcessDefinitionsByKeyIn(keys);

                foreach (IProcessDefinition definition in previousDefinitions)
                {
                    deploymentsToRegister.Add(definition.DeploymentId);
                }
            }
            return deploymentsToRegister;
        }

        protected internal virtual void RegisterWithJobExecutor(CommandContext commandContext,
            DeploymentEntity deployment)
        {
            try
            {
                (new RegisterDeploymentCmd(deployment.Id)).Execute(commandContext);
            }
            finally
            {
                DeploymentFailListener listener = new DeploymentFailListener(deployment.Id);

                try
                {
                    commandContext.TransactionContext.AddTransactionListener(TransactionJavaStatus.RolledBack, listener);
                }
                catch (System.Exception)
                {
                    TxLog.DebugTransactionOperation(
                        "Could not register transaction synchronization. Probably the TX has already been rolled back by application code.");
                    listener.Execute(commandContext);
                }
            }
        }

    }
}