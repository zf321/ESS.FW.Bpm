//using System;
//using System.Collections.Generic;



//namespace ESS.FW.Bpm.Engine.Tests.History
//{

//	/ <summary>
//	/ 
//	/ </summary>
//[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_AUDIT)

//    public class HistoricCaseInstanceTest : CmmnProcessEngineTestCase
//    {
//        JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: Deployment(new string[]{ "resources/api/cmmn/emptyStageCase.cmmn"}) public void testCaseInstanceProperties()

//        public virtual void testCaseInstanceProperties()
//        {
//            ICaseInstance caseInstance = createCaseInstance();

//            IHistoricCaseInstance historicInstance = queryHistoricCaseInstance(caseInstance.Id);

//            Assert case instance properties are set correctly

//        Assert.AreEqual(caseInstance.Id, historicInstance.Id);
//            Assert.AreEqual(caseInstance.BusinessKey, historicInstance.BusinessKey);
//            Assert.AreEqual(caseInstance.CaseDefinitionId, historicInstance.CaseDefinitionId);
//        }

//        JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: Deployment(new string[]{ "resources/api/cmmn/emptyStageWithManualActivationCase.cmmn"}) public void testCaseInstanceStates()

//      public virtual void testCaseInstanceStates()
//        {
//            string caseInstanceId = createCaseInstance().Id;

//            IHistoricCaseInstance historicCaseInstance = queryHistoricCaseInstance(caseInstanceId);

//            Assert.True(historicCaseInstance.Active);
//            AssertCount(1, historicQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode));
//            AssertCount(1, historicQuery().NotClosed());

//            start empty stage to complete case instance

//        string stageExecutionId = queryCaseExecutionByActivityId("PI_Stage_1").Id;
//            manualStart(stageExecutionId);

//            historicCaseInstance = queryHistoricCaseInstance(caseInstanceId);
//            Assert.True(historicCaseInstance.Completed);
//            AssertCount(1, historicQuery().Completed());
//            AssertCount(1, historicQuery().NotClosed());

//            reactive and terminate case instance

//        reactivate(caseInstanceId);
//            terminate(caseInstanceId);

//            historicCaseInstance = queryHistoricCaseInstance(caseInstanceId);
//            Assert.True(historicCaseInstance.Terminated);
//            AssertCount(1, historicQuery().Terminated());
//            AssertCount(1, historicQuery().NotClosed());

//            reactive and suspend case instance

//        reactivate(caseInstanceId);
//            suspend(caseInstanceId);

//            historicCaseInstance = queryHistoricCaseInstance(caseInstanceId);
//            not public API
//           Assert.True(((HistoricCaseInstanceEntity) historicCaseInstance).Suspended);

//        AssertCount(1, historicQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode));

//        AssertCount(1, historicQuery().NotClosed());

//        close case instance
//       close(caseInstanceId);

//        historicCaseInstance = queryHistoricCaseInstance(caseInstanceId);
//        Assert.True(historicCaseInstance.Closed);

//        AssertCount(1, historicQuery().Closed());

//        AssertCount(0, historicQuery().NotClosed());
//    }

//    JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: Deployment(new string[]{ "resources/api/cmmn/emptyStageWithManualActivationCase.cmmn"}) public void testHistoricCaseInstanceDates()

//      public virtual void testHistoricCaseInstanceDates()
//    {
//        create test dates

//        long duration = 72 * 3600 * 1000;
//        DateTime created = ClockUtil.CurrentTime;
//        DateTime closed = new DateTime(created.Ticks + duration);

//        create instance

//        ClockUtil.CurrentTime = created;
//        string caseInstanceId = createCaseInstance().Id;

//        terminate(caseInstanceId);

//        close instance

//        ClockUtil.CurrentTime = closed;
//        close(caseInstanceId);

//        IHistoricCaseInstance historicCaseInstance = queryHistoricCaseInstance(caseInstanceId);

//        read historic dates ignoring milliseconds
//       DateTime createTime = historicCaseInstance.CreateTime;
//        DateTime closeTime = historicCaseInstance.CloseTime;
//        long? durationInMillis = historicCaseInstance.DurationInMillis;

//        AssertDateSimilar(created, createTime);
//        AssertDateSimilar(closed, closeTime);

//        test that duration is as expected with a maximal difference of one second

//        Assert.True(durationInMillis >= duration);
//        Assert.True(durationInMillis < duration + 1000);

//        test queries

