using ESS.FW.Bpm.Engine.History.Impl.Event;

namespace ESS.FW.Bpm.Engine.History.Impl
{
    /// <summary>
    ///     
    /// </summary>
    public class HistoryLevelActivity : AbstractHistoryLevel
    {
        public override int Id
        {
            get { return 1; }
        }

        public override string Name
        {
            get { return ProcessEngineConfiguration.HistoryActivity; }
        }

        public override bool IsHistoryEventProduced(HistoryEventTypes eventType, object entity)
        {
            return (HistoryEventTypes.ProcessInstanceStart == eventType) ||
                   (HistoryEventTypes.ProcessInstanceUpdate == eventType) ||
                   (HistoryEventTypes.ProcessInstanceMigrate == eventType) ||
                   (HistoryEventTypes.ProcessInstanceEnd == eventType) ||
                   (HistoryEventTypes.TaskInstanceCreate == eventType) ||
                   (HistoryEventTypes.TaskInstanceUpdate == eventType) ||
                   (HistoryEventTypes.TaskInstanceMigrate == eventType) ||
                   (HistoryEventTypes.TaskInstanceComplete == eventType) ||
                   (HistoryEventTypes.TaskInstanceDelete == eventType) ||
                   (HistoryEventTypes.ActivityInstanceStart == eventType) ||
                   (HistoryEventTypes.ActivityInstanceUpdate == eventType) ||
                   (HistoryEventTypes.ActivityInstanceMigrate == eventType) ||
                   (HistoryEventTypes.ActivityInstanceEnd == eventType) ||
                   (HistoryEventTypes.CaseInstanceCreate == eventType) ||
                   (HistoryEventTypes.CaseInstanceUpdate == eventType) ||
                   (HistoryEventTypes.CaseInstanceClose == eventType) ||
                   (HistoryEventTypes.CaseActivityInstanceCreate == eventType) ||
                   (HistoryEventTypes.CaseActivityInstanceUpdate == eventType) ||
                   (HistoryEventTypes.CaseActivityInstanceEnd == eventType);
        }
    }
}