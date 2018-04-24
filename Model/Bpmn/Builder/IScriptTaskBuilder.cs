using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    public interface IScriptTaskBuilder: ITaskBuilder<IScriptTask>
    {
        IScriptTaskBuilder CamundaResource(string camundaResource);
        IScriptTaskBuilder CamundaResultVariable(string camundaResultVariable);
        IScriptTaskBuilder Script(IScript script);
        IScriptTaskBuilder ScriptFormat(string scriptFormat);
        IScriptTaskBuilder ScriptText(string scriptText);
    }
}