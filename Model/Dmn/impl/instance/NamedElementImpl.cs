

using System;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    
    public abstract class NamedElementImpl : DmnElementImpl, INamedElement
    {

        protected internal static IAttribute/*<string>*/ NameAttribute;

        public NamedElementImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string Name
        {
            get
            {
                return NameAttribute.GetValue<String>(this);
            }
            set
            {
                NameAttribute.SetValue(this, value);
            }
        }


        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<INamedElement>(/*typeof(INamedElement), */DmnModelConstants.DmnElementNamedElement).NamespaceUri(DmnModelConstants.Dmn11Ns).ExtendsType(typeof(IDmnElement)).AbstractType();

            NameAttribute = typeBuilder.StringAttribute(DmnModelConstants.DmnAttributeName).Required().Build();

            typeBuilder.Build();
        }
    }
}