using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Variable;

namespace Engine.Tests.Bpmn.Callactivity
{    
    public class DelegatedVarMapping : IDelegateVariableMapping
    {
        public void MapInputVariables(IDelegateExecution execution, IVariableMap variables)
        {
            variables.PutValue("TestInputVar", "inValue");
        }

        public void MapOutputVariables(IDelegateExecution execution, IVariableScope subInstance)
        {
            execution.SetVariable("TestOutputVar", "outValue");
        }

        //public override void mapInputVariables(IDelegateExecution execution, IVariableMap variables)
        //{
        //    variables.PutValue("TestInputVar", "inValue");
        //}

        //public override void mapOutputVariables(IDelegateExecution execution, IVariableScope subInstance)
        //{
        //    execution.SetVariable("TestOutputVar", "outValue");
        //}
    }

}