using System;
using ESS.FW.Bpm.Model.Xml.impl.type.attribute;
using ESS.FW.Bpm.Model.Xml.instance;



namespace ESS.FW.Bpm.Model.Xml.impl.type.reference
{
    public class QNameAttributeReferenceBuilderImpl<T> : AttributeReferenceBuilderImpl<T> 
        where T : IModelElementInstance
    {
        
        public QNameAttributeReferenceBuilderImpl(AttributeImpl referenceSourceAttribute) 
            : base(referenceSourceAttribute)
        {
            this.AttributeReferenceImpl = new QNameAttributeReferenceImpl<T>(referenceSourceAttribute);
        }
    }
}