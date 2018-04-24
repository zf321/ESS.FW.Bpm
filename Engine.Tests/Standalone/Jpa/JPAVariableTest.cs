//using System;
//using System.Collections.Generic;
//using System.Linq;
//using ESS.FW.Bpm.Engine.Common.Utils;
//using ESS.FW.Bpm.Engine.Impl.Cfg;
//using ESS.FW.Bpm.Engine.Impl.Digest._apacheCommonsCodec;
//using ESS.FW.Bpm.Engine.Runtime;
//using ESS.FW.Bpm.Engine.Tests.Api.Variables;
//using ESS.FW.Bpm.Engine.Variable;
//using ESS.FW.Bpm.Engine.Variable.Value;
//using NUnit.Framework;


//namespace ESS.FW.Bpm.Engine.Tests.Standalone.Jpa
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    [TestFixture]
//    public class JPAVariableTest : AbstractProcessEngineTestCase
//    {

//        protected internal const string ONE_TASK_PROCESS = "resources/api/variables/oneTaskProcess.bpmn20.xml";
//        protected internal static IProcessEngine cachedProcessEngine;

//        private FieldAccessJPAEntity simpleEntityFieldAccess;
//        private PropertyAccessJPAEntity simpleEntityPropertyAccess;
//        private SubclassFieldAccessJPAEntity subclassFieldAccess;
//        private SubclassPropertyAccessJPAEntity subclassPropertyAccess;

//        private ByteIdJPAEntity byteIdJPAEntity;
//        private ShortIdJPAEntity shortIdJPAEntity;
//        private IntegerIdJPAEntity integerIdJPAEntity;
//        private LongIdJPAEntity longIdJPAEntity;
//        private FloatIdJPAEntity floatIdJPAEntity;
//        private DoubleIdJPAEntity doubleIdJPAEntity;
//        private CharIdJPAEntity charIdJPAEntity;
//        private StringIdJPAEntity stringIdJPAEntity;
//        private DateIdJPAEntity dateIdJPAEntity;
//        private SQLDateIdJPAEntity sqlDateIdJPAEntity;
//        private BigDecimalIdJPAEntity bigDecimalIdJPAEntity;
//        private BigIntegerIdJPAEntity bigIntegerIdJPAEntity;
//        private CompoundIdJPAEntity compoundIdJPAEntity;

//        private FieldAccessJPAEntity entityToQuery;
//        private FieldAccessJPAEntity entityToUpdate;

//        //private static EntityManagerFactory entityManagerFactory;

//        // JUnit3-style beforeclass and afterclass
//        // public static Test suite()
//        // {
//        //return new JPAVariableTestSetup();
//        // }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Ignore public static class JPAVariableTestSetup extends junit.extensions.TestSetup
//        public class JPAVariableTestSetup //: TestSetup
//        {
//            //public JPAVariableTestSetup() : base(new TestSuite(typeof(JPAVariableTest)))
//            //{
//            //}

//            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//            //ORIGINAL LINE: protected void setUp() throws Exception
//            protected internal virtual void setUp()
//            {
//                ProcessEngineConfigurationImpl processEngineConfiguration = (ProcessEngineConfigurationImpl)ProcessEngineConfiguration.CreateProcessEngineConfigurationFromResource("resources/standalone/jpa/camunda.cfg.xml");

//                cachedProcessEngine = processEngineConfiguration.BuildProcessEngine();

//                //EntityManagerSessionFactory entityManagerSessionFactory = (EntityManagerSessionFactory) processEngineConfiguration.SessionFactories.Get(typeof(EntityManagerSession));

//                //entityManagerFactory = entityManagerSessionFactory.EntityManagerFactory;
//            }

//            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//            //ORIGINAL LINE: protected void tearDown() throws Exception
//            protected internal virtual void tearDown()
//            {
//                cachedProcessEngine.Close();
//                cachedProcessEngine = null;

//                //entityManagerFactory.Close();
//                //entityManagerFactory = null;
//            }

//        }

//        protected internal virtual void initializeProcessEngine()
//        {
//            ProcessEngine = cachedProcessEngine;
//        }

//        public virtual void setupJPAEntities()
//        {

//            //EntityManager manager = entityManagerFactory.CreateEntityManager();
//            //manager.Transaction.begin();

