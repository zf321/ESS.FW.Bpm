using System;
using ESS.FW.Bpm.Model.Bpmn.builder;



namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	using ParallelGatewayBuilder = ParallelGatewayBuilder;

	/// <summary>
	/// The BPMN parallelGateway element
	/// 
	/// 
	/// </summary>
	public interface IParallelGateway : IGateway
	{

	  /// <summary>
	  /// camunda extensions </summary>

	  /// @deprecated use isCamundaAsyncBefore() instead. 
	  [Obsolete("use isCamundaAsyncBefore() instead.")]
	  bool CamundaAsync {get;set;}


	  ParallelGatewayBuilder Builder();

	}

}