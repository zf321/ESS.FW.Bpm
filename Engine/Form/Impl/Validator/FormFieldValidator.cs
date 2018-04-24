namespace ESS.FW.Bpm.Engine.Form.Impl.Validator
{
    /// <summary>
    ///     <para>Interface for implenting form field validators.</para>
    ///     
    /// </summary>
    public interface IFormFieldValidator
    {
        /// <summary>
        ///     return true if the submitted value is valid for the given form field.
        /// </summary>
        /// <param name="submittedValue">
        ///     the value submitted to the form field
        /// </param>
        /// <param name="validatorContext">
        ///     object providing access to additional information useful wile
        ///     validating the form
        /// </param>
        /// <returns> true if the value is valid, false otherwise. </returns>
        bool Validate(object submittedValue, IFormFieldValidatorContext validatorContext);
    }
}