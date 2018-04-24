

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN error element
	/// 
	/// 
	/// </summary>
	public interface IError : IRootElement
	{

	  string Name {get;set;}


	  string ErrorCode {get;set;}


	  IItemDefinition Structure {get;set;}


	}

}