using System;
using Engine.Tests.Bpmn.Common;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.instance;

namespace Engine.Tests.Bpmn.ServiceTask
{
    
    public class JavaDelegateProcessEngineServicesAccessTest : AbstractProcessEngineServicesAccessTest
    {
        
        protected internal override Type TestServiceAccessibleClass
        {
            get { return typeof(AccessServicesJavaDelegate); }
        }

        protected internal override Type QueryClass
        {
            get { return typeof(PerformQueryJavaDelegate); }
        }

        protected internal override Type StartProcessInstanceClass
        {
            get { return typeof(StartProcessJavaDelegate); }
        }

        protected internal override ITask CreateModelAccessTask(IBpmnModelInstance modelInstance, Type delegateClass)
        {
            var serviceTask = modelInstance.NewInstance<IServiceTask>(typeof(IServiceTask));
            serviceTask.Id = "serviceTask";
            serviceTask.CamundaClass = delegateClass.FullName;
            return serviceTask;
        }

        public class AccessServicesJavaDelegate : IJavaDelegate
        {
            public void Execute(IBaseDelegateExecution execution)
            {
               //AssertCanAccessServices(execution.ProcessEngineServices);
            }
        }

        public class PerformQueryJavaDelegate : IJavaDelegate
        {
            public void Execute(IBaseDelegateExecution execution)
            {
                //TODO
              //AssertCanPerformQuery(execution.ProcessEngineServices);
            }
        }

        public class StartProcessJavaDelegate : IJavaDelegate
        {
             public void Execute(IBaseDelegateExecution execution)
            {
                //TODO
                //AssertCanPerformQuery(execution.BusinessKey);
            }
        }

    }

}