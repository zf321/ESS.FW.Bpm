using System;

namespace ESS.FW.Bpm.Engine.Repository
{
    /// <summary>
    ///     Represents a deployment that is already present in the process repository.
    ///     A deployment is a container for resources such as process definitions, images, forms, etc.
    ///     When a deployment is 'deployed' through the <seealso cref="IRepositoryService" />,
    ///     the engine will recognize certain of such resource types and act upon
    ///     them (e.g. process definitions will be parsed to an executable Java artifact).
    ///     To create a Deployment, use the <seealso cref="org.camunda.bpm.engine.repository.DeploymentBuilder" />.
    ///     A Deployment on itself is a <b>read-only</b> object and its content cannot be
    ///     changed after deployment (hence the builder that needs to be used).
    ///      
    ///     
    /// </summary>
    public interface IDeployment
    {
        string Id { get; }

        string Name { get; }

        DateTime DeploymentTime { get; }

        string Source { get; }

        /// <summary>
        ///     Returns the id of the tenant this deployment belongs to. Can be <code>null</code>
        ///     if the deployment belongs to no single tenant.
        /// </summary>
        string TenantId { get; }
    }
}