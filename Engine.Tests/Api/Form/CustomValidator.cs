using ESS.FW.Bpm.Engine.Form.Impl.Validator;

namespace Engine.Tests.Api.Form
{
    /// <summary>
    /// </summary>
    public class CustomValidator : IFormFieldValidator
    {
        public virtual bool Validate(object submittedValue, IFormFieldValidatorContext validatorContext)
        {
            return submittedValue == null || submittedValue.Equals("validValue");
        }
    }
}