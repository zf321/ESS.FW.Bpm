using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.History
{
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryAudit)]
    public class HistoricActivityStatisticsAuthorizationTest : AuthorizationTest
    {

        protected internal const string PROCESS_KEY = "oneTaskProcess";

        protected internal string deploymentId;

        [SetUp]
        public void setUp()
        {
            deploymentId = createDeployment(null, "resources/api/oneTaskProcess.bpmn20.xml").Id;
            base.setUp();
        }

        public void tearDown()
        {
            base.TearDown();
            DeleteDeployment(deploymentId);
        }

        // historic activity statistics query //////////////////////////////////

        public virtual void testQueryWithoutAuthorization()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);

            // when
            //IQueryable<IHistoricActivityStatistics> query = historyService.CreateHistoricActivityStatisticsQuery(  processDefinitionId);

            //// then
            ////verifyQueryResults(query, 0);
        }

        public virtual void testQueryWithReadHistoryPermissionOnProcessDefinition()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricActivityStatistics> query = historyService.CreateHistoricActivityStatisticsQuery();

            // then
            //verifyQueryResults(query, 1);
            verifyStatisticsResult(query.First(), 3, 0, 0, 0);
        }

        public virtual void testQueryWithReadHistoryPermissionOnAnyProcessDefinition()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricActivityStatistics> query = historyService.CreateHistoricActivityStatisticsQuery(processDefinitionId);

            // then
            //verifyQueryResults(query, 1);
            verifyStatisticsResult(query.First(), 3, 0, 0, 0);
        }

        public virtual void testQueryMultiple()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricActivityStatistics> query = historyService.CreateHistoricActivityStatisticsQuery(processDefinitionId);

            // then
            //verifyQueryResults(query, 1);
            verifyStatisticsResult(query.First(), 3, 0, 0, 0);
        }

        // historic activity statistics query (including finished) //////////////////////////////////

        public virtual void testQueryIncludingFinishedWithoutAuthorization()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);

            string taskId = selectAnyTask().Id;
            disableAuthorization();
            taskService.Complete(taskId);
            enableAuthorization();

            // when
            IQueryable<IHistoricActivityStatistics> query = historyService.CreateHistoricActivityStatisticsQuery(processDefinitionId);///*.IncludeFinished()*/;

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testQueryIncludingFinishedWithReadHistoryPermissionOnProcessDefinition()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);

            string taskId = selectAnyTask().Id;
            disableAuthorization();
            taskService.Complete(taskId);
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricActivityStatistics> query = historyService.CreateHistoricActivityStatisticsQuery(processDefinitionId);///*.IncludeFinished()*/;

            // then
            //verifyQueryResults(query, 3);
            IList<IHistoricActivityStatistics> statistics = query.ToList();

            IHistoricActivityStatistics start = getStatisticsByKey(statistics, "theStart");
            verifyStatisticsResult(start, 0, 3, 0, 0);

            IHistoricActivityStatistics task = getStatisticsByKey(statistics, "theTask");
            verifyStatisticsResult(task, 2, 1, 0, 0);

            IHistoricActivityStatistics end = getStatisticsByKey(statistics, "theEnd");
            verifyStatisticsResult(end, 0, 1, 0, 0);
        }

        public virtual void testQueryIncludingFinishedWithReadHistoryPermissionOnAnyProcessDefinition()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);

            string taskId = selectAnyTask().Id;
            disableAuthorization();
            taskService.Complete(taskId);
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricActivityStatistics> query = historyService.CreateHistoricActivityStatisticsQuery(processDefinitionId);///*.IncludeFinished()*/;

            // then
            //verifyQueryResults(query, 3);
            IList<IHistoricActivityStatistics> statistics = query.ToList();

            IHistoricActivityStatistics start = getStatisticsByKey(statistics, "theStart");
            verifyStatisticsResult(start, 0, 3, 0, 0);

            IHistoricActivityStatistics task = getStatisticsByKey(statistics, "theTask");
            verifyStatisticsResult(task, 2, 1, 0, 0);

            IHistoricActivityStatistics end = getStatisticsByKey(statistics, "theEnd");
            verifyStatisticsResult(end, 0, 1, 0, 0);
        }

        // historic activity statistics query (including canceled) //////////////////////////////////

        public virtual void testQueryIncludingCanceledWithoutAuthorization()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;

            disableAuthorization();
            runtimeService.DeleteProcessInstance(ProcessInstanceId, null);
            enableAuthorization();

            // when
            IQueryable<IHistoricActivityStatistics> query = historyService.CreateHistoricActivityStatisticsQuery(processDefinitionId);///*.IncludeFinished()*/;

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testQueryIncludingCanceledWithReadHistoryPermissionOnProcessDefinition()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;

            disableAuthorization();
            runtimeService.DeleteProcessInstance(ProcessInstanceId, null);
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricActivityStatistics> query = historyService.CreateHistoricActivityStatisticsQuery(processDefinitionId);///*.IncludeFinished()*/;

            // then
            //verifyQueryResults(query, 1);
            IList<IHistoricActivityStatistics> statistics = query.ToList();

            IHistoricActivityStatistics task = getStatisticsByKey(statistics, "theTask");
            verifyStatisticsResult(task, 2, 0, 1, 0);
        }

        public virtual void testQueryIncludingCanceledWithReadHistoryPermissionOnAnyProcessDefinition()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;

            disableAuthorization();
            runtimeService.DeleteProcessInstance(ProcessInstanceId, null);
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricActivityStatistics> query = historyService.CreateHistoricActivityStatisticsQuery(processDefinitionId);///*.IncludeFinished()*/;

            // then
            //verifyQueryResults(query, 1);
            IList<IHistoricActivityStatistics> statistics = query.ToList();

            IHistoricActivityStatistics task = getStatisticsByKey(statistics, "theTask");
            verifyStatisticsResult(task, 2, 0, 1, 0);
        }

        // historic activity statistics query (including complete scope) //////////////////////////////////

        public virtual void testQueryIncludingCompleteScopeWithoutAuthorization()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);

            string taskId = selectAnyTask().Id;
            disableAuthorization();
            taskService.Complete(taskId);
            enableAuthorization();

            // when
            IQueryable<IHistoricActivityStatistics> query = historyService.CreateHistoricActivityStatisticsQuery(processDefinitionId);///*.IncludeCompleteScope()*/;

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testQueryIncludingCompleteScopeWithReadHistoryPermissionOnProcessDefinition()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);

            string taskId = selectAnyTask().Id;
            disableAuthorization();
            taskService.Complete(taskId);
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricActivityStatistics> query = historyService.CreateHistoricActivityStatisticsQuery(processDefinitionId);///*.IncludeCompleteScope()*/;

            // then
            //verifyQueryResults(query, 2);
            IList<IHistoricActivityStatistics> statistics = query.ToList();

            IHistoricActivityStatistics task = getStatisticsByKey(statistics, "theTask");
            verifyStatisticsResult(task, 2, 0, 0, 0);

            IHistoricActivityStatistics end = getStatisticsByKey(statistics, "theEnd");
            verifyStatisticsResult(end, 0, 0, 0, 1);
        }

        public virtual void testQueryIncludingCompleteScopeWithReadHistoryPermissionOnAnyProcessDefinition()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);

            string taskId = selectAnyTask().Id;
            disableAuthorization();
            taskService.Complete(taskId);
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricActivityStatistics> query = historyService.CreateHistoricActivityStatisticsQuery(processDefinitionId);///*.IncludeCompleteScope()*/;

            // then
            //verifyQueryResults(query, 2);
            IList<IHistoricActivityStatistics> statistics = query.ToList();

            IHistoricActivityStatistics task = getStatisticsByKey(statistics, "theTask");
            verifyStatisticsResult(task, 2, 0, 0, 0);

            IHistoricActivityStatistics end = getStatisticsByKey(statistics, "theEnd");
            verifyStatisticsResult(end, 0, 0, 0, 1);
        }

        // historic activity statistics query (including all) //////////////////////////////////

        public virtual void testQueryIncludingAllWithoutAuthorization()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;

            disableAuthorization();
            runtimeService.DeleteProcessInstance(ProcessInstanceId, null);
            enableAuthorization();

            string taskId = selectAnyTask().Id;
            disableAuthorization();
            taskService.Complete(taskId);
            enableAuthorization();

            // when
            IQueryable<IHistoricActivityStatistics> query = historyService.CreateHistoricActivityStatisticsQuery(processDefinitionId);///*.IncludeFinished()*//*.IncludeCanceled()*//*.IncludeCompleteScope()*/;

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testQueryIncludingAllWithReadHistoryPermissionOnProcessDefinition()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;

            disableAuthorization();
            runtimeService.DeleteProcessInstance(ProcessInstanceId, null);
            enableAuthorization();

            string taskId = selectAnyTask().Id;
            disableAuthorization();
            taskService.Complete(taskId);
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricActivityStatistics> query = historyService.CreateHistoricActivityStatisticsQuery(processDefinitionId);///*.IncludeFinished()*//*.IncludeCanceled()*//*.IncludeCompleteScope()*/;

            // then
            //verifyQueryResults(query, 3);
            IList<IHistoricActivityStatistics> statistics = query.ToList();

            IHistoricActivityStatistics start = getStatisticsByKey(statistics, "theStart");
            //verifyStatisticsResult(start, 0, 3, 0, 0);

            IHistoricActivityStatistics task = getStatisticsByKey(statistics, "theTask");
            //verifyStatisticsResult(task, 1, 2, 1, 0);

            IHistoricActivityStatistics end = getStatisticsByKey(statistics, "theEnd");
            //verifyStatisticsResult(end, 0, 1, 0, 1);
        }

        public virtual void testQueryIncludingAllWithReadHistoryPermissionOnAnyProcessDefinition()
        {
            // given
            string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;

            disableAuthorization();
            runtimeService.DeleteProcessInstance(ProcessInstanceId, null);
            enableAuthorization();

            string taskId = selectAnyTask().Id;
            disableAuthorization();
            taskService.Complete(taskId);
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricActivityStatistics> query = historyService.CreateHistoricActivityStatisticsQuery(processDefinitionId);///*.IncludeFinished()*//*.IncludeCanceled()*//*.IncludeCompleteScope()*/;

            // then
            //verifyQueryResults(query, 3);
            IList<IHistoricActivityStatistics> statistics = query.ToList();

            IHistoricActivityStatistics start = getStatisticsByKey(statistics, "theStart");
            verifyStatisticsResult(start, 0, 3, 0, 0);

            IHistoricActivityStatistics task = getStatisticsByKey(statistics, "theTask");
            verifyStatisticsResult(task, 1, 2, 1, 0);

            IHistoricActivityStatistics end = getStatisticsByKey(statistics, "theEnd");
            verifyStatisticsResult(end, 0, 1, 0, 1);
        }

        // helper ////////////////////////////////////////////////////////

        protected internal virtual void verifyQueryResults(IQueryable<IHistoricActivityStatistics> query, int countExpected)
        {
            //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
            //ORIGINAL LINE: //verifyQueryResults((org.camunda.bpm.Engine.impl.AbstractQuery<?, ?>) query, countExpected);
            ////verifyQueryResults((AbstractQuery<object, object>)query, countExpected);
        }

        protected internal virtual void verifyStatisticsResult(IHistoricActivityStatistics statistics, int instances, int finished, int canceled, int completeScope)
        {
            Assert.AreEqual(instances, statistics.Instances, "Instances");
            Assert.AreEqual(finished, statistics.Finished, "Finished");
            Assert.AreEqual(canceled, statistics.Canceled, "Canceled");
            Assert.AreEqual(completeScope, statistics.CompleteScope, "Complete Scope");
        }

        protected internal virtual IHistoricActivityStatistics getStatisticsByKey(IList<IHistoricActivityStatistics> statistics, string key)
        {
            foreach (IHistoricActivityStatistics result in statistics)
            {
                if (key.Equals(result.Id))
                {
                    return result;
                }
            }
            Assert.Fail("No statistics found for key '" + key + "'.");
            return null;
        }

        protected internal virtual ITask selectAnyTask()
        {
            disableAuthorization();
            ITask task = taskService.CreateTaskQuery().First();
            enableAuthorization();
            return task;
        }

    }

}