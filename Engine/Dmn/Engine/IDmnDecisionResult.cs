using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Dmn.engine
{
    /// <summary>
    ///     <para>
    ///         The result of one decision evaluation. It can be composed of multiple
    ///         <seealso cref="IDmnDecisionResultEntries" />s which represents the output entries (i.e.,
    ///         pairs of output name and value).
    ///     </para>
    ///     <para>
    ///         In case of a decision with a decision table, the result has one
    ///         <seealso cref="IDmnDecisionResultEntries" /> for each matched rule that contains the
    ///         output entries of this rule.
    ///     </para>
    /// </summary>
    public interface IDmnDecisionResult : IList<IDmnDecisionResultEntries>
    {
        /// <summary>
        ///     Returns the first <seealso cref="IDmnDecisionResultEntries" />.
        /// </summary>
        /// <returns> the first decision result or null if none exits </returns>
        IDmnDecisionResultEntries FirstResult { get; }

        /// <summary>
        ///     Returns the single <seealso cref="IDmnDecisionResultEntries" /> of the result. Asserts
        ///     that only one decision result exist.
        /// </summary>
        /// <returns>
        ///     the single decision result or null if none exists
        /// </returns>
        /// <exception cref="DmnEngineException">
        ///     if more than one decision result exists
        /// </exception>
        IDmnDecisionResultEntries SingleResult { get; }

        /// <summary>
        ///     Returns the entries of all decision results. For every decision result a
        ///     map of the output names and corresponding entries is returned.
        /// </summary>
        /// <returns>
        ///     the list of all entry maps
        /// </returns>
        /// <seealso cref= DmnDecisionResultEntries# getEntryMap
        /// (
        /// )
        /// </seealso>
        IList<IDictionary<string, object>> ResultList { get; }

        /// <summary>
        ///     Collects the entries for a output name. The list will contain entries for
        ///     the output name of every <seealso cref="IDmnDecisionResultEntries" />. Note that the
        ///     list may contains less entries than decision results if an output does not
        ///     contain a value for the output name.
        /// </summary>
        /// <param name="outputName">
        ///     the name of the output to collect
        /// </param>
        /// @param
        /// <T>
        ///     the type of the result entry </param>
        ///     <returns> the list of collected output values </returns>
        IList<T> CollectEntries<T>(string outputName);

        /// <summary>
        ///     Returns the value of the single entry of the decision result. Asserts that
        ///     only one decision result with a single entry exist.
        /// </summary>
        /// @param
        /// <T>
        ///     the type of the result entry </param>
        ///     <returns>
        ///         the value of the single result entry or null if none exists
        ///     </returns>
        ///     <exception cref="DmnEngineException">
        ///         if more than one decision result or more than one result entry
        ///         exists
        ///     </exception>
        ///     <seealso cref= # getSingleEntryTyped() </seealso>
        IDmnDecisionResultEntries GetSingleEntry();

        /// <summary>
        ///     Returns the typed value of the single entry of the decision result. Asserts
        ///     that only one decision result with a single entry exist.
        /// </summary>
        /// @param
        /// <T>
        ///     the type of the result entry </param>
        ///     <returns>
        ///         the typed value of the single result entry or null if none exists
        ///     </returns>
        ///     <exception cref="DmnEngineException">
        ///         if more than one decision result or more than one result entry
        ///         exists
        ///     </exception>
        ///     <seealso cref= # getSingleEntry() </seealso>
        T GetSingleEntryTyped<T>();
    }
}