using System;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{

    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class DeleteHistoricCaseInstanceCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;

        protected internal string CaseInstanceId;

        public DeleteHistoricCaseInstanceCmd(string caseInstanceId)
        {
            this.CaseInstanceId = caseInstanceId;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("caseInstanceId", CaseInstanceId);
            // Check if case instance is still running
            IHistoricCaseInstance instance =
                commandContext.HistoricCaseInstanceManager.FindHistoricCaseInstance(CaseInstanceId);

            EnsureUtil.EnsureNotNull("No historic case instance found with id: " + CaseInstanceId, "instance", instance);

            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckDeleteHistoricCaseInstance(instance);
            }

            EnsureUtil.EnsureNotNull(
                "Case instance is still running, cannot delete historic case instance: " + CaseInstanceId,
                "instance.getCloseTime()", instance.CloseTime);

            commandContext.HistoricCaseInstanceManager.DeleteHistoricCaseInstanceById(CaseInstanceId);

            return null;
        }
    }
}