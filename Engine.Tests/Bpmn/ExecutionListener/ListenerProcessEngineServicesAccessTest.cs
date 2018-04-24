using System;
using Engine.Tests.Bpmn.Common;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.ExecutionListener
{
    [TestFixture]
    public class ListenerProcessEngineServicesAccessTest : AbstractProcessEngineServicesAccessTest
    {

        protected internal override Type TestServiceAccessibleClass
        {
            get
            {
                return typeof(AccessServicesListener);
            }
        }

        protected internal override Type QueryClass
        {
            get
            {
                return typeof(PerformQueryListener);
            }
        }

        protected internal override Type StartProcessInstanceClass
        {
            get
            {
                return typeof(StartProcessListener);
            }
        }

        protected internal override ITask CreateModelAccessTask(IBpmnModelInstance modelInstance, Type delegateClass)
        {
            IManualTask task = modelInstance.NewInstance<IManualTask>(typeof(IManualTask));
            task.Id = "manualTask";
            ICamundaExecutionListener executionListener = modelInstance.NewInstance<ICamundaExecutionListener>(typeof(ICamundaExecutionListener));
            executionListener.CamundaEvent = ExecutionListenerFields.EventNameStart;
            //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
            executionListener.CamundaClass = delegateClass.FullName;
            task.Builder().AddExtensionElement(executionListener);
            return task;
        }

        public class AccessServicesListener : IDelegateListener<IBaseDelegateExecution>
        {
            public void Notify(IBaseDelegateExecution execution)
            {
                AssertCanAccessServices((execution as IDelegateExecution).ProcessEngineServices);
            }

            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: public void notify(org.Camunda.bpm.engine.delegate.IDelegateExecution execution) throws Exception
            //public virtual void notify(IDelegateExecution execution)
            //{
            //    assertCanAccessServices(execution.ProcessEngineServices);
            //}
        }

        public class PerformQueryListener : IDelegateListener<IBaseDelegateExecution>
        {
            public void Notify(IBaseDelegateExecution execution)
            {
                AssertCanPerformQuery((execution as IDelegateExecution).ProcessEngineServices);
            }

            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: public void notify(org.Camunda.bpm.engine.delegate.IDelegateExecution execution) throws Exception
            //public virtual void notify(IDelegateExecution execution)
            //{
            //    assertCanPerformQuery(execution.ProcessEngineServices);
            //}
        }

        public class StartProcessListener : IDelegateListener<IBaseDelegateExecution>
        {
            public void Notify(IBaseDelegateExecution execution)
            {
                AssertCanStartProcessInstance((execution as IDelegateExecution).ProcessEngineServices);
            }

            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: public void notify(org.Camunda.bpm.engine.delegate.IDelegateExecution execution) throws Exception
            //public virtual void notify(IDelegateExecution execution)
            //{
            //    assertCanStartProcessInstance(execution.ProcessEngineServices);
            //}
        }

    }

}