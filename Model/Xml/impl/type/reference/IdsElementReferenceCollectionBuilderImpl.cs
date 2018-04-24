using System;
using ESS.FW.Bpm.Model.Xml.impl.type.child;
using ESS.FW.Bpm.Model.Xml.instance;



namespace ESS.FW.Bpm.Model.Xml.impl.type.reference
{
    /// <summary>
    /// 
    /// </summary>
    public class IdsElementReferenceCollectionBuilderImpl<TTarget, TSource> : ElementReferenceCollectionBuilderImpl<TTarget, TSource> 
        where TTarget : IModelElementInstance where TSource : IModelElementInstance
    {
        public IdsElementReferenceCollectionBuilderImpl(ChildElementCollectionImpl<TSource> collection) 
            : base(collection)
        {
            this.ElementReferenceCollectionImpl = new IdsElementReferenceCollectionImpl<TTarget, TSource>(collection);
        }
    }
}