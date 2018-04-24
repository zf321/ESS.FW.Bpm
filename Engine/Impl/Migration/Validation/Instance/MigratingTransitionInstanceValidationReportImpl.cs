using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.migration.instance;
using ESS.FW.Bpm.Engine.Migration;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instance
{
    public class MigratingTransitionInstanceValidationReportImpl : IMigratingTransitionInstanceValidationReport
    {
        protected internal IList<string> failures = new List<string>();
        protected internal IMigrationInstruction migrationInstruction;
        protected internal string sourceScopeId;

        protected internal string transitionInstanceId;

        public MigratingTransitionInstanceValidationReportImpl(MigratingTransitionInstance migratingTransitionInstance)
        {
            transitionInstanceId = migratingTransitionInstance.TransitionInstance.Id;
            sourceScopeId = migratingTransitionInstance.SourceScope.Id;
            migrationInstruction = migratingTransitionInstance.MigrationInstruction;
        }

        public virtual string SourceScopeId
        {
            get { return sourceScopeId; }
        }

        public virtual string TransitionInstanceId
        {
            get { return transitionInstanceId; }
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