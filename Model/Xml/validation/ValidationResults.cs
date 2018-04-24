using System.Collections.Generic;
using System.IO;
using ESS.FW.Bpm.Model.Xml.instance;


namespace ESS.FW.Bpm.Model.Xml.validation
{
    /// <summary>
    /// Object in which the results of a model validation are collected.
    /// See: <seealso cref="IModelInstance#validate(java.util.Collection)"/>.
    /// 
    /// 
    /// 
    /// </summary>
    public interface IValidationResults
    {

        /// <returns> true if there are <seealso cref="IValidationResult"/> of type <seealso cref="ValidationResultType#ERROR"/> </returns>
        bool HasErrors();

        /// <returns> the count of <seealso cref="IValidationResult"/> of type <seealso cref="ValidationResultType#ERROR"/> </returns>
        int ErrorCount { get; }

        /// <returns> the count of <seealso cref="IValidationResult"/> of type <seealso cref="ValidationResultType#WARNING"/> </returns>
        int WarinigCount { get; }

        /// <returns> the individual results of the validation grouped by element. </returns>
        IDictionary<IModelElementInstance, IList<IValidationResult>> Results { get; }

        /// <summary>
        /// Utility method to print out a summary of the validation results.
        /// </summary>
        /// <param name="writer"> a <seealso cref="StringWriter"/> to which the result should be printed </param>
        /// <param name="printer"> formatter for printing elements and validation results </param>
        void Write(StringWriter writer, IValidationResultFormatter printer);

    }
}