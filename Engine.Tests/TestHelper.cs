using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Engine.Tests.Test;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Dmn.Impl.Deployer;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Deployer;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests
{
    /// <summary>
    /// </summary>
    public abstract class TestHelper
    {
        protected internal static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;
        public const string EmptyLine =
            "                                                                                           ";

        public static readonly IList<string> TablenamesExcludedFromDbCleanCheck = new List<string> { "ACT_GE_PROPERTY" };

        internal static IDictionary<string, IProcessEngine> ProcessEngines = new Dictionary<string, IProcessEngine>();

        public static readonly IList<string> ResourceSuffixes = new List<string>();

        static TestHelper()
        {
            ((List<string>)ResourceSuffixes).AddRange(BpmnDeployer.BpmnResourceSuffixes);
            //((List<string>)RESOURCE_SUFFIXES).AddRange(CmmnDeployer.CMMN_RESOURCE_SUFFIXES);
            ((List<string>)ResourceSuffixes).AddRange(DecisionDefinitionDeployer.DmnResourceSuffixes);
        }

        /// <summary>
        ///     use <seealso cref="ProcessEngineAssert" /> instead.
        /// </summary>
        [Obsolete]
        public static void AssertProcessEnded(IProcessEngine processEngine, string processInstanceId)
        {
            ProcessEngineAssert.AssertProcessEnded(processEngine, processInstanceId);
        }

        public static string AnnotationDeploymentSetUp(IProcessEngine processEngine, Type testClass, string methodName,
            DeploymentAttribute deploymentAnnotation)
        {

            string deploymentId = null;
            MethodInfo method = null;
            var onMethod = true;

            try
            {
                method = GetMethod(testClass, methodName);
            }
            catch (System.Exception)
            {
                if (deploymentAnnotation == null)
                    return null;
            }

            if (deploymentAnnotation == null)
                deploymentAnnotation = method.GetCustomAttribute<DeploymentAttribute>(true);
            // if not found on method, try on class level
            if (deploymentAnnotation == null)
            {
                onMethod = false;
                var lookForAnnotationClass = testClass;
                while (lookForAnnotationClass != typeof(object))
                {
                    deploymentAnnotation = lookForAnnotationClass.GetCustomAttribute<DeploymentAttribute>();
                    if (deploymentAnnotation != null)
                    {
                        testClass = lookForAnnotationClass;
                        break;
                    }
                    lookForAnnotationClass = lookForAnnotationClass.BaseType;
                }
            }

            if (deploymentAnnotation != null)
            {
                Debug.WriteLine("annotation Deployment creates deployment for {0}.{1}",
                    ClassNameUtil.GetClassNameWithoutPackage(testClass), methodName);
                var resources = deploymentAnnotation.Resources;
                if ((resources.Length == 0) && (method != null))
                {
                    var name = onMethod ? method.Name : null;
                    var resource = GetBpmnProcessDefinitionResource(testClass, name);
                    resources = new[] { resource };
                }
                if (methodName.IndexOf("_") > -1)
                {
                    methodName = methodName.Split('_')[1];
                }
                var deploymentBuilder =
                    processEngine.RepositoryService.CreateDeployment()
                        .Name(ClassNameUtil.GetClassNameWithoutPackage(testClass) + "." + methodName);

                foreach (var resource in resources)
                {
                    Log.LogDebug("加载资源文件：", resource);
                    deploymentBuilder.AddClasspathResource(resource);
                }
                   

                deploymentId = deploymentBuilder.Deploy().Id;
            }

            return deploymentId;
        }

        public static string AnnotationDeploymentSetUp(IProcessEngine processEngine, Type testClass, string methodName)
        {
            return AnnotationDeploymentSetUp(processEngine, testClass, methodName, null);
        }

        public static void AnnotationDeploymentTearDown(IProcessEngine processEngine, string deploymentId,
            Type testClass, string methodName)
        {
            Debug.WriteLine("annotation Deployment deletes deployment for {0}.{1}",
                ClassNameUtil.GetClassNameWithoutPackage(testClass), methodName);
            DeleteDeployment(processEngine, deploymentId);
        }

        public static void DeleteDeployment(IProcessEngine processEngine, string deploymentId)
        {
            if (!ReferenceEquals(deploymentId, null))
                processEngine.RepositoryService.DeleteDeployment(deploymentId, true);
        }

        /// <summary>
        ///     get a resource location by convention based on a class (type) and a
        ///     relative resource name. The return value will be the full classpath
        ///     location of the type, plus a suffix built from the name parameter:
        ///     <code>BpmnDeployer.BPMN_RESOURCE_SUFFIXES</code>.
        ///     The first resource matching a suffix will be returned.
        /// </summary>
        public static string GetBpmnProcessDefinitionResource(Type type, string name)
        {
            if (name.IndexOf("_") > -1)
            {
                name = name.Split('_')[1];
            }
            foreach (var suffix in ResourceSuffixes)
            {
                var resource = CreateResourceName(type, name, suffix);
                if (!ReflectUtil.Existed(resource))
                {
                    continue;
                }
                var inputStream = ReflectUtil.GetResourceAsStream(resource);
                if (inputStream == null)
                    continue;
                return resource;
            }
            return CreateResourceName(type, name, BpmnDeployer.BpmnResourceSuffixes[0]);
        }

        private static string CreateResourceName(Type type, string name, string suffix)
        {
            var r = new StringBuilder(type.FullName.Replace("ESS.FW.Bpm.Engine.Tests", "resources").Replace('.', '/'));
            if (!ReferenceEquals(name, null))
                r.Append("." + name);
            return r.Append("." + suffix).ToString();
        }

        public static bool AnnotationRequiredHistoryLevelCheck(IProcessEngine processEngine, Description description)
        {
            RequiredHistoryLevelAttribute annotation = description.GetAnnotation<RequiredHistoryLevelAttribute>(typeof(RequiredHistoryLevelAttribute));

            if (annotation != null)
            {
                return HistoryLevelCheck(processEngine, annotation);

            }
            else
            {
                return AnnotationRequiredHistoryLevelCheck(processEngine, description.TestClass, description.MethodName);
            }
        }

        private static bool HistoryLevelCheck(IProcessEngine processEngine, RequiredHistoryLevelAttribute annotation)
        {
            var processEngineConfiguration = (ProcessEngineConfigurationImpl)processEngine.ProcessEngineConfiguration;

            var requiredHistoryLevel = GetHistoryLevelForName(processEngineConfiguration.HistoryLevels, annotation.value);
            var currentHistoryLevel = processEngineConfiguration.HistoryLevel;

            return currentHistoryLevel.Id >= requiredHistoryLevel.Id;
        }

        private static IHistoryLevel GetHistoryLevelForName(IList<IHistoryLevel> historyLevels, string name)
        {
            foreach (var historyLevel in historyLevels)
                if (historyLevel.Name.Equals(name))
                    return historyLevel;
            throw new ArgumentException("Unknown history level: " + name);
        }

        public static bool AnnotationRequiredHistoryLevelCheck(IProcessEngine processEngine, Type testClass,
            string methodName)
        {
            var annotation = GetAnnotation<RequiredHistoryLevelAttribute>(processEngine, testClass, methodName,
                typeof(RequiredHistoryLevelAttribute));

            if (annotation != null)
                return HistoryLevelCheck(processEngine, annotation);
            return true;
        }

        private static T GetAnnotation<T>(IProcessEngine processEngine, Type testClass, string methodName,
            Type annotationClass) where T : Attribute
        {
            MethodInfo method = null;
            var annotation = default(T);

            try
            {
                method = GetMethod(testClass, methodName);
                annotation = (T)method.GetCustomAttribute(annotationClass);
            }
            catch (System.Exception ex)
            {
                // - ignore if we cannot access the method
                // - just try again with the class
                // => can for example be the case for parameterized tests where methodName does not correspond to the actual method name
                //    (note that method-level annotations still work in this
                //     scenario due to Description#getAnnotation in annotationRequiredHistoryLevelCheck)
            }

            // if not found on method, try on class level
            if (annotation == null)
                annotation = (T)testClass.GetCustomAttribute(annotationClass);
            return annotation;
        }

        protected internal static MethodInfo GetMethod(Type clazz, string methodName)
        {
            return clazz.GetMethod(methodName);
        }

        /// <summary>
        ///     Ensures that the deployment cache and database is clean after a test. If not the cache
        ///     and database will be cleared.
        /// </summary>
        /// <param name="processEngine"> the <seealso cref="IProcessEngine" /> to test </param>
        /// <exception cref="AssertionError"> if the deployment cache or database was not clean </exception>
        public static void AssertAndEnsureCleanDbAndCache(IProcessEngine processEngine)
        {
            AssertAndEnsureCleanDbAndCache(processEngine, true);
        }

        /// <summary>
        ///     Ensures that the deployment cache and database is clean after a test. If not the cache
        ///     and database will be cleared.
        /// </summary>
        /// <param name="processEngine"> the <seealso cref="IProcessEngine" /> to test </param>
        /// <param name="Assert.Fail">
        ///     if true the method will throw an <seealso cref="AssertionError" /> if the deployment cache
        ///     or database is not clean
        /// </param>
        /// <exception cref="AssertionError"> if the deployment cache or database was not clean </exception>
        public static string AssertAndEnsureCleanDbAndCache(IProcessEngine processEngine, bool fail)
        {
            var processEngineConfiguration =
                (ProcessEngineConfigurationImpl)((ProcessEngineImpl)processEngine).ProcessEngineConfiguration;

            // clear IUser operation log in case some operations are
            // executed with an authenticated user
            ClearUserOperationLog(processEngineConfiguration);

            Debug.WriteLine("verifying that db is clean after test");
            var purgeReport = ((ManagementServiceImpl)processEngine.ManagementService).Purge();

            var paRegistrationMessage = AssertAndEnsureNoProcessApplicationsRegistered(processEngine);

            var message = new StringBuilder();
            var cachePurgeReport = purgeReport.CachePurgeReport;
            if (!cachePurgeReport.Empty)
                message.Append("IDeployment cache is not clean:\n").Append(cachePurgeReport.PurgeReportAsString);
            else
                Debug.WriteLine("IDeployment cache was clean.");
            var databasePurgeReport = purgeReport.DatabasePurgeReport;
            if (!databasePurgeReport.Empty)
                message.Append("Database is not clean:\n").Append(databasePurgeReport.PurgeReportAsString);
            else
                Debug.WriteLine("Database was clean.");
            if (!ReferenceEquals(paRegistrationMessage, null))
                message.Append(paRegistrationMessage);

            if (fail && (message.Length > 0))
                Assert.Fail(message.ToString());
            return message.ToString();
        }

        /// <summary>
        ///     Ensures that the deployment cache is empty after a test. If not the cache
        ///     will be cleared.
        /// </summary>
        /// <param name="processEngine"> the <seealso cref="IProcessEngine" /> to test </param>
        /// <exception cref="AssertionError"> if the deployment cache was not clean </exception>
        public static void AssertAndEnsureCleanDeploymentCache(IProcessEngine processEngine)
        {
            AssertAndEnsureCleanDeploymentCache(processEngine, true);
        }

        /// <summary>
        ///     Ensures that the deployment cache is empty after a test. If not the cache
        ///     will be cleared.
        /// </summary>
        /// <param name="processEngine"> the <seealso cref="IProcessEngine" /> to test </param>
        /// <param name="Assert.Fail">
        ///     if true the method will throw an <seealso cref="AssertionError" /> if the deployment cache
        ///     is not clean
        /// </param>
        /// <returns> the deployment cache summary if Assert.Fail is set to false or null if deployment cache was clean </returns>
        /// <exception cref="AssertionError"> if the deployment cache was not clean and Assert.Fail is set to true </exception>
        public static string AssertAndEnsureCleanDeploymentCache(IProcessEngine processEngine, bool fail)
        {
            var outputMessage = new StringBuilder();
            var processEngineConfiguration =
                (ProcessEngineConfigurationImpl)((ProcessEngineImpl)processEngine).ProcessEngineConfiguration;
            var cachePurgeReport = processEngineConfiguration.DeploymentCache.PurgeCache();

            outputMessage.Append(cachePurgeReport.PurgeReportAsString);
            if (outputMessage.Length > 0)
            {
                outputMessage.Insert(0, "IDeployment cache not clean:\n");
                Debug.WriteLine(outputMessage.ToString());

                if (fail)
                    Assert.Fail(outputMessage.ToString());

                return outputMessage.ToString();
            }
            Debug.WriteLine("IDeployment cache was clean");
            return null;
        }


        public static string AssertAndEnsureNoProcessApplicationsRegistered(IProcessEngine processEngine)
        {
            var engineConfiguration = (ProcessEngineConfigurationImpl)processEngine.ProcessEngineConfiguration;
            var processApplicationManager = engineConfiguration.ProcessApplicationManager;

            if (processApplicationManager.HasRegistrations())
            {
                processApplicationManager.ClearRegistrations();
                return "There are still process applications registered";
            }
            return null;
        }

        public static void WaitForJobExecutorToProcessAllJobs(ProcessEngineConfigurationImpl processEngineConfiguration,
            long maxMillisToWait, long intervalMillis)
        {
            ESS.FW.Bpm.Engine.Impl.JobExecutor.JobExecutor jobExecutor = processEngineConfiguration.JobExecutor;
            jobExecutor.Start();

            //try
            //{
            //  Timer timer = new Timer();
            //  InteruptTask task = new InteruptTask(Thread.CurrentThread);
            //  timer.Schedule(task, maxMillisToWait);
            //  bool areJobsAvailable = true;
            //  try
            //  {
            //	while (areJobsAvailable && !task.TimeLimitExceeded)
            //	{
            //	  Thread.Sleep(intervalMillis);
            //	  areJobsAvailable = areJobsAvailable(processEngineConfiguration);
            //	}
            //  }
            //  //catch (InterruptedException)
            //  //{
            //  //}
            //  finally
            //  {
            //	timer.Cancel();
            //  }
            //  if (areJobsAvailable)
            //  {
            //	throw new ProcessEngineException("time limit of " + maxMillisToWait + " was exceeded");
            //  }

            //}
            //finally
            //{
            //  jobExecutor.Shutdown();
            //}
        }

        public static bool AreJobsAvailable(ProcessEngineConfigurationImpl processEngineConfiguration)
        {
            //TODO JobQuery Executable() Job.xml是否可执行逻辑在xml中？
            return processEngineConfiguration.ManagementService.CreateJobQuery()/*.Executable()*/.Count() > 0;
        }

        public static void ResetIdGenerator(ProcessEngineConfigurationImpl processEngineConfiguration)
        {
            //IdGenerator idGenerator = processEngineConfiguration.IdGenerator;

            //if (idGenerator is DbIdGenerator)
            //{
            //  ((DbIdGenerator) idGenerator).Reset();
            //}
        }

        // private class InteruptTask : TimerTask
        // {
        //protected internal bool timeLimitExceeded = false;
        //protected internal Thread Thread;
        //public InteruptTask(Thread thread)
        //{
        //  this.Thread = thread;
        //}
        //public virtual bool TimeLimitExceeded
        //{
        //	get
        //	{
        //	  return timeLimitExceeded;
        //	}
        //}
        //public override void run()
        //{
        //  timeLimitExceeded = true;
        //  Thread.Interrupt();
        //}
        // }

        public static IProcessEngine GetProcessEngine(string configurationResource)
        {
            IProcessEngine processEngine;
            ProcessEngines.TryGetValue(configurationResource,out processEngine);
            if (processEngine == null)
            {
                Debug.WriteLine(
                    "==== BUILDING PROCESS ENGINE ========================================================================");
                processEngine =
                    ProcessEngineConfiguration.CreateProcessEngineConfigurationFromResource(configurationResource)
                        .BuildProcessEngine();
                Debug.WriteLine(
                    "==== PROCESS ENGINE CREATED =========================================================================");
                ProcessEngines[configurationResource] = processEngine;
            }
            return processEngine;
        }

        public static void CloseProcessEngines()
        {
            foreach (var processEngine in ProcessEngines.Values)
                processEngine.Close();
            ProcessEngines.Clear();
        }

        public static void CreateSchema(ProcessEngineConfigurationImpl processEngineConfiguration)
        {
            processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass());
        }

        public static void DropSchema(ProcessEngineConfigurationImpl processEngineConfiguration)
        {
            processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass2());
        }

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public static void createOrUpdateHistoryLevel(final impl.cfg.ProcessEngineConfigurationImpl processEngineConfiguration)
        public static void CreateOrUpdateHistoryLevel(ProcessEngineConfigurationImpl processEngineConfiguration)
        {
            processEngineConfiguration.CommandExecutorTxRequired.Execute(
                new CommandAnonymousInnerClass3(processEngineConfiguration));
        }

        public static void DeleteHistoryLevel(ProcessEngineConfigurationImpl processEngineConfiguration)
        {
            processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass4());
        }

        public static void ClearUserOperationLog(ProcessEngineConfigurationImpl processEngineConfiguration)
        {
            if (processEngineConfiguration.HistoryLevel.Equals(HistoryLevelFields.HistoryLevelFull))
            {
                var historyService = processEngineConfiguration.HistoryService;
                // IList<IUserOperationLogEntry> logs = historyService.CreateUserOperationLogQuery().ToList();
                // foreach (IUserOperationLogEntry log in logs)
                // {
                //historyService.DeleteUserOperationLogEntry(log.Id);
                // }
            }
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            public virtual object Execute(CommandContext commandContext)
            {
                //commandContext.GetSession(typeof(IPersistenceSession)).DbSchemaCreate();
                return null;
            }
        }

        private class CommandAnonymousInnerClass2 : ICommand<object>
        {
            public virtual object Execute(CommandContext commandContext)
            {
                //commandContext.DbSqlSession.DbSchemaDrop();
                return null;
            }
        }

        private class CommandAnonymousInnerClass3 : ICommand<object>
        {
            private ProcessEngineConfigurationImpl _processEngineConfiguration;

            public CommandAnonymousInnerClass3(ProcessEngineConfigurationImpl processEngineConfiguration)
            {
                _processEngineConfiguration = processEngineConfiguration;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                var dbEntityManager = commandContext.PropertyManager;
                PropertyEntity historyLevelProperty = dbEntityManager.Get("historyLevel");
                if (historyLevelProperty != null)
                {
                    if (_processEngineConfiguration.HistoryLevel.Id != new int?(Convert.ToInt32(historyLevelProperty.Value)))
                    {
                        historyLevelProperty.Value = Convert.ToString(_processEngineConfiguration.HistoryLevel.Id);
                        dbEntityManager.Update(historyLevelProperty);
                    }
                }
                else
                {
                    SchemaOperationsProcessEngineBuild.DbCreateHistoryLevel(commandContext);
                }
                return null;
            }
        }

        private class CommandAnonymousInnerClass4 : ICommand<object>
        {
            public virtual object Execute(CommandContext commandContext)
            {
                var dbEntityManager = commandContext.PropertyManager;
                PropertyEntity historyLevelProperty = dbEntityManager.Get("historyLevel");
                if (historyLevelProperty != null)
                {
                    dbEntityManager.Delete(historyLevelProperty);
                }
                return null;
            }
        }
    }
}