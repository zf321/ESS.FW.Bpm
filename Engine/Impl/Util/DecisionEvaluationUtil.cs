using System;
using ESS.FW.Bpm.Engine.context.Impl;

using ESS.FW.Bpm.Engine.Dmn.engine;
using ESS.FW.Bpm.Engine.Dmn.engine.impl;
using ESS.FW.Bpm.Engine.Dmn.Impl.Invocation;
using ESS.FW.Bpm.Engine.Dmn.Impl.Result;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Context;

namespace ESS.FW.Bpm.Engine.Impl.Util
{
    /// <summary>
    ///     
    /// </summary>
    public class DecisionEvaluationUtil
    {
        public const string DecisionResultVariable = "decisionResult";

        public static IDecisionResultMapper GetDecisionResultMapperForName(string mapDecisionResult)
        {
            if ("singleEntry".Equals(mapDecisionResult))
                return new SingleEntryDecisionResultMapper();
            if ("singleResult".Equals(mapDecisionResult))
                return new SingleResultDecisionResultMapper();
            if ("collectEntries".Equals(mapDecisionResult))
                return new CollectEntriesDecisionResultMapper();
            if ("resultList".Equals(mapDecisionResult) || ReferenceEquals(mapDecisionResult, null))
                return new ResultListDecisionTableResultMapper();
            return null;
        }

//JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void evaluateDecision(org.camunda.bpm.engine.impl.core.Variable.scope.AbstractVariableScope execution, org.camunda.bpm.engine.impl.core.model.BaseCallableElement callableElement, String resultVariable, org.camunda.bpm.engine.impl.dmn.result.DecisionResultMapper decisionResultMapper) throws Exception
        public static void EvaluateDecision(AbstractVariableScope execution, BaseCallableElement callableElement,
            string resultVariable, IDecisionResultMapper decisionResultMapper)
        {
            var decisionDefinition = ResolveDecisionDefinition(callableElement, execution);
            var invocation = CreateInvocation(decisionDefinition, execution);

            Invoke(invocation);

            var result = (IDmnDecisionResult) invocation.InvocationResult;
            if (result != null)
            {
                execution.SetVariableLocalTransient(DecisionResultVariable, result);

                if (!ReferenceEquals(resultVariable, null) && (decisionResultMapper != null))
                {
                    var mappedDecisionResult = decisionResultMapper.MapDecisionResult(result);
                    execution.SetVariable(resultVariable, mappedDecisionResult);
                }
            }
        }

//JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.camunda.bpm.dmn.engine.DmnDecisionResult evaluateDecision(org.camunda.bpm.engine.repository.DecisionDefinition decisionDefinition, org.camunda.bpm.engine.Variable.VariableMap variables) throws Exception
        public static IDmnDecisionResult EvaluateDecision(IDecisionDefinition decisionDefinition, IVariableMap variables)
        {
            var invocation = CreateInvocation(decisionDefinition, variables);
            Invoke(invocation);
            return (IDmnDecisionResult) invocation.InvocationResult;
        }

//JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.camunda.bpm.dmn.engine.DmnDecisionTableResult evaluateDecisionTable(org.camunda.bpm.engine.repository.DecisionDefinition decisionDefinition, org.camunda.bpm.engine.Variable.VariableMap variables) throws Exception
        public static IDmnDecisionTableResult EvaluateDecisionTable(IDecisionDefinition decisionDefinition,
            IVariableMap variables)
        {
            // doesn't throw an exception if the decision definition is not implemented as decision table
            var decisionResult = EvaluateDecision(decisionDefinition, variables);
            return (IDmnDecisionTableResult)DmnDecisionTableResultImpl.Wrap(decisionResult);
        }

//JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected static void invoke(org.camunda.bpm.engine.impl.dmn.invocation.DecisionInvocation invocation) throws Exception
        protected internal static void Invoke(DecisionInvocation invocation)
        {
            Context.ProcessEngineConfiguration.DelegateInterceptor.HandleInvocation(invocation);
        }

        protected internal static DecisionInvocation CreateInvocation(IDecisionDefinition decisionDefinition,
            IVariableMap variables)
        {
            return CreateInvocation(decisionDefinition, variables.AsVariableContext());
        }

        protected internal static DecisionInvocation CreateInvocation(IDecisionDefinition decisionDefinition,
            AbstractVariableScope variableScope)
        {
            return CreateInvocation(decisionDefinition, VariableScopeContext.Wrap(variableScope));
        }

        protected internal static DecisionInvocation CreateInvocation(IDecisionDefinition decisionDefinition,
            IVariableContext variableContext)
        {
            return new DecisionInvocation(decisionDefinition, variableContext);
        }

        protected internal static IDecisionDefinition ResolveDecisionDefinition(BaseCallableElement callableElement,
            AbstractVariableScope execution)
        {
            //throw new NotImplementedException();
            return CallableElementUtil.GetDecisionDefinitionToCall(execution, callableElement);
        }
    }
}