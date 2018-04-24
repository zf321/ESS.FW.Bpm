

using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public class ParallelGatewayBuilder : AbstractParallelGatewayBuilder/*<ParallelGatewayBuilder>*/
	{

	  public ParallelGatewayBuilder(IBpmnModelInstance modelInstance, IParallelGateway element) : base(modelInstance, element)
	  {
	  }

	}

}