using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Application.Impl;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;


namespace Engine.Tests.Api.Repository
{
    

	/// <summary>
	/// 
	/// 
	/// </summary>
	public class ProcessApplicationDeploymentTest : PluggableProcessEngineTestCase
	{

	  private EmbeddedProcessApplication processApplication;

        [SetUp]
	  protected internal virtual void setUp()
	  {
		processApplication = new EmbeddedProcessApplication();
	  }

	   [Test]   public virtual void testEmptyDeployment()
	  {
		try
		{
		  repositoryService.CreateDeployment(processApplication.Reference).Deploy();
		  Assert.Fail("it should not be possible to deploy without deployment resources");
		}
		catch (NotValidException)
		{
		  // expected
		}

		try
		{
		  repositoryService.CreateDeployment().Deploy();
		  Assert.Fail("it should not be possible to deploy without deployment resources");
		}
		catch (NotValidException)
		{
		  // expected
		}
	  }

	   [Test]   public virtual void testSimpleProcessApplicationDeployment()
	  {

		var deployment = repositoryService.CreateDeployment(processApplication.Reference).AddClasspathResource("resources/api/repository/version1.bpmn20.xml").Deploy() as IProcessApplicationDeployment;

		// process is deployed:
		AssertThatOneProcessIsDeployed();

		// registration was performed:
		var registration = deployment.ProcessApplicationRegistration;
            IList<string> deploymentIds = registration.DeploymentIds;
		Assert.AreEqual(1, deploymentIds.Count);
		Assert.AreEqual(ProcessEngine.Name, registration.ProcessEngineName);

		deleteDeployments(deployment);
	  }

	   [Test]   public virtual void testProcessApplicationDeploymentNoChanges()
	  {
            // create initial deployment
        IProcessApplicationDeployment deployment = repositoryService.CreateDeployment(processApplication.Reference).Name("deployment").AddClasspathResource("resources/api/repository/version1.bpmn20.xml").Deploy() as IProcessApplicationDeployment;

		AssertThatOneProcessIsDeployed();

		// deploy update with no changes:
		deployment = repositoryService.CreateDeployment(processApplication.Reference).Name("deployment").EnableDuplicateFiltering(false).AddClasspathResource("resources/api/repository/version1.bpmn20.xml").Deploy() as IProcessApplicationDeployment;

		// no changes
		AssertThatOneProcessIsDeployed();
		var registration = deployment.ProcessApplicationRegistration;
            IList<string> deploymentIds = registration.DeploymentIds;
		Assert.AreEqual(1, deploymentIds.Count);
		Assert.AreEqual(ProcessEngine.Name, registration.ProcessEngineName);

		deleteDeployments(deployment);
	  }

	   [Test]   public virtual void testPartialChangesDeployAll()
	  {
		IBpmnModelInstance model1 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process1").Done();
		IBpmnModelInstance model2 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process2").Done();

		// create initial deployment
		var deployment1 = repositoryService.CreateDeployment(processApplication.Reference).Name("deployment").AddModelInstance("process1.bpmn20.xml", model1).AddModelInstance("process2.bpmn20.xml", model2).Deploy();

		IBpmnModelInstance changedModel2 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process2").StartEvent().Done();

		// second deployment with partial changes:
		IProcessApplicationDeployment deployment2 = repositoryService.CreateDeployment(processApplication.Reference).ResumePreviousVersions().Name("deployment").EnableDuplicateFiltering(false)
                .AddModelInstance("process1.bpmn20.xml", model1).AddModelInstance("process2.bpmn20.xml", changedModel2)
                .Deploy() as IProcessApplicationDeployment;

		Assert.AreEqual(4, repositoryService.CreateProcessDefinitionQuery().Count());

	      IList<IProcessDefinition> processDefinitionsModel1 = repositoryService.CreateProcessDefinitionQuery()
	          //.ProcessDefinitionKey("process1")
	          //.OrderByProcessDefinitionVersion()
	          /*.Asc()*/
	          .ToList();

		// now there are two versions of process1 deployed
		Assert.AreEqual(2, processDefinitionsModel1.Count);
		Assert.AreEqual(1, processDefinitionsModel1[0].Version);
		Assert.AreEqual(2, processDefinitionsModel1[1].Version);

		// now there are two versions of process2 deployed
		IList<IProcessDefinition> processDefinitionsModel2 = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "process1").ToList();//.OrderByProcessDefinitionVersion()/*.Asc()*/.ToList();

