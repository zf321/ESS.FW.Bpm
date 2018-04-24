using System;

namespace ESS.FW.Bpm.Engine
{
    /// <summary>
    ///     <para>Exception resulting from a error during a script evaluation.</para>
    ///     
    /// </summary>
    public class ScriptEvaluationException : ScriptEngineException
    {
        private const long SerialVersionUid = 1L;

        public ScriptEvaluationException()
        {
        }

        public ScriptEvaluationException(string message, System.Exception cause) : base(message, cause)
        {
        }

        public ScriptEvaluationException(string message) : base(message)
        {
        }

        public ScriptEvaluationException(System.Exception cause) : base(cause)
        {
        }
    }
}