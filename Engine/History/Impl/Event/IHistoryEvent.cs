namespace ESS.FW.Bpm.Engine.History.Impl.Event
{
    public interface IHistoryEvent
    {
        string CaseDefinitionId { get; set; }
        string CaseDefinitionKey { get; set; }
        string CaseDefinitionName { get; set; }
        string CaseExecutionId { get; set; }
        string CaseInstanceId { get; set; }
        string EventType { get; set; }
        string ExecutionId { get; set; }
        string Id { get; set; }
        string ProcessDefinitionId { get; set; }
        string ProcessDefinitionKey { get; set; }
        string ProcessDefinitionName { get; set; }
        int? ProcessDefinitionVersion { get; set; }
        string ProcessInstanceId { get; set; }
        long SequenceCounter { get; set; }

        object GetPersistentState();
        bool IsEventOfType(HistoryEventTypes type);
        string ToString();
    }
}