using System;

namespace ESS.FW.Bpm.Engine.Form.Impl.Validator
{
    /// <summary>
    ///     
    /// </summary>
    public class FormFieldConfigurationException : FormException
    {
        

        protected internal string configuration;

        public FormFieldConfigurationException(string configuration)
        {
            this.configuration = configuration;
        }

        public FormFieldConfigurationException(string configuration, string message, System.Exception cause)
            : base(message, cause)
        {
            this.configuration = configuration;
        }

        public FormFieldConfigurationException(string configuration, string message) : base(message)
        {
            this.configuration = configuration;
        }

        public FormFieldConfigurationException(string configuration, System.Exception cause) : base(cause)
        {
            this.configuration = configuration;
        }

        public virtual string Configuration
        {
            get { return configuration; }
        }
    }
}