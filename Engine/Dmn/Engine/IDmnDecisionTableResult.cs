using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Dmn.engine
{
    /// <summary>
    ///     The result of one decision table. Which is the list of its decision rule results (see
    ///     <seealso cref="DmnDecisionRuleResult" />). This represents the output entries of all matching
    ///     decision rules.
    /// </summary>
    public interface IDmnDecisionTableResult : IList<IDmnDecisionRuleResult>
    {
        /// <summary>
        ///     Returns the first <seealso cref="DmnDecisionRuleResult" />.
        /// </summary>
        /// <returns> the first decision rule result or null if none exits </returns>
        IDmnDecisionRuleResult FirstResult { get; }

        /// <summary>
        ///     Returns the single <seealso cref="DmnDecisionRuleResult" /> of the result. Which asserts
        ///     that only one decision rule result exist.
        /// </summary>
        /// <returns>
        ///     the single decision rule result or null if none exists
        /// </returns>
        /// <exception cref="DmnEngineException">
        ///     if more than one decision rule result exists
        /// </exception>
        IDmnDecisionRuleResult SingleResult { get; }

        /// <summary>
        ///     Returns the entries of all decision rule results. For every decision rule
        ///     result a map of the output names and corresponding entries is returned.
        /// </summary>
        /// <returns>
        ///     the list of all entry maps
        /// </returns>
        /// <seealso cref= DmnDecisionRuleResult# getEntryMap
        /// (
        /// )
        /// </seealso>
        IList<IDictionary<string, object>> ResultList { get; }

        /// <summary>
        ///     Collects the entries for a output name. The list will contain entries for
        ///     the output name of every <seealso cref="DmnDecisionRuleResult" />. Note that the list
        ///     may contains less entries than decision rule results if an output does not
        ///     contain a value for the output name.
        /// </summary>
        /// <param name="outputName">
        ///     the name of the output to collect
        /// </param>
        /// @param
        /// <T>
        ///     the type of the rule result entry </param>
        ///     <returns> the list of collected output values </returns>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
        List<T> CollectEntries<T>(string outputName);

        /// <summary>
        ///     Returns the value of the single entry of the decision rule result. Asserts that
        ///     only one decision rule result with a single entry exist.
        /// </summary>
        /// @param
        /// <T>
        ///     the type of the result entry </param>
        ///     <returns>
        ///         the value of the single result entry or null if none exists
        ///     </returns>
        ///     <exception cref="DmnEngineException">
        ///         if more than one decision rule result or more than one result entry
        ///         exists
        ///     </exception>
        ///     <seealso cref= # getSingleEntryTyped() </seealso>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
        T GetSingleEntry<T>();

        /// <summary>
        ///     Returns the typed value of the single entry of the decision rule result. Asserts
        ///     that only one decision rule result with a single entry exist.
        /// </summary>
        /// @param
        /// <T>
        ///     the type of the result entry </param>
        ///     <returns>
        ///         the typed value of the single result entry or null if none exists
        ///     </returns>
        ///     <exception cref="DmnEngineException">
        ///         if more than one decision rule result or more than one result entry
        ///         exists
        ///     </exception>
        ///     <seealso cref= # getSingleEntry() </seealso>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
        T GetSingleEntryTyped<T>();
    }
}