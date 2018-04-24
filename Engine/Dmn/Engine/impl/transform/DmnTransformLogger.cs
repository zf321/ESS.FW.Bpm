using System;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using ESS.FW.Bpm.Model.Dmn;
using ESS.FW.Bpm.Model.Dmn.instance;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.transform
{
    public class DmnTransformLogger : DmnLogger
    {
        public virtual void DecisionTypeNotSupported(IExpression expression, IDecision decision)
        {
            LogInfo("001",
                string.Format(
                    "The expression type '{0}' of the decision '{1}' is not supported. The decision will be ignored.",
                    expression.GetType().Name, decision.Name));
        }

        public virtual DmnTransformException UnableToTransformDecisionsFromFile(string file, System.Exception cause)
        {
            return
                new DmnTransformException(
                    ExceptionMessage("002", string.Format("Unable to transform decisions from file '{0}'.", file)),
                    cause);
        }

        public virtual DmnTransformException UnableToTransformDecisionsFromInputStream(System.Exception cause)
        {
            return new DmnTransformException(
                ExceptionMessage("003", "Unable to transform decisions from input stream."), cause);
        }

        public virtual DmnTransformException ErrorWhileTransformingDecisions(System.Exception cause)
        {
            return
                new DmnTransformException(
                    ExceptionMessage("004", "Error while transforming decisions: " + cause.Message), cause);
        }

        public virtual DmnTransformException DifferentNumberOfInputsAndInputEntries(int inputsSize, int inputEntriesSize,
            DmnDecisionTableRuleImpl rule)
        {
            return
                new DmnTransformException(ExceptionMessage("005",
                    string.Format("The number of inputs '{0}' and input entries differ '{1}' for rule '{2}'.",
                        inputsSize,
                        inputEntriesSize, rule)));
        }

        public virtual DmnTransformException DifferentNumberOfOutputsAndOutputEntries(int outputsSize,
            int outputEntriesSize, DmnDecisionTableRuleImpl rule)
        {
            return
                new DmnTransformException(ExceptionMessage("006",
                    string.Format("The number of outputs '{0}' and output entries differ '{1}' for rule '{2}'.",
                        outputsSize,
                        outputEntriesSize, rule)));
        }

        public virtual DmnTransformException HitPolicyNotSupported(DmnDecisionTableImpl decisionTable,
            HitPolicy hitPolicy, BuiltinAggregator aggregation)
        {
            if (aggregation == null)
                return
                    new DmnTransformException(ExceptionMessage("007",
                        string.Format("The hit policy '{0}' of decision table '{1}' is not supported.", hitPolicy,
                            decisionTable)));
            return
                new DmnTransformException(ExceptionMessage("007",
                    string.Format(
                        "The hit policy '{0}' with aggregation '{1}' of decision table '{2}' is not supported.",
                        hitPolicy,
                        aggregation, decisionTable)));
        }

        public virtual DmnTransformException CompoundOutputsShouldHaveAnOutputName(
            DmnDecisionTableImpl dmnDecisionTable, DmnDecisionTableOutputImpl dmnOutput)
        {
            return
                new DmnTransformException(ExceptionMessage("008",
                    string.Format(
                        "The decision table '{0}' has a compound output but output '{1}' does not have an output name.",
                        dmnDecisionTable, dmnOutput)));
        }

        public virtual DmnTransformException CompoundOutputWithDuplicateName(DmnDecisionTableImpl dmnDecisionTable,
            DmnDecisionTableOutputImpl dmnOutput)
        {
            return
                new DmnTransformException(ExceptionMessage("009",
                    string.Format(
                        "The decision table '{0}' has a compound output but name of output '{1}' is duplicate.",
                        dmnDecisionTable, dmnOutput)));
        }

        public virtual DmnTransformException DecisionIdIsMissing(IDmnDecision dmnDecision)
        {
            return
                new DmnTransformException(ExceptionMessage("010",
                    string.Format("The decision '{0}' must have an 'id' attribute set.",
                        dmnDecision)));
        }

        public virtual DmnTransformException DecisionTableInputIdIsMissing(IDmnDecision dmnDecision,
            DmnDecisionTableInputImpl dmnDecisionTableInput)
        {
            return
                new DmnTransformException(ExceptionMessage("011",
                    string.Format("The decision table input '{0}' of decision '{1}' must have a 'id' attribute set.",
                        dmnDecisionTableInput, dmnDecision)));
        }

        public virtual DmnTransformException DecisionTableOutputIdIsMissing(IDmnDecision dmnDecision,
            DmnDecisionTableOutputImpl dmnDecisionTableOutput)
        {
            return
                new DmnTransformException(ExceptionMessage("012",
                    string.Format("The decision table output '{0}' of decision '{1}' must have a 'id' attribute set.",
                        dmnDecisionTableOutput, dmnDecision)));
        }

        public virtual DmnTransformException DecisionTableRuleIdIsMissing(IDmnDecision dmnDecision,
            DmnDecisionTableRuleImpl dmnDecisionTableRule)
        {
            return
                new DmnTransformException(ExceptionMessage("013",
                    string.Format("The decision table rule '{0}' of decision '{1}' must have a 'id' attribute set.",
                        dmnDecisionTableRule, dmnDecision)));
        }

        public virtual void DecisionWithoutExpression(IDecision decision)
        {
            LogInfo("014", string.Format("The decision '{0}' has no expression and will be ignored.", decision.Name));
        }

        public virtual DmnTransformException RequiredDecisionLoopDetected(string decisionId)
        {
            return
                new DmnTransformException(ExceptionMessage("015",
                    string.Format("The decision '{0}' has a loop.", decisionId)));
        }

        public virtual DmnTransformException ErrorWhileTransformingDefinitions(System.Exception cause)
        {
            return
                new DmnTransformException(
                    ExceptionMessage("016", "Error while transforming decision requirements graph: " + cause.Message),
                    cause);
        }

        public virtual DmnTransformException DrdIdIsMissing(IDmnDecisionRequirementsGraph drd)
        {
            return
                new DmnTransformException(ExceptionMessage("017",
                    string.Format("The decision requirements graph '{0}' must have an 'id' attribute set.", drd)));
        }

        public virtual DmnTransformException DecisionVariableIsMissing(string decisionId)
        {
            return
                new DmnTransformException(ExceptionMessage("018",
                    string.Format(
                        "The decision '{0}' must have an 'variable' element if it contains a literal expression.",
                        decisionId)));
        }
    }
}