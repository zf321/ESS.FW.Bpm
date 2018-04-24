namespace ESS.FW.Bpm.Engine.Form.Impl.Validator
{
    /// <summary>
    ///     
    /// </summary>
    public class RequiredValidator : IFormFieldValidator
    {
        public virtual bool Validate(object submittedValue, IFormFieldValidatorContext validatorContext)
        {
            if (submittedValue == null)
            {
                //ITypedValue value = validatorContext.VariableScope.getVariableTyped(validatorContext.FormFieldHandler.Id);
                //return (value != null && value.Value != null);
            }
            if (submittedValue is string)
                return (submittedValue != null) && (((string) submittedValue).Length > 0);
            return submittedValue != null;
        }
    }
}