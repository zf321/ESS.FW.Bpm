

using ESS.FW.Bpm.Model.Xml.impl.util;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type.child;

namespace ESS.FW.Bpm.Model.Xml.impl.type.reference
{

    using QName = QName;
    /// <summary>
    /// 
    /// </summary>
    public class QNameElementReferenceCollectionImpl<TTarget, TSource> : ElementReferenceCollectionImpl<TTarget, TSource> 
        where TTarget : IModelElementInstance where TSource : IModelElementInstance
    {

        public QNameElementReferenceCollectionImpl(IChildElementCollection referenceSourceCollection) 
            : base(referenceSourceCollection)
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