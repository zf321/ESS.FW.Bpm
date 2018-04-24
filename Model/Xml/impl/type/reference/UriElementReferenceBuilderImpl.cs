using System;
using ESS.FW.Bpm.Model.Xml.impl.type.child;
using ESS.FW.Bpm.Model.Xml.instance;



namespace ESS.FW.Bpm.Model.Xml.impl.type.reference
{
    public class UriElementReferenceBuilderImpl<TTarget, TSource> : ElementReferenceBuilderImpl<TTarget, TSource> 
        where TTarget : IModelElementInstance where TSource : IModelElementInstance
    {

        public UriElementReferenceBuilderImpl(ChildElementImpl<TSource> child) : base(child)
        {
            this.ElementReferenceCollectionImpl = new UriElementReferenceImpl<TTarget, TSource>(child);
        }
    }
}