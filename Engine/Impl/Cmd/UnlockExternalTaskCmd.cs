using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    public class UnlockExternalTaskCmd : ExternalTaskCmd
    {
        public UnlockExternalTaskCmd(string externalTaskId) : base(externalTaskId)
        {
        }

        protected internal override void ValidateInput()
        {
        }

        protected internal override object Execute(ExternalTaskEntity externalTask)
        {
            externalTask.Unlock();
            return null;
        }
    }
}