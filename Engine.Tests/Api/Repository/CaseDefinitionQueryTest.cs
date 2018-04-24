//using System.Collections.Generic;
//using System.Linq;
//using ESS.FW.Bpm.Engine.exception;
//using ESS.FW.Bpm.Engine.Query;
//using ESS.FW.Bpm.Engine.Repository;
//using NUnit.Framework;


//namespace ESS.FW.Bpm.Engine.Tests.Api.Repository
//{

    

//	/// <summary>
//	/// 
//	/// 
//	/// </summary>
//	public class CaseDefinitionQueryTest : AbstractDefinitionQueryTest
//	{

//	  private string deploymentThreeId;

//	  protected internal override string ResourceOnePath
//	  {
//		  get
//		  {
//			return "resources/repository/one.cmmn";
//		  }
//	  }

//	  protected internal override string ResourceTwoPath
//	  {
//		  get
//		  {
//			return "resources/repository/two.cmmn";
//		  }
//	  }

//	  protected internal virtual string ResourceThreePath
//	  {
//		  get
//		  {
//			return "resources/api/repository/three_.cmmn";
//		  }
//	  }


//      [SetUp]
//	  protected internal void setUp()
//	  {
//		deploymentThreeId = repositoryService.CreateDeployment().Name("thirdDeployment").AddClasspathResource(ResourceThreePath).Deploy().Id;
//		base.setUp();
//	  }


//[TearDown]
//	  protected internal void tearDown()
//	  {
//		base.TearDown();
//		repositoryService.DeleteDeployment(deploymentThreeId, true);
//	  }

//	   [Test]   public virtual void testCaseDefinitionProperties()
//	  {
//		IList<ICaseDefinition> caseDefinitions = repositoryService.CreateCaseDefinitionQuery().OrderByCaseDefinitionName()/*.Asc()*/.OrderByCaseDefinitionVersion()/*.Asc()*/.OrderByCaseDefinitionCategory()/*.Asc()*/.ToList();

//		ICaseDefinition caseDefinition = caseDefinitions[0];
//		Assert.AreEqual("one", caseDefinition.Key);
//		Assert.AreEqual("One", caseDefinition.Name);
//		Assert.True(caseDefinition.Id.StartsWith("one:1"));
//		Assert.AreEqual("Examples", caseDefinition.Category);
//		Assert.AreEqual(1, caseDefinition.Version);
//		Assert.AreEqual("resources/repository/one.cmmn", caseDefinition.ResourceName);
//		Assert.AreEqual(deploymentOneId, caseDefinition.DeploymentId);

//		caseDefinition = caseDefinitions[1];
//		Assert.AreEqual("one", caseDefinition.Key);
//		Assert.AreEqual("One", caseDefinition.Name);
//		Assert.True(caseDefinition.Id.StartsWith("one:2"));
//		Assert.AreEqual("Examples", caseDefinition.Category);
//		Assert.AreEqual(2, caseDefinition.Version);
//		Assert.AreEqual("resources/repository/one.cmmn", caseDefinition.ResourceName);
//		Assert.AreEqual(deploymentTwoId, caseDefinition.DeploymentId);

//		caseDefinition = caseDefinitions[2];
//		Assert.AreEqual("two", caseDefinition.Key);
//		Assert.AreEqual("Two", caseDefinition.Name);
//		Assert.True(caseDefinition.Id.StartsWith("two:1"));
//		Assert.AreEqual("Examples2", caseDefinition.Category);
//		Assert.AreEqual(1, caseDefinition.Version);
//		Assert.AreEqual("resources/repository/two.cmmn", caseDefinition.ResourceName);
//		Assert.AreEqual(deploymentOneId, caseDefinition.DeploymentId);
//	  }

//	   [Test]   public virtual void testQueryByCaseDefinitionIds()
//	  {
//		// empty list
//		Assert.True(repositoryService.CreateCaseDefinitionQuery().CaseDefinitionIdIn("a", "b").Count()==0);

//		// collect all ids
//	      IList<ICaseDefinition> caseDefinitions = repositoryService.CreateCaseDefinitionQuery()
//	          .ToList();
//		// no point of the test if the caseDefinitions is empty
//		Assert.IsFalse(caseDefinitions.Count == 0);
//		IList<string> ids = new List<string>();
//		foreach (ICaseDefinition caseDefinition in caseDefinitions)
//		{
//		  ids.Add(caseDefinition.Id);
//		}