//            // Simple test data
//            simpleEntityFieldAccess = new FieldAccessJPAEntity();
//            simpleEntityFieldAccess.Id = 1L;
//            simpleEntityFieldAccess.Value = "value1";
//            //manager.persist(simpleEntityFieldAccess);

//            simpleEntityPropertyAccess = new PropertyAccessJPAEntity();
//            simpleEntityPropertyAccess.Id = 1L;
//            simpleEntityPropertyAccess.Value = "value2";
//            //manager.persist(simpleEntityPropertyAccess);

//            subclassFieldAccess = new SubclassFieldAccessJPAEntity();
//            subclassFieldAccess.Id = 1L;
//            subclassFieldAccess.Value = "value3";
//            //manager.persist(subclassFieldAccess);

//            subclassPropertyAccess = new SubclassPropertyAccessJPAEntity();
//            subclassPropertyAccess.Id = 1L;
//            subclassPropertyAccess.Value = "value4";
//            //manager.persist(subclassPropertyAccess);

//            // Test entities with all possible ID types
//            byteIdJPAEntity = new ByteIdJPAEntity();
//            byteIdJPAEntity.ByteId = (sbyte)1;
//            manager.persist(byteIdJPAEntity);

//            shortIdJPAEntity = new ShortIdJPAEntity();
//            shortIdJPAEntity.ShortId = (short)123;
//            manager.persist(shortIdJPAEntity);

//            integerIdJPAEntity = new IntegerIdJPAEntity();
//            integerIdJPAEntity.IntId = 123;
//            manager.persist(integerIdJPAEntity);

//            longIdJPAEntity = new LongIdJPAEntity();
//            longIdJPAEntity.LongId = 123456789L;
//            manager.persist(longIdJPAEntity);

//            floatIdJPAEntity = new FloatIdJPAEntity();
//            floatIdJPAEntity.FloatId = (float)123.45678;
//            manager.persist(floatIdJPAEntity);

//            doubleIdJPAEntity = new DoubleIdJPAEntity();
//            doubleIdJPAEntity.DoubleId = 12345678.987654;
//            manager.persist(doubleIdJPAEntity);

//            charIdJPAEntity = new CharIdJPAEntity();
//            charIdJPAEntity.CharId = 'g';
//            manager.persist(charIdJPAEntity);

//            dateIdJPAEntity = new DateIdJPAEntity();
//            dateIdJPAEntity.DateId = DateTime.Now;
//            manager.persist(dateIdJPAEntity);

//            sqlDateIdJPAEntity = new SQLDateIdJPAEntity();
//            sqlDateIdJPAEntity.DateId = new DateTime(new DateTime().Ticks);
//            manager.persist(sqlDateIdJPAEntity);

//            stringIdJPAEntity = new StringIdJPAEntity();
//            stringIdJPAEntity.StringId = "azertyuiop";
//            manager.persist(stringIdJPAEntity);

//            bigDecimalIdJPAEntity = new BigDecimalIdJPAEntity();
//            bigDecimalIdJPAEntity.BigDecimalId = Convert.ToDecimal("12345678912345678900000.123456789123456789");
//            manager.persist(bigDecimalIdJPAEntity);

//            bigIntegerIdJPAEntity = new BigIntegerIdJPAEntity();
//            bigIntegerIdJPAEntity.BigIntegerId = Int64.Parse("12345678912345678912345678900000");
//            manager.persist(bigIntegerIdJPAEntity);

//            manager.flush();
//            manager.Transaction.commit();
//            manager.Close();
//        }

//        public virtual void setupIllegalJPAEntities()
//        {
//            EntityManager manager = entityManagerFactory.CreateEntityManager();
//            manager.Transaction.begin();

//            compoundIdJPAEntity = new CompoundIdJPAEntity();
//            EmbeddableCompoundId id = new EmbeddableCompoundId();
//            id.IdPart1 = 123L;
//            id.IdPart2 = "part2";
//            compoundIdJPAEntity.Id = id;
//            manager.persist(compoundIdJPAEntity);

//            manager.flush();
//            manager.Transaction.commit();
//            manager.Close();
//        }

//        public virtual void setupQueryJPAEntity(long id)
//        {
//            if (entityToQuery == null)
//            {
//                EntityManager manager = entityManagerFactory.CreateEntityManager();
//                manager.Transaction.begin();

