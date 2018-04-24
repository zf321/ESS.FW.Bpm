using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Dmn.engine;

namespace ESS.FW.Bpm.Engine
{
    public interface IDmnDecisionTableResult: IList<IDmnDecisionRuleResult>
    {
        /// <summary>
        /// Returns the first <seealso cref="IDmnDecisionRuleResult"/>.
        /// </summary>
        /// <returns> the first decision rule result or null if none exits </returns>
        IDmnDecisionRuleResult GetFirstResult();

        /// <summary>
        /// Returns the single <seealso cref="IDmnDecisionRuleResult"/> of the result. Which asserts
        /// that only one decision rule result exist.
        /// </summary>
        /// <returns> the single decision rule result or null if none exists
        /// </returns>
        /// <exception cref="DmnEngineException">
        ///           if more than one decision rule result exists </exception>
        IDmnDecisionRuleResult GetSingleResult();

        /// <summary>
        /// Collects the entries for a output name. The list will contain entries for
        /// the output name of every <seealso cref="IDmnDecisionRuleResult"/>. Note that the list
        /// may contains less entries than decision rule results if an output does not
        /// contain a value for the output name.
        /// </summary>
        /// <param name="outputName">
        ///          the name of the output to collect </param>
        /// @param <T>
        ///          the type of the rule result entry </param>
        /// <returns> the list of collected output values </returns>
        IList<T> CollectEntries<T>(string outputName);

        /// <summary>
        /// Returns the entries of all decision rule results. For every decision rule
        /// result a map of the output names and corresponding entries is returned.
        /// </summary>
        /// <returns> the list of all entry maps
        /// </returns>
        /// <seealso cref= DmnDecisionRuleResult#getEntryMap() </seealso>
        IList<IDictionary<string, object>> GetResultList();

        /// <summary>
        /// Returns the value of the single entry of the decision rule result. Asserts that
        /// only one decision rule result with a single entry exist.
        /// </summary>
        /// @param <T>
        ///          the type of the result entry </param>
        /// <returns> the value of the single result entry or null if none exists
        /// </returns>
        /// <exception cref="DmnEngineException">
        ///           if more than one decision rule result or more than one result entry
        ///           exists
        /// </exception>
        /// <seealso cref= #getSingleEntryTyped() </seealso>
        T GetSingleEntry<T>();

        /// <summary>
        /// Returns the typed value of the single entry of the decision rule result. Asserts
        /// that only one decision rule result with a single entry exist.
        /// </summary>
        /// @param <T>
        ///          the type of the result entry </param>
        /// <returns> the typed value of the single result entry or null if none exists
        /// </returns>
        /// <exception cref="DmnEngineException">
        ///           if more than one decision rule result or more than one result entry
        ///           exists
        /// </exception>
        /// <seealso cref= #getSingleEntry() </seealso>
        T GetSingleEntryTyped<T>() ;

    }
}
