using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class TimerEventDefinitionImpl : EventDefinitionImpl, ITimerEventDefinition
    {

        protected internal static IChildElement/*<ITimeDate>*/ TimeDateChild;
        protected internal static IChildElement/*<ITimeDuration>*/ TimeDurationChild;
        protected internal static IChildElement/*<ITimeCycle>*/ TimeCycleChild;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ITimerEventDefinition>(/*typeof(ITimerEventDefinition), */BpmnModelConstants.BpmnElementTimerEventDefinition)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IEventDefinition))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            TimeDateChild = sequenceBuilder.Element<ITimeDate>(/*typeof(ITimeDate)*/).Build/*<ITimeDate>*/();

            TimeDurationChild = sequenceBuilder.Element<ITimeDuration>(/*typeof(ITimeDuration)*/).Build/*<ITimeDuration>*/();

            TimeCycleChild = sequenceBuilder.Element<ITimeCycle>(/*typeof(ITimeCycle)*/).Build/*<ITimeCycle>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ITimerEventDefinition>
        {
            public virtual ITimerEventDefinition NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new TimerEventDefinitionImpl(instanceContext);
            }
        }

        public TimerEventDefinitionImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public virtual ITimeDate TimeDate
        {
            get => (ITimeDate)TimeDateChild.GetChild(this);
            set => TimeDateChild.SetChild(this, value);
        }


        public virtual ITimeDuration TimeDuration
        {
            get => (ITimeDuration)TimeDurationChild.GetChild(this);
            set => TimeDurationChild.SetChild(this, value);
        }


        public virtual ITimeCycle TimeCycle
        {
            get => (ITimeCycle)TimeCycleChild.GetChild(this);
            set => TimeCycleChild.SetChild(this, value);
        }
    }
}