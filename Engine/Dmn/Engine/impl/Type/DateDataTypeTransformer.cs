

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.type
{
    /// <summary>
    ///     Transform values of type <seealso cref="Date" /> and <seealso cref="string" /> into
    ///     <seealso cref="DateValue" /> which contains date and time. A String should have the format
    ///     {@code yyyy-MM-dd'T'HH:mm:ss}.
    ///     
    /// </summary>
//    public class DateDataTypeTransformer : IDmnDataTypeTransformer
//    {
//        protected internal format = "yyyy-MM-dd'T'HH:mm:ss";

////JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
////ORIGINAL LINE: @Override public TypedValue transform(Object value) throws IllegalArgumentException
//        public virtual ITypedValue transform(object value)
//        {
//            if (value is DateTime)
//            {
//                return Variables.dateValue((DateTime) value);
//            }
//            if (value is string)
//            {
//                var date = transformString((string) value);
//                return Variables.dateValue(date);
//            }
//            throw new ArgumentException();
//        }

//        protected internal virtual DateTime transformString(string value)
//        {
//            try
//            {
//                return format.parse(value);
//            }
//            catch (ParseException e)
//            {
//                throw new ArgumentException(e);
//            }
//        }
//    }
}