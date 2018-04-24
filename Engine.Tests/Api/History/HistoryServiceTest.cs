using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.Api.History
{
    /// <summary>
    ///     
    /// </summary>
    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_AUDIT) public class HistoryServiceTest extends org.Camunda.bpm.Engine.impl.Test.PluggableProcessEngineTestCase
    [TestFixture]
    public class HistoryServiceTest : PluggableProcessEngineTestCase
    {
        public const string ONE_TASK_PROCESS = "oneTaskProcess";
        //protected internal static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testHistoricProcessInstanceQuery()
        {
            // With a clean IProcessEngine, no instances should be available
            Assert.True(historyService.CreateHistoricProcessInstanceQuery()
                            .Count() == 0);
            var processInstance = runtimeService.StartProcessInstanceByKey(ONE_TASK_PROCESS);
            Assert.True(historyService.CreateHistoricProcessInstanceQuery()
                            .Count() == 1);

            // Complete the task and check if the Count is Count 1
            var tasks = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id)

                .ToList();
            Assert.AreEqual(1, tasks.Count);
            taskService.Complete(tasks[0].Id);
            Assert.True(historyService.CreateHistoricProcessInstanceQuery()
                            .Count() == 1);
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testHistoricProcessInstanceQueryOrderBy()
        {
            // With a clean IProcessEngine, no instances should be available
            Assert.True(historyService.CreateHistoricProcessInstanceQuery()
                            .Count() == 0);
            var processInstance = runtimeService.StartProcessInstanceByKey(ONE_TASK_PROCESS);

            var tasks = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id)
                .ToList();
            Assert.AreEqual(1, tasks.Count);
            taskService.Complete(tasks[0].Id);
            //以下测试封装的查询方法 。
            historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByDeleteReason()
                /*.Asc()*/
                .ToList();
            historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByExecutionId()
                /*.Asc()*/
                .ToList();
            historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByHistoricActivityInstanceId()
                /*.Asc()*/

                .ToList();
            historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByHistoricActivityInstanceStartTime()
                /*.Asc()*/

                .ToList();
            historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByHistoricTaskInstanceDuration()
                /*.Asc()*/

                .ToList();
            historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByHistoricTaskInstanceEndTime()
                /*.Asc()*/

                .ToList();
            historyService.CreateHistoricTaskInstanceQuery()
                /*.OrderByProcessDefinitionId()*/
                /*.Asc()*/

                .ToList();
            historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByProcessInstanceId()
                /*.Asc()*/

                .ToList();
            historyService.CreateHistoricTaskInstanceQuery()
                /*.OrderByTaskAssignee()*/
                /*.Asc()*/

                .ToList();
            historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByTaskDefinitionKey()
                /*.Asc()*/

                .ToList();
            historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByTaskDescription()
                /*.Asc()*/

                .ToList();
            historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByTaskId()
                /*.Asc()*/

                .ToList();
            historyService.CreateHistoricTaskInstanceQuery()
                /*.OrderByTaskName()*/
                /*.Asc()*/

                .ToList();
            historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByTaskOwner()
                /*.Asc()*/

                .ToList();
            historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByTaskPriority()
                /*.Asc()*/

                .ToList();
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testHistoricProcessInstanceUserIdAndActivityId()
        {
            // deprecated method is tested here
            identityService.AuthenticatedUserId = "johndoe";
            var processInstance = runtimeService.StartProcessInstanceByKey(ONE_TASK_PROCESS);
            var historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery()
                .First();
            Assert.AreEqual("johndoe", historicProcessInstance.StartUserId);
            Assert.AreEqual("theStart", historicProcessInstance.StartActivityId);

            var tasks = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id)

                .ToList();
            Assert.AreEqual(1, tasks.Count);
            taskService.Complete(tasks[0].Id);

            historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery()
                .First();
            Assert.AreEqual("theEnd", historicProcessInstance.EndActivityId);
        }

        [Test]
        [Deployment(new[] { "resources/api/history/orderProcess.bpmn20.xml", "resources/api/history/checkCreditProcess.bpmn20.xml" })]
        public virtual void testOrderProcessWithCallActivity()
        {
            // After the process has started, the 'verify credit history' task should be
            // active
            var pi = runtimeService.StartProcessInstanceByKey("orderProcess");
            var taskQuery = taskService.CreateTaskQuery();
            var verifyCreditTask = taskQuery.First();

            // Completing the task with approval, will end the subprocess and continue
            // the original process
            taskService.Complete(verifyCreditTask.Id, CollectionUtil.SingletonMap("creditApproved", true));
            var prepareAndShipTask = taskQuery.First();
            Assert.AreEqual("Prepare and Ship", prepareAndShipTask.Name);

            // verify
            var historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery(m => m.SuperProcessInstanceId == pi.Id)
                //.SuperProcessInstanceId(pi.Id)
                .FirstOrDefault();
            Assert.NotNull(historicProcessInstance);
            Assert.True(historicProcessInstance.ProcessDefinitionId.Contains("checkCreditProcess"));
        }

        [Test]
        [Deployment(new[]{ "resources/api/oneTaskProcess.bpmn20.xml",
            "resources/api/history/orderProcess.bpmn20.xml",
            "resources/api/history/checkCreditProcess.bpmn20.xml"
        }
            )]
        public virtual void testHistoricProcessInstanceQueryByProcessDefinitionKey()
        {
            var processDefinitionKey = ONE_TASK_PROCESS;
            runtimeService.StartProcessInstanceByKey(processDefinitionKey);
            runtimeService.StartProcessInstanceByKey("orderProcess");
            var historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery(c => c.ProcessDefinitionKey == processDefinitionKey)
                .First();
            Assert.NotNull(historicProcessInstance);
            Assert.True(historicProcessInstance.ProcessDefinitionId.StartsWith(processDefinitionKey));
            Assert.AreEqual("theStart", historicProcessInstance.StartActivityId);

            // now complete the task to end the process instance
            //var task = taskService.CreateTaskQuery(/*c => c.ProcessDefinitionKey == "checkCreditProcess"*/)
            //    .First();

            //16:02:27.686 [main] DEBUG o.c.b.e.i.p.e.T.selectTaskByQueryCriteria - ==>  Preparing: select distinct RES.REV_, RES.ID_, RES.NAME_, RES.PARENT_TASK_ID_, RES.DESCRIPTION_, RES.PRIORITY_, RES.CREATE_TIME_, RES.OWNER_, RES.ASSIGNEE_, RES.DELEGATION_, RES.EXECUTION_ID_, RES.PROC_INST_ID_, RES.PROC_DEF_ID_, RES.CASE_EXECUTION_ID_, RES.CASE_INST_ID_, RES.CASE_DEF_ID_, RES.TASK_DEF_KEY_, RES.DUE_DATE_, RES.FOLLOW_UP_DATE_, RES.SUSPENSION_STATE_, RES.TENANT_ID_ from ACT_RU_TASK RES inner join ACT_RE_PROCDEF D on RES.PROC_DEF_ID_ = D.ID_ WHERE ( 1 = 1 and D.KEY_ = ? ) order by RES.ID_ asc LIMIT ? OFFSET ? 
            //16:02:27.691[main] DEBUG o.c.b.e.i.p.e.T.selectTaskByQueryCriteria - ==> Parameters: checkCreditProcess(String), 2147483647(Integer), 0(Integer)
            var db = runtimeService.GetDbContext();
            var query = from res in db.Set<TaskEntity>()
                        join d in db.Set<ProcessDefinitionEntity>() on res.ProcessDefinitionId equals d.Id
                        where d.Key == "checkCreditProcess"
                        orderby res.Id
                        select res;
            var task = query.FirstOrDefault();
            IDictionary<string, object> map = new Dictionary<string, object>();
            map["creditApproved"] = true;
            taskService.Complete(task.Id, map);

            // and make sure the super process instance is set correctly on the
            // IHistoricProcessInstance
            var historicProcessInstanceSub = historyService.CreateHistoricProcessInstanceQuery(c => c.ProcessDefinitionKey == "checkCreditProcess")
                .First();
            var historicProcessInstanceSuper = historyService.CreateHistoricProcessInstanceQuery(c => c.ProcessDefinitionKey == "orderProcess")
                .First();
            Assert.AreEqual(historicProcessInstanceSuper.Id, historicProcessInstanceSub.SuperProcessInstanceId);
        }

        [Test]
        [Deployment(new[] { "resources/api/oneTaskProcess.bpmn20.xml" ,
            "resources/api/runtime/otherOneTaskProcess.bpmn20.xml"
        })]
        public virtual void testHistoricProcessInstanceQueryByProcessInstanceIds()
        {
            var processInstanceIds = new List<string>();
            for (var i = 0; i < 4; i++)
                processInstanceIds.Add(runtimeService.StartProcessInstanceByKey(ONE_TASK_PROCESS, i + "")
                    .Id);
            processInstanceIds.Add(runtimeService.StartProcessInstanceByKey("otherOneTaskProcess", "1")
                .Id);

            // start an instance that will not be part of the query
            runtimeService.StartProcessInstanceByKey("otherOneTaskProcess", "2");

            var manager = runtimeService.GetManager<IHistoricProcessInstanceManager>();
            //var processInstanceQuery = historyService.CreateHistoricProcessInstanceQuery(m => processInstanceIds.Contains(m.ProcessInstanceId))
            //.ProcessInstanceIds(processInstanceIds)
            //;
            var processInstanceQuery = manager.ProcessInstanceIds(processInstanceIds);
            Assert.AreEqual(5, processInstanceQuery.Count());

            var processInstances = processInstanceQuery
                .ToList();
            Assert.NotNull(processInstances);
            Assert.AreEqual(5, processInstances.Count);

            foreach (var historicProcessInstance in processInstances)
                Assert.True(processInstanceIds.Contains(historicProcessInstance.Id));

            // making a query that has contradicting conditions should succeed
            Assert.AreEqual(0, processInstanceQuery
                .Count(c => c.ProcessInstanceId == "dummy"));

        }

        [Test]
        public virtual void testHistoricProcessInstanceQueryByProcessInstanceIdsEmpty()
        {
            try
            {
                var manager = runtimeService.GetManager<IHistoricProcessInstanceManager>();
                //historyService.CreateHistoricProcessInstanceQuery()
                //.ProcessInstanceIds(new HashSet<string>())
                //;
                manager.ProcessInstanceIds(new List<string>());
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException re)
            {
                AssertTextPresent("Set of process instance ids is empty", re.Message);
            }
        }

        [Test]
        public virtual void testHistoricProcessInstanceQueryByProcessInstanceIdsNull()
        {
            try
            {
                //historyService.CreateHistoricProcessInstanceQuery()
                //    //.ProcessInstanceIds(null)
                //    ;
                var manager = runtimeService.GetManager<IHistoricProcessInstanceManager>();
                manager.ProcessInstanceIds(null);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException re)
            {
                AssertTextPresent("Set of process instance ids is null", re.Message);
            }
        }

        [Test]
        [Deployment("resources/api/runtime/concurrentExecution.bpmn20.xml")]
        public virtual void testHistoricVariableInstancesOnParallelExecution()
        {
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["rootValue"] = "test";
            var pi = runtimeService.StartProcessInstanceByKey("concurrent", vars);

            var tasks = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id)
                .ToList();
            foreach (var task in tasks)
            {
                IDictionary<string, object> variables = new Dictionary<string, object>();
                // set token local variable
                Console.WriteLine("setting variables on task " + task.Id + ", execution " + task.ExecutionId);
                runtimeService.SetVariableLocal(task.ExecutionId, "parallelValue1", task.Name);
                runtimeService.SetVariableLocal(task.ExecutionId, "parallelValue2", "test");
                taskService.Complete(task.Id, variables);
            }
            taskService.Complete(taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id)
                .First()
                .Id);

            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery()
            //.VariableValueEquals("rootValue", "test")
            //    .Count());
            var hisProInsManager = runtimeService.GetManager<IHistoricProcessInstanceManager>();
            Assert.AreEqual(1, hisProInsManager.VariableValueEquals("rootValue", "test").Count());

            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery()
            //    //.VariableValueEquals("parallelValue1", "Receive Payment")
            //    .Count());
            Assert.AreEqual(1, hisProInsManager.VariableValueEquals("parallelValue1", "Receive Payment").Count());

            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery()
            //    //.VariableValueEquals("parallelValue1", "Ship Order")
            //    .Count());
            Assert.AreEqual(1, hisProInsManager.VariableValueEquals("parallelValue1", "Ship Order").Count());

            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery()
            //    //.VariableValueEquals("parallelValue2", "test")
            //    .Count());

            //SerializerName == "string" 反序列化时类型判断错误？
            Assert.AreEqual(1, hisProInsManager.VariableValueEquals("parallelValue2", "test").Count());
        }
        //16:28:38.240 [main] DEBUG o.c.b.e.i.p.e.H.selectHistoricProcessInstanceCountByQueryCriteria - ==>  Preparing: select count(distinct RES.ID_) from ( SELECT SELF.*, DEF.NAME_, DEF.VERSION_ FROM ACT_HI_PROCINST SELF LEFT JOIN ACT_RE_PROCDEF DEF ON SELF.PROC_DEF_ID_ = DEF.ID_ WHERE EXISTS ( select ID_ from ACT_HI_VARINST WHERE NAME_= ? AND PROC_INST_ID_ = SELF.PROC_INST_ID_ and ( ( VAR_TYPE_ is not null and VAR_TYPE_ = ? and TEXT_ is not null and TEXT_ = ? ) ) ) ) RES 
        //16:28:38.271 [main]
        // DEBUG o.c.b.e.i.p.e.H.selectHistoricProcessInstanceCountByQueryCriteria - ==> Parameters: rootValue(String), string(String), test(String)


        /// <summary>
        ///     basically copied from <seealso cref="ProcessInstanceQueryTest" />
        /// </summary>
        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testQueryStringVariable()
        {
            var manager = runtimeService.GetManager<IHistoricProcessInstanceManager>();
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["stringVar"] = "abcdef";
            var processInstance1 = runtimeService.StartProcessInstanceByKey(ONE_TASK_PROCESS, vars);
            taskService.Complete(taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance1.Id)
                .First()
                .Id);

            vars = new Dictionary<string, object>();
            vars["stringVar"] = "abcdef";
            vars["stringVar2"] = "ghijkl";
            var processInstance2 = runtimeService.StartProcessInstanceByKey(ONE_TASK_PROCESS, vars);
            taskService.Complete(taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance2.Id)
                .First()
                .Id);

            vars = new Dictionary<string, object>();
            vars["stringVar"] = "azerty";
            var processInstance3 = runtimeService.StartProcessInstanceByKey(ONE_TASK_PROCESS, vars);
            taskService.Complete(taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance3.Id)
                .First()
                .Id);

            // Test EQUAL on single string variable, should result in 2 matches
            //var query = historyService.CreateHistoricProcessInstanceQuery()
            //.VariableValueEquals("stringVar", "abcdef");
            var query = manager.VariableValueEquals("stringVar", "abcdef");
            var processInstances = query
                .ToList();
            Assert.NotNull(processInstances);
            Assert.AreEqual(2, processInstances.Count);

            // Test EQUAL on two string variables, should result in single match
            //query = historyService.CreateHistoricProcessInstanceQuery()
            //.VariableValueEquals("stringVar", "abcdef")
            //.VariableValueEquals("stringVar2", "ghijkl")
            //    ;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["stringVar"] = "abcdef";
            dic["stringVar2"] = "ghijkl";
            query = manager.VariableValueEquals("stringVar","abcdef");
            var query2= manager.VariableValueEquals("stringVar2", "ghijkl");
            var resultInstance = query.Where(m=>query2.Select(n=>n.Id).Contains(m.Id)).FirstOrDefault();
            Assert.NotNull(resultInstance);
            Assert.AreEqual(processInstance2.Id, resultInstance.Id);

            // Test NOT_EQUAL, should return only 1 resultInstance
            //resultInstance = historyService.CreateHistoricProcessInstanceQuery()
            //.VariableValueNotEquals("stringVar", "abcdef")
            //   .First();
            resultInstance = manager.VariableValueNotEquals("stringVar", "abcdef").FirstOrDefault();
            Assert.NotNull(resultInstance);
            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

            // Test GREATER_THAN, should return only matching 'azerty'
            // resultInstance = historyService.CreateHistoricProcessInstanceQuery()
            //.VariableValueGreaterThan("stringVar", "abcdef")
            //    .First();
            resultInstance = manager.VariableValueGreaterThan("stringVar", "abcdef").FirstOrDefault();
            Assert.NotNull(resultInstance);
            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

            // resultInstance = historyService.CreateHistoricProcessInstanceQuery()
            //.VariableValueGreaterThan("stringVar", "z")
            //    .First();
            resultInstance = manager.VariableValueGreaterThan("stringVar", "z").FirstOrDefault();
            Assert.IsNull(resultInstance);

            // Test GREATER_THAN_OR_EQUAL, should return 3 results
            //Assert.AreEqual(3, historyService.CreateHistoricProcessInstanceQuery()
            //    //.VariableValueGreaterThanOrEqual("stringVar", "abcdef")
            //    .Count());
            Assert.AreEqual(3, manager.VariableValueGreaterThanOrEqual("stringVar", "abcdef")
                .Count());
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery()
            //    //.VariableValueGreaterThanOrEqual("stringVar", "z")
            //    .Count());
            Assert.AreEqual(0, manager.VariableValueGreaterThanOrEqual("stringVar", "z")
                .Count());
            // Test LESS_THAN, should return 2 results
            //processInstances = historyService.CreateHistoricProcessInstanceQuery()
            //    //.VariableValueLessThan("stringVar", "abcdeg")
            //    .ToList();
            processInstances = manager.VariableValueLessThan("stringVar", "abcdeg").ToList();
            Assert.AreEqual(2, processInstances.Count);
            IList<string> expecedIds = new[] { processInstance1.Id, processInstance2.Id };
            IList<string> ids = new List<string> { processInstances[0].Id, processInstances[1].Id };
            //JAVA TO C# CONVERTER TODO Resources.Task: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
            //ids.RemoveAll(expecedIds);
            for (int i = 0; i < expecedIds.Count; i++)
            {
                for (int n = 0; n < ids.Count; n++)
                {
                    if (expecedIds[i] == ids[n])
                    {
                        ids.Remove(ids[n]);
                    }
                }
            }
            Assert.AreEqual(0, ids.Count);//.True(ids.Count == 0);

            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery()
            //    //.VariableValueLessThan("stringVar", "abcdef")
            //    .Count());
            Assert.AreEqual(0,manager.VariableValueLessThan("stringVar", "abcdef")
                .Count());
            //Assert.AreEqual(3, historyService.CreateHistoricProcessInstanceQuery()
            //    //.VariableValueLessThanOrEqual("stringVar", "z")
            //    .Count());
            Assert.AreEqual(3, manager.VariableValueLessThanOrEqual("stringVar", "z")
                .Count());

            // Test LESS_THAN_OR_EQUAL
            //processInstances = historyService.CreateHistoricProcessInstanceQuery()
            //    //.VariableValueLessThanOrEqual("stringVar", "abcdef")
            //    .ToList();
            processInstances = manager.VariableValueLessThanOrEqual("stringVar", "abcdef").ToList();
            Assert.AreEqual(2, processInstances.Count);
            expecedIds = new[] { processInstance1.Id, processInstance2.Id };
            ids = new List<string> { processInstances[0].Id, processInstances[1].Id };
            //JAVA TO C# CONVERTER TODO Resources.Task: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
            //ids.RemoveAll(expecedIds);
            for (int i = 0; i < expecedIds.Count; i++)
            {
                for (int n = 0; n < ids.Count; n++)
                {
                    if (expecedIds[i] == ids[n])
                    {
                        ids.Remove(ids[n]);
                    }
                }
            }
            Assert.True(ids.Count == 0);

            Assert.AreEqual(3, manager.VariableValueLessThanOrEqual("stringVar", "z")
                .Count());
            Assert.AreEqual(0, manager
                .VariableValueLessThanOrEqual("stringVar", "aa")
                .Count());

            // Test LIKE
            resultInstance = manager
                .VariableValueLike("stringVar", "azert")
                .FirstOrDefault();
            Assert.NotNull(resultInstance);
            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

            resultInstance = manager
                .VariableValueLike("stringVar", "y")
                .First();
            Assert.NotNull(resultInstance);
            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

            resultInstance = manager
                .VariableValueLike("stringVar", "zer")
                .First();
            Assert.NotNull(resultInstance);
            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

            Assert.AreEqual(3, manager
                .VariableValueLike("stringVar", "a")
                .Count());
            Assert.AreEqual(0, manager
                .VariableValueLike("stringVar", "x")
                .Count());

            historyService.DeleteHistoricProcessInstance(processInstance1.Id);
            historyService.DeleteHistoricProcessInstance(processInstance2.Id);
            historyService.DeleteHistoricProcessInstance(processInstance3.Id);
        }

        /// <summary>
        ///     Only do one second type, as the logic is same as in <seealso cref="ProcessInstanceQueryTest" /> and I do not want
        ///     to duplicate
        ///     all test case logic here.
        ///     Basically copied from <seealso cref="ProcessInstanceQueryTest" />
        /// </summary>
        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testQueryDateVariable()
        {
            IDictionary<string, object> vars = new Dictionary<string, object>();
            var date1 = new DateTime();
            vars["dateVar"] = date1;

            var processInstance1 = runtimeService.StartProcessInstanceByKey(ONE_TASK_PROCESS, vars);
            taskService.Complete(taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance1.Id)
                .First()
                .Id);

            var date2 = new DateTime();
            vars = new Dictionary<string, object>();
            vars["dateVar"] = date1;
            vars["dateVar2"] = date2;
            var processInstance2 = runtimeService.StartProcessInstanceByKey(ONE_TASK_PROCESS, vars);
            taskService.Complete(taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance2.Id)
                .First()
                .Id);

            var nextYear = new DateTime();
            nextYear.AddYears(1);
            vars = new Dictionary<string, object>();
            vars["dateVar"] = nextYear;
            var processInstance3 = runtimeService.StartProcessInstanceByKey(ONE_TASK_PROCESS, vars);
            taskService.Complete(taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance3.Id)
                .First()
                .Id);

            var nextMonth = new DateTime();
            nextMonth.AddMonths(1);

            var twoYearsLater = new DateTime();
            twoYearsLater.AddYears(2);

            var oneYearAgo = new DateTime();
            oneYearAgo.AddYears(-1);

            // Query on single short variable, should result in 2 matches
            var query = historyService.CreateHistoricProcessInstanceQuery()
                //.VariableValueEquals("dateVar", date1)
                ;
            var processInstances = query
                .ToList();
            Assert.NotNull(processInstances);
            Assert.AreEqual(2, processInstances.Count);

            // Query on two short variables, should result in single value
            query = historyService.CreateHistoricProcessInstanceQuery()
                //.VariableValueEquals("dateVar", date1)
                //.VariableValueEquals("dateVar2", date2)
                ;
            var resultInstance = query.First();
            Assert.NotNull(resultInstance);
            Assert.AreEqual(processInstance2.Id, resultInstance.Id);

            // Query with unexisting variable value
            var unexistingDate = DateTime.Parse("01/01/1989 12:00:00");
            //(new SimpleDateFormat("dd/MM/yyyy hh:mm:ss")).parse("01/01/1989 12:00:00");
            resultInstance = historyService.CreateHistoricProcessInstanceQuery()
                //.VariableValueEquals("dateVar", unexistingDate)
                .First();
            Assert.IsNull(resultInstance);

            // Test NOT_EQUALS
            resultInstance = historyService.CreateHistoricProcessInstanceQuery()
                //.VariableValueNotEquals("dateVar", date1)
                .First();
            Assert.NotNull(resultInstance);
            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

            // Test GREATER_THAN
            resultInstance = historyService.CreateHistoricProcessInstanceQuery()
                //.VariableValueGreaterThan("dateVar", nextMonth)
                .First();
            Assert.NotNull(resultInstance);
            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery()
                //.VariableValueGreaterThan("dateVar", nextYear)
                .Count());
            Assert.AreEqual(3, historyService.CreateHistoricProcessInstanceQuery()
                //.VariableValueGreaterThan("dateVar", oneYearAgo)
                .Count());

            // Test GREATER_THAN_OR_EQUAL
            resultInstance = historyService.CreateHistoricProcessInstanceQuery()
                //.VariableValueGreaterThanOrEqual("dateVar", nextMonth)
                .First();
            Assert.NotNull(resultInstance);
            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

            resultInstance = historyService.CreateHistoricProcessInstanceQuery()
                //.VariableValueGreaterThanOrEqual("dateVar", nextYear)
                .First();
            Assert.NotNull(resultInstance);
            Assert.AreEqual(processInstance3.Id, resultInstance.Id);

            Assert.AreEqual(3, historyService.CreateHistoricProcessInstanceQuery()
                //.VariableValueGreaterThanOrEqual("dateVar", oneYearAgo)
                .Count());

            // Test LESS_THAN
            processInstances = historyService.CreateHistoricProcessInstanceQuery()
                //.VariableValueLessThan("dateVar", nextYear)

                .ToList();
            Assert.AreEqual(2, processInstances.Count);

            IList<string> expecedIds = new[] { processInstance1.Id, processInstance2.Id };
            IList<string> ids = new List<string> { processInstances[0].Id, processInstances[1].Id };
            //JAVA TO C# CONVERTER TODO Resources.Task: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
            //ids.RemoveAll(expecedIds);
            Assert.True(ids.Count == 0);

            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery()
                //.VariableValueLessThan("dateVar", date1)
                .Count());
            Assert.AreEqual(3, historyService.CreateHistoricProcessInstanceQuery()
                //.VariableValueLessThan("dateVar", twoYearsLater)
                .Count());

            // Test LESS_THAN_OR_EQUAL
            processInstances = historyService.CreateHistoricProcessInstanceQuery()
                //.VariableValueLessThanOrEqual("dateVar", nextYear)

                .ToList();
            Assert.AreEqual(3, processInstances.Count);

            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery()
                //.VariableValueLessThanOrEqual("dateVar", oneYearAgo)
                .Count());

            historyService.DeleteHistoricProcessInstance(processInstance1.Id);
            historyService.DeleteHistoricProcessInstance(processInstance2.Id);
            historyService.DeleteHistoricProcessInstance(processInstance3.Id);
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testNativeHistoricProcessInstanceTest()
        {
            // just test that the query will be constructed and executed, details are tested in the TaskQueryTest
            runtimeService.StartProcessInstanceByKey(ONE_TASK_PROCESS);
            Assert.AreEqual(1, historyService.CreateNativeHistoricProcessInstanceQuery()
                // .Sql("SELECT Count(*) FROM " + managementService.GetTableName(typeof(IHistoricProcessInstance)))
                .Count());
            Assert.AreEqual(1, historyService.CreateNativeHistoricProcessInstanceQuery()
                //.Sql("SELECT * FROM " + managementService.GetTableName(typeof(IHistoricProcessInstance)))

                .Count());
            //  Assert.AreEqual(1, historyService.CreateNativeHistoricProcessInstanceQuery().Sql("SELECT * FROM " + managementService.GetTableName(IHistoricProcessInstance.class))/*.ListPage(0, 1)*/.Count());
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testNativeHistoricTaskInstanceTest()
        {
            runtimeService.StartProcessInstanceByKey(ONE_TASK_PROCESS);
            Assert.AreEqual(1, historyService.CreateNativeHistoricTaskInstanceQuery()
                //.Sql("SELECT Count(*) FROM " + managementService.GetTableName(typeof(IHistoricProcessInstance)))
                .Count());
            Assert.AreEqual(1, historyService.CreateNativeHistoricTaskInstanceQuery()
                //.Sql("SELECT * FROM " + managementService.GetTableName(typeof(IHistoricProcessInstance)))

                .Count());
            Assert.AreEqual(1, historyService.CreateNativeHistoricTaskInstanceQuery()
                //.Sql("SELECT * FROM " + managementService.GetTableName(typeof(IHistoricProcessInstance)))
                /*.ListPage(0, 1)*/
                .Count());
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testNativeHistoricActivityInstanceTest()
        {
            runtimeService.StartProcessInstanceByKey(ONE_TASK_PROCESS);
            Assert.AreEqual(1, historyService.CreateNativeHistoricActivityInstanceQuery()
                //.Sql("SELECT Count(*) FROM " + managementService.GetTableName(typeof(IHistoricProcessInstance)))
                .Count());
            //Assert.AreEqual(1, historyService.CreateNativeHistoricActivityInstanceQuery().Sql("SELECT * FROM " + managementService.GetTableName(typeof(IHistoricProcessInstance))).Count());
            //Assert.AreEqual(1, historyService.CreateNativeHistoricActivityInstanceQuery().Sql("SELECT * FROM " + managementService.GetTableName(typeof(IHistoricProcessInstance)))/*.ListPage(0, 1)*/.Count());
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        public virtual void testProcessVariableValueEqualsNumber()
        {
            //// long
            //runtimeService.StartProcessInstanceByKey(ONE_TASK_PROCESS, Collections.singletonMap<string, object>("var", 123L));

            //// non-matching long
            //runtimeService.StartProcessInstanceByKey(ONE_TASK_PROCESS, Collections.singletonMap<string, object>("var", 12345L));

            //// short
            //runtimeService.StartProcessInstanceByKey(ONE_TASK_PROCESS, Collections.singletonMap<string, object>("var", (short) 123));

            //// double
            //runtimeService.StartProcessInstanceByKey(ONE_TASK_PROCESS, Collections.singletonMap<string, object>("var", 123.0d));

            //// integer
            //runtimeService.StartProcessInstanceByKey(ONE_TASK_PROCESS, Collections.singletonMap<string, object>("var", 123));

            //// untyped null (should not match)
            //runtimeService.StartProcessInstanceByKey(ONE_TASK_PROCESS, Collections.singletonMap<string, object>("var", null));

            //// typed null (should not match)
            //runtimeService.StartProcessInstanceByKey(ONE_TASK_PROCESS, Collections.singletonMap<string, object>("var", Variables.longValue(null)));

            //runtimeService.StartProcessInstanceByKey(ONE_TASK_PROCESS, Collections.singletonMap<string, object>("var", "123"));

            Assert.AreEqual(4, historyService.CreateHistoricProcessInstanceQuery()
                //.VariableValueEquals("var", Variable.Variables.NumberValue(123))
                .Count());
            Assert.AreEqual(4, historyService.CreateHistoricProcessInstanceQuery()
                //.VariableValueEquals("var", Variable.Variables.NumberValue(123L))
                .Count());
            Assert.AreEqual(4, historyService.CreateHistoricProcessInstanceQuery()
                //.VariableValueEquals("var", Variable.Variables.NumberValue(123.0m))
                .Count());
            Assert.AreEqual(4, historyService.CreateHistoricProcessInstanceQuery()
                //.VariableValueEquals("var", Variable.Variables.NumberValue(123))
                .Count());

            Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery()
                //.VariableValueEquals("var", Variable.Variables.NumberValue(0))
                .Count());
        }
        private IQueryable<ExecutionEntity> ProcessDefinitionKey(string key)
        {
            var db = runtimeService.GetDbContext();
            var query = from a in db.Set<ExecutionEntity>()
                        join b in db.Set<ProcessDefinitionEntity>() on a.ProcessDefinitionId equals b.Id
                        where a.ParentId == null && b.Key == key
                        select a;
            return query;
        }
        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testDeleteProcessInstance()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey(ONE_TASK_PROCESS);
            //Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery(m=>m.processdefinitionkey)
            //    //.SetProcessDefinitionKey(ONE_TASK_PROCESS)
            //    .Count());
            Assert.AreEqual(1, ProcessDefinitionKey(ONE_TASK_PROCESS).Count());

            runtimeService.DeleteProcessInstance(processInstance.Id, null);
            // Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
            //.SetProcessDefinitionKey(ONE_TASK_PROCESS)
            //.Count());
            Assert.AreEqual(0, ProcessDefinitionKey(ONE_TASK_PROCESS).Count());
            Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery(c => c.ProcessDefinitionKey == ONE_TASK_PROCESS)
                .Count());

            historyService.DeleteHistoricProcessInstance(processInstance.Id);
            // Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
            //.SetProcessDefinitionKey(ONE_TASK_PROCESS)
            // .Count());
            Assert.AreEqual(0, ProcessDefinitionKey(ONE_TASK_PROCESS).Count());
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testDeleteRunningProcessInstance()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey(ONE_TASK_PROCESS);
            //Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
            //.SetProcessDefinitionKey(ONE_TASK_PROCESS)
            //.Count());
            Assert.AreEqual(1, ProcessDefinitionKey(ONE_TASK_PROCESS).Count());
            try
            {
                historyService.DeleteHistoricProcessInstance(processInstance.Id);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("Process instance is still running, cannot delete historic process instance",
                    ae.Message);
            }
        }

        [Test]
        public virtual void testDeleteProcessInstanceUnexistingId()
        {
            try
            {
                historyService.DeleteHistoricProcessInstance("enexistingInstanceId");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("No historic process instance found with id", ae.Message);
            }
        }

        [Test]
        public virtual void testDeleteProcessInstanceNullId()
        {
            try
            {
                historyService.DeleteHistoricProcessInstance(null);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("processInstanceId is null", ae.Message);
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testDeleteProcessInstances()
        {
            //given
            var ids = prepareHistoricProcesses();

            //when
            historyService.DeleteHistoricProcessInstances(ids);

            //then
            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery(c => c.ProcessDefinitionKey == ONE_TASK_PROCESS)
                .Count());
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testDeleteProcessInstancesWithFake()
        {
            //given
            var ids = prepareHistoricProcesses();
            ids.Add("aFake");

            try
            {
                //when
                historyService.DeleteHistoricProcessInstances(ids);
                Assert.Fail("Exception expected");
            }
            catch (ProcessEngineException)
            {
                //expected
            }

            //then
            Assert.AreEqual(2, historyService.CreateHistoricProcessInstanceQuery(c => c.ProcessDefinitionKey == ONE_TASK_PROCESS)
                .Count());
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testDeleteProcessInstancesWithNull()
        {
            try
            {
                //when
                historyService.DeleteHistoricProcessInstances(null);
                Assert.Fail("Exception expected");
            }
            catch (ProcessEngineException)
            {
                //expected
            }
        }

        protected internal virtual IList<string> prepareHistoricProcesses()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey(ONE_TASK_PROCESS);
            var processInstance2 = runtimeService.StartProcessInstanceByKey(ONE_TASK_PROCESS);

            IList<string> processInstanceIds = new List<string>(new[] { processInstance.Id, processInstance2.Id });
            runtimeService.DeleteProcessInstances(processInstanceIds, null, true, true);

            return processInstanceIds;
        }
    }
}