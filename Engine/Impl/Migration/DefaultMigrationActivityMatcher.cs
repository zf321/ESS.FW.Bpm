using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.migration
{
    public class DefaultMigrationActivityMatcher : IMigrationActivityMatcher
    {
        public virtual bool MatchActivities(ActivityImpl source, ActivityImpl target)
        {
            return (source != null) && (target != null) && EqualId(source, target);
        }

        protected internal virtual bool EqualId(ActivityImpl source, ActivityImpl target)
        {
            return source.Id.Equals(target.Id);
        }
    }
}