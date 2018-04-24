using System;

namespace Engine.Tests.Dmn.BusinessRuleTask
{

	[Serializable]
	public class TestPojo
	{
	    public TestPojo(string foo, double? bar)
	  {
		this.Foo = foo;
		this.Bar = bar;
	  }

	  public virtual string Foo { get; }

	    public virtual double? Bar { get; }

	    public override string ToString()
	  {
		return "TestPojo{" + "foo='" + Foo + '\'' + ", bar=" + Bar + '}';
	  }

	}

}