//                entityToQuery = new FieldAccessJPAEntity();
//                entityToQuery.Id = id;
//                manager.persist(entityToQuery);

//                manager.flush();
//                manager.Transaction.commit();
//                manager.Close();
//            }
//        }

//        public virtual void setupJPAEntityToUpdate()
//        {
//            EntityManager manager = entityManagerFactory.CreateEntityManager();
//            manager.Transaction.begin();

//            entityToUpdate = new FieldAccessJPAEntity();
//            entityToUpdate.Id = 3L;
//            manager.persist(entityToUpdate);
//            manager.flush();
//            manager.Transaction.commit();
//            manager.Close();
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment public void testStoreJPAEntityAsVariable()
//        public virtual void testStoreJPAEntityAsVariable()
//        {
//            setupJPAEntities();
//            // -----------------------------------------------------------------------------
//            // Simple test, Start process with JPA entities as variables
//            // -----------------------------------------------------------------------------
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["simpleEntityFieldAccess"] = simpleEntityFieldAccess;
//            variables["simpleEntityPropertyAccess"] = simpleEntityPropertyAccess;
//            variables["subclassFieldAccess"] = subclassFieldAccess;
//            variables["subclassPropertyAccess"] = subclassPropertyAccess;

//            // Start the process with the JPA-entities as variables. They will be stored in the DB.
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("JPAVariableProcess", variables);

//            // Read entity with @Id on field
//            object fieldAccessResult = runtimeService.GetVariable(processInstance.Id, "simpleEntityFieldAccess");
//            Assert.True(fieldAccessResult is FieldAccessJPAEntity);
//            Assert.AreEqual(1L, ((FieldAccessJPAEntity)fieldAccessResult).Id.Value);
//            Assert.AreEqual("value1", ((FieldAccessJPAEntity)fieldAccessResult).Value);

//            // Read entity with @Id on property
//            object propertyAccessResult = runtimeService.GetVariable(processInstance.Id, "simpleEntityPropertyAccess");
//            Assert.True(propertyAccessResult is PropertyAccessJPAEntity);
//            Assert.AreEqual(1L, ((PropertyAccessJPAEntity)propertyAccessResult).Id.Value);
//            Assert.AreEqual("value2", ((PropertyAccessJPAEntity)propertyAccessResult).Value);

//            // Read entity with @Id on field of mapped superclass
//            object subclassFieldResult = runtimeService.GetVariable(processInstance.Id, "subclassFieldAccess");
//            Assert.True(subclassFieldResult is SubclassFieldAccessJPAEntity);
//            Assert.AreEqual(1L, ((SubclassFieldAccessJPAEntity)subclassFieldResult).Id.Value);
//            Assert.AreEqual("value3", ((SubclassFieldAccessJPAEntity)subclassFieldResult).Value);

//            // Read entity with @Id on property of mapped superclass
//            object subclassPropertyResult = runtimeService.GetVariable(processInstance.Id, "subclassPropertyAccess");
//            Assert.True(subclassPropertyResult is SubclassPropertyAccessJPAEntity);
//            Assert.AreEqual(1L, ((SubclassPropertyAccessJPAEntity)subclassPropertyResult).Id.Value);
//            Assert.AreEqual("value4", ((SubclassPropertyAccessJPAEntity)subclassPropertyResult).Value);

//            // -----------------------------------------------------------------------------
//            // Test updating JPA-entity to null-value and back again
//            // -----------------------------------------------------------------------------
//            object currentValue = runtimeService.GetVariable(processInstance.Id, "simpleEntityFieldAccess");
//            Assert.NotNull(currentValue);
//            // Set to null
//            runtimeService.SetVariable(processInstance.Id, "simpleEntityFieldAccess", null);
//            currentValue = runtimeService.GetVariable(processInstance.Id, "simpleEntityFieldAccess");
//            Assert.IsNull(currentValue);
//            // Set to JPA-entity again
//            runtimeService.SetVariable(processInstance.Id, "simpleEntityFieldAccess", simpleEntityFieldAccess);
//            currentValue = runtimeService.GetVariable(processInstance.Id, "simpleEntityFieldAccess");
//            Assert.NotNull(currentValue);
//            Assert.True(currentValue is FieldAccessJPAEntity);
//            Assert.AreEqual(1L, ((FieldAccessJPAEntity)currentValue).Id.Value);


