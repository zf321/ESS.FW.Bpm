using System;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.transform;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl
{
    public class DmnEngineLogger : DmnLogger
    {
        public virtual DmnTransformException unableToFindDecisionWithKey(string decisionKey)
        {
            return
                new DmnTransformException(ExceptionMessage("001",
                    string.Format("Unable to find decision with id '{0}' in model.",
                        decisionKey)));
        }

        public virtual DmnEvaluationException unableToEvaluateExpression(string expression, string expressionLanguage,
            System.Exception cause)
        {
            return
                new DmnEvaluationException(
                    ExceptionMessage("002",
                        string.Format("Unable to evaluate expression for language '{0}': '{1}'", expressionLanguage,
                            expression)), cause);
        }

        public virtual DmnEvaluationException noScriptEngineFoundForLanguage(string expressionLanguage)
        {
            return
                new DmnEvaluationException(ExceptionMessage("003",
                    string.Format("Unable to find script engine for expression language '{0}'.", expressionLanguage)));
        }

        public virtual DmnEngineException decisionTypeNotSupported(IDmnDecision decision)
        {
            return
                new DmnEngineException(ExceptionMessage("004",
                    string.Format("Decision type '{0}' not supported by DMN engine.",
                        decision.GetType())));
        }

        public virtual DmnEngineException invalidValueForTypeDefinition(string typeName, object value)
        {
            return
                new DmnEngineException(ExceptionMessage("005",
                    string.Format("Invalid value '{0}' for clause with type '{1}'.", value,
                        typeName)));
        }

        public virtual void unsupportedTypeDefinitionForClause(string typeName)
        {
            LogWarn("006",
                string.Format(
                    "Unsupported type '{0}' for clause. Values of this clause will not transform into another type.",
                    typeName));
        }

        //public virtual DmnDecisionResultException decisionOutputHasMoreThanOneValue(DmnDecisionRuleResult ruleResult)
        //{
        //    return
        //        new DmnDecisionResultException(ExceptionMessage("007",
        //            "Unable to get single decision rule result entry as it has more than one entry '{}'", ruleResult));
        //}

        public virtual DmnDecisionResultException decisionResultHasMoreThanOneOutput(
            IDmnDecisionTableResult decisionResult)
        {
            return
                new DmnDecisionResultException(ExceptionMessage("008",
                    string.Format(
                        "Unable to get single decision rule result as it has more than one rule result '{0}'",
                        decisionResult)));
        }

        public virtual DmnTransformException unableToFindAnyDecisionTable()
        {
            return new DmnTransformException(ExceptionMessage("009", "Unable to find any decision table in model."));
        }

        public virtual DmnDecisionResultException decisionOutputHasMoreThanOneValue(IDmnDecisionResultEntries result)
        {
            return
                new DmnDecisionResultException(ExceptionMessage("010",
                    string.Format("Unable to get single decision result entry as it has more than one entry '{0}'",
                        result)));
        }

        public virtual DmnDecisionResultException decisionResultHasMoreThanOneOutput(IDmnDecisionResult decisionResult)
        {
            return
                new DmnDecisionResultException(ExceptionMessage("011",
                    string.Format("Unable to get single decision result as it has more than one result '{0}'",
                        decisionResult)));
        }

        public virtual DmnEngineException decisionLogicTypeNotSupported(IDmnDecisionLogic decisionLogic)
        {
            return
                new DmnEngineException(ExceptionMessage("012",
                    string.Format("Decision logic type '{0}' not supported by DMN engine.",
                        decisionLogic.GetType())));
        }

        public virtual DmnEngineException decisionIsNotADecisionTable(IDmnDecision decision)
        {
            return
                new DmnEngineException(ExceptionMessage("013",
                    string.Format("The decision '{0}' is not implemented as decision table.",
                        decision)));
        }
    }
}