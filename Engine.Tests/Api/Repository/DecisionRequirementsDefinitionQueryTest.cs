//using System.Collections.Generic;
//using System.Linq;
//using ESS.FW.Bpm.Engine.Tests.Util;
//using NUnit.Framework;
//using ESS.FW.Bpm.Engine.Repository;

//namespace ESS.FW.Bpm.Engine.Tests.Api.Repository
//{
//	using IDecisionRequirementsDefinition = Engine.Repository.IDecisionRequirementsDefinition;
    
//	public class DecisionRequirementsDefinitionQueryTest
//	{
//		private bool InstanceFieldsInitialized = false;

//		public DecisionRequirementsDefinitionQueryTest()
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


//	  protected internal const string DRD_SCORE_RESOURCE = "resources/dmn/deployment/drdScore.Dmn11.xml";
//	  protected internal const string DRD_DISH_RESOURCE = "resources/dmn/deployment/drdDish.Dmn11.xml";
//	  protected internal const string DRD_XYZ_RESOURCE = "resources/api/repository/drdXyz_.Dmn11.xml";

//	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
//	  protected internal ProcessEngineTestRule testRule;

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testRule);
//	  //public RuleChain ruleChain;

//	  protected internal IRepositoryService repositoryService;

//	  protected internal string decisionRequirementsDefinitionId;
//	  protected internal string firstDeploymentId;
//	  protected internal string secondDeploymentId;
//	  protected internal string thirdDeploymentId;

//    [SetUp]
//	  public virtual void init()
//	  {
//		repositoryService = engineRule.RepositoryService;

//		firstDeploymentId = testRule.Deploy(DRD_DISH_RESOURCE, DRD_SCORE_RESOURCE).Id;
//		secondDeploymentId = testRule.Deploy(DRD_DISH_RESOURCE).Id;
//		thirdDeploymentId = testRule.Deploy(DRD_XYZ_RESOURCE).Id;

//		decisionRequirementsDefinitionId = repositoryService.CreateDecisionRequirementsDefinitionQuery(c=>c.Key=="score").First().Id;
//	  }


//        [Test]
//        public virtual void queryByDecisionRequirementsDefinitionId()
//	  {
//		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery();

//		Assert.That(query.DecisionRequirementsDefinitionId("notExisting").Count(), Is.EqualTo(0L));

//		Assert.That(query.DecisionRequirementsDefinitionId(decisionRequirementsDefinitionId).Count(), Is.EqualTo(1L));
//		Assert.That(query.First().Key, Is.EqualTo("score"));
//	  }


//        [Test]
//        public virtual void queryByDecisionRequirementsDefinitionIds()
//	  {
//		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery();

//		Assert.That(query.DecisionRequirementsDefinitionIdIn("not", "existing").Count(), Is.EqualTo(0L));

//		Assert.That(query.DecisionRequirementsDefinitionIdIn(decisionRequirementsDefinitionId, "notExisting").Count(), Is.EqualTo(1L));
//		Assert.That(query.First().Key, Is.EqualTo("score"));
//	  }


//        [Test]
//        public virtual void queryByDecisionRequirementsDefinitionKey()
//	  {
//		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery();

//		Assert.That(query.DecisionRequirementsDefinitionKey("notExisting").Count(), Is.EqualTo(0L));

//		Assert.That(query.DecisionRequirementsDefinitionKey("score").Count(), Is.EqualTo(1L));
//		Assert.That(query.First().Key, Is.EqualTo("score"));
//	  }


//        [Test]
//        public virtual void queryByDecisionRequirementsDefinitionKeyLike()
//	  {
//		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery();

//		Assert.That(query.DecisionRequirementsDefinitionKeyLike("%notExisting%").Count(), Is.EqualTo(0L));

//		Assert.That(query.DecisionRequirementsDefinitionKeyLike("%sco%").Count(), Is.EqualTo(1L));
//		Assert.That(query.DecisionRequirementsDefinitionKeyLike("%dis%").Count(), Is.EqualTo(2L));
//		Assert.That(query.DecisionRequirementsDefinitionKeyLike("%s%").Count(), Is.EqualTo(3L));
//	  }


//        [Test]
//        public virtual void queryByDecisionRequirementsDefinitionName()
//	  {
//		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery();

//		Assert.That(query.DecisionRequirementsDefinitionName("notExisting").Count(), Is.EqualTo(0L));

