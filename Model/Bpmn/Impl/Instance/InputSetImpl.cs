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
    public class InputSetImpl : BaseElementImpl, INputSet
    {

        protected internal static IAttribute/*<string>*/ NameAttribute;
        protected internal static IElementReferenceCollection DataInputDataInputRefsCollection;//IElementReferenceCollection<IDataInput, DataInputRefs>
        protected internal static IElementReferenceCollection OptionalInputRefsCollection;//IElementReferenceCollection<IDataInput, OptionalInputRefs>
        protected internal static IElementReferenceCollection WhileExecutingInputRefsCollection;//IElementReferenceCollection<IDataInput, WhileExecutingInputRefs>
        protected internal static IElementReferenceCollection OutputSetOutputSetRefsCollection;//IElementReferenceCollection<IOutputSet, OutputSetRefs>

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<INputSet>(/*typeof(INputSet),*/ BpmnModelConstants.BpmnElementInputSet)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            //NameAttribute = typeBuilder.StringAttribute("name").Build();
            NameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeName).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            DataInputDataInputRefsCollection = sequenceBuilder
                .ElementCollection<DataInputRefs>(/*typeof(DataInputRefs)*/)
                .IdElementReferenceCollection<IDataInput>(/*typeof(IDataInput)*/)
                .Build();

            OptionalInputRefsCollection = sequenceBuilder
                .ElementCollection<OptionalInputRefs>(/*typeof(OptionalInputRefs)*/)
                .IdElementReferenceCollection<IDataInput>(/*typeof(IDataInput)*/)
                .Build();

            WhileExecutingInputRefsCollection = sequenceBuilder
                .ElementCollection<WhileExecutingInputRefs>(/*typeof(WhileExecutingInputRefs)*/)
                .IdElementReferenceCollection<IDataInput>(/*typeof(IDataInput)*/)
                .Build();

            OutputSetOutputSetRefsCollection = sequenceBuilder
                .ElementCollection<OutputSetRefs>(/*typeof(OutputSetRefs)*/)
                .IdElementReferenceCollection<IOutputSet>(/*typeof(IOutputSet)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<INputSet>
        {
            public virtual INputSet NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new InputSetImpl(instanceContext);
            }
        }

        public InputSetImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string Name
        {
            get => NameAttribute.GetValue<String>(this);
            set => NameAttribute.SetValue(this, value);
        }


        public virtual ICollection<IDataInput> DataInputs => DataInputDataInputRefsCollection.GetReferenceTargetElements<IDataInput>(this);

        public virtual ICollection<IDataInput> OptionalInputs => OptionalInputRefsCollection.GetReferenceTargetElements<IDataInput>(this);

        public virtual ICollection<IDataInput> WhileExecutingInput => WhileExecutingInputRefsCollection.GetReferenceTargetElements<IDataInput>(this);

        public virtual ICollection<IOutputSet> OutputSets => OutputSetOutputSetRefsCollection.GetReferenceTargetElements<IOutputSet>(this);
    }
}