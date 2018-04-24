using System.Text;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Impl.migration.instance;
using ESS.FW.Bpm.Engine.Impl.migration.validation.instance;
using ESS.FW.Bpm.Engine.Impl.migration.validation.instruction;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.migration
{
    /// <summary>
    ///     
    /// </summary>
    public class MigrationLogger : ProcessEngineLogger
    {
        public virtual MigrationPlanValidationException FailingMigrationPlanValidation(
            MigrationPlanValidationReportImpl validationReport)
        {
            var sb = new StringBuilder();
            validationReport.WriteTo(sb);
            return new MigrationPlanValidationException(ExceptionMessage("001", "{}", sb.ToString()), validationReport);
        }

        public virtual ProcessEngineException ProcessDefinitionOfInstanceDoesNotMatchMigrationPlan(
            ExecutionEntity processInstance, string processDefinitionId)
        {
            return
                new ProcessEngineException(ExceptionMessage("002",
                    "Process instance '{}' cannot be migrated. Its process definition '{}' does not match the source process definition of the migration plan '{}'",
                    processInstance.Id, processInstance.ProcessDefinitionId, processDefinitionId));
        }

        public virtual ProcessEngineException ProcessInstanceDoesNotExist(string processInstanceId)
        {
            return
                new ProcessEngineException(ExceptionMessage("003",
                    "Process instance '{}' cannot be migrated. The process instance does not exist", processInstanceId));
        }

        public virtual MigratingProcessInstanceValidationException FailingMigratingProcessInstanceValidation(
            MigratingProcessInstanceValidationReportImpl validationReport)
        {
            var sb = new StringBuilder();
            validationReport.WriteTo(sb);
            return new MigratingProcessInstanceValidationException(ExceptionMessage("004", "{}", sb.ToString()),
                validationReport);
        }

        public virtual ProcessEngineException CannotBecomeSubordinateInNonScope(
            MigratingActivityInstance activityInstance)
        {
            return
                new ProcessEngineException(ExceptionMessage("005", "{}",
                    "Cannot attach a subordinate to activity instance '{}'. Activity '{}' is not a scope",
                    activityInstance.ActivityInstance.Id, activityInstance.ActivityInstance.ActivityId));
        }

        public virtual ProcessEngineException CannotDestroySubordinateInNonScope(
            MigratingActivityInstance activityInstance)
        {
            return
                new ProcessEngineException(ExceptionMessage("006", "{}",
                    "Cannot destroy a subordinate of activity instance '{}'. Activity '{}' is not a scope",
                    activityInstance.ActivityInstance.Id, activityInstance.ActivityInstance.ActivityId));
        }

        public virtual ProcessEngineException CannotAttachToTransitionInstance(IMigratingInstance attachingInstance)
        {
            return
                new ProcessEngineException(ExceptionMessage("007", "{}",
                    "Cannot attach instance '{}' to a transition instance", attachingInstance));
        }

        public virtual BadUserRequestException ProcessDefinitionDoesNotExist(string processDefinitionId, string type)
        {
            return
                new BadUserRequestException(ExceptionMessage("008", "{} process definition with id '{}' does not exist",
                    type, processDefinitionId));
        }

        public virtual ProcessEngineException CannotMigrateBetweenTenants(string sourceTenantId, string targetTenantId)
        {
            return
                new ProcessEngineException(ExceptionMessage("09",
                    "Cannot migrate process instances between processes of different tenants ('{}' != '{}')",
                    sourceTenantId, targetTenantId));
        }

        public virtual ProcessEngineException CannotMigrateInstanceBetweenTenants(string processInstanceId,
            string sourceTenantId, string targetTenantId)
        {
            string detailMessage = null;
            if (!ReferenceEquals(sourceTenantId, null))
                detailMessage = ExceptionMessage("010",
                    "Cannot migrate process instance '{}' to a process definition of a different tenant ('{}' != '{}')",
                    processInstanceId, sourceTenantId, targetTenantId);
            else
                detailMessage = ExceptionMessage("010",
                    "Cannot migrate process instance '{}' without tenant to a process definition with a tenant ('{}')",
                    processInstanceId, targetTenantId);

            return new ProcessEngineException(detailMessage);
        }

        public virtual ProcessEngineException CannotHandleChild(MigratingScopeInstance scopeInstance,
            MigratingProcessElementInstance childCandidate)
        {
            return
                new ProcessEngineException(ExceptionMessage("011",
                    "Scope instance of type {} cannot have child of type {}", scopeInstance.GetType().Name,
                    childCandidate.GetType().Name));
        }
    }
}