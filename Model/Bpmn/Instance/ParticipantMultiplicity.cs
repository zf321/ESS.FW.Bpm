

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN participantMultiplicity element
	/// 
	/// 
	/// </summary>
	public interface IParticipantMultiplicity : IBaseElement
	{

	  int Minimum {get;set;}


	  int Maximum {get;set;}


	}

}