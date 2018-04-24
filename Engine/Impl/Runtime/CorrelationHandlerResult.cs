using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.runtime
{
    /// <summary>
    ///     <para>
    ///         The result of a message correlation. A message may be correlated to either
    ///         a waiting execution (BPMN receive message event) or a process definition
    ///         (BPMN message start event). The type of the correlation (execution vs.
    ///         processDefinition) can be obtained using <seealso cref="#getResultType()" />
    ///     </para>
    ///     <para>Correlation is performed by a <seealso cref="ICorrelationHandler" />.</para>
    ///     
    /// </summary>
    public class CorrelationHandlerResult
    {
        protected internal ExecutionEntity executionEntity;
        protected internal ProcessDefinitionEntity processDefinitionEntity;

        /// <seealso cref= MessageCorrelationResultType# Execution
        /// </seealso>
        /// <seealso cref= MessageCorrelationResultType# ProcessDefinition
        /// </seealso>
        protected internal MessageCorrelationResultType resultType;

        protected internal string startEventActivityId;

        // getters ////////////////////////////////////////////

        public virtual ExecutionEntity ExecutionEntity
        {
            get { return executionEntity; }
        }

        public virtual ProcessDefinitionEntity ProcessDefinitionEntity
        {
            get { return processDefinitionEntity; }
        }

        public virtual string StartEventActivityId
        {
            get { return startEventActivityId; }
        }

        public virtual MessageCorrelationResultType ResultType
        {
            get { return resultType; }
        }

        public virtual IExecution Execution
        {
            get { return (IExecution) executionEntity; }
        }

        public virtual IProcessDefinition ProcessDefinition
        {
            get { return (IProcessDefinition) processDefinitionEntity; }
        }

        public static CorrelationHandlerResult MatchedExecution(ExecutionEntity executionEntity)
        {
            var messageCorrelationResult = new CorrelationHandlerResult();
            messageCorrelationResult.resultType = MessageCorrelationResultType.Execution;
            messageCorrelationResult.executionEntity = executionEntity;
            return messageCorrelationResult;
        }

        public static CorrelationHandlerResult MatchedProcessDefinition(ProcessDefinitionEntity processDefinitionEntity,
            string startEventActivityId)
        {
            var messageCorrelationResult = new CorrelationHandlerResult();
            messageCorrelationResult.processDefinitionEntity = processDefinitionEntity;
            messageCorrelationResult.startEventActivityId = startEventActivityId;
            messageCorrelationResult.resultType = MessageCorrelationResultType.ProcessDefinition;
            return messageCorrelationResult;
        }
    }
}