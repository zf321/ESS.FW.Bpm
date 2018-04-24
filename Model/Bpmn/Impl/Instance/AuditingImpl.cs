

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    /// <summary>
    /// The BPMN auditing element
    /// 
    /// 
    /// </summary>
    public class AuditingImpl : BaseElementImpl, IAuditing
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {            
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IAuditing>(/*typeof(IAuditing),*/ BpmnModelConstants.BpmnElementAuditing)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IAuditing>
        {
            public virtual IAuditing NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new AuditingImpl(instanceContext);
            }
        }

        public AuditingImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }

}