//        DateTime beforeCreate = new DateTime(created.Ticks - 3600 * 1000);
//        DateTime afterClose = new DateTime(closed.Ticks + 3600 * 1000);

//        AssertCount(1, historicQuery().CreatedAfter(beforeCreate));
//        AssertCount(0, historicQuery().CreatedAfter(closed));

//        AssertCount(0, historicQuery().CreatedBefore(beforeCreate));
//        AssertCount(1, historicQuery().CreatedBefore(closed));

//        AssertCount(0, historicQuery().CreatedBefore(beforeCreate).CreatedAfter(closed));

//        AssertCount(1, historicQuery().ClosedAfter(created));
//        AssertCount(0, historicQuery().ClosedAfter(afterClose));

//        AssertCount(0, historicQuery().ClosedBefore(created));
//        AssertCount(1, historicQuery().ClosedBefore(afterClose));

//        AssertCount(0, historicQuery().ClosedBefore(created).ClosedAfter(afterClose));

//        AssertCount(1, historicQuery().ClosedBefore(afterClose).ClosedAfter(created));
//    }

//    JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: Deployment(new string[]{ "resources/api/cmmn/emptyStageCase.cmmn"}) public void testCreateUser()

//      public virtual void testCreateUser()
//    {
//        string userId = "test";
//        identityService.AuthenticatedUserId = userId;

//        string caseInstanceId = createCaseInstance().Id;

//        IHistoricCaseInstance historicCaseInstance = queryHistoricCaseInstance(caseInstanceId);
//        Assert.AreEqual(userId, historicCaseInstance.CreateUserId);
//        AssertCount(1, historicQuery().CreatedBy(userId));

//        identityService.AuthenticatedUserId = null;
//    }

//    JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(new []{"resources/api/cmmn/oneCaseTaskCase.cmmn", "resources/api/cmmn/oneTaskCase.cmmn" }) public void testSuperCaseInstance()

//      public virtual void testSuperCaseInstance()
//    {
//        string caseInstanceId = createCaseInstanceByKey("oneCaseTaskCase").Id;
//        queryCaseExecutionByActivityId("PI_CaseTask_1").Id;

//        IHistoricCaseInstance historicCaseInstance = historicQuery().SuperCaseInstanceId(caseInstanceId).First();

//        Assert.NotNull(historicCaseInstance);
//        Assert.AreEqual(caseInstanceId, historicCaseInstance.SuperCaseInstanceId);

//        string superCaseInstanceId = historicQuery().SubCaseInstanceId(historicCaseInstance.Id).First().Id;

//        Assert.AreEqual(caseInstanceId, superCaseInstanceId);
//    }

//    JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(new []{"resources/api/cmmn/oneTaskCase.cmmn", "resources/api/cmmn/twoTaskCase.cmmn", "resources/api/repository/three_.cmmn" }) public void testHistoricCaseInstanceQuery()

//      public virtual void testHistoricCaseInstanceQuery()
//    {
//        ICaseInstance oneTaskCase = createCaseInstanceByKey("oneTaskCase", "oneBusiness");
//        ICaseInstance twoTaskCase = createCaseInstanceByKey("twoTaskCase", "twoBusiness");
//        createCaseInstanceByKey("xyz_", "xyz_");

//        AssertCount(1, historicQuery(c=>c.CaseInstanceId ==oneTaskCase.Id));
//        AssertCount(1, historicQuery(c=>c.CaseInstanceId ==twoTaskCase.Id));

//        ISet<string> caseInstanceIds = new HashSet<string>();
//        caseInstanceIds.Add(oneTaskCase.Id);
//        caseInstanceIds.Add("unknown1");
//        caseInstanceIds.Add(twoTaskCase.Id);
//        caseInstanceIds.Add("unknown2");

//        AssertCount(2, historicQuery().CaseInstanceIds(caseInstanceIds));
//        AssertCount(0, historicQuery().CaseInstanceIds(caseInstanceIds).CaseInstanceId("someOtherId"));

//        AssertCount(1, historicQuery(c=>c.CaseDefinitionId== oneTaskCase.CaseDefinitionId));

//        AssertCount(1, historicQuery().CaseDefinitionKey("oneTaskCase"));

//        AssertCount(3, historicQuery().CaseDefinitionKeyNotIn("unknown"));
//        AssertCount(2, historicQuery().CaseDefinitionKeyNotIn("oneTaskCase"));
//        AssertCount(1, historicQuery().CaseDefinitionKeyNotIn("oneTaskCase", "twoTaskCase"));
//        AssertCount(0, historicQuery().CaseDefinitionKeyNotIn("oneTaskCase").CaseDefinitionKey("oneTaskCase"));


