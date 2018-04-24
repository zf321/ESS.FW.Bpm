//using System.Collections.Generic;
//using System.Linq;
//using ESS.FW.Bpm.Engine.exception;
//using ESS.FW.Bpm.Engine.Repository;
//using ESS.FW.Bpm.Engine.Tests.Util;
//using NUnit.Framework;


//namespace ESS.FW.Bpm.Engine.Tests.Api.Repository
//{
    

//	public class DecisionDefinitionQueryTest
//	{
//		private bool InstanceFieldsInitialized = false;

//		public DecisionDefinitionQueryTest()
//		{
//			if (!InstanceFieldsInitialized)
//			{
//				InitializeInstanceFields();
//				InstanceFieldsInitialized = true;
//			}
//		}

//		private void InitializeInstanceFields()
//		{
//			testRule = new ProcessEngineTestRule(engineRule);
//			//ruleChain = RuleChain.outerRule(engineRule).around(testRule);
//		}


//	  protected internal const string DMN_ONE_RESOURCE = "resources/repository/one.Dmn";
//	  protected internal const string DMN_TWO_RESOURCE = "resources/repository/two.Dmn";
//	  protected internal const string DMN_THREE_RESOURCE = "resources/api/repository/three_.Dmn";

//	  protected internal const string DRD_SCORE_RESOURCE = "resources/dmn/deployment/drdScore.Dmn11.xml";
//	  protected internal const string DRD_DISH_RESOURCE = "resources/dmn/deployment/drdDish.Dmn11.xml";

//	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
//	  protected internal ProcessEngineTestRule testRule;



//	  protected internal IRepositoryService repositoryService;

//	  protected internal string decisionRequirementsDefinitionId;
//	  protected internal string firstDeploymentId;
//	  protected internal string secondDeploymentId;
//	  protected internal string thirdDeploymentId;

//    [SetUp]
//	  public virtual void init()
//	  {
//		repositoryService = engineRule.RepositoryService;

//		firstDeploymentId = testRule.Deploy(DMN_ONE_RESOURCE, DMN_TWO_RESOURCE).Id;
//		secondDeploymentId = testRule.Deploy(DMN_ONE_RESOURCE).Id;
//		thirdDeploymentId = testRule.Deploy(DMN_THREE_RESOURCE).Id;
//	  }


//        [Test]
//        public virtual void decisionDefinitionProperties()
//        {
//            IList<IDecisionDefinition> decisionDefinitions = repositoryService.CreateDecisionDefinitionQuery()
//                .OrderByDecisionDefinitionName()
//                /*.Asc()*/
//                //.OrderByDecisionDefinitionVersion()
//                /*.Asc()*/
//                //.OrderByDecisionDefinitionCategory()
//                /*.Asc()*/
//                .ToList();

//		IDecisionDefinition decisionDefinition = decisionDefinitions[0];
//		Assert.AreEqual("one", decisionDefinition.Key);
//		Assert.AreEqual("One", decisionDefinition.Name);
//		Assert.True(decisionDefinition.Id.StartsWith("one:1"));
//		Assert.AreEqual("Examples", decisionDefinition.Category);
//		Assert.AreEqual(1, decisionDefinition.Version);
//		Assert.AreEqual("resources/repository/one.Dmn", decisionDefinition.ResourceName);
//		Assert.AreEqual(firstDeploymentId, decisionDefinition.DeploymentId);

//		decisionDefinition = decisionDefinitions[1];
//		Assert.AreEqual("one", decisionDefinition.Key);
//		Assert.AreEqual("One", decisionDefinition.Name);
//		Assert.True(decisionDefinition.Id.StartsWith("one:2"));
//		Assert.AreEqual("Examples", decisionDefinition.Category);
//		Assert.AreEqual(2, decisionDefinition.Version);
//		Assert.AreEqual("resources/repository/one.Dmn", decisionDefinition.ResourceName);
//		Assert.AreEqual(secondDeploymentId, decisionDefinition.DeploymentId);

//		decisionDefinition = decisionDefinitions[2];
//		Assert.AreEqual("two", decisionDefinition.Key);
//		Assert.AreEqual("Two", decisionDefinition.Name);
//		Assert.True(decisionDefinition.Id.StartsWith("two:1"));
//		Assert.AreEqual("Examples2", decisionDefinition.Category);
//		Assert.AreEqual(1, decisionDefinition.Version);
//		Assert.AreEqual("resources/repository/two.Dmn", decisionDefinition.ResourceName);
//		Assert.AreEqual(firstDeploymentId, decisionDefinition.DeploymentId);
//	  }


