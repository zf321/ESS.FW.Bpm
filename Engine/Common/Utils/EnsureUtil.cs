//using System;

//namespace ESS.FW.Bpm.Engine.common.Utils
//{
//    /// <summary>
//    ///     .
//    /// </summary>
//    public class EnsureUtil
//    {
//        private static readonly EnsureUtilLogger LOG = UtilsLogger.ENSURE_UTIL_LOGGER;

//        /// <summary>
//        ///     Ensures that the parameter is not null.
//        /// </summary>
//        /// <param name="parameterName"> the parameter name </param>
//        /// <param name="value"> the value to ensure to be not null </param>
//        /// <exception cref="IllegalArgumentException"> if the parameter value is null </exception>
//        public static void ensureNotNull(string parameterName, object value)
//        {
//            if (value == null)
//                throw LOG.parameterIsNullException(parameterName);
//        }

//        /// <summary>
//        ///     Ensure the object is of a given type and return the casted object
//        /// </summary>
//        /// <param name="objectName"> the name of the parameter </param>
//        /// <param name="object"> the parameter value </param>
//        /// <param name="type"> the expected type </param>
//        /// <returns> the parameter casted to the requested type </returns>
//        /// <exception cref="IllegalArgumentException"> in case object cannot be casted to type </exception>
////JAVA TO C# CONVERTER TODO ITask: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T> T ensureParamInstanceOf(String objectName, Object object, Class<T> type)
//        public static T ensureParamInstanceOf<T>(string objectName, object @object, Type type)
//        {
//            if (type.IsAssignableFrom(@object.GetType()))
//                return (T) @object;
//            throw LOG.unsupportedParameterType(objectName, @object, type);
//        }
//    }
//}