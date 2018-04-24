using System;
using System.Collections.Generic;
using System.IO;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Dmn;

namespace ESS.FW.Bpm.Engine.Repository.Impl
{
    /// <summary>
    ///      
    ///     
    /// </summary>
    [Serializable]
    public class DeploymentBuilderImpl : IDeploymentBuilder
    {
        private const long SerialVersionUid = 1L;
        protected internal bool deployChangedOnly=false;
        protected internal DeploymentEntity deployment = new DeploymentEntity();

        protected internal IDictionary<string, ISet<string>> deploymentResourcesById =
            new Dictionary<string, ISet<string>>();

        protected internal IDictionary<string, ISet<string>> deploymentResourcesByName =
            new Dictionary<string, ISet<string>>();

        protected internal ISet<string> deployments = new HashSet<string>();
        protected internal bool isDuplicateFilterEnabled=false;

        //JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal string nameFromDeployment;
        protected internal DateTime? processDefinitionsActivationDate;

        [NonSerialized]
        protected internal RepositoryServiceImpl RepositoryService;

        public DeploymentBuilderImpl(RepositoryServiceImpl repositoryService)
        {
            this.RepositoryService = repositoryService;
        }

        // getters and setters //////////////////////////////////////////////////////

        public virtual DeploymentEntity Deployment
        {
            get { return deployment; }
        }

        public virtual bool IsDuplicateFilterEnabled
        {
            get { return isDuplicateFilterEnabled; }
        }

        public virtual bool DeployChangedOnly
        {
            get { return deployChangedOnly; }
        }
        /// <summary>
        /// 流程定义激活时间
        /// </summary>
        public virtual DateTime? ProcessDefinitionsActivationDate
        {
            get { return processDefinitionsActivationDate; }
        }

        public virtual string GetNameFromDeployment() => nameFromDeployment;

        public virtual ISet<string> Deployments
        {
            get { return deployments; }
        }

        public virtual IDictionary<string, ISet<string>> DeploymentResourcesById
        {
            get { return deploymentResourcesById; }
        }

        public virtual IDictionary<string, ISet<string>> DeploymentResourcesByName
        {
            get { return deploymentResourcesByName; }
        }

        public ICollection<string> ResourceNames
        {
            get
            {
                if (deployment.Resources == null)
                {
                    return new List<string>();
                }
                else
                {
                    return deployment.Resources.Keys;
                }
            }
        }

        public virtual IDeploymentBuilder AddInputStream(string resourceName, Stream inputStream)
        {
            EnsureUtil.EnsureNotNull("inputStream for resource '" + resourceName + "' is null", "inputStream",
                inputStream);
            var bytes = IoUtil.ReadInputStream(inputStream, resourceName);

            return AddBytes(resourceName, bytes);
        }

        public virtual IDeploymentBuilder AddClasspathResource(string resource)
        {
            var inputStream = ReflectUtil.GetResourceAsStream(resource);
            EnsureUtil.EnsureNotNull("resource '" + resource + "' not found", "inputStream", inputStream);
            return AddInputStream(resource, inputStream);
        }

        public virtual IDeploymentBuilder AddString(string resourceName, string text)
        {
            EnsureUtil.EnsureNotNull("text", text);

            var bytes = (RepositoryService != null) && (RepositoryService.DeploymentCharset != null)
                ? text.GetBytes(RepositoryService.DeploymentCharset)
                : text.GetBytes();

            return AddBytes(resourceName, bytes);
        }

        // public virtual DeploymentBuilder addModelInstance(string resourceName, CmmnModelInstance modelInstance)
        // {
        //EnsureUtil.EnsureNotNull("modelInstance", modelInstance);

        //System.IO.MemoryStream outputStream = new System.IO.MemoryStream();
        //Cmmn.writeModelToStream(outputStream, modelInstance);

        //return addBytes(resourceName, outputStream.toByteArray());
        // }

        public virtual IDeploymentBuilder AddModelInstance(string resourceName, IBpmnModelInstance modelInstance)
        {
            EnsureUtil.EnsureNotNull("modelInstance", modelInstance);
            var outputStream = new MemoryStream();
            Model.Bpmn.Bpmn.WriteModelToStream(outputStream, modelInstance);

            return AddBytes(resourceName, outputStream.ToArray());
        }


        public virtual IDeploymentBuilder AddModelInstance(string resourceName, IDmnModelInstance modelInstance)
        {
            EnsureUtil.EnsureNotNull("modelInstance", modelInstance);

            var outputStream = new MemoryStream();
            Model.Dmn.Dmn.WriteModelToStream(outputStream, modelInstance);

            return AddBytes(resourceName, outputStream.ToArray());
        }

