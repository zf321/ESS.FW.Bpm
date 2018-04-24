using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Dmn.engine;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace Engine.Tests.Dmn.BusinessRuleTask
{
    /// <summary>
    /// </summary>
    public class DecisionResultTestListener : IDelegateListener<IBaseDelegateExecution>
    {
        public static IDmnDecisionResult decisionResult;

        public static IDmnDecisionResult DecisionResult
        {
            get { return decisionResult; }
        }
        //自定义类可获取运行时var
        public void Notify(IBaseDelegateExecution execution)
        {
            decisionResult = (IDmnDecisionResult) execution.GetVariable(DecisionEvaluationUtil.DecisionResultVariable);
        }

        public static void Reset()
        {
            decisionResult = null;
        }
    }
}