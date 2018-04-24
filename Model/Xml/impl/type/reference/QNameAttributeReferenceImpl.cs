

using ESS.FW.Bpm.Model.Xml.impl.type.attribute;
using ESS.FW.Bpm.Model.Xml.impl.util;
using ESS.FW.Bpm.Model.Xml.instance;

namespace ESS.FW.Bpm.Model.Xml.impl.type.reference
{
    public class QNameAttributeReferenceImpl<T> : AttributeReferenceImpl<T> 
        where T : IModelElementInstance
    {
        public QNameAttributeReferenceImpl(AttributeImpl referenceSourceAttribute) : base(referenceSourceAttribute)
        {
        }

        public override string GetReferenceIdentifier(IModelElementInstance referenceSourceElement)
        {
            string identifier = base.GetReferenceIdentifier(referenceSourceElement);
            if (string.IsNullOrEmpty(identifier)) return null;
            QName qName = QName.ParseQName(identifier);
            return qName.LocalName;
        }
    }
}