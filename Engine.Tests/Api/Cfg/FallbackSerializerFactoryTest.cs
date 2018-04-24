using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Variable.Serializer;
using ESS.FW.Bpm.Engine.Variable.Value;
using NUnit.Framework;

namespace Engine.Tests.Api.Cfg
{


    /// <summary>
    /// 
    /// 
    /// </summary>
    public class FallbackSerializerFactoryTest
    {

        protected internal IProcessEngine processEngine;
        protected internal string deployment;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @After public void tearDown()
        public virtual void tearDown()
        {

            if (processEngine != null)
            {
                if (!string.ReferenceEquals(deployment, null))
                {
                    processEngine.RepositoryService.DeleteDeployment(deployment, true);
                }

                processEngine.Close();
            }
        }

        [Test]
        public virtual void testFallbackSerializer()
        {
            // given
            // that the process engine is configured with a fallback serializer factory
            ProcessEngineConfigurationImpl engineConfiguration = (new StandaloneInMemProcessEngineConfiguration()).SetJdbcUrl("jdbc:h2:mem:camunda-forceclose").SetProcessEngineName("engine-forceclose") as ProcessEngineConfigurationImpl;

            engineConfiguration.FallbackSerializerFactory = new ExampleSerializerFactory();

            processEngine = engineConfiguration.BuildProcessEngine();
            deployOneTaskProcess(processEngine);

            // Todo: ExampleSerializer
            // when setting a variable that no regular serializer can handle
            //IObjectValue objectValue = Engine.Variable.Variables.ObjectValue("foo").SerializationDataFormat(ExampleSerializer.FORMAT).Create();

            //IProcessInstance pi = processEngine.RuntimeService.StartProcessInstanceByKey("oneTaskProcess", Engine.Variable.Variables.CreateVariables().PutValueTyped("var", objectValue));

            //IObjectValue fetchedValue = processEngine.RuntimeService.GetVariableTyped<IObjectValue>(pi.Id, "var", true);

            // Todo: ExampleSerializer
            // then the fallback serializer is used
            //Assert.NotNull(fetchedValue);
            //Assert.AreEqual(ExampleSerializer.FORMAT, fetchedValue.SerializationDataFormat);
            //Assert.AreEqual("foo", fetchedValue.Value);
        }

        [Test]
        public virtual void testFallbackSerializerDoesNotOverrideRegularSerializer()
        {
            // given
            // that the process engine is configured with a serializer for a certain format
            // and a fallback serializer factory for the same format
            ProcessEngineConfigurationImpl engineConfiguration = (new StandaloneInMemProcessEngineConfiguration()).SetJdbcUrl("jdbc:h2:mem:camunda-forceclose").SetProcessEngineName("engine-forceclose") as ProcessEngineConfigurationImpl;

            // Todo: ProcessEngineConfigurationImpl.CustomPreVariableSerializers
            //engineConfiguration.CustomPreVariableSerializers = new (new ExampleConstantSerializer());
            engineConfiguration.FallbackSerializerFactory = new ExampleSerializerFactory();

            processEngine = engineConfiguration.BuildProcessEngine();
            deployOneTaskProcess(processEngine);

            // Todo: ExampleSerializer
            // when setting a variable that no regular serializer can handle
            //IObjectValue objectValue = Engine.Variable.Variables.ObjectValue("foo").SerializationDataFormat(ExampleSerializer.FORMAT).Create();

            //IProcessInstance pi = processEngine.RuntimeService.StartProcessInstanceByKey("oneTaskProcess", Engine.Variable.Variables.CreateVariables().PutValueTyped("var", objectValue));

            //IObjectValue fetchedValue = processEngine.RuntimeService.GetVariableTyped<IObjectValue>(pi.Id, "var", true);

            // then the fallback serializer is used
            //Assert.NotNull(fetchedValue);
            // Todo: ExampleSerializer
            //Assert.AreEqual(ExampleSerializer.FORMAT, fetchedValue.SerializationDataFormat);
            //Assert.AreEqual(ExampleConstantSerializer.DESERIALIZED_VALUE, fetchedValue.Value);
        }


        public class ExampleSerializerFactory : IVariableSerializerFactory
        {

            public ITypedValueSerializer GetSerializer<T>(ITypedValue value) where T : ITypedValue
            {
                // Todo: ExampleSerializer
                //return new ExampleSerializer();
                return null;
            }

            public ITypedValueSerializer GetSerializer<T>(string serializerName) where T : ITypedValue
            {
                // Todo: ExampleSerializer
                //return new ExampleSerializer();
                return null;
            }
        }

        // Todo:JavaObjectSerializer
        //public class ExampleSerializer : JavaObjectSerializer
        //{

        //    public const string FORMAT = "example";

        //    public ExampleSerializer() : base()
        //    {
        //        this.serializationDataFormat = FORMAT;
        //    }

        //    public virtual string Name
        //    {
        //        get
        //        {
        //            return FORMAT;
        //        }
        //    }

        //}

        // Todo:JavaObjectSerializer
        //public class ExampleConstantSerializer : JavaObjectSerializer
        //{

        //    public const string DESERIALIZED_VALUE = "bar";

        //    public ExampleConstantSerializer() : base()
        //    {
        //        this.serializationDataFormat = ExampleSerializer.FORMAT;
        //    }

        //    public virtual string Name
        //    {
        //        get
        //        {
        //            return ExampleSerializer.FORMAT;
        //        }
        //    }

        //    //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //    //ORIGINAL LINE: protected Object deserializeFromByteArray(byte[] bytes, String objectTypeName) throws Exception
        //    protected internal virtual object deserializeFromByteArray(byte[] bytes, string objectTypeName)
        //    {
        //        // deserialize everything to a constant string
        //        return DESERIALIZED_VALUE;
        //    }

        //}

        protected internal virtual void deployOneTaskProcess(IProcessEngine engine)
        {
            deployment = engine.RepositoryService.CreateDeployment().AddClasspathResource("resources/api/oneTaskProcess.bpmn20.xml").Deploy().Id;
        }
    }

}