//        try
//        {
//            oracle handles empty string like null which seems to lead to undefined behavior of the LIKE comparison

//          historicQuery().CaseDefinitionKeyNotIn("");
//            Assert.Fail("Exception expected");
//        }
//        catch (NotValidException)
//        {
//            expected

//        }


//        AssertCount(1, historicQuery().CaseDefinitionName("One ITask Case"));

//        AssertCount(2, historicQuery().CaseDefinitionNameLike("%T%"));
//        AssertCount(1, historicQuery().CaseDefinitionNameLike("One%"));
//        AssertCount(0, historicQuery().CaseDefinitionNameLike("%Process%"));
//        AssertCount(1, historicQuery().CaseDefinitionNameLike("%z\\_"));

//        AssertCount(1, historicQuery().CaseInstanceBusinessKey("oneBusiness"));

//        AssertCount(2, historicQuery().CaseInstanceBusinessKeyLike("%Business"));
//        AssertCount(1, historicQuery().CaseInstanceBusinessKeyLike("one%"));
//        AssertCount(0, historicQuery().CaseInstanceBusinessKeyLike("%unknown%"));
//        AssertCount(1, historicQuery().CaseInstanceBusinessKeyLike("%z\\_"));
//    }

//    JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(new []{"resources/api/cmmn/oneTaskCase.cmmn"}) public void testQueryByVariable()

//      public virtual void testQueryByVariable()
//    {
//        string caseInstanceId = createCaseInstance().Id;
//        caseService.SetVariable(caseInstanceId, "foo", "bar");
//        caseService.SetVariable(caseInstanceId, "number", 10);

//        AssertCount(1, historicQuery()//.VariableValueEquals("foo", "bar"));
//        AssertCount(0, historicQuery().VariableValueNotEquals("foo", "bar"));
//        AssertCount(1, historicQuery().VariableValueNotEquals("foo", "lol"));
//        AssertCount(0, historicQuery()//.VariableValueEquals("foo", "lol"));
//        AssertCount(1, historicQuery().VariableValueLike("foo", "%a%"));
//        AssertCount(0, historicQuery().VariableValueLike("foo", "%lol%"));

//        AssertCount(1, historicQuery()//.VariableValueEquals("number", 10));
//        AssertCount(0, historicQuery().VariableValueNotEquals("number", 10));
//        AssertCount(1, historicQuery().VariableValueNotEquals("number", 1));
//        AssertCount(1, historicQuery().VariableValueGreaterThan("number", 1));
//        AssertCount(0, historicQuery().VariableValueLessThan("number", 1));
//        AssertCount(1, historicQuery().VariableValueGreaterThanOrEqual("number", 10));
//        AssertCount(0, historicQuery().VariableValueLessThan("number", 10));
//        AssertCount(1, historicQuery().VariableValueLessThan("number", 20));
//        AssertCount(0, historicQuery().VariableValueGreaterThan("number", 20));
//        AssertCount(1, historicQuery().VariableValueLessThanOrEqual("number", 10));
//        AssertCount(0, historicQuery().VariableValueGreaterThan("number", 10));
//    }

//    JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: Deployment(new string[] "resources/api/cmmn/oneTaskCase.cmmn") public void testCaseVariableValueEqualsNumber() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//	  public virtual void testCaseVariableValueEqualsNumber()
//    {
//        long

//        caseService.WithCaseDefinitionByKey("oneTaskCase").SetVariable("var", 123L).Create();

//        non - matching long

//        caseService.WithCaseDefinitionByKey("oneTaskCase").SetVariable("var", 12345L).Create();

//        short

//        caseService.WithCaseDefinitionByKey("oneTaskCase").SetVariable("var", (short)123).Create();

//        double

//        caseService.WithCaseDefinitionByKey("oneTaskCase").SetVariable("var", 123.0d).Create();

//        integer

//        caseService.WithCaseDefinitionByKey("oneTaskCase").SetVariable("var", 123).Create();

//        untyped null(should not match)

//        caseService.WithCaseDefinitionByKey("oneTaskCase").SetVariable("var", null).Create();

//        typed null(should not match)

//        caseService.WithCaseDefinitionByKey("oneTaskCase").SetVariable("var", Variables.LongValue(null)).Create();

