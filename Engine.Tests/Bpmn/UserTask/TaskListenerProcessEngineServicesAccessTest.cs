using System;
using Engine.Tests.Bpmn.Common;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;

namespace Engine.Tests.Bpmn.UserTask
{

    /// <summary>
    /// 
    /// 
    /// </summary>
    public class TaskListenerProcessEngineServicesAccessTest : AbstractProcessEngineServicesAccessTest
    {

        protected internal override Type TestServiceAccessibleClass
        {
            get { return typeof(AccessServicesListener); }
        }

        protected internal override Type QueryClass
        {
            get { return typeof(PerformQueryListener); }
        }

        protected internal override Type StartProcessInstanceClass
        {
            get { return typeof(StartProcessListener); }
        }


        protected internal override ESS.FW.Bpm.Model.Bpmn.instance.ITask CreateModelAccessTask(IBpmnModelInstance modelInstance,
            Type delegateClass)
        {
            var task = modelInstance.NewInstance<IUserTask>(typeof(IUserTask));
            task.Id = "userTask";
            var executionListener = modelInstance.NewInstance<ICamundaTaskListener>(typeof(ICamundaTaskListener));
            executionListener.CamundaEvent += TaskListenerFields.EventnameCreate;

            executionListener.CamundaClass = delegateClass.FullName;
            task.Builder().AddExtensionElement(executionListener);
            return task;
        }


        public class AccessServicesListener : ITaskListener
        {
            public virtual void Notify(IDelegateTask execution)
            {
                AssertCanAccessServices(execution.ProcessEngineServices);
            }
        }

        public class PerformQueryListener : ITaskListener
        {
            public virtual void Notify(IDelegateTask execution)
            {
                AssertCanPerformQuery(execution.ProcessEngineServices);
            }
        }

        public class StartProcessListener : ITaskListener
        {
            public virtual void Notify(IDelegateTask execution)
            {
                AssertCanPerformQuery(execution.ProcessEngineServices);
            }
        }

    }

}