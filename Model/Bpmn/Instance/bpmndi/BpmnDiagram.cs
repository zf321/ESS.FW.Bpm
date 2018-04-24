using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance.di;



namespace ESS.FW.Bpm.Model.Bpmn.instance.bpmndi
{
    /// <summary>
	/// The BPMNDI BPMNDiagram element
	/// 
	/// 
	/// </summary>
	public interface IBpmnDiagram : IDiagram
	{

	  IBpmnPlane BpmnPlane {get;set;}


	  ICollection<IBpmnLabelStyle> BpmnLabelStyles {get;}

	}

}