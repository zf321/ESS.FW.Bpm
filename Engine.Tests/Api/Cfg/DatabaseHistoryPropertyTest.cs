using System.Collections.Generic;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using NUnit.Framework;

namespace Engine.Tests.Api.Cfg
{

    /// <summary>
	/// @author Christian Lipphardt
	/// </summary>
	public class DatabaseHistoryPropertyTest
    {


        private static ProcessEngineImpl processEngineImpl;

        // make sure schema is dropped
        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @After public void cleanup()
        public virtual void cleanup()
        {
            TestHelper.DropSchema(processEngineImpl.ProcessEngineConfiguration as ProcessEngineConfigurationImpl);
            processEngineImpl.Close();
            processEngineImpl = null;
        }

        [Test]
        public virtual void schemaCreatedByEngineAndDatabaseSchemaUpdateTrue()
        {
            processEngineImpl = createProcessEngineImpl("true", true);

            AssertHistoryLevel();
        }

        [Test]
        public virtual void schemaCreatedByUserAndDatabaseSchemaUpdateTrue()
        {
            processEngineImpl = createProcessEngineImpl("true", false);
            // simulate manual schema creation by user
            TestHelper.CreateSchema(processEngineImpl.ProcessEngineConfiguration as ProcessEngineConfigurationImpl);

            // let the engine do their schema operations thing
            (processEngineImpl.ProcessEngineConfiguration as ProcessEngineConfigurationImpl).CommandExecutorSchemaOperations.Execute(new SchemaOperationsProcessEngineBuild());

            AssertHistoryLevel();
        }

        [Test]
        public virtual void schemaCreatedByUserAndDatabaseSchemaUpdateFalse()
        {
            processEngineImpl = createProcessEngineImpl("false", false);
            // simulate manual schema creation by user
            TestHelper.CreateSchema(processEngineImpl.ProcessEngineConfiguration as ProcessEngineConfigurationImpl);

            // let the engine do their schema operations thing
            (processEngineImpl.ProcessEngineConfiguration as ProcessEngineConfigurationImpl).CommandExecutorSchemaOperations.Execute(new SchemaOperationsProcessEngineBuild());

            AssertHistoryLevel();
        }

        private void AssertHistoryLevel()
        {
            IDictionary<string, string> properties = processEngineImpl.ManagementService.Properties;
            string historyLevel = properties["historyLevel"];
            Assert.NotNull("historyLevel is null -> not set in database", historyLevel);
            Assert.AreEqual(ProcessEngineConfigurationImpl.HistoryFull, int.Parse(historyLevel));
        }


        //----------------------- TEST HELPERS -----------------------

        private class CreateSchemaProcessEngineImpl : ProcessEngineImpl
        {
            public CreateSchemaProcessEngineImpl(ProcessEngineConfigurationImpl processEngineConfiguration) : base(processEngineConfiguration)
            {
            }

            protected internal virtual void executeSchemaOperations()
            {
                base.ExecuteSchemaOperations();
            }
        }

        private class CreateNoSchemaProcessEngineImpl : ProcessEngineImpl
        {
            public CreateNoSchemaProcessEngineImpl(ProcessEngineConfigurationImpl processEngineConfiguration) : base(processEngineConfiguration)
            {
            }

            protected internal virtual void executeSchemaOperations()
            {
                // nop - do not execute create schema operations
            }
        }

        // allows to return a process engine configuration which doesn't create a schema when it's build.
        private class CustomStandaloneInMemProcessEngineConfiguration : StandaloneInMemProcessEngineConfiguration
        {

            internal bool executeSchemaOperations;

            public virtual IProcessEngine buildProcessEngine()
            {
                Init();
                if (executeSchemaOperations)
                {
                    return new CreateSchemaProcessEngineImpl(this);
                }
                else
                {
                    return new CreateNoSchemaProcessEngineImpl(this);
                }
            }

            public virtual ProcessEngineConfigurationImpl SetExecuteSchemaOperations(bool executeSchemaOperations)
            {
                this.executeSchemaOperations = executeSchemaOperations;
                return this;
            }
        }

        private static ProcessEngineImpl createProcessEngineImpl(string databaseSchemaUpdate, bool executeSchemaOperations)
        {
            ProcessEngineImpl processEngine = (ProcessEngineImpl)(new CustomStandaloneInMemProcessEngineConfiguration()).SetExecuteSchemaOperations(executeSchemaOperations).SetProcessEngineName("database-history-test-engine").SetDatabaseSchemaUpdate(databaseSchemaUpdate).SetHistory(ProcessEngineConfiguration.HistoryFull).SetJdbcUrl("jdbc:h2:mem:DatabaseHistoryPropertyTest").BuildProcessEngine();

            return processEngine;
        }

    }

}