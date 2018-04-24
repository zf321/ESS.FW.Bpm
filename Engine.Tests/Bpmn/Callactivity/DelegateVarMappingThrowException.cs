using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Variable;

namespace Engine.Tests.Bpmn.Callactivity
{    
    public class DelegateVarMappingThrowException : IDelegateVariableMapping
    {
        public void MapInputVariables(IDelegateExecution superExecution, IVariableMap subVariables)
        {
            throw new ProcessEngineException("New process engine exception.");
        }

        public void MapOutputVariables(IDelegateExecution superExecution, IVariableScope subInstance)
        {
            throw new ProcessEngineException("New process engine exception.");
        }

        //public override void mapInputVariables(IDelegateExecution superExecution, IVariableMap subVariables)
        //{
        //    throw new ProcessEngineException("New process engine exception.");
        //}

        //public override void mapOutputVariables(IDelegateExecution superExecution, IVariableScope subInstance)
        //{
        //    throw new ProcessEngineException("New process engine exception.");
        //}
    }

}