using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.builder;


namespace ESS.FW.Bpm.Model.Bpmn.instance
{

    using ProcessBuilder = ProcessBuilder;



    /// <summary>
    /// The BPMN process element
    /// 
    /// 
    /// 
    /// </summary>
    public interface IProcess : ICallableElement
    {

        IProcessBuilder Builder();

        ProcessType ProcessType { get; set; }


        bool Closed { get; set; }


        bool Executable { get; set; }


        // TODO: collaboration ref

        IAuditing Auditing { get; set; }


        IMonitoring Monitoring { get; set; }


        ICollection<IProperty> Properties { get; }

        ICollection<ILaneSet> LaneSets { get; }

        ICollection<IFlowElement> FlowElements { get; }

        ICollection<IArtifact> Artifacts { get; }

        ICollection<ICorrelationSubscription> CorrelationSubscriptions { get; }

        ICollection<IResourceRole> ResourceRoles { get; }

        ICollection<IProcess> Supports { get; }

        /// <summary>
        /// camunda extensions </summary>

        string CamundaCandidateStarterGroups { get; set; }


        IList<string> CamundaCandidateStarterGroupsList { get; set; }


        string CamundaCandidateStarterUsers { get; set; }


        IList<string> CamundaCandidateStarterUsersList { get; set; }


        string CamundaJobPriority { get; set; }


        string CamundaTaskPriority { get; set; }

        int? CamundaHistoryTimeToLive { get; set; }

        string CamundaHistoryTimeToLiveString { get; set; }

    }

}