using ESS.FW.Bpm.Engine.Migration;

namespace ESS.FW.Bpm.Engine.Impl.migration
{
    /// <summary>
    ///     
    /// </summary>
    public class MigrationInstructionImpl : IMigrationInstruction
    {
        protected internal string sourceActivityId;
        protected internal string targetActivityId;

        protected internal bool updateEventTrigger;

        public MigrationInstructionImpl(string sourceActivityId, string targetActivityId)
            : this(sourceActivityId, targetActivityId, false)
        {
        }

        public MigrationInstructionImpl(string sourceActivityId, string targetActivityId, bool updateEventTrigger)
        {
            this.sourceActivityId = sourceActivityId;
            this.targetActivityId = targetActivityId;
            this.updateEventTrigger = updateEventTrigger;
        }

        public virtual string SourceActivityId
        {
            get { return sourceActivityId; }
        }

        public virtual string TargetActivityId
        {
            get { return targetActivityId; }
        }

        public virtual bool UpdateEventTrigger
        {
            get { return updateEventTrigger; }
            set { updateEventTrigger = value; }
        }


        public override string ToString()
        {
            return "MigrationInstructionImpl{" + "sourceActivityId='" + sourceActivityId + '\'' + ", targetActivityId='" +
                   targetActivityId + '\'' + ", updateEventTrigger='" + updateEventTrigger + '\'' + '}';
        }
    }
}