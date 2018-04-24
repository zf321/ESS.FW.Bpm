

namespace ESS.FW.Bpm.Model.Bpmn.instance.camunda
{


	/// <summary>
	/// The BPMN potentialStarter camunda extension
	/// 
	/// 
	/// </summary>
	public interface ICamundaPotentialStarter : IBpmnModelElementInstance
	{

	  IResourceAssignmentExpression ResourceAssignmentExpression {get;set;}


	}

}