//        [Test]
//        public virtual void queryByDecisionDefinitionIds()
//	  {
//		// empty list
//		Assert.True(repositoryService.CreateDecisionDefinitionQuery().DecisionDefinitionIdIn("a", "b").Count()==0);

//		// collect all ids
//	      IList<IDecisionDefinition> decisionDefinitions = repositoryService.CreateDecisionDefinitionQuery()
//	          .ToList();
//		IList<string> ids = new List<string>();
//		foreach (IDecisionDefinition decisionDefinition in decisionDefinitions)
//		{
//		  ids.Add(decisionDefinition.Id);
//		}

//	      decisionDefinitions = repositoryService.CreateDecisionDefinitionQuery()
//	          .DecisionDefinitionIdIn(ids.ToArray())
//	          .ToList();

//		Assert.AreEqual(ids.Count, decisionDefinitions.Count);
//		foreach (IDecisionDefinition decisionDefinition in decisionDefinitions)
//		{
//		  if (!ids.Contains(decisionDefinition.Id))
//		  {
//			Assert.Fail("Expected to find decision definition " + decisionDefinition);
//		  }
//		}
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void queryByDeploymentId()
//	  public virtual void queryByDeploymentId()
//	  {
//		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

//		query.DeploymentId(firstDeploymentId);

//		//verifyQueryResults(query, 2);
//	  }


//        [Test]
//        public virtual void queryByInvalidDeploymentId()
//	  {
//		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

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


//        [Test]
//        public virtual void queryByName()
//	  {
//		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

//		query.DecisionDefinitionName("Two");

//		//verifyQueryResults(query, 1);

//		query.DecisionDefinitionName("One");

//		//verifyQueryResults(query, 2);
//	  }


//        [Test]
//        public virtual void queryByInvalidName()
//	  {
//		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

//		query.DecisionDefinitionName("invalid");

//		//verifyQueryResults(query, 0);

//		try
//		{
//		  query.DecisionDefinitionName(null);
//		  Assert.Fail();
//		}
//		catch (NotValidException)
//		{
//		  // Expected exception
//		}
//	  }


//        [Test]
//        public virtual void queryByNameLike()
//	  {
//		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

//		query.DecisionDefinitionNameLike("%w%");

//		//verifyQueryResults(query, 1);

//		query.DecisionDefinitionNameLike("%z\\_");

//		//verifyQueryResults(query, 1);
//	  }


//        [Test]
//        public virtual void queryByInvalidNameLike()
//	  {
//		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

//		query.DecisionDefinitionNameLike("%invalid%");

//		//verifyQueryResults(query, 0);

//		try
//		{
//		  query.DecisionDefinitionNameLike(null);
//		  Assert.Fail();
//		}
//		catch (NotValidException)
//		{
//		  // Expected exception
//		}
//	  }


//        [Test]
//        public virtual void queryByResourceNameLike()
//	  {
//		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

//		query.DecisionDefinitionResourceNameLike("%ree%");

//		//verifyQueryResults(query, 1);

//		query.DecisionDefinitionResourceNameLike("%ee\\_%");

//		//verifyQueryResults(query, 1);
//	  }


//        [Test]
//        public virtual void queryByInvalidNResourceNameLike()
//	  {
//		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

//		query.DecisionDefinitionResourceNameLike("%invalid%");

//		//verifyQueryResults(query, 0);

//		try
//		{
//		  query.DecisionDefinitionNameLike(null);
//		  Assert.Fail();
//		}
//		catch (NotValidException)
//		{
//		  // Expected exception
//		}
//	  }


//        [Test]
//        public virtual void queryByKey()
//	  {
//		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

//		// decision one
//		query.DecisionDefinitionKey("one");

//		//verifyQueryResults(query, 2);

//		// decision two
//		query.DecisionDefinitionKey("two");

//		//verifyQueryResults(query, 1);
//	  }


//        [Test]
//        public virtual void queryByInvalidKey()
//	  {
//		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

//		query.DecisionDefinitionKey("invalid");

//		//verifyQueryResults(query, 0);

//		try
//		{
//		  query.DecisionDefinitionKey(null);
//		  Assert.Fail();
//		}
//		catch (NotValidException)
//		{
//		  // Expected exception
//		}
//	  }


//        [Test]
//        public virtual void queryByKeyLike()
//	  {
//		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

//		query.DecisionDefinitionKeyLike("%o%");

//		//verifyQueryResults(query, 3);

//		query.DecisionDefinitionKeyLike("%z\\_");

