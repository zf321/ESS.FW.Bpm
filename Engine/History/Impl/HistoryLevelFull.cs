using ESS.FW.Bpm.Engine.History.Impl.Event;

namespace ESS.FW.Bpm.Engine.History.Impl
{
    /// <summary>
    ///     
    /// </summary>
    public class HistoryLevelFull : AbstractHistoryLevel
    {
        public override int Id
        {
            get { return 3; }
        }

        public override string Name
        {
            get { return ProcessEngineConfiguration.HistoryFull; }
        }

        public override bool IsHistoryEventProduced(HistoryEventTypes eventType, object entity)
        {
            return true;
        }
    }
}