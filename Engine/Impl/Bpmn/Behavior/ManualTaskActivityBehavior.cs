namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     Implementation of the BPMN 2.0 'manual ITask': a ITask that is external to the
    ///     BPMS and to which there is no reference to IT systems whatsoever.
    ///     Given this definition, this activity will behave simply as a pass-though step
    ///     in the process.
    ///     
    /// </summary>
    public class ManualTaskActivityBehavior : TaskActivityBehavior
    {
    }
}