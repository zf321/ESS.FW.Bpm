using System;
using System.IO;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Util;
using NUnit.Framework;

namespace Engine.Tests
{


    /// <summary>
    /// Convenience for ProcessEngine and services initialization in the form of a JUnit base class.
    /// 
    /// <para>Usage: <code>public class YourTest extends ProcessEngineTestCase</code></para>
    /// 
    /// <para>The ProcessEngine and the services available to subclasses through protected member fields.
    /// The processEngine will be initialized by default with the camunda.cfg.xml resource
    /// on the classpath.  To specify a different configuration file, override the
    /// <seealso cref="#getConfigurationResource()"/> method.
    /// Process engines will be cached statically.  The first time the setUp is called for a given
    /// configuration resource, the process engine will be constructed.</para>
    /// 
    /// <para>You can declare a deployment with the <seealso cref="Deployment"/> annotation.
    /// This base class will make sure that this deployment gets deployed in the
    /// setUp and <seealso cref="RepositoryService#deleteDeploymentCascade(String, boolean) cascade deleted"/>
    /// in the tearDown.
    /// </para>
    /// 
    /// <para>This class also lets you {@link #setCurrentTime(Date) set the current time used by the
    /// process engine}. This can be handy to control the exact time that is used by the engine
    /// in order to verify e.g. e.g. due dates of timers.  Or start, end and duration times
    /// in the history service.  In the tearDown, the internal clock will automatically be
    /// reset to use the current system time rather then the time that was set during
    /// a test method.  In other words, you don't have to clean up your own time messing mess ;-)
    /// </para>
    /// 
    /// @author Tom Baeyens
    ///  (camunda)
    /// </summary>
    public class ProcessEngineTestCase //: TestCase
    {

        protected internal string configurationResource = "camunda.cfg.xml";
        protected internal string ConfigurationResourceCompat = "activiti.cfg.xml";
        protected internal string DeploymentId = null;

        protected internal IProcessEngine ProcessEngine;
        protected internal IRepositoryService RepositoryService;
        protected internal IRuntimeService RuntimeService;
        protected internal ITaskService TaskService;
        [Obsolete]
        protected internal IHistoryService HistoricDataService;
        protected internal IHistoryService HistoryService;
        protected internal IIdentityService IdentityService;
        protected internal IManagementService ManagementService;
        protected internal IFormService FormService;
        protected internal IFilterService FilterService;
        protected internal IAuthorizationService AuthorizationService;
        protected internal ICaseService CaseService;

        protected internal bool SkipTest = false;

        /// <summary>
        /// uses 'camunda.cfg.xml' as it's configuration resource </summary>
        public ProcessEngineTestCase()
        {
        }
        
        public virtual void AssertProcessEnded(string processInstanceId)
        {
            ProcessEngineAssert.AssertProcessEnded(ProcessEngine, processInstanceId);
        }

        [SetUp]
        protected internal  void SetUp()
        {
            //base.SetUp();

            if (ProcessEngine == null)
            {
                InitializeProcessEngine();
                InitializeServices();
            }

            bool hasRequiredHistoryLevel = TestHelper.AnnotationRequiredHistoryLevelCheck(ProcessEngine, this.GetType(), TestContext.CurrentContext.Test.Name);
            // ignore test case when current history level is too low
            SkipTest = !hasRequiredHistoryLevel;

            if (!SkipTest)
            {
                DeploymentId = TestHelper.AnnotationDeploymentSetUp(ProcessEngine, this.GetType(), TestContext.CurrentContext.Test.Name);
            }
        }
        protected internal  void RunTest()
        {
            if (!SkipTest)
            {
                //base.RunTest();
            }
        }

        protected internal virtual void InitializeProcessEngine()
        {
            try
            {
                ProcessEngine = TestHelper.GetProcessEngine(ConfigurationResource);
            }
            catch (System.Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException is FileNotFoundException)
                {
                    ProcessEngine = ProcessEngineConfiguration.CreateProcessEngineConfigurationFromResource(ConfigurationResourceCompat).BuildProcessEngine();
                }
                else
                {
                    throw ex;
                }
            }
        }

        protected internal virtual void InitializeServices()
        {
            RepositoryService = ProcessEngine.RepositoryService;
            RuntimeService = ProcessEngine.RuntimeService;
            TaskService = ProcessEngine.TaskService;
            HistoricDataService = ProcessEngine.HistoryService;
            HistoryService = ProcessEngine.HistoryService;
            IdentityService = ProcessEngine.IdentityService;
            ManagementService = ProcessEngine.ManagementService;
            FormService = ProcessEngine.FormService;
            FilterService = ProcessEngine.FilterService;
            AuthorizationService = ProcessEngine.AuthorizationService;
            CaseService = ProcessEngine.CaseService;
        }

        [TearDown]
        protected internal  void TearDown()
        {
            TestHelper.AnnotationDeploymentTearDown(ProcessEngine, DeploymentId, this.GetType(), TestContext.CurrentContext.Test.Name);

            ClockUtil.Reset();

            //base.TearDown();
        }

        public static void CloseProcessEngines()
        {
            TestHelper.CloseProcessEngines();
        }

        public virtual DateTime CurrentTime
        {
            set
            {
                ClockUtil.CurrentTime = value;
            }
        }

        public virtual string ConfigurationResource
        {
            get
            {
                return configurationResource;
            }
            set
            {
                this.configurationResource = value;
            }
        }


    }
}