//		//verifyQueryResults(query, 1);
//	  }


//        [Test]
//        public virtual void queryByInvalidKeyLike()
//	  {
//		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

//		query.DecisionDefinitionKeyLike("%invalid%");

//		//verifyQueryResults(query, 0);

//		try
//		{
//		  query.DecisionDefinitionKeyLike(null);
//		  Assert.Fail();
//		}
//		catch (NotValidException)
//		{
//		  // Expected exception
//		}
//	  }


//        [Test]
//        public virtual void queryByCategory()
//	  {
//		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

//		query.DecisionDefinitionCategory("Examples");

//		//verifyQueryResults(query, 2);
//	  }


//        [Test]
//        public virtual void queryByInvalidCategory()
//	  {
//		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

//		query.DecisionDefinitionCategory("invalid");

//		//verifyQueryResults(query, 0);

//		try
//		{
//		  query.DecisionDefinitionCategory(null);
//		  Assert.Fail();
//		}
//		catch (NotValidException)
//		{
//		  // Expected exception
//		}
//	  }


//        [Test]
//        public virtual void queryByCategoryLike()
//	  {
//		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

//		query.DecisionDefinitionCategoryLike("%Example%");

//		//verifyQueryResults(query, 3);

//		query.DecisionDefinitionCategoryLike("%amples2");

//		//verifyQueryResults(query, 1);

//		query.DecisionDefinitionCategoryLike("%z\\_");

//		//verifyQueryResults(query, 1);
//	  }


//        [Test]
//        public virtual void queryByInvalidCategoryLike()
//	  {
//		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

//		query.DecisionDefinitionCategoryLike("invalid");

//		//verifyQueryResults(query, 0);

//		try
//		{
//		  query.DecisionDefinitionCategoryLike(null);
//		  Assert.Fail();
//		}
//		catch (NotValidException)
//		{
//		  // Expected exception
//		}
//	  }


//        [Test]
//        public virtual void queryByVersion()
//	  {
//		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

//		query.DecisionDefinitionVersion(2);

//		//verifyQueryResults(query, 1);

//		query.DecisionDefinitionVersion(1);

//		//verifyQueryResults(query, 3);
//	  }


//        [Test]
//        public virtual void queryByInvalidVersion()
//	  {
//		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

//		query.DecisionDefinitionVersion(3);

//		//verifyQueryResults(query, 0);

//		try
//		{
//		  query.DecisionDefinitionVersion(-1);
//		  Assert.Fail();
//		}
//		catch (NotValidException)
//		{
//		  // Expected exception
//		}

//		try
//		{
//		  query.DecisionDefinitionVersion(null);
//		  Assert.Fail();
//		}
//		catch (NotValidException)
//		{
//		  // Expected exception
//		}
//	  }


//        [Test]
//        public virtual void queryByLatest()
//	  {
//		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

//		query/*.LatestVersion()*/;

//		//verifyQueryResults(query, 3);

//		query.DecisionDefinitionKey("one")/*.LatestVersion()*/;

//		//verifyQueryResults(query, 1);

//		query.DecisionDefinitionKey("two")/*.LatestVersion()*/;
//		//verifyQueryResults(query, 1);
//	  }

//        [Test]
//        public virtual void testInvalidUsageOfLatest()
//	  {
//		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

//		try
//		{
//		    query.DecisionDefinitionId("test")
//		        /*.LatestVersion()*/
//		        .ToList();
//		  Assert.Fail();
//		}
//		catch (NotValidException)
//		{
//		  // Expected exception
//		}

//		try
//		{
//		    query.DecisionDefinitionName("test")
//		        /*.LatestVersion()*/
//		        .ToList();
//		  Assert.Fail();
//		}
//		catch (NotValidException)
//		{
//		  // Expected exception
//		}

//		try
//		{
//		    query.DecisionDefinitionNameLike("test")
//		        /*.LatestVersion()*/
//		        .ToList();
//		  Assert.Fail();
//		}
//		catch (NotValidException)
//		{
//		  // Expected exception
//		}

//		try
//		{
//		    query.DecisionDefinitionVersion(1)
//		        /*.LatestVersion()*/
//		        .ToList();
//		  Assert.Fail();
//		}
//		catch (NotValidException)
//		{
//		  // Expected exception
//		}

//		try
//		{
//		    query.DeploymentId("test")
//		        /*.LatestVersion()*/
//		        .ToList();
//		  Assert.Fail();
//		}
//		catch (NotValidException)
//		{
//		  // Expected exception
//		}
//	  }


