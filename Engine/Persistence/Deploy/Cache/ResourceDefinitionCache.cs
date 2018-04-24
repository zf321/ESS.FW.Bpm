using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Common.Cache;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Deploy.Cache
{


    /// <summary>
    /// 
    /// </summary>
    public abstract class ResourceDefinitionCache<T> where T : class,IResourceDefinition,IDbEntity,new()
    {

        protected internal ICache<string, T> cache;
        protected internal CacheDeployer CacheDeployer;

        public ResourceDefinitionCache(ICacheFactory factory, int cacheCapacity, CacheDeployer cacheDeployer)
        {
            this.cache = factory.CreateCache<T>(cacheCapacity);
            this.CacheDeployer = cacheDeployer;
        }

        public virtual T FindDefinitionFromCache(string definitionId)
        {
            return cache.Get(definitionId);//r
                                              
                                              //return cache.get(definitionId);
        }
        //查询已部署得定义（先查缓存）
        public virtual T FindDeployedDefinitionById(string definitionId)
        {
            CheckInvalidDefinitionId(definitionId);
            //TODO 缓存相关，此处缓存并没有随着EF数据更新，暂改为直接取数据库数据
            //T definition = Manager.GetCachedResourceDefinitionEntity(definitionId);
            // Todo: Context.CommandContext.GetDbEntityManager<TEntity>()方法有问题
            //T definition = Context.CommandContext.GetDbEntityManager<T>().Get(definitionId);
            T definition = Context.CommandContext.DbContext.Set<T>().Find(definitionId);
            if (definition == null)
            {
                definition = Manager.FindLatestDefinitionById(definitionId);
            }
            
            CheckDefinitionFound(definitionId, definition);
            definition = ResolveDefinition(definition);
            return definition;
        }

        /// <returns> the latest version of the definition with the given key (from any tenant) </returns>
        /// <exception cref="ProcessEngineException"> if more than one tenant has a definition with the given key </exception>
        public virtual T FindDeployedLatestDefinitionByKey(string definitionKey)
        {
            T definition = Manager.FindLatestDefinitionByKey(definitionKey);
            CheckInvalidDefinitionByKey(definitionKey, definition);
            definition = ResolveDefinition(definition);
            return definition;
        }

        public virtual T FindDeployedLatestDefinitionByKeyAndTenantId(string definitionKey, string tenantId)
        {
            T definition = Manager.FindLatestDefinitionByKeyAndTenantId(definitionKey, tenantId);
            CheckInvalidDefinitionByKeyAndTenantId(definitionKey, tenantId, definition);
            definition = ResolveDefinition(definition);
            return definition;
        }

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: public T findDeployedDefinitionByKeyVersionAndTenantId(final String definitionKey, final Integer definitionVersion, final String tenantId)
        public virtual T FindDeployedDefinitionByKeyVersionAndTenantId(string definitionKey, int? definitionVersion, string tenantId)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext = org.camunda.bpm.engine.impl.context.Context.getCommandContext();
            CommandContext commandContext = context.Impl.Context.CommandContext;
            T definition = commandContext.RunWithoutAuthorization(()=> Manager.FindDefinitionByKeyVersionAndTenantId(definitionKey, definitionVersion, tenantId));
            CheckInvalidDefinitionByKeyVersionAndTenantId(definitionKey, definitionVersion, tenantId, definition);
            definition = ResolveDefinition(definition);
            return definition;
        }
        

        public virtual T FindDeployedDefinitionByDeploymentAndKey(string deploymentId, string definitionKey)
        {
            T definition = Manager.FindDefinitionByDeploymentAndKey(deploymentId, definitionKey);
            CheckInvalidDefinitionByDeploymentAndKey(deploymentId, definitionKey, definition);
            definition = ResolveDefinition(definition);
            return definition;
        }
        //TODO cache未实现
        public virtual T ResolveDefinition(T definition)
        {
            string definitionId = ((IResourceDefinition)definition).Id;

            string deploymentId = definition.DeploymentId;
            T cachedDefinition = cache.Get(definitionId);
            if (cachedDefinition == null)  //双层If+Lock
            {
                lock (this)
                {
                    cachedDefinition = cache.Get(definitionId);
                   // cachedDefinition = Context.CommandContext.DeploymentManager.Get(definitionId) as T;
                    if (cachedDefinition == null)
                    {
                        DeploymentEntity deployment = Context.CommandContext.DeploymentManager.FindDeploymentById(deploymentId);
                        deployment.IsNew = false;
                        CacheDeployer.DeployOnlyGivenResourcesOfDeployment(deployment, definition.ResourceName, definition.DiagramResourceName);
                        cachedDefinition = cache.Get(definitionId);
                    }
                }
                CheckInvalidDefinitionWasCached(deploymentId, definitionId, cachedDefinition);
            }
            return cachedDefinition;
        }

        public virtual void AddDefinition(T definition)
        {
            cache.Put(((IResourceDefinition)definition).Id, definition);                                            
          //Cache.Put(definition.Id, definition);
        }

        //public virtual T GetDefinition(string id)
        //{
        //    return cache.Get(id);//r
                                    
        //                            //return cache.get(id);
        //}

        public virtual void RemoveDefinitionFromCache(string id)
        {
            cache.Remove(id);//r
                             
                             //cache.remove(id);
        }

        public virtual void Clear()
        {
            cache.Clear();//r
                            
                            //cache.clear();
        }

        public virtual ICache<string, T> Cache
        {
            get
            {
                return cache;
            }
        }

        protected internal abstract IAbstractResourceDefinitionManager<T> Manager { get; }

        protected internal abstract void CheckInvalidDefinitionId(string definitionId);

        protected internal abstract void CheckDefinitionFound(string definitionId, T definition);

        protected internal abstract void CheckInvalidDefinitionByKey(string definitionKey, T definition);

        protected internal abstract void CheckInvalidDefinitionByKeyAndTenantId(string definitionKey, string tenantId, T definition);

        protected internal abstract void CheckInvalidDefinitionByKeyVersionAndTenantId(string definitionKey, int? definitionVersion, string tenantId, T definition);

        protected internal abstract void CheckInvalidDefinitionByDeploymentAndKey(string deploymentId, string definitionKey, T definition);

        protected internal abstract void CheckInvalidDefinitionWasCached(string deploymentId, string definitionId, T definition);

    }
}