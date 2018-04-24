using System;
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
    public class ChildElementBuilderImpl<TSource> : ChildElementCollectionBuilderImpl<TSource>, IChildElementBuilder 
        where TSource : IModelElementInstance
    {

        public ChildElementBuilderImpl(IModelElementType parentElementType) 
            : base(parentElementType)
        {
        }

        protected internal override ChildElementCollectionImpl<TSource> CreateCollectionInstance()
        {
            return new ChildElementImpl<TSource>(parentElementType);
        }

        public new IChildElementBuilder Immutable()
        {
            base.Immutable();
            return this;
        }

        public new IChildElementBuilder Required()
        {
            base.Required();
            return this;
        }

        public new IChildElementBuilder MinOccurs(int i)
        {
            base.MinOccurs(i);
            return this;
        }

        public new IChildElementBuilder MaxOccurs(int i)
        {
            base.MaxOccurs(i);
            return this;
        }

        public new IChildElement Build() 
        {
            return (IChildElement)base.Build();
        }        

        public virtual IElementReferenceBuilder QNameElementReference<TTarget>() 
            where TTarget : IModelElementInstance
        {
            ChildElementImpl<TSource> child = (ChildElementImpl<TSource>)Build();
            QNameElementReferenceBuilderImpl<TTarget, TSource> builder = new QNameElementReferenceBuilderImpl<TTarget, TSource>(child);
            SetReferenceBuilder(builder);
            return builder;
        }
        
        public virtual IElementReferenceBuilder IdElementReference<TTarget>() 
            where TTarget:IModelElementInstance
        {
            ChildElementImpl<TSource> child = (ChildElementImpl<TSource>)Build();
            ElementReferenceBuilderImpl<TTarget, TSource> builder = new ElementReferenceBuilderImpl<TTarget, TSource>(child);
            SetReferenceBuilder(builder);
            return builder;
        }
        
        public virtual IElementReferenceBuilder UriElementReference<TTarget>() where TTarget : IModelElementInstance
        {
            ChildElementImpl<TSource> child = (ChildElementImpl<TSource>)Build();
            ElementReferenceBuilderImpl<TTarget, TSource> builder = new UriElementReferenceBuilderImpl<TTarget, TSource>(child);
            SetReferenceBuilder(builder);
            return builder;
        }
        
    }
}