using System.Collections.Generic;
using System.Text;
using ESS.FW.Bpm.Engine.Migration;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instance
{
    public class MigratingProcessInstanceValidationReportImpl : IMigratingProcessInstanceValidationReport
    {
        protected internal IList<IMigratingActivityInstanceValidationReport> activityInstanceReports =
            new List<IMigratingActivityInstanceValidationReport>();

        protected internal IList<string> failures = new List<string>();

        protected internal string processInstanceId;

        protected internal IList<IMigratingTransitionInstanceValidationReport> transitionInstanceReports =
            new List<IMigratingTransitionInstanceValidationReport>();

        public virtual string ProcessInstanceId
        {
            get { return processInstanceId; }
            set { processInstanceId = value; }
        }

        public virtual IList<IMigratingActivityInstanceValidationReport> ActivityInstanceReports
        {
            get { return activityInstanceReports; }
        }

        public virtual IList<IMigratingTransitionInstanceValidationReport> TransitionInstanceReports
        {
            get { return transitionInstanceReports; }
        }

        public virtual IList<string> Failures
        {
            get { return failures; }
        }

        public virtual bool HasFailures()
        {
            return (failures.Count > 0) || (activityInstanceReports.Count > 0) || (transitionInstanceReports.Count > 0);
        }


        public virtual void AddActivityInstanceReport(IMigratingActivityInstanceValidationReport instanceReport)
        {
            activityInstanceReports.Add(instanceReport);
        }

        public virtual void AddTransitionInstanceReport(IMigratingTransitionInstanceValidationReport instanceReport)
        {
            transitionInstanceReports.Add(instanceReport);
        }

        public virtual void AddFailure(string failure)
        {
            failures.Add(failure);
        }

        public virtual void WriteTo(StringBuilder sb)
        {
            sb.Append("Cannot migrate process instance '").Append(processInstanceId).Append("':\n");

            foreach (var failure in failures)
                sb.Append("\t").Append(failure).Append("\n");

            foreach (var report in activityInstanceReports)
            {
                sb.Append("\tCannot migrate activity instance '").Append(report.ActivityInstanceId).Append("':\n");

                foreach (var failure in report.Failures)
                    sb.Append("\t\t").Append(failure).Append("\n");
            }

            foreach (var report in transitionInstanceReports)
            {
                sb.Append("\tCannot migrate transition instance '").Append(report.TransitionInstanceId).Append("':\n");

                foreach (var failure in report.Failures)
                    sb.Append("\t\t").Append(failure).Append("\n");
            }
        }
    }
}