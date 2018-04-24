

using ESS.FW.Bpm.Model.Bpmn.impl.instance;

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	using From = From;
	using To = To;

	/// <summary>
	/// The BPMN assignment element
	/// 
	/// 
	/// </summary>
	public interface IAssignment : IBaseElement
	{

	  From From {get;set;}


	  To To {get;set;}

	}

}