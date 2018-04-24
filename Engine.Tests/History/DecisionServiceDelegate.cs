using System;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Dmn.engine;
using IDmnDecisionTableResult = ESS.FW.Bpm.Engine.IDmnDecisionTableResult;

namespace Engine.Tests.History
{
	[Serializable]
	public class DecisionServiceDelegate : IJavaDelegate, ICaseExecutionListener
	{

	  private const long serialVersionUID = 1L;
        
	  public void Execute(IBaseDelegateExecution execution)
	  {
		IDecisionService decisionService =((IDelegateExecution) execution).ProcessEngineServices.DecisionService;
            EvaluateDecision(decisionService, execution);
	  }
        
	  public virtual void Notify(IDelegateCaseExecution caseExecution)
	  {
		IDecisionService decisionService = caseExecution.ProcessEngineServices.DecisionService;
            EvaluateDecision(decisionService, caseExecution);
	  }

	  public virtual bool Evaluate(IDelegateCaseExecution caseExecution)
	  {
		IDecisionService decisionService = caseExecution.ProcessEngineServices.DecisionService;
		IDmnDecisionTableResult result = EvaluateDecision(decisionService, caseExecution);
		IDmnDecisionRuleResult singleResult = result.GetSingleResult();
            return (bool)singleResult.getFirstEntry<bool>();
	  }

	  protected internal virtual IDmnDecisionTableResult EvaluateDecision(IDecisionService decisionService, IVariableScope variableScope)
	  {
		return decisionService.EvaluateDecisionTableByKey("testDecision", variableScope.Variables);
	  }

	}

}