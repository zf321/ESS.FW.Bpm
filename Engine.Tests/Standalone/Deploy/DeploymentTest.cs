using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Deploy
{
    [TestFixture]
    public class DeploymentTest: ProcessEngineTestRule
    {
        private readonly bool _instanceFieldsInitialized;

        public DeploymentTest()
        {
            engineRule = new ProvidedProcessEngineRule(BootstrapRule);

            if (!_instanceFieldsInitialized)
            {
                InitializeInstanceFields();
                _instanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            //TestHelper = new ProcessEngineTestRule(engineRule);
            //////ruleChain = RuleChain.outerRule(engineRule).around(testHelper);
        }


        //protected internal ProvidedProcessEngineRule EngineRule; // = new ProvidedProcessEngineRule(bootstrapRule);
        //protected internal ProcessEngineTestRule TestHelper;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static test.util.ProcessEngineBootstrapRule bootstrapRule = new test.util.ProcessEngineBootstrapRule()
        public ProcessEngineBootstrapRule BootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

        private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
        {
            public override ProcessEngineConfiguration ConfigureEngine(ProcessEngineConfigurationImpl configuration)
            {
                //configuration.JdbcUrl = "jdbc:h2:mem:DeploymentTest-HistoryLevelNone;DB_CLOSE_DELAY=1000";
                //configuration.DatabaseSchemaUpdate = ProcessEngineConfiguration.DbSchemaUpdateCreateDrop;
                configuration.HistoryLevel = HistoryLevelFields.HistoryLevelNone;
                configuration.DbHistoryUsed = false;
                return configuration;
            }
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testHelper);
        ////public RuleChain ruleChain;


        [Test]
        public virtual void ShouldDeleteDeployment()
        {
            var instance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().EndEvent().Done();

            var deployment =
                engineRule.RepositoryService.CreateDeployment()
                    .AddModelInstance("foo.bpmn", instance)
                    .DeployAndReturnDefinitions();

            engineRule.RepositoryService.DeleteDeployment(deployment.Id, true);

            var count = engineRule.RepositoryService.CreateDeploymentQuery(c=>c.Id == deployment.Id).Count();
            Assert.AreEqual(0, count);
        }
    }
}