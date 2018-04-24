using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class LaneSetImpl : BaseElementImpl, ILaneSet
    {

        protected internal static IAttribute/*<string>*/ NameAttribute;
        protected internal static IChildElementCollection/*<ILane>*/ LaneCollection;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ILaneSet>(/*typeof(ILaneSet),*/ BpmnModelConstants.BpmnElementLaneSet)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            NameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeName).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            LaneCollection = sequenceBuilder.ElementCollection<ILane>(/*typeof(ILane)*/).Build/*<ILane>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ILaneSet>
        {
            public virtual ILaneSet NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new LaneSetImpl(instanceContext);
            }
        }

        public LaneSetImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string Name
        {
            get => NameAttribute.GetValue<String>(this);
            set => NameAttribute.SetValue(this, value);
        }


        public virtual ICollection<ILane> Lanes => LaneCollection.Get<ILane>(this);
    }

}