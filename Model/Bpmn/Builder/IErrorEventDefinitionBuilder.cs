using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    public interface IErrorEventDefinitionBuilder:IBaseElementBuilder<IErrorEventDefinition>
    {
        IErrorEventDefinitionBuilder Error(string errorCode);
        IErrorEventDefinitionBuilder ErrorCodeVariable(string errorCodeVariable);
        IErrorEventDefinitionBuilder ErrorMessageVariable(string errorMessageVariable);
        //IBaseElementBuilder<IErrorEventDefinition> Id(string identifier);
        IEventBuilder<IEvent> ErrorEventDefinitionDone();
    }
}