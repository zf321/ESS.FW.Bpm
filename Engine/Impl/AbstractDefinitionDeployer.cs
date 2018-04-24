using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository.Impl;
using ESS.FW.Bpm.Engine.Persistence.Deploy;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Persistence.Deploy.Cache;
using ESS.FW.DataAccess;

namespace ESS.FW.Bpm.Engine.Impl
{
    /// <summary>
    ///     <seealso cref="Deployer" /> responsible to parse resource files and create the proper entities.
    ///     This class is extended by specific resource deployers.
    ///     Note: Implementations must be thread-safe. In particular they should not keep deployment-specific state.
    /// </summary>
    public abstract class AbstractDefinitionDeployer<TDefinitionEntity> : IDeployer
        where TDefinitionEntity : IResourceDefinitionEntity
    {
        public static readonly string[] DIAGRAM_SUFFIXES = { "png", "jpg", "gif", "svg" };

        private readonly CommandLogger LOG = ProcessEngineLogger.CmdLogger;


        public virtual IDGenerator IdGenerator { get; set; }

        /// <returns> the list of resource suffixes for this cacheDeployer </returns>
        protected internal abstract string[] ResourcesSuffixes { get; }

        protected internal virtual string[] DiagramSuffixes
        {
            get { return DIAGRAM_SUFFIXES; }
        }

        protected internal virtual ProcessEngineConfigurationImpl ProcessEngineConfiguration
        {
            get { return Context.ProcessEngineConfiguration; }
        }

        protected internal virtual CommandContext CommandContext
        {
            get { return Context.CommandContext; }
        }

        protected internal virtual DeploymentCache DeploymentCache
        {
            get { return ProcessEngineConfiguration.DeploymentCache; }
        }


        public virtual void Deploy(DeploymentEntity deployment)
        {
            LOG.DebugProcessingDeployment(deployment.Name);
            var properties = new Dictionary<string, object>();
            IList<TDefinitionEntity> definitions = ParseDefinitionResources(deployment, new Core.Model.Properties(properties));
            EnsureNoDuplicateDefinitionKeys(definitions);
            //Post 把流程定义实例写入持久层（薪）/更新流程定义（旧） 及放到上下文中 供其它领域模型使用
            PostProcessDefinitions(deployment, definitions, new Core.Model.Properties(properties));
        }

        protected internal virtual IList<TDefinitionEntity> ParseDefinitionResources(DeploymentEntity deployment,
            Core.Model.Properties properties)
        {
            IList<TDefinitionEntity> definitions = new List<TDefinitionEntity>();
            foreach (ResourceEntity resource in deployment.Resources.Values)
            {
                LOG.DebugProcessingResource(resource.Name);
                if (IsResourceHandled(resource))
                {
                    var r = TransformResource(deployment, resource, properties);
                    foreach (var item in r)
                    {
                        definitions.Add(item);
                    }
                    //((List<TDefinitionEntity>)definitions).AddRange();
                }
            }
            return definitions;
        }
        /// <summary>
        /// 根据名称 后缀判断是否处理bpm,dmn等区分
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        protected internal virtual bool IsResourceHandled(ResourceEntity resource)
        {
            var resourceName = resource.Name.ToLower();

            foreach (var suffix in ResourcesSuffixes)
                if (resourceName.EndsWith(suffix))
                    return true;
            return false;
        }
        //解析
        protected internal virtual ICollection<TDefinitionEntity> TransformResource(DeploymentEntity deployment,
            ResourceEntity resource, Core.Model.Properties properties)
        {
            var resourceName = resource.Name;
            var definitions = TransformDefinitions(deployment, resource, properties);

            foreach (var definition in definitions)
            {
                definition.ResourceName = resourceName;
                String diagramResourceName = GetDiagramResourceForDefinition(deployment, resourceName, definition, deployment.Resources);
                if (diagramResourceName != null)
                {
                    definition.DiagramResourceName = diagramResourceName;
                }
            }


            return definitions;
        }


        /// <summary>
        ///     Transform the resource entity into definition entities.
        /// </summary>
        /// <param name="deployment"> the deployment the resources belongs to </param>
        /// <param name="resource"> the resource to transform </param>
        /// <returns> a list of transformed definition entities </returns>
        protected internal abstract IList<TDefinitionEntity> TransformDefinitions(DeploymentEntity deployment,
            ResourceEntity resource, Core.Model.Properties properties);

        /// <summary>
        ///     Returns the default name of the image resource for a certain definition.
        ///     It will first look for an image resource which matches the definition
        ///     specifically, before resorting to an image resource which matches the file
        ///     containing the definition.
        ///     Example: if the deployment contains a BPMN 2.0 xml resource called
        ///     'abc.bpmn20.xml' containing only one process with key 'myProcess', then
        ///     this method will look for an image resources called 'abc.myProcess.png'
        ///     (or .jpg, or .gif, etc.) or 'abc.png' if the previous one wasn't found.
        ///     Example 2: if the deployment contains a BPMN 2.0 xml resource called
        ///     'abc.bpmn20.xml' containing three processes (with keys a, b and c),
        ///     then this method will first look for an image resource called 'abc.a.png'
        ///     before looking for 'abc.png' (likewise for b and c).
        ///     Note that if abc.a.png, abc.b.png and abc.c.png don't exist, all
        ///     processes will have the same image: abc.png.
        /// </summary>
        /// <returns> null if no matching image resource is found. </returns>
        protected internal virtual string GetDiagramResourceForDefinition(DeploymentEntity deployment,
            string resourceName, TDefinitionEntity definition, IDictionary<string, ResourceEntity> resources)
        {
            foreach (var diagramSuffix in DiagramSuffixes)
            {
                var definitionDiagramResource = GetDefinitionDiagramResourceName(resourceName, definition, diagramSuffix);
                var diagramForFileResource = GetGeneralDiagramResourceName(resourceName, definition, diagramSuffix);
                if (resources.ContainsKey(definitionDiagramResource))
                    return definitionDiagramResource;
                if (resources.ContainsKey(diagramForFileResource))
                    return diagramForFileResource;
            }
            // no matching diagram found
            return null;
        }

        protected internal virtual string GetDefinitionDiagramResourceName(string resourceName,
            TDefinitionEntity definition, string diagramSuffix)
        {
            var fileResourceBase = StripDefinitionFileSuffix(resourceName);
            var definitionKey = definition.Key;

            return fileResourceBase + definitionKey + "." + diagramSuffix;
        }

        protected internal virtual string GetGeneralDiagramResourceName(string resourceName, TDefinitionEntity definition,
            string diagramSuffix)
        {
            var fileResourceBase = StripDefinitionFileSuffix(resourceName);

            return fileResourceBase + diagramSuffix;
        }

        protected internal virtual string StripDefinitionFileSuffix(string resourceName)
        {
            foreach (var suffix in ResourcesSuffixes)
                if (resourceName.ToLower().EndsWith(suffix))
                    return resourceName.Substring(0, resourceName.Length - suffix.Length);
            return resourceName;
        }

        protected internal virtual void EnsureNoDuplicateDefinitionKeys(IList<TDefinitionEntity> definitions)
        {
            ISet<string> keys = new HashSet<string>();

            foreach (var definition in definitions)
            {
                var key = definition.Key;

                if (keys.Contains(key))
                    throw new ProcessEngineException("The deployment contains definitions with the same key '" + key +
                                                     "' (id attribute), this is not allowed");

                keys.Add(key);
            }
        }

        protected internal virtual void PostProcessDefinitions(DeploymentEntity deployment,
            IList<TDefinitionEntity> definitions, Core.Model.Properties properties)
        {
            if (deployment.IsNew)//新的，写入持久层及上下文中
                PersistDefinitions(deployment, definitions, properties);
            else//已有，从持久层加载到上下文中
                LoadDefinitions(deployment, definitions, properties);
        }

        protected internal virtual void PersistDefinitions(DeploymentEntity deployment,
            IList<TDefinitionEntity> definitions, Core.Model.Properties properties)
        {
            foreach (var definition in definitions)
            {
                var definitionKey = definition.Key;
                var tenantId = deployment.TenantId;

                var latestDefinition = FindLatestDefinitionByKeyAndTenantId(definitionKey, tenantId);

                UpdateDefinitionByLatestDefinition(deployment, definition, latestDefinition);

                PersistDefinition(definition);
                RegisterDefinition(deployment, definition, properties);
            }
        }

        protected internal virtual void UpdateDefinitionByLatestDefinition(DeploymentEntity deployment,
            TDefinitionEntity definition, TDefinitionEntity latestDefinition)
        {
            definition.Version = GetNextVersion(deployment, definition, latestDefinition);
            definition.Id = GenerateDefinitionId(deployment, definition, latestDefinition);
            definition.DeploymentId = deployment.Id;
            definition.TenantId = deployment.TenantId;
        }

        protected internal virtual void LoadDefinitions(DeploymentEntity deployment, IList<TDefinitionEntity> definitions,
            Core.Model.Properties properties)
        {
            foreach (var definition in definitions)
            {
                var deploymentId = deployment.Id;
                var definitionKey = definition.Key;

                var persistedDefinition = FindDefinitionByDeploymentAndKey(deploymentId, definitionKey);
                HandlePersistedDefinition(definition, persistedDefinition, deployment, properties);
            }
        }

        protected internal virtual void HandlePersistedDefinition(TDefinitionEntity definition,
            TDefinitionEntity persistedDefinition, DeploymentEntity deployment, Core.Model.Properties properties)
        {
            PersistedDefinitionLoaded(deployment, definition, persistedDefinition);
            UpdateDefinitionByPersistedDefinition(deployment, definition, persistedDefinition);
            RegisterDefinition(deployment, definition, properties);
        }

        protected internal virtual void UpdateDefinitionByPersistedDefinition(DeploymentEntity deployment,
            TDefinitionEntity definition, TDefinitionEntity persistedDefinition)
        {
            definition.Version = persistedDefinition.Version;
            definition.Id = persistedDefinition.Id;
            definition.DeploymentId = deployment.Id;
            definition.TenantId = persistedDefinition.TenantId;
        }

        /// <summary>
        ///     Called when a previous version of a definition was loaded from the persistent store.
        /// </summary>
        /// <param name="deployment"> the deployment of the definition </param>
        /// <param name="definition"> the definition entity </param>
        /// <param name="persistedDefinition"> the loaded definition entity </param>
        protected internal virtual void PersistedDefinitionLoaded(DeploymentEntity deployment,
            TDefinitionEntity definition, TDefinitionEntity persistedDefinition)
        {
            // do nothing;
        }

        /// <summary>
        ///     Find a definition entity by deployment id and definition key.
        /// </summary>
        /// <param name="deploymentId"> the deployment id </param>
        /// <param name="definitionKey"> the definition key </param>
        /// <returns> the corresponding definition entity or null if non is found </returns>
        protected internal abstract TDefinitionEntity FindDefinitionByDeploymentAndKey(string deploymentId,
            string definitionKey);

        /// <summary>
        ///     Find the last deployed definition entity by definition key and tenant id.
        /// </summary>
        /// <returns> the corresponding definition entity or null if non is found </returns>
        protected internal abstract TDefinitionEntity FindLatestDefinitionByKeyAndTenantId(string definitionKey,
            string tenantId);

        /// <summary>
        ///     Persist definition entity into the database.
        /// </summary>
        /// <param name="definition"> the definition entity </param>
        protected internal abstract void PersistDefinition(TDefinitionEntity definition);
        /// <summary>
        /// 添加definition到持久层及上下文中
        /// </summary>
        /// <param name="deployment"></param>
        /// <param name="definition"></param>
        /// <param name="properties"></param>
        protected internal virtual void RegisterDefinition(DeploymentEntity deployment, TDefinitionEntity definition,
            Core.Model.Properties properties)
        {
            DeploymentCache deploymentCache = DeploymentCache;

            // Add to cache
            AddDefinitionToDeploymentCache(deploymentCache, definition);

            DefinitionAddedToDeploymentCache(deployment, definition, properties);

            // Add to deployment for further usage
            deployment.AddDeployedArtifact(definition);
        }

        /// <summary>
        ///     Add a definition to the deployment cache
        /// </summary>
        /// <param name="deploymentCache"> the deployment cache </param>
        /// <param name="definition"> the definition to add </param>
        protected internal abstract void AddDefinitionToDeploymentCache(DeploymentCache deploymentCache,
            TDefinitionEntity definition);
        /// <summary>
        ///     Called after a definition was added to the deployment cache.
        /// </summary>
        /// <param name="deployment"> the deployment of the definition </param>
        /// <param name="definition"> the definition entity </param>
        protected internal virtual void DefinitionAddedToDeploymentCache(DeploymentEntity deployment,
            TDefinitionEntity definition, Core.Model.Properties properties)
        {
            // do nothing
        }
        /// <summary>
        ///     per default we increment the latest definition version by one - but you
        ///     might want to hook in some own logic here, e.g. to align definition
        ///     versions with deployment / build versions.
        /// </summary>
        protected internal virtual int GetNextVersion(DeploymentEntity deployment, TDefinitionEntity newDefinition,
            TDefinitionEntity latestDefinition)
        {
            var result = 1;
            if (latestDefinition != null)
            {
                var latestVersion = latestDefinition.Version;
                result = latestVersion + 1;
            }
            return result;
        }

        /// <summary>
        ///     create an id for the definition. The default is to ask the <seealso cref="IdGenerator" />
        ///     and add the definition key and version if that does not exceed 64 characters.
        ///     You might want to hook in your own implementation here.
        /// </summary>
        protected internal virtual string GenerateDefinitionId(DeploymentEntity deployment,
            TDefinitionEntity newDefinition, TDefinitionEntity latestDefinition)
        {
            var nextId = IdGenerator.NewGuid();//.NextId;

            var definitionKey = newDefinition.Key;
            var definitionVersion = newDefinition.Version;

            var definitionId = definitionKey + ":" + definitionVersion + ":" + nextId;

            // ACT-115: maximum id length is 64 characters
            if (definitionId.Length > 64)
                definitionId = nextId;
            return definitionId;
        }
    }
}