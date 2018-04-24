using System;

using ESS.FW.Bpm.Engine.Dmn.engine;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Variable;

namespace ESS.FW.Bpm.Engine.Dmn.Impl.Result
{
    /// <summary>
    ///     Maps the decision result to pairs of output name and untyped entries.
    ///     
    /// </summary>
    public class SingleResultDecisionResultMapper : IDecisionResultMapper
    {
        protected internal static readonly DecisionLogger Log = ProcessEngineLogger.DecisionLogger;

        public virtual object MapDecisionResult(IDmnDecisionResult decisionResult)
        {
            try
            {
                var singleResult = decisionResult.SingleResult;
                if (singleResult != null)
                    return singleResult.EntryMap;
                return Variables.UntypedNullValue();
            }
            catch (DmnEngineException e)
            {
                throw Log.DecisionResultMappingException(decisionResult, this, e);
            }
            catch(System.Exception e)
            {
                throw e;
            }
        }

        public override string ToString()
        {
            return "SingleResultDecisionResultMapper{}";
        }
    }
}