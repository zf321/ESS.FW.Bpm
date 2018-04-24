using ESS.FW.Bpm.Engine.Application;

namespace ESS.FW.Bpm.Engine.Repository
{
    /// <summary>
    ///     <para>Builder for a <seealso cref="ProcessApplicationAttribute" /> deployment</para>
    ///     <para>
    ///         A process application deployment is different from a regular deployment.
    ///         Besides deploying a set of process definitions to the database,
    ///         this deployment has the additional side effect that the process application
    ///         is registered for the deployment. This means that the process engine will exeute
    ///         all process definitions contained in the deployment in the context of the process
    ///         application (by calling the process application's
    ///     </para>
    ///     <seealso cref="IProcessApplicationInterface#execute(java.Util.concurrent.Callable)" /> method.
    ///     <para>
    ///         
    ///     </para>
    /// </summary>
    public interface IProcessApplicationDeploymentBuilder : IDeploymentBuilder
    {
        /// <summary>
        ///     <para>
        ///         If this method is called, additional registrations will be created for
        ///         previous versions of the deployment.
        ///     </para>
        /// </summary>
        IProcessApplicationDeploymentBuilder ResumePreviousVersions();

        /// <summary>
        ///     This method defines on what additional registrations will be based.
        ///     The value will only be recognized if <seealso cref="#resumePreviousVersions()" /> is set.
        ///     <para>
        ///     </para>
        /// </summary>
        /// <seealso cref= ResumePreviousBy
        /// </seealso>
        /// <seealso cref= # resumePreviousVersions
        /// (
        /// )
        /// </seealso>
        /// <param name="resumeByProcessDefinitionKey"> one of the constants from <seealso cref="ResumePreviousBy" /> </param>
        IProcessApplicationDeploymentBuilder ResumePreviousVersionsBy(string resumePreviousVersionsBy);

        /* {@inheritDoc} */
        //IProcessApplicationDeployment Deploy();

        // overridden methods //////////////////////////////

        /* {@inheritDoc} */
        //IProcessApplicationDeploymentBuilder AddInputStream(string resourceName, Stream inputStream);
        /* {@inheritDoc} */
        //IProcessApplicationDeploymentBuilder AddClasspathResource(string resource);
        /* {@inheritDoc} */
        //IProcessApplicationDeploymentBuilder AddString(string resourceName, string text);
        /* {@inheritDoc} */
        //IProcessApplicationDeploymentBuilder AddModelInstance(string resourceName, IBpmnModelInstance modelInstance);
        /* {@inheritDoc} */
        //ProcessApplicationDeploymentBuilder addZipInputStream(ZipInputStream zipInputStream);
        /* {@inheritDoc} */
        //IProcessApplicationDeploymentBuilder Name(string name);
        ///* {@inheritDoc} */
        //IProcessApplicationDeploymentBuilder NameFromDeployment(string deploymentId);
        ///* {@inheritDoc} */
        //IProcessApplicationDeploymentBuilder Source(string source);
        ///* {@inheritDoc} */

        //[Obsolete]
        //IProcessApplicationDeploymentBuilder EnableDuplicateFiltering();

        ///* {@inheritDoc} */
        //IProcessApplicationDeploymentBuilder EnableDuplicateFiltering(bool deployChangedOnly);
        ///* {@inheritDoc} */
        //IProcessApplicationDeploymentBuilder ActivateProcessDefinitionsOn(DateTime date);

        ///* {@inheritDoc} */
        //IProcessApplicationDeploymentBuilder AddDeploymentResources(string deploymentId);

        ///* {@inheritDoc} */
        //IProcessApplicationDeploymentBuilder AddDeploymentResourceById(string deploymentId, string resourceId);
        ///* {@inheritDoc} */
        //IProcessApplicationDeploymentBuilder AddDeploymentResourcesById(string deploymentId, IList<string> resourceIds);

        ///* {@inheritDoc} */
        //IProcessApplicationDeploymentBuilder AddDeploymentResourceByName(string deploymentId, string resourceName);
        ///* {@inheritDoc} */

        //new

        /* {@inheritDoc} */
        //IProcessApplicationDeployment Deploy();

        // overridden methods //////////////////////////////

        /* {@inheritDoc} */
        //IProcessApplicationDeploymentBuilder AddInputStream(string resourceName, Stream inputStream);
        /* {@inheritDoc} */
        //IProcessApplicationDeploymentBuilder AddClasspathResource(string resource);
        /* {@inheritDoc} */
        //IProcessApplicationDeploymentBuilder AddString(string resourceName, string text);
        /* {@inheritDoc} */
        //IProcessApplicationDeploymentBuilder AddModelInstance(string resourceName, IBpmnModelInstance modelInstance);
        /* {@inheritDoc} */
        //ProcessApplicationDeploymentBuilder addZipInputStream(ZipInputStream zipInputStream);
        /* {@inheritDoc} */
        //IProcessApplicationDeploymentBuilder Name(string name);
        ///* {@inheritDoc} */
        //IProcessApplicationDeploymentBuilder NameFromDeployment(string deploymentId);
        ///* {@inheritDoc} */
        //IProcessApplicationDeploymentBuilder Source(string source);
        ///* {@inheritDoc} */

        //[Obsolete]
        //IProcessApplicationDeploymentBuilder EnableDuplicateFiltering();

        ///* {@inheritDoc} */
        //IProcessApplicationDeploymentBuilder EnableDuplicateFiltering(bool deployChangedOnly);
        ///* {@inheritDoc} */
        //IProcessApplicationDeploymentBuilder ActivateProcessDefinitionsOn(DateTime date);

        ///* {@inheritDoc} */
        //IProcessApplicationDeploymentBuilder AddDeploymentResources(string deploymentId);

        ///* {@inheritDoc} */
        //IProcessApplicationDeploymentBuilder AddDeploymentResourceById(string deploymentId, string resourceId);
        ///* {@inheritDoc} */
        //IProcessApplicationDeploymentBuilder AddDeploymentResourcesById(string deploymentId, IList<string> resourceIds);

        ///* {@inheritDoc} */
        //IProcessApplicationDeploymentBuilder AddDeploymentResourceByName(string deploymentId, string resourceName);
        ///* {@inheritDoc} */

        //IProcessApplicationDeploymentBuilder AddDeploymentResourcesByName(string deploymentId, IList<string> resourceNames);
    }
}