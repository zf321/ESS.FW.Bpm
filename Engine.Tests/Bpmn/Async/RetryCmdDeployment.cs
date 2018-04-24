//using System.Collections.Generic;
//using ESS.FW.Bpm.Model.Bpmn;

//namespace ESS.FW.Bpm.Engine.Tests.Bpmn.Async
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public class RetryCmdDeployment
//    {

//        public const string FAILING_EVENT = "failingEvent";
//        public const string PROCESS_ID = "failedIntermediateThrowingEventAsync";
//        private const string SCHEDULE = "R5/PT5M";
//        private const string PROCESS_ID_2 = "failingSignalProcess";
//        public const string MESSAGE = "start";
//        private IBpmnModelInstance[] bpmnModelInstances;

//        public static RetryCmdDeployment Deployment()
//        {
//            return new RetryCmdDeployment();
//        }

//        public static IBpmnModelInstance prepareSignalEventProcess()
//        {
//            //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_ID)
//                    .StartEvent()
//                    .IntermediateThrowEvent(FAILING_EVENT)
//                    .CamundaAsyncBefore(true)
//                    .CamundaFailedJobRetryTimeCycle(SCHEDULE)
//                    .Signal(MESSAGE)
//                    .ServiceTask()
//                    .CamundaClass(typeof(FailingDelegate).FullName)
//                    .EndEvent()
//                    .Done();
//            return modelInstance;
//        }

//        public static IBpmnModelInstance prepareMessageEventProcess()
//        {
//            //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
//            return Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_ID)
//                    .StartEvent()
//                    .IntermediateThrowEvent(FAILING_EVENT)
//                    .CamundaAsyncBefore(true)
//                    .CamundaFailedJobRetryTimeCycle(SCHEDULE)
//                    .Message(MESSAGE)
//                    .ServiceTask()
//                    .CamundaClass(typeof(FailingDelegate).FullName)
//                    .Done();
//        }

//        public static IBpmnModelInstance prepareEscalationEventProcess()
//        {
//            //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
//            return Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_ID)
//                    .StartEvent()
//                    .IntermediateThrowEvent(FAILING_EVENT)
//                    .CamundaAsyncBefore(true)
//                    .CamundaFailedJobRetryTimeCycle(SCHEDULE)
//                    .Escalation(MESSAGE)
//                    .ServiceTask()
//                    .CamundaClass(typeof(FailingDelegate).FullName)
//                    .EndEvent()
//                    .Done();
//        }


//        //public static IBpmnModelInstance prepareCompensationEventProcess()
//        //{
//        //    //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
//        //    return Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_ID)
//        //            .StartEvent()
//        //            .SubProcess("subProcess")
//        //            .EmbeddedSubProcess()
//        //            .StartEvent()
//        //            .EndEvent()
//        //            .SubProcessDone()
//        //            .IntermediateThrowEvent(FAILING_EVENT)
//        //            .CamundaAsyncBefore(true)
//        //            .CamundaFailedJobRetryTimeCycle(SCHEDULE)
//        //            .compensateEventDefinition()
//        //            .compensateEventDefinitionDone()
//        //            .ServiceTask()
//        //            .CamundaClass(typeof(FailingDelegate).FullName)
//        //            .EndEvent()
//        //            .Done();
//        //}


//        public virtual RetryCmdDeployment withEventProcess(params IBpmnModelInstance[] bpmnModelInstances)
//        {
//            this.bpmnModelInstances = bpmnModelInstances;
//            return this;
//        }

//        public static ICollection<RetryCmdDeployment[]> asParameters(params RetryCmdDeployment[] deployments)
//        {
//            IList<RetryCmdDeployment[]> deploymentList = new List<RetryCmdDeployment[]>();
//            foreach (RetryCmdDeployment deployment in deployments)
//            {
//                deploymentList.Add(new RetryCmdDeployment[] { deployment });
//            }

//            return deploymentList;
//        }

//        public virtual IBpmnModelInstance[] BpmnModelInstances
//        {
//            get
//            {
//                return bpmnModelInstances;
//            }
//            set
//            {
//                this.bpmnModelInstances = value;
//            }
//        }

//    }

//}