using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type.child;

namespace ESS.FW.Bpm.Model.Xml.impl.type.reference
{
    public class UriElementReferenceCollectionImpl<TTarget, TSource> : ElementReferenceCollectionImpl<TTarget, TSource> 
        where TTarget : IModelElementInstance where TSource : IModelElementInstance
    {

        public UriElementReferenceCollectionImpl(IChildElementCollection referenceSourceCollection) : base(referenceSourceCollection)
        {
        }

        public override string GetReferenceIdentifier(IModelElementInstance referenceSourceElement)
        {
            // TODO: implement something more robust (CAM-4028)
            string identifier = referenceSourceElement.GetAttributeValue("href");
            if (string.IsNullOrEmpty(identifier)) return null;
            string[] parts = identifier.Split('#');
            if (parts.Length > 1)
            {
                return parts[parts.Length - 1];
            }
            return parts[0];
        }

        protected internal override void SetReferenceIdentifier(IModelElementInstance referenceSourceElement, string referenceIdentifier)
        {
            // TODO: implement something more robust (CAM-4028)
            referenceSourceElement.SetAttributeValue("href", "#" + referenceIdentifier);
        }
    }
}