using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Dmn.engine
{
    /// <summary>
    ///     <para>
    ///         Represents the output entries (i.e., pairs of output name and value).
    ///     </para>
    ///     <para>
    ///         In case of a decision with a decision table, the result contains the output
    ///         entries of a matched rule. Each output value is mapped to the output
    ///         {@code name} attribute. If no {@code name} was given then the entry key is
    ///         {@code null}.
    ///     </para>
    /// </summary>
    public interface IDmnDecisionResultEntries : IDictionary<string, object>
    {
        /// <summary>
        ///     Returns a map of the result entry values by output name.
        /// </summary>
        /// <returns>
        ///     the values of the decision result entries
        /// </returns>
        /// <seealso cref= getEntryMapTyped
        /// (
        /// )
        /// </seealso>
        IDictionary<string, object> EntryMap { get; }

        /// <summary>
        ///     Returns a map of the typed result entry values by output name.
        /// </summary>
        /// <returns>
        ///     the typed values of the decision result entries
        /// </returns>
        /// <seealso cref= # getEntryMap
        /// (
        /// )
        /// </seealso>
        IDictionary<string, ITypedValue> EntryMapTyped { get; }

        /// <summary>
        ///     Returns the value of the first result entry.
        /// </summary>
        /// @param
        /// <T>
        ///     the type of the result entry </param>
        ///     <returns>
        ///         the value of the first result entry or null if none exists
        ///     </returns>
        ///     <seealso cref= # getFirstEntryTyped() </seealso>
        object GetFirstEntry();

        /// <summary>
        ///     Returns the typed value of the first result entry.
        /// </summary>
        /// @param
        /// <T>
        ///     the type of the result entry </param>
        ///     <returns>
        ///         the typed value of the first result entry or null if none exists
        ///     </returns>
        ///     <seealso cref= # getFirstEntry() </seealso>
        T GetFirstEntryTyped<T>();

        /// <summary>
        ///     Returns the value of the single entry of the decision result. Asserts that
        ///     the decision result only has one entry.
        /// </summary>
        /// @param
        /// <T>
        ///     the type of the result entry </param>
        ///     <returns>
        ///         the value of the single result entry or null if none exists
        ///     </returns>
        ///     <exception cref="DmnEngineException">
        ///         if more than one result entry exists
        ///     </exception>
        ///     <seealso cref= # getSingleEntryTyped() </seealso>
        T GetSingleEntry<T>();

        /// <summary>
        ///     Returns the typed value of the single entry of the decision result. Asserts
        ///     that the decision result only has one entry.
        /// </summary>
        /// @param
        /// <T>
        ///     the type of the result entry </param>
        ///     <returns>
        ///         the typed value of the single result entry or null if none exists
        ///     </returns>
        ///     <exception cref="DmnEngineException">
        ///         if more than one result entry exists
        ///     </exception>
        ///     <seealso cref= # getSingleEntry() </seealso>
        T GetSingleEntryTyped<T>();

        /// <summary>
        ///     Returns the value of the result entry for a given output name.
        /// </summary>
        /// <param name="name">
        ///     the name of the output
        /// </param>
        /// @param
        /// <T>
        ///     the type of the result entry </param>
        ///     <returns>
        ///         the value for the given name or null if no value exists for this
        ///         name
        ///     </returns>
        ///     <seealso cref= # getEntryTyped( String) </seealso>
        T GetEntry<T>(string name);

        /// <summary>
        ///     Returns the typed value of the result entry for a given output name.
        /// </summary>
        /// <param name="name">
        ///     the name of the output
        /// </param>
        /// @param
        /// <T>
        ///     the type of the result entry </param>
        ///     <returns>
        ///         the typed value for the given name or null if no value exists for
        ///         this name
        ///     </returns>
        ///     <seealso cref= getEntry( String) </seealso>
        T GetEntryTyped<T>(string name);
    }
}