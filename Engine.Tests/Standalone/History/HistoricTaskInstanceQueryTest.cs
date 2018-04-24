//using System.Linq;
//using ESS.FW.Bpm.Engine.Variable;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.Standalone.History
//{
//    /// <summary>
//    /// </summary>
//    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryAudit)]
//    [TestFixture]
//    public class HistoricTaskInstanceQueryTest : PluggableProcessEngineTestCase
//    {
//        [Test]
//        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
//        [Deployment( "resources/api/runtime/oneTaskProcess.bpmn20.xml")]
//        public virtual void TestGroupTaskQuery()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            var taskId = taskService.CreateTaskQuery().First().Id;
//            // if
//            identityService.AuthenticatedUserId = "aAssignerId";
//            taskService.AddCandidateUser(taskId, "aUserId");
//            taskService.AddCandidateGroup(taskId, "aGroupId");
//            taskService.AddCandidateGroup(taskId, "bGroupId");
//            var taskOne = taskService.NewTask("taskOne");
//            taskOne.Assignee = "aUserId";
//            taskService.SaveTask(taskOne);
//            var taskTwo = taskService.NewTask("taskTwo");
//            taskTwo.Assignee = "aUserId";
//            taskService.SaveTask(taskTwo);
//            var taskThree = taskService.NewTask("taskThree");
//            taskThree.Owner = "aUserId";
//            taskService.SaveTask(taskThree);
//            taskService.DeleteCandidateGroup(taskId, "aGroupId");
//            taskService.DeleteCandidateGroup(taskId, "bGroupId");
//            historyService.CreateHistoricTaskInstanceQuery();

//            // Query test
//            var query = historyService.CreateHistoricTaskInstanceQuery();
//            Assert.AreEqual(4, query.TaskInvolvedUser("aUserId").Count());
//            query = historyService.CreateHistoricTaskInstanceQuery();
//            Assert.AreEqual(1, query.TaskHadCandidateUser("aUserId").Count());
//            query = historyService.CreateHistoricTaskInstanceQuery();
//            Assert.AreEqual(1, query.TaskHadCandidateGroup("aGroupId").Count());
//            Assert.AreEqual(1, query.TaskHadCandidateGroup("bGroupId").Count());
//            Assert.AreEqual(0, query.TaskInvolvedUser("aUserId").Count());
//            query = historyService.CreateHistoricTaskInstanceQuery();
//            Assert.AreEqual(4, query.TaskInvolvedUser("aUserId").Count());
//            Assert.AreEqual(1, query.TaskHadCandidateUser("aUserId").Count());
//            Assert.AreEqual(1, query.TaskInvolvedUser("aUserId").Count());
//            // Delete task
//            taskService.DeleteTask("taskOne", true);
//            taskService.DeleteTask("taskTwo", true);
//            taskService.DeleteTask("taskThree", true);
//        }

//        [Test]
//        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
//        public virtual void TestProcessVariableValueEqualsNumber()
//        {
//            //// long
//            //runtimeService.StartProcessInstanceByKey("oneTaskProcess", new Dictionary<string, ITypedValue>(){"var", 123L});

//            //// non-matching long
//            //runtimeService.StartProcessInstanceByKey("oneTaskProcess", new Dictionary<string, ITypedValue>(){{"var", 12345L));

//            //// short
//            //runtimeService.StartProcessInstanceByKey("oneTaskProcess", new Dictionary<string, ITypedValue>(){{"var", (short) 123));

//            //// double
//            //runtimeService.StartProcessInstanceByKey("oneTaskProcess", new Dictionary<string, ITypedValue>(){{"var", 123.0d));

//            //// integer
//            //runtimeService.StartProcessInstanceByKey("oneTaskProcess", new Dictionary<string, ITypedValue>(){{"var", 123));

//            //// untyped null (should not match)
//            //runtimeService.StartProcessInstanceByKey("oneTaskProcess", new Dictionary<string, ITypedValue>(){{"var", null));

//            //// typed null (should not match)
//            //runtimeService.StartProcessInstanceByKey("oneTaskProcess", new Dictionary<string, ITypedValue>(){{"var", Variables.LongValue(null)));

//            //runtimeService.StartProcessInstanceByKey("oneTaskProcess", new Dictionary<string, ITypedValue>(){{"var", "123"));

//            Assert.AreEqual(4,
//                historyService.CreateHistoricTaskInstanceQuery()
//                    //.ProcessVariableValueEquals("var", Engine.Variable.Variables.NumberValue(123))
//                    .Count());
//            Assert.AreEqual(4,
//                historyService.CreateHistoricTaskInstanceQuery()
//                    //.ProcessVariableValueEquals("var", Engine.Variable.Variables.NumberValue(123L))
//                    .Count());
//            Assert.AreEqual(4,
//                historyService.CreateHistoricTaskInstanceQuery()
//                    //.ProcessVariableValueEquals("var", Engine.Variable.Variables.NumberValue(123.0M))
//                    .Count());
//            Assert.AreEqual(4,
//                historyService.CreateHistoricTaskInstanceQuery()
//                    //.ProcessVariableValueEquals("var", Engine.Variable.Variables.NumberValue(123))
//                    .Count());