//		Assert.That(query.DecisionRequirementsDefinitionName("Score").Count(), Is.EqualTo(1L));
//		Assert.That(query.First().Key, Is.EqualTo("score"));
//	  }


//        [Test]
//        public virtual void queryByDecisionRequirementsDefinitionNameLike()
//	  {
//		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery();

//		Assert.That(query.DecisionRequirementsDefinitionNameLike("%notExisting%").Count(), Is.EqualTo(0L));

//		Assert.That(query.DecisionRequirementsDefinitionNameLike("%Sco%").Count(), Is.EqualTo(1L));
//		Assert.That(query.DecisionRequirementsDefinitionNameLike("%ish%").Count(), Is.EqualTo(2L));
//	  }


//        [Test]
//        public virtual void queryByDecisionRequirementsDefinitionCategory()
//	  {
//		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery();

//		//Assert.That(query.DecisionRequirementsDefinitionCategory("notExisting").Count(), Is.EqualTo(0L));

//		//Assert.That(query.DecisionRequirementsDefinitionCategory("test-drd-1").Count(), Is.EqualTo(1L));
//		Assert.That(query.First().Key, Is.EqualTo("score"));
//	  }


//        [Test]
//        public virtual void queryByDecisionRequirementsDefinitionCategoryLike()
//	  {
//		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery();

//		//Assert.That(query.DecisionRequirementsDefinitionCategoryLike("%notExisting%").Count(), Is.EqualTo(0L));

//		//Assert.That(query.DecisionRequirementsDefinitionCategoryLike("%test%").Count(), Is.EqualTo(3L));

//		//Assert.That(query.DecisionRequirementsDefinitionCategoryLike("%z\\_").Count(), Is.EqualTo(1L));
//	  }


//        [Test]
//        public virtual void queryByResourceName()
//	  {
//		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery();

//		Assert.That(query.DecisionRequirementsDefinitionResourceName("notExisting").Count(), Is.EqualTo(0L));

//		Assert.That(query.DecisionRequirementsDefinitionResourceName(DRD_SCORE_RESOURCE).Count(), Is.EqualTo(1L));
//		Assert.That(query.First().Key, Is.EqualTo("score"));
//	  }


//        [Test]
//        public virtual void queryByResourceNameLike()
//	  {
//		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery();

//		Assert.That(query.DecisionRequirementsDefinitionResourceNameLike("%notExisting%").Count(), Is.EqualTo(0L));

//		Assert.That(query.DecisionRequirementsDefinitionResourceNameLike("%.Dmn11.xml%").Count(), Is.EqualTo(4L));
//	  }


//        [Test]
//        public virtual void queryByResourceNameLikeEscape()
//	  {
//		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery();

//		Assert.That(query.DecisionRequirementsDefinitionResourceNameLike("%z\\_.%").Count(), Is.EqualTo(1L));
//	  }


//        [Test]
//        public virtual void queryByVersion()
//	  {
//		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery();

//		Assert.That(query.DecisionRequirementsDefinitionVersion(1).Count(), Is.EqualTo(3L));
//		Assert.That(query.DecisionRequirementsDefinitionVersion(2).Count(), Is.EqualTo(1L));
//		Assert.That(query.DecisionRequirementsDefinitionVersion(3).Count(), Is.EqualTo(0L));
//	  }


//        [Test]
//        public virtual void queryByLatest()
//	  {
//		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery();

//		Assert.That(query/*.LatestVersion()*/.Count(), Is.EqualTo(3L));
//		Assert.That(query.DecisionRequirementsDefinitionKey("score")/*.LatestVersion()*/.Count(), Is.EqualTo(1L));
//	  }


//        [Test]
//        public virtual void queryByDeploymentId()
//	  {
//		IQueryable<IDecisionRequirementsDefinition> query = repositoryService.CreateDecisionRequirementsDefinitionQuery();

//		Assert.That(query.SetDeploymentId("notExisting").Count(), Is.EqualTo(0L));

//		Assert.That(query.SetDeploymentId(firstDeploymentId).Count(), Is.EqualTo(2L));
//		Assert.That(query.SetDeploymentId(secondDeploymentId).Count(), Is.EqualTo(1L));
//	  }


//        [Test]
//        public virtual void orderByDecisionRequirementsDefinitionId()
//	  {
//		IList<IDecisionRequirementsDefinition> decisionRequirementsDefinitions = repositoryService.CreateDecisionRequirementsDefinitionQuery().OrderByDecisionRequirementsDefinitionId()/*.Asc()*/.ToList();

