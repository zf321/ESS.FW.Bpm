using System;

namespace ESS.FW.Bpm.Engine.Common.Utils
{
    /// <summary>
    ///     .
    /// </summary>
    public class EnsureUtilLogger : UtilsLogger
    {
        public virtual ArgumentException ParameterIsNullException(string parameterName)
        {
            return new ArgumentException(ExceptionMessage("001", "Parameter '{}' is null", parameterName));
        }

        public virtual ArgumentException UnsupportedParameterType(string parameterName, object param, Type expectedType)
        {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
            return
                new ArgumentException(ExceptionMessage("002",
                    "Unsupported parameter '{}' of type '{}'. Expected type '{}'.", parameterName, param.GetType(),
                    expectedType.FullName));
        }
    }
}