

using ESS.FW.Bpm.Model.Bpmn.builder;

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	using BoundaryEventBuilder = BoundaryEventBuilder;

	/// <summary>
	/// The BPMN boundaryEvent element
	/// 
	/// 
	/// </summary>
	public interface IBoundaryEvent : ICatchEvent
	{
        
	  bool CancelActivity { get;set;}

	  IActivity AttachedTo {get;set;}


	  BoundaryEventBuilder Builder();

	}

}