//		Assert.That(decisionRequirementsDefinitions.Count, Is.EqualTo(4));
//		Assert.That(decisionRequirementsDefinitions[0].Id.StartsWith("dish:1"));
//		Assert.That(decisionRequirementsDefinitions[1].Id.StartsWith("dish:2"));
//		Assert.That(decisionRequirementsDefinitions[2].Id.StartsWith("score:1"));
//		Assert.That(decisionRequirementsDefinitions[3].Id.StartsWith("xyz:1"));

//		decisionRequirementsDefinitions = repositoryService.CreateDecisionRequirementsDefinitionQuery().OrderByDecisionRequirementsDefinitionId()/*.Desc()*/.ToList();

//		Assert.That(decisionRequirementsDefinitions[0].Id.StartsWith("xyz:1"));
//		Assert.That(decisionRequirementsDefinitions[1].Id.StartsWith("score:1"));
//		Assert.That(decisionRequirementsDefinitions[2].Id.StartsWith("dish:2"));
//		Assert.That(decisionRequirementsDefinitions[3].Id.StartsWith("dish:1"));
//	  }


//        [Test]
//        public virtual void orderByDecisionRequirementsDefinitionKey()
//        {
//            IList<IDecisionRequirementsDefinition> decisionRequirementsDefinitions =
//                repositoryService.CreateDecisionRequirementsDefinitionQuery()
//                    .OrderByDecisionRequirementsDefinitionKey()
//                    /*.Asc()*/
//                    .ToList();

//		Assert.That(decisionRequirementsDefinitions.Count, Is.EqualTo(4));
//		Assert.That(decisionRequirementsDefinitions[0].Key, Is.EqualTo("dish"));
//		Assert.That(decisionRequirementsDefinitions[1].Key, Is.EqualTo("dish"));
//		Assert.That(decisionRequirementsDefinitions[2].Key, Is.EqualTo("score"));
//		Assert.That(decisionRequirementsDefinitions[3].Key, Is.EqualTo("xyz"));

//		decisionRequirementsDefinitions = repositoryService.CreateDecisionRequirementsDefinitionQuery().OrderByDecisionRequirementsDefinitionKey()/*.Desc()*/.ToList();

//		Assert.That(decisionRequirementsDefinitions[0].Key, Is.EqualTo("xyz"));
//		Assert.That(decisionRequirementsDefinitions[1].Key, Is.EqualTo("score"));
//		Assert.That(decisionRequirementsDefinitions[2].Key, Is.EqualTo("dish"));
//		Assert.That(decisionRequirementsDefinitions[3].Key, Is.EqualTo("dish"));
//	  }


//        [Test]
//        public virtual void orderByDecisionRequirementsDefinitionName()
//	  {
//		IList<IDecisionRequirementsDefinition> decisionRequirementsDefinitions = repositoryService.CreateDecisionRequirementsDefinitionQuery().OrderByDecisionRequirementsDefinitionName()/*.Asc()*/.ToList();

//		Assert.That(decisionRequirementsDefinitions.Count, Is.EqualTo(4));
//		Assert.That(decisionRequirementsDefinitions[0].Name, Is.EqualTo("Dish"));
//		Assert.That(decisionRequirementsDefinitions[1].Name, Is.EqualTo("Dish"));
//		Assert.That(decisionRequirementsDefinitions[2].Name, Is.EqualTo("Score"));
//		Assert.That(decisionRequirementsDefinitions[3].Name, Is.EqualTo("Xyz"));

//		decisionRequirementsDefinitions = repositoryService.CreateDecisionRequirementsDefinitionQuery().OrderByDecisionRequirementsDefinitionName()/*.Desc()*/.ToList();

//		Assert.That(decisionRequirementsDefinitions[0].Name, Is.EqualTo("Xyz"));
//		Assert.That(decisionRequirementsDefinitions[1].Name, Is.EqualTo("Score"));
//		Assert.That(decisionRequirementsDefinitions[2].Name, Is.EqualTo("Dish"));
//		Assert.That(decisionRequirementsDefinitions[3].Name, Is.EqualTo("Dish"));
//	  }


