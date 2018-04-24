

using System.Collections.Generic;

namespace ESS.FW.Bpm.Model.Bpmn.instance.camunda
{


    /// <summary>
    /// The BPMN camundaList extension element
    /// 
    /// 
    /// </summary>
    public interface ICamundaList : IBpmnModelElementInstance
    {

        //JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
        //ICollection<T> GetValues<T>() where T : IBpmnModelElementInstance;

        ICollection<IBpmnModelElementInstance> GetValues(); //where T : IBpmnModelElementInstance;k
        
    }

}