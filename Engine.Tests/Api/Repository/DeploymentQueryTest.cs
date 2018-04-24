//using System;
//using System.Collections.Generic;
//using System.Linq;
//using ESS.FW.Bpm.Engine.exception;
//using ESS.FW.Bpm.Engine.Impl.Calendar;
//using ESS.FW.Bpm.Engine.Repository;
//using NUnit.Framework;
//using PluggableProcessEngineTestCase = ESS.FW.Bpm.Engine.Tests.PluggableProcessEngineTestCase;



//namespace ESS.FW.Bpm.Engine.Tests.Api.Repository
//{

    


//    /// <summary>
//	/// 
//	/// @author Ingo Richtsmeier
//	/// </summary>
//	public class DeploymentQueryTest : PluggableProcessEngineTestCase
//	{

//	  private string deploymentOneId;
//	  private string deploymentTwoId;

//        [SetUp]
//	  protected internal  void setUp()
//	  {
//		deploymentOneId = repositoryService.CreateDeployment().Name("resources/repository/one.bpmn20.xml").AddClasspathResource("resources/repository/one.bpmn20.xml").Source(ProcessApplicationDeploymentFields.ProcessApplicationDeploymentSource).Deploy().Id;

//		deploymentTwoId = repositoryService.CreateDeployment().Name("resources/repository/two_.bpmn20.xml").AddClasspathResource("resources/repository/two.bpmn20.xml").Deploy().Id;

//		//base.SetUp();
//	  }

//        [TearDown]
//	  protected internal void tearDown()
//	  {
//		base.TearDown();
//		repositoryService.DeleteDeployment(deploymentOneId, true);
//		repositoryService.DeleteDeployment(deploymentTwoId, true);
//	  }

//	   [Test]   public virtual void testQueryNoCriteria()
//	  {
//		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery();
//		Assert.AreEqual(2, query.Count());
//		Assert.AreEqual(2, query.Count());

//		try
//		{
//		  query.First();
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		}
//	  }

//	   [Test]   public virtual void testQueryByDeploymentId()
//	  {
//		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery(c=> c.DeploymentId == deploymentOneId);
//		Assert.NotNull(query.First());
//		Assert.AreEqual(1, query.Count());
//		Assert.AreEqual(1, query.Count());
//	  }

//	   [Test]   public virtual void testQueryByInvalidDeploymentId()
//	  {
//		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery(c=> c.DeploymentId == "invalid");
//		Assert.IsNull(query.First());
//		Assert.AreEqual(0, query.Count());
//		Assert.AreEqual(0, query.Count());

//		try
//		{
//		  repositoryService.CreateDeploymentQuery(c=> c.DeploymentId == null);
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		}
//	  }

//	   [Test]   public virtual void testQueryByName()
//	  {
//		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery(c=>c.Name == "resources/repository/two_.bpmn20.xml");
//		Assert.NotNull(query.First());
//		Assert.AreEqual(1, query.Count());
//		Assert.AreEqual(1, query.Count());
//	  }

//	   [Test]   public virtual void testQueryByInvalidName()
//	  {
//		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery(c=>c.Name == "invalid");
//		Assert.IsNull(query.First());
//		Assert.AreEqual(0, query.Count());
//		Assert.AreEqual(0, query.Count());

//		try
//		{
//		  repositoryService.CreateDeploymentQuery(c=>c.Name == null);
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		}
//	  }

//	   [Test]   public virtual void testQueryByNameLike()
//	  {
//		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery().DeploymentNameLike("%camunda%");
//		Assert.AreEqual(2, query.Count());
//		Assert.AreEqual(2, query.Count());

//		query = repositoryService.CreateDeploymentQuery().DeploymentNameLike("%two\\_%");
//		Assert.AreEqual(1, query.Count());
//		Assert.AreEqual(1, query.Count());
//		Assert.AreEqual("resources/repository/two_.bpmn20.xml", query.First().Name);
//	  }

//	   [Test]   public virtual void testQueryByInvalidNameLike()
//	  {
//		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery().DeploymentNameLike("invalid");
//		Assert.IsNull(query.First());
//		Assert.AreEqual(0, query.Count());
//		Assert.AreEqual(0, query.Count());

//		try
//		{
//		  repositoryService.CreateDeploymentQuery().DeploymentNameLike(null);
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		}
//	  }
        
//	   [Test]   public virtual void testQueryByDeploymentBefore()
//	  {
//		DateTime later = new DateTime((DateTimeUtil.Now().Millisecond + (10 * 3600)));
//            DateTime earlier = new DateTime((DateTimeUtil.Now().Millisecond - (10 * 3600)));

//            long Count = repositoryService.CreateDeploymentQuery().DeploymentBefore(later).Count();
//		Assert.AreEqual(2, Count);

