using System;

namespace ESS.FW.Bpm.Engine
{
    /// <summary>
    ///     <para>Exception resulting from a error during a script compilation.</para>
    ///     
    /// </summary>
    public class ScriptCompilationException : ScriptEngineException
    {
        private const long SerialVersionUid = 1L;

        public ScriptCompilationException()
        {
        }

        public ScriptCompilationException(string message, System.Exception cause) : base(message, cause)
        {
        }

        public ScriptCompilationException(string message) : base(message)
        {
        }

        public ScriptCompilationException(System.Exception cause) : base(cause)
        {
        }
    }
}