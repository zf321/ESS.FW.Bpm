using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance.di;


namespace ESS.FW.Bpm.Model.Bpmn.instance
{
    /// <summary>
	/// The BPMN baseElement element
	/// 
	/// 
	/// </summary>
	public interface IBaseElement : IBpmnModelElementInstance
	{

	  //string Id {get;set;}


	  ICollection<IDocumentation> Documentations {get;}

	  IExtensionElements ExtensionElements {get;set;}


	  IDiagramElement DiagramElement {get;}

	}

}