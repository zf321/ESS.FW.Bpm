using ESS.FW.Bpm.Engine.Authorization;

namespace ESS.FW.Bpm.Engine.Migration
{
    /// <summary>
    ///     Thrown if a migration plan is not valid, e.g. because it contains instructions that can in general not be executed.
    ///     Contains a <seealso cref="IMigrationPlanValidationReport" /> that contains the details for all validation erorrs.
    ///     
    /// </summary>
    public class MigrationPlanValidationException : BadUserRequestException
    {
        

        protected internal IMigrationPlanValidationReport validationReport;

        public MigrationPlanValidationException(string message, IMigrationPlanValidationReport validationReport)
            : base(message)
        {
            this.validationReport = validationReport;
        }

        /// <summary>
        ///     A report with all invalid instructions
        /// </summary>
        public virtual IMigrationPlanValidationReport ValidationReport
        {
            get { return validationReport; }
        }
    }
}