//            Assert.AreEqual(1,
//                historyService.CreateHistoricTaskInstanceQuery()
//                    //.ProcessVariableValueEquals("var", Engine.Variable.Variables.NumberValue(0))
//                    .Count());
//        }

//        [Test]
//        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
//        public virtual void TestTaskHadCandidateGroup()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            var taskId = taskService.CreateTaskQuery().First().Id;
//            // if
//            identityService.AuthenticatedUserId = "aAssignerId";
//            taskService.AddCandidateGroup(taskId, "bGroupId");
//            taskService.DeleteCandidateGroup(taskId, "bGroupId");
//            // query test
//            Assert.AreEqual(1,
//                historyService.CreateHistoricTaskInstanceQuery().TaskHadCandidateGroup("bGroupId").Count());
//            Assert.AreEqual(0,
//                historyService.CreateHistoricTaskInstanceQuery().TaskHadCandidateGroup("invalidGroupId").Count());
//            // Delete test
//            taskService.DeleteTask("NewTask", true);
//        }

//        [Test]
//        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
//        [Deployment( "resources/api/runtime/oneTaskProcess.bpmn20.xml")]
//        public virtual void TestTaskHadCandidateUser()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            var taskId = taskService.CreateTaskQuery().First().Id;
//            // if
//            identityService.AuthenticatedUserId = "aAssignerId";
//            taskService.AddCandidateUser(taskId, "aUserId");
//            taskService.AddCandidateUser(taskId, "bUserId");
//            taskService.DeleteCandidateUser(taskId, "bUserId");
//            var taskAssignee = taskService.NewTask("NewTask");
//            taskAssignee.Assignee = "aUserId";
//            taskService.SaveTask(taskAssignee);
//            // query test
//            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery().TaskHadCandidateUser("aUserId").Count());
//            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery().TaskHadCandidateUser("bUserId").Count());
//            Assert.AreEqual(0,
//                historyService.CreateHistoricTaskInstanceQuery().TaskHadCandidateUser("invalidUserId").Count());
//            // Delete test
//            taskService.DeleteTask("NewTask", true);
//        }

//        [Test]
//        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull) ]
//        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml" )]
//        public virtual void TestTaskInvolvedGroup()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            var taskId = taskService.CreateTaskQuery().First().Id;
//            // if
//            identityService.AuthenticatedUserId = "aAssignerId";
//            taskService.AddCandidateGroup(taskId, "aGroupId");
//            taskService.AddCandidateGroup(taskId, "bGroupId");
//            taskService.DeleteCandidateGroup(taskId, "aGroupId");
//            taskService.DeleteCandidateGroup(taskId, "bGroupId");
//            // query test
//            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery().TaskInvolvedGroup("aGroupId").Count());
//            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery().TaskInvolvedGroup("bGroupId").Count());
//            Assert.AreEqual(0,
//                historyService.CreateHistoricTaskInstanceQuery().TaskInvolvedGroup("invalidGroupId").Count());

//            taskService.DeleteTask("NewTask", true);
//        }

//        [Test]
//        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
//    [Deployment( "resources/api/runtime/oneTaskProcess.bpmn20.xml" )] 
//        public virtual void TestTaskInvolvedUser()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            var taskId = taskService.CreateTaskQuery().First().Id;
//            // if
//            identityService.AuthenticatedUserId = "aAssignerId";
//            taskService.AddCandidateUser(taskId, "aUserId");
//            taskService.AddCandidateUser(taskId, "bUserId");
//            taskService.DeleteCandidateUser(taskId, "aUserId");
//            taskService.DeleteCandidateUser(taskId, "bUserId");
//            var taskAssignee = taskService.NewTask("NewTask");
//            taskAssignee.Assignee = "aUserId";
//            taskService.SaveTask(taskAssignee);
//            // query test
//            Assert.AreEqual(2, historyService.CreateHistoricTaskInstanceQuery().TaskInvolvedUser("aUserId").Count());
//            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery().TaskInvolvedUser("bUserId").Count());
//            Assert.AreEqual(0,
//                historyService.CreateHistoricTaskInstanceQuery().TaskInvolvedUser("invalidUserId").Count());
//            taskService.DeleteTask("NewTask", true);
//        }

