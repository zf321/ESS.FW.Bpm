using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Impl.Cfg;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
using ESS.FW.Common.Components;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{



    /// <summary>
    /// 
    /// </summary>
    [Component]
    public class ResourceManager : AbstractManagerNet<ResourceEntity>, IResourceManager
    {
        public ResourceManager(DbContext dbContex, ILoggerFactory loggerFactory, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
        }

        public virtual void InsertResource(ResourceEntity resource)
        {
            Add(resource);
        }

        public virtual void DeleteResourcesByDeploymentId(string deploymentId)
        {
            Delete(m => m.DeploymentId == deploymentId);
        }

        public virtual ResourceEntity FindResourceByDeploymentIdAndResourceName(string deploymentId, string resourceName)
        {
            //IDictionary<string, object> @params = new Dictionary<string, object>();
            //@params["deploymentId"] = deploymentId;
            //@params["resourceName"] = resourceName;
            //return (ResourceEntity)DbEntityManager.SelectOne("selectResourceByDeploymentIdAndResourceName", @params);
            //ProcessDefinitionEntity protId==m.DeploymentId);
            return Single(m => m.DeploymentId == deploymentId && m.Name.ToLower() == resourceName.ToLower());
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<ResourceEntity> findResourceByDeploymentIdAndResourceNames(String deploymentId, String... resourceNames)
        public virtual IList<ResourceEntity> FindResourceByDeploymentIdAndResourceNames(string deploymentId, params string[] resourceNames)
        {
            //IDictionary<string, object> @params = new Dictionary<string, object>();
            //@params["deploymentId"] = deploymentId;
            //@params["resourceNames"] = resourceNames;
            //return ListExt.ConvertToListT<ResourceEntity>(DbEntityManager.SelectList("selectResourceByDeploymentIdAndResourceNames", @params));
            return Find(m => m.DeploymentId == deploymentId && resourceNames.Contains(m.Name)).ToList();
        }

        public virtual ResourceEntity FindResourceByDeploymentIdAndResourceId(string deploymentId, string resourceId)
        {
            //IDictionary<string, object> @params = new Dictionary<string, object>();
            //@params["deploymentId"] = deploymentId;
            //@params["resourceId"] = resourceId;
            //return (ResourceEntity)DbEntityManager.SelectOne("selectResourceByDeploymentIdAndResourceId", @params);
            return Single(m => m.DeploymentId == deploymentId && m.Id == resourceId);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<ResourceEntity> findResourceByDeploymentIdAndResourceIds(String deploymentId, String... resourceIds)
        public virtual IList<ResourceEntity> FindResourceByDeploymentIdAndResourceIds(string deploymentId, params string[] resourceIds)
        {
            //IDictionary<string, object> @params = new Dictionary<string, object>();
            //@params["deploymentId"] = deploymentId;
            //@params["resourceIds"] = resourceIds;
            //return ListExt.ConvertToListT<ResourceEntity>(DbEntityManager.SelectList("selectResourceByDeploymentIdAndResourceIds", @params));
            return Find(m => m.DeploymentId == deploymentId && resourceIds.Contains(m.Id)).ToList();
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<ResourceEntity> findResourcesByDeploymentId(String deploymentId)
        public virtual IList<ResourceEntity> FindResourcesByDeploymentId(string deploymentId)
        {
            //return ListExt.ConvertToListT<ResourceEntity>(DbEntityManager.SelectList("selectResourcesByDeploymentId", deploymentId));
            return Find(m => m.DeploymentId == deploymentId).OrderBy(c=>c.Name).ToList();
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.Map<String, ResourceEntity> findLatestResourcesByDeploymentName(String deploymentName, java.util.ISet<string> resourcesToFind, String source, String tenantId)
        public virtual IDictionary<string, ResourceEntity> FindLatestResourcesByDeploymentName(string deploymentName, ISet<string> resourcesToFind, string source, string tenantId)
        {
            //IDictionary<string, object> @params = new Dictionary<string, object>();
            //@params["deploymentName"] = deploymentName;//"BpmProcessEngineCmdTest"
            //@params["resourcesToFind"] = resourcesToFind;//"BpmnParseTest.testInvalidSubProcessWithCompensationStartEvent.bpmn"
            //@params["source"] = source;//"process application"
            //@params["tenantId"] = tenantId;
            //IList<ResourceEntity> resources = ListExt.ConvertToListT<ResourceEntity>(DbEntityManager.SelectList("selectLatestResourcesByDeploymentName", @params));
            //throw new System.NotImplementedException("复杂查询");
            //重写查询 source数据库为null 暂时忽略source @params["source"] = source;//"process application" 解析不到
            IList<ResourceEntity> resources = new List<ResourceEntity>();
            
            DeploymentEntity lastDeploy =context.Impl.Context.CommandContext.DeploymentManager.FindLatestDeploymentByName(deploymentName);
            if (lastDeploy != null)
            {
                resources = Find(m => m.DeploymentId == lastDeploy.Id && resourcesToFind.Contains(m.Name) && m.TenantId == tenantId).ToList();
            }

            IDictionary<string, ResourceEntity> existingResourcesByName = new Dictionary<string, ResourceEntity>();
            foreach (ResourceEntity existingResource in resources)
            {
                existingResourcesByName[existingResource.Name] = existingResource;
            }

            return existingResourcesByName;
        }

    }

}