//        [Test]
//        public virtual void orderByDecisionRequirementsDefinitionCategory()
//	  {
//		IList<IDecisionRequirementsDefinition> decisionRequirementsDefinitions = repositoryService.CreateDecisionRequirementsDefinitionQuery()//.OrderByDecisionRequirementsDefinitionCategory()/*.Asc()*/.ToList();

//		Assert.That(decisionRequirementsDefinitions.Count, Is.EqualTo(4));
//		Assert.That(decisionRequirementsDefinitions[0].Category, Is.EqualTo("test-drd-1"));
//		Assert.That(decisionRequirementsDefinitions[1].Category, Is.EqualTo("test-drd-2"));
//		Assert.That(decisionRequirementsDefinitions[2].Category, Is.EqualTo("test-drd-2"));
//		Assert.That(decisionRequirementsDefinitions[3].Category, Is.EqualTo("xyz_"));

//		decisionRequirementsDefinitions = repositoryService.CreateDecisionRequirementsDefinitionQuery()//.OrderByDecisionRequirementsDefinitionCategory()/*.Desc()*/.ToList();

//		Assert.That(decisionRequirementsDefinitions[0].Category, Is.EqualTo("xyz_"));
//		Assert.That(decisionRequirementsDefinitions[1].Category, Is.EqualTo("test-drd-2"));
//		Assert.That(decisionRequirementsDefinitions[2].Category, Is.EqualTo("test-drd-2"));
//		Assert.That(decisionRequirementsDefinitions[3].Category, Is.EqualTo("test-drd-1"));
//	  }


//        [Test]
//        public virtual void orderByDecisionRequirementsDefinitionVersion()
//	  {
//		IList<IDecisionRequirementsDefinition> decisionRequirementsDefinitions = repositoryService.CreateDecisionRequirementsDefinitionQuery().OrderByDecisionRequirementsDefinitionVersion()/*.Asc()*/.ToList();

//		Assert.That(decisionRequirementsDefinitions.Count, Is.EqualTo(4));
//		Assert.That(decisionRequirementsDefinitions[0].Version, Is.EqualTo(1));
//		Assert.That(decisionRequirementsDefinitions[1].Version, Is.EqualTo(1));
//		Assert.That(decisionRequirementsDefinitions[2].Version, Is.EqualTo(1));
//		Assert.That(decisionRequirementsDefinitions[3].Version, Is.EqualTo(2));

//		decisionRequirementsDefinitions = repositoryService.CreateDecisionRequirementsDefinitionQuery().OrderByDecisionRequirementsDefinitionVersion()/*.Desc()*/.ToList();

//		Assert.That(decisionRequirementsDefinitions[0].Version, Is.EqualTo(2));
//		Assert.That(decisionRequirementsDefinitions[1].Version, Is.EqualTo(1));
//		Assert.That(decisionRequirementsDefinitions[2].Version, Is.EqualTo(1));
//	  }


//        [Test]
//        public virtual void orderByDeploymentId()
//	  {
//		IList<IDecisionRequirementsDefinition> decisionRequirementsDefinitions = repositoryService.CreateDecisionRequirementsDefinitionQuery().OrderByDeploymentId()/*.Asc()*/.ToList();

//		Assert.That(decisionRequirementsDefinitions.Count, Is.EqualTo(4));
//		Assert.That(decisionRequirementsDefinitions[0].DeploymentId, Is.EqualTo(firstDeploymentId));
//		Assert.That(decisionRequirementsDefinitions[1].DeploymentId, Is.EqualTo(firstDeploymentId));
//		Assert.That(decisionRequirementsDefinitions[2].DeploymentId, Is.EqualTo(secondDeploymentId));
//		Assert.That(decisionRequirementsDefinitions[3].DeploymentId, Is.EqualTo(thirdDeploymentId));

//		decisionRequirementsDefinitions = repositoryService.CreateDecisionRequirementsDefinitionQuery().OrderByDeploymentId()/*.Desc()*/.ToList();

//		Assert.That(decisionRequirementsDefinitions[0].DeploymentId, Is.EqualTo(thirdDeploymentId));
//		Assert.That(decisionRequirementsDefinitions[1].DeploymentId, Is.EqualTo(secondDeploymentId));
//		Assert.That(decisionRequirementsDefinitions[2].DeploymentId, Is.EqualTo(firstDeploymentId));
//		Assert.That(decisionRequirementsDefinitions[3].DeploymentId, Is.EqualTo(firstDeploymentId));
//	  }

//	}

//}