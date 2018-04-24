namespace ESS.FW.Bpm.Engine.Form.Impl.Validator
{
    /// <summary>
    ///     
    /// </summary>
    public class MaxValidator : AbstractNumericValidator
    {
        protected internal override bool Validate(int? submittedValue, int? configuration)
        {
            return submittedValue < configuration;
        }

        protected internal override bool Validate(long? submittedValue, long? configuration)
        {
            return submittedValue < configuration;
        }

        protected internal override bool Validate(double? submittedValue, double? configuration)
        {
            return submittedValue < configuration;
        }

        protected internal override bool Validate(float? submittedValue, float? configuration)
        {
            return submittedValue < configuration;
        }

        protected internal override bool Validate(short? submittedValue, short? configuration)
        {
            return submittedValue < configuration;
        }
    }
}