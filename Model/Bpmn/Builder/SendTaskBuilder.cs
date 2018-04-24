

using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public class SendTaskBuilder : AbstractSendTaskBuilder/*<SendTaskBuilder>*/
	{

	  public SendTaskBuilder(IBpmnModelInstance modelInstance, ISendTask element) : base(modelInstance, element)
	  {
	  }
	}

}