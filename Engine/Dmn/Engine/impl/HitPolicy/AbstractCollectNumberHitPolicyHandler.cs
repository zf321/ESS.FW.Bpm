using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Dmn.engine.@delegate;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.@delegate;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.hitpolicy;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Model.Dmn;
using System.Linq;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.hitpolicy
{
    public abstract class AbstractCollectNumberHitPolicyHandler : IDmnHitPolicyHandler
    {
        public static readonly DmnHitPolicyLogger LOG = DmnLogger.HIT_POLICY_LOGGER;

        protected internal abstract BuiltinAggregator Aggregator { get; }
        public abstract HitPolicyEntry HitPolicyEntry { get; }

        public virtual IDmnDecisionTableEvaluationEvent apply(
            IDmnDecisionTableEvaluationEvent decisionTableEvaluationEvent)
        {
            var resultName = getResultName(decisionTableEvaluationEvent);
            var resultValue = getResultValue(decisionTableEvaluationEvent);

            var evaluationEvent = (DmnDecisionTableEvaluationEventImpl)decisionTableEvaluationEvent;
            evaluationEvent.CollectResultName = resultName;
            evaluationEvent.CollectResultValue = resultValue;

            return evaluationEvent;
        }

        protected internal virtual string getResultName(IDmnDecisionTableEvaluationEvent decisionTableEvaluationEvent)
        {
            foreach (var matchingRule in decisionTableEvaluationEvent.MatchingRules)
            {
                var outputEntries = matchingRule.OutputEntries;
                if (outputEntries.Count > 0)
                    return outputEntries.Values.First().OutputName;
            }
            return null;
        }

        protected internal virtual ITypedValue getResultValue(IDmnDecisionTableEvaluationEvent decisionTableEvaluationEvent)
        {
            var values = collectSingleValues(decisionTableEvaluationEvent);
            return aggregateValues(values);
        }

        protected internal virtual IList<ITypedValue> collectSingleValues(
            IDmnDecisionTableEvaluationEvent decisionTableEvaluationEvent)
        {
            IList<ITypedValue> values = new List<ITypedValue>();
            foreach (var matchingRule in decisionTableEvaluationEvent.MatchingRules)
            {
                var outputEntries = matchingRule.OutputEntries;
                if (outputEntries.Count > 1)
                    throw LOG.aggregationNotApplicableOnCompoundOutput(Aggregator, outputEntries);
                if (outputEntries.Count == 1)
                {
                    ITypedValue typedValue = outputEntries.Values.First().Value;
                    values.Add(typedValue);
                }
                // ignore empty output entries
            }
            return values;
        }

        protected internal virtual ITypedValue aggregateValues(IList<ITypedValue> values)
        {
            if (values.Count > 0)
                return aggregateNumberValues(values);
            // return null if no values to aggregate
            return null;
        }

        protected internal virtual ITypedValue aggregateNumberValues(IList<ITypedValue> values)
        {
            try
            {
                IList<int> intValues = convertValuesToInteger(values);
                return Variables.IntegerValue(aggregateIntegerValues(intValues));
            }
            catch (System.ArgumentException)
            {
                // ignore
            }

            try
            {
                IList<long> longValues = convertValuesToLong(values);
                return Variables.LongValue(aggregateLongValues(longValues));
            }
            catch (System.ArgumentException)
            {
                // ignore
            }

            try
            {
                IList<double> doubleValues = convertValuesToDouble(values);
                return Variables.DoubleValue(aggregateDoubleValues(doubleValues));
            }
            catch (System.ArgumentException)
            {
                // ignore
            }

            throw LOG.unableToConvertValuesToAggregatableTypes(values, typeof(int), typeof(long), typeof(double));
        }

        protected internal abstract int? aggregateIntegerValues(IList<int> intValues);

        protected internal abstract long? aggregateLongValues(IList<long> longValues);

        protected internal abstract double? aggregateDoubleValues(IList<double> doubleValues);

        //JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: protected java.Util.List<Nullable<int>> convertValuesToInteger(java.Util.List<TypedValue> typedValues) throws IllegalArgumentException
        protected internal virtual IList<int> convertValuesToInteger(IList<ITypedValue> typedValues)
        {
            IList<int> intValues = new List<int>();
            foreach (ITypedValue typedValue in typedValues)
            {

                if (typedValue is IntegerValueImpl)
                {
                    intValues.Add((int)typedValue.Value);

                }
                else if (typedValue.Type == null)
                {
                    // check if it is an integer

                    object value = typedValue.Value;
                    if (value is int)
                    {
                        intValues.Add((int)value);

                    }
                    else
                    {
                        throw new System.ArgumentException();
                    }

                }
                else
                {
                    // reject other typed values
                    throw new System.ArgumentException();
                }

            }
            return intValues;
        }

        //JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: protected java.Util.List<Nullable<long>> convertValuesToLong(java.Util.List<TypedValue> typedValues) throws IllegalArgumentException
        protected internal virtual IList<long> convertValuesToLong(IList<ITypedValue> typedValues)
        {
            IList<long> longValues = new List<long>();
            foreach (ITypedValue typedValue in typedValues)
            {

                if (typedValue is LongValueImpl)
                {
                    longValues.Add((long)typedValue.Value);

                }
                else if (typedValue.Type == null)
                {
                    // check if it is a long or a string of a number

                    object value = typedValue.Value;
                    if (value is long?)
                    {
                        longValues.Add((long)value);

                    }
                    else
                    {
                        long longValue = Convert.ToInt64(value.ToString());
                        longValues.Add(longValue);
                    }

                }
                else
                {
                    // reject other typed values
                    throw new System.ArgumentException();
                }

            }
            return longValues;
        }


        protected internal virtual IList<double> convertValuesToDouble(IList<ITypedValue> typedValues)
        {
            IList<double> doubleValues = new List<double>();
            foreach (ITypedValue typedValue in typedValues)
            {

                if (typedValue is DoubleValueImpl)
              {
                    doubleValues.Add((double)typedValue.Value);

                }
              else if (typedValue.Type == null)
                {
                    // check if it is a double or a string of a decimal number

                    object value = typedValue.Value;
                    if (value is double)
                    {
                        doubleValues.Add((double)value);

                    }
                    else
                    {
                        double doubleValue = Convert.ToDouble(value.ToString());
                        doubleValues.Add(doubleValue);
                    }

                }
                else
                {
                    // reject other typed values
                    throw new System.ArgumentException();
                }

            }
            return doubleValues;
        }
    }
}