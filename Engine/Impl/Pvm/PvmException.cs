using System;

namespace ESS.FW.Bpm.Engine.Impl.Pvm
{
    /// <summary>
    ///      
    /// </summary>
    public class PvmException : ProcessEngineException
    {
        

        public PvmException(string message, System.Exception cause) : base(message, cause)
        {
        }

        public PvmException(string message) : base(message)
        {
        }
    }
}