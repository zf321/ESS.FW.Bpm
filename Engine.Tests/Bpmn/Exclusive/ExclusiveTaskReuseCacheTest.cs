
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Exclusive
{
    [TestFixture]
    public class ExclusiveTaskReuseCacheTest : ExclusiveTaskTest
    {

        [SetUp]
        protected internal void setUp()
        {
            base.RunBare();
            processEngineConfiguration.DbEntityCacheReuseEnabled = true;
        }

        [TearDown]
        protected internal void tearDown()
        {
            processEngineConfiguration.DbEntityCacheReuseEnabled = false;
            base.TearDown();
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/exclusive/ExclusiveTaskTest.TestNonExclusiveService.bpmn20.xml" })]
        public override void TestNonExclusiveService()
        {
            base.TestNonExclusiveService();
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/exclusive/ExclusiveTaskTest.TestExclusiveService.bpmn20.xml" })]
        public override void TestExclusiveService()
        {
            base.TestExclusiveService();
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/exclusive/ExclusiveTaskTest.TestExclusiveServiceConcurrent.bpmn20.xml" })]
        public override void TestExclusiveServiceConcurrent()
        {
            base.TestExclusiveServiceConcurrent();
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/exclusive/ExclusiveTaskTest.TestExclusiveSequence2.bpmn20.xml" })]
        public override void TestExclusiveSequence2()
        {
            base.TestExclusiveSequence2();
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/exclusive/ExclusiveTaskTest.TestExclusiveSequence3.bpmn20.xml" })]
        public override void TestExclusiveSequence3()
        {
            base.TestExclusiveSequence3();
        }

    }

}