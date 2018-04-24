

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN compensateEventDefinition element
	/// 
	/// 
	/// </summary>
	public interface ICompensateEventDefinition : IEventDefinition
	{

	  bool WaitForCompletion {get;set;}


	  IActivity Activity {get;set;}


	}

}