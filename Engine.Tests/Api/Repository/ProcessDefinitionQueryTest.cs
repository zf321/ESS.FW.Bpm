//using System.Collections.Generic;
//using System.Linq;
//using ESS.FW.Bpm.Engine.Repository;
//using ESS.FW.Bpm.Engine.Runtime;
//using NUnit.Framework;


//namespace ESS.FW.Bpm.Engine.Tests.Api.Repository
//{
    


//	/// <summary>
//	/// 
//	/// </summary>
//	public class ProcessDefinitionQueryTest : AbstractDefinitionQueryTest
//	{

//	  private string deploymentThreeId;

//	  protected internal override string ResourceOnePath
//	  {
//		  get
//		  {
//			return "resources/repository/one.bpmn20.xml";
//		  }
//	  }

//	  protected internal override string ResourceTwoPath
//	  {
//		  get
//		  {
//			return "resources/repository/two.bpmn20.xml";
//		  }
//	  }

//	  protected internal virtual string ResourceThreePath
//	  {
//		  get
//		  {
//			return "resources/repository/three_.bpmn20.xml";
//		  }
//	  }

//        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//        //ORIGINAL LINE: @Override protected void setUp() throws Exception
//        [SetUp]
//        protected internal void setUp()
//	  {
//		deploymentThreeId = repositoryService.CreateDeployment().Name("thirdDeployment").AddClasspathResource(ResourceThreePath).Deploy().Id;
//		base.setUp();
//	  }

//        [TearDown]
//	  protected internal void tearDown()
//	  {
//		base.TearDown();
//		repositoryService.DeleteDeployment(deploymentThreeId, true);
//	  }

//	   [Test]   public virtual void testProcessDefinitionProperties()
//	  {
//		IList<IProcessDefinition> processDefinitions = repositoryService.CreateProcessDefinitionQuery()/*.OrderByProcessDefinitionName()*//*.Asc()*/.OrderByProcessDefinitionVersion()/*.Asc()*/.OrderByProcessDefinitionCategory()/*.Asc()*/.ToList();

//		IProcessDefinition processDefinition = processDefinitions[0];
//		Assert.AreEqual("one", processDefinition.Key);
//		Assert.AreEqual("One", processDefinition.Name);
//		Assert.AreEqual("Desc one", processDefinition.Description);
//		Assert.True(processDefinition.Id.StartsWith("one:1"));
//		Assert.AreEqual("Examples", processDefinition.Category);

//		processDefinition = processDefinitions[1];
//		Assert.AreEqual("one", processDefinition.Key);
//		Assert.AreEqual("One", processDefinition.Name);
//		Assert.AreEqual("Desc one", processDefinition.Description);
//		Assert.True(processDefinition.Id.StartsWith("one:2"));
//		Assert.AreEqual("Examples", processDefinition.Category);

//		processDefinition = processDefinitions[2];
//		Assert.AreEqual("two", processDefinition.Key);
//		Assert.AreEqual("Two", processDefinition.Name);
//		Assert.IsNull(processDefinition.Description);
//		Assert.True(processDefinition.Id.StartsWith("two:1"));
//		Assert.AreEqual("Examples2", processDefinition.Category);
//	  }

//	   [Test]   public virtual void testQueryByDeploymentId()
//	  {
//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery(c=> c.DeploymentId == deploymentOneId);
//		//verifyQueryResults(query, 2);
//	  }

//	   [Test]   public virtual void testQueryByInvalidDeploymentId()
//	  {
//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery(c=> c.DeploymentId == "invalid");
//		//verifyQueryResults(query, 0);

//		try
//		{
//		  repositoryService.CreateProcessDefinitionQuery(c=> c.DeploymentId == null);
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		  // Expected Exception
//		}
//	  }

//	   [Test]   public virtual void testQueryByName()
//	  {
//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery().ProcessDefinitionName("Two");
//		//verifyQueryResults(query, 1);

