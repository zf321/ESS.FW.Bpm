

using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public class ExclusiveGatewayBuilder : AbstractExclusiveGatewayBuilder/*<ExclusiveGatewayBuilder>*/
	{

	  public ExclusiveGatewayBuilder(IBpmnModelInstance modelInstance, IExclusiveGateway element) : base(modelInstance, element)
	  {
	  }

	}

}