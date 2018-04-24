using System.Linq;
using System.Text;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Entity
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class JobEntityTest : PluggableProcessEngineTestCase
    {
        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly JobEntityTest _outerInstance;

            private readonly JobEntity _jobEntity;

            public CommandAnonymousInnerClass(JobEntityTest outerInstance, JobEntity jobEntity)
            {
                _outerInstance = outerInstance;
                _jobEntity = jobEntity;
            }


            public object Execute(CommandContext commandContext)
            {
                commandContext.JobManager.Add(_jobEntity);
                return null;
            }
        }
        
        protected internal virtual void DeleteJob(JobEntity jobEntity)
        {
            processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass2(this, jobEntity));
        }

        private class CommandAnonymousInnerClass2 : ICommand<object>
        {
            private readonly JobEntityTest _outerInstance;

            private readonly JobEntity _jobEntity;

            public CommandAnonymousInnerClass2(JobEntityTest outerInstance, JobEntity jobEntity)
            {
                _outerInstance = outerInstance;
                _jobEntity = jobEntity;
            }


            public object Execute(CommandContext commandContext)
            {
                commandContext.JobManager.Delete(_jobEntity);
                return null;
            }
        }

        protected internal virtual string RepeatCharacter(string encodedCharacter, int numCharacters)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < numCharacters; i++)
                sb.Append(encodedCharacter);

            return sb.ToString();
        }

        [Test]
        protected internal virtual void InsertJob(JobEntity jobEntity)
        {
            processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this, jobEntity));
        }

        /// <summary>
        ///     Note: This does not test a message with 4-byte Unicode supplementary
        ///     characters for two reasons:
        ///     - MySQL 5.1 does not support 4-byte supplementary characters (support from 5.5.3 onwards)
        ///     - <seealso cref="string#length()" /> counts these characters twice (since they are represented by two
        ///     chars), so essentially the cutoff would be half the actual cutoff for such a string
        /// </summary>
        [Test]
        public virtual void TestInsertJobWithExceptionMessage()
        {
            var fittingThreeByteMessage = RepeatCharacter("\u9faf", JobEntity.MaxExceptionMessageLength);

            JobEntity threeByteJobEntity = new MessageEntity();
            threeByteJobEntity.ExceptionMessage = fittingThreeByteMessage;

            // should not fail
            InsertJob(threeByteJobEntity);

            DeleteJob(threeByteJobEntity);
        }

        [Test]
        public virtual void TestJobExceptionMessageCutoff()
        {
            JobEntity threeByteJobEntity = new MessageEntity();

            var message = RepeatCharacter("a", JobEntity.MaxExceptionMessageLength * 2);
            threeByteJobEntity.ExceptionMessage = message;
            Assert.AreEqual(JobEntity.MaxExceptionMessageLength, threeByteJobEntity.ExceptionMessage.Length);
        }


        [Test]
        [Deployment]
        public virtual void TestLongProcessDefinitionKey()
        {
            var key = "myrealrealrealrealrealrealrealrealrealrealreallongprocessdefinitionkeyawesome";
            var processInstanceId = runtimeService.StartProcessInstanceByKey(key).Id;

            var job = managementService.CreateJobQuery(c=>c.ProcessInstanceId == processInstanceId).First();

            Assert.AreEqual(key, job.ProcessDefinitionKey);
        }
    }
}