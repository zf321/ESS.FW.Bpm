using System.Collections.Generic;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.validation;


namespace ESS.FW.Bpm.Model.Xml
{
    
    public interface IModelInstance
    {
        
        IDomDocument Document { get; }
        
        IModelElementInstance DocumentElement { get; set; }

                T NewInstance<T>(System.Type type) where T : IModelElementInstance;
        
        T NewInstance<T>(IModelElementType type) where T : IModelElementInstance;
        
        IModel Model { get; }
        
        IModelElementInstance GetModelElementById(string id);
        
        IEnumerable<IModelElementInstance> GetModelElementsByType(IModelElementType referencingType);
        
        IEnumerable<T> GetModelElementsByType<T>(System.Type referencingClass) where T : IModelElementInstance;
        
        IModelInstance Clone();
        
        IValidationResults Validate(ICollection<IModelElementValidator<IModelElementInstance>> validators);
        
    }
}