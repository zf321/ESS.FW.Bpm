using System;
using System.Collections.Generic;
using System.IO;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Dmn;

namespace ESS.FW.Bpm.Engine.Repository
{
    //using CmmnModelInstance = model.cmmn.CmmnModelInstance;

    /// <summary>
    ///     <para>Builder for creating new deployments.</para>
    ///     <para>
    ///         A builder instance can be obtained through
    ///         <seealso cref="IRepositoryService.createDeployment()" />.
    ///     </para>
    ///     <para>
    ///         Multiple resources can be added to one deployment before calling the <seealso cref=".deploy()" />
    ///         operation.
    ///     </para>
    ///     <para>
    ///         After deploying, no more changes can be made to the returned deployment
    ///         and the builder instance can be disposed.
    ///     </para>
    ///     <para>Valid resource extensions:</para>
    ///     <table>
    ///         <thead>
    ///             <tr>
    ///                 <th>Extension</th><th>Expected content</th>
    ///             </tr>
    ///             <thead>
    ///                 <tbody>
    ///                     <tr>
    ///                         <td>*.bpmn20.xml,.bpmn</td><td>BPMN process definition</td>
    ///                     </tr>
    ///                     <tr>
    ///                         <td>*.cmmn11.xml,.cmmn10.xml,.cmmn</td><td>CMMN case definition</td>
    ///                     </tr>
    ///                     <tr>
    ///                         <td>*.dmn11.xml,.dmn</td><td>DMN decision table</td>
    ///                     </tr>
    ///                     <tr>
    ///                         <td>*.png,.jpg,.gif,.svg</td>
    ///                         <td>
    ///                             Diagram image. The diagram file is considered to represent the specific diagram model
    ///                             by file name, e.g. bpmnDiagram1.png will be considered to be a diagram for
    ///                             bpmnDiagram1.bpmn20.xml
    ///                         </td>
    ///                     </tr>
    ///                 </tbody>
    ///     </table>
    ///      
    ///     
    /// </summary>
    public interface IDeploymentBuilder
    {
        /// <returns> the names of the resources which were added to this builder. </returns>
        ICollection<string> ResourceNames { get; }

        IDeploymentBuilder AddInputStream(string resourceName, Stream inputStream);
        IDeploymentBuilder AddClasspathResource(string resource);
        IDeploymentBuilder AddString(string resourceName, string text);
        IDeploymentBuilder AddModelInstance(string resourceName, IBpmnModelInstance modelInstance);
        IDeploymentBuilder AddModelInstance(string resourceName, IDmnModelInstance modelInstance);
        //DeploymentBuilder addModelInstance(string resourceName, CmmnModelInstance modelInstance);

        //DeploymentBuilder addZipInputStream(ZipInputStream zipInputStream);

        /// <summary>
        ///     All existing resources contained by the given deployment
        ///     will be added to the new deployment to re-deploy them.
        /// </summary>
        /// <exception cref="NotValidException"> if deployment id is null. </exception>
        IDeploymentBuilder AddDeploymentResources(string deploymentId);

        /// <summary>
        ///     A given resource specified by id and deployment id will be added
        ///     to the new deployment to re-deploy the given resource.
        /// </summary>
        /// <exception cref="NotValidException"> if either deployment id or resource id is null. </exception>
        IDeploymentBuilder AddDeploymentResourceById(string deploymentId, string resourceId);

        /// <summary>
        ///     All given resources specified by id and deployment id will be added
        ///     to the new deployment to re-deploy the given resource.
        /// </summary>
        /// <exception cref="NotValidException"> if either deployment id or the list of resource ids is null. </exception>
        IDeploymentBuilder AddDeploymentResourcesById(string deploymentId, IList<string> resourceIds);

        /// <summary>
        ///     A given resource specified by name and deployment id will be added
        ///     to the new deployment to re-deploy the given resource.
        /// </summary>
        /// <exception cref="NotValidException"> if either deployment id or resource name is null. </exception>
        IDeploymentBuilder AddDeploymentResourceByName(string deploymentId, string resourceName);

        /// <summary>
        ///     All given resources specified by name and deployment id will be added
        ///     to the new deployment to re-deploy the given resource.
        /// </summary>
        /// <exception cref="NotValidException"> if either deployment id or the list of resource names is null. </exception>
        IDeploymentBuilder AddDeploymentResourcesByName(string deploymentId, IList<string> resourceNames);

        /// <summary>
        ///     Gives the deployment the given name.
        /// </summary>
        /// <exception cref="NotValidException">
        ///     if <seealso cref=".nameFromDeployment(String)" /> has been called before.
        /// </exception>
        IDeploymentBuilder Name(string name);

        /// <summary>
        ///     Sets the deployment id to retrieve the deployment name from it.
        /// </summary>
        /// <exception cref="NotValidException">
        ///     if <seealso cref=".name(String)" /> has been called before.
        /// </exception>
        IDeploymentBuilder SetNameFromDeployment(string deploymentId);

        /// <summary>
        ///     <para>
        ///         If set, this deployment will be compared to any previous deployment.
        ///         This means that every (non-generated) resource will be compared with the
        ///         provided resources of this deployment. If any resource of this deployment
        ///         is different to the existing resources, <i>all</i> resources are re-deployed.
        ///     </para>
        ///     <para><b>Deprecated</b>: use <seealso cref=".enableDuplicateFiltering(boolean)" /></para>
        /// </summary>
        [Obsolete]
        IDeploymentBuilder EnableDuplicateFiltering();

