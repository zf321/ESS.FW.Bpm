using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class OutputSetImpl : BaseElementImpl, IOutputSet
    {

        protected internal static IAttribute/*<string>*/ NameAttribute;
        protected internal static IElementReferenceCollection DataOutputRefsCollection;//IElementReferenceCollection<IDataOutput, DataOutputRefs>
        protected internal static IElementReferenceCollection OptionalOutputRefsCollection;//IElementReferenceCollection<IDataOutput, OptionalOutputRefs>
        protected internal static IElementReferenceCollection WhileExecutingOutputRefsCollection;//IElementReferenceCollection<IDataOutput, WhileExecutingOutputRefs>
        protected internal static IElementReferenceCollection InputSetInputSetRefsCollection;//IElementReferenceCollection<INputSet, InputSetRefs>

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IOutputSet>(/*typeof(IOutputSet),*/ BpmnModelConstants.BpmnElementOutputSet)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            NameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeName).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            DataOutputRefsCollection = sequenceBuilder
                .ElementCollection<DataOutputRefs>(/*typeof(DataOutputRefs)*/)
                .IdElementReferenceCollection<IDataOutput>(/*typeof(IDataOutput)*/)
                .Build();

            OptionalOutputRefsCollection = sequenceBuilder
                .ElementCollection<OptionalOutputRefs>(/*typeof(OptionalOutputRefs)*/)
                .IdElementReferenceCollection<IDataOutput>(/*typeof(IDataOutput)*/)
                .Build();

            WhileExecutingOutputRefsCollection = sequenceBuilder
                .ElementCollection<WhileExecutingOutputRefs>(/*typeof(WhileExecutingOutputRefs)*/)
                .IdElementReferenceCollection<IDataOutput>(/*typeof(IDataOutput)*/)
                .Build();

            InputSetInputSetRefsCollection = sequenceBuilder
                .ElementCollection<InputSetRefs>(/*typeof(InputSetRefs)*/)
                .IdElementReferenceCollection<INputSet>(/*typeof(INputSet)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IOutputSet>
        {
            public virtual IOutputSet NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new OutputSetImpl(instanceContext);
            }
        }

        public OutputSetImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string Name
        {
            get => NameAttribute.GetValue<String>(this);
            set => NameAttribute.SetValue(this, value);
        }


        public virtual ICollection<IDataOutput> DataOutputRefs => DataOutputRefsCollection.GetReferenceTargetElements<IDataOutput>(this);

        public virtual ICollection<IDataOutput> OptionalOutputRefs => OptionalOutputRefsCollection.GetReferenceTargetElements<IDataOutput>(this);

        public virtual ICollection<IDataOutput> WhileExecutingOutputRefs => WhileExecutingOutputRefsCollection.GetReferenceTargetElements<IDataOutput>(this);

        public virtual ICollection<INputSet> InputSetRefs => InputSetInputSetRefsCollection.GetReferenceTargetElements<INputSet>(this);
    }
}