//using System.Linq;
//using ESS.FW.Bpm.Model.Bpmn;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.Bpmn.Event.Conditional
//{
//    [TestFixture]
//    public class TriggerConditionalEventOnStartAtActivityTest : AbstractConditionalEventTestCase
//    {
//        [Test]
//        public virtual void testNonInterruptingTriggerBoundaryEvent()
//        {
//            //given
//            DeployConditionalBoundaryEventProcess(TaskModel, TaskBeforeConditionId, false);

//            //when
//            RuntimeService.CreateProcessInstanceByKey(ConditionalEventProcessKey)
//                .StartBeforeActivity(TaskBeforeConditionId)
//                .SetVariable(VariableName, "1")
//                .ExecuteWithVariablesInReturn();

//            //then
//            TasksAfterVariableIsSet = TaskService.CreateTaskQuery()
//                .ToList();
//            Assert.AreEqual(2, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(1, TaskService.CreateTaskQuery(c=>c.Name ==TaskAfterCondition)
//                .Count());
//            Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());
//        }


//        [Test]
//        public virtual void testNonInterruptingTriggerBoundaryEventFromInnerSubProcess()
//        {
//            //given
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .SubProcess(SubProcessId)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();
//            DeployConditionalBoundaryEventProcess(modelInstance, SubProcessId, false);


//            //when
//            RuntimeService.CreateProcessInstanceByKey(ConditionalEventProcessKey)
//                .StartBeforeActivity(TaskBeforeConditionId)
//                .SetVariable(VariableName, "1")
//                .ExecuteWithVariablesInReturn();

//            //then
//            TasksAfterVariableIsSet = TaskService.CreateTaskQuery()
//                .ToList();
//            Assert.AreEqual(2, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(1, TaskService.CreateTaskQuery(c=>c.Name ==TaskAfterCondition)
//                .Count());
//            Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());
//        }


//        [Test]
//        public virtual void testNonInterruptingTriggerGlobalAndInnerEventSubProcessFromInnerSubProcess()
//        {
//            //given
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .SubProcess(SubProcessId)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();
//            modelInstance = AddConditionalEventSubProcess(modelInstance, SubProcessId, TaskAfterConditionId + 1, false);
//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, false);

//            //when
//            RuntimeService.CreateProcessInstanceByKey(ConditionalEventProcessKey)
//                .StartBeforeActivity(TaskBeforeConditionId)
//                .SetVariable(VariableName, "1")
//                .ExecuteWithVariablesInReturn();

//            //then
//            TasksAfterVariableIsSet = TaskService.CreateTaskQuery()
//                .ToList();
//            Assert.AreEqual(3, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(2, TaskService.CreateTaskQuery(c=>c.Name ==TaskAfterCondition)
//                .Count());
//            Assert.AreEqual(1, TaskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == TaskAfterConditionId)
//                .Count());
//            Assert.AreEqual(1, TaskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == TaskAfterConditionId + 1)
//                .Count());
//            Assert.AreEqual(2, ConditionEventSubscriptionQuery.Count());
//        }

//        //private class ConditionalEventProcessSpecifierAnonymousInnerClass2 : ConditionalEventProcessSpecifier
//        //{
//        //    private readonly TriggerConditionalEventFromDelegationCodeTest outerInstance;

//        //    public ConditionalEventProcessSpecifierAnonymousInnerClass2(TriggerConditionalEventFromDelegationCodeTest outerInstance)
//        //    {
//        //        this.outerInstance = outerInstance;
//        //    }

//        //    public virtual Type DelegateClass
//        //    {
//        //        get
//        //        {
//        //            return typeof(SetMultipleSameVariableDelegate);
//        //        }
//        //    }

//        //    public virtual int ExpectedInterruptingCount
//        //    {
//        //        get
//        //        {
//        //            return 1;
//        //        }
//        //    }

//        //    public virtual int ExpectedNonInterruptingCount
//        //    {
//        //        get
//        //        {
//        //            return 3;
//        //        }
//        //    }


//        //    public virtual string Condition
//        //    {
//        //        get
//        //        {
//        //            return "${variable2 == 1}";
//        //        }
//        //    }

//        //    public override string ToString()
//        //    {
//        //        return "SetMultipleVariableInDelegate";
//        //    }
//        //}


//        [Test]
//        public virtual void testNonInterruptingTriggerGlobalEventSubProcess()
//        {
//            //given
//            DeployConditionalEventSubProcess(TaskModel, ConditionalEventProcessKey, false);

