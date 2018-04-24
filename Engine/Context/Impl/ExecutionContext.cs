

//using ESS.FW.Bpm.Engine.Persistence.Entity;

//namespace ESS.FW.Bpm.Engine.context.Impl
//{
//    /// <summary>
//    ///     An <seealso cref="ExecutionEntity" /> execution context. Provides access to the process instance and the
//    ///     deployment.
//    /// </summary>
//    /// @deprecated since 7.2: use
//    /// <seealso cref="BpmnExecutionContext" />
//    ///  
//    /// 
//    /// 
//    //[Obsolete("since 7.2: use <seealso cref=\"BpmnExecutionContext\"/>")]
//    public class ExecutionContext : CoreExecutionContext<ExecutionEntity>
//    {
//        public ExecutionContext(ExecutionEntity execution) : base(execution)
//        {
//        }

//        public virtual ExecutionEntity ProcessInstance
//        {
//            get { return execution.GetProcessInstance(); }
//        }

//        public virtual ProcessDefinitionEntity ProcessDefinition
//        {
//            get { return (ProcessDefinitionEntity)execution.GetProcessDefinition(); }
//        }

//        protected internal override string DeploymentId
//        {
//            get { return ProcessDefinition.DeploymentId; }
//        }
//    }
//}