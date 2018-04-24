using System.Linq;
using ESS.FW.Bpm.Engine.Impl.Incident;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Entity
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class ExecutionCachedEntityStateTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment]
        public virtual void TestExecutionTasksScope()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            var processInstance = (ExecutionEntity) runtimeService.CreateProcessInstanceQuery().First();
            Assert.AreEqual(0, processInstance.CachedEntityStateRaw);

            var execution =
                (ExecutionEntity) runtimeService.CreateExecutionQuery(c=>c.ActivityId == "userTask").First();
            Assert.AreEqual(BitMaskUtil.GetMaskForBit(ExecutionEntity.TasksStateBit), execution.CachedEntityStateRaw);
        }

        [Test]
        [Deployment]
        public virtual void TestExecutionTasksParallel()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            var processInstance = (ExecutionEntity) runtimeService.CreateProcessInstanceQuery().First();
            Assert.AreEqual(0, processInstance.CachedEntityStateRaw);

            var execution =
                (ExecutionEntity) runtimeService.CreateExecutionQuery(c=>c.ActivityId == "userTask").First();
            Assert.AreEqual(BitMaskUtil.GetMaskForBit(ExecutionEntity.TasksStateBit), execution.CachedEntityStateRaw);
        }

        [Test]
        [Deployment]
        public virtual void TestExecutionTasksMi()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            var processInstance = (ExecutionEntity) runtimeService.CreateProcessInstanceQuery().First();
            Assert.AreEqual(0, processInstance.CachedEntityStateRaw);

            var executions = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "userTask").ToList();
            foreach (var execution in executions)
            {
                var cachedEntityStateRaw = ((ExecutionEntity) execution).CachedEntityStateRaw;
                if (!((ExecutionEntity) execution).IsScope)
                    Assert.AreEqual(
                        BitMaskUtil.GetMaskForBit(ExecutionEntity.TasksStateBit) |
                        BitMaskUtil.GetMaskForBit(ExecutionEntity.VariablesStateBit), cachedEntityStateRaw);
                else
                    Assert.AreEqual(BitMaskUtil.GetMaskForBit(ExecutionEntity.VariablesStateBit), cachedEntityStateRaw);
            }
        }

        [Test]
        [Deployment]
        public virtual void TestProcessInstanceEventSubscriptions()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            var processInstance = (ExecutionEntity) runtimeService.CreateProcessInstanceQuery().First();
            Assert.AreEqual(BitMaskUtil.GetMaskForBit(ExecutionEntity.EventSubscriptionsStateBit),
                processInstance.CachedEntityStateRaw);
        }

        [Test]
        [Deployment]
        public virtual void TestExecutionEventSubscriptionsScope()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            var processInstance = (ExecutionEntity) runtimeService.CreateProcessInstanceQuery().First();
            Assert.AreEqual(0, processInstance.CachedEntityStateRaw);

            var execution =
                (ExecutionEntity)
                runtimeService.CreateExecutionQuery(c=>c.ActivityId == "IntermediateCatchEvent_1").First();
            Assert.AreEqual(BitMaskUtil.GetMaskForBit(ExecutionEntity.EventSubscriptionsStateBit),
                execution.CachedEntityStateRaw);
        }

        [Test]
        [Deployment]
        public virtual void TestExecutionEventSubscriptionsMi()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            var processInstance = (ExecutionEntity) runtimeService.CreateProcessInstanceQuery().First();
            Assert.AreEqual(0, processInstance.CachedEntityStateRaw);

            var executions = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "ReceiveTask_1").ToList();
            foreach (var execution in executions)
            {
                var cachedEntityStateRaw = ((ExecutionEntity) execution).CachedEntityStateRaw;

                if (!((ExecutionEntity) execution).IsScope)
                    Assert.AreEqual(BitMaskUtil.GetMaskForBit(ExecutionEntity.VariablesStateBit), cachedEntityStateRaw);
                else
                    Assert.AreEqual(BitMaskUtil.GetMaskForBit(ExecutionEntity.EventSubscriptionsStateBit),
                        cachedEntityStateRaw);
            }
        }

        [Test]
        [Deployment]
        public virtual void TestProcessInstanceJobs()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            var processInstance = (ExecutionEntity) runtimeService.CreateProcessInstanceQuery().First();
            Assert.AreEqual(BitMaskUtil.GetMaskForBit(ExecutionEntity.JobsStateBit),
                processInstance.CachedEntityStateRaw);
        }

        [Test]
        [Deployment]
        public virtual void TestExecutionJobsScope()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            var processInstance = (ExecutionEntity) runtimeService.CreateProcessInstanceQuery().First();
            Assert.AreEqual(0, processInstance.CachedEntityStateRaw);

            var execution =
                (ExecutionEntity) runtimeService.CreateExecutionQuery(c=>c.ActivityId == "userTask").First();
            Assert.AreEqual(BitMaskUtil.GetMaskForBit(ExecutionEntity.JobsStateBit), execution.CachedEntityStateRaw);
        }

        [Test]
        [Deployment]
        public virtual void TestExecutionJobsParallel()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            var processInstance = (ExecutionEntity) runtimeService.CreateProcessInstanceQuery().First();
            Assert.AreEqual(0, processInstance.CachedEntityStateRaw);

            var execution =
                (ExecutionEntity) runtimeService.CreateExecutionQuery(c=>c.ActivityId == "userTask").First();
            Assert.AreEqual(BitMaskUtil.GetMaskForBit(ExecutionEntity.JobsStateBit), execution.CachedEntityStateRaw);
        }

        [Test]
        [Deployment]
        public virtual void TestProcessInstanceIncident()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            var processInstance = (ExecutionEntity) runtimeService.CreateProcessInstanceQuery().First();
            Assert.AreEqual(0, processInstance.CachedEntityStateRaw);
            
            var execution = (ExecutionEntity) runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task").First();
            Assert.AreEqual(0, execution.CachedEntityStateRaw);

            processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this, execution));

            var execution2 = (ExecutionEntity) runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task").First();
            Assert.AreEqual(BitMaskUtil.GetMaskForBit(ExecutionEntity.IncidentStateBit), execution2.CachedEntityStateRaw);
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly ExecutionCachedEntityStateTest _outerInstance;

            private readonly ExecutionEntity _execution;

            public CommandAnonymousInnerClass(ExecutionCachedEntityStateTest outerInstance, ExecutionEntity execution)
            {
                _outerInstance = outerInstance;
                _execution = execution;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                var incidentContext = new IncidentContext();
                incidentContext.ExecutionId = _execution.Id;

                IncidentEntity.CreateAndInsertIncident("foo", incidentContext, null);

                return null;
            }
        }

        [Test]
        [Deployment]
        public virtual void TestExecutionIncidentParallel()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            var processInstance = (ExecutionEntity) runtimeService.CreateProcessInstanceQuery().First();
            Assert.AreEqual(0, processInstance.CachedEntityStateRaw);

            var execution = (ExecutionEntity) runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task").First();
            Assert.AreEqual(0, execution.CachedEntityStateRaw);

            processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass2(this, execution));

            var execution2 = (ExecutionEntity) runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task").First();
            Assert.AreEqual(BitMaskUtil.GetMaskForBit(ExecutionEntity.IncidentStateBit), execution2.CachedEntityStateRaw);
        }

        private class CommandAnonymousInnerClass2 : ICommand<object>
        {
            private readonly ExecutionCachedEntityStateTest _outerInstance;

            private readonly ExecutionEntity _execution;

            public CommandAnonymousInnerClass2(ExecutionCachedEntityStateTest outerInstance, ExecutionEntity execution)
            {
                _outerInstance = outerInstance;
                _execution = execution;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                var incidentContext = new IncidentContext();
                incidentContext.ExecutionId = _execution.Id;

                IncidentEntity.CreateAndInsertIncident("foo", incidentContext, null);

                return null;
            }
        }

        [Test]
        [Deployment]
        public virtual void TestExecutionExternalTask()
        {
            runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

            var execution =
                (ExecutionEntity) runtimeService.CreateExecutionQuery(c=>c.ActivityId == "externalTask").First();

            Assert.AreEqual(BitMaskUtil.GetMaskForBit(ExecutionEntity.ExternalTasksBit), execution.CachedEntityStateRaw);
        }

        [Test]
        [Deployment]
        public virtual void TestProcessInstanceTasks()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            var processInstance = (ExecutionEntity) runtimeService.CreateProcessInstanceQuery().First();
            Assert.AreEqual(BitMaskUtil.GetMaskForBit(ExecutionEntity.TasksStateBit),
                processInstance.CachedEntityStateRaw);
        }
    }
}