using System.Collections.Generic;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;


namespace ESS.FW.Bpm.Model.Xml.impl.type.child
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    public class ChildElementCollectionBuilderImpl<TSource> : IChildElementCollectionBuilder, IModelBuildOperation 
        where TSource:IModelElementInstance
    {
        protected internal readonly ModelElementTypeImpl parentElementType;
        private readonly ChildElementCollectionImpl<TSource> _collection;

        private IElementReferenceCollectionBuilder _referenceBuilder;

        private readonly IList<IModelBuildOperation> _modelBuildOperations = new List<IModelBuildOperation>();

        public ChildElementCollectionBuilderImpl(IModelElementType parentElementType)
        {
            this.parentElementType = (ModelElementTypeImpl)parentElementType;
            this._collection = CreateCollectionInstance();

        }

        protected internal virtual ChildElementCollectionImpl<TSource> CreateCollectionInstance()
        {
            return new ChildElementCollectionImpl<TSource>(parentElementType);
        }

        public virtual IChildElementCollectionBuilder Immutable()
        {
            _collection.SetImmutable();
            return this;
        }

        public virtual IChildElementCollectionBuilder Required()
        {
            _collection.MinOccurs = 1;
            return this;
        }

        public virtual IChildElementCollectionBuilder MaxOccurs(int i)
        {
            _collection.MaxOccurs = i;
            return this;
        }

        public virtual IChildElementCollectionBuilder MinOccurs(int i)
        {
            _collection.MinOccurs = i;
            return this;
        }

        public virtual IChildElementCollection Build()
        {
            return _collection;
        }
        
        public virtual IElementReferenceCollectionBuilder QNameElementReferenceCollection<TTarget>() 
            where TTarget :IModelElementInstance
        {
            ChildElementCollectionImpl<TSource> collection = (ChildElementCollectionImpl<TSource>)Build();
            QNameElementReferenceCollectionBuilderImpl<TTarget, TSource> builder = new QNameElementReferenceCollectionBuilderImpl<TTarget, TSource>(/*typeof(TSource), referenceTargetType.GetType(),*/ collection);
            SetReferenceBuilder(builder);
            return builder;
        }
        
        public virtual IElementReferenceCollectionBuilder IdElementReferenceCollection<TTarget>() 
            where TTarget:IModelElementInstance
        {
            ChildElementCollectionImpl<TSource> collection = (ChildElementCollectionImpl<TSource>)Build();
            IElementReferenceCollectionBuilder builder = new ElementReferenceCollectionBuilderImpl<TTarget, TSource>(collection);
            SetReferenceBuilder(builder);
            return builder;
        }

        public virtual IElementReferenceCollectionBuilder IdsElementReferenceCollection<TTarget>() 
            where TTarget : IModelElementInstance
        {
            ChildElementCollectionImpl<TSource> collection = (ChildElementCollectionImpl<TSource>)Build();
            IElementReferenceCollectionBuilder builder = new IdsElementReferenceCollectionBuilderImpl<TTarget, TSource>(collection);
            SetReferenceBuilder(builder);
            return builder;
        }        

        public virtual IElementReferenceCollectionBuilder UriElementReferenceCollection<TTarget>() 
            where TTarget : IModelElementInstance
        {
            ChildElementCollectionImpl<TSource> collection = (ChildElementCollectionImpl<TSource>)Build();
            IElementReferenceCollectionBuilder builder = new UriElementReferenceCollectionBuilderImpl<TTarget, TSource>(collection);
            SetReferenceBuilder(builder);
            return builder;
        }
        
        protected void SetReferenceBuilder(IElementReferenceCollectionBuilder referenceBuilder)
        {
            if (this._referenceBuilder != null)
            {
                throw new ModelException("An collection cannot have more than one reference");
            }
            this._referenceBuilder = referenceBuilder;
            this._modelBuildOperations.Add(referenceBuilder);
        }
        
        public virtual void PerformModelBuild(IModel model)
        {
            IModelElementType elementType = model.GetType(typeof(TSource));
            if (elementType == null)
            {
                throw new ModelException(parentElementType + " declares undefined child element of type " + typeof(TSource) + ".");
            }
            parentElementType.RegisterChildElementType(elementType);
            parentElementType.RegisterChildElementCollection(_collection);
            foreach (IModelBuildOperation modelBuildOperation in _modelBuildOperations)
            {
                modelBuildOperation.PerformModelBuild(model);
            }
        }
    }
}