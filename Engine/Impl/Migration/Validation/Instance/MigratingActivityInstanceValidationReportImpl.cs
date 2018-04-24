using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.migration.instance;
using ESS.FW.Bpm.Engine.Migration;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instance
{
    public class MigratingActivityInstanceValidationReportImpl : IMigratingActivityInstanceValidationReport
    {
        protected internal string activityInstanceId;
        protected internal IList<string> failures = new List<string>();
        protected internal IMigrationInstruction migrationInstruction;
        protected internal string sourceScopeId;

        public MigratingActivityInstanceValidationReportImpl(MigratingActivityInstance migratingActivityInstance)
        {
            activityInstanceId = migratingActivityInstance.ActivityInstance.Id;
            sourceScopeId = migratingActivityInstance.SourceScope.Id;
            migrationInstruction = migratingActivityInstance.MigrationInstruction;
        }

        public virtual string SourceScopeId
        {
            get { return sourceScopeId; }
        }

        public virtual string ActivityInstanceId
        {
            get { return activityInstanceId; }
        }

        public virtual IMigrationInstruction MigrationInstruction
        {
            get { return migrationInstruction; }
        }

        public virtual bool HasFailures()
        {
            return failures.Count > 0;
        }

        public virtual IList<string> Failures
        {
            get { return failures; }
        }

        public virtual void AddFailure(string failure)
        {
            failures.Add(failure);
        }
    }
}