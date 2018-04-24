using System;

namespace ESS.FW.Bpm.Engine.Form.Impl
{
    /// <summary>
    ///     
    /// </summary>
    public class FormException : ProcessEngineException
    {
        

        public FormException()
        {
        }

        public FormException(string message, System.Exception cause) : base(message, cause)
        {
        }

        public FormException(string message) : base(message)
        {
        }

        public FormException(System.Exception cause) : base(cause)
        {
        }
    }
}