//		caseDefinitions = repositoryService.CreateCaseDefinitionQuery().CaseDefinitionIdIn(ids.ToArray()).ToList();

//		Assert.AreEqual(ids.Count, caseDefinitions.Count);
//		foreach (ICaseDefinition caseDefinition in caseDefinitions)
//		{
//		  if (!ids.Contains(caseDefinition.Id))
//		  {
//			Assert.Fail("Expected to find case definition " + caseDefinition);
//		  }
//		}

//		Assert.AreEqual(0, repositoryService.CreateCaseDefinitionQuery().CaseDefinitionIdIn(ids.ToArray()).CaseDefinitionId("nonExistent").Count());
//	  }

//	   [Test]   public virtual void testQueryByDeploymentId()
//	  {
//		IQueryable<ICaseDefinition> query = repositoryService.CreateCaseDefinitionQuery();

//		query.DeploymentId(deploymentOneId);

//		//verifyQueryResults((IQueryable<IProcessDefinition>)query, 2);
//	  }

//	   [Test]   public virtual void testQueryByInvalidDeploymentId()
//	  {
//		IQueryable<ICaseDefinition> query = repositoryService.CreateCaseDefinitionQuery();

//	   query.DeploymentId("invalid");

//		//verifyQueryResults(query, 0);

//		try
//		{
//		  query.DeploymentId(null);
//		  Assert.Fail();
//		}
//		catch (NotValidException)
//		{
//		  // Expected exception
//		}
//	  }

//	   [Test]   public virtual void testQueryByName()
//	  {
//		IQueryable<ICaseDefinition> query = repositoryService.CreateCaseDefinitionQuery();

//		query.CaseDefinitionName("Two");

//		//verifyQueryResults(query, 1);

//		query.CaseDefinitionName("One");

//		//verifyQueryResults(query, 2);
//	  }

//	   [Test]   public virtual void testQueryByInvalidName()
//	  {
//		IQueryable<ICaseDefinition> query = repositoryService.CreateCaseDefinitionQuery();

//		query.CaseDefinitionName("invalid");

//		//verifyQueryResults(query, 0);

//		try
//		{
//		  query.CaseDefinitionName(null);
//		  Assert.Fail();
//		}
//		catch (NotValidException)
//		{
//		  // Expected exception
//		}
//	  }

//	   [Test]   public virtual void testQueryByNameLike()
//	  {
//		IQueryable<ICaseDefinition> query = repositoryService.CreateCaseDefinitionQuery();

//		query.CaseDefinitionNameLike("%w%");

//		//verifyQueryResults(query, 1);

//		query.CaseDefinitionNameLike("%z\\_");

//		//verifyQueryResults(query, 1);
//	  }

//	   [Test]   public virtual void testQueryByInvalidNameLike()
//	  {
//		IQueryable<ICaseDefinition> query = repositoryService.CreateCaseDefinitionQuery();

//		query.CaseDefinitionNameLike("%invalid%");

//		//verifyQueryResults(query, 0);

//		try
//		{
//		  query.CaseDefinitionNameLike(null);
//		  Assert.Fail();
//		}
//		catch (NotValidException)
//		{
//		  // Expected exception
//		}
//	  }

//	   [Test]   public virtual void testQueryByResourceNameLike()
//	  {
//		IQueryable<ICaseDefinition> query = repositoryService.CreateCaseDefinitionQuery();

//		query.CaseDefinitionResourceNameLike("%ree%");

//		//verifyQueryResults(query, 1);

//		query.CaseDefinitionResourceNameLike("%e\\_%");

//		//verifyQueryResults(query, 1);
//	  }

//	   [Test]   public virtual void testQueryByInvalidResourceNameLike()
//	  {
//		IQueryable<ICaseDefinition> query = repositoryService.CreateCaseDefinitionQuery();

//		query.CaseDefinitionResourceNameLike("%invalid%");

//		//verifyQueryResults(query, 0);

//		try
//		{
//		  query.CaseDefinitionNameLike(null);
//		  Assert.Fail();
//		}
//		catch (NotValidException)
//		{
//		  // Expected exception
//		}
//	  }

//	   [Test]   public virtual void testQueryByKey()
//	  {
//		IQueryable<ICaseDefinition> query = repositoryService.CreateCaseDefinitionQuery();