//		query = repositoryService.CreateProcessDefinitionQuery().ProcessDefinitionName("One");
//		//verifyQueryResults(query, 2);
//	  }

//	   [Test]   public virtual void testQueryByInvalidName()
//	  {
//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery().ProcessDefinitionName("invalid");
//		//verifyQueryResults(query, 0);

//		try
//		{
//		  repositoryService.CreateProcessDefinitionQuery().ProcessDefinitionName(null);
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		  // Expected Exception
//		}
//	  }

//	   [Test]   public virtual void testQueryByNameLike()
//	  {
//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery().ProcessDefinitionNameLike("%w%");
//		//verifyQueryResults(query, 1);
//		query = query.ProcessDefinitionNameLike("%z\\_%");
//		//verifyQueryResults(query, 1);
//	  }

//	   [Test]   public virtual void testQueryByInvalidNameLike()
//	  {
//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery().ProcessDefinitionNameLike("%invalid%");
//		//verifyQueryResults(query, 0);
//	  }

//	   [Test]   public virtual void testQueryByKey()
//	  {
//		// process one
//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "one");
//		//verifyQueryResults(query, 2);

//		// process two
//		query = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "two");
//		//verifyQueryResults(query, 1);
//	  }

//	   [Test]   public virtual void testQueryByInvalidKey()
//	  {
//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "invalid");
//		//verifyQueryResults(query, 0);

//		try
//		{
//		  repositoryService.CreateProcessDefinitionQuery(c=>c.Key == null);
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		  // Expected Exception
//		}
//	  }

//	   [Test]   public virtual void testQueryByKeyLike()
//	  {
//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery().ProcessDefinitionKeyLike("%o%");
//		//verifyQueryResults(query, 3);
//		query = query.ProcessDefinitionKeyLike("%z\\_%");
//		//verifyQueryResults(query, 1);
//	  }

//	   [Test]   public virtual void testQueryByInvalidKeyLike()
//	  {
//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery().ProcessDefinitionKeyLike("%invalid%");
//		//verifyQueryResults(query, 0);

//		try
//		{
//		  repositoryService.CreateProcessDefinitionQuery().ProcessDefinitionKeyLike(null);
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		  // Expected Exception
//		}
//	  }

//	   [Test]   public virtual void testQueryByResourceNameLike()
//	  {
//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery().ProcessDefinitionResourceNameLike("%ee\\_%");
//		//verifyQueryResults(query, 1);
//	  }

//	   [Test]   public virtual void testQueryByInvalidResourceNameLike()
//	  {
//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery().ProcessDefinitionResourceNameLike("%invalid%");
//		//verifyQueryResults(query, 0);

//		try
//		{
//		  repositoryService.CreateProcessDefinitionQuery().ProcessDefinitionResourceNameLike(null);
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		  // Expected Exception
//		}
//	  }

//	   [Test]   public virtual void testQueryByCategory()
//	  {
//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery().ProcessDefinitionCategory("Examples");
//		//verifyQueryResults(query, 2);
//	  }

//	   [Test]   public virtual void testQueryByCategoryLike()
//	  {
//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery().ProcessDefinitionCategoryLike("%Example%");
//		//verifyQueryResults(query, 3);

//		query = repositoryService.CreateProcessDefinitionQuery().ProcessDefinitionCategoryLike("%amples2");
//		//verifyQueryResults(query, 1);

//		query = repositoryService.CreateProcessDefinitionQuery().ProcessDefinitionCategoryLike("%z\\_%");
//		//verifyQueryResults(query, 1);
//	  }

//	   [Test]   public virtual void testQueryByVersion()
//	  {
//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery().ProcessDefinitionVersion(2);
//		//verifyQueryResults(query, 1);

//		query = repositoryService.CreateProcessDefinitionQuery().ProcessDefinitionVersion(1);
//		//verifyQueryResults(query, 3);
//	  }

