using System;
using System.Collections.Generic;
using System.IO;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine
{
    /// <summary>
    ///     Helper for initializing and closing process engines in server environments.
    ///     <br>
    ///         All created <seealso cref="IProcessEngine" />s will be registered with this class.
    ///         <br>
    ///             The activiti-webapp-init webapp will
    ///             call the <seealso cref="#init()" /> method when the webapp is deployed and it will call the
    ///             <seealso cref="#destroy()" /> method when the webapp is destroyed, using a context-listener
    ///             (<code>org.camunda.bpm.engine.test.impl.servlet.listener.ProcessEnginesServletContextListener</code>).
    ///             That way,
    ///             all applications can just use the <seealso cref="#getProcessEngines()" /> to
    ///             obtain pre-initialized and cached process engines.
    ///             <br>
    ///                 <br>
    ///                     Please note that there is <b>no lazy initialization</b> of process engines, so make sure the
    ///                     context-listener is configured or <seealso cref="IProcessEngine" />s are already created so they
    ///                     were registered
    ///                     on this class.
    ///                     <br>
    ///                         <br>
    ///                             The <seealso cref="#init()" /> method will try to build one
    ///                             <seealso cref="IProcessEngine" /> for
    ///                             each camunda.cfg.xml file found on the classpath.  If you have more then one,
    ///                             make sure you specify different process.engine.name values.
    ///                              
    ///                             
    /// </summary>
    public abstract class ProcessEngines
    {
        public const string NameDefault = "default";

        private static readonly ProcessEngineLogger Log = ProcessEngineLogger.Instance;

        protected internal static bool IsInitialized;

        protected internal static IDictionary<string, IProcessEngine> processEngines =
            new Dictionary<string, IProcessEngine>();

        protected internal static IDictionary<string, IProcessEngineInfo> ProcessEngineInfosByName =
            new Dictionary<string, IProcessEngineInfo>();

        protected internal static IDictionary<string, IProcessEngineInfo> ProcessEngineInfosByResourceUrl =
            new Dictionary<string, IProcessEngineInfo>();

        protected internal static IList<IProcessEngineInfo> processEngineInfos = new List<IProcessEngineInfo>();

        /// <summary>
        ///     Get initialization results.
        /// </summary>
        public static IList<IProcessEngineInfo> ProcessEngineInfos
        {
            get { return processEngineInfos; }
        }

        public static IProcessEngine DefaultProcessEngine
        {
            get { return GetDefaultProcessEngine(true); }
        }

        public static void Init()
        {
            lock (typeof(ProcessEngines))
            {
                Init(true);
            }
        }

        /// <summary>
        ///     Initializes all process engines that can be found on the classpath for
        ///     resources <code>camunda.cfg.xml</code> (plain Activiti style configuration)
        ///     and for resources <code>activiti-context.xml</code> (Spring style configuration).
        /// </summary>
        public static void Init(bool forceCreate)
        {
            lock (typeof(ProcessEngines))
            {
                if (!IsInitialized)
                {
                    if (processEngines == null)
                        processEngines = new Dictionary<string, IProcessEngine>();
                    //ClassLoader classLoader = ReflectUtil.ClassLoader;
                    IEnumerator<Uri> resources = null;
                    try
                    {
                        //resources = classLoader.getResources("camunda.cfg.xml");
                    }
                    catch (IOException)
                    {
                        try
                        {
                            //resources = classLoader.getResources("activiti.cfg.xml");
                        }
                        catch (IOException ex)
                        {
                            if (forceCreate)
                            {
                                //throw new ProcessEngineException(
                                //    "problem retrieving camunda.cfg.xml and activiti.cfg.xml resources on the classpath: " +
                                //    System.getProperty("java.class.path"), ex);
                            }
                            return;
                        }
                    }

                    // Remove duplicated configuration Uri's using set. Some classloaders may return identical Uri's twice, causing duplicate startups
                    ISet<Uri> configUrls = new HashSet<Uri>();
                    while (resources.MoveNext())
                        configUrls.Add(resources.Current);
                    for (var iterator = configUrls.GetEnumerator(); iterator.MoveNext();)
                    {
                        var resource = iterator.Current;
                        InitProcessEngineFromResource(resource);
                    }

                    try
                    {
                        //resources = classLoader.getResources("activiti-context.xml");
                    }
                    catch (IOException e)
                    {
                        if (forceCreate)
                        {
                            //throw new ProcessEngineException(
                            //    "problem retrieving activiti-context.xml resources on the classpath: " +
                            //    System.getProperty("java.class.path"), e);
                        }
                        return;
                    }
                    while (resources.MoveNext())
                    {
                        var resource = resources.Current;
                        InitProcessEngineFromSpringResource(resource);
                    }

                    IsInitialized = true;
                }
                else
                {
                    Log.ProcessEngineAlreadyInitialized();
                }
            }
        }

        protected internal static void InitProcessEngineFromSpringResource(Uri resource)
        {
            try
            {
                var springConfigurationHelperClass =
                    ReflectUtil.LoadClass("org.camunda.bpm.engine.test.spring.SpringConfigurationHelper");
                var method = springConfigurationHelperClass.GetMethod("buildProcessEngine", new[] {typeof(Uri)});
                var processEngine = (IProcessEngine) method.Invoke(null, new object[] {resource});

                var processEngineName = processEngine.Name;
                IProcessEngineInfo processEngineInfo = new ProcessEngineInfoImpl(processEngineName, resource.ToString(),
                    null);
                ProcessEngineInfosByName[processEngineName] = processEngineInfo;
                ProcessEngineInfosByResourceUrl[resource.ToString()] = processEngineInfo;
            }
            catch (System.Exception e)
            {
                throw new ProcessEngineException(
                    "couldn't initialize process engine from spring configuration resource " + resource + ": " +
                    e.Message, e);
            }
        }

        /// <summary>
        ///     Registers the given process engine. No <seealso cref="IProcessEngineInfo" /> will be
        ///     available for this process engine. An engine that is registered will be closed
        ///     when the <seealso cref="ProcessEngines#destroy()" /> is called.
        /// </summary>
        public static void RegisterProcessEngine(IProcessEngine processEngine)
        {
            processEngines[processEngine.Name] = processEngine;
        }

        /// <summary>
        ///     Unregisters the given process engine.
        /// </summary>
        public static void Unregister(IProcessEngine processEngine)
        {
            processEngines.Remove(processEngine.Name);
        }

        private static IProcessEngineInfo InitProcessEngineFromResource(Uri resourceUrl)
        {
            var processEngineInfo = ProcessEngineInfosByResourceUrl[resourceUrl.LocalPath];
            // if there is an existing process engine info
            if (processEngineInfo != null)
            {
                // remove that process engine from the member fields
                processEngineInfos.Remove(processEngineInfo);
                if (ReferenceEquals(processEngineInfo.Exception, null))
                {
                    var processEngineName = processEngineInfo.Name;
                    processEngines.Remove(processEngineName);
                    ProcessEngineInfosByName.Remove(processEngineName);
                }
                ProcessEngineInfosByResourceUrl.Remove(processEngineInfo.ResourceUrl);
            }

            var resourceUrlString = resourceUrl.ToString();
            try
            {
                Log.InitializingProcessEngineForResource(resourceUrl);
                var processEngine = BuildProcessEngine(resourceUrl);
                var processEngineName = processEngine.Name;
                Log.InitializingProcessEngine(processEngine.Name);
                processEngineInfo = new ProcessEngineInfoImpl(processEngineName, resourceUrlString, null);
                processEngines[processEngineName] = processEngine;
                ProcessEngineInfosByName[processEngineName] = processEngineInfo;
            }
            catch (System.Exception e)
            {
                Log.ExceptionWhileInitializingProcessengine(e);
                processEngineInfo = new ProcessEngineInfoImpl(null, resourceUrlString, GetExceptionString(e));
            }
            ProcessEngineInfosByResourceUrl[resourceUrlString] = processEngineInfo;
            processEngineInfos.Add(processEngineInfo);
            return processEngineInfo;
        }

        private static string GetExceptionString(System.Exception e)
        {
            var sw = new StringWriter();
            //PrintWriter pw = new PrintWriter(sw);
            //e.printStackTrace(pw);
            return sw.ToString();
        }

        private static IProcessEngine BuildProcessEngine(Uri resource)
        {
            Stream inputStream = null;
            try
            {
                //inputStream = resource.openStream();
                var processEngineConfiguration =
                    ProcessEngineConfiguration.CreateProcessEngineConfigurationFromInputStream(inputStream);
                return processEngineConfiguration.BuildProcessEngine();
            }
            catch (IOException e)
            {
                throw new ProcessEngineException("couldn't open resource stream: " + e.Message, e);
            }
            finally
            {
                IoUtil.CloseSilently(inputStream);
            }
        }

        /// <summary>
        ///     Get initialization results. Only info will we available for process engines
        ///     which were added in the <seealso cref="ProcessEngines#init()" />. No <seealso cref="IProcessEngineInfo" />
        ///     is available for engines which were registered programatically.
        /// </summary>
        public static IProcessEngineInfo GetProcessEngineInfo(string processEngineName)
        {
            return ProcessEngineInfosByName[processEngineName];
        }

        public static IProcessEngine GetDefaultProcessEngine(bool forceCreate)
        {
            return GetProcessEngine(NameDefault, forceCreate);
        }

        public static IProcessEngine GetProcessEngine(string processEngineName)
        {
            return GetProcessEngine(processEngineName, true);
        }

        /// <summary>
        ///     obtain a process engine by name.
        /// </summary>
        /// <param name="processEngineName"> is the name of the process engine or null for the default process engine.   </param>
        public static IProcessEngine GetProcessEngine(string processEngineName, bool forceCreate)
        {
            if (!IsInitialized)
                Init(forceCreate);
            return processEngines[processEngineName];
        }

        /// <summary>
        ///     retries to initialize a process engine that previously failed.
        /// </summary>
        public static IProcessEngineInfo Retry(string resourceUrl)
        {
            try
            {
                return InitProcessEngineFromResource(new Uri(resourceUrl));
            }
            catch (System.Exception e)
            {
                throw new ProcessEngineException("invalid url: " + resourceUrl, e);
            }
        }

        /// <summary>
        ///     provides access to process engine to application clients in a
        ///     managed server environment.
        /// </summary>
        public static IDictionary<string, IProcessEngine> GetProcessEngines()
        {
            return processEngines;
        }

        /// <summary>
        ///     closes all process engines.  This method should be called when the server shuts down.
        /// </summary>
        public static void Destroy()
        {
            lock (typeof(ProcessEngines))
            {
                if (IsInitialized)
                {
                    IDictionary<string, IProcessEngine> engines = new Dictionary<string, IProcessEngine>(processEngines);
                    processEngines = new Dictionary<string, IProcessEngine>();

                    foreach (var processEngineName in engines.Keys)
                    {
                        var processEngine = engines[processEngineName];
                        try
                        {
                            processEngine.Close();
                        }
                        catch (System.Exception e)
                        {
                            Log.ExceptionWhileClosingProcessEngine(
                                ReferenceEquals(processEngineName, null)
                                    ? "the default process engine"
                                    : "process engine " + processEngineName, e);
                        }
                    }

                    ProcessEngineInfosByName.Clear();
                    ProcessEngineInfosByResourceUrl.Clear();
                    processEngineInfos.Clear();

                    IsInitialized = false;
                }
            }
        }
    }
}