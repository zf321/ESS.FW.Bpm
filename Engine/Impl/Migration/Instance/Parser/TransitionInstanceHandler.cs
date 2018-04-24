using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance.parser
{
    /// <summary>
    ///     
    /// </summary>
    public class TransitionInstanceHandler : IMigratingInstanceParseHandler<ITransitionInstance>
    {
        public virtual void Handle(MigratingInstanceParseContext parseContext, ITransitionInstance transitionInstance)
        {
            throw new NotImplementedException();
            //if (!IsAsyncTransitionInstance(transitionInstance))
            //    return;

            //var applyingInstruction = parseContext.GetInstructionFor(transitionInstance.ActivityId);

            //ScopeImpl sourceScope = parseContext.SourceProcessDefinition.FindActivity(transitionInstance.ActivityId);
            //ScopeImpl targetScope = null;

            //if (applyingInstruction != null)
            //{
            //    var activityId = applyingInstruction.TargetActivityId;
            //    targetScope = parseContext.TargetProcessDefinition.FindActivity(activityId);
            //}

            //ExecutionEntity asyncExecution =
            //    Context.CommandContext.ExecutionManager.FindExecutionById(transitionInstance.ExecutionId);

            //var migratingTransitionInstance =
            //    parseContext.MigratingProcessInstance.AddTransitionInstance(applyingInstruction, transitionInstance,
            //        sourceScope, targetScope, asyncExecution);

            //var parentInstance =
            //    parseContext.GetMigratingActivityInstanceById(transitionInstance.ParentActivityInstanceId);
            //migratingTransitionInstance.setParent(parentInstance);

            //IList<JobEntity> jobs = asyncExecution.Jobs;
            //parseContext.handleDependentTransitionInstanceJobs(migratingTransitionInstance, jobs);

            //parseContext.handleDependentVariables(migratingTransitionInstance,
            //    collectTransitionInstanceVariables(migratingTransitionInstance));
        }

        /// <summary>
        ///     Workaround for CAM-5609: In general, only async continuations should be represented as TransitionInstances, but
        ///     due to this bug, completed multi-instances are represented like that as well. We tolerate the second case.
        /// </summary>
        protected internal virtual bool IsAsyncTransitionInstance(ITransitionInstance transitionInstance)
        {
            throw new NotImplementedException();
            var executionId = transitionInstance.ExecutionId;
            //ExecutionEntity execution = Context.CommandContext.ExecutionManager.findExecutionById(executionId);
            //foreach (JobEntity job in execution.Jobs)
            //{
            //    if (AsyncContinuationJobHandler.TYPE.Equals(job.JobHandlerType))
            //    {
            //        return true;
            //    }
            //}

            return false;
        }

        protected internal virtual IList<VariableInstanceEntity> CollectTransitionInstanceVariables(
            MigratingTransitionInstance instance)
        {
            IList<VariableInstanceEntity> variables = new List<VariableInstanceEntity>();
            var representativeExecution = instance.ResolveRepresentativeExecution();

            if (representativeExecution.IsConcurrent)
            {
                throw new NotImplementedException();
                //((List<VariableInstanceEntity>)variables).AddRange(representativeExecution.VariablesInternal);
            }
            else
            {
                throw new NotImplementedException();
                //((List<VariableInstanceEntity>)variables).AddRange(ActivityInstanceHandler.GetConcurrentLocalVariables(representativeExecution));
            }

            return variables;
        }
    }
}