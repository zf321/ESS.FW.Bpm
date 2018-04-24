

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;
using ESS.FW.Bpm.Model.Bpmn.instance.di;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class DataInputAssociationImpl : DataAssociationImpl, IDataInputAssociation
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IDataInputAssociation>(/*typeof(IDataInputAssociation),*/ BpmnModelConstants.BpmnElementDataInputAssociation)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IDataAssociation))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IDataInputAssociation>
        {
            public virtual IDataInputAssociation NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new DataInputAssociationImpl(instanceContext);
            }
        }

        public DataInputAssociationImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        //public override IDiagramElement DiagramElement
        //{
        //    get
        //    {
        //        return (IBpmnEdge)base.DiagramElement;
        //    }
        //}
    }
}