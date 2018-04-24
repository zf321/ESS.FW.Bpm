using System;
using ESS.FW.Bpm.Model.Xml.impl.type.child;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type.reference;



namespace ESS.FW.Bpm.Model.Xml.impl.type.reference
{
    /// <summary>
    /// 
    /// </summary>
    public class ElementReferenceBuilderImpl<TTarget, TSource> : ElementReferenceCollectionBuilderImpl<TTarget, TSource>, IElementReferenceBuilder 
        where TTarget : IModelElementInstance where TSource : IModelElementInstance
    {

        public ElementReferenceBuilderImpl(ChildElementImpl<TSource> child) : base(child)
        {
            this.ElementReferenceCollectionImpl = new ElementReferenceImpl<TTarget, TSource>(child);
        }

        public new IElementReferenceCollection Build()
        {
            return ElementReferenceCollectionImpl;
        }

        IElementReference IElementReferenceBuilder.Build()
        {
            return (IElementReference)ElementReferenceCollectionImpl;
        }
    }
}