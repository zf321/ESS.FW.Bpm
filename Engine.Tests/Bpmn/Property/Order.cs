using System;

namespace Engine.Tests.Bpmn.Property
{

	/// <summary>
	/// Test object used in various unit test.
	/// 
	/// 
	/// </summary>
	[Serializable]
	public class Order
	{

		private const long serialVersionUID = 1L;

		private string address;

		public Order()
		{

		}

		public Order(string address)
		{
		  this.Address = address;
		}

		public virtual string Address
		{
			get
			{
			  return address;
			}
			set
			{
			  this.Address = value;
			}
		}


	}
}