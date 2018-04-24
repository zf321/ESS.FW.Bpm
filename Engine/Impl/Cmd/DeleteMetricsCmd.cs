using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class DeleteMetricsCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;
        protected internal string Reporter;

        protected internal DateTime? Timestamp;

        public DeleteMetricsCmd(DateTime? timestamp, string reporter)
        {
            this.Timestamp = timestamp;
            this.Reporter = reporter;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            if ((Timestamp == null) && ReferenceEquals(Reporter, null))
            {
                commandContext.MeterLogManager.DeleteAll();
            }
            else
            {
                commandContext.MeterLogManager
                 .DeleteByTimestampAndReporter(Timestamp, Reporter);
            }
            return null;
        }
    }
}