using ESS.FW.Bpm.Engine.Impl;

namespace ESS.FW.Bpm.Engine.Application.Impl
{
    /// <summary>
    ///     <para>
    ///         An embedded process application is a ProcessApplication that uses an embedded
    ///         process engine. An embedded process engine is loaded by the same classloader as
    ///         the process application which usually means that the <code>camunda-engine.jar</code>
    ///         is deployed as a web application library (in case of WAR deployments) or as an
    ///         application library (in case of EAR deployments).
    ///     </para>
    ///     
    /// </summary>
    public class EmbeddedProcessApplication : AbstractProcessApplication
    {
        public const string DefaultName = "Process Application";
        private static ProcessApplicationLogger _log = ProcessEngineLogger.ProcessApplicationLogger;

        public override IProcessApplicationReference Reference
        {
            get { return new EmbeddedProcessApplicationReferenceImpl(this); }
        }

        protected internal override string AutodetectProcessApplicationName()
        {
            return DefaultName;
        }

        /// </summary>
        /// as the process application, nothing needs to be done.
        /// Since the process engine is loaded by the same classloader

        /// <summary>
//JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public <T> T execute(java.Util.concurrent.Callable<T> callable) throws org.camunda.bpm.application.ProcessApplicationExecutionException
        // public override T execute<T>(Callable<T> callable)
        // {
        //try
        //{
        //  return callable.call();
        //}
        //catch (Exception e)
        //{
        //  throw LOG.processApplicationExecutionException(e);
        //}
        // }
    }
}