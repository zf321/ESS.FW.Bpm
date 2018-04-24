using System;

namespace ESS.FW.Bpm.Engine
{
    /// <summary>
    ///     <para>Base exception resulting from a script engine interaction.</para>
    ///     
    /// </summary>
    public class ScriptEngineException : ProcessEngineException
    {
        private const long SerialVersionUid = 1L;

        public ScriptEngineException()
        {
        }

        public ScriptEngineException(string message, System.Exception cause) : base(message, cause)
        {
        }

        public ScriptEngineException(string message) : base(message)
        {
        }

        public ScriptEngineException(System.Exception cause) : base(cause)
        {
        }
    }
}