

using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Xml.type.child
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    public interface IChildElementCollectionBuilder
    {

        IChildElementCollectionBuilder Immutable();

        IChildElementCollectionBuilder Required();

        IChildElementCollectionBuilder MinOccurs(int i);

        IChildElementCollectionBuilder MaxOccurs(int i);

        IChildElementCollection Build();

        IElementReferenceCollectionBuilder QNameElementReferenceCollection<TTarget>() where TTarget : IModelElementInstance;

        IElementReferenceCollectionBuilder IdElementReferenceCollection<TTarget>() where TTarget : IModelElementInstance;

        IElementReferenceCollectionBuilder IdsElementReferenceCollection<TTarget>() where TTarget : IModelElementInstance;

        IElementReferenceCollectionBuilder UriElementReferenceCollection<TTarget>() where TTarget : IModelElementInstance;

    }
}