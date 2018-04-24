using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Dmn.engine.@delegate;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Model.Dmn;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.hitpolicy
{
    public class DmnHitPolicyLogger : DmnLogger
    {
        public virtual DmnHitPolicyException uniqueHitPolicyOnlyAllowsSingleMatchingRule(
            IList<IDmnEvaluatedDecisionRule> matchingRules)
        {
            return
                new DmnHitPolicyException(ExceptionMessage("001",
                    "Hit policy '{0}' only allows a single rule to match. Actually match rules: '{1}'.", HitPolicy.Unique,
                    matchingRules));
        }

        public virtual DmnHitPolicyException anyHitPolicyRequiresThatAllOutputsAreEqual(
            IList<IDmnEvaluatedDecisionRule> matchingRules)
        {
            return
                new DmnHitPolicyException(ExceptionMessage("002",
                    "Hit policy '{0}' only allows multiple matching rules with equal output. Matching rules: '{1}'.",
                    HitPolicy.Any, matchingRules));
        }

        public virtual DmnHitPolicyException aggregationNotApplicableOnCompoundOutput(BuiltinAggregator aggregator,
            IDictionary<string, IDmnEvaluatedOutput> outputEntries)
        {
            return
                new DmnHitPolicyException(ExceptionMessage("003",
                    "Unable to execute aggregation '{0}' on compound decision output '{1}'. Only one output entry allowed.",
                    aggregator, outputEntries));
        }

        public virtual DmnHitPolicyException unableToConvertValuesToAggregatableTypes(IList<ITypedValue> values,
            params Type[] targetClasses)
        {
            return
                new DmnHitPolicyException(ExceptionMessage("004",
                    "Unable to convert value '{0}' to a support aggregatable type '{1}'.", values, targetClasses));
        }
    }
}