using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    public interface IMultiInstanceLoopCharacteristicsBuilder: IBpmnModelElementBuilder<IMultiInstanceLoopCharacteristics>
    {
        IMultiInstanceLoopCharacteristicsBuilder CamundaCollection(string expression);
        IMultiInstanceLoopCharacteristicsBuilder CamundaElementVariable(string variableName);
        IMultiInstanceLoopCharacteristicsBuilder Cardinality(string expression);
        IMultiInstanceLoopCharacteristicsBuilder CompletionCondition(string expression);
        IMultiInstanceLoopCharacteristicsBuilder Parallel();
        IMultiInstanceLoopCharacteristicsBuilder Sequential();
        IActivityBuilder<IActivity> MultiInstanceDone();
    }
}