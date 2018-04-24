

namespace ESS.FW.Bpm.Model.Bpmn.instance.di
{

    /// <summary>
    /// The DI DiagramElement element
    /// 
    /// 
    /// </summary>
    public interface IDiagramElement : IBpmnModelElementInstance
    {
        new string Id { get; set; }
        IExtension Extension { get; set; }
    }
}