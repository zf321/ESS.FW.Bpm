using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Standalone.Pvm.Activities;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.impl;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.instance;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.ExecutionListener
{
    [TestFixture]
    public class ExecutionListenerBpmnModelExecutionContextTest : PluggableProcessEngineTestCase
    {

        private const string PROCESS_ID = "process";
        private const string START_ID = "start";
        private const string SEQUENCE_FLOW_ID = "sequenceFlow";
        private const string CATCH_EVENT_ID = "catchEvent";
        private const string GATEWAY_ID = "gateway";
        private const string USER_TASK_ID = "userTask";
        private const string END_ID = "end";
        private const string MESSAGE_ID = "messageId";
        private const string MESSAGE_NAME = "messageName";

        private string deploymentId;

        public virtual void testProcessStartEvent()
        {
            deployAndStartTestProcess(PROCESS_ID, ExecutionListenerFields.EventNameStart);
            assertFlowElementIs(typeof(IStartEvent));
            sendMessage();
            completeTask();
        }

        public virtual void testStartEventEndEvent()
        {
            deployAndStartTestProcess(START_ID, ExecutionListenerFields.EventNameEnd);
            assertFlowElementIs(typeof(IStartEvent));
            sendMessage();
            completeTask();
        }

        public virtual void testSequenceFlowTakeEvent()
        {
            deployAndStartTestProcess(SEQUENCE_FLOW_ID, ExecutionListenerFields.EventNameTake);
            assertFlowElementIs(typeof(ISequenceFlow));
            sendMessage();
            completeTask();
        }

        public virtual void testIntermediateCatchEventStartEvent()
        {
            deployAndStartTestProcess(CATCH_EVENT_ID, ExecutionListenerFields.EventNameStart);
            assertFlowElementIs(typeof(INtermediateCatchEvent));
            sendMessage();
            completeTask();
        }

        public virtual void testIntermediateCatchEventEndEvent()
        {
            deployAndStartTestProcess(CATCH_EVENT_ID, ExecutionListenerFields.EventNameEnd);
            assertNotNotified();
            sendMessage();
            assertFlowElementIs(typeof(INtermediateCatchEvent));
            completeTask();
        }

        public virtual void testGatewayStartEvent()
        {
            deployAndStartTestProcess(GATEWAY_ID, ExecutionListenerFields.EventNameStart);
            assertNotNotified();
            sendMessage();
            assertFlowElementIs(typeof(IGateway));
            completeTask();
        }

        public virtual void testGatewayEndEvent()
        {
            deployAndStartTestProcess(GATEWAY_ID, ExecutionListenerFields.EventNameEnd);
            assertNotNotified();
            sendMessage();
            assertFlowElementIs(typeof(ParallelGateway));
            completeTask();
        }

        public virtual void testUserTaskStartEvent()
        {
            deployAndStartTestProcess(USER_TASK_ID, ExecutionListenerFields.EventNameStart);
            assertNotNotified();
            sendMessage();
            assertFlowElementIs(typeof(IUserTask));
            completeTask();
        }

        public virtual void testUserTaskEndEvent()
        {
            deployAndStartTestProcess(USER_TASK_ID, ExecutionListenerFields.EventNameEnd);
            assertNotNotified();
            sendMessage();
            completeTask();
            assertFlowElementIs(typeof(IUserTask));
        }

        public virtual void testEndEventStartEvent()
        {
            deployAndStartTestProcess(END_ID, ExecutionListenerFields.EventNameStart);
            assertNotNotified();
            sendMessage();
            completeTask();
            assertFlowElementIs(typeof(IEndEvent));
        }

        public virtual void testProcessEndEvent()
        {
            deployAndStartTestProcess(PROCESS_ID, ExecutionListenerFields.EventNameEnd);
            assertNotNotified();
            sendMessage();
            completeTask();
            assertFlowElementIs(typeof(IEndEvent));
        }

        private void assertNotNotified()
        {
            Assert.IsNull(ModelExecutionContextExecutionListener.ModelInstance);
            Assert.IsNull(ModelExecutionContextExecutionListener.FlowElement);
        }

        private void assertFlowElementIs(Type elementClass)
        {
            IBpmnModelInstance modelInstance = ModelExecutionContextExecutionListener.ModelInstance;
            Assert.NotNull(modelInstance);

            IModel model = modelInstance.Model;
            IEnumerable<IModelElementInstance> events = modelInstance.GetModelElementsByType(model.GetType(typeof(IEvent)));
            Assert.AreEqual(3, events.Count());
            IEnumerable<IModelElementInstance> gateways = modelInstance.GetModelElementsByType(model.GetType(typeof(IGateway)));
            Assert.AreEqual(1, gateways.Count());
            IEnumerable<IModelElementInstance> tasks = modelInstance.GetModelElementsByType(model.GetType(typeof(ITask)));
            Assert.AreEqual(1, tasks.Count());

            IFlowElement flowElement = ModelExecutionContextExecutionListener.FlowElement;
            Assert.NotNull(flowElement);
            Assert.True(elementClass.IsAssignableFrom(flowElement.GetType()));
        }

        private void sendMessage()
        {
            runtimeService.CorrelateMessage(MESSAGE_NAME);
        }

        private void completeTask()
        {
            string taskId = taskService.CreateTaskQuery().First().Id;
            taskService.Complete(taskId);
        }

        private void deployAndStartTestProcess(string elementId, string eventName)
        {
            IBpmnModelInstance modelInstance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_ID).StartEvent(START_ID).SequenceFlowId(SEQUENCE_FLOW_ID)
                    .IntermediateCatchEvent(CATCH_EVENT_ID).ParallelGateway(GATEWAY_ID).UserTask(USER_TASK_ID).EndEvent(END_ID).Done();

            addMessageEventDefinition((ICatchEvent)modelInstance.GetModelElementById/*<ICatchEvent>*/(CATCH_EVENT_ID));
            addExecutionListener((IBaseElement)modelInstance.GetModelElementById/*<IBaseElement>*/(elementId), eventName);
            deployAndStartProcess(modelInstance);
        }

        private void addMessageEventDefinition(ICatchEvent catchEvent)
        {
            IBpmnModelInstance modelInstance = (IBpmnModelInstance)catchEvent.ModelInstance;
            IMessage message = modelInstance.NewInstance<IMessage>(typeof(IMessage));
            message.Id = MESSAGE_ID;
            message.Name = MESSAGE_NAME;
            modelInstance.Definitions.AddChildElement(message);
            IMessageEventDefinition messageEventDefinition = modelInstance.NewInstance<IMessageEventDefinition>(typeof(IMessageEventDefinition));
            messageEventDefinition.Message = message;
            catchEvent.EventDefinitions.Add(messageEventDefinition);
        }

        private void addExecutionListener(IBaseElement element, string eventName)
        {
            IExtensionElements extensionElements = element.ModelInstance.NewInstance<IExtensionElements>(typeof(IExtensionElements));
            IModelElementInstance executionListener = extensionElements.AddExtensionElement(BpmnModelConstants.CamundaNs, "executionListener");
            //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
            executionListener.SetAttributeValueNs(BpmnModelConstants.CamundaNs, "class", typeof(ModelExecutionContextExecutionListener).FullName);
            executionListener.SetAttributeValueNs(BpmnModelConstants.CamundaNs, "event", eventName);
            element.ExtensionElements = extensionElements;
        }

        private void deployAndStartProcess(IBpmnModelInstance modelInstance)
        {
            deploymentId = repositoryService.CreateDeployment().AddModelInstance("process.bpmn", modelInstance).Deploy().Id;
            runtimeService.StartProcessInstanceByKey(PROCESS_ID);
        }

        public virtual void tearDown()
        {
            ModelExecutionContextExecutionListener.Clear();
            repositoryService.DeleteDeployment(deploymentId, true);
        }

    }

}