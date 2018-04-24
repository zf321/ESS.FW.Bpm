

using System;
using ESS.FW.Bpm.Engine.Impl;

namespace ESS.FW.Bpm.Engine.Application.Impl
{
    /// <summary>
    ///     <para>Common base class for writing EJB process applications.</para>
    ///     <para>
    ///         An EJB Process Application exposes itself as a Session Bean Component inside the EJB container.
    ///         This determines the invocation semantics when invoking code from the process application and the
    ///         nature of the <seealso cref="IProcessApplicationReference" /> held by the process engine.
    ///     </para>
    ///     <h2>Usage</h2>
    ///     <para>
    ///         In order to add a custom EJB process application to your application, extend this class and configure
    ///         it as follows:
    ///     </para>
    ///     <pre>
    ///         {@literal @}ProcessApplication("my process application")
    ///         {@literal @}Singleton
    ///         {@literal @}Startup
    ///         {@literal @}ConcurrencyManagement(ConcurrencyManagementType.BEAN)
    ///         {@literal @}TransactionAttribute(TransactionAttributeType.REQUIRED)
    ///         public class DefaultEjbProcessApplication extends EjbProcessApplication {
    ///         {@literal @}PostConstruct
    ///         public void deploy() {
    ///         super.deploy();
    ///         }
    ///         {@literal @}PreDestroy
    ///         public void undeploy() {
    ///         super.undeploy();
    ///         }
    ///         }
    ///     </pre>
    ///     <para>
    ///         (the same Metadata can of course be provided using an XML-based <code>ejb-jar.xml</code>
    ///         deployment descriptor
    ///     </para>
    ///     <h2>Invocation Semantics</h2>
    ///     <para>
    ///         This allows the process engine as well as other applications to invoke this EJB Process
    ///         Application and get EJB invocation semantics for the invocation. For example, if your
    ///         process application provides a <seealso cref="JavaDelegate" /> implementation, the process engine
    ///         will call the {@link EjbProcessApplication EjbProcessApplication's}
    ///         <seealso cref="#execute(java.Util.concurrent.Callable)" /> MethodInfo and from that method invoke
    ///         the <seealso cref="JavaDelegate" />. This makes sure that
    ///         <ul>
    ///             <li>the call is intercepted by the EJB container and "enters" the process application legally.</li>
    ///             <li>
    ///                 the <seealso cref="JavaDelegate" /> may take advantage of the <seealso cref="EjbProcessApplication" />
    ///                 's invocation context
    ///                 and resolve resources from the component's Environment (such as a <code>java:comp/BeanManager</code>).
    ///         </ul>
    ///     </para>
    ///     <pre>
    ///         Big pile of EJB interceptors
    ///         |
    ///         |  +--------------------+
    ///         |  |Process Application |
    ///         invoke        v  |                    |
    ///         ProcessEngine ----------------OOOOO--> Java Delegate   |
    ///         |                    |
    ///         |                    |
    ///         +--------------------+
    ///     </pre>
    ///     <h2>Process Application Reference</h2>
    ///     <para>
    ///         When the process application registers with a process engine
    ///         (see <seealso cref="IManagementService#registerProcessApplication(String, ProcessApplicationReference)" />,
    ///         the process application passes a reference to itself to the process engine. This reference allows the
    ///         process engine to reference the process application. The <seealso cref="EjbProcessApplication" /> takes
    ///         advantage
    ///         of the Ejb Containers naming context and passes a reference containing the EJBProcessApplication's
    ///         Component Name to the process engine. Whenever the process engine needs access to process application,
    ///         the actual component instance is looked up and invoked.
    ///     </para>
    ///     
    /// </summary>
    public class EjbProcessApplication : AbstractProcessApplication
    {
        private static readonly ProcessApplicationLogger Log = ProcessEngineLogger.ProcessApplicationLogger;

        protected internal static string ModuleNamePath = "java:module/ModuleName";
        protected internal static string JavaAppAppNamePath = "java:app/AppName";
        protected internal static string EjbContextPath = "java:comp/EJBContext";

        private EjbProcessApplicationReference _ejbProcessApplicationReference;
        private IProcessApplicationInterface _selfReference;

        public override IProcessApplicationReference Reference
        {
            get
            {
                EnsureInitialized();
                return _ejbProcessApplicationReference;
            }
        }

        /// <summary>
        ///     allows subclasses to provide a custom business interface
        /// </summary>
        protected internal virtual Type BusinessInterface
        {
            get { return typeof(IProcessApplicationInterface); }
        }

        protected internal override string AutodetectProcessApplicationName()
        {
            return LookupEeApplicationName();
        }

//JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public <T> T execute(java.Util.concurrent.Callable<T> callable) throws org.camunda.bpm.application.ProcessApplicationExecutionException
        // public override T execute<T>(Callable<T> callable)
        // {
        //ClassLoader originalClassloader = ClassLoaderUtil.ContextClassloader;
        //ClassLoader processApplicationClassloader = ProcessApplicationClassloader;

        //try
        //{
        //  if (originalClassloader != processApplicationClassloader)
        //  {
        //	ClassLoaderUtil.ContextClassloader = processApplicationClassloader;
        //  }

        //  return callable.call();

        //}
        //catch (Exception e)
        //{
        //  throw LOG.processApplicationExecutionException(e);
        //}
        //finally
        //{
        //  ClassLoaderUtil.ContextClassloader = originalClassloader;
        //}
        // }

        protected internal virtual void EnsureInitialized()
        {
            if (_selfReference == null)
                _selfReference = LookupSelfReference();
            if (_ejbProcessApplicationReference == null)
                _ejbProcessApplicationReference = new EjbProcessApplicationReference(_selfReference, Name);
        }

        /// <summary>
        ///     lookup a proxy object representing the invoked business view of this component.
        /// </summary>
        protected internal virtual IProcessApplicationInterface LookupSelfReference()
        {
            throw new NotImplementedException();
            //try
            //{
            //    InitialContext ic = new InitialContext();
            //    SessionContext sctxLookup = (SessionContext) ic.lookup(EJB_CONTEXT_PATH);
            //    return sctxLookup.getBusinessObject(BusinessInterface);
            //}
            //catch (NamingException e)
            //{
            //    throw LOG.ejbPaCannotLookupSelfReference(e);
            //}
            return null;
        }

        /// <summary>
        ///     determine the ee application name based on information obtained from JNDI.
        /// </summary>
        protected internal virtual string LookupEeApplicationName()
        {
            try
            {
                throw new NotImplementedException();
                //InitialContext initialContext = new InitialContext();

                //var appName = (string) initialContext.lookup(JAVA_APP_APP_NAME_PATH);
                //var moduleName = (string) initialContext.lookup(MODULE_NAME_PATH);

                //// make sure that if an EAR carries multiple PAs, they are correctly
                //// identified by appName + moduleName
                //if (!ReferenceEquals(moduleName, null) && !moduleName.Equals(appName))
                //{
                //    return appName + "/" + moduleName;
                //}
                //return appName;
            }
            catch (System.Exception e)
            {
                //throw LOG.ejbPaCannotAutodetectName(e);
            }
            return string.Empty;
        }
    }
}