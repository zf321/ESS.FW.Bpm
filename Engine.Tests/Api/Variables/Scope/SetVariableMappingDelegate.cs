using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Variable;

namespace Engine.Tests.Api.Variables.Scope
{
    

	/// <summary>
	/// 
	/// </summary>
	public class SetVariableMappingDelegate : IDelegateVariableMapping
	{

        public void MapInputVariables(IDelegateExecution superExecution, IVariableMap subVariables)
        {
            subVariables.PutValue("orderId", superExecution.GetVariable("orderId"));
        }

     

        public void MapOutputVariables(IDelegateExecution superExecution, IVariableScope subInstance)
        {
            superExecution.SetVariable("targetOrderId", subInstance.GetVariable("orderId"), "SubProcess_1");
        }
    }

}