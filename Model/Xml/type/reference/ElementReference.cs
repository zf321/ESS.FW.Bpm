using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.instance;

namespace ESS.FW.Bpm.Model.Xml.type.reference
{
    public interface IElementReference : IElementReferenceCollection
    {

        IModelElementInstance GetReferenceSource(IModelElementInstance referenceSourceParent);

        IModelElementInstance GetReferenceTargetElement(ModelElementInstanceImpl referenceSourceParentElement);

        void SetReferenceTargetElement<TTarget>(ModelElementInstanceImpl referenceSourceParentElement, TTarget referenceTargetElement) where TTarget : IModelElementInstance;

        void ClearReferenceTargetElement(ModelElementInstanceImpl referenceSourceParentElement);

    }

}