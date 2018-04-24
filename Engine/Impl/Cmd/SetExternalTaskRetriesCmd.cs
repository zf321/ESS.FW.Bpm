using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    ///     
    /// </summary>
    public class SetExternalTaskRetriesCmd : ExternalTaskCmd
    {
        protected internal int Retries;

        public SetExternalTaskRetriesCmd(string externalTaskId, int retries) : base(externalTaskId)
        {
            this.Retries = retries;
        }

        protected internal override void ValidateInput()
        {
            EnsureUtil.EnsureGreaterThanOrEqual("retries", Retries, 0);
        }

        protected internal override object Execute(ExternalTaskEntity externalTask)
        {
            externalTask.RetriesAndManageIncidents = Retries;
            return null;
        }
    }
}