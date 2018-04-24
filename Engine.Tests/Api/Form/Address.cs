using System;

namespace Engine.Tests.Api.Form
{
    /// <summary>
    /// </summary>
    [Serializable]
    public class Address
    {
        private const long serialVersionUID = 1L;

        private string street;

        public virtual string Street
        {
            get { return street; }
            set { street = value; }
        }
    }
}