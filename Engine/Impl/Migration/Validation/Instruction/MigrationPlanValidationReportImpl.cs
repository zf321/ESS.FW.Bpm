using System.Collections.Generic;
using System.Text;
using ESS.FW.Bpm.Engine.Migration;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instruction
{
    public class MigrationPlanValidationReportImpl : IMigrationPlanValidationReport
    {
        protected internal IList<IMigrationInstructionValidationReport> instructionReports =
            new List<IMigrationInstructionValidationReport>();

        protected internal IMigrationPlan migrationPlan;

        public MigrationPlanValidationReportImpl(IMigrationPlan migrationPlan)
        {
            this.migrationPlan = migrationPlan;
        }

        public virtual IMigrationPlan MigrationPlan
        {
            get { return migrationPlan; }
        }

        public virtual bool HasInstructionReports()
        {
            return instructionReports.Count > 0;
        }

        public virtual IList<IMigrationInstructionValidationReport> InstructionReports
        {
            get { return instructionReports; }
        }

        public virtual void AddInstructionReport(IMigrationInstructionValidationReport instructionReport)
        {
            instructionReports.Add(instructionReport);
        }

        public virtual void WriteTo(StringBuilder sb)
        {
            sb.Append("Migration plan for process definition '")
                .Append(migrationPlan.SourceProcessDefinitionId)
                .Append("' to '")
                .Append(migrationPlan.TargetProcessDefinitionId)
                .Append("' is not valid:\n");

            foreach (var instructionReport in instructionReports)
            {
                sb.Append("\t Migration instruction ")
                    .Append(instructionReport.MigrationInstruction)
                    .Append(" is not valid:\n");
                foreach (var failure in instructionReport.Failures)
                    sb.Append("\t\t").Append(failure).Append("\n");
            }
        }
    }
}