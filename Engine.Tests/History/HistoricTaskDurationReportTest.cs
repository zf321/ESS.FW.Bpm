using System;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Query;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.builder;
using NUnit.Framework;

namespace Engine.Tests.History
{
    /// <summary>
    ///     
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class HistoricTaskDurationReportTest
    {
        [SetUp]
        public virtual void setUp()
        {
            historyService = processEngineRule.HistoryService;
            processEngineConfiguration = processEngineRule.ProcessEngineConfiguration;

            processEngineTestRule.Deploy(createProcessWithUserTask(PROCESS_DEFINITION_KEY));
            processEngineTestRule.Deploy(createProcessWithUserTask(ANOTHER_PROCESS_DEFINITION_KEY));
        }

        [TearDown]
        public virtual void cleanUp()
        {
            var list = processEngineRule.TaskService.CreateTaskQuery()

                .ToList();
            foreach (var task in list)
                processEngineRule.TaskService.DeleteTask(task.Id, true);
        }

        private readonly bool InstanceFieldsInitialized;

        public HistoricTaskDurationReportTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            processEngineTestRule = new ProcessEngineTestRule(processEngineRule);
            //ruleChain = RuleChain.outerRule(processEngineTestRule).around(processEngineRule);
        }


        public ProcessEngineRule processEngineRule = new ProvidedProcessEngineRule();
        public ProcessEngineTestRule processEngineTestRule;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(processEngineTestRule).around(processEngineRule);
        //public RuleChain ruleChain;

        protected internal ProcessEngineConfiguration processEngineConfiguration;
        protected internal IHistoryService historyService;

        protected internal const string PROCESS_DEFINITION_KEY = "HISTORIC_TASK_INST_REPORT";
        protected internal const string ANOTHER_PROCESS_DEFINITION_KEY = "ANOTHER_HISTORIC_TASK_INST_REPORT";

        [Test]
        public virtual void testHistoricTaskInstanceDurationReportQuery()
        {
            // given
            startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 6, 14, 11, 43);
            startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 7, 14, 11, 43);
            startAndCompleteProcessInstance(ANOTHER_PROCESS_DEFINITION_KEY, 2016, 8, 14, 11, 43);

            // when
            var taskReportResults = historyService.CreateHistoricTaskInstanceReport()
                .Duration(PeriodUnit.Month);

            // then
            Assert.AreEqual(3, taskReportResults.Count);
        }

        [Test]
        public virtual void testHistoricTaskInstanceDurationReportWithCompletedAfterDate()
        {
            // given
            startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 7, 14, 11, 43);
            startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 8, 14, 11, 43);
            startAndCompleteProcessInstance(ANOTHER_PROCESS_DEFINITION_KEY, 2016, 7, 14, 11, 43);

            // when
            var calendar = new DateTime();
            calendar = new DateTime(2016, 11, 14, 12, 5, 0);

            var taskReportResults = historyService.CreateHistoricTaskInstanceReport()
                .CompletedAfter(calendar)
                .Duration(PeriodUnit.Month);

            // then
            Assert.AreEqual(1, taskReportResults.Count);
        }

        [Test]
        public virtual void testHistoricTaskInstanceDurationReportWithCompletedBeforeDate()
        {
            // given
            startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 7, 14, 11, 43);
            startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 8, 14, 11, 43);
            startAndCompleteProcessInstance(ANOTHER_PROCESS_DEFINITION_KEY, 2016, 6, 14, 11, 43);

            // when
            var calendar = new DateTime();
            calendar = new DateTime(2016, 11, 14, 12, 5, 0);

            var taskReportResults = historyService.CreateHistoricTaskInstanceReport()
                .CompletedBefore(calendar)
                .Duration(PeriodUnit.Month);

            // then
            Assert.AreEqual(2, taskReportResults.Count);
        }
        [Test]
        public virtual void testHistoricTaskInstanceDurationReportResults()
        {
            startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 7, 14, 11, 43);
            startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 7, 14, 11, 43);

            var taskReportResult = historyService.CreateHistoricTaskInstanceReport()
                .Duration(PeriodUnit.Month)[0];

            var historicTaskInstances = historyService.CreateHistoricTaskInstanceQuery(c => c.ProcessDefinitionKey == PROCESS_DEFINITION_KEY)

                .ToList();

            long min = 0;
            long max = 0;
            long sum = 0;

            for (var i = 0; i < historicTaskInstances.Count; i++)
            {
                var historicProcessInstance = historicTaskInstances[i];
                var duration = historicProcessInstance.DurationInMillis ?? 0L;
                sum = sum + duration;
                max = i > 0 ? Math.Max(max, duration) : duration;
                min = i > 0 ? Math.Min(min, duration) : duration;
            }

            var avg = sum / historicTaskInstances.Count;

            Assert.AreEqual(max, taskReportResult.Maximum, "maximum");
            Assert.AreEqual(min, taskReportResult.Minimum, "minimum");
            Assert.AreEqual(avg, taskReportResult.Average, 0, "average");
        }

        [Test]
        public virtual void testCompletedAfterWithNullValue()
        {
            try
            {
                historyService.CreateHistoricTaskInstanceReport()
                    .CompletedAfter(DateTime.Now)
                    .Duration(PeriodUnit.Month);

                Assert.Fail("Expected NotValidException");
            }
            catch (NotValidException nve)
            {
                Assert.True(nve.Message.Contains("completedAfter"));
            }
        }

        [Test]
        public virtual void testCompletedBeforeWithNullValue()
        {
            try
            {
                historyService.CreateHistoricTaskInstanceReport()
                    .CompletedBefore(DateTime.MaxValue)
                    .Duration(PeriodUnit.Month);

                Assert.Fail("Expected NotValidException");
            }
            catch (NotValidException nve)
            {
                Assert.True(nve.Message.Contains("completedBefore"));
            }
        }

        protected internal virtual IBpmnModelInstance createProcessWithUserTask(string key)
        {
            return
            ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(key)
                .StartEvent()
                .UserTask(key + "_task1")
            .Name<UserTaskBuilder>(key + " Task 1")
            .EndEvent()
                .Done();

        }

        protected internal virtual void completeTask(string pid)
        {
            var task = processEngineRule.TaskService.CreateTaskQuery(c => c.ProcessInstanceId == pid)
                .First();
            processEngineRule.TaskService.Complete(task.Id);
        }

        protected internal virtual void setCurrentTime(int year, int month, int dayOfMonth, int hourOfDay, int minute)
        {
            var calendar = new DateTime();
            // Calendars month start with 0 = January
            calendar = new DateTime(year, month - 1, dayOfMonth, hourOfDay, minute, 0);
            ClockUtil.CurrentTime = calendar;
        }

        // protected internal virtual void addToCalendar(int field, int month)
        // {
        //DateTime calendar = new DateTime();
        //calendar = new DateTime(ClockUtil.CurrentTime.Ticks);
        //calendar.Add(field, month);
        //ClockUtil.CurrentTime = calendar;
        // }

        protected internal virtual void startAndCompleteProcessInstance(string key, int year, int month, int dayOfMonth,
            int hourOfDay, int minute)
        {
            setCurrentTime(year, month, dayOfMonth, hourOfDay, minute);

            var pi = processEngineRule.RuntimeService.StartProcessInstanceByKey(key);
            ClockUtil.CurrentTime = ClockUtil.CurrentTime.AddMonths(5);

            completeTask(pi.Id);

            ClockUtil.Reset();
        }
    }
}