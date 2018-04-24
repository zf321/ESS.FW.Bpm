using System;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping.value;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{

    /// <summary>
    ///     
    /// </summary>
    public class DefaultJobPriorityProvider : DefaultPriorityProvider<IJobDeclaration>
    {
        private static readonly JobExecutorLogger Log = ProcessEngineLogger.JobExecutorLogger;

        protected internal override long? GetSpecificPriority(ExecutionEntity execution, IJobDeclaration param,
            string jobDefinitionId)
        {
            long? specificPriority = null;
            var jobDefinition = GetJobDefinitionFor(jobDefinitionId);
            if (jobDefinition != null)
            {
                specificPriority = jobDefinition.OverridingJobPriority;
            }

            if (specificPriority == null)
            {
                var priorityProvider = param.JobPriorityProvider;
                if (priorityProvider != null)
                {
                    specificPriority = EvaluateValueProvider(priorityProvider, execution,
                        DescribeContext(param, execution));
                }
            }
            return specificPriority;
        }

        protected internal override long? GetProcessDefinitionPriority(ExecutionEntity execution,
            IJobDeclaration jobDeclaration)
        {
            var processDefinition = jobDeclaration.ProcessDefinition;
            return GetProcessDefinedPriority(processDefinition, BpmnParse.PropertynameJobPriority, execution,
                DescribeContext(jobDeclaration, execution));
        }

        protected internal virtual JobDefinitionEntity GetJobDefinitionFor(string jobDefinitionId)
        {
            if (!ReferenceEquals(jobDefinitionId, null))
            {
                return Context.CommandContext.JobDefinitionManager.FindById(jobDefinitionId);
            }
            else
            {
                return null;
            }
        }

        protected internal virtual long? GetActivityPriority(ExecutionEntity execution,
            IJobDeclaration jobDeclaration)
        {
            if (jobDeclaration != null)
            {
                var priorityProvider = jobDeclaration.JobPriorityProvider;
                if (priorityProvider != null)
                {
                    return EvaluateValueProvider(priorityProvider, execution, DescribeContext(jobDeclaration, execution));
                }
            }
            return null;
        }

        protected internal override void LogNotDeterminingPriority(ExecutionEntity execution, object value,
            ProcessEngineException e)
        {
            Log.CouldNotDeterminePriority(execution, value, e);
        }

        protected internal virtual string DescribeContext(IJobDeclaration JobDeclaration,
            ExecutionEntity executionEntity)
        {
            return "Job " + JobDeclaration.ActivityId + "/" + JobDeclaration.JobHandlerType + " instantiated " +
                   "in context of " + executionEntity;
        }
    }
}