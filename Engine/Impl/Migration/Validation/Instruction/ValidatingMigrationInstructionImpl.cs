using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Migration;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instruction
{
    public class ValidatingMigrationInstructionImpl : IValidatingMigrationInstruction
    {
        protected internal IPvmActivity sourceActivity;
        protected internal IPvmActivity targetActivity;
        protected internal bool updateEventTrigger;

        public ValidatingMigrationInstructionImpl(IPvmActivity sourceActivity, IPvmActivity targetActivity,
            bool updateEventTrigger)
        {
            this.sourceActivity = sourceActivity;
            this.targetActivity = targetActivity;
            this.updateEventTrigger = updateEventTrigger;
        }

        public virtual IPvmActivity SourceActivity
        {
            get { return sourceActivity; }
        }

        public virtual IPvmActivity TargetActivity
        {
            get { return targetActivity; }
        }

        public virtual bool UpdateEventTrigger
        {
            get { return updateEventTrigger; }
        }

        public virtual IMigrationInstruction ToMigrationInstruction()
        {
            return new MigrationInstructionImpl(sourceActivity.Id, targetActivity.Id, updateEventTrigger);
        }

        public override string ToString()
        {
            return "ValidatingMigrationInstructionImpl{" + "sourceActivity=" + sourceActivity + ", targetActivity=" +
                   targetActivity + '}';
        }
    }
}