//            // -----------------------------------------------------------------------------
//            // Test all allowed types of ID values
//            // -----------------------------------------------------------------------------

//            variables = new Dictionary<string, object>();
//            variables["byteIdJPAEntity"] = byteIdJPAEntity;
//            variables["shortIdJPAEntity"] = shortIdJPAEntity;
//            variables["integerIdJPAEntity"] = integerIdJPAEntity;
//            variables["longIdJPAEntity"] = longIdJPAEntity;
//            variables["floatIdJPAEntity"] = floatIdJPAEntity;
//            variables["doubleIdJPAEntity"] = doubleIdJPAEntity;
//            variables["charIdJPAEntity"] = charIdJPAEntity;
//            variables["stringIdJPAEntity"] = stringIdJPAEntity;
//            variables["dateIdJPAEntity"] = dateIdJPAEntity;
//            variables["sqlDateIdJPAEntity"] = sqlDateIdJPAEntity;
//            variables["bigDecimalIdJPAEntity"] = bigDecimalIdJPAEntity;
//            variables["bigIntegerIdJPAEntity"] = bigIntegerIdJPAEntity;

//            // Start the process with the JPA-entities as variables. They will be stored in the DB.
//            IProcessInstance processInstanceAllTypes = runtimeService.StartProcessInstanceByKey("JPAVariableProcess", variables);
//            object byteIdResult = runtimeService.GetVariable(processInstanceAllTypes.Id, "byteIdJPAEntity");
//            Assert.True(byteIdResult is ByteIdJPAEntity);
//            Assert.AreEqual(byteIdJPAEntity.ByteId, ((ByteIdJPAEntity)byteIdResult).ByteId);

//            object shortIdResult = runtimeService.GetVariable(processInstanceAllTypes.Id, "shortIdJPAEntity");
//            Assert.True(shortIdResult is ShortIdJPAEntity);
//            Assert.AreEqual(shortIdJPAEntity.ShortId, ((ShortIdJPAEntity)shortIdResult).ShortId);

//            object integerIdResult = runtimeService.GetVariable(processInstanceAllTypes.Id, "integerIdJPAEntity");
//            Assert.True(integerIdResult is IntegerIdJPAEntity);
//            Assert.AreEqual(integerIdJPAEntity.IntId, ((IntegerIdJPAEntity)integerIdResult).IntId);

//            object longIdResult = runtimeService.GetVariable(processInstanceAllTypes.Id, "longIdJPAEntity");
//            Assert.True(longIdResult is LongIdJPAEntity);
//            Assert.AreEqual(longIdJPAEntity.LongId, ((LongIdJPAEntity)longIdResult).LongId);

//            object floatIdResult = runtimeService.GetVariable(processInstanceAllTypes.Id, "floatIdJPAEntity");
//            Assert.True(floatIdResult is FloatIdJPAEntity);
//            Assert.AreEqual(floatIdJPAEntity.FloatId, ((FloatIdJPAEntity)floatIdResult).FloatId);

//            object doubleIdResult = runtimeService.GetVariable(processInstanceAllTypes.Id, "doubleIdJPAEntity");
//            Assert.True(doubleIdResult is DoubleIdJPAEntity);
//            Assert.AreEqual(doubleIdJPAEntity.DoubleId, ((DoubleIdJPAEntity)doubleIdResult).DoubleId);

//            object charIdResult = runtimeService.GetVariable(processInstanceAllTypes.Id, "charIdJPAEntity");
//            Assert.True(charIdResult is CharIdJPAEntity);
//            Assert.AreEqual(charIdJPAEntity.CharId, ((CharIdJPAEntity)charIdResult).CharId);

//            object stringIdResult = runtimeService.GetVariable(processInstanceAllTypes.Id, "stringIdJPAEntity");
//            Assert.True(stringIdResult is StringIdJPAEntity);
//            Assert.AreEqual(stringIdJPAEntity.StringId, ((StringIdJPAEntity)stringIdResult).StringId);

//            object dateIdResult = runtimeService.GetVariable(processInstanceAllTypes.Id, "dateIdJPAEntity");
//            Assert.True(dateIdResult is DateIdJPAEntity);
//            Assert.AreEqual(dateIdJPAEntity.DateId, ((DateIdJPAEntity)dateIdResult).DateId);

