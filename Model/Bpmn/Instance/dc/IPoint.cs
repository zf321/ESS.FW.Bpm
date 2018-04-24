

namespace ESS.FW.Bpm.Model.Bpmn.instance.dc
{
    /// <summary>
    ///     The DC point element
    ///     
    /// </summary>
    public interface IPoint : IBpmnModelElementInstance
    {
        double X { get; set; }
        double Y { get; set; }
    }
}