

namespace ESS.FW.Bpm.Model.Bpmn.instance.di
{

    /// <summary>
    /// The DI Diagram element
    /// 
    /// 
    /// </summary>
    public interface IDiagram : IBpmnModelElementInstance
    {

        string Name { get; set; }


        string Documentation { get; set; }


        double Resolution { get; set; }


        string Id { get; set; }
    }

}