//        caseService.WithCaseDefinitionByKey("oneTaskCase").SetVariable("var", "123").Create();

//        Assert.AreEqual(4, historicQuery()//.VariableValueEquals("var", Engine.Variable.Variables.NumberValue(123)).Count());
//        Assert.AreEqual(4, historicQuery()//.VariableValueEquals("var", Engine.Variable.Variables.NumberValue(123L)).Count());
//        Assert.AreEqual(4, historicQuery()//.VariableValueEquals("var", Engine.Variable.Variables.NumberValue(123.0d)).Count());
//        Assert.AreEqual(4, historicQuery()//.VariableValueEquals("var", Engine.Variable.Variables.NumberValue((short)123)).Count());

//        Assert.AreEqual(1, historicQuery()//.VariableValueEquals("var", Engine.Variable.Variables.NumberValue(null)).Count());
//    }


//    JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(new []{"resources/api/cmmn/oneTaskCase.cmmn"}) public void testQueryPaging()

//      public virtual void testQueryPaging()
//    {
//        createCaseInstance();
//        createCaseInstance();
//        createCaseInstance();
//        createCaseInstance();

//        Assert.AreEqual(3, historicQuery().ListPage(0, 3).Count);
//        Assert.AreEqual(2, historicQuery().ListPage(2, 2).Count);
//        Assert.AreEqual(1, historicQuery().ListPage(3, 2).Count);
//    }

//    JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(new []{"resources/api/cmmn/oneTaskCase.cmmn", "resources/api/cmmn/twoTaskCase.cmmn" }) @SuppressWarnings("unchecked") public void testQuerySorting()

//      public virtual void testQuerySorting()
//    {
//        string oneCaseInstanceId = createCaseInstanceByKey("oneTaskCase", "oneBusinessKey").Id;
//        string twoCaseInstanceId = createCaseInstanceByKey("twoTaskCase", "twoBusinessKey").Id;

//        terminate and close case instances => close time and duration is set
//       terminate(oneCaseInstanceId);
//        close(oneCaseInstanceId);
//        set time ahead to get different durations

//        ClockUtil.CurrentTime = DateTimeUtil.Now().plusHours(1);
//        terminate(twoCaseInstanceId);
//        close(twoCaseInstanceId);

//        IHistoricCaseInstance oneCaseInstance = queryHistoricCaseInstance(oneCaseInstanceId);
//        IHistoricCaseInstance twoCaseInstance = queryHistoricCaseInstance(twoCaseInstanceId);

//        sort by case instance ids

//        string property = "id";
//        JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: List <? extends Comparable > sortedList = (oneCaseInstance.GetId(), twoCaseInstance.GetId());
//        IList<IComparable> sortedList = oneCaseInstance.Id, twoCaseInstance.Id;
//        sortedList.Sort();

//        IList<IHistoricCaseInstance> instances = historicQuery()//.OrderByCaseInstanceId()/*.Asc()*/.ToList();
//        Assert.AreEqual(2, instances.Count);
//        Assert.That(instances, contains(hasProperty(property, equalTo(sortedList[0])), hasProperty(property, equalTo(sortedList[1]))));

//        instances = historicQuery()//.OrderByCaseInstanceId()/*.Desc()*/.ToList();
//        Assert.AreEqual(2, instances.Count);
//        Assert.That(instances, contains(hasProperty(property, equalTo(sortedList[1])), hasProperty(property, equalTo(sortedList[0]))));

//        sort by case definition ids

//        property = "caseDefinitionId";
//        sortedList = oneCaseInstance.CaseDefinitionId, twoCaseInstance.CaseDefinitionId;
//        sortedList.Sort();

//        instances = historicQuery()//.OrderByCaseDefinitionId()/*.Asc()*/.ToList();
//        Assert.AreEqual(2, instances.Count);
//        Assert.That(instances, contains(hasProperty(property, equalTo(sortedList[0])), hasProperty(property, equalTo(sortedList[1]))));

//        instances = historicQuery()//.OrderByCaseDefinitionId()/*.Desc()*/.ToList();
//        Assert.AreEqual(2, instances.Count);
//        Assert.That(instances, contains(hasProperty(property, equalTo(sortedList[1])), hasProperty(property, equalTo(sortedList[0]))));

//        sort by business keys

//        property = "businessKey";
//        sortedList = oneCaseInstance.BusinessKey, twoCaseInstance.BusinessKey;
//        sortedList.Sort();

