using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime;
using Engine.Tests.Api.Runtime.Util;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;
using NUnit.Framework;

namespace Engine.Tests.Standalone.History
{




    /// <summary>
    /// 
    /// 
    /// 
    /// 
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class FullHistoryTest
	{
		private bool _instanceFieldsInitialized = false;

		public FullHistoryTest()
		{
			if (!_instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				_instanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			TestHelper = new ProcessEngineTestRule(EngineRule);
			////ruleChain = RuleChain.outerRule(EngineRule).around(TestHelper);
		}


	  protected internal ProvidedProcessEngineRule EngineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule TestHelper;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testHelper);
	  ////public RuleChain ruleChain;

	  protected internal IRuntimeService RuntimeService;
	  protected internal IHistoryService HistoryService;
	  protected internal ITaskService TaskService;
	  protected internal IFormService FormService;
	  protected internal IRepositoryService RepositoryService;
	  protected internal ICaseService CaseService;

        [SetUp]
	  public virtual void InitServices()
	  {
		RuntimeService = EngineRule.RuntimeService;
		HistoryService = EngineRule.HistoryService;
		TaskService = EngineRule.TaskService;
		FormService = EngineRule.FormService;
		RepositoryService = EngineRule.RepositoryService;
		CaseService = EngineRule.CaseService;
	  }

        [Test]
        [Deployment]
        public virtual void TestVariableUpdates()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["number"] = "one";
		variables["character"] = "a";
		variables["bytes"] = ":-(".GetBytes();
		IProcessInstance processInstance = RuntimeService.StartProcessInstanceByKey("receiveTask", variables);
		RuntimeService.SetVariable(processInstance.Id, "number", "two");
		RuntimeService.SetVariable(processInstance.Id, "bytes", ":-)".GetBytes());

		// Start-task should be added to history
		IHistoricActivityInstance historicStartEvent = HistoryService.CreateHistoricActivityInstanceQuery(c=>c.ProcessInstanceId == processInstance.Id&& c.ActivityId =="theStart").First();
		Assert.NotNull(historicStartEvent);

		IHistoricActivityInstance WaitStateActivity = HistoryService.CreateHistoricActivityInstanceQuery(c=>c.ProcessInstanceId == processInstance.Id&& c.ActivityId =="WaitState").First();
		Assert.NotNull(WaitStateActivity);

		IHistoricActivityInstance serviceTaskActivity = HistoryService.CreateHistoricActivityInstanceQuery(c=>c.ProcessInstanceId == processInstance.Id&& c.ActivityId =="serviceTask").First();
		Assert.NotNull(serviceTaskActivity);

		IList<IHistoricDetail> historicDetails = HistoryService.CreateHistoricDetailQuery()/*.OrderByVariableName()*//*.Asc()*//*.OrderByVariableRevision()*//*.Asc()*/.ToList();

		Assert.AreEqual(10, historicDetails.Count);

		IHistoricVariableUpdate historicVariableUpdate = (IHistoricVariableUpdate) historicDetails[0];
		Assert.AreEqual("bytes", historicVariableUpdate.VariableName);
		Assert.AreEqual(":-(", StringHelperClass.NewString((byte[])historicVariableUpdate.Value));
		Assert.AreEqual(0, historicVariableUpdate.Revision);
		Assert.AreEqual(historicStartEvent.Id, historicVariableUpdate.ActivityInstanceId);

		// Variable is updated when process was in Waitstate
		historicVariableUpdate = (IHistoricVariableUpdate) historicDetails[1];
		Assert.AreEqual("bytes", historicVariableUpdate.VariableName);
		Assert.AreEqual(":-)", StringHelperClass.NewString((byte[])historicVariableUpdate.Value));
		Assert.AreEqual(1, historicVariableUpdate.Revision);
		Assert.AreEqual(WaitStateActivity.Id, historicVariableUpdate.ActivityInstanceId);

		historicVariableUpdate = (IHistoricVariableUpdate) historicDetails[2];
		Assert.AreEqual("character", historicVariableUpdate.VariableName);
		Assert.AreEqual("a", historicVariableUpdate.Value);
		Assert.AreEqual(0, historicVariableUpdate.Revision);
		Assert.AreEqual(historicStartEvent.Id, historicVariableUpdate.ActivityInstanceId);

		historicVariableUpdate = (IHistoricVariableUpdate) historicDetails[3];
		Assert.AreEqual("number", historicVariableUpdate.VariableName);
		Assert.AreEqual("one", historicVariableUpdate.Value);
		Assert.AreEqual(0, historicVariableUpdate.Revision);
		Assert.AreEqual(historicStartEvent.Id, historicVariableUpdate.ActivityInstanceId);

		// Variable is updated when process was in Waitstate
		historicVariableUpdate = (IHistoricVariableUpdate) historicDetails[4];
		Assert.AreEqual("number", historicVariableUpdate.VariableName);
		Assert.AreEqual("two", historicVariableUpdate.Value);
		Assert.AreEqual(1, historicVariableUpdate.Revision);
		Assert.AreEqual(WaitStateActivity.Id, historicVariableUpdate.ActivityInstanceId);

		// Variable set from process-start execution listener
		historicVariableUpdate = (IHistoricVariableUpdate) historicDetails[5];
		Assert.AreEqual("zVar1", historicVariableUpdate.VariableName);
		Assert.AreEqual("Event: start", historicVariableUpdate.Value);
		Assert.AreEqual(0, historicVariableUpdate.Revision);
		Assert.AreEqual(historicStartEvent.Id, historicVariableUpdate.ActivityInstanceId);

		// Variable set from transition take execution listener
		historicVariableUpdate = (IHistoricVariableUpdate) historicDetails[6];
		Assert.AreEqual("zVar2", historicVariableUpdate.VariableName);
		Assert.AreEqual("Event: take", historicVariableUpdate.Value);
		Assert.AreEqual(0, historicVariableUpdate.Revision);
		Assert.IsNull(historicVariableUpdate.ActivityInstanceId);

		// Variable set from activity start execution listener on the servicetask
		historicVariableUpdate = (IHistoricVariableUpdate) historicDetails[7];
		Assert.AreEqual("zVar3", historicVariableUpdate.VariableName);
		Assert.AreEqual("Event: start", historicVariableUpdate.Value);
		Assert.AreEqual(0, historicVariableUpdate.Revision);
		Assert.AreEqual(serviceTaskActivity.Id, historicVariableUpdate.ActivityInstanceId);

		// Variable set from activity end execution listener on the servicetask
		historicVariableUpdate = (IHistoricVariableUpdate) historicDetails[8];
		Assert.AreEqual("zVar4", historicVariableUpdate.VariableName);
		Assert.AreEqual("Event: end", historicVariableUpdate.Value);
		Assert.AreEqual(0, historicVariableUpdate.Revision);
		Assert.AreEqual(serviceTaskActivity.Id, historicVariableUpdate.ActivityInstanceId);

		// Variable set from service-task
		historicVariableUpdate = (IHistoricVariableUpdate) historicDetails[9];
		Assert.AreEqual("zzz", historicVariableUpdate.VariableName);
		Assert.AreEqual(123456789L, historicVariableUpdate.Value);
		Assert.AreEqual(0, historicVariableUpdate.Revision);
		Assert.AreEqual(serviceTaskActivity.Id, historicVariableUpdate.ActivityInstanceId);

		// trigger receive task
		RuntimeService.Signal(processInstance.Id);
		TestHelper.AssertProcessEnded(processInstance.Id);

		// check for historic process variables set
		IQueryable<IHistoricVariableInstance> historicProcessVariableQuery = HistoryService.CreateHistoricVariableInstanceQuery()/*.OrderByVariableName()*//*.Asc()*/;

		Assert.AreEqual(8, historicProcessVariableQuery.Count());

		IList<IHistoricVariableInstance> historicVariables = historicProcessVariableQuery.ToList();

		// Variable status when process is finished
		IHistoricVariableInstance historicVariable = historicVariables[0];
		Assert.AreEqual("bytes", historicVariable.VariableName);
		Assert.AreEqual(":-)", StringHelperClass.NewString((byte[])historicVariable.Value));

		historicVariable = historicVariables[1];
		Assert.AreEqual("character", historicVariable.VariableName);
		Assert.AreEqual("a", historicVariable.Value);

		historicVariable = historicVariables[2];
		Assert.AreEqual("number", historicVariable.VariableName);
		Assert.AreEqual("two", historicVariable.Value);

		historicVariable = historicVariables[3];
		Assert.AreEqual("zVar1", historicVariable.VariableName);
		Assert.AreEqual("Event: start", historicVariable.Value);

		historicVariable = historicVariables[4];
		Assert.AreEqual("zVar2", historicVariable.VariableName);
		Assert.AreEqual("Event: take", historicVariable.Value);

		historicVariable = historicVariables[5];
		Assert.AreEqual("zVar3", historicVariable.VariableName);
		Assert.AreEqual("Event: start", historicVariable.Value);

		historicVariable = historicVariables[6];
		Assert.AreEqual("zVar4", historicVariable.VariableName);
		Assert.AreEqual("Event: end", historicVariable.Value);

		historicVariable = historicVariables[7];
		Assert.AreEqual("zzz", historicVariable.VariableName);
		Assert.AreEqual(123456789L, historicVariable.Value);
	  }

        [Test]
        [Deployment (new string[]{"resources/standalone/history/FullHistoryTest.TestVariableUpdates.bpmn20.xml"})]
	  public virtual void TestHistoricVariableInstanceQuery()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["process"] = "one";
		IProcessInstance processInstance = RuntimeService.StartProcessInstanceByKey("receiveTask", variables);
		RuntimeService.Signal(processInstance.ProcessInstanceId);

