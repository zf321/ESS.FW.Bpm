

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN timerEventDefinition element
	/// 
	/// 
	/// </summary>
	public interface ITimerEventDefinition : IEventDefinition
	{

	  ITimeDate TimeDate {get;set;}


	  ITimeDuration TimeDuration {get;set;}


	  ITimeCycle TimeCycle {get;set;}


	}

}