using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;
using IoUtil = ESS.FW.Bpm.Engine.Impl.Util.IoUtil;

namespace Engine.Tests.Bpmn.Deployment
{
    [TestFixture]
    public class BpmnDeploymentTest : PluggableProcessEngineTestCase
    {
        public BpmnDeploymentTest()
        {
            ClearDeploymentAll = true;
        }
        [Test]
        [Deployment]
        public virtual void TestGetBpmnXmlFileThroughService()
        {
            string deploymentId = repositoryService.CreateDeploymentQuery().First().Id;
            IList<string> deploymentResources = repositoryService.GetDeploymentResourceNames(deploymentId);

            // verify bpmn file name
            Assert.AreEqual(1, deploymentResources.Count);
            string bpmnResourceName = "resources/Bpmn/Deployment/BpmnDeploymentTest.TestGetBpmnXmlFileThroughService.bpmn20.xml";
            Assert.AreEqual(bpmnResourceName, deploymentResources[0]);

            IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
            Assert.AreEqual(bpmnResourceName, processDefinition.ResourceName);
            Assert.IsNull(processDefinition.DiagramResourceName);
            Assert.IsFalse(processDefinition.GetHasStartFormKey());

            IReadOnlyProcessDefinition readOnlyProcessDefinition = ((RepositoryServiceImpl)repositoryService).GetDeployedProcessDefinition(processDefinition.Id);
            Assert.IsNull(readOnlyProcessDefinition.DiagramResourceName);

            // verify content
            System.IO.Stream deploymentInputStream = repositoryService.GetResourceAsStream(deploymentId, bpmnResourceName);
            string contentFromDeployment = ReadInputStreamToString(deploymentInputStream);
            Assert.True(contentFromDeployment.Length > 0);
            Assert.True(contentFromDeployment.Contains("process id=\"emptyProcess\""));

            System.IO.Stream fileInputStream = ReflectUtil.GetResourceAsStream("resources/Bpmn/Deployment/BpmnDeploymentTest.TestGetBpmnXmlFileThroughService.bpmn20.xml");
            string contentFromFile = ReadInputStreamToString(fileInputStream);
            Assert.AreEqual(contentFromFile, contentFromDeployment);
        }

        private string ReadInputStreamToString(System.IO.Stream inputStream)
        {
            byte[] bytes = IoUtil.ReadInputStream(inputStream, "input stream");            
            return StringHelperClass.NewString(bytes);
        }
        //[Test]//验证ProcessDefinitionEntity的Key数据库限制最大64
        public virtual void FAILING_testViolateProcessDefinitionIdMaximumLength()
        {
            try
            {
                repositoryService.CreateDeployment().AddClasspathResource("resources/Bpmn/Deployment/processWithLongId.bpmn20.xml").Deploy();
                //Assert.Fail();
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("id can be maximum 64 characters", e.Message);
            }

            // Verify that nothing is deployed
            Assert.AreEqual(0, repositoryService.CreateDeploymentQuery().Count());
        }
        [Test]
        public virtual void TestDeploySameFileTwice()
        {
            string bpmnResourceName = "resources/Bpmn/Deployment/BpmnDeploymentTest.TestGetBpmnXmlFileThroughService.bpmn20.xml";
            repositoryService.CreateDeployment().EnableDuplicateFiltering().AddClasspathResource(bpmnResourceName).Name("twice").Deploy();

            string deploymentId = repositoryService.CreateDeploymentQuery().First().Id;
            IList<string> deploymentResources = repositoryService.GetDeploymentResourceNames(deploymentId);

            // verify bpmn file name
            Assert.AreEqual(1, deploymentResources.Count);
            Assert.AreEqual(bpmnResourceName, deploymentResources[0]);

            repositoryService.CreateDeployment().EnableDuplicateFiltering().AddClasspathResource(bpmnResourceName).Name("twice").Deploy();
            IList<IDeployment> deploymentList = repositoryService.CreateDeploymentQuery().ToList();
            Assert.AreEqual(1, deploymentList.Count);

            repositoryService.DeleteDeployment(deploymentId);
        }
        [Test]
        public virtual void TestPartialChangesDeployAll()
        {
            IBpmnModelInstance model1 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process1").Done();
            IBpmnModelInstance model2 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process2").Done();
            IDeployment deployment1 = repositoryService.CreateDeployment().EnableDuplicateFiltering().AddModelInstance("process1.bpmn20.xml", model1).AddModelInstance("process2.bpmn20.xml", model2).Name("twice").Deploy();

            IList<string> deploymentResources = repositoryService.GetDeploymentResourceNames(deployment1.Id);
            Assert.AreEqual(2, deploymentResources.Count);

            IBpmnModelInstance changedModel2 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process2").StartEvent().Done();

            IDeployment deployment2 = repositoryService.CreateDeployment().EnableDuplicateFiltering().AddModelInstance("process1.bpmn20.xml", model1).AddModelInstance("process2.bpmn20.xml", changedModel2).Name("twice").Deploy();
            IList<IDeployment> deploymentList = repositoryService.CreateDeploymentQuery().ToList();
            Assert.AreEqual(2, deploymentList.Count);

            // there should be new versions of both processes
            Assert.AreEqual(2, repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "process1").Count());
            Assert.AreEqual(2, repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "process2").Count());

