
namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN Import element
	/// 
	/// 
	/// </summary>
	public interface IMport : IBpmnModelElementInstance
	{

	  string Namespace {get;set;}


	  string Location {get;set;}


	  string ImportType {get;set;}


	}

}