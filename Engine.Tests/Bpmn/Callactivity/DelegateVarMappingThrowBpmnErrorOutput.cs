using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Variable;

namespace Engine.Tests.Bpmn.Callactivity
{
    public class DelegateVarMappingThrowBpmnErrorOutput : IDelegateVariableMapping
    {
        public void MapInputVariables(IDelegateExecution superExecution, IVariableMap subVariables)
        {
        }

        public void MapOutputVariables(IDelegateExecution superExecution, IVariableScope subInstance)
        {
            throw new BpmnError("1234");
        }

        //public override void mapInputVariables(IDelegateExecution superExecution, IVariableMap subVariables)
        //{
        //}

        //public override void mapOutputVariables(IDelegateExecution superExecution, IVariableScope subInstance)
        //{
        //    throw new BpmnError("1234");
        //}
    }

}