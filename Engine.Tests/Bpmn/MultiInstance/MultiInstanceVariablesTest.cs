using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.builder;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;
using NUnit.Framework;
using ITask = ESS.FW.Bpm.Engine.Task.ITask;

namespace Engine.Tests.Bpmn.MultiInstance
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MultiInstanceVariablesTest
    {
        public const string All = "all";
        public const string SUB_PROCESS_ID = "testProcess";
        public const string PROCESS_ID = "process";
        public const string CALL_ACTIVITY = "callActivity";

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public ProcessEngineRule engineRule = new util.ProvidedProcessEngineRule();
        public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

        protected internal virtual void addAllOut(IBpmnModelInstance modelInstance,
            CallActivityBuilder callActivityBuilder)
        {
            var camundaOut = modelInstance.NewInstance<ICamundaOut>(typeof(ICamundaOut));
            camundaOut.CamundaVariables = Permissions.All.ToString();
            callActivityBuilder.AddExtensionElement(camundaOut);
        }

        protected internal virtual void addAllIn(IBpmnModelInstance modelInstance,
            CallActivityBuilder callActivityBuilder)
        {
            var camundaIn = modelInstance.NewInstance<ICamundaOut>(typeof(ICamundaOut));
            camundaIn.CamundaVariables = Permissions.All.ToString();
            callActivityBuilder.AddExtensionElement(camundaIn);
        }

        protected internal virtual void deployAndStartProcess(IBpmnModelInstance modelInstance,
            IBpmnModelInstance testProcess)
        {
            engineRule.ManageDeployment(engineRule.RepositoryService.CreateDeployment()
                .AddModelInstance("process.bpmn", modelInstance)
                .Deploy());
            engineRule.ManageDeployment(engineRule.RepositoryService.CreateDeployment()
                .AddModelInstance("testProcess.bpmn", testProcess)
                .Deploy());
            engineRule.RuntimeService.StartProcessInstanceByKey(PROCESS_ID);
        }

        protected internal virtual IBpmnModelInstance BpmnModelInstance
        {
            get
            {
                var start = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_ID)
                    .StartEvent();
              var activity=  start.CallActivity(CALL_ACTIVITY)
                .CalledElement(SUB_PROCESS_ID);
                activity.MultiInstance()
                    .Cardinality("2");
                  return  activity.Done(); //MultiInstanceDone().EndEvent().Done();
            }
        }

        protected internal virtual IBpmnModelInstance BpmnSubProcessModelInstance
        {
            get
            {
                return ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(SUB_PROCESS_ID)
                    .StartEvent()
                    .UserTask("userTask")
                    .EndEvent()
                    .Done();
            }
        }

        [Test]
        public virtual void testMultiInstanceWithAllInOutMapping()
        {
            var modelInstance = BpmnModelInstance;

            var callActivityBuilder = (modelInstance.GetModelElementById/*<ICallActivity>*/(CALL_ACTIVITY) as ICallActivity)
                .Builder();

            addAllIn(modelInstance, callActivityBuilder);

            addAllOut(modelInstance, callActivityBuilder);

            var testProcess = BpmnSubProcessModelInstance;

            deployAndStartProcess(modelInstance, testProcess);
            Assert.That(engineRule.RuntimeService.CreateExecutionQuery(c => c.ProcessBusinessKey == SUB_PROCESS_ID).Count(), Is.EqualTo(2));

            IList<ITask> tasks =
                engineRule.TaskService.CreateTaskQuery(c => c.SuspensionState == SuspensionStateFields.Active.StateCode)
                    .ToList();
            foreach (var task in tasks)
            {
                engineRule.TaskService.SetVariable(task.Id, MultiInstanceActivityBehavior.NumberOfInstances, "3");
                engineRule.TaskService.Complete(task.Id);
            }

            Assert.That(engineRule.RuntimeService.CreateExecutionQuery(c=>c.ProcessBusinessKey == SUB_PROCESS_ID).Count(), Is.EqualTo(0));
            Assert.That(engineRule.RuntimeService.CreateExecutionQuery(c => c.ActivityId == CALL_ACTIVITY)
                .Count(), Is.EqualTo(0));
        }
    }
}