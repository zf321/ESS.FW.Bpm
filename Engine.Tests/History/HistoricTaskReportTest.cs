using System;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.builder;
using NUnit.Framework;

namespace Engine.Tests.History
{
    /// <summary>
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class HistoricTaskReportTest
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

        public HistoricTaskReportTest()
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
        public virtual void testHistoricTaskInstanceReportQuery()
        {
            // given
            startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);
            startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);
            startAndCompleteProcessInstance(ANOTHER_PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);
            startAndCompleteProcessInstance(ANOTHER_PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);

            processEngineTestRule.Deploy(createProcessWithUserTask(PROCESS_DEFINITION_KEY));
            startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);

            // when
            var historicTaskInstanceReportResults = historyService.CreateHistoricTaskInstanceReport()
                .CountByTaskName();

            // then
            Assert.AreEqual(2, historicTaskInstanceReportResults.Count);
            Assert.AreEqual(2, historicTaskInstanceReportResults[0].Count, 0);
            Assert.AreEqual(ANOTHER_PROCESS_DEFINITION_KEY, historicTaskInstanceReportResults[0].ProcessDefinitionKey);
            Assert.AreEqual("name_" + ANOTHER_PROCESS_DEFINITION_KEY,
                historicTaskInstanceReportResults[0].ProcessDefinitionName);
            Assert.AreEqual(ANOTHER_PROCESS_DEFINITION_KEY + " ITask 1", historicTaskInstanceReportResults[0].TaskName);

            Assert.True(historicTaskInstanceReportResults[1].ProcessDefinitionId.Contains(":2:"));
        }

        [Test]
        public virtual void testHistoricTaskInstanceReportGroupedByProcessDefinitionKey()
        {
            // given
            startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);
            startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);
            startAndCompleteProcessInstance(ANOTHER_PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);
            startAndCompleteProcessInstance(ANOTHER_PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);

            processEngineTestRule.Deploy(createProcessWithUserTask(PROCESS_DEFINITION_KEY));
            startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);

            // when
            var historicTaskInstanceReportResults = historyService.CreateHistoricTaskInstanceReport()
                .CountByProcessDefinitionKey();

            // then
            Assert.AreEqual(2, historicTaskInstanceReportResults.Count);
            Assert.True(historicTaskInstanceReportResults[0].ProcessDefinitionId.Contains(":1:"));
            Assert.AreEqual("name_" + ANOTHER_PROCESS_DEFINITION_KEY,
                historicTaskInstanceReportResults[0].ProcessDefinitionName);

            Assert.AreEqual(ANOTHER_PROCESS_DEFINITION_KEY, historicTaskInstanceReportResults[0].ProcessDefinitionKey);
        }

        [Test]
        public virtual void testHistoricTaskInstanceReportWithCompletedAfterDate()
        {
            // given
            startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);
            startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 8, 14, 12, 1);
            startAndCompleteProcessInstance(ANOTHER_PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);

            // when
            var calendar = new DateTime();
            calendar = new DateTime(2016, 11, 14, 12, 5, 0);

            var historicTaskInstanceReportResults = historyService.CreateHistoricTaskInstanceReport()
                .CompletedAfter(calendar)
                .CountByProcessDefinitionKey();

            // then
            Assert.AreEqual(1, historicTaskInstanceReportResults.Count);
            Assert.AreEqual(1, historicTaskInstanceReportResults[0].Count, 0);
        }

        [Test]
        public virtual void testHistoricTaskInstanceReportWithCompletedBeforeDate()
        {
            // given
            startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);
            startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 8, 14, 12, 1);
            startAndCompleteProcessInstance(ANOTHER_PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);

            // when
            var calendar = new DateTime();
            calendar = new DateTime(2016, 11, 14, 12, 5, 0);

            var historicTaskInstanceReportResults = historyService.CreateHistoricTaskInstanceReport()
                .CompletedBefore(calendar)
                .CountByProcessDefinitionKey();

            // then
            Assert.AreEqual(2, historicTaskInstanceReportResults.Count);
            Assert.AreEqual(1, historicTaskInstanceReportResults[0].Count, 0);
        }

        [Test]
        public virtual void testCompletedAfterWithNullValue()
        {
            try
            {
                historyService.CreateHistoricTaskInstanceReport()
                    .CompletedAfter(DateTime.MaxValue)
                    .CountByProcessDefinitionKey();

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
                    .CountByProcessDefinitionKey();

                Assert.Fail("Expected NotValidException");
            }
            catch (NotValidException nve)
            {
                Assert.True(nve.Message.Contains("completedBefore"));
            }
        }

        [Test]
        public virtual void testReportWithNullTaskName()
        {
            // given
            startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);

            // when
            var instance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(ANOTHER_PROCESS_DEFINITION_KEY);
            instance.Name("name_" + ANOTHER_PROCESS_DEFINITION_KEY);
           var ins=  instance.StartEvent()
            .UserTask("task1_" + ANOTHER_PROCESS_DEFINITION_KEY)
            .EndEvent()
            .Done();

            processEngineTestRule.Deploy(ins);
            startAndCompleteProcessInstance(ANOTHER_PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);

            var calendar = new DateTime();
            calendar = new DateTime(2016, 11, 14, 12, 5, 0);

            var historicTaskInstanceReportResults = historyService.CreateHistoricTaskInstanceReport()
                .CompletedBefore(calendar)
                .CountByTaskName();

            Assert.AreEqual(1, historicTaskInstanceReportResults.Count);
            Assert.AreEqual(1, historicTaskInstanceReportResults[0].Count, 0);
        }

        [Test]
        public virtual void testReportWithEmptyTaskName()
        {
            // given
            startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);

            // when
            var instance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(ANOTHER_PROCESS_DEFINITION_KEY);
            instance.Name("name_" + ANOTHER_PROCESS_DEFINITION_KEY);
            var r= instance.StartEvent()
            .UserTask("task1_" + ANOTHER_PROCESS_DEFINITION_KEY)
            .Name<UserTaskBuilder>("")
               .EndEvent()
                .Done();

            processEngineTestRule.Deploy(r);
            startAndCompleteProcessInstance(ANOTHER_PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);

            var calendar = new DateTime();
            calendar = new DateTime(2016, 11, 14, 12, 5, 0);

            var historicTaskInstanceReportResults = historyService.CreateHistoricTaskInstanceReport()
                .CompletedBefore(calendar)
                .CountByTaskName();

            Assert.AreEqual(1, historicTaskInstanceReportResults.Count);
            Assert.AreEqual(1, historicTaskInstanceReportResults[0].Count, 0);
        }

        protected internal virtual IBpmnModelInstance createProcessWithUserTask(string key)
        {
            var random = new Random().NextDouble();
            var model = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(key);
            model.Name("name_" + key);
           var r= model.StartEvent()
            .UserTask(key + "_" + random + "_task1");
            model.Name(key + " ITask 1");
              var re=  r.EndEvent()
                .Done();
            return re;
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
        //calendar = new DateTime(ClockUtil.CurrentTime);
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