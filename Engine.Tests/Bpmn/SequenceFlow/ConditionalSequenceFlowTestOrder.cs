using System;

namespace Engine.Tests.Bpmn.SequenceFlow
{

    [Serializable]
    public class ConditionalSequenceFlowTestOrder
    {

        private const long serialVersionUID = 1L;


        public ConditionalSequenceFlowTestOrder(int price)
        {
            this.Price = price;
        }

        public virtual int Price { get; set; }


        public virtual bool IsPremiumOrder()
        {
            return Price >= 250;
        }

    }

}