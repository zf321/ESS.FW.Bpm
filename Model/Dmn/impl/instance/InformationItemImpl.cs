

using System;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    
    public class InformationItemImpl : NamedElementImpl, INformationItem
    {

        protected internal static IAttribute/*<string>*/ TypeRefAttribute;

        public InformationItemImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string TypeRef
        {
            get
            {
                return TypeRefAttribute.GetValue<String>(this);
            }
            set
            {
                TypeRefAttribute.SetValue(this, value);
            }
        }


        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<INformationItem>(/*typeof(INformationItem),*/ DmnModelConstants.DmnElementInformationItem).NamespaceUri(DmnModelConstants.Dmn11Ns).ExtendsType(typeof(INamedElement)).InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            TypeRefAttribute = typeBuilder.StringAttribute(DmnModelConstants.DmnAttributeTypeRef).Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<INformationItem>
        {
            public ModelTypeInstanceProviderAnonymousInnerClass()
            {
            }

            public virtual INformationItem NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new InformationItemImpl(instanceContext);
            }
        }
    }
}