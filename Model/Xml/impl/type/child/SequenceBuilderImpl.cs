using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type.child;


namespace ESS.FW.Bpm.Model.Xml.impl.type.child
{
    public class SequenceBuilderImpl : ISequenceBuilder, IModelBuildOperation
    {

        private readonly ModelElementTypeImpl _elementType;

        private readonly IList<IModelBuildOperation> _modelBuildOperations = new List<IModelBuildOperation>();

        public SequenceBuilderImpl(ModelElementTypeImpl modelType)
        {
            this._elementType = modelType;
        }
        
        public virtual IChildElementBuilder Element<T>() where T : IModelElementInstance
        {
            ChildElementBuilderImpl<T> builder = new ChildElementBuilderImpl<T>(_elementType);
            _modelBuildOperations.Add(builder);
            return builder;
        }

        public virtual IChildElementCollectionBuilder ElementCollection<T>() where T : IModelElementInstance
        {
            ChildElementCollectionBuilderImpl<T> builder = new ChildElementCollectionBuilderImpl<T>( _elementType);
            _modelBuildOperations.Add(builder);
            return builder;
        }

        public virtual void PerformModelBuild(IModel model)
        {
            foreach (IModelBuildOperation operation in _modelBuildOperations)
            {
                operation.PerformModelBuild(model);
            }
        }

    }
}