//            //when
//            RuntimeService.CreateProcessInstanceByKey(ConditionalEventProcessKey)
//                .StartBeforeActivity(TaskBeforeConditionId)
//                .SetVariable(VariableName, "1")
//                .ExecuteWithVariablesInReturn();

//            //then
//            TasksAfterVariableIsSet = TaskService.CreateTaskQuery()
//                .ToList();
//            Assert.AreEqual(2, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(1, TaskService.CreateTaskQuery(c=>c.Name ==TaskAfterCondition)
//                .Count());
//            Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());
//        }


//        [Test]
//        public virtual void testNonInterruptingTriggerGlobalEventSubProcessFromInnerSubProcess()
//        {
//            //given
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .SubProcess(SubProcessId)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();
//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, false);

//            //when
//            RuntimeService.CreateProcessInstanceByKey(ConditionalEventProcessKey)
//                .StartBeforeActivity(TaskBeforeConditionId)
//                .SetVariable(VariableName, "1")
//                .ExecuteWithVariablesInReturn();

//            //then
//            TasksAfterVariableIsSet = TaskService.CreateTaskQuery()
//                .ToList();
//            Assert.AreEqual(2, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(1, TaskService.CreateTaskQuery(c=>c.Name ==TaskAfterCondition)
//                .Count());
//            Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());
//        }


//        [Test]
//        public virtual void testNonInterruptingTriggerInnerEventSubProcess()
//        {
//            //given
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .SubProcess(SubProcessId)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();
//            DeployConditionalEventSubProcess(modelInstance, SubProcessId, false);


//            //when
//            RuntimeService.CreateProcessInstanceByKey(ConditionalEventProcessKey)
//                .StartBeforeActivity(TaskBeforeConditionId)
//                .SetVariable(VariableName, "1")
//                .ExecuteWithVariablesInReturn();

//            //then
//            TasksAfterVariableIsSet = TaskService.CreateTaskQuery()
//                .ToList();
//            Assert.AreEqual(2, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(1, TaskService.CreateTaskQuery(c=>c.Name ==TaskAfterCondition)
//                .Count());
//            Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());
//        }

//        [Test]
//        public virtual void testNonInterruptingTriggerMixedProcess()
//        {
//            //given
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .SubProcess(SubProcessId)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();
//            var isInterrupting = false;
//            modelInstance = AddConditionalBoundaryEvent(modelInstance, TaskBeforeConditionId, TaskAfterConditionId + 1,
//                isInterrupting);
//            modelInstance = AddConditionalBoundaryEvent(modelInstance, SubProcessId, TaskAfterConditionId + 2,
//                isInterrupting);
//            modelInstance = AddConditionalEventSubProcess(modelInstance, SubProcessId, TaskAfterConditionId + 3,
//                isInterrupting);
//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, isInterrupting);

//            //when
//            RuntimeService.CreateProcessInstanceByKey(ConditionalEventProcessKey)
//                .StartBeforeActivity(TaskBeforeConditionId)
//                .SetVariable(VariableName, "1")
//                .ExecuteWithVariablesInReturn();

//            //then
//            TasksAfterVariableIsSet = TaskService.CreateTaskQuery()
//                .ToList();
//            Assert.AreEqual(5, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(4, TaskService.CreateTaskQuery(c=>c.Name ==TaskAfterCondition)
//                .Count());
//            Assert.AreEqual(1, TaskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == TaskAfterConditionId + 1)
//                .Count());
//            Assert.AreEqual(1, TaskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == TaskAfterConditionId + 2)
//                .Count());
//            Assert.AreEqual(1, TaskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == TaskAfterConditionId + 3)
//                .Count());
//            Assert.AreEqual(1, TaskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == TaskAfterConditionId)
//                .Count());
//            Assert.AreEqual(4, ConditionEventSubscriptionQuery.Count());
//        }


//        [Test]
//        public virtual void testNonInterruptingTriggerUserAndSubProcessBoundaryEventFromInnerSubProcess()
//        {
//            //given
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .SubProcess(SubProcessId)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();
//            modelInstance = AddConditionalBoundaryEvent(modelInstance, TaskBeforeConditionId, TaskAfterConditionId + 1,
//                false);
//            DeployConditionalBoundaryEventProcess(modelInstance, SubProcessId, false);


//            //when
//            RuntimeService.CreateProcessInstanceByKey(ConditionalEventProcessKey)
//                .StartBeforeActivity(TaskBeforeConditionId)
//                .SetVariable(VariableName, "1")
//                .ExecuteWithVariablesInReturn();

