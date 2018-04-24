using System.Collections.Generic;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type.child;



namespace ESS.FW.Bpm.Model.Xml.type.reference
{

    public interface IElementReferenceCollection : IReference
    {

        IChildElementCollection GetReferenceSourceCollection() ;

        ICollection<TTarget> GetReferenceTargetElements<TTarget>(ModelElementInstanceImpl referenceSourceElement) where TTarget:IModelElementInstance;
    }

}