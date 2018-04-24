using System.Collections.Generic;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.util;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type.child;


namespace ESS.FW.Bpm.Model.Xml.impl.type.reference
{

    public class IdsElementReferenceCollectionImpl<TArget, TSource> : ElementReferenceCollectionImpl<TArget, TSource> 
        where TArget : IModelElementInstance where TSource : IModelElementInstance
    {

        protected internal char Separator = ' ';

        public IdsElementReferenceCollectionImpl(IChildElementCollection referenceSourceCollection) : base(referenceSourceCollection)
        {
        }

        protected internal virtual IList<string> GetReferenceIdentifiers(IModelElementInstance referenceSourceElement)
        {
            string referenceIdentifiers = GetReferenceIdentifier(referenceSourceElement);
            return StringUtil.SplitListBySeparator(referenceIdentifiers, Separator);
        }

        protected internal virtual void SetReferenceIdentifiers(IModelElementInstance referenceSourceElement, IList<string> referenceIdentifiers)
        {
            string referenceIdentifier = StringUtil.JoinList(referenceIdentifiers, Separator.ToString());
            referenceSourceElement.TextContent = referenceIdentifier;
        }

        protected internal override IList<IDomElement> GetView(ModelElementInstanceImpl referenceSourceParentElement)
        {
            IDomDocument document = referenceSourceParentElement.ModelInstance.Document;
            ICollection<TSource> referenceSourceElements = GetReferenceSourceCollection().Get<TSource>(referenceSourceParentElement);
            IList<IDomElement> referenceTargetElements = new List<IDomElement>();
            foreach (TSource referenceSourceElement in referenceSourceElements)
            {
                IList<string> identifiers = GetReferenceIdentifiers(referenceSourceElement);
                foreach (string identifier in identifiers)
                {
                    IDomElement referenceTargetElement = document.GetElementById(identifier);
                    if (referenceTargetElement != null)
                    {
                        referenceTargetElements.Add(referenceTargetElement);
                    }
                    else
                    {
                        throw new ModelException("Unable to find a model element instance for id " + identifier);
                    }
                }
            }
            return referenceTargetElements;
        }

        protected internal override void UpdateReference(IModelElementInstance referenceSourceElement, string oldIdentifier, string newIdentifier)
        {
            IList<string> referenceIdentifiers = GetReferenceIdentifiers(referenceSourceElement);
            if (referenceIdentifiers.Contains(oldIdentifier))
            {
                int index = referenceIdentifiers.IndexOf(oldIdentifier);
                referenceIdentifiers.Remove(oldIdentifier);
                referenceIdentifiers.Insert(index, newIdentifier);
                SetReferenceIdentifiers(referenceSourceElement, referenceIdentifiers);
            }
        }

        public override void ReferencedElementRemoved(IModelElementInstance referenceTargetElement, object referenceIdentifier)
        {
            foreach (IModelElementInstance referenceSourceElement in FindReferenceSourceElements(referenceTargetElement))
            {
                IList<string> referenceIdentifiers = GetReferenceIdentifiers(referenceSourceElement);
                if (referenceIdentifiers.Contains(referenceIdentifier.ToString()))
                {
                    if (referenceIdentifiers.Count == 1)
                    {
                        // remove whole element
                        RemoveReference(referenceSourceElement, referenceTargetElement);
                    }
                    else
                    {
                        // remove only single identifier
                        referenceIdentifiers.Remove(referenceIdentifier.ToString());
                        SetReferenceIdentifiers(referenceSourceElement, referenceIdentifiers);
                    }
                }
            }
        }
    }
}