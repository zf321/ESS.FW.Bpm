using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Parser
{
    /// <summary>
    ///     
    /// </summary>
    public interface IUnresolvedReference
    {
        void Resolve(ProcessDefinitionImpl processDefinition);
    }
}