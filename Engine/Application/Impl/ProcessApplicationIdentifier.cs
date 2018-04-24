namespace ESS.FW.Bpm.Engine.Application.Impl
{
    /// <summary>
    ///     
    /// </summary>
    public class ProcessApplicationIdentifier
    {
        protected internal string name;
        protected internal IProcessApplicationInterface processApplication;
        protected internal IProcessApplicationReference reference;

        public ProcessApplicationIdentifier(string name)
        {
            this.name = name;
        }

        public ProcessApplicationIdentifier(IProcessApplicationReference reference)
        {
            this.reference = reference;
        }

        public ProcessApplicationIdentifier(IProcessApplicationInterface processApplication)
        {
            this.processApplication = processApplication;
        }

        public virtual string Name
        {
            get { return name; }
        }

        public virtual IProcessApplicationReference Reference
        {
            get { return reference; }
        }

        public virtual IProcessApplicationInterface ProcessApplication
        {
            get { return processApplication; }
        }

        public override string ToString()
        {
            var paName = name;
            if (ReferenceEquals(paName, null) && (reference != null))
                paName = reference.Name;
            if (ReferenceEquals(paName, null) && (processApplication != null))
                paName = processApplication.Name;
            return "ProcessApplicationIdentifier[name=" + paName + "]";
        }
    }
}