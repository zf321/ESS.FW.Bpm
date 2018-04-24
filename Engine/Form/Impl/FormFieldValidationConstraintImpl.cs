using System;

namespace ESS.FW.Bpm.Engine.Form.Impl
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class FormFieldValidationConstraintImpl : IFormFieldValidationConstraint
    {
        private const long SerialVersionUid = 1L;
        protected internal string configuration;

        protected internal string name;

        public FormFieldValidationConstraintImpl()
        {
        }

        public FormFieldValidationConstraintImpl(string name, string configuration)
        {
            this.name = name;
            this.configuration = configuration;
        }

        public object Configuration
        {
            get { throw new NotImplementedException(); }
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual object GetConfiguration()
        {
            return configuration;
        }

        public virtual void SetConfiguration(string configuration)
        {
            this.configuration = configuration;
        }
    }
}