		Assert.AreEqual(1, HistoryService.CreateHistoricVariableInstanceQuery()/*.VariableName("process")*/.Count());
		Assert.AreEqual(1, HistoryService.CreateHistoricVariableInstanceQuery()/*//.VariableValueEquals("process", "one")*/.Count());

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["process"] = "two";
		IProcessInstance processInstance2 = RuntimeService.StartProcessInstanceByKey("receiveTask", variables2);
		RuntimeService.Signal(processInstance2.ProcessInstanceId);

		Assert.AreEqual(2, HistoryService.CreateHistoricVariableInstanceQuery()/*.VariableName("process")*/.Count());
		Assert.AreEqual(1, HistoryService.CreateHistoricVariableInstanceQuery()/*//.VariableValueEquals("process", "one")*/.Count());
		Assert.AreEqual(1, HistoryService.CreateHistoricVariableInstanceQuery()/*//.VariableValueEquals("process", "two")*/.Count());

		IHistoricVariableInstance historicProcessVariable = HistoryService.CreateHistoricVariableInstanceQuery()/*//.VariableValueEquals("process", "one")*/.First();
		Assert.AreEqual("process", historicProcessVariable.VariableName);
		Assert.AreEqual("one", historicProcessVariable.Value);
		Assert.AreEqual(ValueTypeFields.String.Name, historicProcessVariable.VariableTypeName);
		Assert.AreEqual(ValueTypeFields.String.Name, historicProcessVariable.TypeName);
		Assert.AreEqual(historicProcessVariable.Value, historicProcessVariable.TypedValue.Value);
		Assert.AreEqual(historicProcessVariable.TypeName, historicProcessVariable.TypedValue.Type.Name);

		IDictionary<string, object> variables3 = new Dictionary<string, object>();
		variables3["long"] = 1000l;
		variables3["double"] = 25.43d;
		IProcessInstance processInstance3 = RuntimeService.StartProcessInstanceByKey("receiveTask", variables3);
		RuntimeService.Signal(processInstance3.ProcessInstanceId);

