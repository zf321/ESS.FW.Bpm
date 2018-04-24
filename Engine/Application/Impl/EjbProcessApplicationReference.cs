using System;
using ESS.FW.Bpm.Engine.Impl;

namespace ESS.FW.Bpm.Engine.Application.Impl
{
    /// <summary>
    ///     <para>A reference to an EJB process application.</para>
    ///     <para>An EJB process application is an EJB Session Bean that can be looked up in JNDI.</para>
    ///     
    /// </summary>
    public class EjbProcessApplicationReference : IProcessApplicationReference
    {
        private static readonly ProcessApplicationLogger Log = ProcessEngineLogger.ProcessApplicationLogger;

        /// <summary>
        ///     the name of the process application
        /// </summary>
        protected internal string ProcessApplicationName;

        /// <summary>
        ///     this is an EjbProxy and can be cached
        /// </summary>
        protected internal IProcessApplicationInterface SelfReference;

        public EjbProcessApplicationReference(IProcessApplicationInterface selfReference, string name)
        {
            this.SelfReference = selfReference;
            ProcessApplicationName = name;
        }

        public virtual string Name
        {
            get { return ProcessApplicationName; }
        }
        
        public virtual IProcessApplicationInterface ProcessApplication
        {
            get
            {
                try
                {
                    // check whether process application is still deployed
                    //selfReference.Name;
                    return null;
                }
                catch (System.Exception e)
                {
                    throw Log.ProcessApplicationUnavailableException(ProcessApplicationName, e);
                }
                return SelfReference;
            }
        }
        
        public virtual void ProcessEngineStopping(IProcessEngine processEngine)
        {
            // do nothing.
        }
    }
}