

using ESS.FW.Bpm.Model.Bpmn.instance.di;

namespace ESS.FW.Bpm.Model.Bpmn.instance.bpmndi
{
    /// <summary>
	/// The BPMNDI BPMNShape element
	/// 
	/// 
	/// </summary>
	public interface IBpmnShape : ILabeledShape
	{

	  IBaseElement BpmnElement {get;set;}


	  bool Horizontal {get;set;}


	  bool Expanded {get;set;}


	  bool MarkerVisible {get;set;}


	  bool MessageVisible {get;set;}


	  ParticipantBandKind ParticipantBandKind {get;set;}


	  IBpmnShape ChoreographyActivityShape {get;set;}


	  IBpmnLabel BpmnLabel {get;set;}


	}

}