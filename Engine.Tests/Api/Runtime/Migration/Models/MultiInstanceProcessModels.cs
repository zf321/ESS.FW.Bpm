using ESS.FW.Bpm.Model.Bpmn;

namespace Engine.Tests.Api.Runtime.Migration.Models
{
    /// <summary>
    /// </summary>
    public class MultiInstanceProcessModels
    {
        public static readonly IBpmnModelInstance PAR_MI_ONE_TASK_PROCESS =
                ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
            //.ActivityBuilder("userTask")
            //.MultiInstance()
            //.Parallel()
            //.Cardinality("3")
            //.Done()
            ;

        public static readonly IBpmnModelInstance PAR_MI_SUBPROCESS_PROCESS =
                ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
            //.ActivityBuilder("subProcess")
            //.MultiInstance()
            //.Parallel()
            //.Cardinality("3")
            //.Done()
            ;

        public static readonly IBpmnModelInstance PAR_MI_DOUBLE_SUBPROCESS_PROCESS =
                ModifiableBpmnModelInstance.Modify(ProcessModels.DoubleSubprocessProcess)
            //.ActivityBuilder("outerSubProcess")
            //.MultiInstance()
            //.Parallel()
            //.Cardinality("3")
            //.Done()
            ;

        public static readonly IBpmnModelInstance SEQ_MI_ONE_TASK_PROCESS =
                ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
            //.ActivityBuilder("userTask")
            //.MultiInstance()
            //.Sequential()
            //.Cardinality("3")
            //.Done()
            ;

        public static readonly IBpmnModelInstance SEQ_MI_SUBPROCESS_PROCESS =
                ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
            //.ActivityBuilder("subProcess")
            //.MultiInstance()
            //.Sequential()
            //.Cardinality("3")
            //.Done()
            ;
    }
}