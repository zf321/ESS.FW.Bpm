using System;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping.value;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Core.Model
{
    /// <summary>
    ///     Default implementation for <seealso cref="BaseCallableElement#getTenantIdProvider()" />.
    ///     Uses the tenant id of the calling definition.
    /// </summary>
    public class DefaultCallableElementTenantIdProvider : IParameterValueProvider
    {
        public virtual object GetValue(IVariableScope execution)
        {
            if (execution is ExecutionEntity)
                return GetProcessDefinitionTenantId(execution as ExecutionEntity);
            //if (execution is CaseExecutionEntity)
            //    return GetCaseDefinitionTenantId((CaseExecutionEntity)execution);
            throw new ProcessEngineException("Unexpected execution of type " + execution.GetType().FullName);
        }

        protected internal virtual string GetProcessDefinitionTenantId(ExecutionEntity execution)
        {
            var processDefinition =  execution.GetProcessDefinition();
            return processDefinition.TenantId;
        }

        //protected internal virtual string GetCaseDefinitionTenantId(CaseExecutionEntity caseExecution)
        //{
        //    //var caseDefinition = (CaseDefinitionEntity) caseExecution.CaseDefinition;
        //    //return caseDefinition.TenantId;
        //    return string.Empty;
        //}
    }
}