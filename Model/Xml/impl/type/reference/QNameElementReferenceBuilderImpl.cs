using System;
using ESS.FW.Bpm.Model.Xml.impl.type.child;
using ESS.FW.Bpm.Model.Xml.instance;



namespace ESS.FW.Bpm.Model.Xml.impl.type.reference
{
    /// <summary>
    /// 
    /// </summary>
    public class QNameElementReferenceBuilderImpl<TTarget, TSource> : ElementReferenceBuilderImpl<TTarget, TSource> 
        where TTarget : IModelElementInstance where TSource : IModelElementInstance
    {

        public QNameElementReferenceBuilderImpl(ChildElementImpl<TSource> child) 
            : base(child)
        {
            this.ElementReferenceCollectionImpl = new QNameElementReferenceImpl<TTarget, TSource>(child);
        }
    }
}