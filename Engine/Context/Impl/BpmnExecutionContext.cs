using ESS.FW.Bpm.Engine.Impl.Core.Instance;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.context.Impl
{
    /// <summary>
    ///     
    ///     
    /// </summary>
    public class BpmnExecutionContext : CoreExecutionContext/*<ExecutionEntity>*/
    {
        public BpmnExecutionContext(ExecutionEntity execution) : base(execution)
        {
        }
        public virtual ExecutionEntity ProcessInstance => ((ExecutionEntity)execution).GetProcessInstance();

        public virtual ProcessDefinitionEntity ProcessDefinition => ((ExecutionEntity)execution).GetProcessDefinition();

        protected internal override string DeploymentId => ProcessDefinition.DeploymentId;

        //public new ExecutionEntity Execution
        //{
        //    get
        //    {
        //        return execution as ExecutionEntity;
        //    }
        //}
    }
}