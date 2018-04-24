

using ESS.FW.Bpm.Engine.Exception;

namespace ESS.FW.Bpm.Engine
{
    /// <summary>
    ///     Runtime exception that is the superclass of all exceptions in the process engine.
    ///      
    /// </summary>
    public class ProcessEngineException : RuntimeException //System.Exception
    {
        private const long SerialVersionUid = 1L;

        public ProcessEngineException():base()
        {
        }

        public ProcessEngineException(string message, System.Exception cause) : base(message, cause)
        {
        }

        public ProcessEngineException(string message) : base(message)
        {
        }
        //TODO 隐藏了堆栈及InnerExc
        public ProcessEngineException(System.Exception cause) : base(cause.Message)
        {
        }
    }
}