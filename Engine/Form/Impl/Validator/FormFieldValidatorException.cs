using System;

namespace ESS.FW.Bpm.Engine.Form.Impl.Validator
{
    /// <summary>
    ///     Runtime exception for validation of form fields.
    ///     
    /// </summary>
    public class FormFieldValidatorException : FormException
    {
        
        protected internal readonly string config;

        /// <summary>
        ///     bpmn element id
        /// </summary>
        protected internal readonly string id;

        protected internal readonly string name;
        protected internal readonly object value;

        public FormFieldValidatorException(string id, string name, string config, object value, string message,
            System.Exception cause) : base(message, cause)
        {
            this.id = id;
            this.name = name;
            this.config = config;
            this.value = value;
        }

        public FormFieldValidatorException(string id, string name, string config, object value, string message)
            : base(message)
        {
            this.id = id;
            this.name = name;
            this.config = config;
            this.value = value;
        }

        public virtual string Name
        {
            get { return name; }
        }

        public virtual string Config
        {
            get { return config; }
        }

        public virtual object Value
        {
            get { return value; }
        }

        public virtual string Id
        {
            get { return id; }
        }
    }
}