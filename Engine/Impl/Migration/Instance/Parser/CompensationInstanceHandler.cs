using ESS.FW.Bpm.Engine.Impl.Bpmn.Helper;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance.parser
{
    /// <summary>
    ///     
    /// </summary>
    public class CompensationInstanceHandler : IMigratingInstanceParseHandler<EventSubscriptionEntity>
    {
        public virtual void Handle(MigratingInstanceParseContext parseContext, EventSubscriptionEntity element)
        {
            MigratingProcessElementInstance migratingInstance;
            if (!ReferenceEquals(element.Configuration, null))
                migratingInstance = CreateMigratingEventScopeInstance(parseContext, element);
            else
                migratingInstance = CreateMigratingEventSubscriptionInstance(parseContext, element);


            ExecutionEntity owningExecution = element.Execution;
            MigratingScopeInstance parentInstance = null;
            if (owningExecution.IsEventScope)
            {
                parentInstance = parseContext.GetMigratingCompensationInstanceByExecutionId(owningExecution.Id);
            }
            else
            {
                parentInstance = parseContext.GetMigratingActivityInstanceById(owningExecution.ParentActivityInstanceId);
            }
            migratingInstance.Parent = parentInstance;
        }

        protected internal virtual MigratingProcessElementInstance CreateMigratingEventSubscriptionInstance(
            MigratingInstanceParseContext parseContext, EventSubscriptionEntity element)
        {
            var compensationHandler =
                (ActivityImpl) parseContext.SourceProcessDefinition.FindActivity(element.ActivityId);

            var migrationInstruction = GetMigrationInstruction(parseContext, compensationHandler);

            ActivityImpl targetScope = null;
            if (migrationInstruction != null)
            {
                var targetEventScope = (ActivityImpl) parseContext.GetTargetActivity(migrationInstruction).EventScope;
                targetScope = targetEventScope.findCompensationHandler();
            }

            var migratingCompensationInstance =
                parseContext.MigratingProcessInstance.AddCompensationSubscriptionInstance(migrationInstruction, element,
                    compensationHandler, targetScope);

            parseContext.Consume(element);

            return migratingCompensationInstance;
        }

        protected internal virtual MigratingProcessElementInstance CreateMigratingEventScopeInstance(
            MigratingInstanceParseContext parseContext, EventSubscriptionEntity element)
        {
            var compensatingActivity =
                (ActivityImpl) parseContext.SourceProcessDefinition.FindActivity(element.ActivityId);

            var migrationInstruction = GetMigrationInstruction(parseContext, compensatingActivity);

            ActivityImpl eventSubscriptionTargetScope = null;

            if (migrationInstruction != null)
                if (compensatingActivity.CompensationHandler)
                {
                    var targetEventScope =
                        (ActivityImpl) parseContext.GetTargetActivity(migrationInstruction).EventScope;
                    eventSubscriptionTargetScope = targetEventScope.findCompensationHandler();
                }
                else
                {
                    eventSubscriptionTargetScope = parseContext.GetTargetActivity(migrationInstruction);
                }

            var eventScopeExecution = CompensationUtil.GetCompensatingExecution(element);
            var eventScopeInstruction = parseContext.FindSingleMigrationInstruction(eventScopeExecution.ActivityId);
            var targetScope = parseContext.GetTargetActivity(eventScopeInstruction);

            //var migratingCompensationInstance =
            //    parseContext.MigratingProcessInstance.addEventScopeInstance(eventScopeInstruction, eventScopeExecution,
            //        eventScopeExecution.getActivity(), targetScope, migrationInstruction, element, compensatingActivity,
            //        eventSubscriptionTargetScope);

            //parseContext.consume(element);
            //parseContext.submit(migratingCompensationInstance);

            //parseDependentEntities(parseContext, migratingCompensationInstance);

            //return migratingCompensationInstance;
            return null;
        }

        protected internal virtual IMigrationInstruction GetMigrationInstruction(
            MigratingInstanceParseContext parseContext, ActivityImpl activity)
        {
            if (activity.CompensationHandler)
            {
                var compensationHandlerProperties = activity.Properties;
                var eventTrigger = compensationHandlerProperties.Get(BpmnProperties.CompensationBoundaryEvent);
                if (eventTrigger == null)
                    eventTrigger = compensationHandlerProperties.Get(BpmnProperties.InitialActivity);

                return parseContext.FindSingleMigrationInstruction(eventTrigger.ActivityId);
            }
            return parseContext.FindSingleMigrationInstruction(activity.ActivityId);
        }

        protected internal virtual void ParseDependentEntities(MigratingInstanceParseContext parseContext,
            MigratingEventScopeInstance migratingInstance)
        {
            var representativeExecution = migratingInstance.ResolveRepresentativeExecution();

            //IList<VariableInstanceEntity> variables =
            //    new List<VariableInstanceEntity>(representativeExecution.VariablesInternal);
            //parseContext.handleDependentVariables(migratingInstance, variables);
        }
    }
}