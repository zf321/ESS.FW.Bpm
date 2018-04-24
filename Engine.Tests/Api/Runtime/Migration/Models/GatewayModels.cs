using ESS.FW.Bpm.Model.Bpmn;

namespace Engine.Tests.Api.Runtime.Migration.Models
{
    /// <summary>
    /// </summary>
    public class GatewayModels
    {
        public static readonly IBpmnModelInstance PARALLEL_GW = ProcessModels.NewModel()
            .StartEvent()
            .ParallelGateway("fork")
            .UserTask("parallel1")
            .ParallelGateway("join")
            //.MoveToNode("fork")
            .UserTask("parallel2")
            //.ConnectTo("join")
            .UserTask("afterJoin")
            .EndEvent()
            .Done();

        public static readonly IBpmnModelInstance PARALLEL_GW_IN_SUBPROCESS = ProcessModels.NewModel()
            .StartEvent()
            .SubProcess("subProcess")
            //.EmbeddedSubProcess()
            //.StartEvent()
            .ParallelGateway("fork")
            .UserTask("parallel1")
            .ParallelGateway("join")
            //.MoveToNode("fork")
            .UserTask("parallel2")
            //.ConnectTo("join")
            .UserTask("afterJoin")
            .EndEvent()
            .SubProcessDone()
            .EndEvent()
            .Done();

        public static readonly IBpmnModelInstance INCLUSIVE_GW = ProcessModels.NewModel()
            .StartEvent()
            .InclusiveGateway("fork")
            .UserTask("parallel1")
            .InclusiveGateway("join")
            //.MoveToNode("fork")
            .UserTask("parallel2")
            //.ConnectTo("join")
            .UserTask("afterJoin")
            .EndEvent()
            .Done();

        public static readonly IBpmnModelInstance INCLUSIVE_GW_IN_SUBPROCESS = ProcessModels.NewModel()
            .StartEvent()
            .SubProcess()
            //.EmbeddedSubProcess()
            //.StartEvent()
            .InclusiveGateway("fork")
            .UserTask("parallel1")
            .InclusiveGateway("join")
            //.MoveToNode("fork")
            .UserTask("parallel2")
            //.ConnectTo("join")
            .UserTask("afterJoin")
            .EndEvent()
            .SubProcessDone()
            .EndEvent()
            .Done();
    }
}