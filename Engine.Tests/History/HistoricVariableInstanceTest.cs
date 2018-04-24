using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime.Util;
using Engine.Tests.Dmn.BusinessRuleTask;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Value;
using NUnit.Framework;

namespace Engine.Tests.History
{
    /// <summary>
    ///     
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryAudit)]
    [TestFixture]
    public class HistoricVariableInstanceTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment(new[] { "resources/history/orderProcess.bpmn20.xml", "resources/history/checkCreditProcess.bpmn20.xml" })]
        public virtual void testOrderProcessWithCallActivity()
        {
            // After the process has started, the 'verify credit history' task should be active
            var pi = runtimeService.StartProcessInstanceByKey("orderProcess");
            var taskQuery = taskService.CreateTaskQuery();
            var verifyCreditTask = taskQuery.First();
            Assert.AreEqual("Verify credit history", verifyCreditTask.Name);

            // Verify with Query API
            //var subProcessInstance = runtimeService.CreateProcessInstanceQuery(m=>m.SuperExecutionId==pi.Id)
            //    //.SetSuperProcessInstanceId(pi.Id)
            //    .First();
            var db = runtimeService.GetDbContext();
            var query = from a in db.Set<ExecutionEntity>()
                        join b in db.Set<ProcessDefinitionEntity>() on a.ProcessDefinitionId equals b.Id
                        where a.ParentId == null && (from c in db.Set<ExecutionEntity>() where c.ProcessInstanceId == pi.Id select c.Id).Contains(a.SuperExecutionId)
                        orderby a.Id
                        select a;
            var subProcessInstance = query.FirstOrDefault();

            Assert.NotNull(subProcessInstance);
            //Assert.AreEqual(pi.Id, runtimeService.CreateProcessInstanceQuery(m => m.SuperExecutionId == pi.Id)
            //    /*.SetSubProcessInstanceId(subProcessInstance.Id)*/
            //    .First()
            //    .Id);
            var query_2 = from a in db.Set<ExecutionEntity>()
                          join b in db.Set<ProcessDefinitionEntity>() on a.ProcessDefinitionId equals b.Id
                          where a.ParentId == null && a.Id == (from c in db.Set<ExecutionEntity>() where c.Id == (from d in db.Set<ExecutionEntity>() where d.Id == subProcessInstance.Id select d.SuperExecutionId).FirstOrDefault() select c.ProcessInstanceId).FirstOrDefault()
                          orderby a.Id
                          select a;

            Assert.AreEqual(pi.Id, query_2.FirstOrDefault().Id);

            // Completing the task with approval, will end the subprocess and continue the original process
            taskService.Complete(verifyCreditTask.Id, new Dictionary<string, object> { { "creditApproved", true } });

            var prepareAndShipTask = taskQuery.First();
            Assert.AreEqual("Prepare and Ship", prepareAndShipTask.Name);
        }

        //[Test]//不支持groovy语法,
        [Deployment(new[] { "resources/history/HistoricVariableInstanceTest.TestCallSimpleSubProcess.bpmn20.xml", "resources/history/simpleSubProcess.bpmn20.xml" })]
        public virtual void testHistoricVariableInstanceQuery()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("callSimpleSubProcess");
            AssertProcessEnded(processInstance.Id);

            Assert.AreEqual(5, historyService.CreateHistoricVariableInstanceQuery()
                .Count());
            Assert.AreEqual(5, historyService.CreateHistoricVariableInstanceQuery()

                .Count());
            Assert.AreEqual(5, historyService.CreateHistoricVariableInstanceQuery()
                //.OrderByProcessInstanceId()
                /*.Asc()*/
                .Count());
            Assert.AreEqual(5, historyService.CreateHistoricVariableInstanceQuery()
                //.OrderByProcessInstanceId()
                /*.Asc()*/

                .Count());
            Assert.AreEqual(5, historyService.CreateHistoricVariableInstanceQuery()
                /*.OrderByVariableName()*/
                /*.Asc()*/
                .Count());
            Assert.AreEqual(5, historyService.CreateHistoricVariableInstanceQuery()
                /*.OrderByVariableName()*/
                /*.Asc()*/

                .Count());

            Assert.AreEqual(2, historyService.CreateHistoricVariableInstanceQuery(c => c.ProcessInstanceId == processInstance.Id)
                .Count());
            Assert.AreEqual(2, historyService.CreateHistoricVariableInstanceQuery(c => c.ProcessInstanceId == processInstance.Id)

                .Count());
            Assert.AreEqual(2, historyService.CreateHistoricVariableInstanceQuery()
                //.VariableName("myVar")
                .Count());
            Assert.AreEqual(2, historyService.CreateHistoricVariableInstanceQuery()
                //.VariableName("myVar")

                .Count());
            Assert.AreEqual(2, historyService.CreateHistoricVariableInstanceQuery()
                //.VariableNameLike("myVar1")
                .Count());
            Assert.AreEqual(2, historyService.CreateHistoricVariableInstanceQuery()
                //.VariableNameLike("myVar1")

                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricVariableInstanceQuery()
                //.VariableNameLike("my\\_Var%")
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricVariableInstanceQuery()
                //.VariableNameLike("my\\_Var%")

                .Count());

            var variables = historyService.CreateHistoricVariableInstanceQuery()

                .ToList();
            Assert.AreEqual(5, variables.Count);

            Assert.AreEqual(1, historyService.CreateHistoricVariableInstanceQuery()
                //.VariableValueEquals("myVar", "test123")
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricVariableInstanceQuery()
                //.VariableValueEquals("myVar", "test123")

                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricVariableInstanceQuery()
                //.VariableValueEquals("myVar1", "test456")
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricVariableInstanceQuery()
                //.VariableValueEquals("myVar1", "test456")

                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricVariableInstanceQuery()
                //.VariableValueEquals("myVar", "test666")
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricVariableInstanceQuery()
                //.VariableValueEquals("myVar", "test666")

                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricVariableInstanceQuery()
                //.VariableValueEquals("myVar1", "test666")
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricVariableInstanceQuery()
                //.VariableValueEquals("myVar1", "test666")

                .Count());

            Assert.AreEqual(8, historyService.CreateHistoricActivityInstanceQuery()
                .Count());

            if (FullHistoryEnabled)
                Assert.AreEqual(6, historyService.CreateHistoricDetailQuery()
                    .Count());

            // non-existing id:
            Assert.AreEqual(0, historyService.CreateHistoricVariableInstanceQuery()
                //.VariableId("non-existing")
                .Count());

            // existing-id
            var variable = historyService.CreateHistoricVariableInstanceQuery()
                /*.ListPage(0, 1)*/
                .ToList();
            Assert.AreEqual(1, historyService.CreateHistoricVariableInstanceQuery()
                //.VariableId(variable[0].Id)
                .Count());
        }

        [Test]
        [Deployment(new[] { "resources/history/HistoricVariableInstanceTest.TestCallSubProcessSettingVariableOnStart.bpmn20.xml", "resources/history/subProcessSetVariableOnStart.bpmn20.xml" })]
        public virtual void testCallSubProcessSettingVariableOnStart()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("callSubProcess");
            AssertProcessEnded(processInstance.Id);

            Assert.AreEqual(1, historyService.CreateHistoricVariableInstanceQuery(m=>m.State!= "DELETED")
                .Count());

            Assert.AreEqual(1, historyService.CreateHistoricVariableInstanceQuery(m => m.State != "DELETED" && m.Name == "aVariable" && m.SerializerName == "string" && m.TextValue == "aValue")
                //.VariableValueEquals("aVariable", "aValue")
                .Count());
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testHistoricProcessVariableOnDeletion()
        {
            var variables = new Dictionary<string, object>();
            variables["testVar"] = "Hallo Christian";
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);
            runtimeService.DeleteProcessInstance(processInstance.Id, "deleted");
            AssertProcessEnded(processInstance.Id);
            var test = historyService.CreateHistoricVariableInstanceQuery(c => c.ProcessInstanceId == processInstance.Id).First();
            // check that process variable is set even if the process is canceled and not ended normally
            Assert.AreEqual(1, historyService.CreateHistoricVariableInstanceQuery(c => c.ProcessInstanceId == processInstance.Id)
                //.VariableValueEquals("testVar", "Hallo Christian")
                .Count());
        }
        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public void testHistoricProcessVariableByteArrayId()
        {
            var variables = new Dictionary<string, object>();
            //ObjectValueImpl obj = new ObjectValueImpl(new Exp());
            Exp e = new Exp();
            variables.Add("exp", e);
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);
            IHistoricVariableInstance variableInstance = historyService.CreateHistoricVariableInstanceQuery(c => c.ProcessInstanceId == processInstance.Id).First();
            IObjectValue oval = (IObjectValue)variableInstance.Value;
            Exp val =(Exp)(oval.Value);
            Assert.NotNull(val);
            Assert.AreEqual("ExpName", val.Name);
            Assert.AreEqual("Exp的值", val.Value);
        }
        

        [Test]
        [Deployment("resources/standalone/history/FullHistoryTest.TestVariableUpdatesAreLinkedToActivity.bpmn20.xml")]
        public virtual void testVariableUpdatesLinkedToActivity()
        {
            if (FullHistoryEnabled)
            {
                var pi = runtimeService.StartProcessInstanceByKey("ProcessWithSubProcess");

                var task = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id)
                    .First();
                IDictionary<string, object> variables = new Dictionary<string, object>();
                variables["test"] = "1";
                taskService.Complete(task.Id, variables);

                // now we are in the subprocess
                task = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id)
                    .First();
                variables.Clear();
                variables["test"] = "2";
                taskService.Complete(task.Id, variables);

                // now we are ended
                AssertProcessEnded(pi.Id);

                // check history
                var updates = historyService.CreateHistoricDetailQuery< HistoricVariableUpdateEventEntity>()
                    /*.VariableUpdates()*/

                    .ToList();
                Assert.AreEqual(2, updates.Count);

                IDictionary<string, IHistoricVariableUpdate> updatesMap =
                    new Dictionary<string, IHistoricVariableUpdate>();
                var update = (IHistoricVariableUpdate)updates[0];
                updatesMap[(string)update.Value] = update;
                update = (IHistoricVariableUpdate)updates[1];
                updatesMap[(string)update.Value] = update;

                var update1 = updatesMap["1"];
                var update2 = updatesMap["2"];

                Assert.NotNull(update1.ActivityInstanceId);
                Assert.NotNull(update1.ExecutionId);
                var historicActivityInstance1 = historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityInstanceId == update1.ActivityInstanceId)
                    .First();
                Assert.AreEqual(historicActivityInstance1.ExecutionId, update1.ExecutionId);
                Assert.AreEqual("usertask1", historicActivityInstance1.ActivityId);

                // TODO http://jira.codehaus.org/browse/ACT-1083
                Assert.NotNull(update2.ActivityInstanceId);
                var historicActivityInstance2 = historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityInstanceId == update2.ActivityInstanceId)
                    .First();
                Assert.AreEqual("usertask2", historicActivityInstance2.ActivityId);

                /*
                 * This is OK! The variable is set on the root execution, on a execution never run through the activity, where the process instances
                 * stands when calling the set Variable. But the ActivityId of this flow node is used. So the execution id's doesn't have to be equal.
                 *
                 * execution id: On which execution it was set
                 * activity id: in which activity was the process instance when setting the variable
                 */
                Assert.IsFalse(historicActivityInstance2.ExecutionId.Equals(update2.ExecutionId));
            }
        }

        //[Test]//: .net不支持groovy语法,
        [Deployment("resources/history/HistoricVariableInstanceTest.TestParallel.bpmn20.xml")]
        public virtual void testHistoricVariableInstanceQueryByTaskIds()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("myProc");

            var taskQuery = taskService.CreateTaskQuery();
            var userTask = taskQuery.First();
            Assert.AreEqual("userTask1", userTask.Name);

            // set local variable on IUser task
            taskService.SetVariableLocal(userTask.Id, "taskVariable", "aCustomValue");

            // complete IUser task to finish process instance
            taskService.Complete(userTask.Id);

            AssertProcessEnded(processInstance.Id);

            var tasks = historyService.CreateHistoricTaskInstanceQuery(c => c.ProcessInstanceId == processInstance.ProcessInstanceId)

                .ToList();
            Assert.AreEqual(1, tasks.Count);

            // check existing variables
            Assert.AreEqual(3, historyService.CreateHistoricVariableInstanceQuery()
                .Count());

            // check existing variables for task ID
            Assert.AreEqual(1, historyService.CreateHistoricVariableInstanceQuery(c => c.TaskId == tasks[0].Id)

                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricVariableInstanceQuery(c => c.TaskId == tasks[0].Id)
                .Count());
        }

        //[Test]//: .net不支持groovy语法,
        [Deployment("resources/history/HistoricVariableInstanceTest.TestParallel.bpmn20.xml")]
        public virtual void testHistoricVariableInstanceQueryByProcessIdIn()
        {
            // given
            IDictionary<string, object> vars = new Dictionary<string, object>();
            var processInstance = runtimeService.StartProcessInstanceByKey("myProc", vars);
            var processInstance2 = runtimeService.StartProcessInstanceByKey("myProc", vars);

            // check existing variables for process instance ID
            Assert.AreEqual(4, historyService.CreateHistoricVariableInstanceQuery(m => m.ProcessInstanceId== processInstance.ProcessInstanceId|| m.ProcessInstanceId == processInstance2.ProcessInstanceId)
                .Count());
            //add check with not existing search
            var notExistingSearch = processInstance.ProcessInstanceId + "-notExisting";
            Assert.AreEqual(2, historyService.CreateHistoricVariableInstanceQuery(m => m.ProcessInstanceId == notExistingSearch || m.ProcessInstanceId == processInstance2.ProcessInstanceId)
                //.ProcessInstanceIdIn(notExistingSearch, processInstance2.ProcessInstanceId)
                .Count());
        }

        //[Test]//: .net不支持groovy语法,
        [Deployment("resources/history/HistoricVariableInstanceTest.TestParallel.bpmn20.xml")]
        public virtual void testHistoricVariableInstanceQueryByInvalidProcessIdIn()
        {
            // given
            IDictionary<string, object> vars = new Dictionary<string, object>();
            var processInstance = runtimeService.StartProcessInstanceByKey("myProc", vars);

            // check existing variables for task ID
            try
            {
                historyService.CreateHistoricVariableInstanceQuery()
                    //.ProcessInstanceIdIn(processInstance.ProcessInstanceId, null)
                    ;
                Assert.Fail("Search by process instance ID was finished");
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                historyService.CreateHistoricVariableInstanceQuery(m=>m.ProcessInstanceId==null||m.ProcessInstanceId== processInstance.ProcessInstanceId)
                    //.ProcessInstanceIdIn(null, processInstance.ProcessInstanceId)
                    ;
                Assert.Fail("Search by process instance ID was finished");
            }
            catch (ProcessEngineException)
            {
            }
        }

        [Test]
        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
        public virtual void testHistoricVariableInstanceQueryByExecutionIds()
        {
            // given
            IDictionary<string, object> variables1 = new Dictionary<string, object>();
            variables1["stringVar"] = "test";
            variables1["myVar"] = "test123";
            var processInstance1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

            var query = historyService.CreateHistoricVariableInstanceQuery(m=>m.ExecutionId==processInstance1.Id)
                //.ExecutionIdIn(processInstance1.Id)
                ;
            Assert.AreEqual(2, query.Count());
            var variableInstances = query
                .ToList();
            Assert.AreEqual(2, variableInstances.Count);
            foreach (var variableInstance in variableInstances)
                Assert.AreEqual(processInstance1.Id, variableInstance.ExecutionId);

            IDictionary<string, object> variables2 = new Dictionary<string, object>();
            variables2["myVar"] = "test123";
            var processInstance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

            query = historyService.CreateHistoricVariableInstanceQuery(m=>m.ExecutionId==processInstance1.Id||m.ExecutionId==processInstance2.Id)
                //.ExecutionIdIn(processInstance1.Id, processInstance2.Id)
                ;
            Assert.AreEqual(3, query
                .Count());
            Assert.AreEqual(3, query.Count());
        }

        [Test]
        public virtual void testQueryByInvalidExecutionIdIn()
        {
            var query = historyService.CreateHistoricVariableInstanceQuery(c => c.ExecutionId == "invalid");
            Assert.AreEqual(0, query.Count());

            try
            {
                historyService.CreateHistoricVariableInstanceQuery(c => c.ExecutionId == null);
                Assert.Fail("A ProcessEngineExcpetion was expected.");
            }
            catch (ProcessEngineException e)
            {
            }

            try
            {
                historyService.CreateHistoricVariableInstanceQuery(c => c.ExecutionId == (string)null);
                Assert.Fail("A ProcessEngineExcpetion was expected.");
            }
            catch (/*ProcessEngine*/System.Exception)
            {
            }
        }

        [Test]
        public virtual void testQueryByInvalidTaskIdIn()
        {
            var query = historyService.CreateHistoricVariableInstanceQuery(c => c.TaskId == "invalid");
            Assert.AreEqual(0, query.Count());

            try
            {
                historyService.CreateHistoricVariableInstanceQuery(m=>m.TaskId==null)
                    //.TaskIdIn(null)
                    ;
                Assert.Fail("A ProcessEngineExcpetion was expected.");
            }
            catch (ProcessEngineException e)
            {
            }

            try
            {
                historyService.CreateHistoricVariableInstanceQuery(m=>m.TaskId==null)
                    //.TaskIdIn((string) null)
                    ;
                Assert.Fail("A ProcessEngineExcpetion was expected.");
            }
            catch (ProcessEngineException e)
            {
            }
        }

        [Test]
        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
        public virtual void testQueryByActivityInstanceIdIn()
        {
            // given
            IDictionary<string, object> variables1 = new Dictionary<string, object>();
            variables1["stringVar"] = "test";
            variables1["myVar"] = "test123";
            var processInstance1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

            var query = historyService.CreateHistoricVariableInstanceQuery();

            //query.ActivityInstanceIdIn(processInstance1.Id);

            Assert.AreEqual(2, query
                .Count());
            Assert.AreEqual(2, query.Count());

            IDictionary<string, object> variables2 = new Dictionary<string, object>();
            variables2["myVar"] = "test123";
            var processInstance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

            //query.ActivityInstanceIdIn(processInstance1.Id, processInstance2.Id);

            Assert.AreEqual(3, query
                .Count());
            Assert.AreEqual(3, query.Count());
        }

        [Test]
        public virtual void testQueryByInvalidActivityInstanceIdIn()
        {
            var query = historyService.CreateHistoricVariableInstanceQuery();

            //query.TaskIdIn("invalid");
            query.Where(m => m.TaskId == "invalid");
            Assert.AreEqual(0, query.Count());

            try
            {
                //query.TaskIdIn(null);
                query.Where(m => m.TaskId == null);
                Assert.Fail("A ProcessEngineExcpetion was expected.");
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                //query.TaskIdIn((string) null);
                query.Where(m => m.TaskId == string.Empty);
                Assert.Fail("A ProcessEngineExcpetion was expected.");
            }
            catch (ProcessEngineException)
            {
            }
        }

        [Test]
        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
        public virtual void testQueryByVariableTypeIn()
        {
            // given
            IDictionary<string, object> variables1 = new Dictionary<string, object>();
            variables1["stringVar"] = "test";
            variables1["boolVar"] = true;
            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

            // when
            var query = historyService.CreateHistoricVariableInstanceQuery(m=>m.SerializerName== "string")
                //.VariableTypeIn("string")
                ;

            // then
            Assert.AreEqual(1, query
                .Count());
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(query
                .First().Name, "stringVar");
        }

        [Test]
        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
        public virtual void testQueryByVariableTypeInWithCapitalLetter()
        {
            // given
            IDictionary<string, object> variables1 = new Dictionary<string, object>();
            variables1["stringVar"] = "test";
            variables1["boolVar"] = true;
            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

            // when
            var query = historyService.CreateHistoricVariableInstanceQuery(m=>m.SerializerName== "boolean");
            //query.VariableTypeIn("Boolean");

            // then
            Assert.AreEqual(1, query
                .Count());
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(query
                .First().Name, "boolVar");
        }

        [Test]
        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
        public virtual void testQueryByVariableTypeInWithSeveralTypes()
        {
            // given
            IDictionary<string, object> variables1 = new Dictionary<string, object>();
            variables1["stringVar"] = "test";
            variables1["boolVar"] = true;
            variables1["intVar"] = 5;
            variables1["nullVar"] = null;
            variables1["pojoVar"] = new TestPojo("str", .0);
            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

            // when
            List<string> ids = new List<string>() { "BooLEAN".ToLower(), "string", "Serializable".ToLower() };
            var query = historyService.CreateHistoricVariableInstanceQuery(m=> ids.Contains(m.SerializerName));
            //query.VariableTypeIn("BooLEAN", "string", "Serializable");

            // then
            Assert.AreEqual(3, query
                .Count());
            Assert.AreEqual(3, query.Count());
        }

        [Test]
        public virtual void testQueryByInvalidVariableTypeIn()
        {
            // given
            var query = historyService.CreateHistoricVariableInstanceQuery();

            // when
            //query.VariableTypeIn("invalid");
            query.Where(m => m.SerializerName == "invalid");
            // then
            Assert.AreEqual(0, query.Count());

            try
            {
                // when
                //query.VariableTypeIn(null);
                query.Where(m => m.SerializerName == null);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
                // then fails
            }

            try
            {
                // when
                //query.VariableTypeIn(null);
                query.Where(m => m.SerializerName == null);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
                // then fails
            }
        }

        [Test]
        public virtual void testBinaryFetchingEnabled()
        {
            // by default, binary fetching is enabled

            var NewTask = taskService.NewTask();
            taskService.SaveTask(NewTask);

            var VariableName = "binaryVariableName";
            taskService.SetVariable(NewTask.Id, VariableName, "some bytes".GetBytes());

            var variableInstance = historyService.CreateHistoricVariableInstanceQuery(m=>m.Name== "binaryVariableName")
                /*.VariableName(VariableName)*/
                .First();

            Assert.NotNull(variableInstance.Value);

            taskService.DeleteTask(NewTask.Id, true);
        }

        [Test]//DisableBinaryFetching查询过滤 不支持
        public virtual void testBinaryFetchingDisabled()
        {
            var NewTask = taskService.NewTask();
            taskService.SaveTask(NewTask);

            var VariableName = "binaryVariableName";
            taskService.SetVariable(NewTask.Id, VariableName, "some bytes".GetBytes());

            var variableInstance = historyService.CreateHistoricVariableInstanceQuery().ToList().Where(m=>m.VariableName==VariableName)
                /*.VariableName(VariableName)*/
                /*.DisableBinaryFetching()*/
                .First();

            //Assert.IsNull(variableInstance.Value);
            Assert.NotNull(variableInstance.Value);
            taskService.DeleteTask(NewTask.Id, true);
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: Deployment(new string[] "resources/api/runtime/oneTaskProcess.bpmn20.xml") public void testDisableBinaryFetchingForFileValues()
        // public virtual void testDisableBinaryFetchingForFileValues()
        // {
        //// given
        //string fileName = "text.Txt";
        //string encoding = "crazy-encoding";
        //string mimeType = "martini/dry";

        //IFileValue fileValue = Variables.FileValue(fileName).File("ABC".GetBytes()).Encoding(encoding).MimeType(mimeType).Create();

        //runtimeService.StartProcessInstanceByKey("oneTaskProcess", Variable.Variables.CreateVariables().PutValueTyped("fileVar", fileValue));

        //// when enabling binary fetching
        //IHistoricVariableInstance fileVariableInstance = historyService.CreateHistoricVariableInstanceQuery().First();

        //// then the binary value is accessible
        //Assert.NotNull(fileVariableInstance.Value);

        //// when disabling binary fetching
        //fileVariableInstance = historyService.CreateHistoricVariableInstanceQuery()/*.DisableBinaryFetching()*/.First();

        //// then the byte value is not fetched
        //Assert.NotNull(fileVariableInstance);
        //Assert.AreEqual("fileVar", fileVariableInstance.Name);

        //Assert.IsNull(fileVariableInstance.Value);

        //IFileValue typedValue = (IFileValue) fileVariableInstance.TypedValue;
        //Assert.IsNull(typedValue.Value);

        //// but typed value metadata is accessible
        ////Assert.AreEqual(ValueType.FILE, typedValue.Type);
        //Assert.AreEqual(fileName, typedValue.Filename);
        //Assert.AreEqual(encoding, typedValue.Encoding);
        //Assert.AreEqual(mimeType, typedValue.MimeType);

        // }

        [Test]
        public virtual void testDisableCustomObjectDeserialization()
        {
            // given
            var NewTask = taskService.NewTask();
            taskService.SaveTask(NewTask);

            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["customSerializable"] = new CustomSerializable();
            variables["failingSerializable"] = new FailingSerializable();
            taskService.SetVariables(NewTask.Id, variables);

            // when
            var variableInstances = historyService.CreateHistoricVariableInstanceQuery(m=>m.State!= "DELETED")
                //.DisableCustomObjectDeserialization()

                .ToList();

            // then
            Assert.AreEqual(2, variableInstances.Count);

            foreach (var variableInstance in variableInstances)
            {
                Assert.IsNull(variableInstance.ErrorMessage);

                var typedValue = (IObjectValue)variableInstance.TypedValue;
                Assert.NotNull(typedValue);
                //Assert.IsFalse(typedValue.IsDeserialized);
                // cannot access the deserialized value
                try
                {
                    var typedValueValue = typedValue.Value;
                }
                catch (InvalidOperationException e)
                {
                    AssertTextPresent("Object is not deserialized", e.Message);
                }
                Assert.NotNull(typedValue.ValueSerialized);
            }

            taskService.DeleteTask(NewTask.Id, true);
        }

        [Test]
        public virtual void testErrorMessage()
        {
            var NewTask = taskService.NewTask();
            taskService.SaveTask(NewTask);

            var VariableName = "failingSerializable";
            taskService.SetVariable(NewTask.Id, VariableName, new FailingSerializable());

            var variableInstance = historyService.CreateHistoricVariableInstanceQuery().ToList().Where(m=>m.VariableName== VariableName)
                /*.VariableName(VariableName)*/
                .First();

            //Assert.IsNull(variableInstance.Value);
            //Assert.NotNull(variableInstance.ErrorMessage);
            Assert.IsNull(variableInstance.ErrorMessage);
            Assert.NotNull(variableInstance.Value);
            taskService.DeleteTask(NewTask.Id, true);
        }

        //[Test]
        [Deployment("resources/history/HistoricVariableInstanceTest.TestImplicitVariableUpdate.bpmn20.xml")]
        public virtual void FAILING_testImplicitVariableUpdateActivityInstanceId()
        {
            // given
            var instance = runtimeService.StartProcessInstanceByKey("serviceTaskProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("listVar", new List<string>())
                .PutValue("delegate", new UpdateValueDelegate()));

            var historicServiceTask = historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityId == "task")
                .First();

            var list = (IList<string>)runtimeService.GetVariable(instance.Id, "listVar");
            Assert.NotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(UpdateValueDelegate.NEW_ELEMENT, list[0]);

            // when
            var historicVariableInstance = historyService.CreateHistoricVariableInstanceQuery()
                /*.VariableName("listVar")*/
                .First();

            // then
            Assert.AreEqual(historicServiceTask.Id, historicVariableInstance.ActivityInstanceId);
        }

        //[Test]
        [Deployment("resources/history/HistoricVariableInstanceTest.TestImplicitVariableUpdate.bpmn20.xml")]
        public virtual void FAILING_testImplicitVariableUpdateAndReplacementInOneTransaction()
        {
            // given
            runtimeService.StartProcessInstanceByKey("serviceTaskProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("listVar", new List<string>())
                .PutValue("delegate", new UpdateAndReplaceValueDelegate()));

            var historicVariableInstance = historyService.CreateHistoricVariableInstanceQuery().ToList().Where(m=>m.VariableName== "listVar")
                /*.VariableName("listVar")*/
                .First();

            var historicList = (IList<string>)historicVariableInstance.Value;
            Assert.NotNull(historicList);
            Assert.AreEqual(0, historicList.Count);

            if (FullHistoryEnabled)
            {
                var historicDetails = historyService.CreateHistoricDetailQuery<HistoricVariableUpdateEventEntity>(m=>m.VariableInstanceId== historicVariableInstance.Id).OrderBy(m=>m.SequenceCounter)
                    /*.VariableUpdates()*/
                    ////.VariableInstanceId(historicVariableInstance.Id)
                    //.OrderPartiallyByOccurrence()
                    /*.Asc()*/

                    .ToList();

                Assert.AreEqual(3, historicDetails.Count);

                var update1 = (IHistoricVariableUpdate)historicDetails[0];
                var update2 = (IHistoricVariableUpdate)historicDetails[1];
                var update3 = (IHistoricVariableUpdate)historicDetails[2];

                var value1 = (IList<string>)update1.Value;

                Assert.NotNull(value1);
                Assert.True(value1.Count == 0);

                var value2 = (IList<string>)update2.Value;

                Assert.NotNull(value2);
                Assert.AreEqual(1, value2.Count);
                Assert.AreEqual(UpdateValueDelegate.NEW_ELEMENT, value2[0]);

                var value3 = (IList<string>)update3.Value;

                Assert.NotNull(value3);
                Assert.True(value3.Count == 0);
            }
        }

        [Test]
        [Deployment("resources/history/HistoricVariableInstanceTest.TestImplicitVariableUpdate.bpmn20.xml")]
        public virtual void testImplicitVariableRemoveAndUpdateInOneTransaction()
        {
            // given
            runtimeService.StartProcessInstanceByKey("serviceTaskProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("listVar", new List<string>())
                .PutValue("Delegate", new RemoveAndUpdateValueDelegate()));

            if (FullHistoryEnabled)
            {
                var historicDetails = historyService.CreateHistoricDetailQuery<HistoricVariableUpdateEventEntity>().OrderBy(m=>m.SequenceCounter)
                    /*.VariableUpdates()*/
                    //.OrderPartiallyByOccurrence()
                    /*.Asc()*/

                    .ToList();

                var detailsIt = historicDetails;
                //while (detailsIt.MoveNext())
                //    if (!"listVar".Equals(((IHistoricVariableUpdate)detailsIt.Current).VariableName))
                //    {
                //        //JAVA TO C# CONVERTER TODO Resources.Task: .NET enumerators are read-only:
                //        //detailsIt.Remove();
                //    }
                for (int i = 0; i < detailsIt.Count; i++)
                {
                    if(((IHistoricVariableUpdate)detailsIt[i]).VariableName!= "listVar")
                    {
                        detailsIt.Remove(detailsIt[i]);
                    }
                }
                // one for creation, one for deletion, none for update
                Assert.AreEqual(2, historicDetails.Count);

                var update1 = (IHistoricVariableUpdate)historicDetails[0];

                var value1 = update1.Value;

                Assert.NotNull(value1);
                Assert.True(((IList<string>)value1).Count == 0);
                //Assert.IsNull(value1);

                var update2 = (IHistoricVariableUpdate)historicDetails[1];
                Assert.IsNull(update2.Value);
            }
        }

        [Test]//error.identifier.property.notfound
        [Deployment("resources/history/HistoricVariableInstanceTest.TestNoImplicitUpdateOnHistoricValues.bpmn20.xml")]
        public virtual void testNoImplicitUpdateOnHistoricDetailValues()
        {
            if (!FullHistoryEnabled)
                return;

            // given
            runtimeService.StartProcessInstanceByKey("serviceTaskProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("listVar", new List<string>())
                .PutValue("delegate", new UpdateHistoricDetailValueDelegate()));

            // a task before the delegate ensures that the variables have actually been persisted
            // and can be fetched by querying
            var task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);

            // then
            var historicVariableInstance = historyService.CreateHistoricVariableInstanceQuery().ToList().Where(m=>m.VariableName== "listVar")
                /*.VariableName("listVar")*/
                .First();

            // One for "listvar", one for "delegate"
            Assert.AreEqual(2, historyService.CreateHistoricDetailQuery()
                .Count());

            var historicDetails = historyService.CreateHistoricDetailQuery< HistoricVariableUpdateEventEntity>(m=>m.VariableInstanceId== historicVariableInstance.Id)
                /*.VariableUpdates()*/
                ////.VariableInstanceId(historicVariableInstance.Id)

                .ToList();

            Assert.AreEqual(1, historicDetails.Count);

            var update1 = (IHistoricVariableUpdate)historicDetails[0];

            var value1 = (IList<string>)update1.Value;

            Assert.NotNull(value1);
            Assert.True(value1.Count == 0);
        }

        protected internal virtual bool FullHistoryEnabled
        {
            get { return processEngineConfiguration.HistoryLevel.Equals(HistoryLevelFields.HistoryLevelFull); }
        }

        [Test]
        [Deployment("resources/history/HistoricVariableInstanceTest.TestHistoricVariableInstanceRevision.bpmn20.xml")]
        public virtual void testVariableUpdateOrder()
        {
            // given:
            // a finished process instance
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            AssertProcessEnded(processInstance.Id);

            // when

            // then
            var variable = historyService.CreateHistoricVariableInstanceQuery()
                .First();
            Assert.NotNull(variable);

            if (FullHistoryEnabled)
            {
                var details = historyService.CreateHistoricDetailQuery< HistoricVariableUpdateEventEntity>(m=>m.VariableInstanceId==variable.Id).OrderBy(m=>m.SequenceCounter)
                    ////.VariableInstanceId(variable.Id)
                    //.OrderPartiallyByOccurrence()
                    /*.Asc()*/

                    .ToList();

                Assert.AreEqual(3, details.Count);

                var firstUpdate = (IHistoricVariableUpdate)details[0];
                Assert.AreEqual(1, firstUpdate.Value);

                var secondUpdate = (IHistoricVariableUpdate)details[1];
                Assert.AreEqual(2, secondUpdate.Value);
                Assert.True(((HistoryEvent)secondUpdate).SequenceCounter > ((HistoryEvent)firstUpdate).SequenceCounter);

                var thirdUpdate = (IHistoricVariableUpdate)details[2];
                Assert.AreEqual(3, thirdUpdate.Value);
                Assert.True(((HistoryEvent)thirdUpdate).SequenceCounter > ((HistoryEvent)secondUpdate).SequenceCounter);
            }
        }

        [Test]//async
        [Deployment("resources/history/HistoricVariableInstanceTest.TestHistoricVariableInstanceRevisionAsync.bpmn20.xml")]
        public virtual void testVariableUpdateOrderAsync()
        {
            // given:
            // a finished process instance
            var processInstance = runtimeService.StartProcessInstanceByKey("process");

            // when
            ExecuteAvailableJobs();

            // then
            AssertProcessEnded(processInstance.Id);

            var variable = historyService.CreateHistoricVariableInstanceQuery()
                .First();
            Assert.NotNull(variable);

            if (FullHistoryEnabled)
            {
                var details = historyService.CreateHistoricDetailQuery< HistoricVariableUpdateEventEntity>(m=>m.VariableInstanceId==variable.Id).OrderBy(m=>m.SequenceCounter)
                    ////.VariableInstanceId(variable.Id)
                    //.OrderPartiallyByOccurrence()
                    /*.Asc()*/

                    .ToList();

                Assert.AreEqual(3, details.Count);

                var firstUpdate = (IHistoricVariableUpdate)details[0];
                Assert.AreEqual(1, firstUpdate.Value);

                var secondUpdate = (IHistoricVariableUpdate)details[1];
                Assert.AreEqual(2, secondUpdate.Value);
                Assert.True(((HistoryEvent)secondUpdate).SequenceCounter > ((HistoryEvent)firstUpdate).SequenceCounter);

                var thirdUpdate = (IHistoricVariableUpdate)details[2];
                Assert.AreEqual(3, thirdUpdate.Value);
                Assert.True(((HistoryEvent)thirdUpdate).SequenceCounter > ((HistoryEvent)secondUpdate).SequenceCounter);
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testProcessDefinitionProperty()
        {
            // given
            var key = "oneTaskProcess";
            var processInstance = runtimeService.StartProcessInstanceByKey(key);

            var ProcessInstanceId = processInstance.Id;
            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;

            runtimeService.SetVariable(ProcessInstanceId, "aVariable", "aValue");
            taskService.SetVariableLocal(taskId, "aLocalVariable", "anotherValue");

            // when (1)
            var instance = historyService.CreateHistoricVariableInstanceQuery(c => c.ProcessInstanceId == ProcessInstanceId).ToList().Where(m=>m.VariableName== "aVariable")
                /*.VariableName("aVariable")*/
                .First();

            // then (1)
            Assert.NotNull(instance.ProcessDefinitionKey);
            Assert.AreEqual(key, instance.ProcessDefinitionKey);

            Assert.NotNull(instance.ProcessDefinitionId);
            Assert.AreEqual(processInstance.ProcessDefinitionId, instance.ProcessDefinitionId);

            Assert.IsNull(instance.CaseDefinitionKey);
            Assert.IsNull(instance.CaseDefinitionId);

            // when (2)
            instance = historyService.CreateHistoricVariableInstanceQuery(c => c.ProcessInstanceId == ProcessInstanceId).ToList().Where(m=>m.VariableName== "aLocalVariable")
                /*.VariableName("aLocalVariable")*/
                .First();

            // then (2)
            Assert.NotNull(instance.ProcessDefinitionKey);
            Assert.AreEqual(key, instance.ProcessDefinitionKey);

            Assert.NotNull(instance.ProcessDefinitionId);
            Assert.AreEqual(processInstance.ProcessDefinitionId, instance.ProcessDefinitionId);

            Assert.IsNull(instance.CaseDefinitionKey);
            Assert.IsNull(instance.CaseDefinitionId);
        }

        [Test]
        public virtual void testStandaloneTaskDefinitionProperties()
        {
            // given
            var taskId = "myTask";
            var task = taskService.NewTask(taskId);
            taskService.SaveTask(task);

            taskService.SetVariable(taskId, "aVariable", "anotherValue");

            // when (1)
            var instance = historyService.CreateHistoricVariableInstanceQuery(c => c.TaskId == taskId)
                /*.VariableName("aVariable")*/
                .First();

            // then (1)
            Assert.IsNull(instance.ProcessDefinitionKey);
            Assert.IsNull(instance.ProcessDefinitionId);
            Assert.IsNull(instance.CaseDefinitionKey);
            Assert.IsNull(instance.CaseDefinitionId);

            taskService.DeleteTask(taskId, true);
        }

        [Test]
        public virtual void testTaskIdProperty()
        {
            // given
            var taskId = "myTask";
            var task = taskService.NewTask(taskId);
            taskService.SaveTask(task);

            taskService.SetVariable(taskId, "aVariable", "anotherValue");

            // when
            var instance = historyService.CreateHistoricVariableInstanceQuery(c => c.TaskId == taskId).ToList().Where(m=>m.VariableName== "aVariable")
                /*.VariableName("aVariable")*/
                .First();

            // then
            Assert.AreEqual(taskId, instance.TaskId);

            taskService.DeleteTask(taskId, true);
        }

        //[Test] //case
        [Deployment("resources/api/cmmn/twoTaskCase.cmmn")]
        public virtual void testQueryByCaseActivityIds()
        {
            // given
            caseService.CreateCaseInstanceByKey("twoTaskCase");

            var caseExecution1 = caseService.CreateCaseExecutionQuery(c => c.ActivityId == "PI_HumanTask_1")
                .First();
            caseService.SetVariableLocal(caseExecution1.Id, "foo", "bar");

            var caseExecution2 = caseService.CreateCaseExecutionQuery(c => c.ActivityId == "PI_HumanTask_2")
                .First();
            caseService.SetVariableLocal(caseExecution2.Id, "bar", "foo");

            // when
            var query = historyService.CreateHistoricVariableInstanceQuery()
                //.CaseActivityIdIn("PI_HumanTask_1", "PI_HumanTask_2")
                ;

            // then
            Assert.AreEqual(2, query.Count());
        }

        [Test]
        public virtual void testQueryByInvalidCaseActivityIds()
        {
            var query = historyService.CreateHistoricVariableInstanceQuery();

            //query.CaseActivityIdIn("invalid");
            Assert.AreEqual(0, query.Count());

            try
            {
                //query.CaseActivityIdIn(null);
                Assert.Fail("A ProcessEngineExcpetion was expected.");
            }
            catch (NullValueException)
            {
            }

            try
            {
                //query.CaseActivityIdIn((string) null);
                Assert.Fail("A ProcessEngineExcpetion was expected.");
            }
            catch (NullValueException)
            {
            }

            try
            {
                string[] values = { "a", null, "b" };
                //query.CaseActivityIdIn(values);
                Assert.Fail("A ProcessEngineExcpetion was expected.");
            }
            catch (NullValueException)
            {
            }
        }

        [Test] //ExecuteJob
        public virtual void testSetVariableInSubProcessStartEventWithEndListener()
        {
            //given
            var topProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("topProcess")
                .StartEvent()
                .CallActivity()
                .CalledElement("subProcess")
                .CamundaIn("executionListenerCounter", "executionListenerCounter")
                .EndEvent()
                .Done();

            var subProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("subProcess")
                .StartEvent()
                .CamundaAsyncBefore()
                .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd,
                    "history.SubProcessActivityStartListener")
                .EndEvent()
                .Done();
            var deployment = repositoryService.CreateDeployment()
                .AddModelInstance("process.bpmn", topProcess)
                .AddModelInstance("subProcess.bpmn", subProcess)
                .Deploy();

            //when
            runtimeService.StartProcessInstanceByKey("topProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("executionListenerCounter", 1));
            managementService.ExecuteJob(managementService.CreateJobQuery()
                //.Active()
                .First()
                .Id);

            //then
            Assert.That(historyService.CreateHistoricVariableInstanceQuery()
                .Count(), Is.EqualTo(3L));
            repositoryService.DeleteDeployment(deployment.Id, true);
        }

        [Test] //Async
        public virtual void testSetVariableInEndListenerOfAsyncStartEvent()
        {
            //given
            //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var subProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process")
                .StartEvent()
                .CamundaAsyncBefore()
                .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd,
                    typeof(SubProcessActivityStartListener).FullName)
                .EndEvent()
                .Done();

            var deployment = repositoryService.CreateDeployment()
                .AddModelInstance("process.bpmn", subProcess)
                .Deploy();

            //when
            runtimeService.StartProcessInstanceByKey("process", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("executionListenerCounter", 1));
            managementService.ExecuteJob(managementService.CreateJobQuery()
                //.Active()
                .First()
                .Id);

            //then
            Assert.That(historyService.CreateHistoricVariableInstanceQuery()
                .Count(), Is.EqualTo(2L));
            repositoryService.DeleteDeployment(deployment.Id, true);
        }

        [Test] //Async
        public virtual void testSetVariableInStartListenerOfAsyncStartEvent()
        {
            //given
            //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var subProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process")
                .StartEvent()
                .CamundaAsyncBefore()
                .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart,
                    typeof(SubProcessActivityStartListener).FullName)
                .EndEvent()
                .Done();

            var deployment = repositoryService.CreateDeployment()
                .AddModelInstance("process.bpmn", subProcess)
                .Deploy();

            //when
            runtimeService.StartProcessInstanceByKey("process", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("executionListenerCounter", 1));
            managementService.ExecuteJob(managementService.CreateJobQuery()
                //.Active()
                .First()
                .Id);

            //then
            Assert.That(historyService.CreateHistoricVariableInstanceQuery()
                .Count(), Is.EqualTo(2L));
            repositoryService.DeleteDeployment(deployment.Id, true);
        }

        [Test] //Async
        [Deployment("resources/bpmn/async/AsyncStartEventTest.TestAsyncStartEvent.bpmn20.xml")]
        public virtual void testAsyncStartEventHistory()
        {
            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                runtimeService.StartProcessInstanceByKey("asyncStartEvent");

                var historicInstance = historyService.CreateHistoricProcessInstanceQuery()
                    .First();
                Assert.NotNull(historicInstance);
                Assert.NotNull(historicInstance.StartTime);

                var historicStartEvent = historyService.CreateHistoricActivityInstanceQuery()
                    .FirstOrDefault();
                Assert.IsNull(historicStartEvent);
            }
        }

        [Test] //Async
        [Deployment("resources/bpmn/async/AsyncStartEventTest.TestAsyncStartEvent.bpmn20.xml")]
        public virtual void testAsyncStartEventVariableHistory()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["foo"] = "bar";
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("asyncStartEvent", variables)
                .Id;

            var variableFoo = runtimeService.CreateVariableInstanceQuery()
                .FirstOrDefault();
            Assert.NotNull(variableFoo);
            Assert.AreEqual("foo", variableFoo.Name);
            Assert.AreEqual("bar", variableFoo.Value);

            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
                .Count());

            ExecuteAvailableJobs();

            var task = taskService.CreateTaskQuery()
                .FirstOrDefault();
            Assert.NotNull(task);

            taskService.Complete(task.Id);

            // Assert process instance is ended
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
                .Count());

            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelActivity)
            {
                var variable = historyService.CreateHistoricVariableInstanceQuery()
                    .First();
                Assert.NotNull(variable);
                Assert.AreEqual("foo", variable.Name);
                Assert.AreEqual("bar", variable.Value);
                Assert.AreEqual(ProcessInstanceId, variable.ActivityInstanceId);

                if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelAudit)
                {
                    var startEventId = historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityId == "startEvent")
                        .First()
                        .Id;
                    var hds = historyService.CreateHistoricDetailQuery().ToList();
                    var historicDetail = historyService.CreateHistoricDetailQuery()
                        .First();

                    Assert.AreEqual(startEventId, historicDetail.ActivityInstanceId);
                }
            }
        }

        [Test] //Async
        [Deployment("resources/bpmn/async/AsyncStartEventTest.TestMultipleAsyncStartEvents.bpmn20.xml")]
        public virtual void testMultipleAsyncStartEventsVariableHistory()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["foo"] = "bar";
            runtimeService.CorrelateMessage("newInvoiceMessage", new Dictionary<string, object>(), variables);

            var variableFoo = runtimeService.CreateVariableInstanceQuery()
                .First();
            Assert.NotNull(variableFoo);
            Assert.AreEqual("foo", variableFoo.Name);
            Assert.AreEqual("bar", variableFoo.Value);

            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
                .Count());

            ExecuteAvailableJobs();

            var task = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(task);
            taskService.Complete(task.Id);

            // Assert process instance is ended
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
                .Count());

            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelActivity)
            {
                var ProcessInstanceId = historyService.CreateHistoricProcessInstanceQuery()
                    .First()
                    .Id;

                var variable = historyService.CreateHistoricVariableInstanceQuery()
                    .First();
                Assert.NotNull(variable);
                Assert.AreEqual("foo", variable.Name);
                Assert.AreEqual("bar", variable.Value);
                Assert.AreEqual(ProcessInstanceId, variable.ActivityInstanceId);

                if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelAudit)
                {
                    var theStartActivityInstanceId = historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityId == "messageStartEvent")
                        .First()
                        .Id;

                    var historicDetail = historyService.CreateHistoricDetailQuery()
                        .First();

                    Assert.AreEqual(theStartActivityInstanceId, historicDetail.ActivityInstanceId);
                }
            }
        }

        [Test] //Async
        public virtual void testAsyncStartEventWithAddedVariable()
        {
            // given a process definition with asynchronous start event
            Deployment(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("testProcess")
                .StartEvent()
                .CamundaAsyncBefore()
                .EndEvent()
                .Done());

            // when create an instance with a variable
            var processInstance = runtimeService.StartProcessInstanceByKey("testProcess",
                Variables.PutValue("var1", "foo"));

            // and add a variable before the instance is created
            runtimeService.SetVariable(processInstance.Id, "var2", "bar");

            ExecuteAvailableJobs();

            AssertProcessEnded(processInstance.Id);

            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelActivity)
            {
                // then the history contains one entry for each variable
                var query = historyService.CreateHistoricVariableInstanceQuery();
                Assert.AreEqual(query.Count(), 2);

                var firstVariable = query.Where(m=>m.Name=="var1")/*.VariableName("var1")*/
                    .First();
                Assert.NotNull(firstVariable);
                Assert.AreEqual(firstVariable.Value, "foo");
                Assert.NotNull(firstVariable.ActivityInstanceId);

                var secondVariable = query.Where(m => m.Name == "var2")/*.VariableName("var2")*/
                    .First();
                Assert.NotNull(secondVariable);
                Assert.AreEqual(secondVariable.Value, "bar");
                Assert.NotNull(secondVariable.ActivityInstanceId);
            }
        }


        [Test] //Async
        public virtual void testAsyncStartEventWithChangedVariable()
        {
            // given a process definition with asynchronous start event
            Deployment(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("testProcess")
                .StartEvent()
                .CamundaAsyncBefore()
                .EndEvent()
                .Done());

            // when create an instance with a variable
            var processInstance = runtimeService.StartProcessInstanceByKey("testProcess",
                Variables.PutValue("var", "foo"));

            // and update this variable before the instance is created
            runtimeService.SetVariable(processInstance.Id, "var", "bar");

            ExecuteAvailableJobs();

            AssertProcessEnded(processInstance.Id);

            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelActivity)
            {
                // then the history contains only one entry for the latest update (value = "bar")
                // - the entry for the initial value (value = "foo") is lost because of current limitations
                var query = historyService.CreateHistoricVariableInstanceQuery();
                Assert.AreEqual(query.Count(), 1);

                var variable = query.First();
                Assert.AreEqual(variable.Value, "bar");
                Assert.NotNull(variable.ActivityInstanceId);
            }
        }
        [Test] //Async
        [Deployment("resources/bpmn/async/AsyncStartEventTest.TestAsyncStartEvent.bpmn20.xml")]
        public virtual void testSubmitForm()
        {
            var processDefinitionId = repositoryService.CreateProcessDefinitionQuery(c => c.Key == "asyncStartEvent")
                .First()
                .Id;

            IDictionary<string, object> properties = new Dictionary<string, object>();
            properties["foo"] = "bar";

            formService.SubmitStartForm(processDefinitionId, properties);

            var variableFoo = runtimeService.CreateVariableInstanceQuery()
                .First();
            Assert.NotNull(variableFoo);
            Assert.AreEqual("foo", variableFoo.Name);
            Assert.AreEqual("bar", variableFoo.Value);

            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
                .Count());

            ExecuteAvailableJobs();

            var task = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(task);
            taskService.Complete(task.Id);

            // Assert process instance is ended
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
                .Count());

            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelActivity)
            {
                var ProcessInstanceId = historyService.CreateHistoricProcessInstanceQuery()
                    .First()
                    .Id;

                var variable = historyService.CreateHistoricVariableInstanceQuery()
                    .First();
                Assert.NotNull(variable);
                Assert.AreEqual("foo", variable.Name);
                Assert.AreEqual("bar", variable.Value);
                Assert.AreEqual(ProcessInstanceId, variable.ActivityInstanceId);

                if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelAudit)
                {
                    var theStartActivityInstanceId = historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityId == "startEvent")
                        .First()
                        .Id;

                    var historicFormUpdate = (IHistoricFormField)historyService.CreateHistoricDetailQuery()
                        //.FormFields()
                        .First();

                    Assert.NotNull(historicFormUpdate);
                    Assert.AreEqual("bar", historicFormUpdate.FieldValue);

                    var historicVariableUpdate = (IHistoricVariableUpdate)historyService.CreateHistoricDetailQuery()
                        /*.VariableUpdates()*/
                        .First();

                    Assert.NotNull(historicVariableUpdate);
                    Assert.AreEqual(theStartActivityInstanceId, historicVariableUpdate.ActivityInstanceId);
                    Assert.AreEqual("bar", historicVariableUpdate.Value);
                }
            }
        }

        /// <summary>
        ///     CAM-2828
        /// </summary>
        //[Test]
        [Deployment("resources/bpmn/async/AsyncStartEventTest.TestAsyncStartEvent.bpmn20.xml")]
        public virtual void FAILING_testSubmitFormHistoricUpdates()
        {
            var processDefinitionId = repositoryService.CreateProcessDefinitionQuery(c => c.Key == "asyncStartEvent")
                .First()
                .Id;

            IDictionary<string, object> properties = new Dictionary<string, object>();
            properties["foo"] = "bar";

            formService.SubmitStartForm(processDefinitionId, properties);
            ExecuteAvailableJobs();

            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelAudit)
            {
                var theStartActivityInstanceId = historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityId == "startEvent")
                    .First()
                    .Id;

                var historicFormUpdate = historyService.CreateHistoricDetailQuery()
                    //.FormFields()
                    .First();

                Assert.NotNull(historicFormUpdate);
                Assert.AreEqual(theStartActivityInstanceId, historicFormUpdate.ActivityInstanceId);
            }
        }

        //[Test] case未实现
        [Deployment("resources/api/cmmn/oneTaskCase.cmmn")]
        public virtual void testCaseDefinitionProperty()
        {
            // given
            var key = "oneTaskCase";
            var caseInstance = caseService.CreateCaseInstanceByKey(key);

            var caseInstanceId = caseInstance.Id;

            var humanTask = caseService.CreateCaseExecutionQuery(c => c.ActivityId == "PI_HumanTask_1")
                .First()
                .Id;
            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;

            caseService.SetVariable(caseInstanceId, "aVariable", "aValue");
            taskService.SetVariableLocal(taskId, "aLocalVariable", "anotherValue");

            // when (1)
            var instance = historyService.CreateHistoricVariableInstanceQuery()
                //.CaseInstanceId(caseInstanceId)
                /*.VariableName("aVariable")*/
                .First();

            // then (1)
            Assert.NotNull(instance.CaseDefinitionKey);
            Assert.AreEqual(key, instance.CaseDefinitionKey);

            Assert.NotNull(instance.CaseDefinitionId);
            Assert.AreEqual(caseInstance.CaseDefinitionId, instance.CaseDefinitionId);

            Assert.IsNull(instance.ProcessDefinitionKey);
            Assert.IsNull(instance.ProcessDefinitionId);

            // when (2)
            instance = historyService.CreateHistoricVariableInstanceQuery(m=>m.CaseInstanceId==caseInstanceId)
                //.CaseInstanceId(caseInstanceId)
                /*.VariableName("aLocalVariable")*/
                .First();

            // then (2)
            Assert.NotNull(instance.CaseDefinitionKey);
            Assert.AreEqual(key, instance.CaseDefinitionKey);

            Assert.NotNull(instance.CaseDefinitionId);
            Assert.AreEqual(caseInstance.CaseDefinitionId, instance.CaseDefinitionId);

            Assert.IsNull(instance.ProcessDefinitionKey);
            Assert.IsNull(instance.ProcessDefinitionId);
        }

        //[Test] //case未实现
        [Deployment("resources/api/cmmn/oneTaskCase.cmmn")]
        public virtual void testCaseVariableUpdateOrder()
        {
            // given:
            var caseInstanceId = caseService.CreateCaseInstanceByKey("oneTaskCase")
                .Id;

            // when (1)
            caseService.SetVariable(caseInstanceId, "myVariable", 1);
            caseService.SetVariable(caseInstanceId, "myVariable", 2);
            caseService.SetVariable(caseInstanceId, "myVariable", 3);

            // then (1)
            var variable = historyService.CreateHistoricVariableInstanceQuery()
                .First();
            Assert.NotNull(variable);

            var variableInstanceId = variable.Id;

            if (FullHistoryEnabled)
            {
                var details = historyService.CreateHistoricDetailQuery< HistoricVariableUpdateEventEntity>(m=>m.VariableInstanceId== variableInstanceId).OrderBy(m=>m.SequenceCounter)
                    ////////.VariableInstanceId(variableInstanceId)
                    ////.OrderPartiallyByOccurrence()
                    /*.Asc()*/

                    .ToList();

                Assert.AreEqual(3, details.Count);

                var firstUpdate = (IHistoricVariableUpdate)details[0];
                Assert.AreEqual(1, firstUpdate.Value);

                var secondUpdate = (IHistoricVariableUpdate)details[1];
                Assert.AreEqual(2, secondUpdate.Value);
                Assert.True(((HistoryEvent)secondUpdate).SequenceCounter > ((HistoryEvent)firstUpdate).SequenceCounter);

                var thirdUpdate = (IHistoricVariableUpdate)details[2];
                Assert.AreEqual(3, thirdUpdate.Value);
                Assert.True(((HistoryEvent)thirdUpdate).SequenceCounter > ((HistoryEvent)secondUpdate).SequenceCounter);
            }

            // when (2)
            caseService.SetVariable(caseInstanceId, "myVariable", "abc");

            // then (2)
            variable = historyService.CreateHistoricVariableInstanceQuery()
                .First();
            Assert.NotNull(variable);

            if (FullHistoryEnabled)
            {
                var details = historyService.CreateHistoricDetailQuery< HistoricVariableUpdateEventEntity>(m=>m.VariableInstanceId== variableInstanceId).OrderBy(m=>m.SequenceCounter)
                    ////.VariableInstanceId(variableInstanceId)
                    //.OrderPartiallyByOccurrence()
                    /*.Asc()*/

                    .ToList();

                Assert.AreEqual(4, details.Count);

                var firstUpdate = (IHistoricVariableUpdate)details[0];
                Assert.AreEqual(1, firstUpdate.Value);

                var secondUpdate = (IHistoricVariableUpdate)details[1];
                Assert.AreEqual(2, secondUpdate.Value);
                Assert.True(((HistoryEvent)secondUpdate).SequenceCounter > ((HistoryEvent)firstUpdate).SequenceCounter);

                var thirdUpdate = (IHistoricVariableUpdate)details[2];
                Assert.AreEqual(3, thirdUpdate.Value);
                Assert.True(((HistoryEvent)thirdUpdate).SequenceCounter > ((HistoryEvent)secondUpdate).SequenceCounter);

                var fourthUpdate = (IHistoricVariableUpdate)details[3];
                Assert.AreEqual("abc", fourthUpdate.Value);
                Assert.True(((HistoryEvent)fourthUpdate).SequenceCounter > ((HistoryEvent)thirdUpdate).SequenceCounter);
            }

            // when (3)
            caseService.RemoveVariable(caseInstanceId, "myVariable");

            // then (3)
            variable = historyService.CreateHistoricVariableInstanceQuery()
                .First();
            Assert.IsNull(variable);

            if (FullHistoryEnabled)
            {
                var details = historyService.CreateHistoricDetailQuery< HistoricVariableUpdateEventEntity>(m=>m.VariableInstanceId== variableInstanceId).OrderBy(m=>m.SequenceCounter)
                    ////.VariableInstanceId(variableInstanceId)
                    //.OrderPartiallyByOccurrence()
                    /*.Asc()*/

                    .ToList();

                Assert.AreEqual(5, details.Count);

                var firstUpdate = (IHistoricVariableUpdate)details[0];
                Assert.AreEqual(1, firstUpdate.Value);

                var secondUpdate = (IHistoricVariableUpdate)details[1];
                Assert.AreEqual(2, secondUpdate.Value);
                Assert.True(((HistoryEvent)secondUpdate).SequenceCounter > ((HistoryEvent)firstUpdate).SequenceCounter);

                var thirdUpdate = (IHistoricVariableUpdate)details[2];
                Assert.AreEqual(3, thirdUpdate.Value);
                Assert.True(((HistoryEvent)thirdUpdate).SequenceCounter > ((HistoryEvent)secondUpdate).SequenceCounter);

                var fourthUpdate = (IHistoricVariableUpdate)details[3];
                Assert.AreEqual("abc", fourthUpdate.Value);
                Assert.True(((HistoryEvent)fourthUpdate).SequenceCounter > ((HistoryEvent)thirdUpdate).SequenceCounter);

                var fifthUpdate = (IHistoricVariableUpdate)details[4];
                Assert.IsNull(fifthUpdate.Value);
                Assert.True(((HistoryEvent)fifthUpdate).SequenceCounter > ((HistoryEvent)fourthUpdate).SequenceCounter);
            }
        }

        [Test]
        [Deployment]
        public virtual void testForkParallelGatewayTreeCompaction()
        {
            // given
            runtimeService.StartProcessInstanceByKey("process");

            var task1 = taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "task1")
                .First();

            var task2Execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "task2")
                .First();

            // when
            runtimeService.SetVariableLocal(task2Execution.Id, "foo", "bar");
            taskService.Complete(task1.Id);

            // then
            Assert.AreEqual(1, runtimeService.CreateVariableInstanceQuery()
                .Count());

            var historicVariable = historyService.CreateHistoricVariableInstanceQuery()
                .First();
            Assert.NotNull(historicVariable);
            Assert.AreEqual("foo", historicVariable.Name);
        }

        // Test for ACT-1528, which (correctly) reported that deleting any
        // historic process instance would remove Permissions.All historic variables.
        // Yes. Real serious bug.
        [Test]
        [Deployment]
        public virtual void testHistoricProcessInstanceDeleteCascadesCorrectly()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["var1"] = "value1";
            variables["var2"] = "value2";
            var processInstance = runtimeService.StartProcessInstanceByKey("myProcess", variables);
            Assert.NotNull(processInstance);

            variables = new Dictionary<string, object>();
            variables["var3"] = "value3";
            variables["var4"] = "value4";
            var processInstance2 = runtimeService.StartProcessInstanceByKey("myProcess", variables);
            Assert.NotNull(processInstance2);

            // check variables
            var Count = historyService.CreateHistoricVariableInstanceQuery()
                .Count();
            Assert.AreEqual(4, Count);

            // Delete runtime execution of ONE process instance
            runtimeService.DeleteProcessInstance(processInstance.Id, "reason 1");
            historyService.DeleteHistoricProcessInstance(processInstance.Id);

            // recheck variables
            // this is a bug: all variables was deleted after Delete a history processinstance
            Count = historyService.CreateHistoricVariableInstanceQuery()
                .Count();
            Assert.AreEqual(2, Count);
        }

        [Test]
        [Deployment]
        public virtual void testHistoricVariableInstanceRevision()
        {
            // given:
            // a finished process instance
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            AssertProcessEnded(processInstance.Id);

            // when

            // then
            var variable = historyService.CreateHistoricVariableInstanceQuery()
                .First();

            Assert.NotNull(variable);

            var variableEntity = (HistoricVariableInstanceEntity)variable;

            // the revision has to be 0
            Assert.AreEqual(0, variableEntity.Revision);

            if (FullHistoryEnabled)
            {
                var details = historyService.CreateHistoricDetailQuery()
                    /*.OrderByVariableRevision()*/
                    /*.Asc()*/

                    .ToList();

                foreach (var detail in details)
                {
                    var variableDetail = (IHistoricVariableUpdate)detail;
                    Assert.AreEqual(0, variableDetail.Revision);
                }
            }
        }

        [Test]
        [Deployment]
        public virtual void testHistoricVariableInstanceRevisionAsync()
        {
            // given:
            // a finished process instance
            var processInstance = runtimeService.StartProcessInstanceByKey("process");

            // when
            ExecuteAvailableJobs();

            // then
            AssertProcessEnded(processInstance.Id);

            var variable = historyService.CreateHistoricVariableInstanceQuery()
                .FirstOrDefault();

            Assert.NotNull(variable);

            var variableEntity = (HistoricVariableInstanceEntity)variable;

            // the revision has to be 2
            Assert.AreEqual(2, variableEntity.Revision);

            if (FullHistoryEnabled)
            {
                var details = historyService.CreateHistoricDetailQuery()
                    /*.OrderByVariableRevision()*/
                    /*.Asc()*/

                    .ToList();

                var i = 0;
                foreach (var detail in details)
                {
                    var variableDetail = (IHistoricVariableUpdate)detail;
                    Assert.AreEqual(i, variableDetail.Revision);
                    i++;
                }
            }
        }

        /// <summary>
        ///     CAM-3442
        /// </summary>
        [Test]//ESS.FW.Bpm.Engine.Impl.Javax.EL.PropertyNotFoundException : error.identifier.property.notfound
        [Deployment]
        public virtual void testImplicitVariableUpdate()
        {
            var instance = runtimeService.StartProcessInstanceByKey("serviceTaskProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("listVar", new List<string>())
                .PutValue("delegate", new UpdateValueDelegate()));

            var list = (IList<string>)runtimeService.GetVariable(instance.Id, "listVar");
            Assert.NotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(UpdateValueDelegate.NEW_ELEMENT, list[0]);

            var historicVariableInstance = historyService.CreateHistoricVariableInstanceQuery(m=>m.SerializerName== "listVar")
                /*.VariableName("listVar")*/
                .First();

            var historicList = (IList<string>)historicVariableInstance.Value;
            Assert.NotNull(historicList);
            Assert.AreEqual(1, historicList.Count);
            Assert.AreEqual(UpdateValueDelegate.NEW_ELEMENT, historicList[0]);

            if (FullHistoryEnabled)
            {
                var historicDetails = historyService.CreateHistoricDetailQuery<HistoricVariableUpdateEventEntity>(m => m.VariableInstanceId == historicVariableInstance.Id).OrderBy(m => m.SequenceCounter)
                    /*.VariableUpdates()*/
                    ////.VariableInstanceId(historicVariableInstance.Id)
                    //.OrderPartiallyByOccurrence()
                    /*.Asc()*/

                    .ToList();

                Assert.AreEqual(2, historicDetails.Count);

                var update1 = (IHistoricVariableUpdate)historicDetails[0];
                var update2 = (IHistoricVariableUpdate)historicDetails[1];

                var value1 = (IList<string>)update1.Value;

                Assert.NotNull(value1);
                Assert.True(value1.Count == 0);

                var value2 = (IList<string>)update2.Value;

                Assert.NotNull(value2);
                Assert.AreEqual(1, value2.Count);
                Assert.AreEqual(UpdateValueDelegate.NEW_ELEMENT, value2[0]);
            }
        }

        [Test]
        [Deployment]
        public virtual void testJoinInclusiveGatewayLocalVariableOnLastJoiningExecution()
        {
            // when
            runtimeService.StartProcessInstanceByKey("process");

            // then
            Assert.AreEqual(0, runtimeService.CreateVariableInstanceQuery()
                .Count());

            var historicVariable = historyService.CreateHistoricVariableInstanceQuery()
                .First();
            Assert.NotNull(historicVariable);
            Assert.AreEqual("testVar", historicVariable.Name);
        }

        [Test]
        [Deployment]
        public virtual void testJoinParallelGatewayLocalVariableOnLastJoiningExecution()
        {
            // when
            runtimeService.StartProcessInstanceByKey("process");

            // then
            Assert.AreEqual(0, runtimeService.CreateVariableInstanceQuery()
                .Count());

            var historicVariable = historyService.CreateHistoricVariableInstanceQuery()
                .First();
            Assert.NotNull(historicVariable);
            Assert.AreEqual("testVar", historicVariable.Name);
        }

        [Test]
        [Deployment]
        public virtual void testNestedForkParallelGatewayTreeCompaction()
        {
            // given
            runtimeService.StartProcessInstanceByKey("process");

            var task1 = taskService.CreateTaskQuery(c => c.TaskDefinitionKey== "task1")
                .First();

            var task2Execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "task2")
                .First();

            // when
            runtimeService.SetVariableLocal(task2Execution.Id, "foo", "bar");
            taskService.Complete(task1.Id);

            // then
            Assert.AreEqual(1, runtimeService.CreateVariableInstanceQuery()
                .Count());

            var historicVariable = historyService.CreateHistoricVariableInstanceQuery()
                .First();
            Assert.NotNull(historicVariable);
            Assert.AreEqual("foo", historicVariable.Name);
        }

        [Test]
        [Deployment]
        public virtual void testNestedJoinInclusiveGatewayLocalVariableOnLastJoiningExecution()
        {
            // when
            runtimeService.StartProcessInstanceByKey("process");

            // then
            Assert.AreEqual(0, runtimeService.CreateVariableInstanceQuery()
                .Count());

            var historicVariable = historyService.CreateHistoricVariableInstanceQuery()
                .First();
            Assert.NotNull(historicVariable);
            Assert.AreEqual("testVar", historicVariable.Name);
        }

        [Test]
        [Deployment]
        public virtual void testNestedJoinParallelGatewayLocalVariableOnLastJoiningExecution()
        {
            // when
            runtimeService.StartProcessInstanceByKey("process");

            // then
            Assert.AreEqual(0, runtimeService.CreateVariableInstanceQuery()
                .Count());

            var historicVariable = historyService.CreateHistoricVariableInstanceQuery()
                .First();
            Assert.NotNull(historicVariable);
            Assert.AreEqual("testVar", historicVariable.Name);
        }

        [Test]
        [Deployment]
        public virtual void testNoImplicitUpdateOnHistoricValues()
        {
            //given
            runtimeService.StartProcessInstanceByKey("serviceTaskProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("listVar", new List<string>())
                .PutValue("delegate", new UpdateHistoricValueDelegate()));

            // a task before the delegate ensures that the variables have actually been persisted
            // and can be fetched by querying
            var task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);

            // then
            var historicVariableInstance = historyService.CreateHistoricVariableInstanceQuery()
                /*.VariableName("listVar")*/
                .First();

            var historicList = (IList<string>)historicVariableInstance.Value;
            Assert.NotNull(historicList);
            Assert.AreEqual(0, historicList.Count);

            if (FullHistoryEnabled)
            {
                Assert.AreEqual(2, historyService.CreateHistoricDetailQuery()
                    .Count());

                var historicDetails = historyService.CreateHistoricDetailQuery()
                    /*.VariableUpdates()*/
                    ////.VariableInstanceId(historicVariableInstance.Id)

                    .ToList();

                Assert.AreEqual(1, historicDetails.Count);

                var update1 = (IHistoricVariableUpdate)historicDetails[0];

                var value1 = (IList<string>)update1.Value;

                Assert.NotNull(value1);
                Assert.True(value1.Count == 0);
            }
        }

        //[Test]//: .net不支持groovy语法,
        [Deployment]
        public virtual void testParallel()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("myProc");
            var taskQuery = taskService.CreateTaskQuery();
            var userTask = taskQuery.First();
            Assert.AreEqual("userTask1", userTask.Name);

            taskService.Complete(userTask.Id, new Dictionary<string, object> { { "myVar", "test789" } });

            AssertProcessEnded(processInstance.Id);

            var variables = historyService.CreateHistoricVariableInstanceQuery()
                /*.OrderByVariableName()*/
                /*.Asc()*/

                .ToList();
            Assert.AreEqual(2, variables.Count);

            var historicVariable = (HistoricVariableInstanceEntity)variables[0];
            Assert.AreEqual("myVar", historicVariable.Name);
            Assert.AreEqual("test789", historicVariable.TextValue);

            var historicVariable1 = (HistoricVariableInstanceEntity)variables[1];
            Assert.AreEqual("myVar1", historicVariable1.Name);
            Assert.AreEqual("test456", historicVariable1.TextValue);

            Assert.AreEqual(8, historyService.CreateHistoricActivityInstanceQuery()
                .Count());

            if (FullHistoryEnabled)
                Assert.AreEqual(5, historyService.CreateHistoricDetailQuery()
                    .Count());
        }

        //[Test]//: .net不支持groovy语法,
        [Deployment]
        public virtual void testParallelNoWaitState()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("myProc");
            AssertProcessEnded(processInstance.Id);

            var variables = historyService.CreateHistoricVariableInstanceQuery()

                .ToList();
            Assert.AreEqual(1, variables.Count);

            var historicVariable = (HistoricVariableInstanceEntity)variables[0];
            Assert.AreEqual("test456", historicVariable.TextValue);

            Assert.AreEqual(7, historyService.CreateHistoricActivityInstanceQuery()
                .Count());

            if (FullHistoryEnabled)
                Assert.AreEqual(2, historyService.CreateHistoricDetailQuery()
                    .Count());
        }

        [Test]
        [Deployment("resources/api/cmmn/oneTaskCase.cmmn")]
        public virtual void testQueryByCaseActivityId()
        {
            // given
            caseService.CreateCaseInstanceByKey("oneTaskCase", Variables.PutValue("foo", "bar"));

            var caseExecution = caseService.CreateCaseExecutionQuery(c => c.ActivityId == "PI_HumanTask_1")
                .First();
            caseService.SetVariableLocal(caseExecution.Id, "bar", "foo");

            // when
            var query = historyService.CreateHistoricVariableInstanceQuery(c => c.ActivityInstanceId == "PI_HumanTask_1");

            // then
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual("bar", query.First()
                .Name);
            Assert.AreEqual("foo", query.First()
                .Value);
        }

        //[Test]//.net不支持groovy语法,
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testSetSameVariableUpdateOrder()
        {
            // given:
            runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;

            // when
            taskService.SetVariable(taskId, "myVariable", 1);
            taskService.SetVariable(taskId, "myVariable", 1);
            taskService.SetVariable(taskId, "myVariable", 2);

            // then
            var variable = historyService.CreateHistoricVariableInstanceQuery()
                .First();
            Assert.NotNull(variable);

            var variableInstanceId = variable.Id;

            if (FullHistoryEnabled)
            {
                var details = historyService.CreateHistoricDetailQuery()
                    ////.VariableInstanceId(variableInstanceId)
                    //.OrderPartiallyByOccurrence()
                    /*.Asc()*/

                    .ToList();

                Assert.AreEqual(3, details.Count);

                var firstUpdate = (IHistoricVariableUpdate)details[0];
                Assert.AreEqual(1, firstUpdate.Value);

                var secondUpdate = (IHistoricVariableUpdate)details[1];
                Assert.AreEqual(1, secondUpdate.Value);
                Assert.True(((HistoryEvent)secondUpdate).SequenceCounter > ((HistoryEvent)firstUpdate).SequenceCounter);

                var thirdUpdate = (IHistoricVariableUpdate)details[2];
                Assert.AreEqual(2, thirdUpdate.Value);
                Assert.True(((HistoryEvent)thirdUpdate).SequenceCounter > ((HistoryEvent)secondUpdate).SequenceCounter);
            }
        }

        //[Test]//.net不支持groovy语法,
        [Deployment]
        public virtual void testSimple()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("myProc");
            var taskQuery = taskService.CreateTaskQuery();
            var userTask = taskQuery.First();
            Assert.AreEqual("userTask1", userTask.Name);

            taskService.Complete(userTask.Id, new Dictionary<string, object> { { "myVar", "test789" } });

            AssertProcessEnded(processInstance.Id);

            var variables = historyService.CreateHistoricVariableInstanceQuery()

                .ToList();
            Assert.AreEqual(1, variables.Count);

            var historicVariable = (HistoricVariableInstanceEntity)variables[0];
            Assert.AreEqual("test456", historicVariable.TextValue);

            Assert.AreEqual(5, historyService.CreateHistoricActivityInstanceQuery()
                .Count());

            if (FullHistoryEnabled)
                Assert.AreEqual(3, historyService.CreateHistoricDetailQuery()
                    .Count());
        }

        //[Test] : .net不支持groovy语法,
        [Deployment]
        public virtual void testSimpleNoWaitState()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("myProc");
            AssertProcessEnded(processInstance.Id);

            var variables = historyService.CreateHistoricVariableInstanceQuery()

                .ToList();
            Assert.AreEqual(1, variables.Count);

            var historicVariable = (HistoricVariableInstanceEntity)variables[0];
            Assert.AreEqual("test456", historicVariable.TextValue);

            Assert.AreEqual(4, historyService.CreateHistoricActivityInstanceQuery()
                .Count());

            if (FullHistoryEnabled)
                Assert.AreEqual(2, historyService.CreateHistoricDetailQuery()
                    .Count());
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testTaskVariableUpdateOrder()
        {
            // given:
            runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;

            // when (1)
            taskService.SetVariableLocal(taskId, "myVariable", 1);
            taskService.SetVariableLocal(taskId, "myVariable", 2);
            taskService.SetVariableLocal(taskId, "myVariable", 3);

            // then (1)
            var variable = historyService.CreateHistoricVariableInstanceQuery()
                .First();
            Assert.NotNull(variable);

            var variableInstanceId = variable.Id;

            if (FullHistoryEnabled)
            {
                var details = historyService.CreateHistoricDetailQuery<HistoricVariableUpdateEventEntity>(m=>m.VariableInstanceId== variableInstanceId).OrderBy(m=>m.SequenceCounter)
                    ////.VariableInstanceId(variableInstanceId)
                    //.OrderPartiallyByOccurrence()
                    /*.Asc()*/

                    .ToList();

                Assert.AreEqual(3, details.Count);

                var firstUpdate = (IHistoricVariableUpdate)details[0];
                Assert.AreEqual(1, firstUpdate.Value);

                var secondUpdate = (IHistoricVariableUpdate)details[1];
                Assert.AreEqual(2, secondUpdate.Value);
                Assert.True(((HistoryEvent)secondUpdate).SequenceCounter > ((HistoryEvent)firstUpdate).SequenceCounter);

                var thirdUpdate = (IHistoricVariableUpdate)details[2];
                Assert.AreEqual(3, thirdUpdate.Value);
                Assert.True(((HistoryEvent)thirdUpdate).SequenceCounter > ((HistoryEvent)secondUpdate).SequenceCounter);
            }

            // when (2)
            taskService.SetVariableLocal(taskId, "myVariable", "abc");

            // then (2)
            variable = historyService.CreateHistoricVariableInstanceQuery()
                .First();
            Assert.NotNull(variable);

            if (FullHistoryEnabled)
            {
                var details = historyService.CreateHistoricDetailQuery()
                    ////.VariableInstanceId(variableInstanceId)
                    //.OrderPartiallyByOccurrence()
                    /*.Asc()*/

                    .ToList();

                Assert.AreEqual(4, details.Count);

                var firstUpdate = (IHistoricVariableUpdate)details[0];
                Assert.AreEqual(1, firstUpdate.Value);

                var secondUpdate = (IHistoricVariableUpdate)details[1];
                Assert.AreEqual(2, secondUpdate.Value);
                Assert.True(((HistoryEvent)secondUpdate).SequenceCounter > ((HistoryEvent)firstUpdate).SequenceCounter);

                var thirdUpdate = (IHistoricVariableUpdate)details[2];
                Assert.AreEqual(3, thirdUpdate.Value);
                Assert.True(((HistoryEvent)thirdUpdate).SequenceCounter > ((HistoryEvent)secondUpdate).SequenceCounter);

                var fourthUpdate = (IHistoricVariableUpdate)details[3];
                Assert.AreEqual("abc", fourthUpdate.Value);
                Assert.True(((HistoryEvent)fourthUpdate).SequenceCounter > ((HistoryEvent)thirdUpdate).SequenceCounter);
            }

            // when (3)
            taskService.RemoveVariable(taskId, "myVariable");

            // then (3)
            variable = historyService.CreateHistoricVariableInstanceQuery()
                .First();
            Assert.IsNull(variable);

            if (FullHistoryEnabled)
            {
                var details = historyService.CreateHistoricDetailQuery()
                    ////.VariableInstanceId(variableInstanceId)
                    //.OrderPartiallyByOccurrence()
                    /*.Asc()*/

                    .ToList();

                Assert.AreEqual(5, details.Count);

                var firstUpdate = (IHistoricVariableUpdate)details[0];
                Assert.AreEqual(1, firstUpdate.Value);

                var secondUpdate = (IHistoricVariableUpdate)details[1];
                Assert.AreEqual(2, secondUpdate.Value);
                Assert.True(((HistoryEvent)secondUpdate).SequenceCounter > ((HistoryEvent)firstUpdate).SequenceCounter);

                var thirdUpdate = (IHistoricVariableUpdate)details[2];
                Assert.AreEqual(3, thirdUpdate.Value);
                Assert.True(((HistoryEvent)thirdUpdate).SequenceCounter > ((HistoryEvent)secondUpdate).SequenceCounter);

                var fourthUpdate = (IHistoricVariableUpdate)details[3];
                Assert.AreEqual("abc", fourthUpdate.Value);
                Assert.True(((HistoryEvent)fourthUpdate).SequenceCounter > ((HistoryEvent)thirdUpdate).SequenceCounter);

                var fifthUpdate = (IHistoricVariableUpdate)details[4];
                Assert.IsNull(fifthUpdate.Value);
                Assert.True(((HistoryEvent)fifthUpdate).SequenceCounter > ((HistoryEvent)fourthUpdate).SequenceCounter);
            }
        }

        [Test]
        [Deployment]
        public virtual void testTwoSubProcessInParallelWithinSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("twoSubProcessInParallelWithinSubProcess");
            AssertProcessEnded(processInstance.Id);

            var variables = historyService.CreateHistoricVariableInstanceQuery()
                /*.OrderByVariableName()*/
                /*.Asc()*/

                .ToList();
            Assert.AreEqual(2, variables.Count);

            var historicVariable = (HistoricVariableInstanceEntity)variables[0];
            Assert.AreEqual("myVar", historicVariable.Name);
            Assert.AreEqual("test101112", historicVariable.TextValue);
            Assert.AreEqual("string", historicVariable.VariableTypeName);
            Assert.AreEqual("string", historicVariable.TypeName);

            var historicVariable1 = (HistoricVariableInstanceEntity)variables[1];
            Assert.AreEqual("myVar1", historicVariable1.Name);
            Assert.AreEqual("test789", historicVariable1.TextValue);
            Assert.AreEqual("string", historicVariable1.VariableTypeName);
            Assert.AreEqual("string", historicVariable1.TypeName);

            Assert.AreEqual(18, historyService.CreateHistoricActivityInstanceQuery()
                .Count());

            if (FullHistoryEnabled)
                Assert.AreEqual(7, historyService.CreateHistoricDetailQuery()
                    .Count());
        }
    }
    [Serializable]
    public class Exp
    {
        public string Name { get; set; } = "ExpName";
        public string Value { get; set; } = "Exp的值";
    }
}