//            //then
//            TasksAfterVariableIsSet = TaskService.CreateTaskQuery()
//                .ToList();
//            Assert.AreEqual(3, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(2, TaskService.CreateTaskQuery(c=>c.Name ==TaskAfterCondition)
//                .Count());
//            Assert.AreEqual(1, TaskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == TaskAfterConditionId + 1)
//                .Count());
//            Assert.AreEqual(1, TaskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == TaskAfterConditionId)
//                .Count());
//            Assert.AreEqual(2, ConditionEventSubscriptionQuery.Count());
//        }


//        [Test]
//        public virtual void testTriggerBoundaryEvent()
//        {
//            //given
//            DeployConditionalBoundaryEventProcess(TaskModel, TaskBeforeConditionId, true);

//            //when
//            RuntimeService.CreateProcessInstanceByKey(ConditionalEventProcessKey)
//                .StartBeforeActivity(TaskBeforeConditionId)
//                .SetVariable(VariableName, "1")
//                .ExecuteWithVariablesInReturn();

//            //then
//            TasksAfterVariableIsSet = TaskService.CreateTaskQuery()
//                .ToList();
//            Assert.AreEqual(1, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(TaskAfterConditionId, TasksAfterVariableIsSet[0].TaskDefinitionKey);
//        }


//        [Test]
//        public virtual void testTriggerBoundaryEventFromInnerSubProcess()
//        {
//            //given
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .SubProcess(SubProcessId)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();

//            DeployConditionalBoundaryEventProcess(modelInstance, SubProcessId, true);


//            //when
//            RuntimeService.CreateProcessInstanceByKey(ConditionalEventProcessKey)
//                .StartBeforeActivity(TaskBeforeConditionId)
//                .SetVariable(VariableName, "1")
//                .ExecuteWithVariablesInReturn();

//            //then
//            TasksAfterVariableIsSet = TaskService.CreateTaskQuery()
//                .ToList();
//            Assert.AreEqual(1, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(TaskAfterConditionId, TasksAfterVariableIsSet[0].TaskDefinitionKey);
//        }

//        [Test]
//        public virtual void testTriggerGlobalAndInnerEventSubProcessFromInnerSubProcess()
//        {
//            //given
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .SubProcess(SubProcessId)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();

//            modelInstance = AddConditionalEventSubProcess(modelInstance, SubProcessId, TaskAfterConditionId + 1, true);
//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, true);

//            //when
//            RuntimeService.CreateProcessInstanceByKey(ConditionalEventProcessKey)
//                .StartBeforeActivity(TaskBeforeConditionId)
//                .SetVariable(VariableName, "1")
//                .ExecuteWithVariablesInReturn();

//            //then
//            TasksAfterVariableIsSet = TaskService.CreateTaskQuery()
//                .ToList();
//            Assert.AreEqual(1, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(TaskAfterConditionId, TasksAfterVariableIsSet[0].TaskDefinitionKey);
//        }

//        [Test]
//        public virtual void testTriggerGlobalEventSubProcess()
//        {
//            //given
//            DeployConditionalEventSubProcess(TaskModel, ConditionalEventProcessKey, true);

//            //when
//            RuntimeService.CreateProcessInstanceByKey(ConditionalEventProcessKey)
//                .StartBeforeActivity(TaskBeforeConditionId)
//                .SetVariable(VariableName, "1")
//                .ExecuteWithVariablesInReturn();

//            //then
//            TasksAfterVariableIsSet = TaskService.CreateTaskQuery()
//                .ToList();
//            Assert.AreEqual(1, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(TaskAfterConditionId, TasksAfterVariableIsSet[0].TaskDefinitionKey);
//        }


//        [Test]
//        public virtual void testTriggerGlobalEventSubProcessFromInnerSubProcess()
//        {
//            //given
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .SubProcess(SubProcessId)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();
//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, true);

//            //when
//            RuntimeService.CreateProcessInstanceByKey(ConditionalEventProcessKey)
//                .StartBeforeActivity(TaskBeforeConditionId)
//                .SetVariable(VariableName, "1")
//                .ExecuteWithVariablesInReturn();

//            //then
//            TasksAfterVariableIsSet = TaskService.CreateTaskQuery()
//                .ToList();
//            Assert.AreEqual(1, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(TaskAfterConditionId, TasksAfterVariableIsSet[0].TaskDefinitionKey);
//        }


//        [Test]
//        public virtual void testTriggerInnerEventSubProcess()
//        {
//            //given
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .SubProcess(SubProcessId)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();
//            DeployConditionalEventSubProcess(modelInstance, SubProcessId, true);


