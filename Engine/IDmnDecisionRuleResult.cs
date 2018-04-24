//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using ESS.FW.Bpm.Engine.Variable.Type;
//using ESS.FW.Bpm.Engine.Variable.Value;

//namespace ESS.FW.Bpm.Engine
//{
//    public interface IDmnDecisionRuleResult: IDictionary<string, object>

//    {
//        /// <returns> the value for the given name or null if no value exists for
//        ///         this name
//        /// </returns>
//        /// <seealso cref= #getEntryTyped(String) </seealso>
//        T GetEntry<T>(string name);

//        /// <summary>
//        /// Returns the typed value of the rule result entry for a given output name.
//        /// </summary>
//        /// <param name="name">
//        ///          the name of the output </param>
//        /// @param <T>
//        ///          the type of the rule result entry </param>
//        /// <returns> the typed value for the given name or null if no value exists for
//        ///         this name
//        /// </returns>
//        /// <seealso cref= #getEntry(String) </seealso>
//        T GetEntryTyped<T>(string name) ;

//        /// <summary>
//        /// Returns a map of the rule result entry values by output name.
//        /// </summary>
//        /// <returns> the values of the decision rule result entries
//        /// </returns>
//        /// <seealso cref= #getEntryMapTyped() </seealso>
//        IDictionary<string, object> GetEntryMap();

//        /// <summary>
//        /// Returns a map of the typed rule result entry values by output name.
//        /// </summary>
//        /// <returns> the typed values of the decision rule result entries
//        /// </returns>
//        /// <seealso cref= #getEntryMap() </seealso>
//        IDictionary<string, ITypedValue> GetEntryMapTyped();

//    }
//}
