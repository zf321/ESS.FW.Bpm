using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Async
{
    [TestFixture]
    public class AsyncEndEventTest : PluggableProcessEngineTestCase
    {        
        [Test]
        [Deployment]
        public virtual void testAsyncEndEvent()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("asyncEndEvent");
            long Count = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessInstanceId == pi.Id && c.SuspensionState == SuspensionStateFields.Active.StateCode).Count();

            Assert.AreEqual(1, runtimeService.CreateExecutionQuery(c=>c.ActivityId == "EndEvent").Count());
            Assert.AreEqual(1, Count);

            ExecuteAvailableJobs();
            Count = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessInstanceId == pi.Id).Count();

            Assert.AreEqual(0, runtimeService.CreateExecutionQuery(c=>c.ActivityId == "endEvent"&& c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
            Assert.AreEqual(0, Count);
        }

        [Test]
        [Deployment]
        public virtual void testAsyncEndEventListeners()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("asyncEndEvent");
            long Count = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessInstanceId == pi.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode).Count();

            Assert.IsNull(runtimeService.GetVariable(pi.Id, "listener"));
            Assert.AreEqual(1, runtimeService.CreateExecutionQuery(c=>c.ActivityId == "endEvent").Count());
            Assert.AreEqual(1, Count);

            // as we are standing at the end event, we execute it.
            ExecuteAvailableJobs();

            Count = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessInstanceId == pi.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode).Count();
            Assert.AreEqual(0, Count);

            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelActivity)
            {
                IQueryable<IHistoricVariableInstance> name = historyService.CreateHistoricVariableInstanceQuery()
                    .Where(c=>c.ProcessInstanceId==pi.Id);//.VariableName("listener");
                Assert.NotNull(name);
                Assert.AreEqual("listener invoked", name.First().Value);
            }
        }

        [Test]
        [Deployment]
        public virtual void testMultipleAsyncEndEvents()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("multipleAsyncEndEvent");
            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery().Count());

            // should stop at both end events
            IList<IJob> jobs = managementService.CreateJobQuery(c=>c.Retries > 0).ToList();
            Assert.AreEqual(2, jobs.Count);

            // execute one of the end events
            managementService.ExecuteJob(jobs[0].Id);
            jobs = managementService.CreateJobQuery(c=>c.Retries > 0).ToList();
            Assert.AreEqual(1, jobs.Count);

            // execute the second one
            managementService.ExecuteJob(jobs[0].Id);
            // Assert that we have finished our instance now
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());

            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelActivity)
            {
                IQueryable<IHistoricVariableInstance> name = historyService.CreateHistoricVariableInstanceQuery()
                    .Where(c=>c.ProcessInstanceId==pi.Id);//.VariableName("message");
                Assert.NotNull(name);
                Assert.AreEqual(true, name.First().Value);

            }
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/async/AsyncEndEventTest.TestCallActivity-super.bpmn20.xml", "resources/bpmn/async/AsyncEndEventTest.TestCallActivity-sub.bpmn20.xml" })]
        public virtual void testCallActivity()
        {
            runtimeService.StartProcessInstanceByKey("super");

            IProcessInstance pi = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == "sub").First();

            Assert.True(pi is ExecutionEntity);

            Assert.AreEqual("theSubEnd", ((ExecutionEntity)pi).ActivityId);

        }

    }

}