//        [Test]
//        public virtual void queryByDecisionRequirementsDefinitionId()
//	  {
//		testRule.Deploy(DRD_DISH_RESOURCE, DRD_SCORE_RESOURCE);

//	      IList<IDecisionRequirementsDefinition> drds = repositoryService.CreateDecisionRequirementsDefinitionQuery()
//	          .OrderByDecisionRequirementsDefinitionName()
//	          /*.Asc()*/
//	          .ToList();

//		string dishDrdId = drds[0].Id;
//		string scoreDrdId = drds[1].Id;

//		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

//		//verifyQueryResults(query.DecisionRequirementsDefinitionId("non existing"), 0);
//		//verifyQueryResults(query.DecisionRequirementsDefinitionId(dishDrdId), 3);
//		//verifyQueryResults(query.DecisionRequirementsDefinitionId(scoreDrdId), 2);
//	  }


//        [Test]
//        public virtual void queryByDecisionRequirementsDefinitionKey()
//	  {
//		testRule.Deploy(DRD_DISH_RESOURCE, DRD_SCORE_RESOURCE);

//		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

//		//verifyQueryResults(query.DecisionRequirementsDefinitionKey("non existing"), 0);
//		//verifyQueryResults(query.DecisionRequirementsDefinitionKey("dish"), 3);
//		//verifyQueryResults(query.DecisionRequirementsDefinitionKey("score"), 2);
//	  }


//        [Test]
//        public virtual void queryByWithoutDecisionRequirementsDefinition()
//	  {
//		testRule.Deploy(DRD_DISH_RESOURCE, DRD_SCORE_RESOURCE);

//		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

//		//verifyQueryResults(query, 9);
//		//verifyQueryResults(query.WithoutDecisionRequirementsDefinition(), 4);
//	  }


//        [Test]
//        public virtual void querySorting()
//	  {
//		IQueryable<IDecisionDefinition> query = repositoryService.CreateDecisionDefinitionQuery();

//		// asc
//		query.OrderByDecisionDefinitionId()/*.Asc()*/;
//		//verifyQueryResults(query, 4);

//		query = repositoryService.CreateDecisionDefinitionQuery();

//		query.OrderByDeploymentId()/*.Asc()*/;
//		//verifyQueryResults(query, 4);

//		query = repositoryService.CreateDecisionDefinitionQuery();

//		query//.OrderByDecisionDefinitionKey()/*.Asc()*/;
//		//verifyQueryResults(query, 4);

//		query = repositoryService.CreateDecisionDefinitionQuery();

//		query//.OrderByDecisionDefinitionVersion()/*.Asc()*/;
//		//verifyQueryResults(query, 4);

//		// Desc

//		query = repositoryService.CreateDecisionDefinitionQuery();

//		query.OrderByDecisionDefinitionId()/*.Desc()*/;
//		//verifyQueryResults(query, 4);

//		query = repositoryService.CreateDecisionDefinitionQuery();

//		query.OrderByDeploymentId()/*.Desc()*/;
//		//verifyQueryResults(query, 4);

//		query = repositoryService.CreateDecisionDefinitionQuery();

//		query//.OrderByDecisionDefinitionKey()/*.Desc()*/;
//		//verifyQueryResults(query, 4);

//		query = repositoryService.CreateDecisionDefinitionQuery();

//		query//.OrderByDecisionDefinitionVersion()/*.Desc()*/;
//		//verifyQueryResults(query, 4);

//		query = repositoryService.CreateDecisionDefinitionQuery();

//		// Typical use decision
//		query//.OrderByDecisionDefinitionKey()/*.Asc()*///.OrderByDecisionDefinitionVersion()/*.Desc()*/;

//	      IList<IDecisionDefinition> decisionDefinitions = query.ToList();
//		Assert.AreEqual(4, decisionDefinitions.Count);

//		Assert.AreEqual("one", decisionDefinitions[0].Key);
//		Assert.AreEqual(2, decisionDefinitions[0].Version);
//		Assert.AreEqual("one", decisionDefinitions[1].Key);
//		Assert.AreEqual(1, decisionDefinitions[1].Version);
//		Assert.AreEqual("two", decisionDefinitions[2].Key);
//		Assert.AreEqual(1, decisionDefinitions[2].Version);
//	  }


//	  protected internal virtual void verifyQueryResults(IQueryable<IDecisionDefinition> query, int expectedCount)
//	  {
//		Assert.AreEqual(expectedCount, query.Count());
//		Assert.AreEqual(expectedCount, query.Count());
//	  }

//	}

//}