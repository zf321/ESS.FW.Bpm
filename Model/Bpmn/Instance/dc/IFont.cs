

namespace ESS.FW.Bpm.Model.Bpmn.instance.dc
{

    /// <summary>
    /// The DC font element
    /// 
    /// 
    /// </summary>
    public interface IFont : IBpmnModelElementInstance
    {

        string Name { get; set; }


        double? Size { get; set; }

        bool? Bold { get; set; }

        bool? Italic { get; set; }

        bool? Underline { get; set; }


        bool? StrikeThrough { get; set; }

    }

}