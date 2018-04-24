using System;
using System.IO;

namespace Engine.Tests.Api.Variables
{


    /// <summary>
    /// @author Daniel Meyer
    /// 
    /// </summary>
    [Serializable]
    public class FailingJavaSerializable : JavaSerializable
    {

        public FailingJavaSerializable(string property) : base(property)
        {
        }

        private const long serialVersionUID = 1L;

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: private void readObject(java.io.ObjectInputStream ois) throws ClassNotFoundException, java.io.IOException
        private void readObject(Stream ois)
        {
            throw new System.Exception("Exception while deserializing object.");
        }

    }

}