//        instances = historicQuery().OrderByCaseInstanceBusinessKey()/*.Asc()*/.ToList();
//        Assert.AreEqual(2, instances.Count);
//        Assert.That(instances, contains(hasProperty(property, equalTo(sortedList[0])), hasProperty(property, equalTo(sortedList[1]))));

//        instances = historicQuery().OrderByCaseInstanceBusinessKey()/*.Desc()*/.ToList();
//        Assert.AreEqual(2, instances.Count);
//        Assert.That(instances, contains(hasProperty(property, equalTo(sortedList[1])), hasProperty(property, equalTo(sortedList[0]))));

//        sort by create time

//        property = "createTime";
//        sortedList = oneCaseInstance.CreateTime, twoCaseInstance.CreateTime;
//        sortedList.Sort();

//        instances = historicQuery().OrderByCaseInstanceCreateTime()/*.Asc()*/.ToList();
//        Assert.AreEqual(2, instances.Count);
//        Assert.That(instances, contains(hasProperty(property, equalTo(sortedList[0])), hasProperty(property, equalTo(sortedList[1]))));

//        instances = historicQuery().OrderByCaseInstanceCreateTime()/*.Desc()*/.ToList();
//        Assert.AreEqual(2, instances.Count);
//        Assert.That(instances, contains(hasProperty(property, equalTo(sortedList[1])), hasProperty(property, equalTo(sortedList[0]))));

//        sort by close time

//        property = "closeTime";
//        sortedList = oneCaseInstance.CloseTime, twoCaseInstance.CloseTime;
//        sortedList.Sort();

//        instances = historicQuery().OrderByCaseInstanceCloseTime()/*.Asc()*/.ToList();
//        Assert.AreEqual(2, instances.Count);
//        Assert.That(instances, contains(hasProperty(property, equalTo(sortedList[0])), hasProperty(property, equalTo(sortedList[1]))));

//        instances = historicQuery().OrderByCaseInstanceCloseTime()/*.Desc()*/.ToList();
//        Assert.AreEqual(2, instances.Count);
//        Assert.That(instances, contains(hasProperty(property, equalTo(sortedList[1])), hasProperty(property, equalTo(sortedList[0]))));

//        sort by duration
//       property = "durationInMillis";
//        sortedList = oneCaseInstance.DurationInMillis, twoCaseInstance.DurationInMillis;
//        sortedList.Sort();

//        instances = historicQuery().OrderByCaseInstanceDuration()/*.Asc()*/.ToList();
//        Assert.AreEqual(2, instances.Count);
//        Assert.That(instances, contains(hasProperty(property, equalTo(sortedList[0])), hasProperty(property, equalTo(sortedList[1]))));

//        instances = historicQuery().OrderByCaseInstanceDuration()/*.Desc()*/.ToList();
//        Assert.AreEqual(2, instances.Count);
//        Assert.That(instances, contains(hasProperty(property, equalTo(sortedList[1])), hasProperty(property, equalTo(sortedList[0]))));

//    }

//    public virtual void testInvalidSorting()
//    {
//        try
//        {
//            historicQuery()/*.Asc()*/;
//            Assert.Fail("Exception expected");
//        }
//        catch (ProcessEngineException)
//        {
//            expected

//        }

//        try
//        {
//            historicQuery()/*.Desc()*/;
//            Assert.Fail("Exception expected");
//        }
//        catch (ProcessEngineException)
//        {
//            expected

//        }

//        try
//        {
//            historicQuery()//.OrderByCaseInstanceId().Count();
//            Assert.Fail("Exception expected");
//        }
//        catch (ProcessEngineException)
//        {
//            expected

//        }
//    }

//    JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(new []{"resources/api/cmmn/oneTaskCase.cmmn"}) public void testNativeQuery()

//      public virtual void testNativeQuery()
//    {
//        string id = createCaseInstance().Id;
//        createCaseInstance();
//        createCaseInstance();
//        createCaseInstance();

//        string tablePrefix = processEngineConfiguration.DatabaseTablePrefix;
//        string tableName = managementService.GetTableName(typeof(IHistoricCaseInstance));

//        Assert.AreEqual(tablePrefix + "ACT_HI_CASEINST", tableName);
//        Assert.AreEqual(tableName, managementService.GetTableName(typeof(HistoricCaseInstanceEntity)));

//        Assert.AreEqual(4, historyService.CreateNativeHistoricCaseInstanceQuery().Sql("SELECT * FROM " + tableName).Count());
//        Assert.AreEqual(4, historyService.CreateNativeHistoricCaseInstanceQuery().Sql("SELECT Count(*) FROM " + tableName).Count());

