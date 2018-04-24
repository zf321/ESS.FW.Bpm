using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Identity.Util;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Identity;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Digest;
using NUnit.Framework;

namespace Engine.Tests.Api.Identity
{


    public class PasswordHashingTest
    {

        protected internal static ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        protected internal static ProcessEngineTestRule testRule = new ProcessEngineTestRule(engineRule);

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.ExpectedException thrown = org.junit.Rules.ExpectedException.None();
        //public ExpectedException thrown = ExpectedException.None();

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testRule);
       // public RuleChain //ruleChain = RuleChain.outerRule(engineRule).around(testRule);

        protected internal const string PASSWORD = "password";
        protected internal const string USER_NAME = "johndoe";
        protected internal const string ALGORITHM_NAME = "awesome";

        protected internal IIdentityService identityService;
        protected internal IRuntimeService runtimeService;
        protected internal ProcessEngineConfigurationImpl processEngineConfiguration;

        protected internal IPasswordEncryptor camundaDefaultEncryptor;
        protected internal IList<IPasswordEncryptor> camundaDefaultPasswordChecker;
        protected internal ISaltGenerator camundaDefaultSaltGenerator;


        [SetUp]
        public virtual void initialize()
        {
            runtimeService = engineRule.RuntimeService;
            identityService = engineRule.IdentityService;
            processEngineConfiguration = engineRule.ProcessEngineConfiguration;
            camundaDefaultEncryptor = processEngineConfiguration.PasswordEncryptor;
            camundaDefaultPasswordChecker = processEngineConfiguration.CustomPasswordChecker;
            camundaDefaultSaltGenerator = processEngineConfiguration.SaltGenerator;
        }

        [TearDown]
        public virtual void cleanUp()
        {
            removeAllUser();
            resetEngineConfiguration();
        }

        protected internal virtual void removeAllUser()
        {
            IList<IUser> list = identityService.CreateUserQuery()         
                .ToList();
            foreach (IUser user in list)
            {
                identityService.DeleteUser(user.Id);
            }
        }

        protected internal virtual void resetEngineConfiguration()
        {
            setEncryptors(camundaDefaultEncryptor, camundaDefaultPasswordChecker);
            processEngineConfiguration.SaltGenerator = camundaDefaultSaltGenerator;
        }

        [Test]
        public virtual void saltHashingOnHashedPasswordWithoutSaltThrowsNoError()
        {
            // given
            processEngineConfiguration.SaltGenerator = new MyConstantSaltGenerator(null);
            IUser user = identityService.NewUser(USER_NAME);
            user.Password = PASSWORD;

            // when
            identityService.SaveUser(user);

            // then
            Assert.That(identityService.CheckPassword(USER_NAME, PASSWORD), Is.EqualTo(true));
        }


        [Test]
        public virtual void enteringTheSamePasswordShouldProduceTwoDifferentEncryptedPassword()
        {
            // given
            IUser user1 = identityService.NewUser(USER_NAME);
            user1.Password = PASSWORD;
            identityService.SaveUser(user1);

            // when
            IUser user2 = identityService.NewUser("kermit");
            user2.Password = PASSWORD;
            identityService.SaveUser(user2);

            // then
            Assert.That(user1.Password!=(user2.Password));
        }


        [Test]
        public virtual void ensurePasswordIsCorrectlyHashedWithSHA1()
        {
            // given
            DefaultEncryptor = new ShaHashDigest();
            processEngineConfiguration.SaltGenerator = new MyConstantSaltGenerator("12345678910");
            IUser user = identityService.NewUser(USER_NAME);
            user.Password = PASSWORD;
            identityService.SaveUser(user);

            // when
            user = identityService.CreateUserQuery(c=>c.Id == USER_NAME).First();

            // then
            // obtain the expected value on the command line like so: echo -n password12345678910 | openssl dgst -binary -sha1 | openssl base64
            Assert.That(user.Password, Is.EqualTo("{SHA}n3fE9/7XOmgD3BkeJlC+JLyb/Qg="));
        }


        [Test]
        public virtual void ensurePasswordIsCorrectlyHashedWithSHA512()
        {
            // given
            processEngineConfiguration.SaltGenerator = new MyConstantSaltGenerator("12345678910");
            IUser user = identityService.NewUser(USER_NAME);
            user.Password = PASSWORD;
            identityService.SaveUser(user);

            // when
            user = identityService.CreateUserQuery(c=>c.Id == USER_NAME).First();

            // then
            // obtain the expected value on the command line like so: echo -n password12345678910 | openssl dgst -binary -sha512 | openssl base64
            Assert.That(user.Password, Is.EqualTo("{SHA-512}sM1U4nCzoDbdUugvJ7dJ6rLc7t1ZPPsnAbUpTqi5nXCYp7PTZCHExuzjoxLLYoUK" + "Gd637jKqT8d9tpsZs3K5+g=="));
        }


