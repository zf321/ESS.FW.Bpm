using System;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.transform
{
    /// <summary>
    ///     Exception throw for errors during the transformation of a decision.
    /// </summary>
    public class DmnTransformException : DmnEngineException
    {
        public DmnTransformException(string message, System.Exception cause) : base(message, cause)
        {
        }

        public DmnTransformException(string message) : base(message)
        {
        }
    }
}