//        Assert.AreEqual(16, historyService.CreateNativeHistoricCaseInstanceQuery().Sql("SELECT Count(*) FROM " + tableName + " H1, " + tableName + " H2").Count());

//        select with distinct

//        Assert.AreEqual(4, historyService.CreateNativeHistoricCaseInstanceQuery().Sql("SELECT DISTINCT * FROM " + tableName).Count());

//        Assert.AreEqual(1, historyService.CreateNativeHistoricCaseInstanceQuery().Sql("SELECT Count(*) FROM " + tableName + " H WHERE H.ID_ = '" + id + "'").Count());
//        Assert.AreEqual(1, historyService.CreateNativeHistoricCaseInstanceQuery().Sql("SELECT * FROM " + tableName + " H WHERE H.ID_ = '" + id + "'").Count());

//        use parameters

//        Assert.AreEqual(1, historyService.CreateNativeHistoricCaseInstanceQuery().Sql("SELECT Count(*) FROM " + tableName + " H WHERE H.ID_ = #{caseInstanceId}").Parameter("caseInstanceId", id).Count());
//    }

//    JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(new []{"resources/api/cmmn/oneTaskCase.cmmn"}) public void testNativeQueryPaging()

//      public virtual void testNativeQueryPaging()
//    {
//        createCaseInstance();
//        createCaseInstance();
//        createCaseInstance();
//        createCaseInstance();

//        string tableName = managementService.GetTableName(typeof(IHistoricCaseInstance));
//        Assert.AreEqual(3, historyService.CreateNativeHistoricCaseInstanceQuery().Sql("SELECT * FROM " + tableName).ListPage(0, 3).Count);
//        Assert.AreEqual(2, historyService.CreateNativeHistoricCaseInstanceQuery().Sql("SELECT * FROM " + tableName).ListPage(2, 2).Count);
//    }

//    JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: Deployment(new string[]{ "resources/api/cmmn/emptyStageWithManualActivationCase.cmmn"}) public void testDeleteHistoricCaseInstance()

//      public virtual void testDeleteHistoricCaseInstance()
//    {
//        ICaseInstance caseInstance = createCaseInstance();

//        IHistoricCaseInstance historicInstance = queryHistoricCaseInstance(caseInstance.Id);
//        Assert.NotNull(historicInstance);

//        try
//        {
//            should not be able to Delete historic case instance cause the case instance is still running

//          historyService.DeleteHistoricCaseInstance(historicInstance.Id);
//            Assert.Fail("Exception expected");
//        }
//        catch (NullValueException)
//        {
//            expected

//        }

//        terminate(caseInstance.Id);
//        close(caseInstance.Id);

//        historyService.DeleteHistoricCaseInstance(historicInstance.Id);

//        AssertCount(0, historicQuery());
//    }

//    JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(new []{"resources/api/runtime/superProcessWithCaseCallActivity.bpmn20.xml", "resources/api/cmmn/oneTaskCase.cmmn" }) public void testQueryBySuperProcessInstanceId()

//      public virtual void testQueryBySuperProcessInstanceId()
//    {
//        string superProcessInstanceId = runtimeService.StartProcessInstanceByKey("subProcessQueryTest").Id;

//        HistoricCaseInstanceQuery query = historyService.CreateHistoricCaseInstanceQuery().SuperProcessInstanceId(superProcessInstanceId);

//        Assert.AreEqual(1, query.Count());
//        Assert.AreEqual(1, query.Count());

//        IHistoricCaseInstance subCaseInstance = query.First();
//        Assert.NotNull(subCaseInstance);
//        Assert.AreEqual(superProcessInstanceId, subCaseInstance.SuperProcessInstanceId);
//        Assert.IsNull(subCaseInstance.SuperCaseInstanceId);
//    }

//    public virtual void testQueryByInvalidSuperProcessInstanceId()
//    {
//        HistoricCaseInstanceQuery query = historyService.CreateHistoricCaseInstanceQuery();

//        query.SuperProcessInstanceId("invalid");

//        Assert.AreEqual(0, query.Count());
//        Assert.AreEqual(0, query.Count());

//        query.CaseInstanceId(null);

//        Assert.AreEqual(0, query.Count());
//        Assert.AreEqual(0, query.Count());

//    }

