using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Dmn.engine
{
    /// <summary>
    ///     The result of one decision rule. This represents the output entry
    ///     values of a matching decision rule. It is a mapping from the output
    ///     {@code name} attribute to the output value. If no {@code name}
    ///     was given the key is {@code null}.
    /// </summary>
    public interface IDmnDecisionRuleResult : IDictionary<string, object>
    {
        /// <summary>
        ///     Returns a map of the rule result entry values by output name.
        /// </summary>
        /// <returns>
        ///     the values of the decision rule result entries
        /// </returns>
        /// <seealso cref= # getEntryMapTyped
        /// (
        /// )
        /// </seealso>
        IDictionary<string, object> EntryMap { get; }

        /// <summary>
        ///     Returns a map of the typed rule result entry values by output name.
        /// </summary>
        /// <returns>
        ///     the typed values of the decision rule result entries
        /// </returns>
        /// <seealso cref= # getEntryMap
        /// (
        /// )
        /// </seealso>
        IDictionary<string, ITypedValue> EntryMapTyped { get; }

        /// <summary>
        ///     Returns the value of the first rule result entry.
        /// </summary>
        /// @param
        /// <T>
        ///     the type of the rule result entry </param>
        ///     <returns>
        ///         the value of the first rule result entry or null if none exists
        ///     </returns>
        ///     <seealso cref= # getFirstEntryTyped() </seealso>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
        T getFirstEntry<T>();

        /// <summary>
        ///     Returns the typed value of the first rule result entry.
        /// </summary>
        /// @param
        /// <T>
        ///     the type of the rule result entry </param>
        ///     <returns>
        ///         the typed value of the first rule result entry or null if none exists
        ///     </returns>
        ///     <seealso cref= # getFirstEntry() </seealso>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
        T getFirstEntryTyped<T>();

        /// <summary>
        ///     Returns the value of the single entry of the decision rule result.
        ///     Which asserts that the decision rule result only has one entry.
        /// </summary>
        /// @param
        /// <T>
        ///     the type of the rule result entry </param>
        ///     <returns>
        ///         the value of the single rule result entry or null if none exists
        ///     </returns>
        ///     <exception cref="DmnEngineException">
        ///         if more than one rule result entry exists
        ///     </exception>
        ///     <seealso cref= # getSingleEntryTyped() </seealso>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
        T getSingleEntry<T>();

        /// <summary>
        ///     Returns the typed value of the single entry of the decision rule result.
        ///     Which asserts that the decision rule result only has one entry.
        /// </summary>
        /// @param
        /// <T>
        ///     the type of the rule result entry </param>
        ///     <returns>
        ///         the typed value of the single rule result entry or null if none exists
        ///     </returns>
        ///     <exception cref="DmnEngineException">
        ///         if more than one rule result entry exists
        ///     </exception>
        ///     <seealso cref= # getSingleEntry() </seealso>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
        T getSingleEntryTyped<T>();

        /// <summary>
        ///     Returns the value of the rule result entry for a given output name.
        /// </summary>
        /// <param name="name">
        ///     the name of the output
        /// </param>
        /// @param
        /// <T>
        ///     the type of the rule result entry </param>
        ///     <returns>
        ///         the value for the given name or null if no value exists for
        ///         this name
        ///     </returns>
        ///     <seealso cref= # getEntryTyped( String) </seealso>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
        T getEntry<T>(string name);

        /// <summary>
        ///     Returns the typed value of the rule result entry for a given output name.
        /// </summary>
        /// <param name="name">
        ///     the name of the output
        /// </param>
        /// @param
        /// <T>
        ///     the type of the rule result entry </param>
        ///     <returns>
        ///         the typed value for the given name or null if no value exists for
        ///         this name
        ///     </returns>
        ///     <seealso cref= # getEntry( String) </seealso>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
        T getEntryTyped<T>(string name);
    }
}