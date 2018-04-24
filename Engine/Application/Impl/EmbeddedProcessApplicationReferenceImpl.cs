namespace ESS.FW.Bpm.Engine.Application.Impl
{
    /// <summary>
    ///     <para>The process engine holds a strong reference to the embedded process application.</para>
    ///     
    /// </summary>
    public class EmbeddedProcessApplicationReferenceImpl : IProcessApplicationReference
    {
        protected internal EmbeddedProcessApplication Application;

        public EmbeddedProcessApplicationReferenceImpl(EmbeddedProcessApplication application)
        {
            this.Application = application;
        }

        public virtual string Name
        {
            get { return Application.Name; }
        }

//JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.application.ProcessApplicationInterface getProcessApplication() throws org.camunda.bpm.application.ProcessApplicationUnavailableException
        public virtual IProcessApplicationInterface ProcessApplication
        {
            get { return Application; }
        }
    }
}