//            object sqlDateIdResult = runtimeService.GetVariable(processInstanceAllTypes.Id, "sqlDateIdJPAEntity");
//            Assert.True(sqlDateIdResult is SQLDateIdJPAEntity);
//            Assert.AreEqual(sqlDateIdJPAEntity.DateId, ((SQLDateIdJPAEntity)sqlDateIdResult).DateId);

//            object bigDecimalIdResult = runtimeService.GetVariable(processInstanceAllTypes.Id, "bigDecimalIdJPAEntity");
//            Assert.True(bigDecimalIdResult is BigDecimalIdJPAEntity);
//            Assert.AreEqual(bigDecimalIdJPAEntity.BigDecimalId, ((BigDecimalIdJPAEntity)bigDecimalIdResult).BigDecimalId);

//            object bigIntegerIdResult = runtimeService.GetVariable(processInstanceAllTypes.Id, "bigIntegerIdJPAEntity");
//            Assert.True(bigIntegerIdResult is BigIntegerIdJPAEntity);
//            Assert.AreEqual(bigIntegerIdJPAEntity.BigIntegerId, ((BigIntegerIdJPAEntity)bigIntegerIdResult).BigIntegerId);
//        }


//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment public void testIllegalEntities()
//        public virtual void testIllegalEntities()
//        {
//            setupIllegalJPAEntities();
//            // Starting process instance with a variable that has a compound primary key, which is not supported.
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["compoundIdJPAEntity"] = compoundIdJPAEntity;

//            try
//            {
//                runtimeService.StartProcessInstanceByKey("JPAVariableProcessExceptions", variables);
//                Assert.Fail("Exception expected");
//            }
//            catch (ProcessEngineException ae)
//            {
//                AssertTextPresent("Cannot find field or method with annotation @Id on class", ae.Message);
//                AssertTextPresent("only single-valued primary keys are supported on JPA-enities", ae.Message);
//            }

//            // Starting process instance with a variable that has null as ID-value
//            variables = new Dictionary<string, object>();
//            variables["nullValueEntity"] = new FieldAccessJPAEntity();

//            try
//            {
//                runtimeService.StartProcessInstanceByKey("JPAVariableProcessExceptions", variables);
//                Assert.Fail("Exception expected");
//            }
//            catch (ProcessEngineException ae)
//            {
//                AssertTextPresent("Value of primary key for JPA-Entity is null", ae.Message);
//            }

//            // Starting process instance with an invalid type of ID
//            // Under normal circumstances, JPA will throw an exception for this of the class is
//            // present in the PU when creating EntityanagerFactory, but we test it *just in case*
//            variables = new Dictionary<string, object>();
//            IllegalIdClassJPAEntity illegalIdTypeEntity = new IllegalIdClassJPAEntity();
//            illegalIdTypeEntity.Id = new DateTime();
//            variables["illegalTypeId"] = illegalIdTypeEntity;

//            try
//            {
//                runtimeService.StartProcessInstanceByKey("JPAVariableProcessExceptions", variables);
//                Assert.Fail("Exception expected");
//            }
//            catch (ProcessEngineException ae)
//            {
//                AssertTextPresent("Unsupported Primary key type for JPA-Entity", ae.Message);
//            }

//            // Start process instance with JPA-entity which has an ID but isn't persisted. When reading
//            // the variable we should get an exception.
//            variables = new Dictionary<string, object>();
//            FieldAccessJPAEntity nonPersistentEntity = new FieldAccessJPAEntity();
//            nonPersistentEntity.Id = 9999L;
//            variables["nonPersistentEntity"] = nonPersistentEntity;

//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("JPAVariableProcessExceptions", variables);

//            try
//            {
//                runtimeService.GetVariable(processInstance.Id, "nonPersistentEntity");
//                Assert.Fail("Exception expected");
//            }
//            catch (ProcessEngineException ae)
//            {
//                //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
//                AssertTextPresent("Entity does not exist: " + typeof(FieldAccessJPAEntity).FullName + " - 9999", ae.Message);
//            }
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment public void testQueryJPAVariable()
//        public virtual void testQueryJPAVariable()
//        {
//            setupQueryJPAEntity(2L);

//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["entityToQuery"] = entityToQuery;

//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("JPAVariableProcess", variables);

