

using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Repository
{
    /// <summary>
    ///     An extension of the deployment interface to expose the deployed definitions.
    ///      
    /// </summary>
    public interface IDeploymentWithDefinitions : IDeployment
    {
        /// <summary>
        ///     Returns the process definitions, which are deployed with that deployment.
        /// </summary>
        /// <returns> the process definitions which are deployed </returns>
        IList<IProcessDefinition> DeployedProcessDefinitions { get; }

        /// <summary>
        ///     Returns the case definitions, which are deployed with that deployment.
        /// </summary>
        /// <returns> the case definitions, which are deployed </returns>
        IList<ICaseDefinition> DeployedCaseDefinitions { get; }

        /// <summary>
        ///     Returns the decision definitions, which are deployed with that deployment
        /// </summary>
        /// <returns> the decision definitions, which are deployed </returns>
        IList<IDecisionDefinition> DeployedDecisionDefinitions { get; }

        /// <summary>
        ///     Returns the decision requirements definitions, which are deployed with that deployment
        /// </summary>
        /// <returns> the decision definitions, which are deployed </returns>
        IList<IDecisionRequirementsDefinition> DeployedDecisionRequirementsDefinitions { get; }
    }
}