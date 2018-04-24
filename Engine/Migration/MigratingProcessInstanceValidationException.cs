namespace ESS.FW.Bpm.Engine.Migration
{
    /// <summary>
    ///     Thrown if at least one migration instruction cannot be applied to the activity instance it matches. Contains
    ///     a object that contains the details for all validation errors.
    ///     
    /// </summary>
    public class MigratingProcessInstanceValidationException : ProcessEngineException
    {
        

        protected internal IMigratingProcessInstanceValidationReport validationReport;

        public MigratingProcessInstanceValidationException(string message,
            IMigratingProcessInstanceValidationReport validationReport) : base(message)
        {
            this.validationReport = validationReport;
        }

        /// <summary>
        ///     A report with all instructions that cannot be applied to the given process instance
        /// </summary>
        public virtual IMigratingProcessInstanceValidationReport ValidationReport
        {
            get { return validationReport; }
        }
    }
}