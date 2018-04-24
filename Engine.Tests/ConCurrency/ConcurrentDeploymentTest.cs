using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Repository.Impl;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.ConCurrency
{
    [TestFixture]
    public class ConcurrentDeploymentTest : ConcurrencyTestCase
    {

        private static string processResource;

        static ConcurrentDeploymentTest()
        {
            IBpmnModelInstance modelInstance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess().StartEvent().Done();
            System.IO.MemoryStream outputStream = new System.IO.MemoryStream();
            ESS.FW.Bpm.Model.Bpmn.Bpmn.WriteModelToStream(outputStream, modelInstance);
            processResource = StringHelperClass.NewString(outputStream.ToArray());
        }


        // Todo Parent Class->TestCase
        //protected internal override void runTest()
        //{
        //    string databaseType = processEngineConfiguration.DbSqlSessionFactory.DatabaseType;

        //    if ("h2".Equals(databaseType))
        //    {
        //        // skip test method - if database is H2
        //    }
        //    else
        //    {
        //        // invoke the test method
        //        base.RunTest();
        //    }
        //}

        [Test]
        public virtual void testDuplicateFiltering()
        {

            deployOnTwoConcurrentThreads(createDeploymentBuilder().EnableDuplicateFiltering(false), createDeploymentBuilder().EnableDuplicateFiltering(false));

            // ensure that although both transactions were run concurrently, only one deployment was constructed.
             IQueryable<IDeployment> deploymentQuery = repositoryService.CreateDeploymentQuery();
            Assert.That(deploymentQuery.Count(), Is.EqualTo(1L));
        }

        [Test]
        public virtual void testVersioning()
        {

            deployOnTwoConcurrentThreads(createDeploymentBuilder(), createDeploymentBuilder());

            // ensure that although both transactions were run concurrently, the process definitions have different versions
            IList<IProcessDefinition> processDefinitions = repositoryService.CreateProcessDefinitionQuery()
                //.OrderByProcessDefinitionVersion()
                /*.Asc()*/
                .ToList();

            Assert.That(processDefinitions.Count, Is.EqualTo(2));
            Assert.That(processDefinitions[0].Version, Is.EqualTo(1));
            Assert.That(processDefinitions[1].Version, Is.EqualTo(2));
        }

        protected internal virtual IDeploymentBuilder createDeploymentBuilder()
        {
            return (new DeploymentBuilderImpl(null)).Name("some-deployment-name").AddString("foo.bpmn", processResource);
        }

        protected internal virtual void deployOnTwoConcurrentThreads(IDeploymentBuilder deploymentOne, IDeploymentBuilder deploymentTwo)
        {
            Assert.That(deploymentOne, Is.Not.EqualTo(deploymentTwo));

            // STEP 1: bring two threads to a point where they have
            // 1) started a new transaction
            // 2) are ready to deploy
            ThreadControl thread1 = ExecuteControllableCommand(new ControllableDeployCommand(deploymentOne));
            thread1.WaitForSync();

            ThreadControl thread2 = ExecuteControllableCommand(new ControllableDeployCommand(deploymentTwo));
            thread2.WaitForSync();

            // STEP 2: make Thread 1 proceed and wait until it has deployed but not yet committed
            // -> will still hold the exclusive lock
            thread1.MakeContinue();
            thread1.WaitForSync();

            // STEP 3: make Thread 2 continue
            // -> it will attempt to acquire the exclusive lock and block on the lock
            thread2.MakeContinue();

            // wait for 2 seconds (Thread 2 is blocked on the lock)
            Thread.Sleep(2000);

            // STEP 4: allow Thread 1 to terminate
            // -> Thread 1 will commit and release the lock
            thread1.WaitUntilDone();

            // STEP 5: wait for Thread 2 to terminate
            thread2.WaitForSync();
            thread2.WaitUntilDone();
        }

        [TearDown]
        protected internal void tearDown()
        {

            foreach (IDeployment deployment in repositoryService.CreateDeploymentQuery().ToList())
            {
                repositoryService.DeleteDeployment(deployment.Id, true);
            }
        }

        protected internal class ControllableDeployCommand : ControllableCommand<object>
        {

            internal readonly IDeploymentBuilder deploymentBuilder;

            public ControllableDeployCommand(IDeploymentBuilder deploymentBuilder)
            {
                this.deploymentBuilder = deploymentBuilder;
            }

            public override object Execute(CommandContext commandContext)
            {
                Monitor.Sync(); // thread will block here until makeContinue() is called form main thread

                (new DeployCmd((DeploymentBuilderImpl)deploymentBuilder)).Execute(commandContext);

                Monitor.Sync(); // thread will block here until waitUntilDone() is called form main thread

                return null;
            }

        }

    }

}