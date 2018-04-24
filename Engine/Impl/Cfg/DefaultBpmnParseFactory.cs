using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;

namespace ESS.FW.Bpm.Engine.Impl.Cfg
{
    /// <summary>
    ///     
    /// </summary>
    public class DefaultBpmnParseFactory : IBpmnParseFactory
    {
        public virtual BpmnParse CreateBpmnParse(BpmnParser bpmnParser)
        {
            return new BpmnParse(bpmnParser);
        }
    }
}