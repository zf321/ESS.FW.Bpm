namespace ESS.FW.Bpm.Engine.Delegate
{
    /// <summary>
    ///     
    /// </summary>
    public interface IDelegateCaseExecution : IBaseDelegateExecution, IProcessEngineServicesAware
        //, CmmnModelExecutionContext
    {
        string Id { get; }

        string CaseInstanceId { get; }

        string EventName { get; }

        string CaseBusinessKey { get; }

        string CaseDefinitionId { get; }

        string ParentId { get; }

        string ActivityId { get; }

        string ActivityName { get; }

        string TenantId { get; }

        bool Available { get; }

        bool Enabled { get; }

        bool Disabled { get; }

        bool Active { get; }

        bool Suspended { get; }

        bool Terminated { get; }

        bool Completed { get; }

        bool Failed { get; }

        bool Closed { get; }
    }
}