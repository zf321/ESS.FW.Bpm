

namespace ESS.FW.Bpm.Engine.History
{
    /// <summary>
    ///     
    /// </summary>
    public interface IHistoricDecisionInstanceStatistics
    {
        /// <returns> count of decision definition evaluations </returns>
        int Evaluations { get; }

        /// <returns> key of decision definition </returns>
        string DecisionDefinitionKey { get; }
    }
}