//        [Test]
//        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
//        public virtual void TestTaskVariableValueEqualsNumber()
//        {
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//            var tasks = taskService.CreateTaskQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess").ToList();
//            Assert.AreEqual(8, tasks.Count);
//            taskService.SetVariableLocal(tasks[0].Id, "var", 123L);
//            taskService.SetVariableLocal(tasks[1].Id, "var", 12345L);
//            taskService.SetVariableLocal(tasks[2].Id, "var", (short) 123);
//            taskService.SetVariableLocal(tasks[3].Id, "var", 123.0d);
//            taskService.SetVariableLocal(tasks[4].Id, "var", 123);
//            taskService.SetVariableLocal(tasks[5].Id, "var", null);
//            taskService.SetVariableLocal(tasks[6].Id, "var", Variables.LongValue(null));
//            taskService.SetVariableLocal(tasks[7].Id, "var", "123");

//            Assert.AreEqual(4,
//                historyService.CreateHistoricTaskInstanceQuery()
//                    ////.TaskVariableValueEquals("var", Engine.Variable.Variables.NumberValue(123))
//                    .Count());
//            Assert.AreEqual(4,
//                historyService.CreateHistoricTaskInstanceQuery()
//                    ////.TaskVariableValueEquals("var", Engine.Variable.Variables.NumberValue(123L))
//                    .Count());
//            Assert.AreEqual(4,
//                historyService.CreateHistoricTaskInstanceQuery()
//                    ////.TaskVariableValueEquals("var", Engine.Variable.Variables.NumberValue(123.0M))
//                    .Count());
//            Assert.AreEqual(4,
//                historyService.CreateHistoricTaskInstanceQuery()
//                    ////.TaskVariableValueEquals("var", Engine.Variable.Variables.NumberValue(123))
//                    .Count());

//            Assert.AreEqual(1,
//                historyService.CreateHistoricTaskInstanceQuery()
//                    ////.TaskVariableValueEquals("var", Engine.Variable.Variables.NumberValue(0))
//                    .Count());
//        }

//        [Test]
//        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
//        [Deployment( "resources/api/runtime/oneTaskProcess.bpmn20.xml" )]
//        public virtual void TestTaskWasAssigned()
//        {
//            // given
//            var taskOne = taskService.NewTask("taskOne");
//            var taskTwo = taskService.NewTask("taskTwo");
//            var taskThree = taskService.NewTask("taskThree");

//            // when
//            taskOne.Assignee = "aUserId";
//            taskService.SaveTask(taskOne);

//            taskTwo.Assignee = "anotherUserId";
//            taskService.SaveTask(taskTwo);

//            taskService.SaveTask(taskThree);

//            var list = historyService.CreateHistoricTaskInstanceQuery()/*.TaskAssigned()*/.ToList();

//            // then
//            Assert.AreEqual(list.Count, 2);

//            // cleanup
//            taskService.DeleteTask("taskOne", true);
//            taskService.DeleteTask("taskTwo", true);
//            taskService.DeleteTask("taskThree", true);
//        }

//        [Test]
//        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
//        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml" )]
//        public virtual void TestTaskWasUnassigned()
//        {
//            // given
//            var taskOne = taskService.NewTask("taskOne");
//            var taskTwo = taskService.NewTask("taskTwo");
//            var taskThree = taskService.NewTask("taskThree");

//            // when
//            taskOne.Assignee = "aUserId";
//            taskService.SaveTask(taskOne);

//            taskTwo.Assignee = "anotherUserId";
//            taskService.SaveTask(taskTwo);

//            taskService.SaveTask(taskThree);

//            var list = historyService.CreateHistoricTaskInstanceQuery()/*.TaskUnassigned()*/.ToList();

//            // then
//            Assert.AreEqual(list.Count, 1);

//            // cleanup
//            taskService.DeleteTask("taskOne", true);
//            taskService.DeleteTask("taskTwo", true);
//            taskService.DeleteTask("taskThree", true);
//        }

//        [Test]
//        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
//        [Deployment( "resources/api/runtime/oneTaskProcess.bpmn20.xml" )]
//        public virtual void TestWithCandidateGroups()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            var taskId = taskService.CreateTaskQuery().First().Id;

//            // when
//            identityService.AuthenticatedUserId = "aAssignerId";
//            taskService.AddCandidateGroup(taskId, "aGroupId");

//            // then
//            Assert.AreEqual(historyService.CreateHistoricTaskInstanceQuery().WithCandidateGroups().Count(), 1);

//            // cleanup
//            taskService.DeleteTask("NewTask", true);
//        }

//        [Test]
//        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull) ]
//        [Deployment( "resources/api/runtime/oneTaskProcess.bpmn20.xml" ) ]
//        public virtual void TestWithoutCandidateGroups()
//        {
//            // given
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            var taskId = taskService.CreateTaskQuery().First().Id;
//            identityService.AuthenticatedUserId = "aAssignerId";
//            taskService.AddCandidateGroup(taskId, "aGroupId");

//            // when
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//            // then
//            Assert.AreEqual(historyService.CreateHistoricTaskInstanceQuery().Count(), 2);
//            Assert.AreEqual(historyService.CreateHistoricTaskInstanceQuery().WithoutCandidateGroups().Count(), 1);

//            // cleanup
//            taskService.DeleteTask("NewTask", true);
//        }
//    }
//}