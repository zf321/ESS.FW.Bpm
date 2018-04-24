
 

using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.runtime
{
    /// <summary>
    ///      
    /// </summary>
    public class MessageCorrelationResultImpl : IMessageCorrelationResult
    {
        protected internal readonly IExecution execution;
        protected internal readonly MessageCorrelationResultType resultType;


        public MessageCorrelationResultImpl(CorrelationHandlerResult handlerResult)
        {
            execution = handlerResult.Execution;
            resultType = handlerResult.ResultType;
        }

        public virtual IExecution Execution
        {
            get { return execution; }
        }

        public virtual IProcessInstance ProcessInstance { get; set; }


        public virtual MessageCorrelationResultType ResultType
        {
            get { return resultType; }
        }
    }
}

