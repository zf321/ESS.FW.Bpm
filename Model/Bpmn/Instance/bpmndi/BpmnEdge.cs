

using ESS.FW.Bpm.Model.Bpmn.instance.di;

namespace ESS.FW.Bpm.Model.Bpmn.instance.bpmndi
{
    /// <summary>
    /// The BPMNDI BPMNEdge element
    /// 
    /// 
    /// </summary>
    public interface IBpmnEdge : ILabeledEdge
    {

        IBaseElement BpmnElement { get; set; }


        IDiagramElement SourceElement { get; set; }


        IDiagramElement TargetElement { get; set; }


        MessageVisibleKind MessageVisibleKind { get; set; }


        IBpmnLabel BpmnLabel { get; set; }
    }
}