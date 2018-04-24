using ESS.FW.Bpm.Engine.Impl.Digest;
using NUnit.Framework;

namespace Engine.Tests.Api.Identity
{

	public class DatabasePrefixHandlerTest
	{

	  internal DatabasePrefixHandler prefixHandler;

        [SetUp]
	  public virtual void init()
	  {
		prefixHandler = new DatabasePrefixHandler();
	  }

        [Test]
	  public virtual void testGeneratePrefix()
	  {

		// given
		string algorithmName = "test";

		// when
		string prefix = prefixHandler.GeneratePrefix(algorithmName);

		// then
		Assert.That(prefix, Is.EqualTo("{test}"));
	  }


        [Test]
        public virtual void testRetrieveAlgorithmName()
	  {

		// given
		string encryptedPasswordWithPrefix = "{SHA}n3fE9/7XOmgD3BkeJlC+JLyb/Qg=";

		// when
		string algorithmName = prefixHandler.RetrieveAlgorithmName(encryptedPasswordWithPrefix);

		// then
		Assert.That(algorithmName, Is.EqualTo("SHA"));
	  }


        [Test]
        public virtual void retrieveAlgorithmForInvalidInput()
	  {

		// given
		string encryptedPasswordWithPrefix = "xxx{SHA}n3fE9/7XOmgD3BkeJlC+JLyb/Qg=";

		// when
		string algorithmName = prefixHandler.RetrieveAlgorithmName(encryptedPasswordWithPrefix);

		// then
		Assert.That(algorithmName, Is.EqualTo(null));
	  }


        [Test]
        public virtual void retrieveAlgorithmWithMissingAlgorithmPrefix()
	  {

		// given
		string encryptedPasswordWithPrefix = "n3fE9/7XOmgD3BkeJlC+JLyb/Qg=";

		// when
		string algorithmName = prefixHandler.RetrieveAlgorithmName(encryptedPasswordWithPrefix);

		// then
		Assert.That(algorithmName, Is.EqualTo(null));
	  }


        [Test]
        public virtual void retrieveAlgorithmWithErroneousAlgorithmPrefix()
	  {

		// given
		string encryptedPasswordWithPrefix = "{SHAn3fE9/7XOmgD3BkeJlC+JLyb/Qg=";

		// when
		string algorithmName = prefixHandler.RetrieveAlgorithmName(encryptedPasswordWithPrefix);

		// then
		Assert.That(algorithmName, Is.EqualTo(null));
	  }


        [Test]
        public virtual void removePrefix()
	  {

		// given
		string encryptedPasswordWithPrefix = "{SHA}n3fE9/7XOmgD3BkeJlC+JLyb/Qg=";

		// when
		string encryptedPassword = prefixHandler.RemovePrefix(encryptedPasswordWithPrefix);

		// then
		Assert.That(encryptedPassword, Is.EqualTo("n3fE9/7XOmgD3BkeJlC+JLyb/Qg="));

	  }


        [Test]
        public virtual void removePrefixForInvalidInput()
	  {

		// given
		string encryptedPasswordWithPrefix = "xxx{SHA}n3fE9/7XOmgD3BkeJlC+JLyb/Qg=";

		// when
		string encryptedPassword = prefixHandler.RemovePrefix(encryptedPasswordWithPrefix);

		// then
		Assert.That(encryptedPassword, Is.EqualTo(null));

	  }


        [Test]
        public virtual void removePrefixWithMissingAlgorithmPrefix()
	  {

		// given
		string encryptedPasswordWithPrefix = "n3fE9/7XOmgD3BkeJlC+JLyb/Qg=";

		// when
		string encryptedPassword = prefixHandler.RemovePrefix(encryptedPasswordWithPrefix);

		// then
		Assert.That(encryptedPassword, Is.EqualTo(null));

	  }


        [Test]
        public virtual void removePrefixWithErroneousAlgorithmPrefix()
	  {

		// given
		string encryptedPasswordWithPrefix = "SHAn3fE9}/7XOmgD3BkeJlC+JLyb/Qg=";

		// when
		string encryptedPassword = prefixHandler.RemovePrefix(encryptedPasswordWithPrefix);

		// then
		Assert.That(encryptedPassword, Is.EqualTo(null));
	  }


	}

}