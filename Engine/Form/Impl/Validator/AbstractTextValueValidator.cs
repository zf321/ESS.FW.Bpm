namespace ESS.FW.Bpm.Engine.Form.Impl.Validator
{
    /// <summary>
    ///     
    /// </summary>
    public abstract class AbstractTextValueValidator : IFormFieldValidator
    {
        protected internal virtual bool NullValid
        {
            get { return true; }
        }

        public virtual bool Validate(object submittedValue, IFormFieldValidatorContext validatorContext)
        {
            if (submittedValue == null)
                return NullValid;

            var configuration = validatorContext.Configuration;

            if (submittedValue is string)
                return Validate((string) submittedValue, configuration);

            throw new ProcessEngineException("String validator " + GetType().Name +
                                             " cannot be used on non-string value of type " + submittedValue.GetType());
        }

        protected internal abstract bool Validate(string submittedValue, string configuration);
    }
}