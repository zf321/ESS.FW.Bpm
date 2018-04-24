using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    public interface ICallActivityBuilder:IActivityBuilder<ICallActivity>,IFlowElementBuilder<ICallActivity>,IFlowNodeBuilder<ICallActivity>
    {
        ICallActivityBuilder CalledElement(string calledElement);
        ICallActivityBuilder CamundaAsync();
        ICallActivityBuilder CamundaAsync(bool isCamundaAsync);
        ICallActivityBuilder CamundaCalledElementBinding(string camundaCalledElementBinding);
        ICallActivityBuilder CamundaCalledElementTenantId(string camundaCalledElementTenantId);
        ICallActivityBuilder CamundaCalledElementVersion(string camundaCalledElementVersion);
        ICallActivityBuilder CamundaCaseBinding(string camundaCaseBinding);
        ICallActivityBuilder CamundaCaseRef(string caseRef);
        ICallActivityBuilder CamundaCaseTenantId(string tenantId);
        ICallActivityBuilder CamundaCaseVersion(string camundaCaseVersion);
        ICallActivityBuilder CamundaIn(string source, string target);
        ICallActivityBuilder CamundaOut(string source, string target);
        ICallActivityBuilder CamundaVariableMappingClass(string camundaVariableMappingClass);
        ICallActivityBuilder CamundaVariableMappingDelegateExpression(string camundaVariableMappingDelegateExpression);
    }
}