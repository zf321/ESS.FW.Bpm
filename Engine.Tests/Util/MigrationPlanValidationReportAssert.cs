using System.Collections.Generic;
using System.Text;
using ESS.FW.Bpm.Engine.Migration;
using NUnit.Framework;

namespace Engine.Tests.Util
{


    public class MigrationPlanValidationReportAssert
    {

        protected internal IMigrationPlanValidationReport Actual;

        public MigrationPlanValidationReportAssert(IMigrationPlanValidationReport report)
        {
            this.Actual = report;
        }

        public virtual MigrationPlanValidationReportAssert NotNull
        {
            get
            {
                Assert.NotNull(Actual, "Expected report to be not null");

                return this;
            }
        }

        public virtual MigrationPlanValidationReportAssert Failures(string activityId, params string[] Failures)
        {
            var migrationPlanValidationReportAssert = NotNull;

            IList<string> FailuresFound = new List<string>();

            foreach (IMigrationInstructionValidationReport instructionReport in Actual.InstructionReports)
            {
                string sourceActivityId = instructionReport.MigrationInstruction.SourceActivityId;
                if ((string.ReferenceEquals(activityId, null) && string.ReferenceEquals(sourceActivityId, null)) ||
                    (!string.ReferenceEquals(activityId, null) && activityId.Equals(sourceActivityId)))
                {
                    ((List<string>) FailuresFound).AddRange(instructionReport.Failures);
                }
            }

            //ICollection<Matcher> matchers = new List<Matcher>();
            //foreach (string expectedAssert.Failure in expectedAssert.Failures)
            //{
            //  matchers.Add(Matchers.Does.Contain(expectedAssert.Failure));
            //}

            //Assert.That("Expected Assert.Failures for activity id '" + activityId + "':\n" + JoinAssert.Failures(expectedAssert.Failures) + "But found Assert.Failures:\n" + JoinAssert.Failures(Assert.FailuresFound.ToArray()), Assert.FailuresFound, Matchers.containsInAnyOrder(matchers));

            return this;
        }

        public static MigrationPlanValidationReportAssert That(IMigrationPlanValidationReport report)
        {
            return new MigrationPlanValidationReportAssert(report);
        }

        public virtual string JoinFailures(object[] failures)
        {
            StringBuilder builder = new StringBuilder();
            foreach (object failure in failures)
            {
                builder.Append("\t\t")
                    .Append(failure)
                    .Append("\n");
            }

            return builder.ToString();
        }



        public virtual MigrationPlanValidationReportAssert HasInstructionFailures(string activityId,
            params string[] expectedFailures)
        {
            var migrationPlanValidationReportAssert = NotNull;

            IList<string> failuresFound = new List<string>();

            foreach (IMigrationInstructionValidationReport instructionReport in Actual.InstructionReports)
            {
                string sourceActivityId = instructionReport.MigrationInstruction.SourceActivityId;
                if ((string.ReferenceEquals(activityId, null) && string.ReferenceEquals(sourceActivityId, null)) ||
                    (!string.ReferenceEquals(activityId, null) && activityId.Equals(sourceActivityId)))
                {
                    ((List<string>) failuresFound).AddRange(instructionReport.Failures);
                }
            }

            //JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
            //ORIGINAL LINE: Collection<Matcher<? super String>> matchers = new ArrayList<Matcher<? super String>>();
            //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
            //ICollection<Matcher> matchers = new List<Matcher>();
            //foreach (string expectedFailure in expectedFailures)
            //{
            //    matchers.Add(Matchers.containsString(expectedFailure));
            //}

            //Assert.assertThat(
            //    "Expected failures for activity id '" + activityId + "':\n" + joinFailures(expectedFailures) +
            //    "But found failures:\n" + joinFailures(failuresFound.ToArray()), failuresFound,
            //    Matchers.containsInAnyOrder(matchers));

            return this;
        }

    }
}