using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.instance;

namespace Engine.Tests.Api.Runtime.Migration.Models
{
    /// <summary>
    /// </summary>
    public class CompensationModels
    {
        public static readonly IBpmnModelInstance ONE_COMPENSATION_TASK_MODEL = ProcessModels.NewModel()
            .StartEvent()
            .UserTask("userTask1")
            .BoundaryEvent("compensationBoundary")
            //.CompensateEventDefinition()
            //.CompensateEventDefinitionDone()
            //.MoveToActivity("userTask1")
            .UserTask("userTask2")
            .IntermediateThrowEvent("compensationEvent")
            //.CompensateEventDefinition()
            //.WaitForCompletion(true)
            //.CompensateEventDefinitionDone()
            .EndEvent()
            .Done();

        public static readonly IBpmnModelInstance COMPENSATION_ONE_TASK_SUBPROCESS_MODEL = ProcessModels.NewModel()
            .StartEvent()
            .SubProcess("subProcess")
            //.EmbeddedSubProcess()
            //.StartEvent()
            .UserTask("userTask1")
            .BoundaryEvent("compensationBoundary")
            //.CompensateEventDefinition()
            //.CompensateEventDefinitionDone()
            //.MoveToActivity("userTask1")
            .EndEvent()
            .SubProcessDone()
            .UserTask("userTask2")
            .IntermediateThrowEvent("compensationEvent")
            //.CompensateEventDefinition()
            //.WaitForCompletion(true)
            //.CompensateEventDefinitionDone()
            .EndEvent()
            .Done();

        public static readonly IBpmnModelInstance COMPENSATION_TWO_TASKS_SUBPROCESS_MODEL = ProcessModels.NewModel()
            .StartEvent()
            .SubProcess("subProcess")
            //.EmbeddedSubProcess()
            //.StartEvent()
            .UserTask("userTask1")
            .BoundaryEvent("compensationBoundary")
            //.CompensateEventDefinition()
            //.CompensateEventDefinitionDone()
            //.MoveToActivity("userTask1")
            .UserTask("userTask2")
            .EndEvent("subProcessEnd")
            .SubProcessDone()
            .IntermediateThrowEvent("compensationEvent")
            //.CompensateEventDefinition()
            //.WaitForCompletion(true)
            //.CompensateEventDefinitionDone()
            .EndEvent()
            .Done();


        public static readonly IBpmnModelInstance DOUBLE_SUBPROCESS_MODEL = ProcessModels.NewModel()
                .StartEvent()
                .SubProcess("outerSubProcess")
                //.EmbeddedSubProcess()
                //.StartEvent()
                //.SubProcess("innerSubProcess")
                ////.EmbeddedSubProcess()
                //.StartEvent()
                //.UserTask("userTask1")
                //.BoundaryEvent("compensationBoundary")
                //.CompensateEventDefinition()
                //.CompensateEventDefinitionDone()
                //.MoveToActivity("userTask1")
                //.EndEvent()
                //.SubProcessDone()
                //.EndEvent()
                //.SubProcessDone()
                //.UserTask("userTask2")
                //.IntermediateThrowEvent("compensationEvent")
                //.CompensateEventDefinition()
                //.WaitForCompletion(true)
                //.CompensateEventDefinitionDone()
                //.EndEvent()
                .Done()
            ;

        public static readonly IBpmnModelInstance COMPENSATION_END_EVENT_MODEL = ProcessModels.NewModel()
            .StartEvent()
            .UserTask("userTask1")
            .BoundaryEvent("compensationBoundary")
            //.CompensateEventDefinition()
            //.CompensateEventDefinitionDone()
            //.MoveToActivity("userTask1")
            .UserTask("userTask2")
            .EndEvent("compensationEvent")
            //.CompensateEventDefinition()
            //.WaitForCompletion(true)
            .Done();


        public static readonly IBpmnModelInstance TRANSACTION_COMPENSATION_MODEL =
                ModifiableBpmnModelInstance.Modify(TransactionModels.CANCEL_BOUNDARY_EVENT)
            //.ActivityBuilder("userTask")
            //.BoundaryEvent("compensationBoundary")
            //.CompensateEventDefinition()
            //.CompensateEventDefinitionDone()
            //.Done()
            ;

        public static readonly IBpmnModelInstance COMPENSATION_EVENT_SUBPROCESS_MODEL =
            ModifiableBpmnModelInstance.Modify(COMPENSATION_ONE_TASK_SUBPROCESS_MODEL)
                .AddSubProcessTo("subProcess")
                //.Id("eventSubProcess")
                .TriggerByEvent()
                //.EmbeddedSubProcess()
                //.StartEvent("eventSubProcessStart")
                //.CompensateEventDefinition()
                //.CompensateEventDefinitionDone()
                .UserTask("eventSubProcessTask")
                .IntermediateThrowEvent("eventSubProcessCompensationEvent")
                //.CompensateEventDefinition()
                //.WaitForCompletion(true)
                //.CompensateEventDefinitionDone()
                .EndEvent()
                .EndEvent()
                .Done();

        static CompensationModels()
        {
            addUserTaskCompensationHandler(ONE_COMPENSATION_TASK_MODEL, "compensationBoundary", "compensationHandler");
            addUserTaskCompensationHandler(COMPENSATION_ONE_TASK_SUBPROCESS_MODEL, "compensationBoundary",
                "compensationHandler");
            addUserTaskCompensationHandler(COMPENSATION_TWO_TASKS_SUBPROCESS_MODEL, "compensationBoundary",
                "compensationHandler");
            addUserTaskCompensationHandler(DOUBLE_SUBPROCESS_MODEL, "compensationBoundary",
                "compensationHandler");
            addUserTaskCompensationHandler(COMPENSATION_END_EVENT_MODEL, "compensationBoundary", "compensationHandler");
            addUserTaskCompensationHandler(TRANSACTION_COMPENSATION_MODEL, "compensationBoundary", "compensationHandler");
        }

        public static void addUserTaskCompensationHandler(IBpmnModelInstance modelInstance, string boundaryEventId,
            string compensationHandlerId)
        {
            var boundaryEvent = (IBoundaryEvent)modelInstance.GetModelElementById/*<IBoundaryEvent>*/(boundaryEventId);
            var scope = (IBaseElement) boundaryEvent.ParentElement;

            var compensationHandler = modelInstance.NewInstance<IUserTask>(typeof(IUserTask));
            compensationHandler.Id = compensationHandlerId;
            compensationHandler.ForCompensation = true;
            scope.AddChildElement(compensationHandler);

            var association = modelInstance.NewInstance<IAssociation>(typeof(IAssociation));
            association.AssociationDirection = AssociationDirection.One;
            association.Source = boundaryEvent;
            association.Target = compensationHandler;
            scope.AddChildElement(association);
        }
    }
}