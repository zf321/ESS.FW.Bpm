using System;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Form.Impl.Type
{
    /// <summary>
    ///      
    /// </summary>
    public class DateFormType : AbstractFormFieldType
    {
        public const string TypeName = "date";

        protected internal string DatePattern;
        //protected internal DateFormat dateFormat;

        public DateFormType(string datePattern)
        {
            this.DatePattern = datePattern;
            //this.dateFormat = new SimpleDateFormat(datePattern);
        }

        public override string Name
        {
            get { return TypeName; }
        }

        public override object GetInformation(string key)
        {
            if ("datePattern".Equals(key))
                return DatePattern;
            return null;
        }

        public override ITypedValue ConvertToModelValue(ITypedValue propertyValue)
        {
            var value = propertyValue;
            if (value == null)
            {
                //return Variables.dateValue(null);
            }
            if (value is DateTime)
            {
                //return Variables.dateValue((DateTime) value);
            }
            if (value is string)
            {
                // try
                // {
                //return Variables.dateValue((DateTime) dateFormat.parseObject((string) value));
                // }
                // catch (ParseException)
                // {
                //throw new ProcessEngineException("Could not parse value '" + value + "' as date using date format '" + datePattern + "'.");
                // }
            }
            else
            {
                throw new ProcessEngineException("Value '" + value + "' cannot be transformed into a Date.");
            }
            return null;
        }

        public override ITypedValue ConvertToFormValue(ITypedValue modelValue)
        {
            if (modelValue == null)
            {
                //return Variables.(null);
            }
            //else if (modelValue.Type == IITypeValue.DATE)
            //{
            //  return Variables.stringValue(dateFormat.format(modelValue.Value));
            //}
            return null;
        }

        // deprecated //////////////////////////////////////////////////////////

        public override object ConvertFormValueToModelValue(object propertyValue)
        {
            if ((propertyValue == null) || "".Equals(propertyValue))
                return null;
            //try
            //{
            //  return dateFormat.parseObject(propertyValue.ToString());
            //}
            //catch (ParseException)
            //{
            //  throw new ProcessEngineException("invalid date value " + propertyValue);
            //}
            return null;
        }

        public override string ConvertModelValueToFormValue(object modelValue)
        {
            if (modelValue == null)
                return null;
            return null;
            //return dateFormat.format(modelValue);
        }
    }
}