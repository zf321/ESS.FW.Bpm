

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;
using ESS.FW.Bpm.Model.Bpmn.instance.dc;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{


    /// <summary>
    /// 
    /// </summary>
    public class EmbeddedSubProcessBuilder : AbstractEmbeddedSubProcessBuilder<AbstractSubProcessBuilder/*<SubProcessBuilder>*/>
    {

        protected internal EmbeddedSubProcessBuilder(AbstractSubProcessBuilder/*<SubProcessBuilder>*/ subProcessBuilder) : base(subProcessBuilder)
        {
        }

        public virtual StartEventBuilder StartEvent()
        {
            return StartEvent(null);
        }

        public virtual StartEventBuilder StartEvent(string id)
        {
            IStartEvent start = SubProcessBuilder.CreateChild<IStartEvent>(typeof(IStartEvent), id);

            IBpmnShape startShape = SubProcessBuilder.CreateBpmnShape(start);
            IBpmnShape subProcessShape = SubProcessBuilder.FindBpmnShape(SubProcessBuilder.element);

            if (subProcessShape != null)
            {
                IBounds subProcessBounds = subProcessShape.Bounds;
                IBounds startBounds = startShape.Bounds;

                double subProcessX = subProcessBounds.GetX().Value;
                double subProcessY = subProcessBounds.GetY().Value;
                double subProcessHeight = subProcessBounds.GetHeight().Value;
                double startHeight = startBounds.GetHeight().Value;

                startBounds.SetX(subProcessX);
                startBounds.SetY(subProcessY + subProcessHeight / 2 - startHeight / 2);
            }

            return start.Builder<StartEventBuilder,IStartEvent>();
        }
    }

}