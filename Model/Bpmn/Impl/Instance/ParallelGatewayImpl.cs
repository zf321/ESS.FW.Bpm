using System;
using ESS.FW.Bpm.Model.Bpmn.builder;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class ParallelGatewayImpl : GatewayImpl, IParallelGateway
    {

        protected internal static IAttribute/*<bool>*/ CamundaAsyncAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IParallelGateway>(/*typeof(IParallelGateway),*/ BpmnModelConstants.BpmnElementParallelGateway)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IGateway))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            CamundaAsyncAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.CamundaAttributeAsync)
                .Namespace(BpmnModelConstants.CamundaNs)
                .DefaultValue(false)
                .Build();

            typeBuilder.Build();
        }

        public new ParallelGatewayBuilder Builder()
        {
            return new ParallelGatewayBuilder((IBpmnModelInstance)modelInstance, this);
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IParallelGateway>
        {
            public virtual IParallelGateway NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ParallelGatewayImpl(instanceContext);
            }
        }

        /// <summary>
        /// camunda extensions </summary>

        /// @deprecated use isCamundaAsyncBefore() instead. 
        [Obsolete("use isCamundaAsyncBefore() instead.")]
        public virtual bool CamundaAsync
        {
            get { return CamundaAsyncAttribute.GetValue<Boolean>(this); }
            set { CamundaAsyncAttribute.SetValue(this, value); }
        }


        public ParallelGatewayImpl(ModelTypeInstanceContext context) : base(context)
        {
        }
    }
}