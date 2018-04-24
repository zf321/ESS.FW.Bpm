//using System;
//using ESS.FW.Bpm.Engine.Impl.Core.Instance;

//namespace ESS.FW.Bpm.Engine.context.Impl
//{

//    /// <summary>
//    ///     
//    ///     
//    /// </summary>
//    public class CaseExecutionContext : CoreExecutionContext<CoreExecution>
//    {
//        public CaseExecutionContext(CoreExecution execution) : base(execution)
//        {
//        }



//        public virtual CaseExecutionEntity CaseInstance

//        {
//            get { return execution.GetCaseInstance(); }
//        }

//        public virtual CaseDefinitionEntity CaseDefinition
//        {
//            get { return (CaseDefinitionEntity)execution.CaseDefinition; }
//        }

//        protected internal override string DeploymentId
//        {
//            get { throw new NotImplementedException(); /*return CaseDefinition.DeploymentId;*/ }
//        }
//    }
//}