using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.History
{
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull) ]
    [TestFixture]
    public class HistoricIdentityLinkLogTestByXml : PluggableProcessEngineTestCase
    {
        private static readonly string PROCESS_DEFINITION_KEY_CANDIDATE_USER =
            "oneTaskProcessForHistoricIdentityLinkWithCanidateUser";

        private static readonly string PROCESS_DEFINITION_KEY_CANDIDATE_GROUP =
            "oneTaskProcessForHistoricIdentityLinkWithCanidateGroup";

        private static readonly string PROCESS_DEFINITION_KEY_ASSIGNEE =
            "oneTaskProcessForHistoricIdentityLinkWithAssignee";

        private static readonly string PROCESS_DEFINITION_KEY_CANDIDATE_STARTER_USER =
            "oneTaskProcessForHistoricIdentityLinkWithCanidateStarterUsers";

        private static readonly string PROCESS_DEFINITION_KEY_CANDIDATE_STARTER_GROUP =
            "oneTaskProcessForHistoricIdentityLinkWithCanidateStarterGroups";

        private const string XML_USER = "demo";
        private const string XML_GROUP = "demoGroups";
        private const string XML_ASSIGNEE = "assignee";

        [Test][Deployment(  "resources/api/runtime/OneTaskProcessWithCandidateUser.bpmn20.xml" ) ]
        public virtual void testShouldAddTaskCandidateforAddIdentityLinkUsingXml()
        {
            // Pre test
            var historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 0);

            // given
            startProcessInstance(PROCESS_DEFINITION_KEY_CANDIDATE_USER);
            historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 1);

            // query Test
            var query = historyService.CreateHistoricIdentityLinkLogQuery();
            Assert.AreEqual(query.Where(c=>c.UserId==XML_USER)
                .Count(), 1);
        }

        [Test][Deployment( "resources/api/runtime/OneTaskProcessWithTaskAssignee.bpmn20.xml" ) ]
        public virtual void testShouldAddTaskAssigneeforAddIdentityLinkUsingXml()
        {
            // Pre test
            var historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 0);

            // given
            startProcessInstance(PROCESS_DEFINITION_KEY_ASSIGNEE);
            historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 1);

            // query Test
            var query = historyService.CreateHistoricIdentityLinkLogQuery();
            Assert.AreEqual(query.Where(c=>c.UserId==XML_ASSIGNEE)
                .Count(), 1);
        }
        [Test][Deployment(  "resources/api/runtime/OneTaskProcessWithCandidateGroups.bpmn20.xml" ) ]
        public virtual void testShouldAddTaskCandidateGroupforAddIdentityLinkUsingXml()
        {
            // Pre test
            var historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 0);

            // given
            startProcessInstance(PROCESS_DEFINITION_KEY_CANDIDATE_GROUP);
            historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 1);

            // query Test
            var query = historyService.CreateHistoricIdentityLinkLogQuery();
            Assert.AreEqual(query.Where(c=>c.GroupId==XML_GROUP)
                .Count(), 1);
        }

        [Test][Deployment( "resources/api/runtime/OneTaskProcessWithCandidateStarterUsers.bpmn20.xml" ) ]
        public virtual void testShouldAddProcessCandidateStarterUserforAddIdentityLinkUsingXml()
        {
            // Pre test - Historical identity link is added as part of deployment
            var historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 1);

            // given
            var latestProcessDef = repositoryService.CreateProcessDefinitionQuery(c=>c.Key ==PROCESS_DEFINITION_KEY_CANDIDATE_STARTER_USER)
                .First();
            Assert.NotNull(latestProcessDef);

            var links = repositoryService.GetIdentityLinksForProcessDefinition(latestProcessDef.Id);
            Assert.AreEqual(1, links.Count);

            historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 1);

            // query Test
            var query = historyService.CreateHistoricIdentityLinkLogQuery();
            Assert.AreEqual(query.Where(c=>c.UserId==XML_USER)
                .Count(), 1);
        }

        [Test][Deployment(  "resources/api/runtime/OneTaskProcessWithCandidateStarterGroups.bpmn20.xml" ) ]
        public virtual void testShouldAddProcessCandidateStarterGroupforAddIdentityLinkUsingXml()
        {
            // Pre test - Historical identity link is added as part of deployment
            var historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 1);

            // given
            var latestProcessDef = repositoryService.CreateProcessDefinitionQuery(c=>c.Key ==PROCESS_DEFINITION_KEY_CANDIDATE_STARTER_GROUP)
                .First();
            Assert.NotNull(latestProcessDef);

            var links = repositoryService.GetIdentityLinksForProcessDefinition(latestProcessDef.Id);
            Assert.AreEqual(1, links.Count);

            historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 1);

            // query Test
            var query = historyService.CreateHistoricIdentityLinkLogQuery();
            Assert.AreEqual(query.Where(c=>c.GroupId==XML_GROUP)
                .Count(), 1);
        }

        protected internal virtual IProcessInstance startProcessInstance(string key)
        {
            return runtimeService.StartProcessInstanceByKey(key);
        }
    }
}