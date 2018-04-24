using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;
using ESS.FW.Bpm.Model.Bpmn.instance.dc;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
    /// </summary>
    public abstract class AbstractActivityBuilder<TE> : AbstractFlowNodeBuilder<TE>, IActivityBuilder<TE>
        where TE : IActivity
    {
        protected internal AbstractActivityBuilder(IBpmnModelInstance modelInstance, TE element) : base(modelInstance,
            element)
        {
        }


        protected internal virtual IBpmnShape BoundaryEventCoordinates
        {
            set
            {
                var activity = FindBpmnShape(element);
                var boundaryBounds = value.Bounds;

                double x = 0;
                double y = 0;

                if (activity != null)
                {
                    var activityBounds = activity.Bounds;
                    var activityY = activityBounds.GetY()
                        .Value;
                    var activityHeight = activityBounds.GetHeight()
                        .Value;
                    var boundaryHeight = boundaryBounds.GetHeight()
                        .Value;
                    x = CalculateXCoordinate(boundaryBounds);
                    y = activityY + activityHeight - boundaryHeight / 2;
                }

                boundaryBounds.SetX(x);
                boundaryBounds.SetY(y);
            }
        }

        public virtual BoundaryEventBuilder BoundaryEvent()
        {
            return BoundaryEvent(null);
        }

        public virtual BoundaryEventBuilder BoundaryEvent(string id)
        {
            var boundaryEvent = CreateSibling<IBoundaryEvent>(typeof(IBoundaryEvent), id);
            boundaryEvent.AttachedTo = element;

            var boundaryEventBpmnShape = CreateBpmnShape(boundaryEvent);
            BoundaryEventCoordinates = boundaryEventBpmnShape;

            return boundaryEvent.Builder();
        }

        public virtual MultiInstanceLoopCharacteristicsBuilder MultiInstance()
        {
            var miCharacteristics =
                CreateChild<IMultiInstanceLoopCharacteristics>(typeof(IMultiInstanceLoopCharacteristics));

            return miCharacteristics.Builder();
        }

        /// <summary>
        ///     Creates a new camunda input parameter extension element with the
        ///     given name and value.
        /// </summary>
        /// <param name="name"> the name of the input parameter </param>
        /// <param name="value"> the value of the input parameter </param>
        /// <returns> the builder object </returns>
        public virtual IActivityBuilder<TE> CamundaInputParameter(string name, string value)
        {
            var camundaInputOutput = GetCreateSingleExtensionElement<ICamundaInputOutput>(typeof(ICamundaInputOutput));

            var camundaInputParameter =
                CreateChild<ICamundaInputParameter>(camundaInputOutput, typeof(ICamundaInputParameter));
            camundaInputParameter.CamundaName = name;
            camundaInputParameter.TextContent = value;

            return this;
        }

        /// <summary>
        ///     Creates a new camunda output parameter extension element with the
        ///     given name and value.
        /// </summary>
        /// <param name="name"> the name of the output parameter </param>
        /// <param name="value"> the value of the output parameter </param>
        /// <returns> the builder object </returns>
        public virtual IActivityBuilder<TE> CamundaOutputParameter(string name, string value)
        {
            var camundaInputOutput = GetCreateSingleExtensionElement<ICamundaInputOutput>(typeof(ICamundaInputOutput));

            var camundaOutputParameter =
                CreateChild<ICamundaOutputParameter>(camundaInputOutput, typeof(ICamundaOutputParameter));
            camundaOutputParameter.CamundaName = name;
            camundaOutputParameter.TextContent = value;

            return this;
        }

        protected internal virtual double CalculateXCoordinate(IBounds boundaryEventBounds)
        {
            var attachedToElement = FindBpmnShape(element);

            double x = 0;

            if (attachedToElement != null)
            {
                var attachedToBounds = attachedToElement.Bounds;

                ICollection<IBoundaryEvent> boundaryEvents =
                    element.ParentElement.GetChildElementsByType<IBoundaryEvent>(typeof(IBoundaryEvent));
                ICollection<IBoundaryEvent> attachedBoundaryEvents = new List<IBoundaryEvent>();

                var iterator = boundaryEvents.GetEnumerator();
                while (iterator.MoveNext())
                {
                    var tmp = iterator.Current;
                    if (tmp.AttachedTo.Equals(element))
                        attachedBoundaryEvents.Add(tmp);
                }

                var attachedToX = attachedToBounds.GetX()
                    .Value;
                var attachedToWidth = attachedToBounds.GetWidth()
                    .Value;
                var boundaryWidth = boundaryEventBounds.GetWidth()
                    .Value;

                switch (attachedBoundaryEvents.Count)
                {
                    case 2:
                    {
                        x = attachedToX + attachedToWidth / 2 + boundaryWidth / 2;
                        break;
                    }
                    case 3:
                    {
                        x = attachedToX + attachedToWidth / 2 - 1.5 * boundaryWidth;
                        break;
                    }
                    default:
                    {
                        x = attachedToX + attachedToWidth / 2 - boundaryWidth / 2;
                        break;
                    }
                }
            }

            return x;
        }
    }
}