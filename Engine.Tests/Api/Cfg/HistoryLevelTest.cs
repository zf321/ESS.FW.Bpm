using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using NUnit.Framework;

namespace Engine.Tests.Api.Cfg
{

    public class HistoryLevelTest
    {

        protected internal IProcessEngine processEngine;

        [Test]
        public virtual void shouldInitHistoryLevelByObject()
        {
            ProcessEngineConfigurationImpl config = createConfig();
            config.HistoryLevel = HistoryLevelFields.HistoryLevelFull;

            ProcessEngineConfigurationImpl processEngineConfiguration = buildProcessEngine(config);

            Assert.That(processEngineConfiguration.HistoryLevels.Count, Is.EqualTo(4));
            Assert.That(processEngineConfiguration.HistoryLevel, Is.EqualTo(HistoryLevelFields.HistoryLevelFull));
            Assert.That(processEngineConfiguration.History, Is.EqualTo(HistoryLevelFields.HistoryLevelFull.Name));
        }

        [Test]
        public virtual void shouldInitHistoryLevelByString()
        {
            ProcessEngineConfigurationImpl config = createConfig();
            config.History = HistoryLevelFields.HistoryLevelFull.Name;

            ProcessEngineConfigurationImpl processEngineConfiguration = buildProcessEngine(config);

            Assert.That(processEngineConfiguration.HistoryLevels.Count, Is.EqualTo(4));
            Assert.That(processEngineConfiguration.HistoryLevel, Is.EqualTo(HistoryLevelFields.HistoryLevelFull));
            Assert.That(processEngineConfiguration.History, Is.EqualTo(HistoryLevelFields.HistoryLevelFull.Name));
        }

        protected internal virtual ProcessEngineConfigurationImpl createConfig()
        {
            StandaloneInMemProcessEngineConfiguration configuration = new StandaloneInMemProcessEngineConfiguration();
            configuration.ProcessEngineName = "process-engine-HistoryTest";
            configuration.DbMetricsReporterActivate = false;
            configuration.JdbcUrl = "jdbc:h2:mem:HistoryTest";
            return configuration;
        }

        protected internal virtual ProcessEngineConfigurationImpl buildProcessEngine(ProcessEngineConfigurationImpl config)
        {
            processEngine = config.BuildProcessEngine();

            return (ProcessEngineConfigurationImpl)processEngine.ProcessEngineConfiguration;
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @After public void closeEngine()
        public virtual void closeEngine()
        {
            processEngine.Close();
        }

    }
}