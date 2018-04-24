
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Util.xml;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Variable;

namespace ESS.FW.Bpm.Engine.Form.Impl.Handler
{


    /// <summary>
    ///      
    /// </summary>
    public interface IFormHandler
    {
        void ParseConfiguration(Element activityElement, DeploymentEntity deployment,
            ProcessDefinitionEntity processDefinition, BpmnParse bpmnParse);

        void SubmitFormVariables(IVariableMap properties, IVariableScope variableScope);
    }
}