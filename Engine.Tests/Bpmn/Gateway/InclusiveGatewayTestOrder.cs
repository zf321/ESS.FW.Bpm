using System;

namespace Engine.Tests.Bpmn.Gateway
{
    
    [Serializable]
    public class InclusiveGatewayTestOrder
    {

        private const long serialVersionUID = 1L;

        private int price;

        public InclusiveGatewayTestOrder(int price)
        {
            this.price = price;
        }

        public virtual int Price
        {
            set
            {
                this.price = value;
            }
            get
            {
                return price;
            }
        }


        public virtual bool Basic
        {
            get
            {
                return price <= 100;
            }
        }

        public virtual bool Standard
        {
            get
            {
                return price <= 150;
            }
        }

        public virtual bool Gold
        {
            get
            {
                return price <= 200;
            }
        }

    }

}