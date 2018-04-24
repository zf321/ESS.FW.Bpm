using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.activity
{
    /// <summary>
    ///     
    /// </summary>
    public class NoCompensationHandlerActivityValidator : IMigrationActivityValidator
    {
        public static NoCompensationHandlerActivityValidator Instance = new NoCompensationHandlerActivityValidator();

        public virtual bool Valid(ActivityImpl activity)
        {
            return !activity.CompensationHandler;
        }
    }
}