//		// case one
//		query.CaseDefinitionKey("one");

//		//verifyQueryResults(query, 2);

//		// case two
//		query.CaseDefinitionKey("two");

//		//verifyQueryResults(query, 1);
//	  }

//	   [Test]   public virtual void testQueryByInvalidKey()
//	  {
//		IQueryable<ICaseDefinition> query = repositoryService.CreateCaseDefinitionQuery();

//		query.CaseDefinitionKey("invalid");

//		//verifyQueryResults(query, 0);

//		try
//		{
//		  query.CaseDefinitionKey(null);
//		  Assert.Fail();
//		}
//		catch (NotValidException)
//		{
//		  // Expected exception
//		}
//	  }

//	   [Test]   public virtual void testQueryByKeyLike()
//	  {
//		IQueryable<ICaseDefinition> query = repositoryService.CreateCaseDefinitionQuery();

//		query.CaseDefinitionKeyLike("%o%");

//		//verifyQueryResults(query, 3);

//		query.CaseDefinitionKeyLike("%z\\_");

//		//verifyQueryResults(query, 1);
//	  }

//	   [Test]   public virtual void testQueryByInvalidKeyLike()
//	  {
//		IQueryable<ICaseDefinition> query = repositoryService.CreateCaseDefinitionQuery();

//		query.CaseDefinitionKeyLike("%invalid%");

//		//verifyQueryResults(query, 0);

//		try
//		{
//		  query.CaseDefinitionKeyLike(null);
//		  Assert.Fail();
//		}
//		catch (NotValidException)
//		{
//		  // Expected exception
//		}
//	  }

//	   [Test]   public virtual void testQueryByCategory()
//	  {
//		IQueryable<ICaseDefinition> query = repositoryService.CreateCaseDefinitionQuery();

//		query.CaseDefinitionCategory("Examples");

//		//verifyQueryResults(query, 2);
//	  }

//	   [Test]   public virtual void testQueryByInvalidCategory()
//	  {
//		IQueryable<ICaseDefinition> query = repositoryService.CreateCaseDefinitionQuery();

//		query.CaseDefinitionCategory("invalid");

//		//verifyQueryResults(query, 0);

//		try
//		{
//		  query.CaseDefinitionCategory(null);
//		  Assert.Fail();
//		}
//		catch (NotValidException)
//		{
//		  // Expected exception
//		}
//	  }

//	   [Test]   public virtual void testQueryByCategoryLike()
//	  {
//		IQueryable<ICaseDefinition> query = repositoryService.CreateCaseDefinitionQuery();

//		query.CaseDefinitionCategoryLike("%Example%");

//		//verifyQueryResults(query, 3);

//		query.CaseDefinitionCategoryLike("%amples2");

//		//verifyQueryResults(query, 1);

//		query.CaseDefinitionCategoryLike("%z\\_");

//		//verifyQueryResults(query, 1);

//	  }

//	   [Test]   public virtual void testQueryByInvalidCategoryLike()
//	  {
//		IQueryable<ICaseDefinition> query = repositoryService.CreateCaseDefinitionQuery();

//		query.CaseDefinitionCategoryLike("invalid");

//		//verifyQueryResults(query, 0);

//		try
//		{
//		  query.CaseDefinitionCategoryLike(null);
//		  Assert.Fail();
//		}
//		catch (NotValidException)
//		{
//		  // Expected exception
//		}
//	  }

//	   [Test]   public virtual void testQueryByVersion()
//	  {
//		IQueryable<ICaseDefinition> query = repositoryService.CreateCaseDefinitionQuery();

//		query.CaseDefinitionVersion(2);

//		//verifyQueryResults(query, 1);

//		query.CaseDefinitionVersion(1);

//		//verifyQueryResults(query, 3);
//	  }

//	   [Test]   public virtual void testQueryByInvalidVersion()
//	  {
//		IQueryable<ICaseDefinition> query = repositoryService.CreateCaseDefinitionQuery();

//		query.CaseDefinitionVersion(3);

//		//verifyQueryResults(query, 0);

//		try
//		{
//		  query.CaseDefinitionVersion(-1);
//		  Assert.Fail();
//		}
//		catch (NotValidException)
//		{
//		  // Expected exception
//		}

//		try
//		{
//		  query.CaseDefinitionVersion(null);
//		  Assert.Fail();
//		}
//		catch (NotValidException)
//		{
//		  // Expected exception
//		}
//	  }

