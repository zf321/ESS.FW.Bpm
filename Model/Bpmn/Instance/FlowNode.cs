using System.Collections.Generic;


namespace ESS.FW.Bpm.Model.Bpmn.instance
{
    /// <summary>
    /// The BPMN flowNode element
    /// </summary>
    public interface IFlowNode : IFlowElement
    {
        //IFlowNodeBuilder Builder();

        ICollection<ISequenceFlow> Incoming { get; }

        ICollection<ISequenceFlow> Outgoing { get; }

        IQuery<IFlowNode> PreviousNodes { get; }

        IQuery<IFlowNode> SucceedingNodes { get; }

        bool CamundaAsyncBefore { get; set; }


        bool CamundaAsyncAfter { get; set; }


        bool CamundaExclusive { get; set; }


        string CamundaJobPriority { get; set; }
    }

}