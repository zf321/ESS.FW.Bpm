using System;
using ESS.FW.Bpm.Engine.Dmn.engine;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable;

namespace ESS.FW.Bpm.Engine.Dmn.Impl.Result
{
    /// <summary>
    ///     Maps the decision result to a single typed entry.
    ///     
    /// </summary>
    public class SingleEntryDecisionResultMapper : IDecisionResultMapper
    {
        protected internal static readonly DecisionLogger Log = ProcessEngineLogger.DecisionLogger;

        public virtual object MapDecisionResult(IDmnDecisionResult decisionResult)
        {
            try
            {
                ITypedValue typedValue = decisionResult.GetSingleEntryTyped<ITypedValue>();//.SingleEntryTyped;
                if (typedValue != null)
                {
                    return typedValue;
                }
                return Variables.UntypedNullValue();
            }
            catch (DmnEngineException e)
            {
                throw Log.DecisionResultMappingException(decisionResult, this, e);
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        public override string ToString()
        {
            return "SingleEntryDecisionResultMapper{}";
        }
    }
}