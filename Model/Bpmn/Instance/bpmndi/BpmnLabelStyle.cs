

using ESS.FW.Bpm.Model.Bpmn.instance.dc;
using ESS.FW.Bpm.Model.Bpmn.instance.di;

namespace ESS.FW.Bpm.Model.Bpmn.instance.bpmndi
{
    /// <summary>
	/// The BPMNDI BPMNLabelStyle element
	/// 
	/// 
	/// </summary>
	public interface IBpmnLabelStyle : IStyle
	{

	  IFont Font {get;set;}


	}

}