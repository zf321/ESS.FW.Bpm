using ESS.FW.Bpm.Engine.Dmn.engine;

namespace ESS.FW.Bpm.Engine.Dmn.Impl.Result
{
    /// <summary>
    ///     Maps the decision result to a list of pairs that contains output name and
    ///     untyped entry.
    ///     
    /// </summary>
    public class ResultListDecisionTableResultMapper : IDecisionResultMapper
    {
        public virtual object MapDecisionResult(IDmnDecisionResult decisionResult)
        {
            return decisionResult.ResultList;
        }
    }
}