        [Test]
        public virtual void twoEncryptorsWithSamePrefixThrowError()
        {

            // given two algorithms with the same prefix
            IList<IPasswordEncryptor> additionalEncryptorsForPasswordChecking = new List<IPasswordEncryptor>();
            additionalEncryptorsForPasswordChecking.Add(new ShaHashDigest());
            IPasswordEncryptor defaultEncryptor = new ShaHashDigest();

            // then
           // thrown.Expect(typeof(PasswordEncryptionException));
            //thrown.ExpectMessage("Hash algorithm with the name 'SHA' was already added");

            // when
            setEncryptors(defaultEncryptor, additionalEncryptorsForPasswordChecking);
        }


        [Test]
        public virtual void prefixThatCannotBeResolvedThrowsError()
        {
            // given
            DefaultEncryptor = new MyCustomPasswordEncryptorCreatingPrefixThatCannotBeResolved();
            IUser user = identityService.NewUser(USER_NAME);
            user.Password = PASSWORD;
            identityService.SaveUser(user);
            user = identityService.CreateUserQuery(c=>c.Id == USER_NAME).First();

            // then
            //thrown.Expect(typeof(PasswordEncryptionException));
            //thrown.ExpectMessage("Could not resolve hash algorithm name of a hashed password");

            // when
            identityService.CheckPassword(user.Id, PASSWORD);
        }


        [Test]
        public virtual void plugInCustomPasswordEncryptor()
        {
            // given
            setEncryptors(new MyCustomPasswordEncryptor(PASSWORD, ALGORITHM_NAME), System.Linq.Enumerable.Empty<IPasswordEncryptor>().ToList());
            IUser user = identityService.NewUser(USER_NAME);
            user.Password = PASSWORD;
            identityService.SaveUser(user);

            // when
            user = identityService.CreateUserQuery(c=>c.Id == USER_NAME).First();

            // then
            Assert.That(user.Password, Is.EqualTo("{" + ALGORITHM_NAME + "}xxx"));
        }


        [Test]
        public virtual void useSeveralCustomEncryptors()
        {

            // given three users with different hashed passwords
            processEngineConfiguration.SaltGenerator = new MyConstantSaltGenerator("12345678910");

            string userName1 = "Kermit";
            createUserWithEncryptor(userName1, new MyCustomPasswordEncryptor(PASSWORD, ALGORITHM_NAME));

            string userName2 = "Fozzie";
            string anotherAlgorithmName = "marvelousAlgorithm";
            createUserWithEncryptor(userName2, new MyCustomPasswordEncryptor(PASSWORD, anotherAlgorithmName));

            string userName3 = "Gonzo";
            createUserWithEncryptor(userName3, new ShaHashDigest());

            IList<IPasswordEncryptor> additionalEncryptorsForPasswordChecking = new List<IPasswordEncryptor>();
            additionalEncryptorsForPasswordChecking.Add(new MyCustomPasswordEncryptor(PASSWORD, ALGORITHM_NAME));
            additionalEncryptorsForPasswordChecking.Add(new MyCustomPasswordEncryptor(PASSWORD, anotherAlgorithmName));
            IPasswordEncryptor defaultEncryptor = new ShaHashDigest();
            setEncryptors(defaultEncryptor, additionalEncryptorsForPasswordChecking);

            // when
            IUser user1 = identityService.CreateUserQuery(c=>c.Id == userName1).First();
            IUser user2 = identityService.CreateUserQuery(c=>c.Id == userName2).First();
            IUser user3 = identityService.CreateUserQuery(c=>c.Id == userName3).First();

            // then
            Assert.That(user1.Password, Is.EqualTo("{" + ALGORITHM_NAME + "}xxx"));
            Assert.That(user2.Password, Is.EqualTo("{" + anotherAlgorithmName + "}xxx"));
            Assert.That(user3.Password, Is.EqualTo("{SHA}n3fE9/7XOmgD3BkeJlC+JLyb/Qg="));
        }

        protected internal virtual void createUserWithEncryptor(string userName, IPasswordEncryptor encryptor)
        {
            setEncryptors(encryptor, System.Linq.Enumerable.Empty<IPasswordEncryptor>().ToList());
            IUser user = identityService.NewUser(userName);
            user.Password = PASSWORD;
            identityService.SaveUser(user);
        }

        protected internal virtual IPasswordEncryptor DefaultEncryptor
        {
            set
            {
                setEncryptors(value, System.Linq.Enumerable.Empty<IPasswordEncryptor>().ToList());
            }
        }

        protected internal virtual void setEncryptors(IPasswordEncryptor defaultEncryptor, IList<IPasswordEncryptor> additionalEncryptorsForPasswordChecking)
        {
            processEngineConfiguration.PasswordManager = new PasswordManager(defaultEncryptor, additionalEncryptorsForPasswordChecking);
        }

    }

}