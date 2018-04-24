using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using NUnit.Framework;

namespace Engine.Tests.Api.Cfg
{


    public class DatabaseHistoryPropertyAutoTest
    {

        protected internal IList<ProcessEngineImpl> processEngines = new List<ProcessEngineImpl>();

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public final org.junit.Rules.ExpectedException thrown = org.junit.Rules.ExpectedException.None();
        //public readonly ExpectedException thrown = ExpectedException.None();

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: private static ProcessEngineConfigurationImpl config(final String historyLevel)
        private ProcessEngineConfigurationImpl config(string historyLevel)
        {

            return config("false", historyLevel);
        }

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: private static ProcessEngineConfigurationImpl config(final String schemaUpdate, final String historyLevel)
        private ProcessEngineConfigurationImpl config(string schemaUpdate, string historyLevel)
        {
            StandaloneInMemProcessEngineConfiguration engineConfiguration = new StandaloneInMemProcessEngineConfiguration();
            engineConfiguration.ProcessEngineName = Guid.NewGuid().ToString();// UUID.RandomUUID().ToString();
            engineConfiguration.DatabaseSchemaUpdate = schemaUpdate;
            engineConfiguration.History = historyLevel;
            engineConfiguration.DbMetricsReporterActivate = false;
            engineConfiguration.JdbcUrl = "jdbc:h2:mem:DatabaseHistoryPropertyAutoTest";

            return engineConfiguration;
        }

        [Test]
        public virtual void failWhenSecondEngineDoesNotHaveTheSameHistoryLevel()
        {
            buildEngine(config("true", ProcessEngineConfiguration.HistoryFull));
            // Todo: junit ExpectedException
            //thrown.Expect(typeof(ProcessEngineException));
            //thrown.ExpectMessage("historyLevel mismatch: configuration says HistoryLevelAudit(name=audit, id=2) and database says HistoryLevelFull(name=full, id=3)");

            buildEngine(config(ProcessEngineConfiguration.HistoryAudit));
        }

        [Test]
        public virtual void secondEngineCopiesHistoryLevelFromFirst()
        {
            // given
            buildEngine(config("true", ProcessEngineConfiguration.HistoryFull));

            // when
            ProcessEngineImpl processEngineTwo = buildEngine(config("true", ProcessEngineConfiguration.HistoryAuto));

            // then
            Assert.That(processEngineTwo.ProcessEngineConfiguration.History, Is.EqualTo(ProcessEngineConfiguration.HistoryAuto));
            Assert.That((processEngineTwo.ProcessEngineConfiguration as ProcessEngineConfigurationImpl).HistoryLevel, Is.EqualTo(HistoryLevelFields.HistoryLevelFull));

        }

        [Test]
        public virtual void usesDefaultValueAuditWhenNoValueIsConfigured()
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ProcessEngineConfigurationImpl config = config("true", org.Camunda.bpm.Engine.ProcessEngineConfiguration.HISTORY_AUTO);
            ProcessEngineConfigurationImpl config = this.config("true", ProcessEngineConfiguration.HistoryAuto);
            ProcessEngineImpl processEngine = buildEngine(config);

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final Nullable<int> level = config.GetCommandExecutorSchemaOperations().Execute(new org.Camunda.bpm.Engine.impl.interceptor.Command<Nullable<int>>()
            int? level = config.CommandExecutorSchemaOperations.Execute(new CommandAnonymousInnerClass(this));

            Assert.That(level, Is.EqualTo(HistoryLevelFields.HistoryLevelAudit.Id));

            Assert.That((processEngine.ProcessEngineConfiguration as ProcessEngineConfigurationImpl).HistoryLevel, Is.EqualTo(HistoryLevelFields.HistoryLevelAudit));
        }

        private class CommandAnonymousInnerClass : ICommand<int?>
        {
            private readonly DatabaseHistoryPropertyAutoTest outerInstance;

            public CommandAnonymousInnerClass(DatabaseHistoryPropertyAutoTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public int? Execute(CommandContext commandContext)
            {
                return SchemaOperationsProcessEngineBuild.DatabaseHistoryLevel(commandContext);
            }

        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @After public void after()
        public virtual void after()
        {
            foreach (ProcessEngineImpl engine in processEngines)
            {
                // no need to drop schema when testing with h2
                engine.Close();
            }

            processEngines.Clear();
        }

        protected internal virtual ProcessEngineImpl buildEngine(ProcessEngineConfigurationImpl engineConfiguration)
        {
            ProcessEngineImpl engine = (ProcessEngineImpl)engineConfiguration.BuildProcessEngine();
            processEngines.Add(engine);

            return engine;
        }

    }

}