		Assert.AreEqual(2, processDefinitionsModel2.Count);
		Assert.AreEqual(1, processDefinitionsModel2[0].Version);
		Assert.AreEqual(2, processDefinitionsModel2[1].Version);

		// old deployment was resumed
		var registration = deployment2.ProcessApplicationRegistration;
            IList<string> deploymentIds = registration.DeploymentIds;
		Assert.AreEqual(2, deploymentIds.Count);
		Assert.AreEqual(ProcessEngine.Name, registration.ProcessEngineName);

		deleteDeployments(deployment1, deployment2);
	  }

	  /// <summary>
	  /// Test re-deployment of only those resources that have actually changed
	  /// </summary>
	   [Test]   public virtual void testPartialChangesDeployChangedOnly()
	  {
		IBpmnModelInstance model1 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process1").Done();
		IBpmnModelInstance model2 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process2").Done();

		// create initial deployment
		var deployment1 = repositoryService.CreateDeployment(processApplication.Reference).Name("deployment").AddModelInstance("process1.bpmn20.xml", model1).AddModelInstance("process2.bpmn20.xml", model2).Deploy();

		IBpmnModelInstance changedModel2 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process2").StartEvent().Done();

		// second deployment with partial changes:
		IProcessApplicationDeployment deployment2 = repositoryService.CreateDeployment(processApplication.Reference).ResumePreviousVersions().Name("deployment").EnableDuplicateFiltering(true)
                .AddModelInstance("process1.bpmn20.xml", model1).AddModelInstance("process2.bpmn20.xml", changedModel2)
                .Deploy() as IProcessApplicationDeployment;

		Assert.AreEqual(3, repositoryService.CreateProcessDefinitionQuery().Count());

		// there is one version of process1 deployed
		IProcessDefinition processDefinitionModel1 = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "process1").First();

		Assert.NotNull(processDefinitionModel1);
		Assert.AreEqual(1, processDefinitionModel1.Version);
		Assert.AreEqual(deployment1.Id, processDefinitionModel1.DeploymentId);

