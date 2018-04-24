using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance.parser
{
    /// <summary>
    ///     
    /// </summary>
    public class ActivityInstanceHandler : IMigratingInstanceParseHandler<IActivityInstance>
    {
        public virtual void Handle(MigratingInstanceParseContext parseContext, IActivityInstance element)
        {
            MigratingActivityInstance migratingInstance = null;

            var applyingInstruction = parseContext.GetInstructionFor(element.ActivityId);
            ScopeImpl sourceScope = null;
            ScopeImpl targetScope = null;
            var representativeExecution = parseContext.Mapping.GetExecution(element);

            if (element.Id.Equals(element.ProcessInstanceId))
            {
                sourceScope = parseContext.SourceProcessDefinition;
                targetScope = parseContext.TargetProcessDefinition;
            }
            else
            {
                sourceScope = (ScopeImpl) parseContext.SourceProcessDefinition.FindActivity(element.ActivityId);

                if (applyingInstruction != null)
                {
                    var activityId = applyingInstruction.TargetActivityId;
                    targetScope = (ScopeImpl) parseContext.TargetProcessDefinition.FindActivity(activityId);
                }
            }

            migratingInstance = parseContext.MigratingProcessInstance.AddActivityInstance(applyingInstruction, element,
                sourceScope, targetScope, representativeExecution);

            var parentInstance = parseContext.GetMigratingActivityInstanceById(element.ParentActivityInstanceId);

            if (parentInstance != null)
            {
                //migratingInstance.setParent(parentInstance);
            }

            //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
            //ORIGINAL LINE: org.camunda.bpm.engine.impl.core.delegate.CoreActivityBehavior<object> sourceActivityBehavior = sourceScope.getActivityBehavior();
            var sourceActivityBehavior = sourceScope.ActivityBehavior;
            if (sourceActivityBehavior is IMigrationObserverBehavior)
                ((IMigrationObserverBehavior) sourceActivityBehavior).OnParseMigratingInstance(parseContext,
                    migratingInstance);

            parseContext.Submit(migratingInstance);

            ParseTransitionInstances(parseContext, migratingInstance);

            ParseDependentInstances(parseContext, migratingInstance);
        }

        public virtual void ParseTransitionInstances(MigratingInstanceParseContext parseContext,
            MigratingActivityInstance migratingInstance)
        {
            foreach (var transitionInstance in migratingInstance.ActivityInstance.ChildTransitionInstances)
                parseContext.HandleTransitionInstance(transitionInstance);
        }

        public virtual void ParseDependentInstances(MigratingInstanceParseContext parseContext,
            MigratingActivityInstance migratingInstance)
        {
            parseContext.HandleDependentVariables(migratingInstance, CollectActivityInstanceVariables(migratingInstance));
            parseContext.HandleDependentActivityInstanceJobs(migratingInstance,
                CollectActivityInstanceJobs(migratingInstance));
            parseContext.HandleDependentEventSubscriptions(migratingInstance,
                CollectActivityInstanceEventSubscriptions(migratingInstance));
        }

        protected internal virtual IList<VariableInstanceEntity> CollectActivityInstanceVariables(
            MigratingActivityInstance instance)
        {
            throw new NotImplementedException();
            IList<VariableInstanceEntity> variables = new List<VariableInstanceEntity>();
            var representativeExecution = instance.ResolveRepresentativeExecution();
            //ExecutionEntity parentExecution = representativeExecution.Parent;

            // decide for representative execution and parent execution whether to none/all/concurrentLocal variables
            // belong to this activity instance
            //var addAllRepresentativeExecutionVariables = instance.SourceScope.Scope ||
            //                                             representativeExecution.Concurrent;

            //if (addAllRepresentativeExecutionVariables)
            //{
            //    ((List<VariableInstanceEntity>) variables).AddRange(representativeExecution.VariablesInternal);
            //}
            //else
            //{
            //    ((List<VariableInstanceEntity>) variables).AddRange(getConcurrentLocalVariables(representativeExecution));
            //}

            //var addAnyParentExecutionVariables = parentExecution != null && instance.SourceScope.Scope;
            //if (addAnyParentExecutionVariables)
            //{
            //    bool addAllParentExecutionVariables = parentExecution.Concurrent;

            //    if (addAllParentExecutionVariables)
            //    {
            //        ((List<VariableInstanceEntity>) variables).AddRange(parentExecution.VariablesInternal);
            //    }
            //    else
            //    {
            //        ((List<VariableInstanceEntity>) variables).AddRange(getConcurrentLocalVariables(parentExecution));
            //    }
            //}

            return variables;
        }

        protected internal virtual IList<EventSubscriptionEntity> CollectActivityInstanceEventSubscriptions(
            MigratingActivityInstance migratingInstance)
        {
            if (migratingInstance.SourceScope.IsScope)
            {
                //return migratingInstance.resolveRepresentativeExecution().EventSubscriptions;
            }
            return null;
        }

        protected internal virtual IList<JobEntity> CollectActivityInstanceJobs(
            MigratingActivityInstance migratingInstance)
        {
            if (migratingInstance.SourceScope.IsScope)
            {
                //return migratingInstance.resolveRepresentativeExecution().Jobs;
            }
            return null;
        }

        public static IList<VariableInstanceEntity> GetConcurrentLocalVariables(ExecutionEntity execution)
        {
            IList<VariableInstanceEntity> variables = new List<VariableInstanceEntity>();

            //foreach (VariableInstanceEntity variable in execution.VariablesInternal)
            //{
            //    if (variable.ConcurrentLocal)
            //    {
            //        variables.Add(variable);
            //    }
            //}

            return variables;
        }
    }
}