//            //when
//            RuntimeService.CreateProcessInstanceByKey(ConditionalEventProcessKey)
//                .StartBeforeActivity(TaskBeforeConditionId)
//                .SetVariable(VariableName, "1")
//                .ExecuteWithVariablesInReturn();

//            //then
//            TasksAfterVariableIsSet = TaskService.CreateTaskQuery()
//                .ToList();
//            Assert.AreEqual(1, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(TaskAfterConditionId, TasksAfterVariableIsSet[0].TaskDefinitionKey);
//        }

//        [Test]
//        public virtual void testTriggerMixedProcess()
//        {
//            //given
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .SubProcess(SubProcessId)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();
//            var isInterrupting = true;
//            modelInstance = AddConditionalBoundaryEvent(modelInstance, TaskBeforeConditionId, TaskAfterConditionId + 1,
//                isInterrupting);
//            modelInstance = AddConditionalBoundaryEvent(modelInstance, SubProcessId, TaskAfterConditionId + 2,
//                isInterrupting);
//            modelInstance = AddConditionalEventSubProcess(modelInstance, SubProcessId, TaskAfterConditionId + 3,
//                isInterrupting);
//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, isInterrupting);

//            //when
//            RuntimeService.CreateProcessInstanceByKey(ConditionalEventProcessKey)
//                .StartBeforeActivity(TaskBeforeConditionId)
//                .SetVariable(VariableName, "1")
//                .ExecuteWithVariablesInReturn();

//            //then
//            TasksAfterVariableIsSet = TaskService.CreateTaskQuery()
//                .ToList();
//            Assert.AreEqual(1, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(TaskAfterConditionId, TasksAfterVariableIsSet[0].TaskDefinitionKey);
//        }


//        [Test]
//        public virtual void testTriggerUserAndSubProcessBoundaryEventFromInnerSubProcess()
//        {
//            //given
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .SubProcess(SubProcessId)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();
//            modelInstance = AddConditionalBoundaryEvent(modelInstance, TaskBeforeConditionId, TaskAfterConditionId + 1,
//                true);
//            DeployConditionalBoundaryEventProcess(modelInstance, SubProcessId, true);


//            //when
//            RuntimeService.CreateProcessInstanceByKey(ConditionalEventProcessKey)
//                .StartBeforeActivity(TaskBeforeConditionId)
//                .SetVariable(VariableName, "1")
//                .ExecuteWithVariablesInReturn();

//            //then
//            TasksAfterVariableIsSet = TaskService.CreateTaskQuery()
//                .ToList();
//            Assert.AreEqual(1, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(TaskAfterConditionId, TasksAfterVariableIsSet[0].TaskDefinitionKey);
//        }

//        [Test]
//        public virtual void testTwoInstructions()
//        {
//            //given

//            var start = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//             .StartEvent("start")
//             .SubProcess(SubProcessId)
//             .EmbeddedSubProcess()
//             .StartEvent();
//               start .UserTask(TaskBeforeConditionId);
//            start.Name(TaskBeforeCondition);
//            var start2= start.EndEvent()
//            .SubProcessDone()
//            .EndEvent()
//            .MoveToNode("start")
//            .SubProcess(SubProcessId + 1)
//            .EmbeddedSubProcess()
//            .StartEvent();
//            start2.UserTask(TaskBeforeConditionId + 1);
//            start2.Name(TaskBeforeCondition + 1);
//            IBpmnModelInstance modelInstance = start2.EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();

//            var isInterrupting = true;
//            modelInstance = AddConditionalBoundaryEvent(modelInstance, SubProcessId, TaskAfterConditionId,
//                isInterrupting);
//            modelInstance = AddConditionalBoundaryEvent(modelInstance, SubProcessId + 1, TaskAfterConditionId + 1,
//                isInterrupting);
//            Engine.ManageDeployment(RepositoryService.CreateDeployment()
//                .AddModelInstance(AbstractConditionalEventTestCase.ConditionalModel, modelInstance)
//                .Deploy());

//            //when
//            RuntimeService.CreateProcessInstanceByKey(ConditionalEventProcessKey)
//                .StartBeforeActivity(TaskBeforeConditionId)
//                .SetVariable(VariableName, "1")
//                .StartBeforeActivity(TaskBeforeConditionId + 1)
//                .ExecuteWithVariablesInReturn();

//            //then
//            TasksAfterVariableIsSet = TaskService.CreateTaskQuery()
//                .ToList();
//            AssertTaskNames(TasksAfterVariableIsSet, TaskAfterConditionId, TaskAfterConditionId + 1);
//        }
//    }
//}