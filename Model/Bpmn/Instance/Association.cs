

using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;

namespace ESS.FW.Bpm.Model.Bpmn.instance
{
    /// <summary>
	/// The BPMN association element
	/// 
	/// 
	/// </summary>
	public interface IAssociation : IArtifact
	{

	  IBaseElement Source {get;set;}


	  IBaseElement Target {get;set;}


	  AssociationDirection AssociationDirection {get;set;}


	  //IBpmnEdge DiagramElement {get;}

	}

}