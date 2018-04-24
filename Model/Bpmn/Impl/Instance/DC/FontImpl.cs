

using System;
using ESS.FW.Bpm.Model.Bpmn.instance.dc;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.dc
{
    public class FontImpl : BpmnModelElementInstanceImpl, IFont
    {

        protected internal static IAttribute/*<string>*/ NameAttribute;
        protected internal static IAttribute/*<double?>*/ SizeAttribute;
        protected internal static IAttribute/*<bool>*/ IsBoldAttribute;
        protected internal static IAttribute/*<bool>*/ IsItalicAttribute;
        protected internal static IAttribute/*<bool>*/ IsUnderlineAttribute;
        protected internal static IAttribute/*<bool>*/ IsStrikeTroughAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IFont>(/*typeof(IFont),*/ BpmnModelConstants.DcElementFont)
                    .NamespaceUri(BpmnModelConstants.DcNs)
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            NameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.DcAttributeName).Build();

            SizeAttribute = typeBuilder.DoubleAttribute(BpmnModelConstants.DcAttributeSize).Build();

            IsBoldAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.DcAttributeIsBold).Build();

            IsItalicAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.DcAttributeIsItalic).Build();

            IsUnderlineAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.DcAttributeIsUnderline).Build();

            IsStrikeTroughAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.DcAttributeIsStrikeThrough).Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IFont>
        {
            public virtual IFont NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new FontImpl(instanceContext);
            }
        }

        public FontImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string Name
        {
            get { return NameAttribute.GetValue<String>(this); }
            set { NameAttribute.SetValue(this, value); }
        }


        public virtual double? Size
        {
            get { return SizeAttribute.GetValue<Double?>(this); }
            set { SizeAttribute.SetValue(this, value); }
        }


        public virtual bool? Bold
        {
            get { return IsBoldAttribute.GetValue<Boolean?>(this); }
            set { IsBoldAttribute.SetValue(this, value.GetValueOrDefault()); }
        }

        public virtual bool? Italic
        {
            get { return IsItalicAttribute.GetValue<Boolean?>(this); }
            set { IsItalicAttribute.SetValue(this, value.GetValueOrDefault()); }
        }
        

        public virtual bool? Underline
        {
            get { return IsUnderlineAttribute.GetValue<Boolean?>(this); }
            set { IsUnderlineAttribute.SetValue(this,value.GetValueOrDefault());}
        }
        

        public virtual bool? StrikeThrough
        {
            get { return IsStrikeTroughAttribute.GetValue<Boolean?>(this); }
            set{IsStrikeTroughAttribute.SetValue(this,value.GetValueOrDefault());}
        }
        
    }

}