//	   [Test]   public virtual void testQueryByInvalidVersion()
//	  {
//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery().ProcessDefinitionVersion(3);
//		//verifyQueryResults(query, 0);

//		try
//		{
//		    repositoryService.CreateProcessDefinitionQuery()
//		        .ProcessDefinitionVersion(-1)
//		        .ToList();
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		  // Expected Exception
//		}

//		try
//		{
//		  repositoryService.CreateProcessDefinitionQuery().ProcessDefinitionVersion(null).ToList();
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		  // Expected Exception
//		}
//	  }

//	   [Test]   public virtual void testQueryByKeyAndVersion()
//	  {
//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "one").ProcessDefinitionVersion(1);
//		//verifyQueryResults(query, 1);

//		query = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "one").ProcessDefinitionVersion(2);
//		//verifyQueryResults(query, 1);

//		query = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "one").ProcessDefinitionVersion(3);
//		//verifyQueryResults(query, 0);
//	  }

//	   [Test]   public virtual void testQueryByLatest()
//	  {
//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery()/*.LatestVersion()*/;
//		//verifyQueryResults(query, 3);

//		query = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "one")/*.LatestVersion()*/;
//		//verifyQueryResults(query, 1);

//		query = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "two")/*.LatestVersion()*/;
//		//verifyQueryResults(query, 1);
//	  }

//	   [Test]   public virtual void testInvalidUsageOfLatest()
//	  {
//		try
//		{
//		  repositoryService.CreateProcessDefinitionQuery(c=>c.ProcessDefinitionId =="test")/*.LatestVersion()*/.ToList();
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		  // Expected Exception
//		}

//		try
//		{
//		  repositoryService.CreateProcessDefinitionQuery().ProcessDefinitionVersion(1)/*.LatestVersion()*/.ToList();
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		  // Expected Exception
//		}

//		try
//		{
//		  repositoryService.CreateProcessDefinitionQuery(c=> c.DeploymentId == "test")/*.LatestVersion()*/.ToList();
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		  // Expected Exception
//		}
//	  }

//	   [Test]   public virtual void testQuerySorting()
//	  {

//		// asc

//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery()/*.OrderByProcessDefinitionId()*//*.Asc()*/;
//		//verifyQueryResults(query, 4);

//		query = repositoryService.CreateProcessDefinitionQuery().OrderByDeploymentId()/*.Asc()*/;
//		//verifyQueryResults(query, 4);

//		query = repositoryService.CreateProcessDefinitionQuery()//.OrderByProcessDefinitionKey()/*.Asc()*/;
//		//verifyQueryResults(query, 4);

//		query = repositoryService.CreateProcessDefinitionQuery().OrderByProcessDefinitionVersion()/*.Asc()*/;
//		//verifyQueryResults(query, 4);

//		// Desc

//		query = repositoryService.CreateProcessDefinitionQuery()/*.OrderByProcessDefinitionId()*//*.Desc()*/;
//		//verifyQueryResults(query, 4);

//		query = repositoryService.CreateProcessDefinitionQuery().OrderByDeploymentId()/*.Desc()*/;
//		//verifyQueryResults(query, 4);

//		query = repositoryService.CreateProcessDefinitionQuery()//.OrderByProcessDefinitionKey()/*.Desc()*/;
//		//verifyQueryResults(query, 4);

//		query = repositoryService.CreateProcessDefinitionQuery().OrderByProcessDefinitionVersion()/*.Desc()*/;
//		//verifyQueryResults(query, 4);

//		// Typical use case
//		query = repositoryService.CreateProcessDefinitionQuery()//.OrderByProcessDefinitionKey()/*.Asc()*/.OrderByProcessDefinitionVersion()/*.Desc()*/;
//		IList<IProcessDefinition> processDefinitions = query.ToList();
//		Assert.AreEqual(4, processDefinitions.Count);

