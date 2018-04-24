using System.Linq;
using NUnit.Framework;

namespace Engine.Tests.Api.Mgmt.Metrics
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class ActivityInstanceCountMetricsTest : AbstractMetricsTest
    {
        [Test]
        public virtual void testStandaloneTask()
        {
            // given
            // that no activity instances have been executed
            Assert.AreEqual(0l, managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .Sum());

            // if
            // I complete a standalone task
            var task = taskService.NewTask();
            taskService.SaveTask(task);

            // then
            // the increased Count is immediately visible
            Assert.AreEqual(1l, managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .Sum());

            // and force the db metrics reporter to report
            processEngineConfiguration.DbMetricsReporter.ReportNow();

            // still 1
            Assert.AreEqual(1l, managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .Sum());

            taskService.DeleteTask(task.Id);

            // clean up
            var hti = historyService.CreateHistoricTaskInstanceQuery()
                .First();
            if (hti != null)
                historyService.DeleteHistoricTaskInstance(hti.Id);
        }

        [Test][Deployment]
        public virtual void testCmmnActivitiyInstances()
        {
            // given
            // that no activity instances have been executed
            Assert.AreEqual(0l, managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .Sum());

            caseService.CreateCaseInstanceByKey("case");

            Assert.AreEqual(1l, managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .Sum());

            // start PI_HumanTask_1 and PI_Milestone_1
            var list = caseService.CreateCaseExecutionQuery()
                //.Enabled()
                
                .ToList();
            foreach (var caseExecution in list)
                caseService.WithCaseExecution(caseExecution.Id)
                    .ManualStart();

            Assert.AreEqual(2l, managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .Sum());

            // and force the db metrics reporter to report
            processEngineConfiguration.DbMetricsReporter.ReportNow();

            // still 2
            Assert.AreEqual(2l, managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .Sum());

            // trigger the milestone
            var taskExecution = caseService.CreateCaseExecutionQuery(c=> c.ActivityId =="PI_HumanTask_1")
                .First();
            caseService.CompleteCaseExecution(taskExecution.Id);

            // milestone is counted
            Assert.AreEqual(3l, managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .Sum());
        }

        [Test]
        public virtual void testBpmnActivityInstances()
        {
            Deployment(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("testProcess")
                .StartEvent()
                .ManualTask()
                .EndEvent()
                .Done());

            // given
            // that no activity instances have been executed
            Assert.AreEqual(0l, managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .Sum());

            // if
            // a process instance is started
            runtimeService.StartProcessInstanceByKey("testProcess");

            // then
            // the increased Count is immediately visible
            Assert.AreEqual(3l, managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .Sum());

            // and force the db metrics reporter to report
            processEngineConfiguration.DbMetricsReporter.ReportNow();

            // still 3
            Assert.AreEqual(3l, managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .Sum());
        }
    }
}