//		Count = repositoryService.CreateDeploymentQuery().DeploymentBefore(earlier).Count();
//		Assert.AreEqual(0, Count);

//		try
//		{
//		  repositoryService.CreateDeploymentQuery().DeploymentBefore(DateTime.Now);
//		  Assert.Fail("Exception expected");
//		}
//		catch (NullValueException)
//		{
//		  // expected
//		}
//	  }

////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: public void testQueryDeploymentAfter() throws Exception
//	   [Test]   public virtual void testQueryDeploymentAfter()
//        {
//            DateTime later = new DateTime((DateTimeUtil.Now().Millisecond + (10 * 3600)));
//            DateTime earlier = new DateTime((DateTimeUtil.Now().Millisecond - (10 * 3600)));

//            long Count = repositoryService.CreateDeploymentQuery().DeploymentAfter(later).Count();
//		Assert.AreEqual(0, Count);

//		Count = repositoryService.CreateDeploymentQuery().DeploymentAfter(earlier).Count();
//		Assert.AreEqual(2, Count);

//		try
//		{
//		  repositoryService.CreateDeploymentQuery().DeploymentAfter(DateTime.Now);
//		  Assert.Fail("Exception expected");
//		}
//		catch (NullValueException)
//		{
//		  // expected
//		}
//	  }

//	   [Test]   public virtual void testQueryBySource()
//	  {
//		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery().DeploymentSource(ProcessApplicationDeploymentFields.ProcessApplicationDeploymentSource);

//		Assert.AreEqual(1, query.Count());
//		Assert.AreEqual(1, query.Count());
//	  }

//	   [Test]   public virtual void testQueryByNullSource()
//	  {
//		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery().DeploymentSource(null);

//		Assert.AreEqual(1, query.Count());
//		Assert.AreEqual(1, query.Count());
//	  }

//	   [Test]   public virtual void testQueryByInvalidSource()
//	  {
//		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery().DeploymentSource("invalid");

//		Assert.AreEqual(0, query.Count());
//		Assert.AreEqual(0, query.Count());
//	  }

////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: public void testQueryDeploymentBetween() throws Exception
//	   [Test]   public virtual void testQueryDeploymentBetween()
//        {
//            DateTime later = new DateTime((DateTimeUtil.Now().Millisecond + (10 * 3600)));
//            DateTime earlier = new DateTime((DateTimeUtil.Now().Millisecond - (10 * 3600)));

//            long Count = repositoryService.CreateDeploymentQuery().DeploymentAfter(earlier).DeploymentBefore(later).Count();
//		Assert.AreEqual(2, Count);

//		Count = repositoryService.CreateDeploymentQuery().DeploymentAfter(later).DeploymentBefore(later).Count();
//		Assert.AreEqual(0, Count);

//		Count = repositoryService.CreateDeploymentQuery().DeploymentAfter(earlier).DeploymentBefore(earlier).Count();
//		Assert.AreEqual(0, Count);

//		Count = repositoryService.CreateDeploymentQuery().DeploymentAfter(later).DeploymentBefore(earlier).Count();
//		Assert.AreEqual(0, Count);
//	  }

//	   [Test]   public virtual void testVerifyDeploymentProperties()
//	   {
//	       IList<IDeployment> deployments = repositoryService.CreateDeploymentQuery()
//	           .OrderByDeploymentName()
//	           /*.Asc()*/
//	           .ToList();

//		IDeployment deploymentOne = deployments[0];
//		Assert.AreEqual("resources/repository/one.bpmn20.xml", deploymentOne.Name);
//		Assert.AreEqual(deploymentOneId, deploymentOne.Id);
//		Assert.AreEqual(ProcessApplicationDeploymentFields.ProcessApplicationDeploymentSource, deploymentOne.Source);
//		Assert.IsNull(deploymentOne.TenantId);

//		IDeployment deploymentTwo = deployments[1];
//		Assert.AreEqual("resources/repository/two_.bpmn20.xml", deploymentTwo.Name);
//		Assert.AreEqual(deploymentTwoId, deploymentTwo.Id);
//		Assert.IsNull(deploymentTwo.Source);
//		Assert.IsNull(deploymentTwo.TenantId);
//	  }

//	   [Test]   public virtual void testQuerySorting()
//	  {
//		Assert.AreEqual(2, repositoryService.CreateDeploymentQuery().OrderByDeploymentName()/*.Asc()*/.Count());

//		Assert.AreEqual(2, repositoryService.CreateDeploymentQuery().OrderByDeploymentId()/*.Asc()*/.Count());

//		Assert.AreEqual(2, repositoryService.CreateDeploymentQuery().OrderByDeploymenTime()/*.Asc()*/.Count());

//		Assert.AreEqual(2, repositoryService.CreateDeploymentQuery().OrderByDeploymenTime()/*.Asc()*/.Count());
//	  }

//	}

//}