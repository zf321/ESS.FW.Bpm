namespace ESS.FW.Bpm.Engine.Repository
{
    /// <summary>
    ///     Definition of a decision resource
    /// </summary>
    public interface IDecisionDefinition : IResourceDefinition
    {
        /// <summary>
        ///     Returns the id of the related decision requirements definition. Can be
        ///     <code>null</code> if the decision has no relations to other decisions.
        /// </summary>
        /// <returns> the id of the decision requirements definition if exists. </returns>
        string DecisionRequirementsDefinitionId { get; }

        /// <summary>
        ///     Returns the key of the related decision requirements definition. Can be
        ///     <code>null</code> if the decision has no relations to other decisions.
        /// </summary>
        /// <returns> the key of the decision requirements definition if exists. </returns>
        string DecisionRequirementsDefinitionKey { get; }
    }
}