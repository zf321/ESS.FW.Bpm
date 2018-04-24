using ESS.FW.Bpm.Engine.Dmn.engine;

namespace ESS.FW.Bpm.Engine.Dmn.Impl.Result
{
    /// <summary>
    ///     Mapping function for a decision result.
    ///     
    /// </summary>
    public interface IDecisionResultMapper
    {
        /// <summary>
        ///     Maps the decision result into a value that can set as process variable.
        /// </summary>
        /// <param name="decisionResult">
        ///     the result of the evaluated decision
        /// </param>
        /// <returns> the value that should set as process variable </returns>
        /// <exception cref="ProcessEngineException">
        ///     if the decision result can not be mapped
        /// </exception>
        object MapDecisionResult(IDmnDecisionResult decisionResult);
    }
}