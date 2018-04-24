using System;

namespace ESS.FW.Bpm.Engine.Form.Impl.Validator
{
    /// <summary>
    ///     Runtime exception for use within a <seealso cref="IFormFieldValidator" />.
    ///     Optionally contains a detail which uniquely identifies the problem.
    ///     
    /// </summary>
    public class FormFieldValidationException : FormException
    {
        

        /// <summary>
        ///     optional object for detailing the nature of the validation error
        /// </summary>
        protected internal object Detail;

        public FormFieldValidationException()
        {
        }

        public FormFieldValidationException(object detail)
        {
            this.Detail = detail;
        }

        public FormFieldValidationException(object detail, string message, System.Exception cause) : base(message, cause)
        {
            this.Detail = detail;
        }

        public FormFieldValidationException(object detail, string message) : base(message)
        {
            this.Detail = detail;
        }

        public FormFieldValidationException(object detail, System.Exception cause) : base(cause)
        {
            this.Detail = detail;
        }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> T getDetail()
        public virtual T GetDetail<T>()
        {
            return (T) Detail;
        }
    }
}