        //public virtual DeploymentBuilder addZipInputStream(ZipInputStream zipInputStream)
        //{
        //    try
        //    {
        //        ZipEntry entry = zipInputStream.NextEntry;
        //        while (entry != null)
        //        {
        //            if (!entry.Directory)
        //            {
        //                string entryName = entry.Name;
        //                addInputStream(entryName, zipInputStream);
        //            }
        //            entry = zipInputStream.NextEntry;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new ProcessEngineException("problem reading zip input stream", e);
        //    }
        //    return this;
        //}

        public virtual IDeploymentBuilder AddDeploymentResources(string deploymentId)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "deploymentId", deploymentId);
            deployments.Add(deploymentId);
            return this;
        }

        public virtual IDeploymentBuilder AddDeploymentResourceById(string deploymentId, string resourceId)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "deploymentId", deploymentId);
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "resourceId", resourceId);

            CollectionUtil.AddToMapOfSets(deploymentResourcesById, deploymentId, resourceId);

            return this;
        }

        public virtual IDeploymentBuilder AddDeploymentResourcesById(string deploymentId, IList<string> resourceIds)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "deploymentId", deploymentId);

            EnsureUtil.EnsureNotNull(typeof(NotValidException), "resourceIds", resourceIds);
            EnsureUtil.EnsureNotEmpty(typeof(NotValidException), "resourceIds", resourceIds);
            EnsureUtil.EnsureNotContainsNull(typeof(NotValidException), "resourceIds", resourceIds);

            CollectionUtil.AddCollectionToMapOfSets(deploymentResourcesById, deploymentId, resourceIds);

            return this;
        }

        public virtual IDeploymentBuilder AddDeploymentResourceByName(string deploymentId, string resourceName)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "deploymentId", deploymentId);
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "resourceName", resourceName);

            CollectionUtil.AddToMapOfSets(deploymentResourcesByName, deploymentId, resourceName);

            return this;
        }

        public virtual IDeploymentBuilder AddDeploymentResourcesByName(string deploymentId, IList<string> resourceNames)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "deploymentId", deploymentId);

            EnsureUtil.EnsureNotNull(typeof(NotValidException), "resourceNames", resourceNames);
            EnsureUtil.EnsureNotEmpty(typeof(NotValidException), "resourceNames", resourceNames);
            EnsureUtil.EnsureNotContainsNull(typeof(NotValidException), "resourceNames", resourceNames);

            CollectionUtil.AddCollectionToMapOfSets(deploymentResourcesByName, deploymentId, resourceNames);

            return this;
        }

        public virtual IDeploymentBuilder Name(string name)
        {
            if (!string.IsNullOrEmpty(nameFromDeployment))
            {
                var message =
                    string.Format(
                        "Cannot set the deployment name to '{0}', because the property 'nameForDeployment' has been already set to '{1}'.",
                        name, nameFromDeployment);
                throw new NotValidException(message);
            }
            deployment.Name = name;
            return this;
        }

        public virtual IDeploymentBuilder SetNameFromDeployment(string deploymentId)
        {
            var name = deployment.Name;
            if (!string.IsNullOrEmpty(name))
            {
                var message =
                    string.Format(
                        "Cannot set the given deployment id '{0}' to get the name from it, because the deployment name has been already set to '{1}'.",
                        deploymentId, name);
                throw new NotValidException(message);
            }
            nameFromDeployment = deploymentId;
            return this;
        }

        public virtual IDeploymentBuilder EnableDuplicateFiltering()
        {
            return EnableDuplicateFiltering(false);
        }

        public virtual IDeploymentBuilder EnableDuplicateFiltering(bool deployChangedOnly)
        {
            isDuplicateFilterEnabled = true;
            this.deployChangedOnly = deployChangedOnly;
            return this;
        }

        public virtual IDeploymentBuilder ActivateProcessDefinitionsOn(DateTime date)
        {
            processDefinitionsActivationDate = date;
            return this;
        }

        public virtual IDeploymentBuilder Source(string source)
        {
            deployment.Source = source;
            return this;
        }

        public virtual IDeploymentBuilder TenantId(string tenantId)
        {
            deployment.TenantId = tenantId;
            return this;
        }

        public virtual IDeployment Deploy()
        {
            return DeployAndReturnDefinitions();
        }

        public virtual IDeploymentWithDefinitions DeployAndReturnDefinitions()
        {
            return RepositoryService.DeployAndReturnDefinitions(this);
        }
        public IDeploymentWithDefinitions DeployWithResult()
        {
            return RepositoryService.DeployWithResult(this);
        }


        //public virtual ICollection<string> ResourceNames
        //{
        //    get
        //    {
        //        if (deployment.Resources == null)
        //        {
        //            return null;
        //        }
        //        return deployment.Resources.Keys;
        //    }
        //}

        protected internal virtual IDeploymentBuilder AddBytes(string resourceName, byte[] bytes)
        {
            
            var resource = new ResourceEntity();
            resource.Bytes = bytes;
            resource.Name = resourceName;
            deployment.AddResource(resource);
            return this;
        }
    }
}