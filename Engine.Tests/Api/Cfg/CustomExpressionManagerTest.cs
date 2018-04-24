using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.EL;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using NUnit.Framework;

namespace Engine.Tests.Api.Cfg
{

    /// <summary>
	/// 
	/// </summary>
	public class CustomExpressionManagerTest
    {

        protected internal IProcessEngine engine;

        [Test]
        public virtual void testBuiltinFunctionMapperRegistration()
        {
            // given a process engine configuration with a custom function mapper
            ProcessEngineConfigurationImpl config = (ProcessEngineConfigurationImpl)ProcessEngineConfiguration.CreateStandaloneInMemProcessEngineConfiguration().SetJdbcUrl("jdbc:h2:mem:camunda" + this.GetType().Name);

            CustomExpressionManager customExpressionManager = new CustomExpressionManager();
            Assert.True(customExpressionManager.FunctionMappers.Count == 0);
            config.ExpressionManager = customExpressionManager;

            // when the engine is initialized
            engine = config.BuildProcessEngine();

            // then two default function mappers should be registered
            Assert.AreSame(customExpressionManager, config.ExpressionManager);
            Assert.AreEqual(2, customExpressionManager.FunctionMappers.Count);

            bool commandContextMapperFound = false;
            bool dateTimeMapperFound = false;

            foreach (FunctionMapper functionMapper in customExpressionManager.FunctionMappers)
            {
                if (functionMapper is CommandContextFunctionMapper)
                {
                    commandContextMapperFound = true;
                }

                if (functionMapper is DateTimeFunctionMapper)
                {
                    dateTimeMapperFound = true;
                }
            }

            Assert.True(commandContextMapperFound && dateTimeMapperFound);
        }
        [TearDown]
        public virtual void tearDown()
        {
            if (engine != null)
            {
                engine.Close();
                engine = null;
            }
        }
    }

}