using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.task;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Entity
{
    [TestFixture]
    public class EntitySerializationTest //: TestCase
    {
        private byte[] WriteObject(object @object)
        {
            using (MemoryStream rems = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(rems, @object);
                return rems.GetBuffer();
            }
        }

        private object ReadObject(byte[] data)
        {
            using (MemoryStream rems = new MemoryStream(data))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                data = null;
                return formatter.Deserialize(rems);
            }
        }

        [Test]
        public virtual void TestExecutionEntitySerialization()
        {
            var execution = new ExecutionEntity();

            var activityImpl = new ActivityImpl("test", null);
            activityImpl.ExecutionListeners.Add("start", new List<IDelegateListener<IBaseDelegateExecution>> { new TestExecutionListener() });
            execution.Activity = activityImpl;

            var processDefinitionImpl = new ProcessDefinitionImpl("test");
            processDefinitionImpl.ExecutionListeners.Add("start",
                new List<IDelegateListener<IBaseDelegateExecution>> { new TestExecutionListener() });
            execution.ProcessDefinition = processDefinitionImpl;

            var transitionImpl = new TransitionImpl("test", new ProcessDefinitionImpl("test"));
            transitionImpl.AddExecutionListener(new TestExecutionListener());
            execution.Transition = (transitionImpl);

            execution.ProcessInstanceStartContext.Initial = activityImpl;
            execution.SuperExecution=(new ExecutionEntity());

            execution.IsActive = true;
            execution.Canceled = false;
            execution.BusinessKey = "myBusinessKey";
            execution.DeleteReason = "no reason";
            execution.ActivityInstanceId = "123";
            execution.IsScope = false;

            var data = WriteObject(execution);
            execution = (ExecutionEntity)ReadObject(data);

            Assert.AreEqual("myBusinessKey", execution.BusinessKey);
            Assert.AreEqual("no reason", execution.DeleteReason);
            Assert.AreEqual("123", execution.ActivityInstanceId);
        }

        [Test]
        public virtual void TestTaskEntitySerialization()
        {
            var task = new TaskEntity();
            task.DelegationState = DelegationState.Resolved;
            task.SetExecution(new ExecutionEntity());
            task.SetProcessInstance(new ExecutionEntity());
            task.TaskDefinition = new TaskDefinition(null);

            task.Assignee = "kermit";
            task.CreateTime = DateTime.Now;
            task.Description = "Test description";
            task.DueDate = DateTime.Now;
            task.Name = "myTask";
            task.EventName = "end";
            task.Deleted = false;
            task.DelegationStateString = DelegationState.Resolved.ToString();

            var data = WriteObject(task);
            task = (TaskEntity)ReadObject(data);

            Assert.AreEqual("kermit", task.Assignee);
            Assert.AreEqual("myTask", task.Name);
            Assert.AreEqual("end", task.EventName);
        }
    }
}