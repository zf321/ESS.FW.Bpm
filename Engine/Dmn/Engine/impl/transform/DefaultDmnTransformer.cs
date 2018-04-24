using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.hitpolicy;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.hitpolicy;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.transform;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.type;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.type;
using ESS.FW.Bpm.Model.Dmn.instance;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.transform
{
    public class DefaultDmnTransformer : IDmnTransformer
    {
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal IDmnDataTypeTransformerRegistry DataTypeTransformerRegistryRenamed =
            new DefaultDataTypeTransformerRegistry();

        //JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal IDmnElementTransformHandlerRegistry ElementTransformHandlerRegistryRenamed =
            new DefaultElementTransformHandlerRegistry<IDmnModelElementInstance, string>();

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal IDmnHitPolicyHandlerRegistry HitPolicyHandlerRegistryRenamed =
            new DefaultHitPolicyHandlerRegistry();

        protected internal IDmnTransformFactory transformFactory = new DefaultTransformFactory();
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal IList<IDmnTransformListener> TransformListenersRenamed = new List<IDmnTransformListener>();

        public virtual IDmnTransformFactory TransformFactory
        {
            get { return transformFactory; }
        }

        public virtual IList<IDmnTransformListener> TransformListeners
        {
            get { return TransformListenersRenamed; }
            set { TransformListenersRenamed = value; }
        }


        public virtual IDmnTransformer transformListeners(IList<IDmnTransformListener> transformListeners)
        {
            TransformListeners = transformListeners;
            return this;
        }

        public virtual IDmnElementTransformHandlerRegistry ElementTransformHandlerRegistry
        {
            get { return ElementTransformHandlerRegistryRenamed; }
            set { ElementTransformHandlerRegistryRenamed = value; }
        }


        public virtual IDmnTransformer elementTransformHandlerRegistry(
            IDmnElementTransformHandlerRegistry elementTransformHandlerRegistry)
        {
            ElementTransformHandlerRegistry = elementTransformHandlerRegistry;
            return this;
        }

        public virtual IDmnDataTypeTransformerRegistry DataTypeTransformerRegistry
        {
            get { return DataTypeTransformerRegistryRenamed; }
            set { DataTypeTransformerRegistryRenamed = value; }
        }


        public virtual IDmnTransformer dataTypeTransformerRegistry(
            IDmnDataTypeTransformerRegistry dataTypeTransformerRegistry)
        {
            DataTypeTransformerRegistry = dataTypeTransformerRegistry;
            return this;
        }

        public virtual IDmnHitPolicyHandlerRegistry HitPolicyHandlerRegistry
        {
            get { return HitPolicyHandlerRegistryRenamed; }
            set { HitPolicyHandlerRegistryRenamed = value; }
        }


        public virtual IDmnTransformer hitPolicyHandlerRegistry(IDmnHitPolicyHandlerRegistry hitPolicyHandlerRegistry)
        {
            HitPolicyHandlerRegistry = hitPolicyHandlerRegistry;
            return this;
        }

        public virtual IDmnTransform createTransform()
        {
            return transformFactory.createTransform(this);
        }
    }
}