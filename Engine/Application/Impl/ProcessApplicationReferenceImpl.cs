using System;
using ESS.FW.Bpm.Engine.Impl;

namespace ESS.FW.Bpm.Engine.Application.Impl
{
    /// <summary>
    ///     <para>
    ///         A <seealso cref="IProcessApplicationReference" /> implementation using
    ///         <seealso cref="WeakReference" />.
    ///     </para>
    ///     <para>
    ///         As long as the process application is deployed, the container or the
    ///         application will hold a strong reference to the <seealso cref="AbstractProcessApplication" />
    ///         object. This class holds a <seealso cref="WeakReference" />. When the process
    ///         application is undeployed, the container or application releases all strong
    ///         references. Since we only pass {@link ProcessApplicationReference
    ///         ProcessApplicationReferences} to the process engine, it is guaranteed that
    ///         the <seealso cref="AbstractProcessApplication" /> object can be reclaimed by the garbage
    ///         collector, even if the undeployment and unregistration should fail for some
    ///         improbable reason.
    ///     </para>
    ///     
    /// </summary>
    public class ProcessApplicationReferenceImpl : IProcessApplicationReference
    {
        private static readonly ProcessApplicationLogger Log = ProcessEngineLogger.ProcessApplicationLogger;

        protected internal string name;

        /// <summary>
        ///     the weak reference to the process application
        /// </summary>
        protected internal WeakReference<AbstractProcessApplication> processApplication;

        public ProcessApplicationReferenceImpl(AbstractProcessApplication processApplication)
        {
            this.processApplication = new WeakReference<AbstractProcessApplication>(processApplication);
            name = processApplication.Name;
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public org.camunda.bpm.application.AbstractProcessApplication getProcessApplication() throws org.camunda.bpm.application.ProcessApplicationUnavailableException
        public virtual AbstractProcessApplication ProcessApplication
        {
            get
            {
                //AbstractProcessApplication application = processApplication;
                //if (application == null)
                //{
                //    throw LOG.processApplicationUnavailableException(name);
                //}
                //return application;
                return null;
            }
        }

        public virtual string Name
        {
            get { return name; }
        }

        IProcessApplicationInterface IProcessApplicationReference.ProcessApplication
        {
            get { throw new NotImplementedException(); }
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void processEngineStopping(org.camunda.bpm.engine.ProcessEngine processEngine) throws org.camunda.bpm.application.ProcessApplicationUnavailableException
        public virtual void ProcessEngineStopping(IProcessEngine processEngine)
        {
            // do nothing
        }

        public virtual void Clear()
        {
            //processApplication.clear();
        }
    }
}