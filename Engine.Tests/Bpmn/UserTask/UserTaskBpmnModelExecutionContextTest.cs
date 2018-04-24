using System.Linq;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Model.Bpmn.impl;
using ESS.FW.Bpm.Model.Bpmn.instance;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.UserTask
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class UserTaskBpmnModelExecutionContextTest : PluggableProcessEngineTestCase
    {
        [TearDown]
        public virtual void tearDown()
        {
            ModelExecutionContextTaskListener.Clear();
            repositoryService.DeleteDeployment(deploymentId, true);
        }

        private const string PROCESS_ID = "process";
        private const string USER_TASK_ID = "userTask";
        private string deploymentId;

        private void deployProcess(string eventName)
        {
            var modelInstance =
                ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_ID)
                    .StartEvent()
                    .UserTask(USER_TASK_ID)
                    .EndEvent()
                    .Done();

            var extensionElements = modelInstance.NewInstance<IExtensionElements>(typeof(IExtensionElements));
            var taskListener =
                extensionElements.AddExtensionElement(BpmnModelConstants.CamundaNs, "taskListener"); //CAMUNDA_NS

            //	import static org.Camunda.bpm.Model.bpmn.impl.BpmnModelConstants.CAMUNDA_NS;

            taskListener.SetAttributeValueNs(BpmnModelConstants.CamundaNs, "class",
                typeof(ModelExecutionContextTaskListener).FullName);
            //CAMUNDA_NS
            taskListener.SetAttributeValueNs(BpmnModelConstants.CamundaNs, "event", eventName); //

            var userTask = modelInstance.GetModelElementById/*<IUserTask>*/(USER_TASK_ID) as IUserTask;
            userTask.ExtensionElements = extensionElements;

            deploymentId = repositoryService.CreateDeployment()
                .AddModelInstance("process.bpmn", modelInstance)
                .Deploy()
                .Id;
        }

        private void AssertModelInstance()
        {
            var modelInstance = ModelExecutionContextTaskListener.ModelInstance;
            Assert.NotNull(modelInstance);

            var events =
                modelInstance
                    .GetModelElementsByType<IEvent>(typeof(IEvent)); //modelInstance.Model.GetType(typeof(IEvent)));
            Assert.AreEqual(2, events.Count());

            var tasks = modelInstance
                .GetModelElementsByType<ITask>(typeof(ITask)); //modelInstance.Model.GetType(typeof(ITask)));
            Assert.AreEqual(1, tasks.Count());

            var process = (IProcess) modelInstance.Definitions.RootElements.GetEnumerator()
                .Current;
            Assert.AreEqual(PROCESS_ID, process.Id);
            Assert.True(process.Executable);
        }

        private void AssertUserTask(string eventName)
        {
            var userTask = ModelExecutionContextTaskListener.UserTask;
            Assert.NotNull(userTask);

            var taskListener =
                userTask.ExtensionElements.GetUniqueChildElementByNameNs(BpmnModelConstants.CamundaNs, "taskListener");
            Assert.AreEqual(eventName, taskListener.GetAttributeValueNs(BpmnModelConstants.CamundaNs, "event"));

            Assert.AreEqual(typeof(ModelExecutionContextTaskListener).FullName,
                taskListener.GetAttributeValueNs(BpmnModelConstants.CamundaNs, "class")); //CAMUNDA_NS

            var modelInstance = ModelExecutionContextTaskListener.ModelInstance;
            var tasks = modelInstance.GetModelElementsByType<ITask>(typeof(ITask));
            //modelInstance.Model.GetType(typeof(ITask)));

            Assert.True(tasks.Contains(userTask));
        }

        [Test]
        public virtual void testGetBpmnModelElementInstanceOnAssignment()
        {
            var eventName = TaskListenerFields.EventnameAssignment;
            deployProcess(eventName);

            runtimeService.StartProcessInstanceByKey(PROCESS_ID);

            Assert.IsNull(ModelExecutionContextTaskListener.ModelInstance);
            Assert.IsNull(ModelExecutionContextTaskListener.UserTask);

            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;
            taskService.SetAssignee(taskId, "demo");

            AssertModelInstance();
            AssertUserTask(eventName);

            taskService.Complete(taskId);
        }

        [Test]
        public virtual void testGetBpmnModelElementInstanceOnComplete()
        {
            var eventName = TaskListenerFields.EventnameComplete;
            deployProcess(eventName);

            runtimeService.StartProcessInstanceByKey(PROCESS_ID);

            Assert.IsNull(ModelExecutionContextTaskListener.ModelInstance);
            Assert.IsNull(ModelExecutionContextTaskListener.UserTask);

            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;
            taskService.SetAssignee(taskId, "demo");

            Assert.IsNull(ModelExecutionContextTaskListener.ModelInstance);
            Assert.IsNull(ModelExecutionContextTaskListener.UserTask);

            taskService.Complete(taskId);

            AssertModelInstance();
            AssertUserTask(eventName);
        }


        [Test]
        public virtual void testGetBpmnModelElementInstanceOnCreate()
        {
            var eventName = TaskListenerFields.EventnameCreate;
            deployProcess(eventName);

            runtimeService.StartProcessInstanceByKey(PROCESS_ID);

            AssertModelInstance();
            AssertUserTask(eventName);

            var taskId = taskService.CreateTaskQuery(c => c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First()
                .Id;
            taskService.Complete(taskId);
        }
    }
}