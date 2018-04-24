

using ESS.FW.Bpm.Model.Bpmn.instance.di;

namespace ESS.FW.Bpm.Model.Bpmn.instance.bpmndi
{
    /// <summary>
	/// The BPMNDI BPMNLabel element
	/// 
	/// 
	/// </summary>
	public interface IBpmnLabel : ILabel
	{

	  IBpmnLabelStyle LabelStyle {get;set;}


	}

}