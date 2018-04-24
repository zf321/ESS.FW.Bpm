using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using ESS.FW.Bpm.Engine.Impl.Juel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel.El
{
    public class FeelTypeConverter: TypeConverterImpl
    {
        public static readonly FeelEngineLogger LOG= FeelLogger.ENGINE_LOGGER;

        public FeelTypeConverter()
        {
        }

        protected internal override bool? CoerceToBoolean(object value)
        {
            if (value is bool?)
            {
                return (bool?)value;
            }
            else
            {
                throw LOG.unableToConvertValue(value, typeof(bool?));
            }
        }

        protected internal override decimal CoerceToBigDecimal(object value)
        {
            if (value is decimal @decimal)
            {
                return @decimal;
            }
            else if (value is Int64)
            {
                return System.Convert.ToDecimal(value);
            }
            else if(value is double)
            {
                return new decimal((double)value);
            }
            else if (value is int)
            {
                return new decimal((double)((int)value));
            }
            else
            {
                throw LOG.unableToConvertValue(value, typeof(decimal));
            }
        }

        protected internal override long CoerceToBigInteger(object value)
        {
            if (value is Int64)
            {
                return (Int64)value;
            }
            else if (value is decimal)
            {
                return System.Convert.ToInt64(value);
            }
            else if (value is int)
            {
                return System.Convert.ToInt64(value);// System.Numerics.BigInteger.valueOf((long)((Number)value));
            }
            else
            {
                throw LOG.unableToConvertValue(value, typeof(Int64));
            }
        }

        protected internal override double? CoerceToDouble(object value)
        {
            if (value is double?)
            {
                return (double?)value;
            }
            else if (value is decimal)
            {
                return System.Convert.ToInt64(value);
            }
            else if (value is int)
            {
                return System.Convert.ToDouble(value);
            }
            else if (value is string)
            {
                return System.Convert.ToDouble(value);
            }
            else
            {
                throw LOG.unableToConvertValue(value, typeof(double?));
            }
        }

        protected internal override long? CoerceToLong(object value)
        {
            if (value is long?)
            {
                return (long?)value;
            }
            else if (IsLong(value))
            {
                return System.Convert.ToInt64(value);
            }
            else
            {
                throw LOG.unableToConvertValue(value, typeof(long?));
            }
        }

        protected internal override string CoerceToString(object value)
        {
            if (value is string)
            {
                return (string)value;
            }
            else if (value is Enum)
            {
                return ((Enum)value).ToString();//.name();
            }
            else
            {
                throw LOG.unableToConvertValue(value, typeof(string));
            }
        }
        
        public override T Convert<T>(object value, Type type)
        {
            try
            {
                return base.Convert<T>(value, type);
            }
            catch (ELException var4)
            {
                throw LOG.unableToConvertValue(value, type, var4);
            }
        }

        protected internal virtual bool IsLong(object value)
        {            
            //double doubleValue = (double)value;
            //return doubleValue == (double)((long)doubleValue);
            double doubleValue = System.Convert.ToDouble(value);
            return doubleValue == System.Convert.ToDouble(System.Convert.ToInt64(doubleValue));
        }


    }
}