		Assert.AreEqual(1, HistoryService.CreateHistoricVariableInstanceQuery()/*.VariableName("long")*/.Count());
		Assert.AreEqual(1, HistoryService.CreateHistoricVariableInstanceQuery()/*//.VariableValueEquals("long", 1000l)*/.Count());
		Assert.AreEqual(1, HistoryService.CreateHistoricVariableInstanceQuery()/*.VariableName("double")*/.Count());
		Assert.AreEqual(1, HistoryService.CreateHistoricVariableInstanceQuery()/*//.VariableValueEquals("double", 25.43d)*/.Count());

	  }

        [Test]
        [Deployment]
        public virtual void TestHistoricVariableUpdatesAllTypes()
	  {

		//SimpleDateFormat sdf = new SimpleDateFormat("dd/MM/yyyy hh:mm:ss SSS");
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariable"] = "initial value";

		DateTime startedDate = DateTime.Parse("01/01/2001 01:23:45 000");

		// In the javaDelegate, the current time is manipulated
		DateTime updatedDate = DateTime.Parse("01/01/2001 01:23:46 000");

		ClockUtil.CurrentTime = startedDate;

		IProcessInstance processInstance = RuntimeService.StartProcessInstanceByKey("HistoricVariableUpdateProcess", variables);

		IList<IHistoricDetail> details = HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*/.Where(c=>c.ProcessInstanceId==processInstance.Id)/*.OrderByVariableName()*//*.Asc()*//*.OrderByTime()*//*.Asc()*/.ToList();

		// 8 variable updates should be present, one performed when starting process
		// the other 7 are set in VariableSetter serviceTask
		Assert.AreEqual(9, details.Count);

		// Since we order by varName, first entry should be aVariable update from startTask
		IHistoricVariableUpdate startVarUpdate = (IHistoricVariableUpdate) details[0];
		Assert.AreEqual("aVariable", startVarUpdate.VariableName);
		Assert.AreEqual("initial value", startVarUpdate.Value);
		Assert.AreEqual(0, startVarUpdate.Revision);
		Assert.AreEqual(processInstance.Id, startVarUpdate.ProcessInstanceId);
		// Date should the the one set when starting
		Assert.AreEqual(startedDate, startVarUpdate.Time);

		IHistoricVariableUpdate updatedStringVariable = (IHistoricVariableUpdate) details[1];
		Assert.AreEqual("aVariable", updatedStringVariable.VariableName);
		Assert.AreEqual("updated value", updatedStringVariable.Value);
		Assert.AreEqual(processInstance.Id, updatedStringVariable.ProcessInstanceId);
		// Date should be the updated date
		Assert.AreEqual(updatedDate, updatedStringVariable.Time);

		IHistoricVariableUpdate intVariable = (IHistoricVariableUpdate) details[2];
		Assert.AreEqual("bVariable", intVariable.VariableName);
		Assert.AreEqual(123, intVariable.Value);
		Assert.AreEqual(processInstance.Id, intVariable.ProcessInstanceId);
		Assert.AreEqual(updatedDate, intVariable.Time);

		IHistoricVariableUpdate longVariable = (IHistoricVariableUpdate) details[3];
		Assert.AreEqual("cVariable", longVariable.VariableName);
		Assert.AreEqual(12345L, longVariable.Value);
		Assert.AreEqual(processInstance.Id, longVariable.ProcessInstanceId);
		Assert.AreEqual(updatedDate, longVariable.Time);

		IHistoricVariableUpdate doubleVariable = (IHistoricVariableUpdate) details[4];
		Assert.AreEqual("dVariable", doubleVariable.VariableName);
		Assert.AreEqual(1234.567, doubleVariable.Value);
		Assert.AreEqual(processInstance.Id, doubleVariable.ProcessInstanceId);
		Assert.AreEqual(updatedDate, doubleVariable.Time);

		IHistoricVariableUpdate shortVariable = (IHistoricVariableUpdate) details[5];
		Assert.AreEqual("eVariable", shortVariable.VariableName);
		Assert.AreEqual((short)12, shortVariable.Value);
		Assert.AreEqual(processInstance.Id, shortVariable.ProcessInstanceId);
		Assert.AreEqual(updatedDate, shortVariable.Time);

		IHistoricVariableUpdate dateVariable = (IHistoricVariableUpdate) details[6];
		Assert.AreEqual("fVariable", dateVariable.VariableName);
		Assert.AreEqual(DateTime.Parse("01/01/2001 01:23:45 678"), dateVariable.Value);
		Assert.AreEqual(processInstance.Id, dateVariable.ProcessInstanceId);
		Assert.AreEqual(updatedDate, dateVariable.Time);

		IHistoricVariableUpdate serializableVariable = (IHistoricVariableUpdate) details[7];
		Assert.AreEqual("gVariable", serializableVariable.VariableName);
		//Assert.AreEqual(new SerializableVariable("hello hello"), serializableVariable.Value);
		Assert.AreEqual(processInstance.Id, serializableVariable.ProcessInstanceId);
		Assert.AreEqual(updatedDate, serializableVariable.Time);

		IHistoricVariableUpdate byteArrayVariable = (IHistoricVariableUpdate) details[8];
		Assert.AreEqual("hVariable", byteArrayVariable.VariableName);
		Assert.AreEqual(";-)", StringHelperClass.NewString((byte[])byteArrayVariable.Value));
		Assert.AreEqual(processInstance.Id, byteArrayVariable.ProcessInstanceId);
		Assert.AreEqual(updatedDate, byteArrayVariable.Time);

		// end process instance
		IList<ITask> tasks = TaskService.CreateTaskQuery().ToList();
		Assert.AreEqual(1, tasks.Count);
		TaskService.Complete(tasks[0].Id);
		TestHelper.AssertProcessEnded(processInstance.Id);

		// check for historic process variables set
		IQueryable<IHistoricVariableInstance> historicProcessVariableQuery = HistoryService.CreateHistoricVariableInstanceQuery()/*.OrderByVariableName()*//*.Asc()*/;

		Assert.AreEqual(8, historicProcessVariableQuery.Count());

		IList<IHistoricVariableInstance> historicVariables = historicProcessVariableQuery.ToList();

	 // Variable status when process is finished
		IHistoricVariableInstance historicVariable = historicVariables[0];
		Assert.AreEqual("aVariable", historicVariable.VariableName);
		Assert.AreEqual("updated value", historicVariable.Value);
		Assert.AreEqual(processInstance.Id, historicVariable.ProcessInstanceId);

		historicVariable = historicVariables[1];
		Assert.AreEqual("bVariable", historicVariable.VariableName);
		Assert.AreEqual(123, historicVariable.Value);
		Assert.AreEqual(processInstance.Id, historicVariable.ProcessInstanceId);

		historicVariable = historicVariables[2];
		Assert.AreEqual("cVariable", historicVariable.VariableName);
		Assert.AreEqual(12345L, historicVariable.Value);
		Assert.AreEqual(processInstance.Id, historicVariable.ProcessInstanceId);

		historicVariable = historicVariables[3];
		Assert.AreEqual("dVariable", historicVariable.VariableName);
		Assert.AreEqual(1234.567, historicVariable.Value);
		Assert.AreEqual(processInstance.Id, historicVariable.ProcessInstanceId);

		historicVariable = historicVariables[4];
		Assert.AreEqual("eVariable", historicVariable.VariableName);
		Assert.AreEqual((short) 12, historicVariable.Value);
		Assert.AreEqual(processInstance.Id, historicVariable.ProcessInstanceId);

		historicVariable = historicVariables[5];
		Assert.AreEqual("fVariable", historicVariable.VariableName);
		Assert.AreEqual(DateTime.Parse("01/01/2001 01:23:45 678"), historicVariable.Value);
		Assert.AreEqual(processInstance.Id, historicVariable.ProcessInstanceId);

		historicVariable = historicVariables[6];
		Assert.AreEqual("gVariable", historicVariable.VariableName);
		//Assert.AreEqual(new SerializableVariable("hello hello"), historicVariable.Value);
		Assert.AreEqual(processInstance.Id, historicVariable.ProcessInstanceId);

		historicVariable = historicVariables[7];
		Assert.AreEqual("hVariable", historicVariable.VariableName);
		Assert.AreEqual(";-)", ";-)", StringHelperClass.NewString((byte[])historicVariable.Value));
		Assert.AreEqual(processInstance.Id, historicVariable.ProcessInstanceId);
	  }

        [Test]
        [Deployment]
        public virtual void TestHistoricFormProperties()
	  {
            DateTime startedDate = DateTime.Parse("01/01/2001 01:23:46 000");

            ClockUtil.CurrentTime = startedDate;

		IDictionary<string, string> formProperties = new Dictionary<string, string>();
		formProperties["formProp1"] = "Activiti rocks";
		formProperties["formProp2"] = "12345";

		IProcessDefinition procDef = RepositoryService.CreateProcessDefinitionQuery(c=>c.Key == "historicFormPropertiesProcess").First();

		IProcessInstance processInstance = FormService.SubmitStartFormData(procDef.Id, formProperties);

		// Submit form-properties on the created task
		ITask task = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First();
		Assert.NotNull(task);

		// Out execution only has a single activity Waiting, the task
		IList<string> activityIds = RuntimeService.GetActiveActivityIds(task.ExecutionId);
		Assert.NotNull(activityIds);
		Assert.AreEqual(1, activityIds.Count);

		string taskActivityId = activityIds[0];

		// Submit form properties
		formProperties = new Dictionary<string, string>();
		formProperties["formProp3"] = "Activiti still rocks!!!";
		formProperties["formProp4"] = "54321";
		FormService.SubmitTaskFormData(task.Id, formProperties);

		// 4 historic form properties should be created. 2 when process started, 2 when task completed
		IList<IHistoricDetail> props = HistoryService.CreateHistoricDetailQuery()/*.FormProperties(c=>c.ProcessInstanceId == processInstance.Id)*//*.OrderByFormPropertyId()*//*.Asc()*/.ToList();

		IHistoricFormProperty historicProperty1 = (IHistoricFormProperty) props[0];
		Assert.AreEqual("formProp1", historicProperty1.PropertyId);
		Assert.AreEqual("Activiti rocks", historicProperty1.PropertyValue);
		Assert.AreEqual(startedDate, historicProperty1.Time);
		Assert.AreEqual(processInstance.Id, historicProperty1.ProcessInstanceId);
		Assert.IsNull(historicProperty1.TaskId);

		Assert.NotNull(historicProperty1.ActivityInstanceId);
		IHistoricActivityInstance historicActivityInstance = HistoryService.CreateHistoricActivityInstanceQuery(c=>c.ActivityInstanceId == historicProperty1.ActivityInstanceId).First();
		Assert.NotNull(historicActivityInstance);
		Assert.AreEqual("start", historicActivityInstance.ActivityId);

		IHistoricFormProperty historicProperty2 = (IHistoricFormProperty) props[1];
		Assert.AreEqual("formProp2", historicProperty2.PropertyId);
		Assert.AreEqual("12345", historicProperty2.PropertyValue);
		Assert.AreEqual(startedDate, historicProperty2.Time);
		Assert.AreEqual(processInstance.Id, historicProperty2.ProcessInstanceId);
		Assert.IsNull(historicProperty2.TaskId);

		Assert.NotNull(historicProperty2.ActivityInstanceId);
		historicActivityInstance = HistoryService.CreateHistoricActivityInstanceQuery(c=>c.ActivityInstanceId == historicProperty2.ActivityInstanceId).First();
		Assert.NotNull(historicActivityInstance);
		Assert.AreEqual("start", historicActivityInstance.ActivityId);

		IHistoricFormProperty historicProperty3 = (IHistoricFormProperty) props[2];
		Assert.AreEqual("formProp3", historicProperty3.PropertyId);
		Assert.AreEqual("Activiti still rocks!!!", historicProperty3.PropertyValue);
		Assert.AreEqual(startedDate, historicProperty3.Time);
		Assert.AreEqual(processInstance.Id, historicProperty3.ProcessInstanceId);
		string activityInstanceId = historicProperty3.ActivityInstanceId;
		historicActivityInstance = HistoryService.CreateHistoricActivityInstanceQuery(c=>c.ActivityInstanceId == activityInstanceId).First();
		Assert.NotNull(historicActivityInstance);
		Assert.AreEqual(taskActivityId, historicActivityInstance.ActivityId);
		Assert.NotNull(historicProperty3.TaskId);

		IHistoricFormProperty historicProperty4 = (IHistoricFormProperty) props[3];
		Assert.AreEqual("formProp4", historicProperty4.PropertyId);
		Assert.AreEqual("54321", historicProperty4.PropertyValue);
		Assert.AreEqual(startedDate, historicProperty4.Time);
		Assert.AreEqual(processInstance.Id, historicProperty4.ProcessInstanceId);
		activityInstanceId = historicProperty4.ActivityInstanceId;
		historicActivityInstance = HistoryService.CreateHistoricActivityInstanceQuery(c=>c.ActivityInstanceId == activityInstanceId).First();
		Assert.NotNull(historicActivityInstance);
		Assert.AreEqual(taskActivityId, historicActivityInstance.ActivityId);
		Assert.NotNull(historicProperty4.TaskId);

		Assert.AreEqual(4, props.Count);
	  }

        [Test]
        [Deployment("resources/history/oneTaskProcess.bpmn20.xml")]
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        public virtual void TestHistoricVariableQuery()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "activiti rocks!";
		variables["longVar"] = 12345L;

		IProcessInstance processInstance = RuntimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

		// Query on activity-instance, activity instance null will return all vars set when starting process
		Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*//*.ActivityInstanceId(null)*/.Count());
		Assert.AreEqual(0, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*//*.ActivityInstanceId("unexisting")*/.Count());

		// Query on process-instance
		Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*/.Where(c=>c.ProcessInstanceId==processInstance.Id).Count());
		Assert.AreEqual(0, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*/.Where(c=>c.ProcessInstanceId=="unexisting").Count());

		// Query both process-instance and activity-instance
		Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*//*.ActivityInstanceId(null)*/.Where(c=>c.ProcessInstanceId==processInstance.Id).Count());

		// end process instance
		IList<ITask> tasks = TaskService.CreateTaskQuery().ToList();
		Assert.AreEqual(1, tasks.Count);
		TaskService.Complete(tasks[0].Id);
		TestHelper.AssertProcessEnded(processInstance.Id);

		Assert.AreEqual(2, HistoryService.CreateHistoricVariableInstanceQuery().Count());

		// Query on process-instance
		Assert.AreEqual(2, HistoryService.CreateHistoricVariableInstanceQuery(c=>c.ProcessInstanceId == processInstance.Id).Count());
		Assert.AreEqual(0, HistoryService.CreateHistoricVariableInstanceQuery(c=>c.ProcessInstanceId == "unexisting").Count());
	  }


        [Test]
        [Deployment("resources/history/oneTaskProcess.bpmn20.xml")]
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        public virtual void TestHistoricVariableQueryExcludeTaskRelatedDetails()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "activiti rocks!";
		variables["longVar"] = 12345L;

		IProcessInstance processInstance = RuntimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

		// Set a local task-variable
		ITask task = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First();
		Assert.NotNull(task);
		TaskService.SetVariableLocal(task.Id, "taskVar", "It is I, le Variable");

		// Query on process-instance
		Assert.AreEqual(3, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*/.Where(c=>c.ProcessInstanceId==processInstance.Id).Count());

		// Query on process-instance, excluding task-details
		Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*/.Where(c=>c.ProcessInstanceId==processInstance.Id)/*.ExcludeTaskDetails()*/.Count());

		// Check task-id precedence on excluding task-details
		Assert.AreEqual(1, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*/.Where(c=>c.ProcessInstanceId==processInstance.Id)/*.ExcludeTaskDetails(c=>c.Id == task.Id)*/.Count());
	  }

        [Test]
        [Deployment(new string[]{"resources/history/oneTaskProcess.bpmn20.xml"})]
        public virtual void TestHistoricFormPropertiesQuery()
	  {
		IDictionary<string, string> formProperties = new Dictionary<string, string>();
		formProperties["stringVar"] = "activiti rocks!";
		formProperties["longVar"] = "12345";

		IProcessDefinition procDef = RepositoryService.CreateProcessDefinitionQuery(c=>c.Key == "oneTaskProcess").First();
		IProcessInstance processInstance = FormService.SubmitStartFormData(procDef.Id, formProperties);

		// Query on activity-instance, activity instance null will return all vars set when starting process
		Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery()/*.FormProperties(c=>c.ActivityInstanceId == null)*/.Count());
		Assert.AreEqual(0, HistoryService.CreateHistoricDetailQuery()/*.FormProperties(c=>c.ActivityInstanceId == "unexisting")*/.Count());

		// Query on process-instance
		Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery()/*.FormProperties(c=>c.ProcessInstanceId == processInstance.Id)*/.Count());
		Assert.AreEqual(0, HistoryService.CreateHistoricDetailQuery()/*.FormProperties(c=>c.ProcessInstanceId == "unexisting")*/.Count());

		// Complete the task by submitting the task properties
		ITask task = TaskService.CreateTaskQuery().First();
		formProperties = new Dictionary<string, string>();
		formProperties["taskVar"] = "task form property";
		FormService.SubmitTaskFormData(task.Id, formProperties);

		Assert.AreEqual(3, HistoryService.CreateHistoricDetailQuery()/*.FormProperties(c=>c.ProcessInstanceId == processInstance.Id)*/.Count());
		Assert.AreEqual(0, HistoryService.CreateHistoricDetailQuery()/*.FormProperties(c=>c.ProcessInstanceId == "unexisting")*/.Count());
	  }


        [Test]
        [Deployment(new string[]{"resources/history/oneTaskProcess.bpmn20.xml"})]
        public virtual void TestHistoricVariableQuerySorting()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "activiti rocks!";
		variables["longVar"] = 12345L;

		RuntimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*///.OrderByProcessInstanceId()/*.Asc()*/.Count());
		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*/.OrderByTime()/*.Asc()*/.Count());
		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*//*.OrderByVariableName()*//*.Asc()*/.Count());
		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*//*.OrderByVariableRevision()*//*.Asc()*/.Count());
		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*/.OrderByVariableType()/*.Asc()*/.Count());

		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*///.OrderByProcessInstanceId()/*.Desc()*/.Count());
		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*/.OrderByTime()/*.Desc()*/.Count());
		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*//*.OrderByVariableName()*//*.Desc()*/.Count());
		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*//*.OrderByVariableRevision()*//*.Desc()*/.Count());
		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*/.OrderByVariableType()/*.Desc()*/.Count());

		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*///.OrderByProcessInstanceId()/*.Asc()*/.Count());
		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*/.OrderByTime()/*.Asc()*/.Count());
		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*//*.OrderByVariableName()*//*.Asc()*/.Count());
		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*//*.OrderByVariableRevision()*//*.Asc()*/.Count());
		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*/.OrderByVariableType()/*.Asc()*/.Count());

		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*///.OrderByProcessInstanceId()/*.Desc()*/.Count());
		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*/.OrderByTime()/*.Desc()*/.Count());
		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*//*.OrderByVariableName()*//*.Desc()*/.Count());
		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*//*.OrderByVariableRevision()*//*.Desc()*/.Count());
		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*/.OrderByVariableType()/*.Desc()*/.Count());
	  }

        [Test]
        [Deployment(new string[]{"resources/history/oneTaskProcess.bpmn20.xml"})] 
        public virtual void TestHistoricFormPropertySorting()
	  {

		IDictionary<string, string> formProperties = new Dictionary<string, string>();
		formProperties["stringVar"] = "activiti rocks!";
		formProperties["longVar"] = "12345";

		IProcessDefinition procDef = RepositoryService.CreateProcessDefinitionQuery(c=>c.Key == "oneTaskProcess").First();
		FormService.SubmitStartFormData(procDef.Id, formProperties);

		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery().FormProperties()//.OrderByProcessInstanceId()/*.Asc()*/.Count());
		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery().FormProperties().OrderByTime()/*.Asc()*/.Count());
		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery().FormProperties().OrderByFormPropertyId()/*.Asc()*/.Count());

		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery().FormProperties()//.OrderByProcessInstanceId()/*.Desc()*/.Count());
		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery().FormProperties().OrderByTime()/*.Desc()*/.Count());
		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery().FormProperties().OrderByFormPropertyId()/*.Desc()*/.Count());

		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery().FormProperties()//.OrderByProcessInstanceId()/*.Asc()*/.Count());
		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery().FormProperties().OrderByTime()/*.Asc()*/.Count());
		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery().FormProperties().OrderByFormPropertyId()/*.Asc()*/.Count());

		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery().FormProperties()//.OrderByProcessInstanceId()/*.Desc()*/.Count());
		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery().FormProperties().OrderByTime()/*.Desc()*/.Count());
		//Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery().FormProperties().OrderByFormPropertyId()/*.Desc()*/.Count());
	  }

        [Test]
        [Deployment]
        public virtual void TestHistoricDetailQueryMixed()
	  {

		IDictionary<string, string> formProperties = new Dictionary<string, string>();
		formProperties["formProp1"] = "activiti rocks!";
		formProperties["formProp2"] = "12345";

		IProcessDefinition procDef = RepositoryService.CreateProcessDefinitionQuery(c=>c.Key == "historicDetailMixed").First();
		IProcessInstance processInstance = FormService.SubmitStartFormData(procDef.Id, formProperties);

		IList<IHistoricDetail> details = HistoryService.CreateHistoricDetailQuery(c=>c.ProcessInstanceId == processInstance.Id)/*.OrderByVariableName()*//*.Asc()*/.ToList();

		Assert.AreEqual(4, details.Count);

		Assert.True(details[0] is IHistoricFormProperty);
		IHistoricFormProperty formProp1 = (IHistoricFormProperty) details[0];
		Assert.AreEqual("formProp1", formProp1.PropertyId);
		Assert.AreEqual("activiti rocks!", formProp1.PropertyValue);

		Assert.True(details[1] is IHistoricFormProperty);
		IHistoricFormProperty formProp2 = (IHistoricFormProperty) details[1];
		Assert.AreEqual("formProp2", formProp2.PropertyId);
		Assert.AreEqual("12345", formProp2.PropertyValue);


		Assert.True(details[2] is IHistoricVariableUpdate);
		IHistoricVariableUpdate varUpdate1 = (IHistoricVariableUpdate) details[2];
		Assert.AreEqual("variable1", varUpdate1.VariableName);
		Assert.AreEqual("activiti rocks!", varUpdate1.Value);


		// This variable should be of type LONG since this is defined in the process-definition
		Assert.True(details[3] is IHistoricVariableUpdate);
		IHistoricVariableUpdate varUpdate2 = (IHistoricVariableUpdate) details[3];
		Assert.AreEqual("variable2", varUpdate2.VariableName);
		Assert.AreEqual(12345L, varUpdate2.Value);
	  }

        [Test]
        public virtual void TestHistoricDetailQueryInvalidSorting()
	  {
		try
		{
		  HistoryService.CreateHistoricDetailQuery()/*.Asc()*/.ToList();
		  Assert.Fail();
		}
		catch (ProcessEngineException)
		{

		}

		try
		{
		  HistoryService.CreateHistoricDetailQuery()/*.Desc()*/.ToList();
		  Assert.Fail();
		}
		catch (ProcessEngineException)
		{

		}

		try
		{
		  //HistoryService.CreateHistoricDetailQuery()//.OrderByProcessInstanceId().ToList();
		  Assert.Fail();
		}
		catch (ProcessEngineException)
		{

		}

		try
		{
		  //HistoryService.CreateHistoricDetailQuery().OrderByTime().ToList();
		  Assert.Fail();
		}
		catch (ProcessEngineException)
		{

		}

		try
		{
		  HistoryService.CreateHistoricDetailQuery()/*.OrderByVariableName()*/.ToList();
		  Assert.Fail();
		}
		catch (ProcessEngineException)
		{

		}

		try
		{
		  //HistoryService.CreateHistoricDetailQuery()/*.OrderByVariableRevision()*/.ToList();
		  Assert.Fail();
		}
		catch (ProcessEngineException)
		{

		}

		try
		{
		  //HistoryService.CreateHistoricDetailQuery().OrderByVariableType().ToList();
		  Assert.Fail();
		}
		catch (ProcessEngineException)
		{

		}
	  }

        [Test]
        [Deployment]
        public virtual void TestHistoricTaskInstanceVariableUpdates()
	  {
		string processInstanceId = RuntimeService.StartProcessInstanceByKey("HistoricTaskInstanceTest").Id;

		string taskId = TaskService.CreateTaskQuery().First().Id;

		RuntimeService.SetVariable(processInstanceId, "deadline", "yesterday");

		TaskService.SetVariableLocal(taskId, "bucket", "23c");
		TaskService.SetVariableLocal(taskId, "mop", "37i");

		TaskService.Complete(taskId);

		Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery().Count());

		IList<IHistoricDetail> historicTaskVariableUpdates = HistoryService.CreateHistoricDetailQuery(c=>c.Id == taskId)/*.VariableUpdates()*//*.OrderByVariableName()*//*.Asc()*/.ToList();

		Assert.AreEqual(2, historicTaskVariableUpdates.Count);

		HistoryService.DeleteHistoricTaskInstance(taskId);

		// Check if the variable updates have been removed as well
		historicTaskVariableUpdates = HistoryService.CreateHistoricDetailQuery(c=>c.Id == taskId)/*.VariableUpdates()*//*.OrderByVariableName()*//*.Asc()*/.ToList();

		 Assert.AreEqual(0, historicTaskVariableUpdates.Count);
	  }

        [Test]
        [Deployment]
        public virtual void TestSetVariableOnProcessInstanceWithTimer()
	  {
		IProcessInstance processInstance = RuntimeService.StartProcessInstanceByKey("timerVariablesProcess");
		RuntimeService.SetVariable(processInstance.Id, "myVar", 123456L);
		Assert.AreEqual(123456L, RuntimeService.GetVariable(processInstance.Id, "myVar"));
	  }

        [Test]
        [Deployment]
        public virtual void TestDeleteHistoricProcessInstance()
	  {
		// Start process-instance with some variables set
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["processVar"] = 123L;
		vars["anotherProcessVar"] = new DummySerializable();

		IProcessInstance processInstance = RuntimeService.StartProcessInstanceByKey("HistoricTaskInstanceTest", vars);
		Assert.NotNull(processInstance);

		// Set 2 task properties
		ITask task = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First();
		TaskService.SetVariableLocal(task.Id, "taskVar", 45678);
		TaskService.SetVariableLocal(task.Id, "anotherTaskVar", "value");

		// Finish the task, this end the process-instance
		TaskService.Complete(task.Id);

		Assert.AreEqual(1, HistoryService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId == processInstance.Id).Count());
		Assert.AreEqual(3, HistoryService.CreateHistoricActivityInstanceQuery(c=>c.ProcessInstanceId == processInstance.Id).Count());
		Assert.AreEqual(4, HistoryService.CreateHistoricVariableInstanceQuery(c=>c.ProcessInstanceId == processInstance.Id).Count());
		Assert.AreEqual(4, HistoryService.CreateHistoricDetailQuery(c=>c.ProcessInstanceId == processInstance.Id).Count());
		Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery(c=>c.ProcessInstanceId == processInstance.Id).Count());

		// Delete the historic process-instance
		HistoryService.DeleteHistoricProcessInstance(processInstance.Id);

		// Verify no traces are left in the history tables
		Assert.AreEqual(0, HistoryService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId == processInstance.Id).Count());
		Assert.AreEqual(0, HistoryService.CreateHistoricActivityInstanceQuery(c=>c.ProcessInstanceId == processInstance.Id).Count());
		Assert.AreEqual(0, HistoryService.CreateHistoricVariableInstanceQuery(c=>c.ProcessInstanceId == processInstance.Id).Count());
		Assert.AreEqual(0, HistoryService.CreateHistoricDetailQuery(c=>c.ProcessInstanceId == processInstance.Id).Count());
		Assert.AreEqual(0, HistoryService.CreateHistoricTaskInstanceQuery(c=>c.ProcessInstanceId == processInstance.Id).Count());

		try
		{
		  // Delete the historic process-instance, which is still running
		  HistoryService.DeleteHistoricProcessInstance("unexisting");
		  Assert.Fail("Exception expected when deleting process-instance that is still running");
		}
		catch (ProcessEngineException ae)
		{
		  // Expected exception
		  Assert.That(ae.Message, Does.Contain("No historic process instance found with id: unexisting"));
		}
	  }

        [Test]
        [Deployment]
        public virtual void TestDeleteRunningHistoricProcessInstance()
	  {
		IProcessInstance processInstance = RuntimeService.StartProcessInstanceByKey("HistoricTaskInstanceTest");
		Assert.NotNull(processInstance);

		try
		{
		  // Delete the historic process-instance, which is still running
		  HistoryService.DeleteHistoricProcessInstance(processInstance.Id);
		  Assert.Fail("Exception expected when deleting process-instance that is still running");
		}
		catch (ProcessEngineException ae)
		{
		  // Expected exception
		  Assert.That(ae.Message,Does.Contain("Process instance is still running, cannot Delete historic process instance"));
		}
	  }

        [Test]
        [Deployment]
        public virtual void TestDeleteCachedHistoricDetails()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String processDefinitionId = repositoryService.CreateProcessDefinitionQuery().First().GetId();
		string processDefinitionId = RepositoryService.CreateProcessDefinitionQuery().First().Id;


		EngineRule.ProcessEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this, processDefinitionId));

		// the historic process instance should still be there
		Assert.AreEqual(1, HistoryService.CreateHistoricProcessInstanceQuery().Count());

		// the historic details should be deleted
		Assert.AreEqual(0, HistoryService.CreateHistoricDetailQuery().Count());
	  }

	  private class CommandAnonymousInnerClass : ICommand<object>
	  {
		  private readonly FullHistoryTest _outerInstance;

		  private string _processDefinitionId;

		  public CommandAnonymousInnerClass(FullHistoryTest outerInstance, string processDefinitionId)
		  {
			  this._outerInstance = outerInstance;
			  this._processDefinitionId = processDefinitionId;
		  }


		  public virtual object Execute(CommandContext commandContext)
		  {
			IDictionary<string, object> formProperties = new Dictionary<string, object>();
			formProperties["formProp1"] = "value1";

			IProcessInstance processInstance = (new SubmitStartFormCmd(_processDefinitionId, null, formProperties)).Execute(commandContext);

			// two historic details should be in cache: one form property and one variable update
			commandContext.HistoricDetailManager.DeleteHistoricDetailsByProcessInstanceId(processInstance.Id);
			return null;
		  }
	  }

        /// <summary>
        /// Test created to validate ACT-621 fix.
        /// </summary>
        [Test]
        [Deployment]
        public virtual void TestHistoricFormPropertiesOnReEnteringActivity()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["comeBack"] = true;

		IProcessInstance processInstance = RuntimeService.StartProcessInstanceByKey("HistoricFormPropertiesProcess", variables);
		Assert.NotNull(processInstance);

		// Submit form on task
		IDictionary<string, string> data = new Dictionary<string, string>();
		data["formProp1"] = "Property value";

		ITask task = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First();
		FormService.SubmitTaskFormData(task.Id, data);

		// Historic property should be available
		IList<IHistoricDetail> details = HistoryService.CreateHistoricDetailQuery()/*.FormProperties(c=>c.ProcessInstanceId == processInstance.Id)*/.ToList();
		Assert.NotNull(details);
		Assert.AreEqual(1, details.Count);

		// ITask should be active in the same activity as the previous one
		task = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First();
		FormService.SubmitTaskFormData(task.Id, data);

		details = HistoryService.CreateHistoricDetailQuery()/*.FormProperties(c=>c.ProcessInstanceId == processInstance.Id)*/.ToList();
		Assert.NotNull(details);
		Assert.AreEqual(2, details.Count);

		// Should have 2 different historic activity instance ID's, with the same activityId
		Assert.AreNotEqual(details[0].ActivityInstanceId, details[1].ActivityInstanceId);

		IHistoricActivityInstance historicActInst1 = HistoryService.CreateHistoricActivityInstanceQuery(c=>c.ActivityInstanceId == details[0].ActivityInstanceId).First();

		IHistoricActivityInstance historicActInst2 = HistoryService.CreateHistoricActivityInstanceQuery(c=>c.ActivityInstanceId == details[1].ActivityInstanceId).First();

		Assert.AreEqual(historicActInst1.ActivityId, historicActInst2.ActivityId);
	  }
        
        [Test]
        [Deployment]
        public virtual void TestHistoricTaskInstanceQueryTaskVariableValueEquals()
	  {
		IProcessInstance processInstance = RuntimeService.StartProcessInstanceByKey("HistoricTaskInstanceTest");
		ITask task = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First();

		// Set some variables on the task
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["longVar"] = 12345L;
		variables["shortVar"] = (short) 123;
		variables["integerVar"] = 1234;
		variables["stringVar"] = "stringValue";
		variables["booleanVar"] = true;
		DateTime date = new DateTime();
		variables["dateVar"] = date;
		variables["nullVar"] = null;

		TaskService.SetVariablesLocal(task.Id, variables);

		// Validate all variable-updates are present in DB
		Assert.AreEqual(7, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates(c=>c.Id == task.Id)*/.Count());

		// Query Historic task instances based on variable
		Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()/*//.TaskVariableValueEquals("longVar", 12345L)*/.Count());
		Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()/*//.TaskVariableValueEquals("shortVar", (short) 123)*/.Count());
		//Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()//.TaskVariableValueEquals("integerVar",1234).Count());
		//Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()//.TaskVariableValueEquals("stringVar","stringValue").Count());
		//Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()//.TaskVariableValueEquals("booleanVar", true).Count());
		//Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()//.TaskVariableValueEquals("dateVar", date).Count());
		//Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()//.TaskVariableValueEquals("nullVar", null).Count());

		// Update the variables
		variables["longVar"] = 67890L;
		variables["shortVar"] = (short) 456;
		variables["integerVar"] = 5678;
		variables["stringVar"] = "updatedStringValue";
		variables["booleanVar"] = false;
		DateTime otherCal = new DateTime();
		otherCal.AddDays(1);
		DateTime otherDate = otherCal;
		variables["dateVar"] = otherDate;
		variables["nullVar"] = null;

		TaskService.SetVariablesLocal(task.Id, variables);

		// Validate all variable-updates are present in DB
		Assert.AreEqual(14, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates(c=>c.Id == task.Id)*/.Count());

		// Previous values should NOT match
		Assert.AreEqual(0, HistoryService.CreateHistoricTaskInstanceQuery()/*//.TaskVariableValueEquals("longVar", 12345L)*/.Count());
		Assert.AreEqual(0, HistoryService.CreateHistoricTaskInstanceQuery()/*//.TaskVariableValueEquals("shortVar", (short) 123)*/.Count());
		//Assert.AreEqual(0, HistoryService.CreateHistoricTaskInstanceQuery()//.TaskVariableValueEquals("integerVar",1234).Count());
		//Assert.AreEqual(0, HistoryService.CreateHistoricTaskInstanceQuery()//.TaskVariableValueEquals("stringVar","stringValue").Count());
		//Assert.AreEqual(0, HistoryService.CreateHistoricTaskInstanceQuery()//.TaskVariableValueEquals("booleanVar", true).Count());
		//Assert.AreEqual(0, HistoryService.CreateHistoricTaskInstanceQuery()//.TaskVariableValueEquals("dateVar", date).Count());

		//// New values should match
		//Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()//.TaskVariableValueEquals("longVar", 67890L).Count());
		//Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()//.TaskVariableValueEquals("shortVar", (short) 456).Count());
		//Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()//.TaskVariableValueEquals("integerVar",5678).Count());
		//Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()//.TaskVariableValueEquals("stringVar","updatedStringValue").Count());
		//Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()//.TaskVariableValueEquals("booleanVar", false).Count());
		//Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()//.TaskVariableValueEquals("dateVar", otherDate).Count());
		//Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()//.TaskVariableValueEquals("nullVar", null).Count());
	  }

        [Test]
        [Deployment(new string[] {"resources/api/oneTaskProcess.bpmn20.xml"})] 
        public virtual void TestHistoricTaskInstanceQueryTaskVariableValueEqualsOverwriteType()
	  {
		IProcessInstance processInstance = RuntimeService.StartProcessInstanceByKey("oneTaskProcess");
		ITask task = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First();

		// Set a long variable on a task
		TaskService.SetVariableLocal(task.Id, "var", 12345L);

		// Validate all variable-updates are present in DB
		Assert.AreEqual(1, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates(c=>c.Id == task.Id)*/.Count());

		// Query Historic task instances based on variable
		Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()/*//.TaskVariableValueEquals("var", 12345L)*/.Count());

		// Update the variables to an int variable
		TaskService.SetVariableLocal(task.Id, "var", 12345);

		// Validate all variable-updates are present in DB
		Assert.AreEqual(2, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates(c=>c.Id == task.Id)*/.Count());

		// The previous long value should not match
		Assert.AreEqual(0, HistoryService.CreateHistoricTaskInstanceQuery()/*//.TaskVariableValueEquals("var", 12345L)*/.Count());

		// The previous int value should not match
		Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()/*//.TaskVariableValueEquals("var", 12345)*/.Count());
	  }

        [Test]
        [Deployment]
        public virtual void TestHistoricTaskInstanceQueryVariableInParallelBranch()
	  {
		RuntimeService.StartProcessInstanceByKey("parallelGateway");

		// when there are two process variables of the same name but different types
		IExecution task1Execution = RuntimeService.CreateExecutionQuery(c=>c.ActivityId == "task1").First();
		RuntimeService.SetVariableLocal(task1Execution.Id, "var", 12345L);
		IExecution task2Execution = RuntimeService.CreateExecutionQuery(c=>c.ActivityId == "task2").First();
		RuntimeService.SetVariableLocal(task2Execution.Id, "var", 12345);

		// then the task query should be able to filter by both variables and return both tasks
		//Assert.AreEqual(2, HistoryService.CreateHistoricTaskInstanceQuery()//.ProcessVariableValueEquals("var", 12345).Count());
		//Assert.AreEqual(2, HistoryService.CreateHistoricTaskInstanceQuery()//.ProcessVariableValueEquals("var", 12345L).Count());
	  }

        [Test]
        [Deployment(new string[]{ "resources/standalone/history/FullHistoryTest.TestHistoricTaskInstanceQueryVariableInParallelBranch.bpmn20.xml"})]
        public virtual void TestHistoricTaskInstanceQueryVariableOfSameTypeInParallelBranch()
	  {
		RuntimeService.StartProcessInstanceByKey("parallelGateway");

		// when there are two process variables of the same name but different types
		IExecution task1Execution = RuntimeService.CreateExecutionQuery(c=>c.ActivityId == "task1").First();
		RuntimeService.SetVariableLocal(task1Execution.Id, "var", 12345L);
		IExecution task2Execution = RuntimeService.CreateExecutionQuery(c=>c.ActivityId == "task2").First();
		RuntimeService.SetVariableLocal(task2Execution.Id, "var", 45678L);

		// then the task query should be able to filter by both variables and return both tasks
		Assert.AreEqual(2, HistoryService.CreateHistoricTaskInstanceQuery()/*//.ProcessVariableValueEquals("var", 12345L)*/.Count());
		Assert.AreEqual(2, HistoryService.CreateHistoricTaskInstanceQuery()/*//.ProcessVariableValueEquals("var", 45678L)*/.Count());
        }
        [Test]
        [Deployment]
        public virtual void TestHistoricTaskInstanceQueryProcessVariableValueEquals()
	  {
		// Set some variables on the process instance
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["longVar"] = 12345L;
		variables["shortVar"] = (short) 123;
		variables["integerVar"] = 1234;
		variables["stringVar"] = "stringValue";
		variables["booleanVar"] = true;
		DateTime date = new DateTime();
		variables["dateVar"] = date;
		variables["nullVar"] = null;

		IProcessInstance processInstance = RuntimeService.StartProcessInstanceByKey("HistoricTaskInstanceTest", variables);
		ITask task = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First();

		// Validate all variable-updates are present in DB
		Assert.AreEqual(7, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*/.Where(c=>c.ProcessInstanceId==processInstance.Id).Count());

		// Query Historic task instances based on process variable
		Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()/*//.ProcessVariableValueEquals("longVar", 12345L)*/.Count());
		Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()/*//.ProcessVariableValueEquals("shortVar", (short) 123)*/.Count());
		Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()/*//.ProcessVariableValueEquals("integerVar",1234)*/.Count());
		Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()/*//.ProcessVariableValueEquals("stringVar","stringValue")*/.Count());
		//Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()//.ProcessVariableValueEquals("booleanVar", true).Count());
		//Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()//.ProcessVariableValueEquals("dateVar", date).Count());
		//Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()//.ProcessVariableValueEquals("nullVar", null).Count());

		// Update the variables
		variables["longVar"] = 67890L;
		variables["shortVar"] = (short) 456;
		variables["integerVar"] = 5678;
		variables["stringVar"] = "updatedStringValue";
		variables["booleanVar"] = false;
		DateTime otherCal = new DateTime();
		otherCal.AddDays(1);
		DateTime otherDate = otherCal;
		variables["dateVar"] = otherDate;
		variables["nullVar"] = null;

		RuntimeService.SetVariables(processInstance.Id, variables);

		// Validate all variable-updates are present in DB
		Assert.AreEqual(14, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*/.Where(c=>c.ProcessInstanceId==processInstance.Id).Count());

		// Previous values should NOT match
		Assert.AreEqual(0, HistoryService.CreateHistoricTaskInstanceQuery()/*//.ProcessVariableValueEquals("longVar", 12345L)*/.Count());
		Assert.AreEqual(0, HistoryService.CreateHistoricTaskInstanceQuery()/*//.ProcessVariableValueEquals("shortVar", (short) 123)*/.Count());
		Assert.AreEqual(0, HistoryService.CreateHistoricTaskInstanceQuery()/*//.ProcessVariableValueEquals("integerVar",1234)*/.Count());
		Assert.AreEqual(0, HistoryService.CreateHistoricTaskInstanceQuery()/*//.ProcessVariableValueEquals("stringVar","stringValue")*/.Count());
		//Assert.AreEqual(0, HistoryService.CreateHistoricTaskInstanceQuery()//.ProcessVariableValueEquals("booleanVar", true).Count());
		//Assert.AreEqual(0, HistoryService.CreateHistoricTaskInstanceQuery()//.ProcessVariableValueEquals("dateVar", date).Count());

		//// New values should match
		//Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()//.ProcessVariableValueEquals("longVar", 67890L).Count());
		//Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()//.ProcessVariableValueEquals("shortVar", (short) 456).Count());
		//Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()//.ProcessVariableValueEquals("integerVar",5678).Count());
		//Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()//.ProcessVariableValueEquals("stringVar","updatedStringValue").Count());
		//Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()//.ProcessVariableValueEquals("booleanVar", false).Count());
		//Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()//.ProcessVariableValueEquals("dateVar", otherDate).Count());
		//Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()//.ProcessVariableValueEquals("nullVar", null).Count());

		//// Set a task-variables, shouldn't affect the process-variable matches
		//TaskService.SetVariableLocal(task.Id, "longVar", 9999L);
		//Assert.AreEqual(0, HistoryService.CreateHistoricTaskInstanceQuery()//.ProcessVariableValueEquals("longVar", 9999L).Count());
		//Assert.AreEqual(1, HistoryService.CreateHistoricTaskInstanceQuery()//.ProcessVariableValueEquals("longVar", 67890L).Count());
	  }

        [Test]
        [Deployment]
        public virtual void TestHistoricProcessInstanceVariableValueEquals()
	  {
		// Set some variables on the process instance
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["longVar"] = 12345L;
		variables["shortVar"] = (short) 123;
		variables["integerVar"] = 1234;
		variables["stringVar"] = "stringValue";
		variables["booleanVar"] = true;
		DateTime date = new DateTime();
		variables["dateVar"] = date;
		variables["nullVar"] = null;

		IProcessInstance processInstance = RuntimeService.StartProcessInstanceByKey("HistoricProcessInstanceTest", variables);

		// Validate all variable-updates are present in DB
		Assert.AreEqual(7, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*/.Where(c=>c.ProcessInstanceId==processInstance.Id).Count());

        }
        [Test]
        [Deployment(new string[]{"resources/standalone/history/FullHistoryTest.TestHistoricProcessInstanceVariableValueEquals.bpmn20.xml"})] 
        public virtual void TestHistoricProcessInstanceVariableValueNotEquals()
	  {
		// Set some variables on the process instance
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["longVar"] = 12345L;
		variables["shortVar"] = (short) 123;
		variables["integerVar"] = 1234;
		variables["stringVar"] = "stringValue";
		variables["booleanVar"] = true;
		DateTime date = new DateTime();
		DateTime otherCal = new DateTime();
		otherCal.AddDays(1);
		DateTime otherDate = otherCal;
		variables["dateVar"] = date;
		variables["nullVar"] = null;

		IProcessInstance processInstance = RuntimeService.StartProcessInstanceByKey("HistoricProcessInstanceTest", variables);

		// Validate all variable-updates are present in DB
		Assert.AreEqual(7, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*/.Where(c=>c.ProcessInstanceId==processInstance.Id).Count());

		// Query Historic process instances based on process variable, shouldn't match
		Assert.AreEqual(0, HistoryService.CreateHistoricProcessInstanceQuery()/*.VariableValueNotEquals("longVar", 12345L)*/.Count());
		Assert.AreEqual(0, HistoryService.CreateHistoricProcessInstanceQuery()/*.VariableValueNotEquals("shortVar", (short) 123)*/.Count());
		Assert.AreEqual(0, HistoryService.CreateHistoricProcessInstanceQuery()/*.VariableValueNotEquals("integerVar",1234)*/.Count());
		Assert.AreEqual(0, HistoryService.CreateHistoricProcessInstanceQuery()/*.VariableValueNotEquals("stringVar","stringValue")*/.Count());
		Assert.AreEqual(0, HistoryService.CreateHistoricProcessInstanceQuery()/*.VariableValueNotEquals("booleanVar", true)*/.Count());
		Assert.AreEqual(0, HistoryService.CreateHistoricProcessInstanceQuery()/*.VariableValueNotEquals("dateVar", date)*/.Count());
		Assert.AreEqual(0, HistoryService.CreateHistoricProcessInstanceQuery()/*.VariableValueNotEquals("nullVar", null)*/.Count());

	  }

        [Test]
        [Deployment("resources/standalone/history/FullHistoryTest.TestHistoricProcessInstanceVariableValueEquals.bpmn20.xml")]
       public virtual void TestHistoricProcessInstanceVariableValueLessThanAndGreaterThan()
	  {
		// Set some variables on the process instance
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["longVar"] = 12345L;

		IProcessInstance processInstance = RuntimeService.StartProcessInstanceByKey("HistoricProcessInstanceTest", variables);

		// Validate all variable-updates are present in DB
		Assert.AreEqual(1, HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*/.Where(c=>c.ProcessInstanceId==processInstance.Id).Count());

		Assert.AreEqual(0, HistoryService.CreateHistoricProcessInstanceQuery()/*.VariableValueGreaterThan("longVar", 12345L)*/.Count());
	  }
        [Test]
        [Deployment("resources/standalone/history/FullHistoryTest.TestVariableUpdatesAreLinkedToActivity.bpmn20.xml")]
       public virtual void TestVariableUpdatesLinkedToActivity()
	  {
		IProcessInstance pi = RuntimeService.StartProcessInstanceByKey("ProcessWithSubProcess");

		ITask task = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi.Id).First();
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["test"] = "1";
		TaskService.Complete(task.Id, variables);

		// now we are in the subprocess
		task = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi.Id).First();
		variables.Clear();
		variables["test"] = "2";
		TaskService.Complete(task.Id, variables);

		// now we are ended
		TestHelper.AssertProcessEnded(pi.Id);

		// check history
		IList<IHistoricDetail> updates = HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*/.ToList();
		Assert.AreEqual(2, updates.Count);

		IDictionary<string, IHistoricVariableUpdate> updatesMap = new Dictionary<string, IHistoricVariableUpdate>();
		IHistoricVariableUpdate update = (IHistoricVariableUpdate) updates[0];
		updatesMap[(string)update.Value] = update;
		update = (IHistoricVariableUpdate) updates[1];
		updatesMap[(string)update.Value] = update;

		IHistoricVariableUpdate update1 = updatesMap["1"];
		IHistoricVariableUpdate update2 = updatesMap["2"];

		Assert.NotNull(update1.ActivityInstanceId);
		Assert.NotNull(update1.ExecutionId);
		IHistoricActivityInstance historicActivityInstance1 = HistoryService.CreateHistoricActivityInstanceQuery(c=>c.ActivityInstanceId == update1.ActivityInstanceId).First();
		Assert.AreEqual(historicActivityInstance1.ExecutionId, update1.ExecutionId);
		Assert.AreEqual("usertask1", historicActivityInstance1.ActivityId);

		Assert.NotNull(update2.ActivityInstanceId);
		IHistoricActivityInstance historicActivityInstance2 = HistoryService.CreateHistoricActivityInstanceQuery(c=>c.ActivityInstanceId == update2.ActivityInstanceId).First();
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
        [Test]
        [Deployment("resources/history/oneTaskProcess.bpmn20.xml")] 
        public virtual void TestHistoricDetailQueryByVariableInstanceId()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["testVar"] = "testValue";
		RuntimeService.StartProcessInstanceByKey("oneTaskProcess", @params);

		IHistoricVariableInstance testVariable = HistoryService.CreateHistoricVariableInstanceQuery()/*.VariableName("testVar")*/.First();

		IQueryable<IHistoricDetail> query = HistoryService.CreateHistoricDetailQuery();

		//query//.VariableInstanceId(testVariable.Id);

		Assert.AreEqual(1, query.Count());
		Assert.AreEqual(1, query.Count());
	  }
        [Test]
      public virtual void TestHistoricDetailQueryByInvalidVariableInstanceId()
	  {
		IQueryable<IHistoricDetail> query = HistoryService.CreateHistoricDetailQuery();

		//query//.VariableInstanceId("invalid");
		Assert.AreEqual(0, query.Count());

		try
		{
		  //query//.VariableInstanceId(null);
		  Assert.Fail("A ProcessEngineExcpetion was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  //query//.VariableInstanceId((string)null);
		  Assert.Fail("A ProcessEngineExcpetion was expected.");
		}
		catch (ProcessEngineException)
		{
		}
	  }
        [Test]
        [Deployment]
        public virtual void TestHistoricDetailActivityInstanceIdForInactiveScopeExecution()
	  {

		IProcessInstance pi = RuntimeService.StartProcessInstanceByKey("testProcess");

		RuntimeService.SetVariable(pi.Id, "foo", "bar");

		IHistoricDetail historicDetail = HistoryService.CreateHistoricDetailQuery().First();
		Assert.NotNull(historicDetail.ActivityInstanceId);
	  }

        [Test]
        public virtual void TestHistoricDetailQueryById()
	  {

		ITask newTask = TaskService.NewTask();
		TaskService.SaveTask(newTask);

		string variableName = "someName";
		string variableValue = "someValue";
		TaskService.SetVariable(newTask.Id, variableName, variableValue);

		IHistoricDetail result = HistoryService.CreateHistoricDetailQuery().First();

		IHistoricDetail resultById = HistoryService.CreateHistoricDetailQuery()/*.DetailId(result.Id)*/.First();
		Assert.NotNull(resultById);
		Assert.AreEqual(result.Id, resultById.Id);
		Assert.AreEqual(variableName, ((IHistoricVariableUpdate)resultById).VariableName);
		Assert.AreEqual(variableValue, ((IHistoricVariableUpdate)resultById).Value);
		Assert.AreEqual(ValueTypeFields.String.Name, ((IHistoricVariableUpdate)resultById).VariableTypeName);
		Assert.AreEqual(ValueTypeFields.String.Name, ((IHistoricVariableUpdate)resultById).TypeName);

		TaskService.DeleteTask(newTask.Id, true);
	  }

        [Test]
        public virtual void TestHistoricDetailQueryByNonExistingId()
	  {

		ITask newTask = TaskService.NewTask();
		TaskService.SaveTask(newTask);

		string variableName = "someName";
		string variableValue = "someValue";
		TaskService.SetVariable(newTask.Id, variableName, variableValue);

		IHistoricDetail result = HistoryService.CreateHistoricDetailQuery(c=>c.Id== "non-existing").First();
		Assert.IsNull(result);

		TaskService.DeleteTask(newTask.Id, true);
	  }

        [Test]
        public virtual void TestBinaryFetchingEnabled()
	  {

		// by default, binary fetching is enabled

		ITask newTask = TaskService.NewTask();
		TaskService.SaveTask(newTask);

		string variableName = "binaryVariableName";
		TaskService.SetVariable(newTask.Id, variableName, "some bytes".GetBytes());

		IHistoricDetail result = HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*/.First();

		Assert.NotNull(((IHistoricVariableUpdate)result).Value);

		TaskService.DeleteTask(newTask.Id, true);
	  }

        [Test]
        public virtual void TestBinaryFetchingDisabled()
	  {

		ITask newTask = TaskService.NewTask();
		TaskService.SaveTask(newTask);

		string variableName = "binaryVariableName";
		TaskService.SetVariable(newTask.Id, variableName, "some bytes".GetBytes());

		IHistoricDetail result = HistoryService.CreateHistoricDetailQuery()/*.DisableBinaryFetching()*//*.VariableUpdates()*/.First();

		Assert.IsNull(((IHistoricVariableUpdate)result).Value);

		TaskService.DeleteTask(newTask.Id, true);
	  }

        [Test]
        [Deployment( "resources/api/runtime/oneTaskProcess.bpmn20.xml")]
        public virtual void TestDisableBinaryFetchingForFileValues()
	  {
		// given
		string fileName = "text.Txt";
		string encoding = "crazy-encoding";
		string mimeType = "martini/dry";

		//IFileValue fileValue = Variables.FileValue(fileName).File("ABC".GetBytes()).Encoding(encoding).MimeType(mimeType).Create();

		//RuntimeService.StartProcessInstanceByKey("oneTaskProcess", Variable.Variables.CreateVariables().PutValueTyped("fileVar", fileValue));

		// when enabling binary fetching
		IHistoricVariableUpdate fileVariableInstance = (IHistoricVariableUpdate) HistoryService.CreateHistoricDetailQuery().First();

		// then the binary value is accessible
		Assert.NotNull(fileVariableInstance.Value);

		// when disabling binary fetching
		fileVariableInstance = (IHistoricVariableUpdate) HistoryService.CreateHistoricDetailQuery()/*/*.DisableBinaryFetching()*/.First();

		// then the byte value is not fetched
		Assert.NotNull(fileVariableInstance);
		Assert.AreEqual("fileVar", fileVariableInstance.VariableName);

		Assert.IsNull(fileVariableInstance.Value);

		IFileValue typedValue = (IFileValue) fileVariableInstance.TypedValue;
		Assert.IsNull(typedValue.Value);

		// but typed value metadata is accessible
		Assert.AreEqual(ValueTypeFields.File, typedValue.Type);
		Assert.AreEqual(fileName, typedValue.Filename);
		Assert.AreEqual(encoding, typedValue.Encoding);
		Assert.AreEqual(mimeType, typedValue.MimeType);

	  }

        [Test]
        public virtual void TestDisableCustomObjectDeserialization()
	  {

		ITask newTask = TaskService.NewTask();
		TaskService.SaveTask(newTask);

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["customSerializable"] = new CustomSerializable();
		variables["failingSerializable"] = new FailingSerializable();
		TaskService.SetVariables(newTask.Id, variables);

		IList<IHistoricDetail> results = HistoryService.CreateHistoricDetailQuery()/*.DisableBinaryFetching()*//*.DisableCustomObjectDeserialization()*//*.VariableUpdates()*/.ToList();

		// both variables are not deserialized, but their serialized values are available
		Assert.AreEqual(2, results.Count);

		foreach (IHistoricDetail update in results)
		{
		  IHistoricVariableUpdate variableUpdate = (IHistoricVariableUpdate) update;
		  Assert.IsNull(variableUpdate.ErrorMessage);

		  IObjectValue typedValue = (IObjectValue) variableUpdate.TypedValue;
		  Assert.NotNull(typedValue);
		  Assert.IsFalse(typedValue.IsDeserialized);
		  // cannot access the deserialized value
		  try
		  {
			//typedValue.Value;
		  }
		  catch (System.InvalidOperationException e)
		  {
			Assert.That(e.Message,Does.Contain("Object is not deserialized"));
		  }
		  Assert.NotNull(typedValue.ValueSerialized);
		}

		TaskService.DeleteTask(newTask.Id, true);
	  }

        [Test]
        public virtual void TestErrorMessage()
	  {

		ITask newTask = TaskService.NewTask();
		TaskService.SaveTask(newTask);

		string variableName = "failingSerializable";
		TaskService.SetVariable(newTask.Id, variableName, new FailingSerializable());

		IHistoricDetail result = HistoryService.CreateHistoricDetailQuery()/*.DisableBinaryFetching()*//*.VariableUpdates()*/.First();

		Assert.IsNull(((IHistoricVariableUpdate)result).Value);
		Assert.NotNull(((IHistoricVariableUpdate)result).ErrorMessage);

		TaskService.DeleteTask(newTask.Id, true);

	  }

        [Test]
        public virtual void TestVariableInstance()
	  {

		ITask newTask = TaskService.NewTask();
		TaskService.SaveTask(newTask);

		string variableName = "someName";
		string variableValue = "someValue";
		TaskService.SetVariable(newTask.Id, variableName, variableValue);

		IVariableInstance variable = RuntimeService.CreateVariableInstanceQuery().First();
		Assert.NotNull(variable);

		IHistoricVariableUpdate result = (IHistoricVariableUpdate) HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*/.First();
		Assert.NotNull(result);

		Assert.AreEqual(variable.Id, result.VariableInstanceId);

		TaskService.DeleteTask(newTask.Id, true);
	  }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")] 
        public virtual void TestHistoricVariableUpdateProcessDefinitionProperty()
	  {
		// given
		string key = "oneTaskProcess";
		IProcessInstance processInstance = RuntimeService.StartProcessInstanceByKey(key);

		string processInstanceId = processInstance.Id;
		string taskId = TaskService.CreateTaskQuery().First().Id;

		RuntimeService.SetVariable(processInstanceId, "aVariable", "aValue");
		TaskService.SetVariableLocal(taskId, "aLocalVariable", "anotherValue");

		string firstVariable = RuntimeService.CreateVariableInstanceQuery()/*.VariableName("aVariable")*/.First().Id;

		string secondVariable = RuntimeService.CreateVariableInstanceQuery()/*.VariableName("aLocalVariable")*/.First().Id;

		// when (1)
		IHistoricVariableUpdate instance = (IHistoricVariableUpdate) HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*//*//.VariableInstanceId(firstVariable)*/.First();

		// then (1)
		Assert.NotNull(instance.ProcessDefinitionKey);
		Assert.AreEqual(key, instance.ProcessDefinitionKey);

		Assert.NotNull(instance.ProcessDefinitionId);
		Assert.AreEqual(processInstance.ProcessDefinitionId, instance.ProcessDefinitionId);

		Assert.IsNull(instance.CaseDefinitionKey);
		Assert.IsNull(instance.CaseDefinitionId);

		// when (2)
		instance = (IHistoricVariableUpdate) HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*//*//.VariableInstanceId(secondVariable)*/.First();

		// then (2)
		Assert.NotNull(instance.ProcessDefinitionKey);
		Assert.AreEqual(key, instance.ProcessDefinitionKey);

		Assert.NotNull(instance.ProcessDefinitionId);
		Assert.AreEqual(processInstance.ProcessDefinitionId, instance.ProcessDefinitionId);

		Assert.IsNull(instance.CaseDefinitionKey);
		Assert.IsNull(instance.CaseDefinitionId);
	  }
        [Test]
        [Deployment("resources/api/cmmn/oneTaskCase.cmmn")] 
        public virtual void TestHistoricVariableUpdateCaseDefinitionProperty()
	  {
		// given
		string key = "oneTaskCase";
		ICaseInstance caseInstance = CaseService.CreateCaseInstanceByKey(key);

		string caseInstanceId = caseInstance.Id;

		string humanTask = CaseService.CreateCaseExecutionQuery(c=>c.ActivityId == "PI_HumanTask_1").First().Id;
		string taskId = TaskService.CreateTaskQuery().First().Id;

		CaseService.SetVariable(caseInstanceId, "aVariable", "aValue");
		TaskService.SetVariableLocal(taskId, "aLocalVariable", "anotherValue");

		string firstVariable = RuntimeService.CreateVariableInstanceQuery()/*.VariableName("aVariable")*/.First().Id;

		string secondVariable = RuntimeService.CreateVariableInstanceQuery()/*.VariableName("aLocalVariable")*/.First().Id;


		// when (1)
		IHistoricVariableUpdate instance = (IHistoricVariableUpdate) HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*//*//.VariableInstanceId(firstVariable)*/.First();

		// then (1)
		Assert.NotNull(instance.CaseDefinitionKey);
		Assert.AreEqual(key, instance.CaseDefinitionKey);

		Assert.NotNull(instance.CaseDefinitionId);
		Assert.AreEqual(caseInstance.CaseDefinitionId, instance.CaseDefinitionId);

		Assert.IsNull(instance.ProcessDefinitionKey);
		Assert.IsNull(instance.ProcessDefinitionId);

		// when (2)
		instance = (IHistoricVariableUpdate) HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*//*//.VariableInstanceId(secondVariable)*/.First();

		// then (2)
		Assert.NotNull(instance.CaseDefinitionKey);
		Assert.AreEqual(key, instance.CaseDefinitionKey);

		Assert.NotNull(instance.CaseDefinitionId);
		Assert.AreEqual(caseInstance.CaseDefinitionId, instance.CaseDefinitionId);

		Assert.IsNull(instance.ProcessDefinitionKey);
		Assert.IsNull(instance.ProcessDefinitionId);
	  }

        [Test]
        public virtual void TestHistoricVariableUpdateStandaloneTaskDefinitionProperties()
	  {
		// given
		string taskId = "myTask";
		ITask task = TaskService.NewTask(taskId);
		TaskService.SaveTask(task);

		TaskService.SetVariable(taskId, "aVariable", "anotherValue");

		string firstVariable = RuntimeService.CreateVariableInstanceQuery()/*.VariableName("aVariable")*/.First().Id;

		// when
		IHistoricVariableUpdate instance = (IHistoricVariableUpdate) HistoryService.CreateHistoricDetailQuery()/*.VariableUpdates()*//*//.VariableInstanceId(firstVariable)*/.First();

		// then
		Assert.IsNull(instance.ProcessDefinitionKey);
		Assert.IsNull(instance.ProcessDefinitionId);
		Assert.IsNull(instance.CaseDefinitionKey);
		Assert.IsNull(instance.CaseDefinitionId);

		TaskService.DeleteTask(taskId, true);
	  }

        [Test][Deployment( "resources/api/oneTaskProcess.bpmn20.xml")] 
        public virtual void TestHistoricFormFieldProcessDefinitionProperty()
	  {
		// given
		string key = "oneTaskProcess";
		IProcessInstance processInstance = RuntimeService.StartProcessInstanceByKey(key);

		string taskId = TaskService.CreateTaskQuery().First().Id;

		//FormService.SubmitTaskForm(taskId, Variable.Variables.CreateVariables().PutValue("aVariable", "aValue"));

		// when
		//IHistoricFormField instance = (IHistoricFormField) HistoryService.CreateHistoricDetailQuery().FormFields().First();

		// then
		//Assert.NotNull(instance.ProcessDefinitionKey);
		//Assert.AreEqual(key, instance.ProcessDefinitionKey);

		//Assert.NotNull(instance.ProcessDefinitionId);
		//Assert.AreEqual(processInstance.ProcessDefinitionId, instance.ProcessDefinitionId);

		//Assert.IsNull(instance.CaseDefinitionKey);
		//Assert.IsNull(instance.CaseDefinitionId);
	  }

        [Test][Deployment("resources/api/oneTaskProcess.bpmn20.xml")] 
        public virtual void TestDeleteProcessInstanceSkipCustomListener()
	  {
		// given
		string processInstanceId = RuntimeService.StartProcessInstanceByKey("oneTaskProcess").Id;

		// when
		RuntimeService.DeleteProcessInstance(processInstanceId, null, true);

		// then
		IHistoricProcessInstance instance = HistoryService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId == processInstanceId).First();
		Assert.NotNull(instance);

		Assert.AreEqual(processInstanceId, instance.Id);
		Assert.NotNull(instance.EndTime);
	  }

	}

}