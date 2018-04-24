

using ESS.FW.Bpm.Model.Bpmn.instance.di;

namespace ESS.FW.Bpm.Model.Bpmn.instance.bpmndi
{
    /// <summary>
	/// The BPMNDI BPMNPlane element
	/// 
	/// 
	/// </summary>
	public interface IBpmnPlane : IPlane
	{

	  IBaseElement BpmnElement {get;set;}


	}

}