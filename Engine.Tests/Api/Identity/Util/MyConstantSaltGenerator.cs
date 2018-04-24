
using ESS.FW.Bpm.Engine.Impl.Digest;

namespace Engine.Tests.Api.Identity.Util
{
    
	public class MyConstantSaltGenerator : Default16ByteSaltGenerator
	{

	  protected internal string salt;

	  public MyConstantSaltGenerator(string salt)
	  {
		this.salt = salt;
	  }

	  public string generateSalt()
	  {
		return salt;
	  }
	}

}