
using ESS.FW.Bpm.Engine.Impl.Digest;

namespace Engine.Tests.Api.Identity.Util
{
	public class MyCustomPasswordEncryptor : IPasswordEncryptor
	{

	  protected internal string password;
	  protected internal string algorithmName;

	  public MyCustomPasswordEncryptor(string password, string algorithmName)
	  {
		this.password = password;
		this.algorithmName = algorithmName;
	  }

	  public string Encrypt(string password)
	  {
		return "xxx";
	  }

	  public bool Check(string password, string encrypted)
	  {
		return password.Equals(this.password);
	  }

	  public string HashAlgorithmName()
	  {
		return algorithmName;
	  }
	}

}