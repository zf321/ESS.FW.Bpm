using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Identity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Authorization
{
    [TestFixture]
    public class StartAuthorizationTest : PluggableProcessEngineTestCase
    {

        internal IIdentityService identityService;

        internal IUser userInGroup1;
        internal IUser userInGroup2;
        internal IUser userInGroup3;

        internal IGroup group1;
        internal IGroup group2;
        internal IGroup group3;


        protected internal virtual void setUpUsersAndGroups()
        {

            identityService = ProcessEngine.IdentityService;

            identityService.SaveUser(identityService.NewUser("user1"));
            identityService.SaveUser(identityService.NewUser("user2"));
            identityService.SaveUser(identityService.NewUser("user3"));

            // create users
            userInGroup1 = identityService.NewUser("userInGroup1");
            identityService.SaveUser(userInGroup1);

            userInGroup2 = identityService.NewUser("userInGroup2");
            identityService.SaveUser(userInGroup2);

            userInGroup3 = identityService.NewUser("userInGroup3");
            identityService.SaveUser(userInGroup3);

            // create groups
            group1 = identityService.NewGroup("group1");
            identityService.SaveGroup(group1);

            group2 = identityService.NewGroup("group2");
            identityService.SaveGroup(group2);

            group3 = identityService.NewGroup("group3");
            identityService.SaveGroup(group3);

            // relate users to groups
            identityService.CreateMembership(userInGroup1.Id, group1.Id);
            identityService.CreateMembership(userInGroup2.Id, group2.Id);
            identityService.CreateMembership(userInGroup3.Id, group3.Id);
        }

        protected internal virtual void tearDownUsersAndGroups()
        {
            identityService.DeleteMembership(userInGroup1.Id, group1.Id);
            identityService.DeleteMembership(userInGroup2.Id, group2.Id);
            identityService.DeleteMembership(userInGroup3.Id, group3.Id);

            identityService.DeleteGroup(group1.Id);
            identityService.DeleteGroup(group2.Id);
            identityService.DeleteGroup(group3.Id);

            identityService.DeleteUser(userInGroup1.Id);
            identityService.DeleteUser(userInGroup2.Id);
            identityService.DeleteUser(userInGroup3.Id);

            identityService.DeleteUser("user1");
            identityService.DeleteUser("user2");
            identityService.DeleteUser("user3");
        }

        [Test]
        [Deployment]
        public virtual void testIdentityLinks()
        {

            setUpUsersAndGroups();

            try
            {
                IProcessDefinition latestProcessDef = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "process1").First();
                Assert.NotNull(latestProcessDef);
                IList<IIdentityLink> links = repositoryService.GetIdentityLinksForProcessDefinition(latestProcessDef.Id);
                Assert.AreEqual(0, links.Count);

                latestProcessDef = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "process2").First();
                Assert.NotNull(latestProcessDef);
                links = repositoryService.GetIdentityLinksForProcessDefinition(latestProcessDef.Id);
                Assert.AreEqual(2, links.Count);
                Assert.AreEqual(true, containsUserOrGroup("user1", null, links));
                Assert.AreEqual(true, containsUserOrGroup("user2", null, links));

                latestProcessDef = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "process3").First();
                Assert.NotNull(latestProcessDef);
                links = repositoryService.GetIdentityLinksForProcessDefinition(latestProcessDef.Id);
                Assert.AreEqual(1, links.Count);
                Assert.AreEqual("user1", links[0].UserId);

                latestProcessDef = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "process4").First();
                Assert.NotNull(latestProcessDef);
                links = repositoryService.GetIdentityLinksForProcessDefinition(latestProcessDef.Id);
                Assert.AreEqual(4, links.Count);
                Assert.AreEqual(true, containsUserOrGroup("userInGroup2", null, links));
                Assert.AreEqual(true, containsUserOrGroup(null, "group1", links));
                Assert.AreEqual(true, containsUserOrGroup(null, "group2", links));
                Assert.AreEqual(true, containsUserOrGroup(null, "group3", links));

            }
            finally
            {
                tearDownUsersAndGroups();
            }
        }

        [Test]
        [Deployment]
        public virtual void testAddAndRemoveIdentityLinks()
        {

            setUpUsersAndGroups();

            try
            {
                IProcessDefinition latestProcessDef = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "potentialStarterNoDefinition").First();
                Assert.NotNull(latestProcessDef);
                IList<IIdentityLink> links = repositoryService.GetIdentityLinksForProcessDefinition(latestProcessDef.Id);
                Assert.AreEqual(0, links.Count);

                repositoryService.AddCandidateStarterGroup(latestProcessDef.Id, "group1");
                links = repositoryService.GetIdentityLinksForProcessDefinition(latestProcessDef.Id);
                Assert.AreEqual(1, links.Count);
                Assert.AreEqual("group1", links[0].GroupId);

                repositoryService.AddCandidateStarterUser(latestProcessDef.Id, "user1");
                links = repositoryService.GetIdentityLinksForProcessDefinition(latestProcessDef.Id);
                Assert.AreEqual(2, links.Count);
                Assert.AreEqual(true, containsUserOrGroup(null, "group1", links));
                Assert.AreEqual(true, containsUserOrGroup("user1", null, links));

                repositoryService.DeleteCandidateStarterGroup(latestProcessDef.Id, "nonexisting");
                links = repositoryService.GetIdentityLinksForProcessDefinition(latestProcessDef.Id);
                Assert.AreEqual(2, links.Count);

                repositoryService.DeleteCandidateStarterGroup(latestProcessDef.Id, "group1");
                links = repositoryService.GetIdentityLinksForProcessDefinition(latestProcessDef.Id);
                Assert.AreEqual(1, links.Count);
                Assert.AreEqual("user1", links[0].UserId);

                repositoryService.DeleteCandidateStarterUser(latestProcessDef.Id, "user1");
                links = repositoryService.GetIdentityLinksForProcessDefinition(latestProcessDef.Id);
                Assert.AreEqual(0, links.Count);

            }
            finally
            {
                tearDownUsersAndGroups();
            }
        }

        private bool containsUserOrGroup(string userId, string groupId, IList<IIdentityLink> links)
        {
            bool found = false;
            foreach (IIdentityLink identityLink in links)
            {
                if (!string.ReferenceEquals(userId, null) && userId.Equals(identityLink.UserId))
                {
                    found = true;
                    break;
                }
                else if (!string.ReferenceEquals(groupId, null) && groupId.Equals(identityLink.GroupId))
                {
                    found = true;
                    break;
                }
            }
            return found;
        }

        [Test]
        [Deployment]
        public virtual void testPotentialStarter()
        {
            // first check an unauthorized user. An exception is expected

            setUpUsersAndGroups();

            try
            {

                // Authentication should not be done. So an unidentified IUser should also be able to start the process
                identityService.AuthenticatedUserId = "unauthorizedUser";
                try
                {
                    runtimeService.StartProcessInstanceByKey("potentialStarter");

                }
                catch (System.Exception e)
                {
                    //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
                    Assert.Fail("No StartAuthorizationException expected, " + e.GetType().FullName + " caught.");
                }

                // check with an authorized IUser obviously it should be no problem starting the process
                identityService.AuthenticatedUserId = "user1";
                IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("potentialStarter");
                AssertProcessEnded(processInstance.Id);
                Assert.True(processInstance.IsEnded);
            }
            finally
            {

                tearDownUsersAndGroups();
            }
        }

        [Test]
        [Deployment]
        public virtual void testPotentialStarterNoDefinition()
        {
            identityService = ProcessEngine.IdentityService;

            identityService.AuthenticatedUserId = "someOneFromMars";
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("potentialStarterNoDefinition");
            Assert.NotNull(processInstance.Id);
            AssertProcessEnded(processInstance.Id);
            Assert.True(processInstance.IsEnded);
        }

        [Test]
        [Deployment]
        public virtual void testProcessDefinitionList()
        {

            setUpUsersAndGroups();
            try
            {

                // Process 1 has no potential starters
                IProcessDefinition latestProcessDef = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "process1").First();
                IList<IUser> authorizedUsers = identityService.CreateUserQuery()
                    //.PotentialStarter(latestProcessDef.Id)
                    .ToList();
                Assert.AreEqual(0, authorizedUsers.Count);

                // user1 and user2 are potential Startes of Process2
                latestProcessDef = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "process2").First();
                authorizedUsers = identityService.CreateUserQuery()
                    //.PotentialStarter(latestProcessDef.Id).OrderByUserId()/*.Asc()*/
                    .ToList();
                Assert.AreEqual(2, authorizedUsers.Count);
                Assert.AreEqual("user1", authorizedUsers[0].Id);
                Assert.AreEqual("user2", authorizedUsers[1].Id);

                // Process 2 has no potential starter groups
                latestProcessDef = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "process2").First();
                IList<IGroup> authorizedGroups = identityService.CreateGroupQuery()//.PotentialStarter(latestProcessDef.Id)
                    .ToList();
                Assert.AreEqual(0, authorizedGroups.Count);

                // Process 3 has 3 groups as authorized starter groups
                latestProcessDef = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "process4").First();
                authorizedGroups = identityService.CreateGroupQuery()//.PotentialStarter(latestProcessDef.Id).OrderByGroupId()/*.Asc()*/
                    .ToList();
                Assert.AreEqual(3, authorizedGroups.Count);
                Assert.AreEqual("group1", authorizedGroups[0].Id);
                Assert.AreEqual("group2", authorizedGroups[1].Id);
                Assert.AreEqual("group3", authorizedGroups[2].Id);

                // do not mention user, all processes should be selected
                IList<IProcessDefinition> processDefinitions = repositoryService.CreateProcessDefinitionQuery()/*.OrderByProcessDefinitionName()*//*.Asc()*/.ToList();

                Assert.AreEqual(4, processDefinitions.Count);

                Assert.AreEqual("process1", processDefinitions[0].Key);
                Assert.AreEqual("process2", processDefinitions[1].Key);
                Assert.AreEqual("process3", processDefinitions[2].Key);
                Assert.AreEqual("process4", processDefinitions[3].Key);

                // check user1, process3 has "user1" as only authorized starter, and
                // process2 has two authorized starters, of which one is "user1"
                processDefinitions = repositoryService.CreateProcessDefinitionQuery()/*.OrderByProcessDefinitionName()*//*.Asc()*/.ToList();//.StartableByUser("user2").ToList();("user1").ToList();

                Assert.AreEqual(2, processDefinitions.Count);
                Assert.AreEqual("process2", processDefinitions[0].Key);
                Assert.AreEqual("process3", processDefinitions[1].Key);


                // "user2" can only start process2
                processDefinitions = repositoryService.CreateProcessDefinitionQuery().ToList();//.StartableByUser("user2").ToList();("user2").ToList();

                Assert.AreEqual(1, processDefinitions.Count);
                Assert.AreEqual("process2", processDefinitions[0].Key);

                // no process could be started with "user4"
                processDefinitions = repositoryService.CreateProcessDefinitionQuery().ToList();//.StartableByUser("user2").ToList();("user4").ToList();
                Assert.AreEqual(0, processDefinitions.Count);

                // "userInGroup3" is in "group3" and can start only process4 via group authorization
                processDefinitions = repositoryService.CreateProcessDefinitionQuery().ToList();//.StartableByUser("user2").ToList();("userInGroup3").ToList();
                Assert.AreEqual(1, processDefinitions.Count);
                Assert.AreEqual("process4", processDefinitions[0].Key);

                // "userInGroup2" can start process4, via both IUser and group authorizations
                // but we have to be sure that process4 appears only once
                processDefinitions = repositoryService.CreateProcessDefinitionQuery().ToList();//.StartableByUser("user2").ToList();("userInGroup2").ToList();
                Assert.AreEqual(1, processDefinitions.Count);
                Assert.AreEqual("process4", processDefinitions[0].Key);

            }
            finally
            {
                tearDownUsersAndGroups();
            }
        }

    }

}