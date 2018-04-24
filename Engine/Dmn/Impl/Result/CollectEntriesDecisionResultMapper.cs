using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Dmn.engine;
using ESS.FW.Bpm.Engine.Impl;
using System.Linq;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Dmn.Impl.Result
{
    /// <summary>
    ///     Maps the decision result to a list of untyped entries.
    ///     
    /// </summary>
    public class CollectEntriesDecisionResultMapper : IDecisionResultMapper
    {
        protected internal static readonly DecisionLogger Log = ProcessEngineLogger.DecisionLogger;

        public virtual object MapDecisionResult(IDmnDecisionResult decisionResult)
        {
            if ((decisionResult == null) || (decisionResult.Count == 0))
                return null;
            var outputNames = CollectOutputNames(decisionResult);
            if (outputNames.Count > 1)
                throw Log.DecisionResultCollectMappingException(outputNames, decisionResult, this);
            var outputName = outputNames.FirstOrDefault();
            //return decisionResult.CollectEntries<IDmnDecisionResultEntries>(outputName);
            return decisionResult.CollectEntries<ITypedValue>(outputName);
        }

        protected internal virtual ICollection<string> CollectOutputNames(IDmnDecisionResult decisionResult)
        {
            var outputNames = new List<string>();

            foreach (var entryMap in decisionResult.ResultList)
            {
                foreach (var item in entryMap.Keys)
                {
                    if (!outputNames.Contains(item))
                    {
                        outputNames.Add(item);
                    }
                }
            }


            return outputNames;
        }

        public override string ToString()
        {
            return "CollectEntriesDecisionResultMapper{}";
        }
    }
}