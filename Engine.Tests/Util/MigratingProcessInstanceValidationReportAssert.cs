using System.Collections.Generic;
using System.Text;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Util
{
    

    public class MigratingProcessInstanceValidationReportAssert
    {

        protected internal IMigratingProcessInstanceValidationReport Actual;

        public MigratingProcessInstanceValidationReportAssert(IMigratingProcessInstanceValidationReport report)
        {
            this.Actual = report;
        }

        public virtual MigratingProcessInstanceValidationReportAssert NotNull
        {
            get
            {
                Assert.NotNull(Actual,"Expected report to be not null" );

                return this;
            }
        }

        public virtual MigratingProcessInstanceValidationReportAssert HasProcessInstance(IProcessInstance processInstance)
        {
            return HasProcessInstanceId(processInstance.Id);
        }

        public virtual MigratingProcessInstanceValidationReportAssert HasProcessInstanceId(string processInstanceId)
        {
            var migratingProcessInstanceValidationReportAssert = NotNull;

            Assert.AreEqual("Expected report to be for process instance", processInstanceId, Actual.ProcessInstanceId);

            return this;
        }

        public virtual MigratingProcessInstanceValidationReportAssert HasFailures(params string[] expectedFailures)
        {
            var migratingProcessInstanceValidationReportAssert = NotNull;

            IList<string> actualFailures = Actual.Failures;

            //JAVA TO C# CONVERTER TODO Resources.Task: There is no .NET equivalent to the Java 'super' constraint:
            //ORIGINAL LINE: java.util.Collection<org.hamcrest.Matcher<? super String>> matchers = new java.util.ArrayList<org.hamcrest.Matcher<? super String>>();
            //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
            //ICollection<Matcher> matchers = new List<Matcher>();
            //foreach (string expectedFailure in expectedFailures)
            //{
            //    matchers.Add(Matchers.Does.Contain(expectedFailure));
            //}

            //Assert.That("Expected failures:\n" + joinFailures((expectedFailures)) + "But found failures:\n" + joinFailures(actualFailures), actualFailures, Matchers.containsInAnyOrder(matchers));

            return this;
        }

        public virtual MigratingProcessInstanceValidationReportAssert HasActivityInstanceFailures(string sourceScopeId, params string[] expectedFailures)
        {
            var migratingProcessInstanceValidationReportAssert = NotNull;

            IMigratingActivityInstanceValidationReport actualReport = null;
            foreach (IMigratingActivityInstanceValidationReport instanceReport in Actual.ActivityInstanceReports)
            {
                if (sourceScopeId.Equals(instanceReport.SourceScopeId))
                {
                    actualReport = instanceReport;
                    break;
                }
            }

            Assert.NotNull( actualReport, "No validation report found for source scope: " + sourceScopeId);

            AssertFailures(sourceScopeId, (expectedFailures), actualReport.Failures);

            return this;
        }

        public virtual MigratingProcessInstanceValidationReportAssert HasTransitionInstanceFailures(string sourceScopeId, params string[] expectedFailures)
        {
            var migratingProcessInstanceValidationReportAssert = NotNull;

            IMigratingTransitionInstanceValidationReport actualReport = null;
            foreach (IMigratingTransitionInstanceValidationReport instanceReport in Actual.TransitionInstanceReports)
            {
                if (sourceScopeId.Equals(instanceReport.SourceScopeId))
                {
                    actualReport = instanceReport;
                    break;
                }
            }

            Assert.NotNull( actualReport, "No validation report found for source scope: " + sourceScopeId);

            AssertFailures(sourceScopeId, (expectedFailures), actualReport.Failures);

            return this;
        }

        protected internal virtual void AssertFailures(string sourceScopeId, IList<string> expectedFailures, IList<string> actualFailures)
        {

            //JAVA TO C# CONVERTER TODO Resources.Task: There is no .NET equivalent to the Java 'super' constraint:
            //ORIGINAL LINE: java.util.Collection<org.hamcrest.Matcher<? super String>> matchers = new java.util.ArrayList<org.hamcrest.Matcher<? super String>>();
            //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
            //ICollection<Matcher> matchers = new List<Matcher>();
            //foreach (string expectedFailure in expectedFailures)
            //{
            //    matchers.Add(Matchers.Does.Contain(expectedFailure));
            //}

            //Assert.That("Expected failures for source scope: " + sourceScopeId + "\n" + joinFailures(expectedFailures) + "But found failures:\n" + joinFailures(actualFailures), actualFailures, Matchers.containsInAnyOrder(matchers));
        }

        public static MigratingProcessInstanceValidationReportAssert That(IMigratingProcessInstanceValidationReport report)
        {
            return new MigratingProcessInstanceValidationReportAssert(report);
        }

        public virtual string JoinFailures(IList<string> failures)
        {
            StringBuilder builder = new StringBuilder();
            foreach (object failure in failures)
            {
                builder.Append("\t\t").Append(failure).Append("\n");
            }

            return builder.ToString();
        }



    }
}
