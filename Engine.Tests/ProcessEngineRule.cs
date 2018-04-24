using System;
using System.Collections.Generic;
using System.IO;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Repository;
using NUnit.Framework;

namespace Engine.Tests
{
    

    /// <summary>
    /// Convenience for ProcessEngine and services initialization in the form of a
    /// JUnit rule.
    /// <para>
    /// Usage:
    /// </para>
    /// 
    /// <pre>
    /// public class YourTest {
    /// 
    ///   &#64;Rule
    ///   public ProcessEngineRule processEngineRule = new ProcessEngineRule();
    /// 
    ///   ..
    /// }
    /// </pre>
    /// <para>
    /// The ProcessEngine and the services will be made available to the test class
    /// through the getters of the processEngineRule. The processEngine will be
    /// initialized by default with the camunda.cfg.xml resource on the classpath. To
    /// specify a different configuration file, pass the resource location in
    /// <seealso cref="#ProcessEngineRule(String) the appropriate constructor"/>. Process
    /// engines will be cached statically. Right before the first time the setUp is
    /// called for a given configuration resource, the process engine will be
    /// constructed.
    /// </para>
    /// <para>
    /// You can declare a deployment with the <seealso cref="Deployment"/> annotation. This
    /// base class will make sure that this deployment gets deployed before the setUp
    /// and {@link RepositoryService#deleteDeployment(String, boolean) cascade
    /// deleted} after the tearDown.
    /// </para>
    /// <para>
    /// The processEngineRule also lets you
    /// {@link ProcessEngineRule#setCurrentTime(Date) set the current time used by
    /// the process engine}. This can be handy to control the exact time that is used
    /// by the engine in order to verify e.g. e.g. due dates of timers. Or start, end
    /// and duration times in the history service. In the tearDown, the internal
    /// clock will automatically be reset to use the current system time rather then
    /// the time that was set during a test method. In other words, you don't have to
    /// clean up your own time messing mess ;-)
    /// </para>
    /// <para>
    /// If you need the history service for your tests then you can specify the
    /// required history level of the test method or class, using the
    /// <seealso cref="RequiredHistoryLevel"/> annotation. If the current history level of the
    /// process engine is lower than the specified one then the test is skipped.
    /// </para>
    /// 
    /// 
    /// </summary>
    public class ProcessEngineRule : /*TestWatcher,*/ IProcessEngineServices
    {

        protected internal string configurationResource = "camunda.cfg.xml";
        protected internal string configurationResourceCompat = "activiti.cfg.xml";
        protected internal string deploymentId = null;
        protected internal IList<string> additionalDeployments = new List<string>();

        protected internal bool ensureCleanAfterTest = false;

        protected internal IProcessEngine processEngine;
        protected internal ProcessEngineConfigurationImpl processEngineConfiguration;
        protected internal IRepositoryService repositoryService;
        protected internal IRuntimeService runtimeService;
        protected internal ITaskService taskService;
        protected internal IIdentityService identityService;
        protected internal IManagementService managementService;
        protected internal IFormService formService;
        protected internal IFilterService filterService;
        protected internal IAuthorizationService authorizationService;
        protected internal ICaseService caseService;
        protected internal IExternalTaskService externalTaskService;
        protected internal IDecisionService decisionService;

        public ProcessEngineRule() : this(false)
        {
            //Apply();
        }

        public ProcessEngineRule(bool ensureCleanAfterTest)
        {
            this.ensureCleanAfterTest = ensureCleanAfterTest;
            //Apply();
        }

        public ProcessEngineRule(string configurationResource) : this(configurationResource, false)
        {
        }

        public ProcessEngineRule(string configurationResource, bool ensureCleanAfterTest)
        {
            this.configurationResource = configurationResource;
            this.ensureCleanAfterTest = ensureCleanAfterTest;
        }

        public ProcessEngineRule(IProcessEngine processEngine) : this(processEngine, false)
        {
        }

        public ProcessEngineRule(IProcessEngine processEngine, bool ensureCleanAfterTest)
        {
            this.processEngine = processEngine;
            this.ensureCleanAfterTest = ensureCleanAfterTest;
        }
        [SetUp]
        public  void Apply()
        {

            if (processEngine == null)
            {
                InitializeProcessEngine();
            }

            InitializeServices();

            //bool hasRequiredHistoryLevel = TestHelper.AnnotationRequiredHistoryLevelCheck(processEngine, description);
            //return () =>
            //{
            //    Assume.That(hasRequiredHistoryLevel, Is.True, "ignored because the current history level is too low");
            //    base.Apply(@base, description)();
            //};
        }

        [SetUp]
        public void Starting()
        {
            //DeploymentId = TestHelper.AnnotationDeploymentSetUp(processEngine, description.TestClass, description.MethodName, description.GetAnnotation(typeof(Deployment)));
            Apply();
            var methodName = TestContext.CurrentContext.Test.Name.IndexOf('(') < 0
                ? TestContext.CurrentContext.Test.Name
                : TestContext.CurrentContext.Test.Name.Substring(0, TestContext.CurrentContext.Test.Name.IndexOf('('));
            deploymentId = TestHelper.AnnotationDeploymentSetUp(processEngine,
                Type.GetType(TestContext.CurrentContext.Test.FullName.Substring(0,
                    TestContext.CurrentContext.Test.FullName.LastIndexOf('.'))),
                methodName);
        }


