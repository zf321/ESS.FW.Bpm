

using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Xml.type.child
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    public interface IChildElementBuilder : IChildElementCollectionBuilder 
    {

        new IChildElementBuilder Immutable();

        new IChildElementBuilder Required();

        new IChildElementBuilder MinOccurs(int i);

        new IChildElementBuilder MaxOccurs(int i);

        new IChildElement Build();
        
        IElementReferenceBuilder QNameElementReference<TTarget>() where TTarget : IModelElementInstance ;

        IElementReferenceBuilder IdElementReference<TTarget>() where TTarget : IModelElementInstance ;

        IElementReferenceBuilder UriElementReference<TTarget>() where TTarget : IModelElementInstance;
        
    }
}