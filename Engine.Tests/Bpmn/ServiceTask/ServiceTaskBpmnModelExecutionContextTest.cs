//using NUnit.Framework;
//using System.Collections.Generic;
//using System.Linq;
//using ESS.FW.Bpm.Model.Bpmn;
//using ESS.FW.Bpm.Model.Bpmn.instance;
//using IEvent = ESS.FW.Bpm.Engine.Task.IEvent;
//using ITask = ESS.FW.Bpm.Engine.Task.ITask;

//namespace ESS.FW.Bpm.Engine.Tests.Bpmn.ServiceTask
//{
//    [TestFixture]
//    public class ServiceTaskBpmnModelExecutionContextTest : PluggableProcessEngineTestCase
//    {

//        private const string PROCESS_ID = "process";
//        private string deploymentId;

//        [Test]
//        public virtual void testJavaDelegateModelExecutionContext()
//        {
//            deploy();

//            runtimeService.StartProcessInstanceByKey(PROCESS_ID);

//            var modelInstance = ModelExecutionContextServiceTask.ModelInstance;
//            Assert.NotNull(modelInstance);

//            var model = modelInstance.Model;
//            var events = modelInstance.GetModelElementsByType(typeof(IEvent)); //model.GetType(typeof(Event)));
//            Assert.AreEqual(2, events.Count);
//            var tasks = modelInstance.GetModelElementsByType(typeof(ITask)); //model.GetType(typeof(ITask)));
//            Assert.AreEqual(1, tasks.Count);
            
//            var process = (IProcess) modelInstance.Definitions.RootElements.ToList().First(); //next
//            Assert.AreEqual(PROCESS_ID, process.Id);
//            Assert.True(process.Executable);

//            IServiceTask serviceTask = ModelExecutionContextServiceTask.ServiceTask;
//            Assert.NotNull(serviceTask);

//            Assert.AreEqual(typeof(ModelExecutionContextServiceTask).FullName, serviceTask.CamundaClass);
//        }

//        private void deploy()
//        {
//            IBpmnModelInstance modelInstance =
//                Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_ID)
//                    .StartEvent()
//                    .ServiceTask()
//                    .CamundaClass(typeof(ModelExecutionContextServiceTask).FullName)
//                    .EndEvent()
//                    .Done();

//            deploymentId =
//                repositoryService.CreateDeployment().AddModelInstance("process.bpmn", modelInstance).Deploy().Id;
//        }
//        [TearDown]
//        public virtual void tearDown()
//        {
//            ModelExecutionContextServiceTask.Clear();
//            repositoryService.DeleteDeployment(deploymentId, true);
//        }

//    }

//}