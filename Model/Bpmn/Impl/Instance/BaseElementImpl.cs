using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.di;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.util;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;


namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{

    /// <summary>
    /// The BPMN baseElement element
    /// 
    /// 
    /// 
    /// </summary>
    public abstract class BaseElementImpl : BpmnModelElementInstanceImpl, IBaseElement
    {

        protected internal static IAttribute/*<string>*/ IdAttribute;
        protected internal static IChildElementCollection/*<IDocumentation>*/ DocumentationCollection;
        protected internal static IChildElement/*<IExtensionElements>*/ ExtensionElementsChild;

        public new static void RegisterType(ModelBuilder bpmnModelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = bpmnModelBuilder.DefineType<IBaseElement>(/*typeof(IBaseElement),*/ BpmnModelConstants.BpmnElementBaseElement)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .AbstractType();

            IdAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeId).IdAttribute().Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            DocumentationCollection = sequenceBuilder.ElementCollection<IDocumentation>(/*typeof(IDocumentation)*/).Build/*<IDocumentation>*/();

            ExtensionElementsChild = sequenceBuilder.Element<IExtensionElements>(/*typeof(IExtensionElements)*/).Build/*<IExtensionElements>*/();

            typeBuilder.Build();
        }

        public BaseElementImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public override string Id
        {
            get
            {
                return IdAttribute.GetValue<String>(this);
            }
            set
            {
                IdAttribute.SetValue(this, value);
            }
        }


        public virtual ICollection<IDocumentation> Documentations
        {
            get
            {
                return DocumentationCollection.Get<IDocumentation>(this);
            }
        }

        public virtual IExtensionElements ExtensionElements
        {
            get
            {
                return (IExtensionElements)ExtensionElementsChild.GetChild(this);
            }
            set
            {
                ExtensionElementsChild.SetChild(this, value);
            }
        }

        
        public virtual IDiagramElement DiagramElement
        {
            get
            {
                //ICollection<IDiagramElement> incomingReferences = GetIncomingReferencesByType(typeof(IDiagramElement));
                ICollection<IReference> incomingReferences = GetIncomingReferencesByType(typeof(IDiagramElement));
                foreach (IReference reference in incomingReferences)
                {
                    foreach (IModelElementInstance sourceElement in reference.FindReferenceSourceElements(this))
                    {
                        string referenceIdentifier = reference.GetReferenceIdentifier(sourceElement);
                        if (!string.ReferenceEquals(referenceIdentifier, null) && referenceIdentifier.Equals(Id))
                        {
                            return (IDiagramElement)sourceElement;
                        }
                    }
                }
                return null;
            }
        }
        
        //public virtual ICollection<IDiagramElement> GetIncomingReferencesByType(Type referenceSourceTypeClass)
        //{
        //    ICollection<IDiagramElement> references = new List<IDiagramElement>();
        //    // we traverse all incoming references in reverse direction
        //    foreach (IReference reference in IdAttribute.IncomingReferences)
        //    {

        //        IModelElementType sourceElementType = reference.ReferenceSourceElementType;
        //        Type sourceInstanceType = sourceElementType.InstanceType;

        //        // if the referencing element (source element) is a BPMNDI element, dig deeper
        //        if (referenceSourceTypeClass.IsAssignableFrom(sourceInstanceType))
        //        {
        //            references.Add((IDiagramElement)reference);
        //        }
        //    }
        //    return references;
        //}

        public ICollection<IReference> GetIncomingReferencesByType(Type referenceSourceTypeClass)
        {
            ICollection<IReference> references = new List<IReference>();
            // we traverse all incoming references in reverse direction
            foreach (IReference reference in IdAttribute.IncomingReferences)
            {

                IModelElementType sourceElementType = reference.ReferenceSourceElementType;
                Type sourceInstanceType = sourceElementType.InstanceType;

                // if the referencing element (source element) is a BPMNDI element, dig deeper
                if (referenceSourceTypeClass.IsAssignableFrom(sourceInstanceType))
                {
                    references.Add(reference);
                }
            }
            return references;
        }

    }

}