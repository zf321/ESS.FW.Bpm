using ESS.FW.Bpm.Engine.History.Impl.Event;

namespace ESS.FW.Bpm.Engine.History.Impl
{

    /// <summary>
    ///     
    /// </summary>
    public class HistoryLevelAudit : HistoryLevelActivity
    {
        public override int Id
        {
            get { return 2; }
        }

        public override string Name
        {
            get { return ProcessEngineConfiguration.HistoryAudit; }
        }

        public override bool IsHistoryEventProduced(HistoryEventTypes eventType, object entity)
        {
            return base.IsHistoryEventProduced(eventType, entity) ||
                   (HistoryEventTypes.VariableInstanceCreate == eventType) ||
                   (HistoryEventTypes.VariableInstanceUpdate == eventType) ||
                   (HistoryEventTypes.VariableInstanceMigrate == eventType) ||
                   (HistoryEventTypes.VariableInstanceDelete == eventType) ||
                   (HistoryEventTypes.FormPropertyUpdate == eventType);
        }
    }
}