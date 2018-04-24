using ESS.FW.Bpm.Model.Bpmn.instance.camunda;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.camunda
{

    public class CamundaConnectorImpl : BpmnModelElementInstanceImpl, ICamundaConnector
    {

        protected internal static IChildElement/*<ICamundaConnectorId>*/ CamundaConnectorIdChild;
        protected internal static IChildElement/*<ICamundaInputOutput>*/ CamundaInputOutputChild;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICamundaConnector>(/*typeof(ICamundaConnector),*/ BpmnModelConstants.CamundaElementConnector)
                    .NamespaceUri(BpmnModelConstants.CamundaNs)
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            CamundaConnectorIdChild = sequenceBuilder.Element<ICamundaConnectorId>(/*typeof(ICamundaConnectorId)*/).Required().Build/*<ICamundaConnectorId>*/();

            CamundaInputOutputChild = sequenceBuilder.Element<ICamundaInputOutput>(/*typeof(ICamundaInputOutput)*/).Build/*<ICamundaInputOutput>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICamundaConnector>
        {
            public virtual ICamundaConnector NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CamundaConnectorImpl(instanceContext);
            }
        }

        public CamundaConnectorImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual ICamundaConnectorId CamundaConnectorId
        {
            get => (ICamundaConnectorId)CamundaConnectorIdChild.GetChild(this);
            set => CamundaConnectorIdChild.SetChild(this, value);
        }


        public virtual ICamundaInputOutput CamundaInputOutput
        {
            get => (ICamundaInputOutput)CamundaInputOutputChild.GetChild(this);
            set => CamundaInputOutputChild.SetChild(this, value);
        }


    }

}