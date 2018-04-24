

using ESS.FW.Bpm.Model.Xml.impl.util;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type.child;

namespace ESS.FW.Bpm.Model.Xml.impl.type.reference
{
    /// <summary>
    /// 
    /// </summary>
    public class QNameElementReferenceImpl<TTarget, TSource> : ElementReferenceImpl<TTarget, TSource> 
        where TTarget : IModelElementInstance where TSource : IModelElementInstance
    {

        public QNameElementReferenceImpl(IChildElement referenceSourceCollection) : base(referenceSourceCollection)
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