            repositoryService.DeleteDeployment(deployment1.Id);
            repositoryService.DeleteDeployment(deployment2.Id);
        }
        [Test]
        public virtual void TestPartialChangesDeployChangedOnly()
        {
            IBpmnModelInstance model1 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process1").Done();
            IBpmnModelInstance model2 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process2").Done();
            IDeployment deployment1 = repositoryService.CreateDeployment().AddModelInstance("process1.bpmn20.xml", model1).AddModelInstance("process2.bpmn20.xml", model2).Name("thrice").Deploy();

            IList<string> deploymentResources = repositoryService.GetDeploymentResourceNames(deployment1.Id);
            Assert.AreEqual(2, deploymentResources.Count);

            IBpmnModelInstance changedModel2 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process2").StartEvent().Done();

            IDeployment deployment2 = repositoryService.CreateDeployment().EnableDuplicateFiltering(true).AddModelInstance("process1.bpmn20.xml", model1).AddModelInstance("process2.bpmn20.xml", changedModel2).Name("thrice").Deploy();

            IList<IDeployment> deploymentList = repositoryService.CreateDeploymentQuery().ToList();
            Assert.AreEqual(2, deploymentList.Count);

            // there should be only one version of process 1
            IProcessDefinition process1Definition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "process1").First();
            Assert.NotNull(process1Definition);
            Assert.AreEqual(1, process1Definition.Version);
            Assert.AreEqual(deployment1.Id, process1Definition.DeploymentId);

            // there should be two versions of process 2
            Assert.AreEqual(2, repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "process2").Count());

            IBpmnModelInstance anotherChangedModel2 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process2").StartEvent().EndEvent().Done();

            // testing with a third deployment to ensure the change check is not only performed against
            // the last version of the deployment
            IDeployment deployment3 = repositoryService.CreateDeployment().EnableDuplicateFiltering(true).AddModelInstance("process1.bpmn20.xml", model1).AddModelInstance("process2.bpmn20.xml", anotherChangedModel2).Name("thrice").Deploy();

            // there should still be one version of process 1
            Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "process1").Count());

            // there should be three versions of process 2
            Assert.AreEqual(3, repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "process2").Count());

            repositoryService.DeleteDeployment(deployment1.Id);
            repositoryService.DeleteDeployment(deployment2.Id);
            repositoryService.DeleteDeployment(deployment3.Id);
        }

        [Test]
        public virtual void TestPartialChangesRedeployOldVersion()
        {
            // deployment 1 deploys process version 1
            IBpmnModelInstance model1 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process1").Done();
            IDeployment deployment1 = repositoryService.CreateDeployment().AddModelInstance("process1.bpmn20.xml", model1).Name("deployment").Deploy();

            // deployment 2 deploys process version 2
            IBpmnModelInstance changedModel1 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process1").StartEvent().Done();
            IDeployment deployment2 = repositoryService.CreateDeployment().EnableDuplicateFiltering(true).AddModelInstance("process1.bpmn20.xml", changedModel1).Name("deployment").Deploy();

            // deployment 3 deploys process version 1 again
            IDeployment deployment3 = repositoryService.CreateDeployment().EnableDuplicateFiltering(true).AddModelInstance("process1.bpmn20.xml", model1).Name("deployment").Deploy();

            // should result in three process definitions
            Assert.AreEqual(3, repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "process1").Count());

            repositoryService.DeleteDeployment(deployment1.Id);
            repositoryService.DeleteDeployment(deployment2.Id);
            repositoryService.DeleteDeployment(deployment3.Id);
        }
        [Test]
        public virtual void TestDeployTwoProcessesWithDuplicateIdAtTheSameTime()
        {
            try
            {
                string bpmnResourceName = "resources/Bpmn/Deployment/BpmnDeploymentTest.TestGetBpmnXmlFileThroughService.bpmn20.xml";
                string bpmnResourceName2 = "resources/Bpmn/Deployment/BpmnDeploymentTest.TestGetBpmnXmlFileThroughService2.bpmn20.xml";
                repositoryService.CreateDeployment().EnableDuplicateFiltering().AddClasspathResource(bpmnResourceName).AddClasspathResource(bpmnResourceName2).Name("duplicateAtTheSameTime").Deploy();
                Assert.Fail();
            }
            catch (System.Exception)
            {
                // Verify that nothing is deployed
                Assert.AreEqual(0, repositoryService.CreateDeploymentQuery().Count());
            }
        }
        [Test]
        public virtual void TestDeployDifferentFiles()
        {
            string bpmnResourceName = "resources/Bpmn/Deployment/BpmnDeploymentTest.TestGetBpmnXmlFileThroughService.bpmn20.xml";
            repositoryService.CreateDeployment().EnableDuplicateFiltering(false).AddClasspathResource(bpmnResourceName).Name("twice").Deploy();

            string deploymentId = repositoryService.CreateDeploymentQuery().First().Id;
            IList<string> deploymentResources = repositoryService.GetDeploymentResourceNames(deploymentId);

            // verify bpmn file name
            Assert.AreEqual(1, deploymentResources.Count);
            Assert.AreEqual(bpmnResourceName, deploymentResources[0]);

            bpmnResourceName = "resources/bpmn/deployment/BpmnDeploymentTest.TestProcessDiagramResource.bpmn20.xml";
            repositoryService.CreateDeployment().EnableDuplicateFiltering().AddClasspathResource(bpmnResourceName).Name("twice").Deploy();
            IList<IDeployment> deploymentList = repositoryService.CreateDeploymentQuery().ToList();
            Assert.AreEqual(2, deploymentList.Count);

            deleteDeployments(deploymentList);
        }
        [Test]
        public virtual void TestDiagramCreationDisabled()
        {
            repositoryService.CreateDeployment().AddClasspathResource("resources/Bpmn/Parse/BpmnParseTest.TestParseDiagramInterchangeElements.bpmn20.xml").Deploy();

            // Graphical information is not yet exposed publicly, so we need to do some plumbing
            ICommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
            ProcessDefinitionEntity processDefinitionEntity = commandExecutor.Execute(new CommandAnonymousInnerClass(this));

            Assert.NotNull(processDefinitionEntity);
            Assert.AreEqual(7, processDefinitionEntity.Activities.Count);

            // Check that no diagram has been created
            IList<string> resourceNames = repositoryService.GetDeploymentResourceNames(processDefinitionEntity.DeploymentId);
            Assert.AreEqual(1, resourceNames.Count);

            repositoryService.DeleteDeployment(repositoryService.CreateDeploymentQuery().First().Id, true);
        }

        private class CommandAnonymousInnerClass : ICommand<ProcessDefinitionEntity>
        {
            private readonly BpmnDeploymentTest _outerInstance;

            public CommandAnonymousInnerClass(BpmnDeploymentTest outerInstance)
            {
                this._outerInstance = outerInstance;
            }

            public ProcessDefinitionEntity Execute(CommandContext commandContext)
            {
                return Context.ProcessEngineConfiguration.DeploymentCache.FindDeployedLatestProcessDefinitionByKey("myProcess");
            }

            //public override ProcessDefinitionEntity execute(CommandContext commandContext)
            //{
            //    return Context.ProcessEngineConfiguration.DeploymentCache.FindDeployedLatestProcessDefinitionByKey("myProcess");
            //}
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/deployment/BpmnDeploymentTest.TestProcessDiagramResource.bpmn20.xml", "resources/bpmn/deployment/BpmnDeploymentTest.TestProcessDiagramResource.jpg" })]
        public virtual void TestProcessDiagramResource()
        {
            IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();

            Assert.AreEqual("resources/Bpmn/Deployment/BpmnDeploymentTest.TestProcessDiagramResource.bpmn20.xml".ToLower(), processDefinition.ResourceName.ToLower());
            Assert.True(processDefinition.GetHasStartFormKey());

            string diagramResourceName = processDefinition.DiagramResourceName;
            Assert.AreEqual("resources/Bpmn/Deployment/BpmnDeploymentTest.TestProcessDiagramResource.jpg".ToLower(), diagramResourceName.ToLower());

            System.IO.Stream diagramStream = repositoryService.GetResourceAsStream(DeploymentId, "resources/Bpmn/Deployment/BpmnDeploymentTest.TestProcessDiagramResource.jpg");
            byte[] diagramBytes = IoUtil.ReadInputStream(diagramStream, "diagram stream");
            Assert.AreEqual(33343, diagramBytes.Length);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/deployment/BpmnDeploymentTest.TestMultipleDiagramResourcesProvided.bpmn20.xml", "resources/bpmn/deployment/BpmnDeploymentTest.TestMultipleDiagramResourcesProvided.a.jpg", "resources/bpmn/deployment/BpmnDeploymentTest.TestMultipleDiagramResourcesProvided.b.jpg", "resources/bpmn/deployment/BpmnDeploymentTest.TestMultipleDiagramResourcesProvided.c.jpg" })]
        public virtual void TestMultipleDiagramResourcesProvided()
        {
            IProcessDefinition processA = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "a").First();
            IProcessDefinition processB = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "b").First();
            IProcessDefinition processC = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "c").First();

            Assert.AreEqual("resources/Bpmn/Deployment/BpmnDeploymentTest.TestMultipleDiagramResourcesProvided.a.jpg".ToLower(), processA.DiagramResourceName.ToLower());
            Assert.AreEqual("resources/Bpmn/Deployment/BpmnDeploymentTest.TestMultipleDiagramResourcesProvided.b.jpg".ToLower(), processB.DiagramResourceName.ToLower());
            Assert.AreEqual("resources/Bpmn/Deployment/BpmnDeploymentTest.TestMultipleDiagramResourcesProvided.c.jpg".ToLower(), processC.DiagramResourceName.ToLower());
        }

        [Test]
        [Deployment]
        public virtual void TestProcessDefinitionDescription()
        {
            string id = repositoryService.CreateProcessDefinitionQuery().First().Id;
            IReadOnlyProcessDefinition processDefinition = ((RepositoryServiceImpl)repositoryService).GetDeployedProcessDefinition(id);
            Assert.AreEqual("This is really good process documentation!", processDefinition.Description);
        }
        [Test]
        public virtual void TestDeployInvalidExpression()
        {
            // ACT-1391: Deploying a process with invalid expressions inside should cause the deployment to Assert.Fail, since
            // the process is not deployed and useless..
            try
            {
                //repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/deployment/BpmnDeploymentTest.TestInvalidExpression.bpmn20.xml").Deploy();
                repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/deployment/test.xml").Deploy();

                Assert.Fail("Expected exception when deploying process with invalid expression.");
            }
            catch (ProcessEngineException expected)
            {
                // Check if no deployments are made
                Assert.AreEqual(0, repositoryService.CreateDeploymentQuery().Count());
                Assert.True(expected.Message.Contains("ENGINE-01009 Error while parsing process"));
            }
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/deployment/BpmnDeploymentTest.TestGetBpmnXmlFileThroughService.bpmn20.xml" })]
        public virtual void TestDeploymentIdOfResource()
        {
            string deploymentId = repositoryService.CreateDeploymentQuery().First().Id;

            IList<IResource> resources = repositoryService.GetDeploymentResources(deploymentId);
            Assert.AreEqual(1, resources.Count);

            IResource resource = resources[0];
            Assert.AreEqual(deploymentId, resource.DeploymentId);
        }

        private void deleteDeployments(IList<IDeployment> deploymentList)
        {
            foreach (IDeployment deployment in deploymentList)
            {
                repositoryService.DeleteDeployment(deployment.Id);
            }
        }
        [Test]
        public virtual void TestDeployBpmnModelInstance()
        {
            // given
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess("foo").StartEvent().UserTask().EndEvent().Done();
            IBpmnModelInstance modelInstance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("foo").StartEvent().UserTask().EndEvent().Done();

            // when
            DeploymentWithBuilder(repositoryService.CreateDeployment().AddModelInstance("foo.bpmn", modelInstance));

            // then
            Assert.NotNull(repositoryService.CreateProcessDefinitionQuery(c=>c.ResourceName == "foo.bpmn").First());
        }
        [Test]
        public virtual void TestDeployAndGetProcessDefinition()
        {

            // given process model
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess("foo").StartEvent().UserTask().EndEvent().Done();
            IBpmnModelInstance modelInstance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("foo").StartEvent().UserTask().EndEvent().Done();

            // when process model is deployed
            IDeploymentWithDefinitions deployment = repositoryService.CreateDeployment().AddModelInstance("foo.bpmn", modelInstance).DeployAndReturnDefinitions();
            DeploymentIds.Add(deployment.Id);

            // then deployment contains deployed process definitions
            IList<IProcessDefinition> deployedProcessDefinitions = deployment.DeployedProcessDefinitions;
            Assert.AreEqual(1, deployedProcessDefinitions.Count);
            Assert.IsNull(deployment.DeployedCaseDefinitions);
            Assert.IsNull(deployment.DeployedDecisionDefinitions);
            Assert.IsNull(deployment.DeployedDecisionRequirementsDefinitions);

            // and persisted process definition is equal to deployed process definition
            IProcessDefinition persistedProcDef = repositoryService.CreateProcessDefinitionQuery(c=>c.ResourceName == "foo.bpmn").First();
            Assert.AreEqual(persistedProcDef.Id, deployedProcessDefinitions[0].Id);
        }
        [Test]
        public virtual void TestDeployNonExecutableProcess()
        {

            // given non executable process definition
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateProcess("foo").StartEvent().UserTask().EndEvent().Done();
            IBpmnModelInstance modelInstance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateProcess("foo").StartEvent().UserTask().EndEvent().Done();

            // when process model is deployed
            IDeploymentWithDefinitions deployment = repositoryService.CreateDeployment().AddModelInstance("foo.bpmn", modelInstance).DeployAndReturnDefinitions();
            DeploymentIds.Add(deployment.Id);

            // then deployment contains no deployed process definition
            Assert.IsNull(deployment.DeployedProcessDefinitions);

            // and there exist no persisted process definitions
            Assert.IsNull(repositoryService.CreateProcessDefinitionQuery(c=>c.ResourceName == "foo.bpmn").First());
        }

    }

}