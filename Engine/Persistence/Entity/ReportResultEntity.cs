using System;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Query;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    public abstract class ReportResultEntity : IReportResult
    {
        protected internal PeriodUnit periodUnit;

        public virtual int Period { get; set; }

        public PeriodUnit PeriodUnit
        {
            get { return periodUnit; }
        }

        public virtual PeriodUnit GetPeriodUnit()
        {
            return periodUnit;
        }

        public virtual void SetPeriodUnit(string periodUnit)
        {
            PeriodUnit.TryParse(periodUnit, out this.periodUnit);
        }

    }

}