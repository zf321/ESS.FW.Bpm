using System;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.transform;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.transform
{
    public class DefaultTransformFactory : IDmnTransformFactory
    {
        public virtual IDmnTransform createTransform(IDmnTransformer transformer)
        {
            //throw new NotImplementedException();
            return new DefaultDmnTransform(transformer);
        }
    }
}