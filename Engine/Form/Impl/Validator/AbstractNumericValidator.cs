using System;

namespace ESS.FW.Bpm.Engine.Form.Impl.Validator
{
    /// <summary>
    ///     
    /// </summary>
    public abstract class AbstractNumericValidator : IFormFieldValidator
    {
        protected internal virtual bool NullValid
        {
            get { return true; }
        }

        public virtual bool Validate(object submittedValue, IFormFieldValidatorContext validatorContext)
        {
            if (submittedValue == null)
                return NullValid;

            var configurationString = validatorContext.Configuration;

            // Double

            if (submittedValue is double?)
            {
                double? configuration = null;
                try
                {
                    configuration = double.Parse(configurationString);
                }
                catch (FormatException)
                {
                    throw new FormFieldConfigurationException(configurationString,
                        "Cannot validate Double value " + submittedValue + ": configuration " + configurationString +
                        " cannot be parsed as Double.");
                }
                return Validate((double?) submittedValue, configuration);
            }

            // Float

            if (submittedValue is float?)
            {
                float? configuration = null;
                try
                {
                    configuration = float.Parse(configurationString);
                }
                catch (FormatException)
                {
                    throw new FormFieldConfigurationException(configurationString,
                        "Cannot validate Float value " + submittedValue + ": configuration " + configurationString +
                        " cannot be parsed as Float.");
                }
                return Validate((float?) submittedValue, configuration);
            }

            // Long

            if (submittedValue is long?)
            {
                long? configuration = null;
                try
                {
                    configuration = long.Parse(configurationString);
                }
                catch (FormatException)
                {
                    throw new FormFieldConfigurationException(configurationString,
                        "Cannot validate Long value " + submittedValue + ": configuration " + configurationString +
                        " cannot be parsed as Long.");
                }
                return Validate((long?) submittedValue, configuration);
            }

            // Integer

            if (submittedValue is int?)
            {
                int? configuration = null;
                try
                {
                    configuration = int.Parse(configurationString);
                }
                catch (FormatException)
                {
                    throw new FormFieldConfigurationException(configurationString,
                        "Cannot validate Integer value " + submittedValue + ": configuration " + configurationString +
                        " cannot be parsed as Integer.");
                }
                return Validate((int?) submittedValue, configuration);
            }

            // Short

            if (submittedValue is short?)
            {
                short? configuration = null;
                try
                {
                    configuration = short.Parse(configurationString);
                }
                catch (FormatException)
                {
                    throw new FormFieldConfigurationException(configurationString,
                        "Cannot validate Short value " + submittedValue + ": configuration " + configurationString +
                        " cannot be parsed as Short.");
                }
                return Validate((short?) submittedValue, configuration);
            }

            throw new FormFieldValidationException("Numeric validator " + GetType().Name +
                                                   " cannot be used on non-numeric value " + submittedValue);
        }

        protected internal abstract bool Validate(int? submittedValue, int? configuration);

        protected internal abstract bool Validate(long? submittedValue, long? configuration);

        protected internal abstract bool Validate(double? submittedValue, double? configuration);

        protected internal abstract bool Validate(float? submittedValue, float? configuration);

        protected internal abstract bool Validate(short? submittedValue, short? configuration);
    }
}