//    JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(new []{"resources/api/cmmn/oneProcessTaskCase.cmmn", "resources/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryBySubProcessInstanceId()

//      public virtual void testQueryBySubProcessInstanceId()
//    {
//        string superCaseInstanceId = caseService.CreateCaseInstanceByKey("oneProcessTaskCase").Id;

//        string subProcessInstanceId = runtimeService.CreateProcessInstanceQuery().SuperCaseInstanceId(superCaseInstanceId).First().Id;

//        HistoricCaseInstanceQuery query = historyService.CreateHistoricCaseInstanceQuery().SubProcessInstanceId(subProcessInstanceId);

//        Assert.AreEqual(1, query.Count());
//        Assert.AreEqual(1, query.Count());

//        IHistoricCaseInstance caseInstance = query.First();
//        Assert.AreEqual(superCaseInstanceId, caseInstance.Id);
//        Assert.IsNull(caseInstance.SuperCaseInstanceId);
//        Assert.IsNull(caseInstance.SuperProcessInstanceId);
//    }

//    public virtual void testQueryByInvalidSubProcessInstanceId()
//    {
//        HistoricCaseInstanceQuery query = historyService.CreateHistoricCaseInstanceQuery();

//        query.SubProcessInstanceId("invalid");

//        Assert.AreEqual(0, query.Count());
//        Assert.AreEqual(0, query.Count());

//        query.CaseInstanceId(null);

//        Assert.AreEqual(0, query.Count());
//        Assert.AreEqual(0, query.Count());

//    }

//    JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(new []{"resources/api/cmmn/oneCaseTaskCase.cmmn", "resources/api/cmmn/oneTaskCase.cmmn" }) public void testQueryBySuperCaseInstanceId()

//      public virtual void testQueryBySuperCaseInstanceId()
//    {
//        string superCaseInstanceId = caseService.CreateCaseInstanceByKey("oneCaseTaskCase").Id;

//        HistoricCaseInstanceQuery query = historyService.CreateHistoricCaseInstanceQuery().SuperCaseInstanceId(superCaseInstanceId);

//        Assert.AreEqual(1, query.Count());
//        Assert.AreEqual(1, query.Count());

//        IHistoricCaseInstance caseInstance = query.First();
//        Assert.AreEqual(superCaseInstanceId, caseInstance.SuperCaseInstanceId);
//        Assert.IsNull(caseInstance.SuperProcessInstanceId);
//    }

//    public virtual void testQueryByInvalidSuperCaseInstanceId()
//    {
//        HistoricCaseInstanceQuery query = historyService.CreateHistoricCaseInstanceQuery();

//        query.SuperCaseInstanceId("invalid");

//        Assert.AreEqual(0, query.Count());
//        Assert.AreEqual(0, query.Count());

//        query.CaseInstanceId(null);

//        Assert.AreEqual(0, query.Count());
//        Assert.AreEqual(0, query.Count());

//    }

//    JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(new []{"resources/api/cmmn/oneCaseTaskCase.cmmn", "resources/api/cmmn/oneTaskCase.cmmn" }) public void testQueryBySubCaseInstanceId()

//      public virtual void testQueryBySubCaseInstanceId()
//    {
//        string superCaseInstanceId = caseService.CreateCaseInstanceByKey("oneCaseTaskCase").Id;

//        string subCaseInstanceId = caseService.CreateCaseInstanceQuery().SuperCaseInstanceId(superCaseInstanceId).First().Id;

//        HistoricCaseInstanceQuery query = historyService.CreateHistoricCaseInstanceQuery().SubCaseInstanceId(subCaseInstanceId);

//        Assert.AreEqual(1, query.Count());
//        Assert.AreEqual(1, query.Count());

//        IHistoricCaseInstance caseInstance = query.First();
//        Assert.AreEqual(superCaseInstanceId, caseInstance.Id);
//        Assert.IsNull(caseInstance.SuperProcessInstanceId);
//        Assert.IsNull(caseInstance.SuperCaseInstanceId);
//    }

//    public virtual void testQueryByInvalidSubCaseInstanceId()
//    {
//        HistoricCaseInstanceQuery query = historyService.CreateHistoricCaseInstanceQuery();

//        query.SubCaseInstanceId("invalid");

//        Assert.AreEqual(0, query.Count());
//        Assert.AreEqual(0, query.Count());

//        query.CaseInstanceId(null);

//        Assert.AreEqual(0, query.Count());
//        Assert.AreEqual(0, query.Count());
//    }

