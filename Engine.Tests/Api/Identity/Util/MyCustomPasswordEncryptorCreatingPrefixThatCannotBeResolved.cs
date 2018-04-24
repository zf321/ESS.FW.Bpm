
using ESS.FW.Bpm.Engine.Impl.Digest;

namespace Engine.Tests.Api.Identity.Util
{

	public class MyCustomPasswordEncryptorCreatingPrefixThatCannotBeResolved : IPasswordEncryptor
	{

	  protected internal int counter = 0;

	  public string Encrypt(string password)
	  {
		return "xxx";
	  }

	  public bool Check(string password, string encrypted)
	  {
		return password.Equals("xxx");
	  }

	  public string HashAlgorithmName()
	  {
		counter++;
		return "Bla" + counter;
	  }
	}

}