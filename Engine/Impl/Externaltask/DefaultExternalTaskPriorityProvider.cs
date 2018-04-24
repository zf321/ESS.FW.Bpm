using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Externaltask
{

    /// <summary>
    ///     Represents the default priority provider for external tasks.
    ///      
    /// </summary>
    public class DefaultExternalTaskPriorityProvider : DefaultPriorityProvider<ExternalTaskActivityBehavior>
    {
        public static readonly ExternalTaskLogger Log = ProcessEngineLogger.ExternalTaskLogger;

        protected internal override void LogNotDeterminingPriority(ExecutionEntity execution, object value,
            ProcessEngineException e)
        {
            Log.CouldNotDeterminePriority(execution, value, e);
        }

        protected internal override long? GetSpecificPriority(ExecutionEntity execution,
            ExternalTaskActivityBehavior param, string jobDefinitionId)
        {
            var priorityProvider = param.PriorityValueProvider;
            if (priorityProvider != null)
                return EvaluateValueProvider(priorityProvider, execution, "");
            return null;
        }

        protected internal override long? GetProcessDefinitionPriority(ExecutionEntity execution,
            ExternalTaskActivityBehavior param)
        {
            return GetProcessDefinedPriority(execution.GetProcessDefinition(), BpmnParse.PropertynameTaskPriority, execution, "");
        }
    }
}