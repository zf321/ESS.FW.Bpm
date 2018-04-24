using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.instance;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Common
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public abstract class AbstractProcessEngineServicesAccessTest : PluggableProcessEngineTestCase
    {
        private const string TaskDefKey = "someTask";

        private const string ProcessDefKey = "testProcess";

        private const string CalledProcessDefId = "calledProcess";

        private const string CALLED_PROCESS_DEF_ID = "calledProcess";

        protected internal IList<string> deploymentIds = new List<string>();

        [TearDown]
        protected internal void tearDown()
        {
            foreach (string deploymentId in deploymentIds)
            {
                repositoryService.DeleteDeployment(deploymentId, true);
            }
            base.TearDown();
        }
        public virtual void testServicesAccessible()
        {
            // this test makes sure that the process engine services can be accessed and are non-null.
            CreateAndDeployModelForClass(TestServiceAccessibleClass);

            // this would Assert.Fail if api access was not assured.
            runtimeService.StartProcessInstanceByKey(ProcessDefKey);
        }

        public virtual void TestQueryAccessible()
        {
            // this test makes sure we can perform a query
            CreateAndDeployModelForClass(QueryClass);

            // this would Assert.Fail if api access was not assured.
            runtimeService.StartProcessInstanceByKey(ProcessDefKey);
        }

        public virtual void testStartProcessInstance()
        {

            // given
            CreateAndDeployModelForClass(StartProcessInstanceClass);

            DeployModel(
                ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(CalledProcessDefId)
                    .StartEvent()
                    .UserTask(TaskDefKey)
                    .EndEvent()
                    .Done());

            // if
            runtimeService.StartProcessInstanceByKey(ProcessDefKey);

            // then
            // the started process instance is still active and Waiting at the IUser task
            Assert.AreEqual(1, taskService.CreateTaskQuery(c => c.TaskDefinitionKey == TaskDefKey).Count());
        }

        public virtual void testStartProcessInstanceFails()
        {

            // given
            CreateAndDeployModelForClass(StartProcessInstanceClass);

            //var modelInstance =
            var start = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(CalledProcessDefId)
                  .StartEvent();
            start.ScriptTask("ScriptTask")
            .ScriptFormat("groovy")
            .ScriptText("throw new RuntimeException(\"BOOOM!\")");
            var modelInstance = start.EndEvent()
                    .Done();

            DeployModel(modelInstance);

            // if
            try
            {
                runtimeService.StartProcessInstanceByKey(ProcessDefKey);
                Assert.Fail("exception expected");
            }
            catch (System.Exception e)
            {
                AssertTextPresent("BOOOM", e.Message);
            }

            // then
            // starting the process fails and everything is rolled back:
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery().Count());
        }

        protected internal abstract Type TestServiceAccessibleClass { get; }

        protected internal abstract Type QueryClass { get; }

        protected internal abstract Type StartProcessInstanceClass { get; }

        protected internal abstract ITask CreateModelAccessTask(IBpmnModelInstance modelInstance, Type delegateClass);

        // Helper methods //////////////////////////////////////////////

        private void CreateAndDeployModelForClass(Type delegateClass)
        {
            var modelInstance =
                ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(ProcessDefKey)
                    .StartEvent()
                    .ManualTask("templateTask")
                    .EndEvent()
                    .Done();

            // replace the template task with the actual task provided by the subtask
            //modelInstance.GetModelElementById<ITask>("templateTask").ReplaceWithElement(CreateModelAccessTask(modelInstance, delegateClass));

            DeployModel(modelInstance);
        }


        private void DeployModel(IBpmnModelInstance model)
        {
            var deployment = repositoryService.CreateDeployment().AddModelInstance("testProcess.bpmn", model).Deploy();
            deploymentIds.Add(deployment.Id);
        }


        public static void AssertCanAccessServices(IProcessEngineServices services)
        {
            Assert.NotNull(services.AuthorizationService);
            Assert.NotNull(services.FormService);
            Assert.NotNull(services.HistoryService);
            Assert.NotNull(services.IdentityService);
            Assert.NotNull(services.ManagementService);
            Assert.NotNull(services.RepositoryService);
            Assert.NotNull(services.RuntimeService);
            Assert.NotNull(services.TaskService);
        }

        public static void AssertCanPerformQuery(IProcessEngineServices services)
        {
            services.RepositoryService.CreateProcessDefinitionQuery().Count();
        }

        public static void AssertCanStartProcessInstance(IProcessEngineServices services)
        {
            services.RuntimeService.StartProcessInstanceByKey(CalledProcessDefId);
        }
    }
}