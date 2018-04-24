using System.Collections.Generic;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using NUnit.Framework;

namespace Engine.Tests.Standalone.History
{
    [TestFixture]
    public class DetermineHistoryLevelCmdTest
    {
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public final org.junit.Rules.ExpectedException thrown = org.junit.Rules.ExpectedException.None();
        //public readonly ExpectedException Thrown = ExpectedException.None();

        private ProcessEngineImpl _processEngineImpl;

        
        private static ProcessEngineConfigurationImpl Config(string historyLevel)
        {
            return Config("false", historyLevel);
        }
        
        private static ProcessEngineConfigurationImpl Config(string schemaUpdate, string historyLevel)
        {
            var engineConfiguration = new StandaloneInMemProcessEngineConfiguration();
            //engineConfiguration.ProcessEngineName = Guid.NewGuid().ToString();
            //engineConfiguration.DatabaseSchemaUpdate = schemaUpdate;
            //engineConfiguration.History = historyLevel;
            //engineConfiguration.DbMetricsReporterActivate = false;
            //engineConfiguration.JdbcUrl = "jdbc:h2:mem:DatabaseHistoryPropertyAutoTest";

            return engineConfiguration;
        }

        [Test]
        public virtual void ReadLevelFullfromDb()
        {
            ProcessEngineConfigurationImpl config = Config("true", ProcessEngineConfiguration.HistoryFull);

            // init the db with level=full
            _processEngineImpl = (ProcessEngineImpl) config.BuildProcessEngine();

            var historyLevel =
                config.CommandExecutorSchemaOperations.Execute(new DetermineHistoryLevelCmd(config.HistoryLevels));

            Assert.That(historyLevel, Is.EqualTo(HistoryLevelFields.HistoryLevelFull));
        }


        [Test]
        public virtual void UseDefaultLevelAudit()
        {
            ProcessEngineConfigurationImpl config = Config("true", ProcessEngineConfiguration.HistoryAuto);

            // init the db with level=auto -> audit
            _processEngineImpl = (ProcessEngineImpl) config.BuildProcessEngine();
            // the history Level has been overwritten with audit
            Assert.That(config.HistoryLevel, Is.EqualTo(HistoryLevelFields.HistoryLevelAudit));

            // and this is written to the database
            var databaseLevel =
                config.CommandExecutorSchemaOperations.Execute(new DetermineHistoryLevelCmd(config.HistoryLevels));
            Assert.That(databaseLevel, Is.EqualTo(HistoryLevelFields.HistoryLevelAudit));
        }

        [Test]
        public virtual void FailWhenExistingHistoryLevelIsNotRegistered()
        {
            // init the db with custom level
            IHistoryLevel customLevel = new HistoryLevelAnonymousInnerClass(this);
            ProcessEngineConfigurationImpl config = Config("true", "custom");
            config.SetCustomHistoryLevels( new List<IHistoryLevel> {customLevel});
            _processEngineImpl = (ProcessEngineImpl) config.BuildProcessEngine();

            //Thrown.Expect(typeof(ProcessEngineException));
            //Thrown.ExpectMessage("The configured history level with id='99' is not registered in this config.");

            config.CommandExecutorSchemaOperations.Execute(new DetermineHistoryLevelCmd(new List<IHistoryLevel>()));
        }
        [TearDown]
        public virtual void After()
        {
            //TestHelper.DropSchema(_processEngineImpl.ProcessEngineConfiguration);
            _processEngineImpl.Close();
            _processEngineImpl = null;
        }

        private class HistoryLevelAnonymousInnerClass : IHistoryLevel
        {
            private readonly DetermineHistoryLevelCmdTest _outerInstance;

            public HistoryLevelAnonymousInnerClass(DetermineHistoryLevelCmdTest outerInstance)
            {
                _outerInstance = outerInstance;
            }

            public int Id
            {
                get { return 99; }
            }

            public string Name
            {
                get { return "custom"; }
            }

            public bool IsHistoryEventProduced(HistoryEventTypes eventType, object entity)
            {
                return false;
            }
        }
    }
}