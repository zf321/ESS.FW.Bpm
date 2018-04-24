using System.Linq;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    [TestFixture]
    public class ProcessInstantiationAtStartEventTest : PluggableProcessEngineTestCase
    {
        protected internal const string PROCESS_DEFINITION_KEY = "testProcess";

        [SetUp]
        protected internal void setUp()
        {
            Deployment(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY)
                .StartEvent()
                .UserTask()
                .EndEvent()
                .Done());
        }

        [Test]
        public virtual void testStartProcessInstanceById()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            runtimeService.CreateProcessInstanceById(processDefinition.Id)
                .Execute();

            Assert.That(runtimeService.CreateProcessInstanceQuery()
                .Count(), Is.EqualTo(1L));
        }

        [Test]
        public virtual void testStartProcessInstanceByKey()
        {
            runtimeService.CreateProcessInstanceByKey(PROCESS_DEFINITION_KEY)
                .Execute();

            Assert.That(runtimeService.CreateProcessInstanceQuery()
                .Count(), Is.EqualTo(1L));
        }

        [Test]
        public virtual void testStartProcessInstanceAndSetBusinessKey()
        {
            runtimeService.CreateProcessInstanceByKey(PROCESS_DEFINITION_KEY)
                .SetBusinessKey("businessKey")
                .Execute();

            var processInstance = runtimeService.CreateProcessInstanceQuery()
                .First();
            Assert.That(processInstance, Is.Not.Null);
            Assert.That(processInstance.BusinessKey, Is.EqualTo("businessKey"));
        }

        [Test]
        public virtual void testStartProcessInstanceAndSetCaseInstanceId()
        {
            runtimeService.CreateProcessInstanceByKey(PROCESS_DEFINITION_KEY)
                .SetCaseInstanceId("caseInstanceId")
                .Execute();

            var processInstance = runtimeService.CreateProcessInstanceQuery()
                .First();
            Assert.That(processInstance, Is.Not.Null);
            Assert.That(processInstance.CaseInstanceId, Is.EqualTo("caseInstanceId"));
        }

        [Test]
        public virtual void testStartProcessInstanceAndSetVariable()
        {
            var processInstance = runtimeService.CreateProcessInstanceByKey(PROCESS_DEFINITION_KEY)
                .SetVariable("var", "value")
                .Execute();

            var variable = runtimeService.GetVariable(processInstance.Id, "var");
            Assert.That(variable, Is.Not.Null);
            Assert.That(variable, Is.EqualTo("value"));
        }

        [Test]
        public virtual void testStartProcessInstanceAndSetVariables()
        {
            var variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("var1", new StringValueImpl("v1"))
                .PutValue("var2", new StringValueImpl("v2"));

            var processInstance = runtimeService.CreateProcessInstanceByKey(PROCESS_DEFINITION_KEY)
                //.SetVariables(variables)
                .Execute();

            Assert.That(runtimeService.GetVariables(processInstance.Id), Is.EqualTo(variables));
        }

        [Test]
        public virtual void testStartProcessInstanceNoSkipping()
        {
            runtimeService.CreateProcessInstanceByKey(PROCESS_DEFINITION_KEY)
                .Execute(false, false);

            Assert.That(runtimeService.CreateProcessInstanceQuery()
                .Count(), Is.EqualTo(1L));
        }

        [Test]
        public virtual void testFailToStartProcessInstanceSkipListeners()
        {
            try
            {
                runtimeService.CreateProcessInstanceByKey(PROCESS_DEFINITION_KEY)
                    .Execute(true, false);

                Assert.Fail("expected exception");
            }
            catch (BadUserRequestException e)
            {
                Assert.That(e.Message, Does.Contain("Cannot skip"));
            }
        }

        [Test]
        public virtual void testFailToStartProcessInstanceSkipInputOutputMapping()
        {
            try
            {
                runtimeService.CreateProcessInstanceByKey(PROCESS_DEFINITION_KEY)
                    .Execute(false, true);

                Assert.Fail("expected exception");
            }
            catch (BadUserRequestException e)
            {
                Assert.That(e.Message, Does.Contain("Cannot skip"));
            }
        }
    }
}