using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class IoSpecificationImpl : BaseElementImpl, IOSpecification
    {

        protected internal static IChildElementCollection/*<IDataInput>*/ DataInputCollection;
        protected internal static IChildElementCollection/*<IDataOutput>*/ DataOutputCollection;
        protected internal static IChildElementCollection/*<INputSet>*/ InputSetCollection;
        protected internal static IChildElementCollection/*<IOutputSet>*/ OutputSetCollection;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IOSpecification>(/*typeof(IOSpecification),*/ BpmnModelConstants.BpmnElementIoSpecification)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            DataInputCollection = sequenceBuilder.ElementCollection<IDataInput>(/*typeof(IDataInput)*/).Build/*<IDataInput>*/();

            DataOutputCollection = sequenceBuilder.ElementCollection<IDataOutput>(/*typeof(IDataOutput)*/).Build/*<IDataOutput>*/();

            InputSetCollection = sequenceBuilder.ElementCollection<INputSet>(/*typeof(INputSet)*/).Required().Build/*<INputSet>*/();

            OutputSetCollection = sequenceBuilder.ElementCollection<IOutputSet>(/*typeof(IOutputSet)*/).Required().Build/*<IOutputSet>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IOSpecification>
        {
            public virtual IOSpecification NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new IoSpecificationImpl(instanceContext);
            }
        }

        public IoSpecificationImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual ICollection<IDataInput> DataInputs => DataInputCollection.Get<IDataInput>(this);

        public virtual ICollection<IDataOutput> DataOutputs => DataOutputCollection.Get<IDataOutput>(this);

        public virtual ICollection<INputSet> InputSets => InputSetCollection.Get<INputSet>(this);

        public virtual ICollection<IOutputSet> OutputSets => OutputSetCollection.Get<IOutputSet>(this);
    }

}