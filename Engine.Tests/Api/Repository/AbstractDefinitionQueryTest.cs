using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Repository;
using NUnit.Framework;

namespace Engine.Tests.Api.Repository
{

    [TestFixture]
    public abstract class AbstractDefinitionQueryTest : PluggableProcessEngineTestCase
    {

        protected internal string deploymentOneId;
        protected internal string deploymentTwoId;


        [SetUp]
        protected internal void setUp()
        {
            deploymentOneId = repositoryService.CreateDeployment().Name("firstDeployment").AddClasspathResource(ResourceOnePath).AddClasspathResource(ResourceTwoPath).Deploy().Id;

            deploymentTwoId = repositoryService.CreateDeployment().Name("secondDeployment").AddClasspathResource(ResourceOnePath).Deploy().Id;

            //base.setUp();
        }

        protected internal abstract string ResourceOnePath { get; }

        protected internal abstract string ResourceTwoPath { get; }

        [TearDown]
        protected internal void tearDown()
        {
            base.TearDown();
            repositoryService.DeleteDeployment(deploymentOneId, true);
            repositoryService.DeleteDeployment(deploymentTwoId, true);
        }

        //protected internal virtual void verifyQueryResults(IQueryable<ICaseDefinition> query, int countExpected)
        //{
        //    Assert.AreEqual(countExpected, query.Count());
        //    Assert.AreEqual(countExpected, query.Count());

        //    if (countExpected == 1)
        //    {
        //        Assert.NotNull(query.First());
        //    }
        //    else if (countExpected > 1)
        //    {
        //        verifySingleResultFails(query);
        //    }
        //    else if (countExpected == 0)
        //    {
        //        Assert.IsNull(query.First());
        //    }
        //}
        protected internal virtual void verifyQueryResults(IQueryable<IProcessDefinition> query, int countExpected)
        {
            Assert.AreEqual(countExpected, query.Count());
            Assert.AreEqual(countExpected, query.Count());

            if (countExpected == 1)
            {
                Assert.NotNull(query.First());
            }
            else if (countExpected > 1)
            {
                verifySingleResultFails(query);
            }
            else if (countExpected == 0)
            {
                Assert.IsNull(query.First());
            }
        }

        private void verifySingleResultFails(IQueryable<IProcessDefinition> query)
        {
            try
            {
                query.First();
                Assert.Fail("Exception expected");
            }
            catch (ProcessEngineException)
            {
                // expected exception
            }
        }
        //private void verifySingleResultFails(IQueryable<ICaseDefinition> query)
        //{
        //    try
        //    {
        //        query.First();
        //        Assert.Fail("Exception expected");
        //    }
        //    catch (ProcessEngineException)
        //    {
        //        // expected exception
        //    }
        //}
    }

}