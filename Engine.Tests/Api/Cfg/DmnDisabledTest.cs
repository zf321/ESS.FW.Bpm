using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using NUnit.Framework;

namespace Engine.Tests.Api.Cfg
{

    /// <summary>
    /// 
    /// 
    /// </summary>
    public class DmnDisabledTest
    {

        protected internal static ProcessEngineImpl processEngineImpl;

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
        public virtual void disabledDmn()
        {
            processEngineImpl = createProcessEngineImpl(false);

            // simulate manual schema creation by user
            TestHelper.CreateSchema(processEngineImpl.ProcessEngineConfiguration as ProcessEngineConfigurationImpl);

            // let the engine do their schema operations thing
            (processEngineImpl.ProcessEngineConfiguration as ProcessEngineConfigurationImpl).CommandExecutorSchemaOperations.Execute(new SchemaOperationsProcessEngineBuild());
        }

        // allows to return a process engine configuration which doesn't create a schema when it's build.
        protected internal class CustomStandaloneInMemProcessEngineConfiguration : StandaloneInMemProcessEngineConfiguration
        {

            public virtual IProcessEngine buildProcessEngine()
            {
                Init();
                return new CreateNoSchemaProcessEngineImpl(this);
            }
        }

        protected internal class CreateNoSchemaProcessEngineImpl : ProcessEngineImpl
        {

            public CreateNoSchemaProcessEngineImpl(ProcessEngineConfigurationImpl processEngineConfiguration) : base(processEngineConfiguration)
            {
            }

            protected internal virtual void executeSchemaOperations()
            {
                // noop - do not execute create schema operations
            }
        }

        protected internal static ProcessEngineImpl createProcessEngineImpl(bool dmnEnabled)
        {
            StandaloneInMemProcessEngineConfiguration config = (StandaloneInMemProcessEngineConfiguration)(new CustomStandaloneInMemProcessEngineConfiguration()).SetProcessEngineName("database-dmn-test-engine").SetDatabaseSchemaUpdate("false").SetHistory(ProcessEngineConfiguration.HistoryFull).SetJdbcUrl("jdbc:h2:mem:DatabaseDmnTest");

            config.DmnEnabled = dmnEnabled;

            return (ProcessEngineImpl)config.BuildProcessEngine();
        }

    }

}