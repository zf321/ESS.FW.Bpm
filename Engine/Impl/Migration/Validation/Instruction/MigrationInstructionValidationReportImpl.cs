using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Migration;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instruction
{
    public class MigrationInstructionValidationReportImpl : IMigrationInstructionValidationReport
    {
        protected internal IList<string> failures = new List<string>();

        protected internal IMigrationInstruction migrationInstruction;

        public MigrationInstructionValidationReportImpl(IMigrationInstruction migrationInstruction)
        {
            this.migrationInstruction = migrationInstruction;
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

        public override string ToString()
        {
            return "MigrationInstructionValidationReportImpl{" + "migrationInstruction=" + migrationInstruction +
                   ", failures=" + failures + '}';
        }
    }
}