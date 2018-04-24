

using ESS.FW.Bpm.Model.Xml.instance;

namespace ESS.FW.Bpm.Model.Xml.validation
{
    /// <summary>
	/// An individual validation result.
	/// 
	/// 
	/// 
	/// </summary>
	public interface IValidationResult
	{

	  /// <returns> The type of the result. </returns>
	  ValidationResultType Type {get;}

	  /// <returns> the element </returns>
	  IModelElementInstance XmlElement {get;}

	  /// <returns> A human consumable detail message about the validation </returns>
	  string Message {get;}

	  /// <returns> A reference code for this validation result </returns>
	  int Code {get;}

	}

}