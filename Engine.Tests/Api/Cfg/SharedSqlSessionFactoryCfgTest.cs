
//using ESS.FW.Bpm.Engine.Impl.Cfg;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.Api.Cfg
//{

//    /// <summary>
//    /// 
//    /// 
//    /// </summary>
//    public class SharedSqlSessionFactoryCfgTest
//    {

//        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Before @After public void cleanCachedSessionFactory()
//        public virtual void cleanCachedSessionFactory()
//        {
//            ProcessEngineConfigurationImpl.cachedSqlSessionFactory = null;
//        }

//        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Test public void shouldNotReuseSqlSessionFactoryByDefault()
//        public virtual void shouldNotReuseSqlSessionFactoryByDefault()
//        {
//            Assert.IsFalse((new StandaloneInMemProcessEngineConfiguration()).UseSharedSqlSessionFactory);
//        }

//        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Test public void shouldCacheDbSqlSessionFactoryIfConfigured()
//        public virtual void shouldCacheDbSqlSessionFactoryIfConfigured()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final TestEngineCfg cfg = new TestEngineCfg();
//            TestEngineCfg cfg = new TestEngineCfg();

//            // given
//            cfg.UseSharedSqlSessionFactory = true;

//            // if
//            cfg.initSqlSessionFactory();

//            // then
//            Assert.NotNull(ProcessEngineConfigurationImpl.cachedSqlSessionFactory);
//        }

//        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Test public void shouldNotCacheDbSqlSessionFactoryIfNotConfigured()
//        public virtual void shouldNotCacheDbSqlSessionFactoryIfNotConfigured()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final TestEngineCfg cfg = new TestEngineCfg();
//            TestEngineCfg cfg = new TestEngineCfg();

//            // if
//            cfg.initSqlSessionFactory();

//            // then
//            Assert.IsNull(ProcessEngineConfigurationImpl.cachedSqlSessionFactory);
//            Assert.NotNull(cfg.SqlSessionFactory);
//        }

//        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Test public void shouldReuseCachedSqlSessionFactoryIfConfigured()
//        public virtual void shouldReuseCachedSqlSessionFactoryIfConfigured()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final TestEngineCfg cfg = new TestEngineCfg();
//            TestEngineCfg cfg = new TestEngineCfg();
//            SqlSessionFactory existingSessionFactory = mock(typeof(SqlSessionFactory));

//            // given
//            ProcessEngineConfigurationImpl.cachedSqlSessionFactory = existingSessionFactory;
//            cfg.UseSharedSqlSessionFactory = true;

//            // if
//            cfg.initSqlSessionFactory();

//            // then
//            AssertSame(existingSessionFactory, ProcessEngineConfigurationImpl.cachedSqlSessionFactory);
//            AssertSame(existingSessionFactory, cfg.SqlSessionFactory);
//        }

//        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Test public void shouldNotReuseCachedSqlSessionIfNotConfigured()
//        public virtual void shouldNotReuseCachedSqlSessionIfNotConfigured()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final TestEngineCfg cfg = new TestEngineCfg();
//            TestEngineCfg cfg = new TestEngineCfg();
//            SqlSessionFactory existingSessionFactory = mock(typeof(SqlSessionFactory));

//            // given
//            ProcessEngineConfigurationImpl.cachedSqlSessionFactory = existingSessionFactory;

//            // if
//            cfg.initSqlSessionFactory();

//            // then
//            AssertSame(existingSessionFactory, ProcessEngineConfigurationImpl.cachedSqlSessionFactory);
//            AssertNotSame(existingSessionFactory, cfg.SqlSessionFactory);
//        }

//        internal class TestEngineCfg : StandaloneInMemProcessEngineConfiguration
//        {

//            public TestEngineCfg()
//            {
//                dataSource = mock(typeof(DataSource));
//                transactionFactory = mock(typeof(TransactionFactory));
//            }

//            public override void initSqlSessionFactory()
//            {
//                base.initSqlSessionFactory();
//            }

//            public override SqlSessionFactory SqlSessionFactory
//            {
//                get
//                {
//                    return base.SqlSessionFactory;
//                }
//            }

//        }

//    }

//}