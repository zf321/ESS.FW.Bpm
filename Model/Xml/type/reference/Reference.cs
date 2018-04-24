using System.Collections.Generic;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type.attribute;


namespace ESS.FW.Bpm.Model.Xml.type.reference
{
    public interface IReference
    {        
        string GetReferenceIdentifier(IModelElementInstance referenceSourceElement);                   

        T GetReferenceTargetElement<T>(IModelElementInstance modelElement) where T:IModelElementInstance;
       
        void SetReferenceTargetElement<T>(IModelElementInstance referenceSourceElement, T referenceTargetElement) where T:IModelElementInstance;

        IAttribute ReferenceTargetAttribute { get;}
        
        IEnumerable<IModelElementInstance> FindReferenceSourceElements(IModelElementInstance referenceTargetElement);
        
        IModelElementType ReferenceSourceElementType { get; }
    }

}