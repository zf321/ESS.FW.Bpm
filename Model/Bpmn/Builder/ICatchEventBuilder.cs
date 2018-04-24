using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    public interface ICatchEventBuilder<TE> : IEventBuilder<TE>, IFlowNodeBuilder<TE> where TE: ICatchEvent
    {
        CompensateEventDefinitionBuilder CompensateEventDefinition();
        CompensateEventDefinitionBuilder CompensateEventDefinition(string id);
        ICatchEventBuilder<TE> Condition(string condition);
        ConditionalEventDefinitionBuilder ConditionalEventDefinition();
        ConditionalEventDefinitionBuilder ConditionalEventDefinition(string id);
        ICatchEventBuilder<TE> Message(string messageName);
        ICatchEventBuilder<TE> ParallelMultiple();
        ICatchEventBuilder<TE> Signal(string signalName);
        ICatchEventBuilder<TE> TimerWithCycle(string timerCycle);
        ICatchEventBuilder<TE> TimerWithDate(string timerDate);
        ICatchEventBuilder<TE> TimerWithDuration(string timerDuration);
    }
}