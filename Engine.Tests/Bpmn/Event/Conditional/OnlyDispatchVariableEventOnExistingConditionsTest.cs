using System.Collections.Generic;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Conditional
{

    public class OnlyDispatchVariableEventOnExistingConditionsTest
    {

        public class CheckDelayedVariablesDelegate : IJavaDelegate
        {
            public void Execute(IBaseDelegateExecution execution)
            {
                //given conditional event exist

                //when variable is set
                execution.SetVariable("v", 1);

                //then variable events should be delayed
                IList<DelayedVariableEvent> delayedEvents = ((ExecutionEntity)execution).DelayedEvents;
                Assert.AreEqual(1, delayedEvents.Count);
                Assert.AreEqual("v", delayedEvents[0].Event.VariableInstance.Name);
            }

            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: @Override public void execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
            //public void Execute(IBaseDelegateExecution execution)
            //{
            //    //given conditional event exist

            //    //when variable is set
            //    execution.SetVariable("v", 1);

            //    //then variable events should be delayed
            //    IList<DelayedVariableEvent> delayedEvents = ((ExecutionEntity)execution).DelayedEvents;
            //    Assert.AreEqual(1, delayedEvents.Count);
            //    Assert.AreEqual("v", delayedEvents[0].Event.VariableInstance.Name);
            //}
        }

        public class CheckNoDelayedVariablesDelegate : IJavaDelegate
        {
            public void Execute(IBaseDelegateExecution execution)
            {
                //given no conditional event exist

                //when variable is set
                execution.SetVariable("v", 1);

                //then no variable events should be delayed
                IList<DelayedVariableEvent> delayedEvents = ((ExecutionEntity)execution).DelayedEvents;
                Assert.AreEqual(0, delayedEvents.Count);
            }

            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: @Override public void execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
            //public void Execute(IBaseDelegateExecution execution)
            //{
            //    //given no conditional event exist

            //    //when variable is set
            //    execution.SetVariable("v", 1);

            //    //then no variable events should be delayed
            //    IList<DelayedVariableEvent> delayedEvents = ((ExecutionEntity)execution).DelayedEvents;
            //    Assert.AreEqual(0, delayedEvents.Count);
            //}
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public ProcessEngineRule rule = new util.ProvidedProcessEngineRule();
        public ProcessEngineRule rule = new ProvidedProcessEngineRule();

        // Todo: AbstractConditionalEventDefinitionBuilder.ConditionalEventDefinitionDone()
        //[Test]
        //public virtual void testProcessWithIntermediateConditionalEvent()
        //{
        //    //given
        //    //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
        //    IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(AbstractConditionalEventTestCase.ConditionalEventProcessKey)
        //        .StartEvent().ServiceTask().CamundaClass(typeof(CheckDelayedVariablesDelegate).FullName).IntermediateCatchEvent().ConditionalEventDefinition()
        //        .Condition("${var==1}")
        //        .ConditionalEventDefinitionDone().EndEvent().Done();

        //    //when process is deployed and instance created
        //    rule.ManageDeployment(rule.RepositoryService.CreateDeployment().AddModelInstance(AbstractConditionalEventTestCase.CONDITIONAL_MODEL, modelInstance).Deploy());
        //    ProcessInstanceWithVariablesImpl processInstance = (ProcessInstanceWithVariablesImpl)rule.RuntimeService
        //        .StartProcessInstanceByKey(AbstractConditionalEventTestCase.ConditionalEventProcessKey);

        //    //then process definition contains property which indicates that conditional events exists
        //    object property = processInstance.ExecutionEntity.ProcessDefinition.GetProperty(BpmnParse.PropertynameHasConditionalEvents);
        //    Assert.NotNull(property);
        //    Assert.AreEqual(true, property);
        //}

        // Todo: ModifiableBpmnModelInstance.Modify()
        //[Test]
        //public virtual void testProcessWithBoundaryConditionalEvent()
        //{
        //    //given
        //    //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
        //    IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(AbstractConditionalEventTestCase.ConditionalEventProcessKey)
        //        .StartEvent().ServiceTask().CamundaClass(typeof(CheckDelayedVariablesDelegate).FullName).UserTask(AbstractConditionalEventTestCase.TaskWithConditionId)
        //        .EndEvent().Done();

        //    modelInstance = ModifiableBpmnModelInstance.Modify(modelInstance).UserTaskBuilder(AbstractConditionalEventTestCase.TaskWithConditionId)
        //        .BoundaryEvent().ConditionalEventDefinition().Condition("${var==1}").ConditionalEventDefinitionDone().EndEvent().Done();

        //    //when process is deployed and instance created
        //    rule.ManageDeployment(rule.RepositoryService.CreateDeployment().AddModelInstance(AbstractConditionalEventTestCase.CONDITIONAL_MODEL, modelInstance).Deploy());
        //    ProcessInstanceWithVariablesImpl processInstance = (ProcessInstanceWithVariablesImpl)rule.RuntimeService.StartProcessInstanceByKey(AbstractConditionalEventTestCase.ConditionalEventProcessKey);

        //    //then process definition contains property which indicates that conditional events exists
        //    object property = processInstance.ExecutionEntity.ProcessDefinition.GetProperty(BpmnParse.PropertynameHasConditionalEvents);
        //    Assert.NotNull(property);
        //    Assert.AreEqual(true, property);
        //}

        // Todo: ModifiableBpmnModelInstance.Modify()
        //[Test]
        //public virtual void testProcessWithEventSubProcessConditionalEvent()
        //{
        //    //given
        //    //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
        //    IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(AbstractConditionalEventTestCase.ConditionalEventProcessKey)
        //        .StartEvent().ServiceTask().CamundaClass(typeof(CheckDelayedVariablesDelegate).FullName).UserTask().EndEvent().Done();

        //    modelInstance = ModifiableBpmnModelInstance.Modify(modelInstance).AddSubProcessTo(AbstractConditionalEventTestCase.ConditionalEventProcessKey)
        //        .TriggerByEvent()//.EmbeddedSubProcess().StartEvent().ConditionalEventDefinition().Condition("${var==1}")
        //        .ConditionalEventDefinitionDone().EndEvent().Done();

        //    //when process is deployed and instance created
        //    rule.ManageDeployment(rule.RepositoryService.CreateDeployment().AddModelInstance(AbstractConditionalEventTestCase.CONDITIONAL_MODEL, modelInstance).Deploy());
        //    ProcessInstanceWithVariablesImpl processInstance = (ProcessInstanceWithVariablesImpl)rule.RuntimeService.StartProcessInstanceByKey(AbstractConditionalEventTestCase.ConditionalEventProcessKey);

        //    //then process definition contains property which indicates that conditional events exists
        //    object property = processInstance.ExecutionEntity.ProcessDefinition.GetProperty(BpmnParse.PropertynameHasConditionalEvents);
        //    Assert.NotNull(property);
        //    Assert.AreEqual(true, property);
        //}


        [Test]
        public virtual void testProcessWithoutConditionalEvent()
        {
            //given
            //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            IBpmnModelInstance modelInstance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(AbstractConditionalEventTestCase.ConditionalEventProcessKey)
                .StartEvent().ServiceTask().CamundaClass(typeof(CheckNoDelayedVariablesDelegate).FullName).UserTask().EndEvent().Done();

            //when process is deployed and instance created
            rule.ManageDeployment(rule.RepositoryService.CreateDeployment().AddModelInstance(AbstractConditionalEventTestCase.ConditionalModel, modelInstance).Deploy());
            ProcessInstanceWithVariablesImpl processInstance = (ProcessInstanceWithVariablesImpl)rule.RuntimeService.StartProcessInstanceByKey(AbstractConditionalEventTestCase.ConditionalEventProcessKey);

            //then process definition contains no property which indicates that conditional events exists
            object property = (processInstance.ExecutionEntity as ExecutionEntity).ProcessDefinition.GetProperty(BpmnParse.PropertynameHasConditionalEvents);
            Assert.IsNull(property);
        }
    }

}