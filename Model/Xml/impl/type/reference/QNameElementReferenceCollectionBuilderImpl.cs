using System;
using ESS.FW.Bpm.Model.Xml.impl.type.child;
using ESS.FW.Bpm.Model.Xml.instance;



namespace ESS.FW.Bpm.Model.Xml.impl.type.reference
{
    /// <summary>
    /// 
    /// </summary>
    public class QNameElementReferenceCollectionBuilderImpl<TArget, TSource> : ElementReferenceCollectionBuilderImpl<TArget, TSource>
        where TArget : IModelElementInstance where TSource : IModelElementInstance
    {

        public QNameElementReferenceCollectionBuilderImpl(ChildElementCollectionImpl<TSource> collection) 
            : base(collection)
        {
            this.ElementReferenceCollectionImpl = new QNameElementReferenceCollectionImpl<TArget, TSource>(collection);
        }

    }

}