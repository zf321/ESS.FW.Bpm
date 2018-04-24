using System;

namespace ESS.FW.Bpm.Engine.Dmn.engine
{
    /// <summary>
    ///     The base exception of the DMN Engine.
    /// </summary>
    public class DmnEngineException : System.Exception
    {
        public DmnEngineException(string message) : base(message)
        {
        }

        public DmnEngineException(string message, System.Exception cause) : base(message, cause)
        {
        }
    }
}