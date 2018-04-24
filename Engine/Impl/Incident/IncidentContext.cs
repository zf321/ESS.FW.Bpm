using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.Incident
{
    /// <summary>
    ///     The context of an <seealso cref="IIncident" />.
    /// </summary>
    public class IncidentContext
    {
        public virtual string ProcessDefinitionId { get; set; }


        public virtual string ActivityId { get; set; }


        public virtual string ExecutionId { get; set; }


        public virtual string Configuration { get; set; }


        public virtual string TenantId { get; set; }


        public virtual string JobDefinitionId { get; set; }
    }
}