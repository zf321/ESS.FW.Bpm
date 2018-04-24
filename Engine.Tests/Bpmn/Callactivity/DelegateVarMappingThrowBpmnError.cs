using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Variable;

namespace Engine.Tests.Bpmn.Callactivity
{
    public class DelegateVarMappingThrowBpmnError : IDelegateVariableMapping
    {
        public void MapInputVariables(IDelegateExecution superExecution, IVariableMap subVariables)
        {
            throw new BpmnError("1234");
        }

        public void MapOutputVariables(IDelegateExecution superExecution, IVariableScope subInstance)
        {
            throw new ProcessEngineException("New process engine exception.");
        }

        //public override void mapInputVariables(IDelegateExecution superExecution, IVariableMap subVariables)
        //{
        //    throw new BpmnError("1234");
        //}

        //public override void mapOutputVariables(IDelegateExecution superExecution, IVariableScope subInstance)
        //{
        //    throw new ProcessEngineException("New process engine exception.");
        //}
    }

}