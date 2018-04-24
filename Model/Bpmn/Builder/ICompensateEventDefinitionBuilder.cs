using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    public interface ICompensateEventDefinitionBuilder: IBaseElementBuilder<ICompensateEventDefinition>
    {
        ICompensateEventDefinitionBuilder ActivityRef(string activityId);
        //IBaseElementBuilder<ICompensateEventDefinition> Id(string identifier);
        ICompensateEventDefinitionBuilder WaitForCompletion(bool waitForCompletion);
        IEventBuilder<IEvent> CompensateEventDefinitionDone();
    }
}