//    JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(new []{"resources/api/cmmn/oneTaskCase.cmmn" }) public void testQueryByCaseActivityId()

//      public virtual void testQueryByCaseActivityId()
//    {

//        given

//        createCaseInstanceByKey("oneTaskCase");

//        when
//       HistoricCaseInstanceQuery query = historyService.CreateHistoricCaseInstanceQuery().CaseActivityIdIn("PI_HumanTask_1");

//        then

//        Assert.AreEqual(1, query.Count());
//        Assert.AreEqual(1, query.Count());
//    }

//    JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(new []{"resources/api/cmmn/oneCaseTaskCase.cmmn", "resources/api/cmmn/oneTaskCase.cmmn" }) public void testQueryByCaseActivityIds()

//      public virtual void testQueryByCaseActivityIds()
//    {

//        given

//        createCaseInstanceByKey("oneCaseTaskCase");

//        when
//       HistoricCaseInstanceQuery query = historyService.CreateHistoricCaseInstanceQuery().CaseActivityIdIn("PI_HumanTask_1", "PI_CaseTask_1");

//        then

//        Assert.AreEqual(2, query.Count());
//        Assert.AreEqual(2, query.Count());
//    }

//    JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(new []{"resources/api/cmmn/twoTaskCase.cmmn" }) public void testDistinctQueryByCaseActivityIds()

//      public virtual void testDistinctQueryByCaseActivityIds()
//    {

//        given

//        createCaseInstanceByKey("twoTaskCase");

//        when
//       HistoricCaseInstanceQuery query = historyService.CreateHistoricCaseInstanceQuery().CaseActivityIdIn("PI_HumanTask_1", "PI_HumanTask_2");

//        then

//        Assert.AreEqual(1, query.Count());
//        Assert.AreEqual(1, query.Count());
//    }

//    public virtual void testQueryByNonExistingCaseActivityId()
//    {
//        HistoricCaseInstanceQuery query = historyService.CreateHistoricCaseInstanceQuery().CaseActivityIdIn("nonExisting");

//        Assert.AreEqual(0, query.Count());
//    }

//    public virtual void testFailQueryByCaseActivityIdNull()
//    {
//        try
//        {
//            historyService.CreateHistoricCaseInstanceQuery().CaseActivityIdIn((string)null);

//            Assert.Fail("expected exception");
//        }
//        catch (NullValueException)
//        {
//        }
//    }

//    JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: Deployment(new string[] "resources/api/cmmn/oneTaskCase.cmmn") public void testRetrieveCaseDefinitionKey()

//      public virtual void testRetrieveCaseDefinitionKey()
//    {

//        given

//        string id = createCaseInstance("oneTaskCase").Id;

//        when
//       IHistoricCaseInstance caseInstance = historyService.CreateHistoricCaseInstanceQuery(c=>c.CaseInstanceId ==id).First();

//        then

//        Assert.AreEqual("oneTaskCase", caseInstance.CaseDefinitionKey);

//    }

//    JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: Deployment(new string[] "resources/api/cmmn/oneTaskCase.cmmn") public void testRetrieveCaseDefinitionName()

//      public virtual void testRetrieveCaseDefinitionName()
//    {

//        given

//        string id = createCaseInstance("oneTaskCase").Id;

//        when
//       IHistoricCaseInstance caseInstance = historyService.CreateHistoricCaseInstanceQuery(c=>c.CaseInstanceId ==id).First();

//        then

//        Assert.AreEqual("One ITask Case", caseInstance.CaseDefinitionName);

//    }

//    protected internal virtual IHistoricCaseInstance queryHistoricCaseInstance(string caseInstanceId)
//    {
//        IHistoricCaseInstance historicCaseInstance = historicQuery(c=>c.CaseInstanceId ==caseInstanceId).First();
//        Assert.NotNull(historicCaseInstance);
//        return historicCaseInstance;
//    }

//    protected internal virtual HistoricCaseInstanceQuery historicQuery()
//    {
//        return historyService.CreateHistoricCaseInstanceQuery();
//    }

//    protected internal virtual void AssertCount(long Count, HistoricCaseInstanceQuery historicQuery)
//    {
//        Assert.AreEqual(Count, historicQuery.Count());
//    }

//    protected internal virtual void AssertDateSimilar(DateTime date1, DateTime date2)
//    {
//        long difference = Math.Abs(date1.Ticks - date2.Ticks);
//        Assert.True(difference < 1000);
//    }

//}

//}