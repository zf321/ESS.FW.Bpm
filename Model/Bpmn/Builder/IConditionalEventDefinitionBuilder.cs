using ESS.FW.Bpm.Model.Bpmn.instance;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    public interface IConditionalEventDefinitionBuilder
    {
        IConditionalEventDefinitionBuilder CamundaVariableEvents(IList<string> variableEvents);
        IConditionalEventDefinitionBuilder CamundaVariableEvents(string variableEvents);
        IConditionalEventDefinitionBuilder CamundaVariableName(string variableName);
        IConditionalEventDefinitionBuilder Condition(string conditionText);
        IEventBuilder<IEvent> ConditionalEventDefinitionDone();
    }
}