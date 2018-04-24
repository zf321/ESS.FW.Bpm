using System;
using System.Collections.Generic;
using System.IO;
using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.Impl;

namespace ESS.FW.Bpm.Engine.Repository.Impl
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class ProcessApplicationDeploymentBuilderImpl : DeploymentBuilderImpl, IProcessApplicationDeploymentBuilder
    {
        private const long SerialVersionUid = 1L;

        protected internal bool isResumePreviousVersions;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal string ResumePreviousVersionsByRenamed = ResumePreviousBy.ResumeByProcessDefinitionKey;

        public ProcessApplicationDeploymentBuilderImpl(RepositoryServiceImpl repositoryService,
            IProcessApplicationReference reference) : base(repositoryService)
        {
            ProcessApplicationReference = reference;
            Source(ProcessApplicationDeploymentFields.ProcessApplicationDeploymentSource);
        }

        // getters / setters ///////////////////////////////////////////////

        public virtual bool IsResumePreviousVersions()
        {
            return isResumePreviousVersions; 
        }

        public virtual IProcessApplicationReference ProcessApplicationReference { get; }

        public virtual string ResumePreviousVersionsBy_Renamed()
        {
           return ResumePreviousVersionsByRenamed;
        }

        public virtual IProcessApplicationDeploymentBuilder ResumePreviousVersions()
        {
            isResumePreviousVersions = true;
            return this;
        }

        public virtual IProcessApplicationDeploymentBuilder ResumePreviousVersionsBy(string resumePreviousVersionsBy)
        {
            ResumePreviousVersionsByRenamed = resumePreviousVersionsBy;
            return this;
        }
        // overrides from parent ////////////////////////////////////////////////

        public override IDeployment Deploy()
        {
            return (IProcessApplicationDeployment) base.Deploy();
        }

        //public override ProcessApplicationDeploymentBuilderImpl activateProcessDefinitionsOn(DateTime date)
        //{
        //    return (ProcessApplicationDeploymentBuilderImpl) base.activateProcessDefinitionsOn(date);
        //}

        public override IDeploymentBuilder AddInputStream(string resourceName, Stream inputStream)
        {
            return (ProcessApplicationDeploymentBuilderImpl) base.AddInputStream(resourceName, inputStream);
        }

        public override IDeploymentBuilder AddClasspathResource(string resource)
        {
            return (ProcessApplicationDeploymentBuilderImpl) base.AddClasspathResource(resource);
        }

        public override IDeploymentBuilder AddString(string resourceName, string text)
        {
            return base.AddString(resourceName, text);
        }

        //public  ProcessApplicationDeploymentBuilderImpl AddModelInstance(string resourceName,
        //    IBpmnModelInstance modelInstance)
        //{
        //    return (ProcessApplicationDeploymentBuilderImpl)base.addModelInstance(resourceName, modelInstance);
        //}

        //public override ProcessApplicationDeploymentBuilderImpl addZipInputStream(ZipInputStream zipInputStream)
        //{
        //    return (ProcessApplicationDeploymentBuilderImpl)base.addZipInputStream(zipInputStream);
        //}

        public override IDeploymentBuilder Name(string name)
        {
            return (ProcessApplicationDeploymentBuilderImpl) base.Name(name);
        }

        public override IDeploymentBuilder TenantId(string tenantId)
        {
            return (ProcessApplicationDeploymentBuilderImpl) base.TenantId(tenantId);
        }

        public override IDeploymentBuilder SetNameFromDeployment(string deploymentId)
        {
            return (ProcessApplicationDeploymentBuilderImpl) base.SetNameFromDeployment(deploymentId);
        }

        public override IDeploymentBuilder Source(string source)
        {
            return (ProcessApplicationDeploymentBuilderImpl) base.Source(source);
        }

        public override IDeploymentBuilder EnableDuplicateFiltering()
        {
            return (ProcessApplicationDeploymentBuilderImpl) base.EnableDuplicateFiltering();
        }

        public override IDeploymentBuilder EnableDuplicateFiltering(bool deployChangedOnly)
        {
            return (ProcessApplicationDeploymentBuilderImpl) base.EnableDuplicateFiltering(deployChangedOnly);
        }

        public override IDeploymentBuilder AddDeploymentResources(string deploymentId)
        {
            return (ProcessApplicationDeploymentBuilderImpl) base.AddDeploymentResources(deploymentId);
        }

        public override  IDeploymentBuilder  AddDeploymentResourceById(string deploymentId,
            string resourceId)
        {
            return (ProcessApplicationDeploymentBuilderImpl) base.AddDeploymentResourceById(deploymentId, resourceId);
        }

        public override  IDeploymentBuilder  AddDeploymentResourcesById(string deploymentId,
            IList<string> resourceIds)
        {
            return (ProcessApplicationDeploymentBuilderImpl) base.AddDeploymentResourcesById(deploymentId, resourceIds);
        }

        public override  IDeploymentBuilder  AddDeploymentResourceByName(string deploymentId,
            string resourceName)
        {
            return
                (ProcessApplicationDeploymentBuilderImpl) base.AddDeploymentResourceByName(deploymentId, resourceName);
        }

        public override  IDeploymentBuilder  AddDeploymentResourcesByName(string deploymentId,
            IList<string> resourceNames)
        {
            return
                (ProcessApplicationDeploymentBuilderImpl) base.AddDeploymentResourcesByName(deploymentId, resourceNames);
        }

    }
}