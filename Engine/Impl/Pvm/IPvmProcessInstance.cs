using ESS.FW.Bpm.Engine.Persistence.Entity;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ESS.FW.Bpm.Engine.Impl.Pvm
{
    /// <summary>
    ///      
    ///     
    /// </summary>
    public interface IPvmProcessInstance : IPvmExecution
    {
        /// <summary>
        ///     returns whether this execution has ended or not.
        /// </summary>
        bool IsEnded { get; set; }

        void Start();

        void Start(IDictionary<string, object> variables);

        IPvmExecution FindExecution(string activityId);

        IList<IPvmExecution> FindExecutions(string activityId);

        IList<string> FindActiveActivityIds();

        void DeleteCascade(string deleteReason);
        void AddExecutionObserver(IExecutionObserver observer);
        [JsonProperty("ProcessInstance_ProcessDefinitionId")]
        string ProcessDefinitionId { get; set; }
        [JsonProperty("ProcessInstance_ProcessInstanceId")]
        string ProcessInstanceId { get; set; }
        [JsonProperty("ProcessInstance_TenantId")]
        string TenantId { get; set; }
        [JsonProperty("ProcessInstance_Id")]
        string Id { get; set; }
        [JsonProperty("ProcessInstance_IsSuspended")]
        bool IsSuspended { get; set; }
        [JsonProperty("ProcessInstance_CaseInstanceId")]
        string CaseInstanceId { get; set; }
        [JsonProperty("ProcessInstance_BusinessKey")]
        string BusinessKey { get; set; }
    }
}