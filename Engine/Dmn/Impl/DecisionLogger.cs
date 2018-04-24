using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Dmn.engine;
using ESS.FW.Bpm.Engine.Dmn.Impl.Result;
using ESS.FW.Bpm.Engine.Impl;

namespace ESS.FW.Bpm.Engine.Dmn.Impl
{
    /// <summary>
    ///     
    /// </summary>
    public class DecisionLogger : ProcessEngineLogger
    {
        public virtual ProcessEngineException DecisionResultMappingException(IDmnDecisionResult decisionResult,
            IDecisionResultMapper resultMapper, DmnEngineException cause)
        {
            return
                new ProcessEngineException(
                    ExceptionMessage("001", "The decision result mapper '{0}' failed to process '{1}'", resultMapper,
                        decisionResult), cause);
        }

        public virtual ProcessEngineException DecisionResultCollectMappingException(ICollection<string> outputNames,
            IDmnDecisionResult decisionResult, IDecisionResultMapper resultMapper)
        {
            return
                new ProcessEngineException(ExceptionMessage("002",
                    "The decision result mapper '{0}' failed to process '{1}'. The decision outputs should only contains values for one output name but found '{2}'.",
                    resultMapper, decisionResult, outputNames));
        }

        public virtual BadUserRequestException ExceptionEvaluateDecisionDefinitionByIdAndTenantId()
        {
            return
                new BadUserRequestException(ExceptionMessage("003",
                    "Cannot specify a tenant-id when evaluate a decision definition by decision definition id."));
        }

        public virtual ProcessEngineException ExceptionParseDmnResource(string resouceName, System.Exception cause)
        {
            return
                new ProcessEngineException(
                    ExceptionMessage("004", "Unable to transform DMN resource '{0}'.", resouceName), cause);
        }

        public virtual ProcessEngineException ExceptionNoDrdForResource(string resourceName)
        {
            return
                new ProcessEngineException(ExceptionMessage("005",
                    "Found no decision requirements definition for DMN resource '{0}'.", resourceName));
        }
    }
}