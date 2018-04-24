using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace Engine.Tests.Bpmn.Event.Compensate.Helper
{
    public class WaitStateUndoService : AbstractBpmnActivityBehavior, ISignallableActivityBehavior
    {

        private IExpression _counterName;


        public override void Signal(IActivityExecution execution, string signalName, object signalData)
        {
            Leave(execution);
        }

        public override void Execute(IActivityExecution execution)
        {
            Log.LogDebug("WaitStateUndoService执行Execute：", $"execution：{execution.GetType().Name} _counterName is null?:{(_counterName == null).ToString()}");
            string variableName = (string)_counterName.GetValue(execution);
            object variable = execution.GetVariable(variableName);
            if (variable == null)
            {
                execution.SetVariable(variableName, (int?)1);
            }
            else
            {
                execution.SetVariable(variableName, ((int?)variable) + 1);
            }
            Log.LogDebug("WaitStateUndoService执行Execute完毕：", $"variable is null?：{(variable==null).ToString()}");
        }

        

    }

}