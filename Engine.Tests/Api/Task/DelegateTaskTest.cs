using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Engine.Tests.Api.Task
{
    /// <summary>
    /// </summary>
    public class DelegateTaskTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment]
        public virtual void testGetCandidates()
        {
            runtimeService.StartProcessInstanceByKey("testGetCandidates");

            var task = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(task);
            
            var candidateUsers =
                (ISet<string>) taskService.GetVariable(task.Id, DelegateTaskTestTaskListener.VARNAME_CANDIDATE_USERS);
            Assert.AreEqual(2, candidateUsers.Count);
            Assert.True(candidateUsers.Contains("kermit"));
            Assert.True(candidateUsers.Contains("gonzo"));
            
            var candidateGroups =
                (ISet<string>) taskService.GetVariable(task.Id, DelegateTaskTestTaskListener.VARNAME_CANDIDATE_GROUPS);
            Assert.AreEqual(2, candidateGroups.Count);
            Assert.True(candidateGroups.Contains("management"));
            Assert.True(candidateGroups.Contains("accountancy"));
        }
    }
}