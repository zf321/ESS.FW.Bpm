
namespace ESS.FW.Bpm.Model.Xml.validation
{

	/// <summary>
	/// Object passed to the <seealso cref="IModelElementValidator{T}"/> to collect validation results.
	/// 
	/// 
	/// 
	/// </summary>
	public interface IValidationResultCollector
	{

	  /// <summary>
	  /// Adds an error
	  /// </summary>
	  /// <param name="code"> a reference code for the error </param>
	  /// <param name="message"> a human consumable error message </param>
	  void AddError(int code, string message);

	  /// <summary>
	  /// Adds a warining
	  /// </summary>
	  /// <param name="code"> a reference code for the error </param>
	  /// <param name="message"> a human consumable error message </param>
	  void AddWarning(int code, string message);

	}

}