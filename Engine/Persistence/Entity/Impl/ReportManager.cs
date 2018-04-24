using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Common.Components;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{
    /// <summary>
	/// 
	/// 
	/// </summary>
    [Component]
    public class ReportManager : IReportManager /*: AbstractManager*/
    {
        protected ITenantManager tenantManager;
        public ReportManager(ITenantManager _tenantManager)
        {
            tenantManager = _tenantManager;
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.history.DurationReportResult> selectHistoricProcessInstanceDurationReport(org.camunda.bpm.engine.impl.HistoricProcessInstanceReportImpl query)
        public virtual IList<IDurationReportResult> SelectHistoricProcessInstanceDurationReport(HistoricProcessInstanceReportImpl query)
        {
            ConfigureQuery(query);
            throw new System.NotImplementedException("selectHistoricProcessInstanceDurationReport");
            //return ListExt.ConvertToListT<IDurationReportResult>(DbEntityManager.SelectListWithRawParameter("selectHistoricProcessInstanceDurationReport", query, 0, int.MaxValue));
        }

        protected internal virtual void ConfigureQuery(HistoricProcessInstanceReportImpl parameter)
        {
            tenantManager.ConfigureTenantCheck(parameter.TenantCheck);
        }

    }

}