//		Assert.AreEqual("one", processDefinitions[0].Key);
//		Assert.AreEqual(2, processDefinitions[0].Version);
//		Assert.AreEqual("one", processDefinitions[1].Key);
//		Assert.AreEqual(1, processDefinitions[1].Version);
//		Assert.AreEqual("two", processDefinitions[2].Key);
//		Assert.AreEqual(1, processDefinitions[2].Version);
//	  }

//	   [Test]   public virtual void testQueryByMessageSubscription()
//	  {
//		IDeployment deployment = repositoryService.CreateDeployment().AddClasspathResource("resources/api/repository/processWithNewBookingMessage.bpmn20.xml").AddClasspathResource("resources/api/repository/processWithNewInvoiceMessage.bpmn20.xml").Deploy();

//	      Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery()
//	          .Count());/*.MessageEventSubscriptionName("newInvoiceMessage")*/;

//		Assert.AreEqual(1,repositoryService.CreateProcessDefinitionQuery()
//              .Count());//.MessageEventSubscriptionName("newBookingMessage").Count());

//            Assert.AreEqual(0,repositoryService.CreateProcessDefinitionQuery()
//              .Count());//.MessageEventSubscriptionName("bogus").Count());

//            repositoryService.DeleteDeployment(deployment.Id);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Deployment(resources={"resources/api/repository/failingProcessCreateOneIncident.bpmn20.xml"}) public void testQueryByIncidentId()
//	   [Test]   public virtual void testQueryByIncidentId()
//	  {
//		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "failingProcess").Count());

//		IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("failingProcess");

//		ExecuteAvailableJobs();

//		IList<IIncident> incidentList = runtimeService.CreateIncidentQuery().ToList();
//		Assert.AreEqual(1, incidentList.Count);

//		IIncident incident = runtimeService.CreateIncidentQuery(c=>c.ProcessInstanceId == processInstance.Id).First();

//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery().IncidentId(incident.Id);

//		//verifyQueryResults(query, 1);
//	  }

//	   [Test]   public virtual void testQueryByInvalidIncidentId()
//	  {
//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

//		//verifyQueryResults(query.IncidentId("invalid"), 0);

//		try
//		{
//		  query.IncidentId(null);
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		  // Expected Exception
//		}
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Deployment(resources={"resources/api/repository/failingProcessCreateOneIncident.bpmn20.xml"}) public void testQueryByIncidentType()
//	   [Test]   public virtual void testQueryByIncidentType()
//	  {
//		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "failingProcess").Count());

//		IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("failingProcess");

//		ExecuteAvailableJobs();

//		IList<IIncident> incidentList = runtimeService.CreateIncidentQuery().ToList();
//		Assert.AreEqual(1, incidentList.Count);

//		IIncident incident = runtimeService.CreateIncidentQuery(c=>c.ProcessInstanceId == processInstance.Id).First();

//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery().IncidentType(incident.IncidentType);

//		//verifyQueryResults(query, 1);
//	  }

//	   [Test]   public virtual void testQueryByInvalidIncidentType()
//	  {
//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

//		//verifyQueryResults(query.IncidentType("invalid"), 0);

//		try
//		{
//		  query.IncidentType(null);
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		  // Expected Exception
//		}
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Deployment(resources={"resources/api/repository/failingProcessCreateOneIncident.bpmn20.xml"}) public void testQueryByIncidentMessage()
//	   [Test]   public virtual void testQueryByIncidentMessage()
//	  {
//		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "failingProcess").Count());

//		IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("failingProcess");

//		ExecuteAvailableJobs();

//		IList<IIncident> incidentList = runtimeService.CreateIncidentQuery().ToList();
//		Assert.AreEqual(1, incidentList.Count);

//		IIncident incident = runtimeService.CreateIncidentQuery(c=>c.ProcessInstanceId == processInstance.Id).First();

