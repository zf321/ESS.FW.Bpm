using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Query;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.History
{
    /// <summary>
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryActivity)]
    [TestFixture]
    public class HistoricProcessInstanceDurationReportTest : PluggableProcessEngineTestCase
    {
        public virtual void TestReportByInvalidPeriodUnit()
        {
            var report = historyService.CreateHistoricProcessInstanceReport();

            try
            {
                report.Duration(PeriodUnit.Month);
                Assert.Fail();
            }
            catch (NotValidException)
            {
            }
        }

        protected internal virtual IBpmnModelInstance CreateProcessWithUserTask(string key)
        {
            return ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(key)
                .StartEvent()
                .UserTask()
                .EndEvent()
                .Done();
        }

        protected internal class DurationReportScenarioBuilder
        {
            private readonly HistoricProcessInstanceDurationReportTest _outerInstance;

            protected internal DurationReportResultAssertion Assertion;


//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
            protected internal PeriodUnit PeriodUnitRenamed = ESS.FW.Bpm.Engine.Query.PeriodUnit.Month;

            public DurationReportScenarioBuilder(HistoricProcessInstanceDurationReportTest outerInstance)
            {
                _outerInstance = outerInstance;
                Assertion = new DurationReportResultAssertion(_outerInstance);
            }

            public virtual DurationReportScenarioBuilder PeriodUnit(PeriodUnit periodUnit)
            {
                PeriodUnitRenamed = periodUnit;
                Assertion.PeriodUnit = periodUnit;
                return this;
            }

            protected internal virtual void SetCurrentTime(int year, int month, int dayOfMonth, int hourOfDay,
                int minute)
            {
                var calendar = new DateTime();
                calendar = new DateTime(year, month, dayOfMonth, hourOfDay, minute, 0);
                ClockUtil.CurrentTime = calendar;
            }

            //protected internal virtual void AddToCalendar(int field, int month)
            //{
            //  DateTime calendar = new DateTime();
            //  calendar = new DateTime(ClockUtil.CurrentTime);
            //  calendar.Add(field, month);
            //  ClockUtil.CurrentTime = calendar;
            //}

            public virtual DurationReportScenarioBuilder StartAndCompleteProcessInstance(string key, int year, int month,
                int dayOfMonth, int hourOfDay, int minute)
            {
                SetCurrentTime(year, month, dayOfMonth, hourOfDay, minute);

                var pi = _outerInstance.runtimeService.StartProcessInstanceByKey(key);

                var period = month;
                // if (PeriodUnitRenamed == PeriodUnit.Quarter)
                // {
                //period = month / 3;
                // }
                Assertion.AddDurationReportResult(period + 1, pi.Id);
                ClockUtil.CurrentTime.AddMonths(5);
                //AddToCalendar(DateTime.Query.PeriodUnit.Month, 5);
                var task = _outerInstance.taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi.Id)
                    .First();
                _outerInstance.taskService.Complete(task.Id);

                return this;
            }

            public virtual DurationReportResultAssertion Done()
            {
                return Assertion;
            }
        }

        protected internal class DurationReportResultAssertion
        {
            private readonly HistoricProcessInstanceDurationReportTest _outerInstance;

            protected internal IDictionary<int?, ISet<string>> PeriodToProcessInstancesMap =
                new Dictionary<int?, ISet<string>>();


            protected internal PeriodUnit PeriodUnit = PeriodUnit.Month;

            public DurationReportResultAssertion(HistoricProcessInstanceDurationReportTest outerInstance)
            {
                _outerInstance = outerInstance;
            }

            public virtual DurationReportResultAssertion AddDurationReportResult(int period, string processInstanceId)
            {
                var processInstances = PeriodToProcessInstancesMap[period];
                if (processInstances == null)
                {
                    processInstances = new HashSet<string>();
                    PeriodToProcessInstancesMap[period] = processInstances;
                }
                processInstances.Add(processInstanceId);
                return this;
            }

            public virtual DurationReportResultAssertion SetPeriodUnit(PeriodUnit periodUnit)
            {
                PeriodUnit = periodUnit;
                return this;
            }

            public virtual void AssertReportResults(IList<IDurationReportResult> actual)
            {
                Assert.AreEqual(PeriodToProcessInstancesMap.Count, actual.Count, "Report Count");

                foreach (var reportResult in actual)
                {
                    Assert.AreEqual(PeriodUnit, reportResult.PeriodUnit, "Period unit");

                    var period = reportResult.Period;
                    var processInstancesInPeriod = PeriodToProcessInstancesMap[period];
                    Assert.NotNull(processInstancesInPeriod, "Unexpected report for period " + period);

                    var historicProcessInstances = _outerInstance.historyService.CreateHistoricProcessInstanceQuery()
                      //  .ProcessInstanceIds(processInstancesInPeriod)
                      //  .Finished()
                        
                        .ToList();

                    long max = 0;
                    long min = 0;
                    long sum = 0;

                    for (var i = 0; i < historicProcessInstances.Count(); i++)
                    {
                        var historicProcessInstance = historicProcessInstances[i];
                        var duration = historicProcessInstance.DurationInMillis ?? 0;
                        sum = sum + duration;
                        max = i > 0 ? Math.Max(max, duration) : duration;
                        min = i > 0 ? Math.Min(min, duration) : duration;
                    }

                    var avg = sum / historicProcessInstances.Count();

                    Assert.AreEqual(max, reportResult.Maximum, "maximum");
                    Assert.AreEqual(min, reportResult.Minimum, "minimum");
                    Assert.AreEqual(avg, reportResult.Average, 1, "average");
                }
            }
        }

        protected internal class DurationReportResultAssert
        {
            private readonly HistoricProcessInstanceDurationReportTest _outerInstance;


            protected internal IList<IDurationReportResult> Actual;

            public DurationReportResultAssert(HistoricProcessInstanceDurationReportTest outerInstance,
                IList<IDurationReportResult> actual)
            {
                _outerInstance = outerInstance;
                Actual = actual;
            }

            public virtual void Matches(DurationReportResultAssertion assertion)
            {
                assertion.AssertReportResults(Actual);
            }
        }

        protected internal virtual DurationReportScenarioBuilder CreateReportScenario()
        {
            return new DurationReportScenarioBuilder(this);
        }

        protected internal virtual DurationReportResultAssert That(IList<IDurationReportResult> actual)
        {
            return new DurationReportResultAssert(this, actual);
        }

        [Test]
        public virtual void TestDurationReportByMonth()
        {
            // given
            Deployment(CreateProcessWithUserTask("process"));

            var assertion = CreateReportScenario()
                .PeriodUnit(PeriodUnit.Month)
                .StartAndCompleteProcessInstance("process", 2016, 0, 1, 10, 0)
                .Done(); // 01.01.2016 10:00
            // period: 01 (January)

            // when
            var result = historyService.CreateHistoricProcessInstanceReport()
                .Duration(PeriodUnit.Month);

            // then
            That(result)
                .Matches(assertion);
        }

        [Test]
        public virtual void TestDurationReportByQuarter()
        {
            // given
            Deployment(CreateProcessWithUserTask("process"));

            var assertion = CreateReportScenario()
                .PeriodUnit(PeriodUnit.Quarter)
                .StartAndCompleteProcessInstance("process", 2016, 3, 1, 10, 0)
                .Done(); // 01.04.2016 10:00
            // period: 2. quarter

            // when
            var result = historyService.CreateHistoricProcessInstanceReport()
                .Duration(PeriodUnit.Quarter);

            // then
            That(result)
                .Matches(assertion);
        }

        [Test]
        public virtual void TestDurationReportInDifferentPeriodsByMonth()
        {
            // given
            Deployment(CreateProcessWithUserTask("process"));

            var assertion = CreateReportScenario()
                    .PeriodUnit(PeriodUnit.Month)
                    .StartAndCompleteProcessInstance("process", 2015, 10, 1, 10, 0)
                    .StartAndCompleteProcessInstance("process", 2015, 11, 1, 10, 0)
                    .StartAndCompleteProcessInstance("process", 2016, 0, 1, 10, 0)
                    .StartAndCompleteProcessInstance("process", 2016, 1, 1, 10, 0)
                    .StartAndCompleteProcessInstance("process", 2016, 2, 1, 10, 0)
                    .StartAndCompleteProcessInstance("process", 2016, 3, 1, 10, 0)
                    .StartAndCompleteProcessInstance("process", 2016, 4, 1, 10, 0)
                    .StartAndCompleteProcessInstance("process", 2016, 5, 1, 10, 0)
                    .StartAndCompleteProcessInstance("process", 2016, 6, 1, 10, 0)
                    .StartAndCompleteProcessInstance("process", 2016, 7, 1, 10, 0)
                    .StartAndCompleteProcessInstance("process", 2016, 8, 1, 10, 0)
                    .StartAndCompleteProcessInstance("process", 2016, 9, 1, 10, 0)
                    .Done();
                // 01.10.2016 10:00 -  01.09.2016 10:00 -  01.08.2016 10:00 -  01.07.2016 10:00 -  01.06.2016 10:00 -  01.05.2016 10:00 -  01.04.2016 10:00 -  01.03.2016 10:00 -  01.02.2016 10:00 -  01.01.2016 10:00 -  01.12.2015 10:00 -  01.11.2015 10:00
            // period: 11 (November)
            // period: 12 (December)
            // period: 01 (January)
            // period: 02 (February)
            // period: 03 (March)
            // period: 04 (April)
            // period: 05 (May)
            // period: 06 (June)
            // period: 07 (July)
            // period: 08 (August)
            // period: 09 (September)
            // period: 10 (October)

            // when
            var result = historyService.CreateHistoricProcessInstanceReport()
                .Duration(PeriodUnit.Month);

            // then
            That(result)
                .Matches(assertion);
        }

        [Test]
        public virtual void TestDurationReportInDifferentPeriodsByQuarter()
        {
            // given
            Deployment(CreateProcessWithUserTask("process"));

            var assertion = CreateReportScenario()
                    .PeriodUnit(PeriodUnit.Quarter)
                    .StartAndCompleteProcessInstance("process", 2015, 10, 1, 10, 0)
                    .StartAndCompleteProcessInstance("process", 2015, 11, 1, 10, 0)
                    .StartAndCompleteProcessInstance("process", 2016, 1, 1, 10, 0)
                    .StartAndCompleteProcessInstance("process", 2015, 2, 1, 10, 0)
                    .StartAndCompleteProcessInstance("process", 2015, 3, 1, 10, 0)
                    .StartAndCompleteProcessInstance("process", 2015, 5, 1, 10, 0)
                    .StartAndCompleteProcessInstance("process", 2015, 6, 1, 10, 0)
                    .StartAndCompleteProcessInstance("process", 2015, 7, 1, 10, 0)
                    .Done();
                // 01.08.2016 10:00 -  01.07.2016 10:00 -  01.06.2016 10:00 -  01.04.2016 10:00 -  01.03.2016 10:00 -  01.02.2016 10:00 -  01.12.2015 10:00 -  01.11.2015 10:00
            // period: 4. quarter (2015)
            // period: 1. quarter (2016)
            // period: 2. quarter (2016)
            // period: 3. quarter (2016)

            // when
            var result = historyService.CreateHistoricProcessInstanceReport()
                .Duration(PeriodUnit.Quarter);

            // then
            That(result)
                .Matches(assertion);
        }

        [Test]
        public virtual void TestReportByInvalidProcessDefinitionId()
        {
            var report = historyService.CreateHistoricProcessInstanceReport();

            try
            {
                report.ProcessDefinitionIdIn((string) null);
            }
            catch (NotValidException)
            {
            }

            try
            {
                report.ProcessDefinitionIdIn("abc", (string) null, "def");
            }
            catch (NotValidException)
            {
            }
        }

        [Test]
        public virtual void TestReportByInvalidProcessDefinitionKey()
        {
            var report = historyService.CreateHistoricProcessInstanceReport();

            try
            {
                report.ProcessDefinitionKeyIn((string) null);
            }
            catch (NotValidException)
            {
            }

            try
            {
                report.ProcessDefinitionKeyIn("abc", (string) null, "def");
            }
            catch (NotValidException)
            {
            }
        }

        [Test]
        public virtual void TestReportByInvalidStartedAfter()
        {
            var report = historyService.CreateHistoricProcessInstanceReport();

            try
            {
                report.StartedAfter(DateTime.MaxValue);
                Assert.Fail();
            }
            catch (NotValidException)
            {
            }
        }

        [Test]
        public virtual void TestReportByInvalidStartedBefore()
        {
            var report = historyService.CreateHistoricProcessInstanceReport();

            try
            {
                report.StartedBefore(DateTime.Now);
                Assert.Fail();
            }
            catch (NotValidException)
            {
            }
        }

        [Test]
        public virtual void TestReportByMultipleProcessDefinitionIdByMonth()
        {
            // given
            Deployment(CreateProcessWithUserTask("process1"), CreateProcessWithUserTask("process2"));

            var processDefinitionId1 = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="process1")
                .First()
                .Id;

            var processDefinitionId2 = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="process2")
                .First()
                .Id;

            var assertion = CreateReportScenario()
                .PeriodUnit(PeriodUnit.Month)
                .StartAndCompleteProcessInstance("process1", 2016, 1, 15, 10, 0)
                .StartAndCompleteProcessInstance("process2", 2016, 3, 15, 10, 0)
                .Done(); // 15.04.2016 10:00 -  15.02.2016 10:00

            // when
            var result = historyService.CreateHistoricProcessInstanceReport()
                .ProcessDefinitionIdIn(processDefinitionId1, processDefinitionId2)
                .Duration(PeriodUnit.Month);

            // then
            That(result)
                .Matches(assertion);
        }

        [Test]
        public virtual void TestReportByMultipleProcessDefinitionIdByQuarter()
        {
            // given
            Deployment(CreateProcessWithUserTask("process1"), CreateProcessWithUserTask("process2"));

            var processDefinitionId1 = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="process1")
                .First()
                .Id;

            var processDefinitionId2 = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="process2")
                .First()
                .Id;

            var assertion = CreateReportScenario()
                .PeriodUnit(PeriodUnit.Quarter)
                .StartAndCompleteProcessInstance("process1", 2016, 1, 15, 10, 0)
                .StartAndCompleteProcessInstance("process2", 2016, 3, 15, 10, 0)
                .Done(); // 15.04.2016 10:00 -  15.02.2016 10:00

            // when
            var result = historyService.CreateHistoricProcessInstanceReport()
                .ProcessDefinitionIdIn(processDefinitionId1, processDefinitionId2)
                .Duration(PeriodUnit.Quarter);

            // then
            That(result)
                .Matches(assertion);
        }

        [Test]
        public virtual void TestReportByMultipleProcessDefinitionKeyByMonth()
        {
            // given
            Deployment(CreateProcessWithUserTask("process1"), CreateProcessWithUserTask("process2"));

            var assertion = CreateReportScenario()
                .PeriodUnit(PeriodUnit.Month)
                .StartAndCompleteProcessInstance("process1", 2016, 1, 15, 10, 0)
                .StartAndCompleteProcessInstance("process2", 2016, 3, 15, 10, 0)
                .Done(); // 15.04.2016 10:00 -  15.02.2016 10:00

            // when
            var result = historyService.CreateHistoricProcessInstanceReport()
                .ProcessDefinitionKeyIn("process1", "process2")
                .Duration(PeriodUnit.Month);

            // then
            That(result)
                .Matches(assertion);
        }

        [Test]
        public virtual void TestReportByMultipleProcessDefinitionKeyByQuarter()
        {
            // given
            Deployment(CreateProcessWithUserTask("process1"), CreateProcessWithUserTask("process2"));

            var assertion = CreateReportScenario()
                .PeriodUnit(PeriodUnit.Quarter)
                .StartAndCompleteProcessInstance("process1", 2016, 1, 15, 10, 0)
                .StartAndCompleteProcessInstance("process2", 2016, 3, 15, 10, 0)
                .Done(); // 15.04.2016 10:00 -  15.02.2016 10:00

            // when
            var result = historyService.CreateHistoricProcessInstanceReport()
                .ProcessDefinitionKeyIn("process1", "process2")
                .Duration(PeriodUnit.Quarter);

            // then
            That(result)
                .Matches(assertion);
        }

        [Test]
        public virtual void TestReportByProcessDefinitionIdByMonth()
        {
            // given
            Deployment(CreateProcessWithUserTask("process1"), CreateProcessWithUserTask("process2"));

            var processDefinitionId1 = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="process1")
                .First()
                .Id;

            var assertion = CreateReportScenario()
                .PeriodUnit(PeriodUnit.Month)
                .StartAndCompleteProcessInstance("process1", 2016, 1, 15, 10, 0)
                .Done(); // 15.02.2016 10:00

            CreateReportScenario()
                .StartAndCompleteProcessInstance("process2", 2016, 3, 15, 10, 0)
                .Done(); // 15.04.2016 10:00

            // when
            var result = historyService.CreateHistoricProcessInstanceReport()
                .ProcessDefinitionIdIn(processDefinitionId1)
                .Duration(PeriodUnit.Month);

            // then
            That(result)
                .Matches(assertion);
        }

        [Test]
        public virtual void TestReportByProcessDefinitionIdByQuarter()
        {
            // given
            Deployment(CreateProcessWithUserTask("process1"), CreateProcessWithUserTask("process2"));

            var processDefinitionId1 = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="process1")
                .First()
                .Id;

            var assertion = CreateReportScenario()
                .PeriodUnit(PeriodUnit.Quarter)
                .StartAndCompleteProcessInstance("process1", 2016, 1, 15, 10, 0)
                .Done(); // 15.02.2016 10:00

            CreateReportScenario()
                .StartAndCompleteProcessInstance("process2", 2016, 3, 15, 10, 0)
                .Done(); // 15.04.2016 10:00

            // when
            var result = historyService.CreateHistoricProcessInstanceReport()
                .ProcessDefinitionIdIn(processDefinitionId1)
                .Duration(PeriodUnit.Quarter);

            // then
            That(result)
                .Matches(assertion);
        }

        [Test]
        public virtual void TestReportByProcessDefinitionKeyByMonth()
        {
            // given
            Deployment(CreateProcessWithUserTask("process1"), CreateProcessWithUserTask("process2"));

            var assertion = CreateReportScenario()
                .PeriodUnit(PeriodUnit.Month)
                .StartAndCompleteProcessInstance("process1", 2016, 1, 15, 10, 0)
                .Done(); // 15.02.2016 10:00

            CreateReportScenario()
                .StartAndCompleteProcessInstance("process2", 2016, 3, 15, 10, 0)
                .Done(); // 15.04.2016 10:00

            // when
            var result = historyService.CreateHistoricProcessInstanceReport()
                .ProcessDefinitionKeyIn("process1")
                .Duration(PeriodUnit.Month);

            // then
            That(result)
                .Matches(assertion);
        }

        [Test]
        public virtual void TestReportByProcessDefinitionKeyByQuarter()
        {
            // given
            Deployment(CreateProcessWithUserTask("process1"), CreateProcessWithUserTask("process2"));

            var assertion = CreateReportScenario()
                .PeriodUnit(PeriodUnit.Quarter)
                .StartAndCompleteProcessInstance("process1", 2016, 1, 15, 10, 0)
                .Done(); // 15.02.2016 10:00

            CreateReportScenario()
                .StartAndCompleteProcessInstance("process2", 2016, 3, 15, 10, 0)
                .Done(); // 15.04.2016 10:00

            // when
            var result = historyService.CreateHistoricProcessInstanceReport()
                .ProcessDefinitionKeyIn("process1")
                .Duration(PeriodUnit.Quarter);

            // then
            That(result)
                .Matches(assertion);
        }

        [Test]
        public virtual void TestReportByStartedAfterAndStartedBeforeByMonth()
        {
            // given
            Deployment(CreateProcessWithUserTask("process"));

            var assertion = CreateReportScenario()
                .PeriodUnit(PeriodUnit.Month)
                .StartAndCompleteProcessInstance("process", 2016, 1, 15, 10, 0)
                .StartAndCompleteProcessInstance("process", 2016, 2, 1, 10, 0)
                .Done(); // 01.03.2016 10:00 -  15.02.2016 10:00

            CreateReportScenario()
                .StartAndCompleteProcessInstance("process", 2016, 3, 15, 10, 0)
                .Done(); // 15.04.2016 10:00

            var calendar = new DateTime();
            calendar = new DateTime(2016, 0, 1, 0, 0, 0);
            var after = calendar;
            calendar = new DateTime(2016, 2, 31, 23, 59, 59);
            var before = calendar;

            // when
            var result = historyService.CreateHistoricProcessInstanceReport()
                .StartedAfter(after)
                .StartedBefore(before)
                .Duration(PeriodUnit.Month);

            // then
            That(result)
                .Matches(assertion);
        }

        [Test]
        public virtual void TestReportByStartedAfterAndStartedBeforeByQuarter()
        {
            // given
            Deployment(CreateProcessWithUserTask("process"));

            var assertion = CreateReportScenario()
                .PeriodUnit(PeriodUnit.Quarter)
                .StartAndCompleteProcessInstance("process", 2016, 1, 15, 10, 0)
                .StartAndCompleteProcessInstance("process", 2016, 2, 1, 10, 0)
                .Done(); // 01.03.2016 10:00 -  15.02.2016 10:00

            CreateReportScenario()
                .StartAndCompleteProcessInstance("process", 2016, 3, 15, 10, 0)
                .Done(); // 15.04.2016 10:00

            var calendar = new DateTime();
            calendar = new DateTime(2016, 0, 1, 0, 0, 0);
            var after = calendar;
            calendar = new DateTime(2016, 2, 31, 23, 59, 59);
            var before = calendar;

            // when
            var result = historyService.CreateHistoricProcessInstanceReport()
                .StartedAfter(after)
                .StartedBefore(before)
                .Duration(PeriodUnit.Quarter);

            // then
            That(result)
                .Matches(assertion);
        }

        [Test]
        public virtual void TestReportByStartedAfterByMonth()
        {
            // given
            Deployment(CreateProcessWithUserTask("process"));

            CreateReportScenario()
                .StartAndCompleteProcessInstance("process", 2015, 11, 15, 10, 0)
                .Done(); // 15.12.2015 10:00

            var assertion = CreateReportScenario()
                .PeriodUnit(PeriodUnit.Month)
                .StartAndCompleteProcessInstance("process", 2016, 3, 1, 10, 0)
                .Done(); // 01.04.2016 10:00

            var calendar = new DateTime();
            calendar = new DateTime(2016, 0, 1, 0, 0, 0);

            // when
            var result = historyService.CreateHistoricProcessInstanceReport()
                .StartedAfter(calendar)
                .Duration(PeriodUnit.Month);

            // then
            That(result)
                .Matches(assertion);
        }

        [Test]
        public virtual void TestReportByStartedAfterByQuarter()
        {
            // given
            Deployment(CreateProcessWithUserTask("process"));

            CreateReportScenario()
                .StartAndCompleteProcessInstance("process", 2015, 11, 15, 10, 0)
                .Done(); // 15.12.2015 10:00

            var assertion = CreateReportScenario()
                .PeriodUnit(PeriodUnit.Quarter)
                .StartAndCompleteProcessInstance("process", 2016, 3, 1, 10, 0)
                .Done(); // 01.04.2016 10:00

            var calendar = new DateTime();
            calendar = new DateTime(2016, 0, 1, 0, 0, 0);

            // when
            var result = historyService.CreateHistoricProcessInstanceReport()
                .StartedAfter(calendar)
                .Duration(PeriodUnit.Quarter);

            // then
            That(result)
                .Matches(assertion);
        }

        [Test]
        public virtual void TestReportByStartedBeforeByMonth()
        {
            // given
            Deployment(CreateProcessWithUserTask("process"));

            var assertion = CreateReportScenario()
                .PeriodUnit(PeriodUnit.Month)
                .StartAndCompleteProcessInstance("process", 2016, 0, 15, 10, 0)
                .Done(); // 15.01.2016 10:00

            // start a second process instance
            CreateReportScenario()
                .StartAndCompleteProcessInstance("process", 2016, 3, 1, 10, 0)
                .Done(); // 01.04.2016 10:00

            var calendar = new DateTime();
            calendar = new DateTime(2016, 0, 16, 0, 0, 0);

            // when
            var result = historyService.CreateHistoricProcessInstanceReport()
                .StartedBefore(calendar)
                .Duration(PeriodUnit.Month);

            // then
            That(result)
                .Matches(assertion);
        }

        [Test]
        public virtual void TestReportByStartedBeforeByQuarter()
        {
            // given
            Deployment(CreateProcessWithUserTask("process"));

            var assertion = CreateReportScenario()
                .PeriodUnit(PeriodUnit.Quarter)
                .StartAndCompleteProcessInstance("process", 2016, 0, 15, 10, 0)
                .Done(); // 15.01.2016 10:00

            // start a second process instance
            CreateReportScenario()
                .StartAndCompleteProcessInstance("process", 2016, 3, 1, 10, 0)
                .Done(); // 01.04.2016 10:00

            var calendar = new DateTime();
            calendar = new DateTime(2016, 0, 16, 0, 0, 0);

            // when
            var result = historyService.CreateHistoricProcessInstanceReport()
                .StartedBefore(calendar)
                .Duration(PeriodUnit.Quarter);

            // then
            That(result)
                .Matches(assertion);
        }

        [Test]
        public virtual void TestReportWithExcludingConditions()
        {
            // given
            Deployment(CreateProcessWithUserTask("process"));

            runtimeService.StartProcessInstanceByKey("process");
            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;
            taskService.Complete(taskId);

            var hourAgo = new DateTime();
            hourAgo.AddHours(-1);

            var hourFromNow = new DateTime();
            hourFromNow.AddHours(1);

            // when
            var result = historyService.CreateHistoricProcessInstanceReport()
                .StartedAfter(hourFromNow)
                .StartedBefore(hourAgo)
                .Duration(PeriodUnit.Month);

            // then
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public virtual void TestSamePeriodDifferentYearByMonth()
        {
            // given
            Deployment(CreateProcessWithUserTask("process"));

            var assertion = CreateReportScenario()
                .PeriodUnit(PeriodUnit.Month)
                .StartAndCompleteProcessInstance("process", 2015, 1, 1, 10, 0)
                .StartAndCompleteProcessInstance("process", 2016, 1, 1, 10, 0)
                .Done(); // 01.01.2016 10:00 -  01.01.2015 10:00
            // period: 01 (January)

            // when
            var result = historyService.CreateHistoricProcessInstanceReport()
                .Duration(PeriodUnit.Month);

            // then
            That(result)
                .Matches(assertion);
        }

        [Test]
        public virtual void TestSamePeriodDifferentYearByQuarter()
        {
            // given
            Deployment(CreateProcessWithUserTask("process"));

            var assertion = CreateReportScenario()
                .PeriodUnit(PeriodUnit.Quarter)
                .StartAndCompleteProcessInstance("process", 2015, 1, 1, 10, 0)
                .StartAndCompleteProcessInstance("process", 2016, 1, 1, 10, 0)
                .Done(); // 01.01.2016 10:00 -  01.01.2015 10:00
            // period: 1. quarter

            // when
            var result = historyService.CreateHistoricProcessInstanceReport()
                .Duration(PeriodUnit.Quarter);

            // then
            That(result)
                .Matches(assertion);
        }

        [Test]
        public virtual void TestTwoInstancesInSamePeriodByMonth()
        {
            // given
            Deployment(CreateProcessWithUserTask("process"));

            var assertion = CreateReportScenario()
                .PeriodUnit(PeriodUnit.Month)
                .StartAndCompleteProcessInstance("process", 2016, 0, 1, 10, 0)
                .StartAndCompleteProcessInstance("process", 2016, 0, 15, 10, 0)
                .Done(); // 15.01.2016 10:00 -  01.01.2016 10:00
            // period: 01 (January)

            // when
            var result = historyService.CreateHistoricProcessInstanceReport()
                .Duration(PeriodUnit.Month);

            // then
            That(result)
                .Matches(assertion);
        }

        [Test]
        public virtual void TestTwoInstancesInSamePeriodDurationReportByQuarter()
        {
            // given
            Deployment(CreateProcessWithUserTask("process"));

            var assertion = CreateReportScenario()
                .PeriodUnit(PeriodUnit.Quarter)
                .StartAndCompleteProcessInstance("process", 2016, 3, 1, 10, 0)
                .StartAndCompleteProcessInstance("process", 2016, 5, 1, 10, 0)
                .Done(); // 01.05.2016 10:00 -  01.04.2016 10:00
            // period: 2. quarter

            // when
            var result = historyService.CreateHistoricProcessInstanceReport()
                .Duration(PeriodUnit.Quarter);

            // then
            That(result)
                .Matches(assertion);
        }
    }
}