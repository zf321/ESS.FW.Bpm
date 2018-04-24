using System;
using ESS.FW.Bpm.Model.Bpmn.instance;


namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    public abstract class AbstractCompensateEventDefinitionBuilder : AbstractRootElementBuilder<ICompensateEventDefinition>, ICompensateEventDefinitionBuilder
    {

        public AbstractCompensateEventDefinitionBuilder(IBpmnModelInstance modelInstance, ICompensateEventDefinition element) : base(modelInstance, element)
        {
        }

        public override ICompensateEventDefinitionBuilder Id<ICompensateEventDefinitionBuilder>(string identifier)
        {
            return base.Id<ICompensateEventDefinitionBuilder>(identifier);
        }

        public virtual ICompensateEventDefinitionBuilder ActivityRef(string activityId)
        {
            IActivity activity = (IActivity)modelInstance.GetModelElementById(activityId);

            if (activity == null)
            {
                throw new BpmnModelException("Activity with id '" + activityId + "' does not exist");
            }
            IEvent Event = (IEvent)element.ParentElement;
            if (activity.ParentElement != Event.ParentElement)
            {
                throw new BpmnModelException("Activity with id '" + activityId + "' must be in the same scope as '" + Event.Id + "'");
            }

            element.Activity = activity;
            return this;
        }

        public virtual ICompensateEventDefinitionBuilder WaitForCompletion(bool waitForCompletion)
        {
            element.WaitForCompletion = waitForCompletion;
            return this;
        }
        
        public virtual IEventBuilder<IEvent> CompensateEventDefinitionDone() /*where T : AbstractFlowNodeBuilder*/
        {
            return ((IEvent)element.ParentElement).Builder<IEventBuilder<IEvent>,IEvent>();
        }
    }
}