//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery().IncidentMessage(incident.IncidentMessage);

//		//verifyQueryResults(query, 1);
//	  }

//	   [Test]   public virtual void testQueryByInvalidIncidentMessage()
//	  {
//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

//		//verifyQueryResults(query.IncidentMessage("invalid"), 0);

//		try
//		{
//		  query.IncidentMessage(null);
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		  // Expected Exception
//		}
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Deployment(resources={"resources/api/repository/failingProcessCreateOneIncident.bpmn20.xml"}) public void testQueryByIncidentMessageLike()
//	   [Test]   public virtual void testQueryByIncidentMessageLike()
//	  {
//		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "failingProcess").Count());

//		runtimeService.StartProcessInstanceByKey("failingProcess");

//		ExecuteAvailableJobs();

//		IList<IIncident> incidentList = runtimeService.CreateIncidentQuery().ToList();
//		Assert.AreEqual(1, incidentList.Count);

//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery(c=>c.Incidents.Any(d=>d.IncidentMessage.Contains("expected")));

//		//verifyQueryResults(query, 1);

//		query = repositoryService.CreateProcessDefinitionQuery(c=>c.Incidents.Any(d=>d.IncidentMessage.Contains("\\_expected%")));

//		//verifyQueryResults(query, 1);
//	  }

//	   [Test]   public virtual void testQueryByInvalidIncidentMessageLike()
//	  {
//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

//		//verifyQueryResults(query.IncidentMessageLike("invalid"), 0);

//		try
//		{
//		  query.IncidentMessageLike(null);
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		  // Expected Exception
//		}
//	  }

//	   [Test]   public virtual void testQueryByProcessDefinitionIds()
//	  {

//		// empty list
//		Assert.True(repositoryService.CreateProcessDefinitionQuery().ProcessDefinitionIdIn("a", "b").Count()==0);


//		// collect all ids
//		IList<IProcessDefinition> list = repositoryService.CreateProcessDefinitionQuery().ToList();
//		string[] ids = new string[list.Count];
//		for (int i = 0; i < ids.Length; i++)
//		{
//		  ids[i] = list[i].Id;
//		}

//		IList<IProcessDefinition> idInList = repositoryService.CreateProcessDefinitionQuery().ProcessDefinitionIdIn(ids).ToList();
//		foreach (IProcessDefinition processDefinition in idInList)
//		{
//		  bool found = false;
//		  foreach (IProcessDefinition otherProcessDefinition in list)
//		  {
//			if (otherProcessDefinition.Id.Equals(processDefinition.Id))
//			{
//			  found = true;
//			  break;
//			}
//		  }
//		  if (!found)
//		  {
//			Assert.Fail("Expected to find process definition " + processDefinition);
//		  }
//		}

//		Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery(c=>c.ProcessDefinitionId =="dummyId").ProcessDefinitionIdIn(ids).Count());
//	  }

//	   [Test]   public virtual void testQueryByLatestAndName()
//	  {
//		string firstDeployment = repositoryService.CreateDeployment().AddClasspathResource("resources/api/repository/first-process.bpmn20.xml").Deploy().Id;

//		string secondDeployment = repositoryService.CreateDeployment().AddClasspathResource("resources/api/repository/first-process.bpmn20.xml").Deploy().Id;

//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

//		query.ProcessDefinitionName("First Test Process")/*.LatestVersion()*/;

//		//verifyQueryResults(query, 1);

//		IProcessDefinition result = query.First();

//		Assert.AreEqual("First Test Process", result.Name);
//		Assert.AreEqual(2, result.Version);

//		repositoryService.DeleteDeployment(firstDeployment, true);
//		repositoryService.DeleteDeployment(secondDeployment, true);

//	  }

//	   [Test]   public virtual void testQueryByLatestAndName_NotFound()
//	  {
//		string firstDeployment = repositoryService.CreateDeployment().AddClasspathResource("resources/api/repository/first-process.bpmn20.xml").Deploy().Id;

