using System;

namespace ESS.FW.Bpm.Engine.Form.Impl.Validator
{
    /// <summary>
    ///     
    /// </summary>
    public class MaxLengthValidator : AbstractTextValueValidator
    {
        protected internal override bool Validate(string submittedValue, string configuration)
        {
            int? maxLength = null;
            try
            {
                maxLength = int.Parse(configuration);
            }
            catch (FormatException)
            {
                // do not throw validation exception, as the issue is not with the submitted value
                throw new FormFieldConfigurationException(configuration,
                    "Cannot validate \"maxlength\": configuration " + configuration +
                    " cannot be interpreted as Integer");
            }

            return submittedValue.Length < maxLength;
        }
    }
}