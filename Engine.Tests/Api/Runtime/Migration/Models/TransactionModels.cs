using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml.instance;

namespace Engine.Tests.Api.Runtime.Migration.Models
{
    /// <summary>
    /// </summary>
    public class TransactionModels
    {
        public static readonly IBpmnModelInstance ONE_TASK_TRANSACTION = ProcessModels.NewModel()
            .StartEvent()
            .Transaction("transaction")
            //.EmbeddedSubProcess()
            //.StartEvent()
            .UserTask("userTask")
            .EndEvent("transactionEndEvent")
            .TransactionDone()
            .EndEvent()
            .Done();

        public static readonly IBpmnModelInstance CANCEL_BOUNDARY_EVENT =
                ModifiableBpmnModelInstance.Modify(ONE_TASK_TRANSACTION)
            //.ActivityBuilder("transaction")
            //.BoundaryEvent("boundaryEvent")
            //.UserTask("afterBoundaryTask")
            //.EndEvent()
            //.Done()
            ;

        static TransactionModels()
        {
            makeCancelEvent(CANCEL_BOUNDARY_EVENT, "transactionEndEvent");
            makeCancelEvent(CANCEL_BOUNDARY_EVENT, "boundaryEvent");
        }

        protected internal static void makeCancelEvent(IBpmnModelInstance model, string eventId)
        {
            var element = model.GetModelElementById/*<IModelElementInstance>*/(eventId) as IModelElementInstance;

            var eventDefinition = model.NewInstance<ICancelEventDefinition>(typeof(ICancelEventDefinition));
            element.AddChildElement(eventDefinition);
        }
    }
}