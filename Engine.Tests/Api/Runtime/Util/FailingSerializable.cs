using System;
using System.IO;
using ESS.FW.Bpm.Engine.exception;

namespace Engine.Tests.Api.Runtime.Util
{
    /// <summary>
    ///     Objects of this class cannot be de-serialized.
    /// </summary>
    [Serializable]
    public class FailingSerializable
    {
        private const long serialVersionUID = 1L;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream in) throws java.io.IOException, ClassNotFoundException
        private void readObject(Stream @in)
        {
            throw new NotFoundException("Cannot load class FailingSerializable");
        }
    }
}