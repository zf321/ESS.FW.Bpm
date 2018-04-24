

namespace ESS.FW.Bpm.Model.Bpmn.instance.camunda
{

	/// <summary>
	/// The BPMN script camunda extension element
	/// 
	/// 
	/// </summary>
	public interface ICamundaScript : IBpmnModelElementInstance
	{

	  string CamundaScriptFormat {get;set;}


	  string CamundaResource {get;set;}


	}

}