//	   [Test]   public virtual void testQueryByLatest()
//	  {
//		IQueryable<ICaseDefinition> query = repositoryService.CreateCaseDefinitionQuery();

//		query/*.LatestVersion()*/;

//		//verifyQueryResults(query, 3);

//		query.CaseDefinitionKey("one")/*.LatestVersion()*/;

//		//verifyQueryResults(query, 1);

//		query.CaseDefinitionKey("two")/*.LatestVersion()*/;
//		//verifyQueryResults(query, 1);
//	  }

//	   [Test]   public virtual void testInvalidUsageOfLatest()
//	  {
//		IQueryable<ICaseDefinition> query = repositoryService.CreateCaseDefinitionQuery();

//		try
//		{
//		  query.CaseDefinitionId("test")/*.LatestVersion()*/.ToList();
//		  Assert.Fail();
//		}
//		catch (NotValidException)
//		{
//		  // Expected exception
//		}

//		try
//		{
//		  query.CaseDefinitionName("test")/*.LatestVersion()*/.ToList();
//		  Assert.Fail();
//		}
//		catch (NotValidException)
//		{
//		  // Expected exception
//		}

//		try
//		{
//		  query.CaseDefinitionNameLike("test")/*.LatestVersion()*/.ToList();
//		  Assert.Fail();
//		}
//		catch (NotValidException)
//		{
//		  // Expected exception
//		}

//		try
//		{
//		  query.CaseDefinitionVersion(1)/*.LatestVersion()*/.ToList();
//		  Assert.Fail();
//		}
//		catch (NotValidException)
//		{
//		  // Expected exception
//		}

//		try
//		{
//		  query.DeploymentId("test")/*.LatestVersion()*/.ToList();
//		  Assert.Fail();
//		}
//		catch (NotValidException)
//		{
//		  // Expected exception
//		}
//	  }

//	   [Test]   public virtual void testQuerySorting()
//	  {
//		IQueryable<ICaseDefinition> query = repositoryService.CreateCaseDefinitionQuery();

//		// asc
//		query//.OrderByCaseDefinitionId()/*.Asc()*/;
//		//verifyQueryResults(query, 4);

//		query = repositoryService.CreateCaseDefinitionQuery();

//		query.OrderByDeploymentId()/*.Asc()*/;
//		//verifyQueryResults(query, 4);

//		query = repositoryService.CreateCaseDefinitionQuery();

//		query.OrderByCaseDefinitionKey()/*.Asc()*/;
//		//verifyQueryResults(query, 4);

//		query = repositoryService.CreateCaseDefinitionQuery();

//		query.OrderByCaseDefinitionVersion()/*.Asc()*/;
//		//verifyQueryResults(query, 4);

//		// Desc

//		query = repositoryService.CreateCaseDefinitionQuery();

//		query//.OrderByCaseDefinitionId()/*.Desc()*/;
//		//verifyQueryResults(query, 4);

//		query = repositoryService.CreateCaseDefinitionQuery();

//		query.OrderByDeploymentId()/*.Desc()*/;
//		//verifyQueryResults(query, 4);

//		query = repositoryService.CreateCaseDefinitionQuery();

//		query.OrderByCaseDefinitionKey()/*.Desc()*/;
//		//verifyQueryResults(query, 4);

//		query = repositoryService.CreateCaseDefinitionQuery();

//		query.OrderByCaseDefinitionVersion()/*.Desc()*/;
//		//verifyQueryResults(query, 4);

//		query = repositoryService.CreateCaseDefinitionQuery();

//		// Typical use case
//		query.OrderByCaseDefinitionKey()/*.Asc()*/.OrderByCaseDefinitionVersion()/*.Desc()*/;

//		IList<ICaseDefinition> caseDefinitions = query.ToList();
//		Assert.AreEqual(4, caseDefinitions.Count);

//		Assert.AreEqual("one", caseDefinitions[0].Key);
//		Assert.AreEqual(2, caseDefinitions[0].Version);
//		Assert.AreEqual("one", caseDefinitions[1].Key);
//		Assert.AreEqual(1, caseDefinitions[1].Version);
//		Assert.AreEqual("two", caseDefinitions[2].Key);
//		Assert.AreEqual(1, caseDefinitions[2].Version);
//	  }

//	}

//}