        /// <summary>
        ///     Check the resources for duplicates in the set of previous deployments with
        ///     same deployment source. If no resources have changed in this deployment,
        ///     its contained resources are not deployed at all. For further configuration,
        ///     use the parameter <code>deployChangedOnly</code>.
        /// </summary>
        /// <param name="deployChangedOnly">
        ///     determines whether only those resources should be
        ///     deployed that have changed from the previous versions of the deployment.
        ///     If false, all of the resources are re-deployed if any resource differs.
        /// </param>
        IDeploymentBuilder EnableDuplicateFiltering(bool deployChangedOnly);

        /// <summary>
        ///     Sets the date on which the process definitions contained in this deployment
        ///     will be activated. This means that all process definitions will be deployed
        ///     as usual, but they will be suspended from the start until the given activation date.
        /// </summary>
        IDeploymentBuilder ActivateProcessDefinitionsOn(DateTime date);

        /// <summary>
        ///     <para>Sets the source of a deployment.</para>
        ///     <para>
        ///         Furthermore if duplicate check of deployment resources is enabled (by calling
        ///         <seealso cref=".enableDuplicateFiltering(boolean)" />) then only previous deployments
        ///         with the same given source are considered to perform the duplicate check.
        ///     </para>
        /// </summary>
        IDeploymentBuilder Source(string source);

        /// <summary>
        ///     Deploys all provided sources to the process engine and returns the created deployment.
        /// </summary>
        /// <exception cref="NotFoundException">
        ///     thrown
        ///     <ul>
        ///         <li>if the deployment specified by <seealso cref=".nameFromDeployment(String)" /> does not exist or</li>
        ///         <li>
        ///             if at least one of given deployments provided by <seealso cref=".addDeploymentResources(String)" /> does
        ///             not exist.
        ///         </li>
        ///     </ul>
        /// </exception>
        /// <exception cref="NotValidException">
        ///     if there are duplicate resource names from different deployments to re-deploy.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     thrown if the current user does not possess the following permissions:
        ///     <ul>
        ///         <li><seealso cref="Permissions.CREATE" /> on <seealso cref="Resources.DEPLOYMENT" /></li>
        ///         <li>
        ///             <seealso cref="Permissions.READ" /> on <seealso cref="Resources.DEPLOYMENT" /> (if resources from previous
        ///             deployments are redeployed)
        ///         </li>
        ///     </ul>
        /// </exception>
        /// <returns> the created deployment </returns>
        /// @deprecated use
        /// <seealso cref=".deployAndReturnDefinitions()" />
        /// instead.
        [Obsolete("实际调用deployAndReturnDefinitions use <seealso cref=\".deployAndReturnDefinitions()\"/> instead.")]
        IDeployment Deploy();

        /// <summary>
        ///     Deploys all provided sources to the process engine and returns the created deployment with the deployed
        ///     definitions.
        /// </summary>
        /// <exception cref="NotFoundException">
        ///     thrown
        ///     <ul>
        ///         <li>if the deployment specified by <seealso cref=".nameFromDeployment(String)" /> does not exist or</li>
        ///         <li>
        ///             if at least one of given deployments provided by <seealso cref=".addDeploymentResources(String)" /> does
        ///             not exist.
        ///         </li>
        ///     </ul>
        /// </exception>
        /// <exception cref="NotValidException">
        ///     if there are duplicate resource names from different deployments to re-deploy.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     thrown if the current user does not possess the following permissions:
        ///     <ul>
        ///         <li><seealso cref="Permissions.CREATE" /> on <seealso cref="Resources.DEPLOYMENT" /></li>
        ///         <li>
        ///             <seealso cref="Permissions.READ" /> on <seealso cref="Resources.DEPLOYMENT" /> (if resources from previous
        ///             deployments are redeployed)
        ///         </li>
        ///     </ul>
        /// </exception>
        /// <returns> the created deployment, contains the deployed definitions </returns>
        IDeploymentWithDefinitions DeployAndReturnDefinitions();
        /// <summary>
        /// Deploys all provided sources to the process engine and returns the created deployment with the deployed definitions.
        /// </summary>
        /// <exception cref="NotFoundException"> thrown
        ///  <ul>
        ///    <li>if the deployment specified by <seealso cref=".nameFromDeployment(String)"/> does not exist or</li>
        ///    <li>if at least one of given deployments provided by <seealso cref=".addDeploymentResources(String)"/> does not exist.</li>
        ///  </ul>
        /// </exception>
        /// <exception cref="NotValidException">
        ///    if there are duplicate resource names from different deployments to re-deploy.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///  thrown if the current user does not possess the following permissions:
        ///   <ul>
        ///     <li><seealso cref="Permissions.CREATE"/> on <seealso cref="Resources.DEPLOYMENT"/></li>
        ///     <li><seealso cref="Permissions.READ"/> on <seealso cref="Resources.DEPLOYMENT"/> (if resources from previous deployments are redeployed)</li>
        ///   </ul> </exception>
        /// <returns> the created deployment, contains the deployed definitions </returns>
        IDeploymentWithDefinitions DeployWithResult();

        /// <summary>
        ///     Sets the tenant id of a deployment.
        /// </summary>
        IDeploymentBuilder TenantId(string tenantId);
    }
}