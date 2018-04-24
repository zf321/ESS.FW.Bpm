using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Impl.Util.xml;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    /// <summary>
    ///     Represents a test class, which uses parse listeners
    ///     to Delete job definitions for asyncBefore activities and create job
    ///     definitions for asyncAfter.
    ///     The parse listeners are called after the bpmn xml was parsed.
    ///     They set the activity asyncBefore property to false and the asyncAfter to true.
    ///     In this case there should Delete and create some job declarations for the activity which
    ///     was asyncBefore before and are now asyncAfter.
    /// </summary>
    [TestFixture]
    public class JobDefinitionCreationAndDeletionWithParseListenerTest
    {
        [SetUp]
        public void SetUpEngineRule()
        {
            if (EngineRule.ProcessEngine == null)
                EngineRule.InitializeProcessEngine();

            EngineRule.InitializeServices();

            EngineRule.Starting();
        }

        [TearDown]
        public void TearDownEngineRule()
        {
            BootstrapRule.Finished();
        }

        private readonly bool _instanceFieldsInitialized;

        public JobDefinitionCreationAndDeletionWithParseListenerTest()
        {
            if (!_instanceFieldsInitialized)
            {
                InitializeInstanceFields();
                _instanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            BootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();
            EngineRule = new ProvidedProcessEngineRule(BootstrapRule);
        }


        /// <summary>
        ///     The custom rule which adjust the process engine configuration.
        /// </summary>
        protected internal ProcessEngineBootstrapRule BootstrapRule;
            

        private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
        {
            public override ProcessEngineConfiguration ConfigureEngine(ProcessEngineConfigurationImpl configuration)
            {
                IList<IBpmnParseListener> listeners = new List<IBpmnParseListener>();
                listeners.Add(new AbstractBpmnParseListenerAnonymousInnerClass());

               configuration.CustomPreBpmnParseListeners = listeners;
                return configuration;
            }

            private class AbstractBpmnParseListenerAnonymousInnerClass : AbstractBpmnParseListener
            {
                //private readonly ProcessEngineBootstrapRuleAnonymousInnerClass _outerInstance;

                //public AbstractBpmnParseListenerAnonymousInnerClass(
                //    ProcessEngineBootstrapRuleAnonymousInnerClass outerInstance)
                //{
                //    _outerInstance = outerInstance;
                //}


                public override void ParseServiceTask(Element serviceTaskElement, ScopeImpl scope, ActivityImpl activity)
                {
                    activity.AsyncBefore = false;
                    activity.AsyncAfter = true;
                }
            }
        }

        /// <summary>
        ///     The engine rule.
        /// </summary>
        protected internal ProcessEngineRule EngineRule;

        /// <summary>
        ///     The rule chain for the defined rules.
        /// </summary>
        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(bootstrapRule).around(engineRule);
        ////public RuleChain ruleChain;

        [Test]
        public virtual void TestDeleteNonExistingAndCreateNewJobDefinitionWithParseListener()
        {
            //given
            //var modelFileName = "jobexecutor/jobCreationWithinParseListener.bpmn20.xml";
            //var @in = ReflectUtil.GetResourceAsStream(modelFileName);
            //var builder = EngineRule.RepositoryService.CreateDeployment().AddInputStream(modelFileName, @in);
            //var deployment = builder.Deploy();

            var deployment = EngineRule.RepositoryService.CreateDeployment()
                .Name(ClassNameUtil.GetClassNameWithoutPackage(this.GetType()) + "." + TestContext.CurrentContext.Test.Name)
                .AddClasspathResource("resources/jobexecutor/jobCreationWithinParseListener.bpmn20.xml").Deploy();
            //when the asyncBefore is set to false and the asyncAfter to true in the parse listener
            
            EngineRule.ManageDeployment(deployment);

            //then there exists one job definition
            var query = EngineRule.ManagementService.CreateJobDefinitionQuery();
            var jobDef = query.First();
            Assert.NotNull(jobDef);
            Assert.AreEqual(jobDef.ProcessDefinitionKey, "oneTaskProcess");
            Assert.AreEqual(jobDef.ActivityId, "servicetask1");
            Assert.AreEqual(jobDef.JobConfiguration, MessageJobDeclaration.AsyncAfter);
        }

        [Test]
        public virtual void TestDeleteJobDefinitionWithParseListenerAndAsyncInXml()
        {
            //given the asyncBefore is set in the xml
            //var modelFileName = "jobexecutor/jobAsyncBeforeCreationWithinParseListener.bpmn20.xml";
            //var @in = ReflectUtil.GetResourceAsStream(modelFileName);
            //var builder = EngineRule.RepositoryService.CreateDeployment().AddInputStream(modelFileName, @in);
            ////when the asyncBefore is set to false and the asyncAfter to true in the parse listener
            //var deployment = builder.Deploy();
            var deployment = EngineRule.RepositoryService.CreateDeployment()
                .Name(ClassNameUtil.GetClassNameWithoutPackage(this.GetType()) + "." + TestContext.CurrentContext.Test.Name)
                .AddClasspathResource("resources/jobexecutor/jobAsyncBeforeCreationWithinParseListener.bpmn20.xml").Deploy();
            EngineRule.ManageDeployment(deployment);

            //then there exists one job definition
            var query = EngineRule.ManagementService.CreateJobDefinitionQuery();
            var jobDef = query.First();
            Assert.NotNull(jobDef);
            Assert.AreEqual(jobDef.ProcessDefinitionKey, "oneTaskProcess");
            Assert.AreEqual(jobDef.ActivityId, "servicetask1");
            Assert.AreEqual(jobDef.JobConfiguration, MessageJobDeclaration.AsyncAfter);
        }

        [Test]
        public virtual void TestDeleteJobDefinitionWithParseListenerAndAsyncBothInXml()
        {
            //given the asyncBefore AND asyncAfter is set in the xml
            //var modelFileName = "jobexecutor/jobAsyncBothCreationWithinParseListener.bpmn20.xml";
            //var @in = ReflectUtil.GetResourceAsStream(modelFileName);
            //var builder = EngineRule.RepositoryService.CreateDeployment().AddInputStream(modelFileName, @in);

            ////when the asyncBefore is set to false and the asyncAfter to true in the parse listener
            //var deployment = builder.Deploy();

            var deployment = EngineRule.RepositoryService.CreateDeployment()
                .Name(ClassNameUtil.GetClassNameWithoutPackage(this.GetType()) + "." + TestContext.CurrentContext.Test.Name)
                .AddClasspathResource("resources/jobexecutor/jobAsyncBothCreationWithinParseListener.bpmn20.xml").Deploy();

            EngineRule.ManageDeployment(deployment);

            //then there exists one job definition
            var query = EngineRule.ManagementService.CreateJobDefinitionQuery();
            var jobDef = query.First();
            Assert.NotNull(jobDef);
            Assert.AreEqual(jobDef.ProcessDefinitionKey, "oneTaskProcess");
            Assert.AreEqual(jobDef.ActivityId, "servicetask1");
            Assert.AreEqual(jobDef.JobConfiguration, MessageJobDeclaration.AsyncAfter);
        }
    }
}