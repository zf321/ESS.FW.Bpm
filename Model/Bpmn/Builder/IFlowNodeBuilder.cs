using ESS.FW.Bpm.Model.Bpmn.instance;
using System;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    public interface IFlowNodeBuilder<TE> : IBpmnModelElementBuilder<TE> where TE: IFlowNode
    {
        BusinessRuleTaskBuilder BusinessRuleTask();
        BusinessRuleTaskBuilder BusinessRuleTask(string id);
        CallActivityBuilder CallActivity();
        CallActivityBuilder CallActivity(string id);
        IFlowNodeBuilder<TE> CamundaAsyncAfter();
        IFlowNodeBuilder<TE> CamundaAsyncAfter(bool asyncAfter);
        IFlowNodeBuilder<TE> CamundaAsyncBefore();
        IFlowNodeBuilder<TE> CamundaAsyncBefore(bool asyncBefore);
        IFlowNodeBuilder<TE> CamundaExclusive(bool exclusive);
        IFlowNodeBuilder<TE> CamundaExecutionListenerClass(string eventName, string fullQualifiedClassName);
        IFlowNodeBuilder<TE> CamundaExecutionListenerDelegateExpression(string eventName, string delegateExpression);
        IFlowNodeBuilder<TE> CamundaExecutionListenerExpression(string eventName, string expression);
        IFlowNodeBuilder<TE> CamundaFailedJobRetryTimeCycle(string retryTimeCycle);
        IFlowNodeBuilder<TE> CamundaJobPriority(string jobPriority);
        IFlowNodeBuilder<TE> Condition(string name, string condition);
        EndEventBuilder EndEvent();
        EndEventBuilder EndEvent(string id);
        EventBasedGatewayBuilder EventBasedGateway();
        ExclusiveGatewayBuilder ExclusiveGateway();
        ExclusiveGatewayBuilder ExclusiveGateway(string id);
        IGateway FindLastGateway();
        InclusiveGatewayBuilder InclusiveGateway();
        InclusiveGatewayBuilder InclusiveGateway(string id);
        IntermediateCatchEventBuilder IntermediateCatchEvent();
        IntermediateCatchEventBuilder IntermediateCatchEvent(string id);
        IntermediateThrowEventBuilder IntermediateThrowEvent();
        IntermediateThrowEventBuilder IntermediateThrowEvent(string id);
        ManualTaskBuilder ManualTask();
        ManualTaskBuilder ManualTask(string id);
        IGatewayBuilder<TE> /*IFlowNodeBuilder<TE>*/ MoveToLastGateway();
        IFlowNodeBuilder<TE> MoveToNode(string identifier);
        IFlowNodeBuilder<TE> NotCamundaExclusive();
        ParallelGatewayBuilder ParallelGateway();
        ParallelGatewayBuilder ParallelGateway(string id);
        ReceiveTaskBuilder ReceiveTask();
        ReceiveTaskBuilder ReceiveTask(string id);
        ScriptTaskBuilder ScriptTask();
        ScriptTaskBuilder ScriptTask(string id);
        SendTaskBuilder SendTask();
        SendTaskBuilder SendTask(string id);
        IFlowNodeBuilder<TE> SequenceFlowId(string sequenceFlowId);
        ServiceTaskBuilder ServiceTask();
        ServiceTaskBuilder ServiceTask(string id);
        SubProcessBuilder SubProcess();
        SubProcessBuilder SubProcess(string id);
        TransactionBuilder Transaction();
        TransactionBuilder Transaction(string id);
        UserTaskBuilder UserTask();
        UserTaskBuilder UserTask(string id);
        IFlowNodeBuilder<TE> ConnectTo(string identifier);
        IActivityBuilder<IActivity> MoveToActivity(string identifier);
        IFlowNodeBuilder<TE> Func(Action<TE> action);
    }
}