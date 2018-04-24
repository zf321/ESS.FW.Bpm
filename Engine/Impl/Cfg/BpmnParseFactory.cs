using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;

namespace ESS.FW.Bpm.Engine.Impl.Cfg
{
    /// <summary>
    ///     
    /// </summary>
    public interface IBpmnParseFactory
    {
        BpmnParse CreateBpmnParse(BpmnParser bpmnParser);
    }
}