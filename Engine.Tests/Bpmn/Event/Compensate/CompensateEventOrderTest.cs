using System.Collections.Generic;
using Engine.Tests.Bpmn.Event.Compensate.Helper;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.instance;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Compensate
{
    [TestFixture]
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryActivity)]
    public class CompensateEventOrderTest
    {
        private bool InstanceFieldsInitialized = false;

        public CompensateEventOrderTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            testHelper = new ProcessEngineTestRule(engineRule);
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public ProcessEngineRule engineRule = new ProcessEngineRule();
        public ProcessEngineRule engineRule = new ProcessEngineRule();
        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public util.ProcessEngineTestRule testHelper = new util.ProcessEngineTestRule(engineRule);
        public ProcessEngineTestRule testHelper;

        // Todo: compensateEventDefinitionDone()
        //[Test]
        //public virtual void testTwoCompensateEventsInReverseOrder()
        //{
        //    //given
        //    //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
        //    IBpmnModelInstance model = Model.Bpmn.Bpmn.CreateExecutableProcess("Process_1")
        //        .StartEvent()
        //        .ServiceTask("serviceTask1")
        //        .CamundaClass(typeof(IncreaseCurrentTimeServiceTask).FullName)
        //        .BoundaryEvent("compensationBoundary1")
        //        .CompensateEventDefinition()
        //        .CompensateEventDefinitionDone()
        //        .MoveToActivity("serviceTask1")
        //        .ServiceTask("serviceTask2")
        //        .CamundaClass(typeof(IncreaseCurrentTimeServiceTask).FullName)
        //        .BoundaryEvent("compensationBoundary2")
        //        .CompensateEventDefinition()
        //        .CompensateEventDefinitionDone()
        //        .MoveToActivity("serviceTask2")
        //        .IntermediateThrowEvent("compensationEvent")
        //        .CompensateEventDefinition()
        //        .WaitForCompletion(true)
        //        .CompensateEventDefinitionDone()
        //        .EndEvent()
        //        .Done();

        //    addServiceTaskCompensationHandler(model, "compensationBoundary1", "A");
        //    addServiceTaskCompensationHandler(model, "compensationBoundary2", "B");


        //    testHelper.Deploy(model);

        //    //when
        //    engineRule.RuntimeService.StartProcessInstanceByKey("Process_1", Variable.Variables.CreateVariables().PutValue("currentTime", DateTime.Now));

        //    //then compensation activities are executed in the reverse order
        //    IList<IHistoricActivityInstance> list = engineRule.HistoryService.CreateHistoricActivityInstanceQuery()/*.OrderByHistoricActivityInstanceEndTime()*//*.Asc()*/.ToList();

        //    long indexA = searchForActivityIndex(list, "A");
        //    long indexB = searchForActivityIndex(list, "B");

        //    //AssertNotEquals(-1, indexA);
        //    //AssertNotEquals(-1, indexB);
        //    Assert.AreNotEqual(-1, indexA);
        //    Assert.AreNotEqual(-1, indexB);

        //    //Assert.True("Compensation activities were executed in wrong order.", indexA > indexB);
        //    Assert.True(indexA>indexB, "Compensation activities were executed in wrong order.");

        //}

        private long searchForActivityIndex(IList<IHistoricActivityInstance> historicActivityInstances, string activityId)
        {
            for (int i = 0; i < historicActivityInstances.Count; i++)
            {
                IHistoricActivityInstance historicActivityInstance = historicActivityInstances[i];
                if (historicActivityInstance.ActivityId.Equals(activityId))
                {
                    return i;
                }
            }
            return -1;
        }

        private void addServiceTaskCompensationHandler(IBpmnModelInstance modelInstance, string boundaryEventId, string compensationHandlerId)
        {

            IBoundaryEvent boundaryEvent = (IBoundaryEvent)modelInstance.GetModelElementById/*<IBoundaryEvent>*/(boundaryEventId);
            IBaseElement scope = (IBaseElement)boundaryEvent.ParentElement;

            IServiceTask compensationHandler = modelInstance.NewInstance<IServiceTask>(typeof(IServiceTask));
            compensationHandler.Id = compensationHandlerId;
            compensationHandler.ForCompensation = true;
            //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            compensationHandler.CamundaClass = typeof(IncreaseCurrentTimeServiceTask).FullName;
            scope.AddChildElement(compensationHandler);

            IAssociation association = modelInstance.NewInstance<IAssociation>(typeof(IAssociation));
            association.AssociationDirection = AssociationDirection.One;
            association.Source = boundaryEvent;
            association.Target = compensationHandler;
            scope.AddChildElement(association);

        }


    }

}