		// there are two versions of process2 deployed
		IList<IProcessDefinition> processDefinitionsModel2 = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "process2").ToList();//.OrderByProcessDefinitionVersion()/*.Asc()*/.ToList();

		Assert.AreEqual(2, processDefinitionsModel2.Count);
		Assert.AreEqual(1, processDefinitionsModel2[0].Version);
		Assert.AreEqual(2, processDefinitionsModel2[1].Version);

		// old deployment was resumed
		var registration = deployment2.ProcessApplicationRegistration;
            IList<string> deploymentIds = registration.DeploymentIds;
		Assert.AreEqual(2, deploymentIds.Count);

		IBpmnModelInstance anotherChangedModel2 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process2").StartEvent().EndEvent().Done();

		// testing with a third deployment to ensure the change check is not only performed against
		// the last version of the deployment
		IProcessApplicationDeployment deployment3 = repositoryService.CreateDeployment(processApplication.Reference).ResumePreviousVersions()
                .EnableDuplicateFiltering(true).AddModelInstance("process1.bpmn20.xml", model1)
                .AddModelInstance("process2.bpmn20.xml", anotherChangedModel2).Name("deployment")
                .Deploy() as IProcessApplicationDeployment;

		// there should still be one version of process 1
		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "process1").Count());

		// there should be three versions of process 2
		Assert.AreEqual(3, repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "process2").Count());

		// old deployments are resumed
		registration = deployment3.ProcessApplicationRegistration;
		deploymentIds = registration.DeploymentIds;
		Assert.AreEqual(3, deploymentIds.Count);

		deleteDeployments(deployment1, deployment2, deployment3);
	  }

	   [Test]   public virtual void testPartialChangesResumePreviousVersion()
	  {
		IBpmnModelInstance model1 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process1").Done();
		IBpmnModelInstance model2 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process2").Done();

		// create initial deployment
		var deployment1 = repositoryService.CreateDeployment(processApplication.Reference).Name("deployment").AddModelInstance("process1.bpmn20.xml", model1).Deploy();

		var deployment2 = repositoryService.CreateDeployment(processApplication.Reference).ResumePreviousVersions().Name("deployment").EnableDuplicateFiltering(true).AddModelInstance("process1.bpmn20.xml", model1).AddModelInstance("process2.bpmn20.xml", model2).Deploy() as IProcessApplicationDeployment;

		var registration = deployment2.ProcessApplicationRegistration;
		Assert.AreEqual(2, registration.DeploymentIds.Count);

		deleteDeployments(deployment1, deployment2);
	  }

	   [Test]   public virtual void testProcessApplicationDeploymentResumePreviousVersions()
	  {
		// create initial deployment
		var deployment1 = repositoryService.CreateDeployment(processApplication.Reference).Name("deployment").AddClasspathResource("resources/api/repository/version1.bpmn20.xml").Deploy();

		AssertThatOneProcessIsDeployed();

		// deploy update with changes:
		IProcessApplicationDeployment deployment2 = repositoryService.CreateDeployment(processApplication.Reference).ResumePreviousVersions().Name("deployment").EnableDuplicateFiltering(false).AddClasspathResource("resources/api/repository/version2.bpmn20.xml").Deploy() as IProcessApplicationDeployment;

		IList<IProcessDefinition> processDefinitions = repositoryService.CreateProcessDefinitionQuery().ToList();//.OrderByProcessDefinitionVersion()/*.Asc()*/.ToList();
		// now there are 2 process definitions deployed
		Assert.AreEqual(1, processDefinitions[0].Version);
		Assert.AreEqual(2, processDefinitions[1].Version);

		// old deployment was resumed
		var registration = deployment2.ProcessApplicationRegistration;
            IList<string> deploymentIds = registration.DeploymentIds;
		Assert.AreEqual(2, deploymentIds.Count);
		Assert.AreEqual(ProcessEngine.Name, registration.ProcessEngineName);

		deleteDeployments(deployment1, deployment2);
	  }

	   [Test]   public virtual void testProcessApplicationDeploymentResumePreviousVersionsDifferentKeys()
	  {
		// create initial deployment
		var deployment1 = repositoryService.CreateDeployment(processApplication.Reference).Name("deployment").AddClasspathResource("resources/api/repository/version1.bpmn20.xml").Deploy();

		AssertThatOneProcessIsDeployed();

		// deploy update with changes:
		IProcessApplicationDeployment deployment2 = repositoryService.CreateDeployment(processApplication.Reference)
                .ResumePreviousVersions().Name("deployment")
                .AddClasspathResource("resources/api/oneTaskProcess.bpmn20.xml").Deploy() as IProcessApplicationDeployment;

		IList<IProcessDefinition> processDefinitions = repositoryService.CreateProcessDefinitionQuery().ToList();//.OrderByProcessDefinitionVersion()/*.Asc()*/.ToList();
		// now there are 2 process definitions deployed
		Assert.AreEqual(1, processDefinitions[0].Version);
		Assert.AreEqual(1, processDefinitions[1].Version);

		// and the old deployment was not resumed
		var registration = deployment2.ProcessApplicationRegistration;
            IList<string> deploymentIds = registration.DeploymentIds;
		Assert.AreEqual(1, deploymentIds.Count);
		Assert.AreEqual(deployment2.Id, deploymentIds.GetEnumerator().MoveNext());
		Assert.AreEqual(ProcessEngine.Name, registration.ProcessEngineName);

		deleteDeployments(deployment1, deployment2);
	  }

	   [Test]   public virtual void testProcessApplicationDeploymentNoResume()
	  {
		// create initial deployment
		var deployment1 = repositoryService.CreateDeployment(processApplication.Reference).Name("deployment").AddClasspathResource("resources/api/repository/version1.bpmn20.xml").Deploy();

		AssertThatOneProcessIsDeployed();

		// deploy update with changes:
		var deployment2 = repositoryService.CreateDeployment(processApplication.Reference).Name("deployment")
                .EnableDuplicateFiltering(false).AddClasspathResource("resources/api/repository/version2.bpmn20.xml")
                .Deploy() as IProcessApplicationDeployment;

		IList<IProcessDefinition> processDefinitions = repositoryService.CreateProcessDefinitionQuery().ToList();//.OrderByProcessDefinitionVersion()/*.Asc()*/.ToList();
		// now there are 2 process definitions deployed
		Assert.AreEqual(1, processDefinitions[0].Version);
		Assert.AreEqual(2, processDefinitions[1].Version);

		// old deployment was NOT resumed
		var registration = deployment2.ProcessApplicationRegistration;
            IList<string> deploymentIds = registration.DeploymentIds;
		Assert.AreEqual(1, deploymentIds.Count);
		Assert.AreEqual(ProcessEngine.Name, registration.ProcessEngineName);

		deleteDeployments(deployment1, deployment2);
	  }

	   [Test]   public virtual void testProcessApplicationDeploymentResumePreviousVersionsByDeploymentName()
	  {
		// create initial deployment
		var deployment1 = repositoryService.CreateDeployment(processApplication.Reference).Name("deployment").AddClasspathResource("resources/api/repository/version1.bpmn20.xml").Deploy();

		AssertThatOneProcessIsDeployed();

		// deploy update with changes:
		IProcessApplicationDeployment deployment2 = repositoryService.CreateDeployment(processApplication.Reference)
                //.ResumePreviousVersions()
                .ResumePreviousVersionsBy(ResumePreviousBy.ResumeByDeploymentName).ResumePreviousVersions()
                  .Name("deployment")
                .EnableDuplicateFiltering(false)
                .AddClasspathResource("resources/api/repository/version2.bpmn20.xml").Deploy() as IProcessApplicationDeployment;

		IList<IProcessDefinition> processDefinitions = repositoryService.CreateProcessDefinitionQuery().ToList();//.OrderByProcessDefinitionVersion()/*.Asc()*/.ToList();
		// now there are 2 process definitions deployed
		Assert.AreEqual(1, processDefinitions[0].Version);
		Assert.AreEqual(2, processDefinitions[1].Version);

		// old deployment was resumed
		var registration = deployment2.ProcessApplicationRegistration;
            IList<string> deploymentIds = registration.DeploymentIds;
		Assert.AreEqual(2, deploymentIds.Count);
		Assert.AreEqual(ProcessEngine.Name, registration.ProcessEngineName);

		deleteDeployments(deployment1, deployment2);
	  }

	   [Test]   public virtual void testProcessApplicationDeploymentResumePreviousVersionsByDeploymentNameDeployDifferentProcesses()
	  {
		IBpmnModelInstance process1 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process1").Done();
		IBpmnModelInstance process2 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process2").Done();
		var deployment = repositoryService.CreateDeployment(processApplication.Reference).Name("deployment").AddModelInstance("process1.bpmn", process1).Deploy();

		AssertThatOneProcessIsDeployed();

		IProcessApplicationDeployment deployment2 = repositoryService.CreateDeployment(processApplication.Reference)
                .ResumePreviousVersions().ResumePreviousVersionsBy(ResumePreviousBy.ResumeByDeploymentName).Name("deployment").AddModelInstance("process2.bpmn", process2).Deploy() as IProcessApplicationDeployment;

		IList<IProcessDefinition> processDefinitions = repositoryService.CreateProcessDefinitionQuery().ToList();//.OrderByProcessDefinitionVersion()/*.Asc()*/.ToList();
		// now there are 2 process definitions deployed but both with version 1
		Assert.AreEqual(1, processDefinitions[0].Version);
		Assert.AreEqual(1, processDefinitions[1].Version);

		// old deployment was resumed
		var registration = deployment2.ProcessApplicationRegistration;
            IList<string> deploymentIds = registration.DeploymentIds;
		Assert.AreEqual(2, deploymentIds.Count);
		Assert.AreEqual(ProcessEngine.Name, registration.ProcessEngineName);

		deleteDeployments(deployment, deployment2);
	  }

	   [Test]   public virtual void testProcessApplicationDeploymentResumePreviousVersionsByDeploymentNameNoResume()
	  {
		IBpmnModelInstance process1 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process1").Done();
		var deployment = repositoryService.CreateDeployment(processApplication.Reference).Name("deployment").AddModelInstance("process1.bpmn", process1).Deploy();

		AssertThatOneProcessIsDeployed();

		IProcessApplicationDeployment deployment2 = repositoryService.CreateDeployment(processApplication.Reference).ResumePreviousVersions()
                .ResumePreviousVersionsBy(ResumePreviousBy.ResumeByDeploymentName).Name("anotherDeployment")
                .AddModelInstance("process2.bpmn", process1).Deploy() as IProcessApplicationDeployment;

		IList<IProcessDefinition> processDefinitions = repositoryService.CreateProcessDefinitionQuery()
                //.OrderByProcessDefinitionVersion()/*.Asc()*/
                .ToList();
		// there is a new version of the process
		Assert.AreEqual(1, processDefinitions[0].Version);
		Assert.AreEqual(2, processDefinitions[1].Version);

		// but the old deployment was not resumed
		var registration = deployment2.ProcessApplicationRegistration;
            IList<string> deploymentIds = registration.DeploymentIds;
		Assert.AreEqual(1, deploymentIds.Count);
		Assert.AreEqual(deployment2.Id, deploymentIds.GetEnumerator().MoveNext());
		Assert.AreEqual(ProcessEngine.Name, registration.ProcessEngineName);

		deleteDeployments(deployment, deployment2);
	  }

	   [Test]   public virtual void testPartialChangesResumePreviousVersionByDeploymentName()
	  {
		IBpmnModelInstance model1 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process1").Done();
		IBpmnModelInstance model2 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process2").Done();

		// create initial deployment
		var deployment1 = repositoryService.CreateDeployment(processApplication.Reference).Name("deployment").AddModelInstance("process1.bpmn20.xml", model1).Deploy();

		IProcessApplicationDeployment deployment2 = repositoryService.CreateDeployment(processApplication.Reference)
                .ResumePreviousVersions().ResumePreviousVersionsBy(ResumePreviousBy.ResumeByDeploymentName)
                .Name("deployment").EnableDuplicateFiltering(true).AddModelInstance("process1.bpmn20.xml", model1)
                .AddModelInstance("process2.bpmn20.xml", model2).Deploy() as IProcessApplicationDeployment;

		var registration = deployment2.ProcessApplicationRegistration;
		Assert.AreEqual(2, registration.DeploymentIds.Count);

		deleteDeployments(deployment1, deployment2);
	  }

	   [Test]   public virtual void testDeploymentSourceShouldBeNull()
	  {
		string key = "process";

		IBpmnModelInstance model = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(key).Done();

		 IQueryable<IDeployment> deploymentQuery = repositoryService.CreateDeploymentQuery();

		IDeployment deployment1 = repositoryService.CreateDeployment().Name("first-deployment-without-a-source").AddModelInstance("process.bpmn", model).Deploy();

		Assert.IsNull(deploymentQuery.Where(c=>c.Name == "first-deployment-without-a-source").First().Source);

		IDeployment deployment2 = repositoryService.CreateDeployment(processApplication.Reference).Name("second-deployment-with-a-source").Source(null).AddModelInstance("process.bpmn", model).Deploy();

		Assert.IsNull(deploymentQuery.Where(c=>c.Name == "second-deployment-with-a-source").First().Source);

		deleteDeployments(deployment1, deployment2);
	  }

	   [Test]   public virtual void testDeploymentSourceShouldNotBeNull()
	  {
		string key = "process";

		IBpmnModelInstance model = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(key).Done();

		 IQueryable<IDeployment> deploymentQuery = repositoryService.CreateDeploymentQuery();

		IDeployment deployment1 = repositoryService.CreateDeployment().Name("first-deployment-without-a-source").Source("my-first-deployment-source").AddModelInstance("process.bpmn", model).Deploy();

		Assert.AreEqual("my-first-deployment-source", deploymentQuery.Where(c=>c.Name == "first-deployment-without-a-source").First().Source);

		IDeployment deployment2 = repositoryService.CreateDeployment(processApplication.Reference).Name("second-deployment-with-a-source").Source("my-second-deployment-source").AddModelInstance("process.bpmn", model).Deploy();

		Assert.AreEqual("my-second-deployment-source", deploymentQuery.Where(c=>c.Name == "second-deployment-with-a-source").First().Source);

		deleteDeployments(deployment1, deployment2);
	  }

	   [Test]   public virtual void testDefaultDeploymentSource()
	  {
		string key = "process";

		IBpmnModelInstance model = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(key).Done();

		 IQueryable<IDeployment> deploymentQuery = repositoryService.CreateDeploymentQuery();

		IDeployment deployment = repositoryService.CreateDeployment(processApplication.Reference).Name("first-deployment-with-a-source").AddModelInstance("process.bpmn", model).Deploy();

		Assert.AreEqual(ProcessApplicationDeploymentFields.ProcessApplicationDeploymentSource, deploymentQuery.Where(c=>c.Name == "first-deployment-with-a-source").First().Source);

		deleteDeployments(deployment);
	  }

	   [Test]   public virtual void testOverwriteDeploymentSource()
	  {
		string key = "process";

		IBpmnModelInstance model = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(key).Done();

		 IQueryable<IDeployment> deploymentQuery = repositoryService.CreateDeploymentQuery();

		IDeployment deployment = repositoryService.CreateDeployment(processApplication.Reference).Name("first-deployment-with-a-source").Source("my-source").AddModelInstance("process.bpmn", model).Deploy();

		Assert.AreEqual("my-source", deploymentQuery.Where(c=>c.Name == "first-deployment-with-a-source").First().Source);

		deleteDeployments(deployment);
	  }

	   [Test]   public virtual void testNullDeploymentSourceAwareDuplicateFilter()
	  {
		// given
		string key = "process";
		string name = "my-deployment";

		IBpmnModelInstance model = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(key).Done();

		IQueryable<IProcessDefinition> processDefinitionQuery = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == key);

		 IQueryable<IDeployment> deploymentQuery = repositoryService.CreateDeploymentQuery(c=>c.Name == name);

		// when

		var deployment1 = repositoryService.CreateDeployment(processApplication.Reference).Name(name).Source(null).AddModelInstance("process.bpmn", model).EnableDuplicateFiltering(true).Deploy();

		Assert.AreEqual(1, processDefinitionQuery.Count());
		Assert.AreEqual(1, deploymentQuery.Count());

		var deployment2 = repositoryService.CreateDeployment(processApplication.Reference).Name(name).Source(null).AddModelInstance("process.bpmn", model).EnableDuplicateFiltering(true).Deploy();

		// then

		Assert.AreEqual(1, processDefinitionQuery.Count());
		Assert.AreEqual(1, deploymentQuery.Count());

		deleteDeployments(deployment1, deployment2);
	  }

	   [Test]   public virtual void testNullAndProcessApplicationDeploymentSourceAwareDuplicateFilter()
	  {
		// given

		string key = "process";
		string name = "my-deployment";

		IBpmnModelInstance model = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(key).Done();

		IQueryable<IProcessDefinition> processDefinitionQuery = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == key);

		 IQueryable<IDeployment> deploymentQuery = repositoryService.CreateDeploymentQuery(c=>c.Name == name);

		// when

		var deployment1 = repositoryService.CreateDeployment(processApplication.Reference).Name(name).Source(null).AddModelInstance("process.bpmn", model).EnableDuplicateFiltering(true).Deploy();

		Assert.AreEqual(1, processDefinitionQuery.Count());
		Assert.AreEqual(1, deploymentQuery.Count());

		var deployment2 = repositoryService.CreateDeployment(processApplication.Reference).Name(name).AddModelInstance("process.bpmn", model).EnableDuplicateFiltering(true).Deploy();

		// then

		Assert.AreEqual(1, processDefinitionQuery.Count());
		Assert.AreEqual(1, deploymentQuery.Count());

		deleteDeployments(deployment1, deployment2);
	  }

	   [Test]   public virtual void testProcessApplicationAndNullDeploymentSourceAwareDuplicateFilter()
	  {
		// given

		string key = "process";
		string name = "my-deployment";

		IBpmnModelInstance model = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(key).Done();

		IQueryable<IProcessDefinition> processDefinitionQuery = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == key);

		 IQueryable<IDeployment> deploymentQuery = repositoryService.CreateDeploymentQuery(c=>c.Name == name);

		// when

		var deployment1 = repositoryService.CreateDeployment(processApplication.Reference).Name(name).AddModelInstance("process.bpmn", model).EnableDuplicateFiltering(true).Deploy();

		Assert.AreEqual(1, processDefinitionQuery.Count());
		Assert.AreEqual(1, deploymentQuery.Count());

		var deployment2 = repositoryService.CreateDeployment(processApplication.Reference).Name(name).Source(null).AddModelInstance("process.bpmn", model).EnableDuplicateFiltering(true).Deploy();

		// then

		Assert.AreEqual(1, processDefinitionQuery.Count());
		Assert.AreEqual(1, deploymentQuery.Count());

		deleteDeployments(deployment1, deployment2);
	  }

	   [Test]   public virtual void testProcessApplicationDeploymentSourceAwareDuplicateFilter()
	  {
		// given

		string key = "process";
		string name = "my-deployment";

		IBpmnModelInstance model = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(key).Done();

		IQueryable<IProcessDefinition> processDefinitionQuery = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == key);

		 IQueryable<IDeployment> deploymentQuery = repositoryService.CreateDeploymentQuery(c=>c.Name == name);

		// when

		var deployment1 = repositoryService.CreateDeployment(processApplication.Reference).Name(name).AddModelInstance("process.bpmn", model).EnableDuplicateFiltering(true).Deploy();

		Assert.AreEqual(1, processDefinitionQuery.Count());
		Assert.AreEqual(1, deploymentQuery.Count());

		var deployment2 = repositoryService.CreateDeployment(processApplication.Reference).Name(name).AddModelInstance("process.bpmn", model).EnableDuplicateFiltering(true).Deploy();

		// then

		Assert.AreEqual(1, processDefinitionQuery.Count());
		Assert.AreEqual(1, deploymentQuery.Count());

		deleteDeployments(deployment1, deployment2);
	  }

	   [Test]   public virtual void testSameDeploymentSourceAwareDuplicateFilter()
	  {
		// given

		string key = "process";
		string name = "my-deployment";

		IBpmnModelInstance model = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(key).Done();

		IQueryable<IProcessDefinition> processDefinitionQuery = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == key);

		 IQueryable<IDeployment> deploymentQuery = repositoryService.CreateDeploymentQuery(c=>c.Name == name);

		// when

		var deployment1 = repositoryService.CreateDeployment(processApplication.Reference).Name(name).Source("cockpit").AddModelInstance("process.bpmn", model).EnableDuplicateFiltering(true).Deploy();

		Assert.AreEqual(1, processDefinitionQuery.Count());
		Assert.AreEqual(1, deploymentQuery.Count());

		var deployment2 = repositoryService.CreateDeployment(processApplication.Reference).Name("my-deployment").Source("cockpit").AddModelInstance("process.bpmn", model).EnableDuplicateFiltering(true).Deploy();

		// then

		Assert.AreEqual(1, processDefinitionQuery.Count());
		Assert.AreEqual(1, deploymentQuery.Count());

		deleteDeployments(deployment1, deployment2);
	  }

	   [Test]   public virtual void testDifferentDeploymentSourceShouldDeployNewVersion()
	  {
		// given

		string key = "process";
		string name = "my-deployment";

		IBpmnModelInstance model = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(key).Done();

		IQueryable<IProcessDefinition> processDefinitionQuery = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == key);

		 IQueryable<IDeployment> deploymentQuery = repositoryService.CreateDeploymentQuery(c=>c.Name == name);

		// when

		var deployment1 = repositoryService.CreateDeployment(processApplication.Reference).Name(name).Source("my-source1").AddModelInstance("process.bpmn", model).EnableDuplicateFiltering(true).Deploy();

		Assert.AreEqual(1, processDefinitionQuery.Count());
		Assert.AreEqual(1, deploymentQuery.Count());

		var deployment2 = repositoryService.CreateDeployment(processApplication.Reference).Name(name).Source("my-source2").AddModelInstance("process.bpmn", model).EnableDuplicateFiltering(true).Deploy();

		// then

		Assert.AreEqual(2, processDefinitionQuery.Count());
		Assert.AreEqual(2, deploymentQuery.Count());

		deleteDeployments(deployment1, deployment2);
	  }

	   [Test]   public virtual void testNullAndNotNullDeploymentSourceShouldDeployNewVersion()
	  {
		// given

		string key = "process";
		string name = "my-deployment";

		IBpmnModelInstance model = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(key).Done();

		IQueryable<IProcessDefinition> processDefinitionQuery = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == key);

		 IQueryable<IDeployment> deploymentQuery = repositoryService.CreateDeploymentQuery(c=>c.Name == name);

		// when

		var deployment1 = repositoryService.CreateDeployment(processApplication.Reference).Name(name).Source(null).AddModelInstance("process.bpmn", model).EnableDuplicateFiltering(true).Deploy();

		Assert.AreEqual(1, processDefinitionQuery.Count());
		Assert.AreEqual(1, deploymentQuery.Count());

		var deployment2 = repositoryService.CreateDeployment(processApplication.Reference).Name(name).Source("my-source2").AddModelInstance("process.bpmn", model).EnableDuplicateFiltering(true).Deploy();

		// then

		Assert.AreEqual(2, processDefinitionQuery.Count());
		Assert.AreEqual(2, deploymentQuery.Count());

		deleteDeployments(deployment1, deployment2);
	  }

	   [Test]   public virtual void testNotNullAndNullDeploymentSourceShouldDeployNewVersion()
	  {
		// given

		string key = "process";
		string name = "my-deployment";

		IBpmnModelInstance model = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(key).Done();

		IQueryable<IProcessDefinition> processDefinitionQuery = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == key);

		 IQueryable<IDeployment> deploymentQuery = repositoryService.CreateDeploymentQuery(c=>c.Name == name);

		// when

		var deployment1 = repositoryService.CreateDeployment(processApplication.Reference).Name(name).Source("my-source1").AddModelInstance("process.bpmn", model).EnableDuplicateFiltering(true).Deploy();

		Assert.AreEqual(1, processDefinitionQuery.Count());
		Assert.AreEqual(1, deploymentQuery.Count());

		var deployment2 = repositoryService.CreateDeployment(processApplication.Reference).Name(name).Source(null).AddModelInstance("process.bpmn", model).EnableDuplicateFiltering(true).Deploy();

		// then

		Assert.AreEqual(2, processDefinitionQuery.Count());
		Assert.AreEqual(2, deploymentQuery.Count());

		deleteDeployments(deployment1, deployment2);
	  }

	   [Test]   public virtual void testUnregisterProcessApplicationOnDeploymentDeletion()
	  {
		// given a deployment with a process application registration
		IDeployment deployment = repositoryService.CreateDeployment().AddModelInstance("process.bpmn", ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("foo").Done()).Deploy();

		// and a process application registration
		managementService.RegisterProcessApplication(deployment.Id, processApplication.Reference);

		// when deleting the deploymen
		repositoryService.DeleteDeployment(deployment.Id, true);

		// then the registration is removed
		Assert.IsNull(managementService.GetProcessApplicationForDeployment(deployment.Id));



	  }

	  /// <summary>
	  /// Deletes the deployments cascading.
	  /// </summary>
	  private void deleteDeployments(params IDeployment[] deployments)
	  {
		foreach (IDeployment deployment in deployments)
		{
		  repositoryService.DeleteDeployment(deployment.Id, true);
		}
	  }

	  /// <summary>
	  /// Creates a process definition query and checks that only one process with version 1 is present.
	  /// </summary>
	  private void AssertThatOneProcessIsDeployed()
	  {
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		Assert.That(processDefinition!=null);
		Assert.AreEqual(1, processDefinition.Version);
	  }

	}

}