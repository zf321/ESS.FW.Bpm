using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class FailedJobListenerWithRetriesTest
    [TestFixture]
    public class FailedJobListenerWithRetriesTest
    {
        #region members

        private ProcessEngineBootstrapRule _bootstrapRule;
        private ProvidedProcessEngineRule _engineRule;
        private ProcessEngineTestRule _testRule;

        //private RuleChain _ruleChain;

        private IRuntimeService _runtimeService;

        private bool _instanceFieldsInitialized = false;

        #endregion

        public FailedJobListenerWithRetriesTest()
        {

            if (!_instanceFieldsInitialized)
            {
                InitializeInstanceFields();
                _instanceFieldsInitialized = true;
            }

        }

        private void InitializeInstanceFields()
        {
            ProcessEngineBootstrapRuleAnonymousInnerClass.OuterInstance = this;
            _bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass ();
            _engineRule = new ProvidedProcessEngineRule(_bootstrapRule);
            _testRule = new ProcessEngineTestRule(_engineRule);
            //_ruleChain = RuleChain.OuterRule(_engineRule).Around(_testRule);
            ////ruleChain = RuleChain.outerRule(EngineRule).around(TestRule);
        }

        //[SetUp]
        //public void Init()
        //{
        //    var rule = _ruleChain.Apply(() =>
        //    {
        //        var m = GetType().GetMethod(TestContext.CurrentContext.Test.Name);
        //        m.Invoke(this, null);
        //    }, Description.CreateTestDescription(ToString(), TestContext.CurrentContext.Test.Name));
        //    _runtimeService = _engineRule.RuntimeService;
        //    rule();
        //}

        [SetUp]
        public void SetUpEngineRule()
        {
            try
            {
                if (_engineRule.ProcessEngine == null)
                    _engineRule.InitializeProcessEngine();

                _engineRule.InitializeServices();

                _engineRule.Starting();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        [SetUp]
        public void SetUpTestRule()
        {
            try
            {
                _testRule.Starting();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        [SetUp]
        public void SetUp()
        {
            _runtimeService = _engineRule.RuntimeService;
        }


        [TearDown]
        public void TearDownEngineRule()
        {
            _bootstrapRule.Finished();
            //_engineRule.Finished();
        }

        [TearDown]
        public void TearDownTestRule()
        {
            //_testRule.Finished();
        }

        [TearDown]
        public void TearDown()
        {
        }


        private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
        {
            //private readonly FailedJobListenerWithRetriesTest _outerInstance;

            //public ProcessEngineBootstrapRuleAnonymousInnerClass(FailedJobListenerWithRetriesTest outerInstance)
            //{
            //    this._outerInstance = outerInstance;
            //}

            public  static FailedJobListenerWithRetriesTest OuterInstance { private get; set; }

            public override ProcessEngineConfiguration ConfigureEngine(ProcessEngineConfigurationImpl configuration)
            {
                configuration.FailedJobCommandFactory = new OleFailedJobCommandFactory(OuterInstance);
                configuration.FailedJobListenerMaxRetries = 5;
                return base.ConfigureEngine(configuration);
            }
        }



        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testRule);
        ////public RuleChain ruleChain;



        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameterized.Parameter(0) public int failedRetriesNumber;
        public int FailedRetriesNumber;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameterized.Parameter(1) public int jobRetries;
        public int JobRetries;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameterized.Parameter(2) public boolean jobLocked;
        public bool JobLocked;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameterized.Parameters public static java.util.Collection<Object[]> scenarios()
        public static ICollection<object[]> Scenarios()
        {
            return new[]
            {
                new object[] {4, 0, false},
                new object[] {5, 1, true}
            };
            //all retries are depleted without success -> the job is still locked
        }

        private static object[] parameters =
        {
            new object[] {4, 0, false}//,
           // new object[] {5, 1, true}
        };

        [Test]
        [Deployment("resources/api/mgmt/IncidentTest.TestShouldCreateOneIncident.bpmn")]
        public virtual void TestFailedJobListenerRetries()
        {
            // Toto: 参数化测试实现

            FailedRetriesNumber = 4;
            JobRetries = 0;
            JobLocked = false;

            //given
            _runtimeService.StartProcessInstanceByKey("failingProcess");

            //when the job is run several times till the incident creation
            var job = Job;
            while (job.Retries > 0 && ((JobEntity)job).LockOwner == null)
            {
                try
                {
                    LockTheJob(job.Id);
                    _engineRule.ManagementService.ExecuteJob(job.Id);
                }
                catch (System.Exception ex)
                {
                    var tmp = ex;
                    //Excepted Exception
                }
                job = Job;
            }

            //then
            var jobFinalState = (JobEntity)_engineRule.ManagementService.CreateJobQuery(c => c.Id == job.Id).First();
            Assert.AreEqual(JobRetries, jobFinalState.Retries);
            if (JobLocked)
            {
                Assert.NotNull(jobFinalState.LockOwner);
                Assert.NotNull(jobFinalState.LockExpirationTime);
            }
            else
            {
                Assert.IsNull(jobFinalState.LockOwner);
                Assert.IsNull(jobFinalState.LockExpirationTime);
            }

        }

        internal virtual void LockTheJob(string jobId)
        {
            _engineRule.ProcessEngineConfiguration.CommandExecutorTxRequiresNew.Execute(new CommandAnonymousInnerClass(this, jobId));
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly FailedJobListenerWithRetriesTest _outerInstance;

            private readonly string _jobId;

            public CommandAnonymousInnerClass(FailedJobListenerWithRetriesTest outerInstance, string jobId)
            {
                _outerInstance = outerInstance;
                _jobId = jobId;
            }

            public object Execute(CommandContext commandContext)
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final org.Camunda.bpm.Engine.impl.persistence.Entity.JobEntity job = commandContext.GetJobManager().FindJobById(jobId);
                var job = commandContext.JobManager.FindJobById(_jobId);
                job.LockOwner = "someLockOwner";
                job.LockExpirationTime = ClockUtil.CurrentTime.AddHours(1);
                //DateUtils.AddHours(ClockUtil.CurrentTime, 1));
                return null;
            }
        }

        private IJob Job
        {
            get
            {
                var jobs = _engineRule.ManagementService.CreateJobQuery()
                    .ToList();
                Assert.AreEqual(1, jobs.Count);
                return jobs[0];
            }
        }

        private class OleFailedJobCommandFactory : IFailedJobCommandFactory
        {
            private readonly FailedJobListenerWithRetriesTest _outerInstance;


            internal readonly IDictionary<string, OleFoxJobRetryCmd> OleFoxJobRetryCmds = new Dictionary<string, OleFoxJobRetryCmd>();

            public OleFailedJobCommandFactory(FailedJobListenerWithRetriesTest outerInstance)
            {
                _outerInstance = outerInstance;
            }

            public ICommand<object> GetCommand(string jobId, System.Exception exception)
            {
                return GetOleFoxJobRetryCmds(jobId, exception);
            }

            public virtual OleFoxJobRetryCmd GetOleFoxJobRetryCmds(string jobId, System.Exception exception)
            {
                if (!OleFoxJobRetryCmds.ContainsKey(jobId))
                    OleFoxJobRetryCmds[jobId] = new OleFoxJobRetryCmd(_outerInstance, jobId, exception);
                return OleFoxJobRetryCmds[jobId];
            }
        }

        private class OleFoxJobRetryCmd : FoxJobRetryCmd
        {
            private readonly FailedJobListenerWithRetriesTest _outerInstance;


            internal int CountRuns;

            public OleFoxJobRetryCmd(FailedJobListenerWithRetriesTest outerInstance, string jobId, System.Exception exception)
                : base(jobId, exception)
            {
                _outerInstance = outerInstance;
            }

            public override object Execute(CommandContext commandContext)
            {
                var job = Job;
                //on last attempt the incident will be created, we imitate OLE
                if (job.Retries == 1)
                {
                    CountRuns++;
                    if (CountRuns <= _outerInstance.FailedRetriesNumber)
                    {
                        base.Execute(commandContext);
                        throw new OptimisticLockingException("OLE");
                    }
                }
                return base.Execute(commandContext);
            }
        }
    }
}