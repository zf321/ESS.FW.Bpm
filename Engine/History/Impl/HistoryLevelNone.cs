using ESS.FW.Bpm.Engine.History.Impl.Event;

namespace ESS.FW.Bpm.Engine.History.Impl
{
    /// <summary>
    ///     
    /// </summary>
    public class HistoryLevelNone : AbstractHistoryLevel
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Name
        {
            get { return ProcessEngineConfiguration.HistoryNone; }
        }

        public override bool IsHistoryEventProduced(HistoryEventTypes eventType, object entity)
        {
            return false;
        }
    }
}