

namespace ESS.FW.Bpm.Model.Bpmn.instance.camunda
{

    /// <summary>
    /// A helper interface for camunda extension elements which
    /// hold a generic child element like camunda:inputParameter,
    /// camunda:outputParameter and camunda:entry.
    /// 
    /// 
    /// </summary>
    public interface ICamundaGenericValueElement
    {

        //JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
        T GetValue<T>() where T : IBpmnModelElementInstance;

        void RemoveValue();

        //JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
        void SetValue<T>(T value) where T : IBpmnModelElementInstance;
    }
}