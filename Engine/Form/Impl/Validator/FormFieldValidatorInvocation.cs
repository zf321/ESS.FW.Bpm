using ESS.FW.Bpm.Engine.Impl.Delegate;

namespace ESS.FW.Bpm.Engine.Form.Impl.Validator
{
    /// <summary>
    ///     
    /// </summary>
    public class FormFieldValidatorInvocation : DelegateInvocation
    {
        protected internal IFormFieldValidator FormFieldValidator;
        protected internal object SubmittedValue;
        protected internal IFormFieldValidatorContext ValidatorContext;

        public FormFieldValidatorInvocation(IFormFieldValidator formFieldValidator, object submittedValue,
            IFormFieldValidatorContext validatorContext) : base(null, null)
        {
            this.FormFieldValidator = formFieldValidator;
            this.SubmittedValue = submittedValue;
            this.ValidatorContext = validatorContext;
        }

        public override object InvocationResult
        {
            get { return (bool?) base.InvocationResult; }
        }

//JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void invoke() throws Exception
        protected internal override void Invoke()
        {
            InvocationResult = FormFieldValidator.Validate(SubmittedValue, ValidatorContext);
        }
    }
}