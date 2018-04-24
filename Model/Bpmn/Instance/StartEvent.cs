using System;
using ESS.FW.Bpm.Model.Bpmn.builder;


namespace ESS.FW.Bpm.Model.Bpmn.instance
{
	/// <summary>
	/// The BPMN startEvent element
	/// 
	/// 
	/// 
	/// </summary>
	public interface IStartEvent : ICatchEvent
	{

	  //StartEventBuilder Builder();

	  bool Interrupting {get;set;}


	  /// <summary>
	  /// camunda extensions </summary>

	  /// @deprecated use isCamundaAsyncBefore() instead. 
	  [Obsolete("use isCamundaAsyncBefore() instead.")]
	  bool CamundaAsync {get;set;}


	  string CamundaFormHandlerClass {get;set;}


	  string CamundaFormKey {get;set;}


	  string CamundaInitiator {get;set;}

	}

}