using System;

namespace ESS.FW.Bpm.Engine.exception
{
    /// <summary>
    ///     
    /// </summary>
    public class NullValueException : ProcessEngineException
    {
        

        public NullValueException()
        {
        }

        public NullValueException(string message, System.Exception cause) : base(message, cause)
        {
        }

        public NullValueException(string message) : base(message)
        {
        }

        public NullValueException(System.Exception cause) : base( cause)
        {
        }
    }
}