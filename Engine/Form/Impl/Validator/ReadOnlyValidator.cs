namespace ESS.FW.Bpm.Engine.Form.Impl.Validator
{
    /// <summary>
    ///     
    /// </summary>
    public class ReadOnlyValidator : IFormFieldValidator
    {
        public virtual bool Validate(object submittedValue, IFormFieldValidatorContext validatorContext)
        {
            // no value was submitted
            return submittedValue == null;
        }
    }
}