        protected internal virtual void InitializeProcessEngine()
        {
            try
            {
                processEngine = TestHelper.GetProcessEngine(configurationResource);
            }
            catch (System.Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException is FileNotFoundException)
                {
                    processEngine = TestHelper.GetProcessEngine(configurationResourceCompat);
                }
                else
                {
                    throw ex;
                }
            }
            InitializeServices();
        }

        protected internal virtual void InitializeServices()
        {            
            processEngineConfiguration = processEngine.ProcessEngineConfiguration as ProcessEngineConfigurationImpl;
            repositoryService = processEngine.RepositoryService;
            runtimeService = processEngine.RuntimeService;
            taskService = processEngine.TaskService;
            HistoryService = processEngine.HistoryService;
            identityService = processEngine.IdentityService;
            managementService = processEngine.ManagementService;
            formService = processEngine.FormService;
            authorizationService = processEngine.AuthorizationService;
            caseService = processEngine.CaseService;
            filterService = processEngine.FilterService;
            externalTaskService = processEngine.ExternalTaskService;
            decisionService = processEngine.DecisionService;
        }

        protected internal virtual void ClearServiceReferences()
        {
            processEngineConfiguration = null;
            repositoryService = null;
            runtimeService = null;
            taskService = null;
            formService = null;
            HistoryService = null;
            identityService = null;
            managementService = null;
            authorizationService = null;
            caseService = null;
            filterService = null;
            externalTaskService = null;
            decisionService = null;
        }
        [TearDown]
        public  void Finished()
        {
            identityService.ClearAuthentication();
            //processEngine.ProcessEngineConfiguration.SetTenantCheckEnabled(true);

            TestHelper.AnnotationDeploymentTearDown(processEngine, deploymentId,this.GetType(), TestContext.CurrentContext.Test.Name);
            foreach (string additionalDeployment in additionalDeployments)
            {
                TestHelper.DeleteDeployment(processEngine, additionalDeployment);
            }

            if (ensureCleanAfterTest)
            {
                TestHelper.AssertAndEnsureCleanDbAndCache(processEngine);
            }

            TestHelper.ResetIdGenerator(processEngineConfiguration);
            ClockUtil.Reset();


            ClearServiceReferences();
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


        public virtual IProcessEngine ProcessEngine
        {
            get
            {
                return processEngine;
            }
            set
            {
                this.processEngine = value;
            }
        }


        public virtual ProcessEngineConfigurationImpl ProcessEngineConfiguration
        {
            get
            {
                return processEngineConfiguration;
            }
            set
            {
                this.ProcessEngineConfiguration = value;
            }
        }


        public IRepositoryService RepositoryService
        {
            get
            {
                return repositoryService;
            }
            set
            {
                this.RepositoryService = value;
            }
        }


        public IRuntimeService RuntimeService
        {
            get
            {
                return runtimeService;
            }
            set
            {
                this.runtimeService = value;
            }
        }


        public ITaskService TaskService
        {
            get
            {
                return taskService;
            }
            set
            {
                this.TaskService = value;
            }
        }


        public IHistoryService HistoryService { get; set; }


        /// <seealso cref= #setHistoryService(HistoryService) </seealso>
        /// <param name="historicService">
        ///          the historiy service instance </param>
        public virtual IHistoryService HistoricDataService
        {
            set
            {
                this.HistoryService = value;
            }
        }

        public IIdentityService IdentityService
        {
            get
            {
                return identityService;
            }
            set
            {
                this.identityService = value;
            }
        }


        public IManagementService ManagementService
        {
            get
            {
                return managementService;
            }
            set
            {
                this.ManagementService = value;
            }
        }

        public  IAuthorizationService IAuthorizationService
        {
            get
            {
                return authorizationService;
            }
            set
            {
                this.authorizationService = value;
            }
        }


        public  ICaseService CaseService
        {
            get
            {
                return caseService;
            }
            set
            {
                this.CaseService = value;
            }
        }


        public  IFormService FormService
        {
            get
            {
                return formService;
            }
            set
            {
                this.FormService = value;
            }
        }



        public  IFilterService FilterService
        {
            get
            {
                return filterService;
            }
            set
            {
                this.FilterService = value;
            }
        }


        public  IExternalTaskService ExternalTaskService
        {
            get
            {
                return externalTaskService;
            }
            set
            {
                this.ExternalTaskService = value;
            }
        }


        public  IDecisionService DecisionService
        {
            get
            {
                return decisionService;
            }
            set
            {
                this.DecisionService = value;
            }
        }


        public virtual void ManageDeployment(IDeployment deployment)
        {
            this.additionalDeployments.Add(deployment.Id);
        }

        public IAuthorizationService AuthorizationService { get; }
    }
}
