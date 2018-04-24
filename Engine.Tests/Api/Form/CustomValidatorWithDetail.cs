using System;
using ESS.FW.Bpm.Engine.Form.Impl.Validator;

namespace Engine.Tests.Api.Form
{
    /// <summary>
    ///     
    /// </summary>
    public class CustomValidatorWithDetail : IFormFieldValidator
    {
        public CustomValidatorWithDetail()
        {
            Console.WriteLine("CREATED");
        }

        public virtual bool Validate(object submittedValue, IFormFieldValidatorContext validatorContext)
        {
            if (submittedValue == null)
                return true;

            if (submittedValue.Equals("A") || submittedValue.Equals("B"))
                return true;

            if (submittedValue.Equals("C"))
                throw new FormFieldValidationException("EXPIRED", "Unable to validate " + submittedValue);

            // return false in the generic case
            return false;
        }
    }
}