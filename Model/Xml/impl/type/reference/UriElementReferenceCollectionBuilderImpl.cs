using System;
using ESS.FW.Bpm.Model.Xml.impl.type.child;
using ESS.FW.Bpm.Model.Xml.instance;



namespace ESS.FW.Bpm.Model.Xml.impl.type.reference
{
    public class UriElementReferenceCollectionBuilderImpl<TTarget, TSource> : ElementReferenceCollectionBuilderImpl<TTarget, TSource> 
        where TTarget : IModelElementInstance where TSource : IModelElementInstance
    {

        public UriElementReferenceCollectionBuilderImpl(ChildElementCollectionImpl<TSource> collection) 
            : base(collection)
        {
            this.ElementReferenceCollectionImpl = new UriElementReferenceCollectionImpl<TTarget, TSource>(collection);
        }
    }
}