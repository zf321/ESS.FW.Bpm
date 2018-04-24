
using ESS.FW.Bpm.Engine.Form.Impl.Handler;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Variable;

namespace ESS.FW.Bpm.Engine.Form.Impl
{
    /// <summary>
    ///     
    ///     
    /// </summary>
    public class FormPropertyHelper
    {
        public static void InitFormPropertiesOnScope(IVariableMap variables, IActivityExecution execution)
        {
            ProcessDefinitionEntity pd = (ProcessDefinitionEntity)((PvmExecutionImpl)execution).ProcessDefinition;
            IStartFormHandler startFormHandler = pd.StartFormHandler;
            startFormHandler.SubmitFormVariables(variables, execution);
        }
    }
}