//            // Query the processInstance
//            IProcessInstance result = runtimeService.CreateProcessInstanceQuery()/*.VariableValueEquals("entityToQuery", entityToQuery)*/.First();

//        Assert.NotNull(result);
//            Assert.AreEqual(result.Id, processInstance.Id);

//            // Query with the same entity-type but with different ID should have no result
//            FieldAccessJPAEntity unexistingEntity = new FieldAccessJPAEntity();
//            unexistingEntity.Id = 8888L;

//            result = runtimeService.CreateProcessInstanceQuery()/*//.VariableValueEquals("entityToQuery", unexistingEntity)*/.First();

//        Assert.IsNull(result);

//            // All other operators are unsupported
//            try
//            {
//                runtimeService.CreateProcessInstanceQuery().VariableValueNotEquals("entityToQuery", entityToQuery).First();
//                Assert.Fail("Exception expected");
//            }
//            catch (ProcessEngineException ae)
//            {
//                AssertTextPresent("JPA entity variables can only be used in 'variableValueEquals'", ae.Message);
//            }
//            try
//            {
//                runtimeService.CreateProcessInstanceQuery().VariableValueGreaterThan("entityToQuery", entityToQuery).First();
//                Assert.Fail("Exception expected");
//            }
//            catch (ProcessEngineException ae)
//            {
//                AssertTextPresent("JPA entity variables can only be used in 'variableValueEquals'", ae.Message);
//            }
//            try
//            {
//                runtimeService.CreateProcessInstanceQuery().VariableValueGreaterThanOrEqual("entityToQuery", entityToQuery).First();
//                Assert.Fail("Exception expected");
//            }
//            catch (ProcessEngineException ae)
//            {
//                AssertTextPresent("JPA entity variables can only be used in 'variableValueEquals'", ae.Message);
//            }
//            try
//            {
//                runtimeService.CreateProcessInstanceQuery().VariableValueLessThan("entityToQuery", entityToQuery).First();
//                Assert.Fail("Exception expected");
//            }
//            catch (ProcessEngineException ae)
//            {
//                AssertTextPresent("JPA entity variables can only be used in 'variableValueEquals'", ae.Message);
//            }
//            try
//            {
//                runtimeService.CreateProcessInstanceQuery().VariableValueLessThanOrEqual("entityToQuery", entityToQuery).First();
//                Assert.Fail("Exception expected");
//            }
//            catch (ProcessEngineException ae)
//            {
//                AssertTextPresent("JPA entity variables can only be used in 'variableValueEquals'", ae.Message);
//            }
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment public void FAILING_testUpdateJPAEntityValues()
//        public virtual void FAILING_testUpdateJPAEntityValues()
//        {
//            setupJPAEntityToUpdate();
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["entityToUpdate"] = entityToUpdate;

//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("UpdateJPAValuesProcess", variables);

//            // Servicetask in process 'UpdateJPAValuesProcess' should have set value on entityToUpdate.
//            object updatedEntity = runtimeService.GetVariable(processInstance.Id, "entityToUpdate");
//            Assert.True(updatedEntity is FieldAccessJPAEntity);
//            Assert.AreEqual("updatedValue", ((FieldAccessJPAEntity)updatedEntity).Value);
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = ONE_TASK_PROCESS) public void testFailSerializationForUnknownSerializedValueType() throws java.io.IOException
//        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//        public virtual void testFailSerializationForUnknownSerializedValueType()
//        {
//            // given
//            JavaSerializable pojo = new JavaSerializable("foo");
//            System.IO.MemoryStream baos = new System.IO.MemoryStream();
//            (new ObjectOutputStream(baos)).writeObject(pojo);
//            string serializedObject = StringUtil.fromBytes(Base64.encodeBase64(baos.toByteArray()), processEngine);

//            //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
//            IObjectValue serializedObjectValue = Variables.serializedObjectValue(serializedObject).serializationDataFormat(Variables.SerializationDataFormats.JAVA).objectTypeName(pojo.GetType().FullName).Create();
//            IVariableMap variables = Variable.Variables.CreateVariables().PutValueTyped("var", serializedObjectValue);

//            // when
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

//            // then
//            JavaSerializable returnedPojo = (JavaSerializable)runtimeService.GetVariable(processInstance.Id, "var");
//            Assert.AreEqual(pojo, returnedPojo);
//        }

//    }

//}