//		string secondDeployment = repositoryService.CreateDeployment().AddClasspathResource("resources/api/repository/second-process.bpmn20.xml").Deploy().Id;

//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

//		query.ProcessDefinitionName("First Test Process")/*.LatestVersion()*/;

//		//verifyQueryResults(query, 0);

//		repositoryService.DeleteDeployment(firstDeployment, true);
//		repositoryService.DeleteDeployment(secondDeployment, true);

//	  }

//	   [Test]   public virtual void testQueryByLatestAndNameLike()
//	  {
//		string firstDeployment = repositoryService.CreateDeployment().AddClasspathResource("resources/api/repository/first-process.bpmn20.xml").Deploy().Id;

//		string secondDeployment = repositoryService.CreateDeployment().AddClasspathResource("resources/api/repository/second-process.bpmn20.xml").Deploy().Id;

//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

//		query.ProcessDefinitionNameLike("%Test Process")/*.LatestVersion()*/;

//		//verifyQueryResults(query, 1);

//		IProcessDefinition result = query.First();

//		Assert.AreEqual("Second Test Process", result.Name);
//		Assert.AreEqual(2, result.Version);

//		query.ProcessDefinitionNameLike("%Test%")/*.LatestVersion()*/;

//		//verifyQueryResults(query, 1);

//		result = query.First();

//		Assert.AreEqual("Second Test Process", result.Name);
//		Assert.AreEqual(2, result.Version);

//		query.ProcessDefinitionNameLike("Second%")/*.LatestVersion()*/;

//		result = query.First();

//		Assert.AreEqual("Second Test Process", result.Name);
//		Assert.AreEqual(2, result.Version);

//		repositoryService.DeleteDeployment(firstDeployment, true);
//		repositoryService.DeleteDeployment(secondDeployment, true);
//	  }

//	   [Test]   public virtual void testQueryByLatestAndNameLike_NotFound()
//	  {
//		string firstDeployment = repositoryService.CreateDeployment().AddClasspathResource("resources/api/repository/first-process.bpmn20.xml").Deploy().Id;

//		string secondDeployment = repositoryService.CreateDeployment().AddClasspathResource("resources/api/repository/second-process.bpmn20.xml").Deploy().Id;

//		IQueryable<IProcessDefinition> query = repositoryService.CreateProcessDefinitionQuery();

//		query.ProcessDefinitionNameLike("First%")/*.LatestVersion()*/;

//		//verifyQueryResults(query, 0);

//		repositoryService.DeleteDeployment(firstDeployment, true);
//		repositoryService.DeleteDeployment(secondDeployment, true);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Deployment(resources={"resources/api/repository/failingProcessCreateOneIncident.bpmn20.xml"}) public void testQueryByVersionTag()
//	   [Test]   public virtual void testQueryByVersionTag()
//	  {
//		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery().VersionTag("ver_tag_2").Count());
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Deployment(resources={"resources/api/repository/failingProcessCreateOneIncident.bpmn20.xml"}) public void testQueryByVersionTagLike()
//	   [Test]   public virtual void testQueryByVersionTagLike()
//	  {
//		Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery().VersionTagLike("ver\\_tag\\_%").Count());
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Deployment(resources={ "resources/api/repository/failingProcessCreateOneIncident.bpmn20.xml", "resources/api/repository/VersionTagTest.TestParsingVersionTag.bpmn20.xml" }) public void testQueryOrderByVersionTag()
//	   [Test]   public virtual void testQueryOrderByVersionTag()
//	  {
//		IList<IProcessDefinition> processDefinitionList = repositoryService.CreateProcessDefinitionQuery().VersionTagLike("ver%tag%").OrderByVersionTag()/*.Asc()*/.ToList();

//		Assert.AreEqual("ver_tag_2", processDefinitionList[1].VersionTag);
//	  }


//	}

//}