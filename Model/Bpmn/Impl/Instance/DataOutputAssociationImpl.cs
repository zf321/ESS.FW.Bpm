using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class DataOutputAssociationImpl : DataAssociationImpl, IDataOutputAssociation
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IDataOutputAssociation>(/*typeof(IDataOutputAssociation),*/ BpmnModelConstants.BpmnElementDataOutputAssociation)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IDataAssociation))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IDataOutputAssociation>
        {
            public virtual IDataOutputAssociation NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new DataOutputAssociationImpl(instanceContext);
            }
        }

        public DataOutputAssociationImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }

}