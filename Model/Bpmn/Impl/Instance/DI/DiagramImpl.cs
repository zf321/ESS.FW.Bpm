using System;
using ESS.FW.Bpm.Model.Bpmn.instance.di;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.di
{
    public abstract class DiagramImpl : BpmnModelElementInstanceImpl, IDiagram
    {

        protected internal static IAttribute/*<string>*/ NameAttribute;
        protected internal static IAttribute/*<string>*/ DocumentationAttribute;
        protected internal static IAttribute/*<double?>*/ ResolutionAttribute;
        protected internal static IAttribute/*<string>*/ IdAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IDiagram>(/*typeof(IDiagram),*/ BpmnModelConstants.DiElementDiagram)
                .NamespaceUri(BpmnModelConstants.DiNs)
                .AbstractType();

            NameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.DiAttributeName).Build();

            DocumentationAttribute = typeBuilder.StringAttribute(BpmnModelConstants.DiAttributeDocumentation).Build();

            ResolutionAttribute = typeBuilder.DoubleAttribute(BpmnModelConstants.DiAttributeResolution).Build();

            IdAttribute = typeBuilder.StringAttribute(BpmnModelConstants.DiAttributeId).IdAttribute().Build();

            typeBuilder.Build();
        }

        public DiagramImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string Name
        {
            get { return NameAttribute.GetValue<String>(this); }
            set { NameAttribute.SetValue(this, value); }
        }


        public virtual string Documentation
        {
            get { return DocumentationAttribute.GetValue<String>(this); }
            set { DocumentationAttribute.SetValue(this, value); }
        }


        public virtual double Resolution
        {
            get { return ResolutionAttribute.GetValue<Double?>(this).GetValueOrDefault(); }
            set { ResolutionAttribute.SetValue(this, value); }
        }


        public override string Id
        {
            get { return IdAttribute.GetValue<String>(this); }
            set { IdAttribute.SetValue(this, value); }
        }
    }
}