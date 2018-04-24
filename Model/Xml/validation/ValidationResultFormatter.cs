

using System.IO;
using ESS.FW.Bpm.Model.Xml.instance;

namespace ESS.FW.Bpm.Model.Xml.validation
{
    /// <summary>
    /// SPI which can be implemented to print out a summary of a validation result.
    /// See <seealso cref="IValidationResults#write(StringWriter, ValidationResultFormatter)"/>
    /// 
    /// 
    /// 
    /// </summary>
    public interface IValidationResultFormatter
	{

	  /// <summary>
	  /// formats an element in the summary
	  /// </summary>
	  /// <param name="writer"> the writer </param>
	  /// <param name="element"> the element to write </param>
	  void FormatElement(StringWriter writer, IModelElementInstance element);

	  /// <summary>
	  /// formats a validation result
	  /// </summary>
	  /// <param name="writer"> the writer </param>
	  /// <param name="result"> the result to format </param>
	  void FormatResult(StringWriter writer, IValidationResult result);

	}

}